using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Mortgage_Plugins
{
    public class MortgageOnCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Extract the tracing service for use in debugging sandboxed plug-ins.
            // If you are not registering the plug-in in the sandbox, then you do
            // not have to add any tracing service related code.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                Entity mortgage = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for
                // web service calls.
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // The mortgage has been created, now we will create the relevant payments

                    // @TODO: Actually get the tax, for now we're in Canada
                    decimal taxRate = 5.0M;

                    QueryExpression aprQuery = new QueryExpression()
                    {
                        EntityName = "mortage_configurations",
                        ColumnSet = new ColumnSet(new string[] { "mortage_value" })
                    };

                    aprQuery.Criteria.AddCondition("mortage_name", ConditionOperator.Equal, "APR");

                    string aprConfig = (string)service.RetrieveMultiple(aprQuery).Entities.First().Attributes["mortage_value"];

                    decimal baseApr;

                    // We default the APR if the parsing fails
                    // DO NOT DO THIS IN PRODUCTION!!!!!
                    if(!Decimal.TryParse(aprConfig, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, new CultureInfo("en-us"), out baseApr ))
                    {
                        baseApr = .3M;
                    }

                    // In case Bunmi made it a decimal value instead of a whole percentage
                    if(baseApr < 1)
                    {
                        baseApr = baseApr * 100;
                    }

                    QueryExpression marginQuery = new QueryExpression()
                    {
                        EntityName = "mortage_configurations",
                        ColumnSet = new ColumnSet(new string[] { "mortage_value" })
                    };

                    marginQuery.Criteria.AddCondition("mortage_name", ConditionOperator.Equal, "Margin");

                    decimal margin = Decimal.Parse((string)service.RetrieveMultiple(marginQuery).Entities.First().Attributes["mortage_value"],
                                     NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
                    
                    Entity contact = service.Retrieve("contact",((EntityReference)mortgage.Attributes["mortage_contactid"]).Id,new ColumnSet(
                        "mortage_riskscore"
                        ));

                    int riskScore = Utilities.Utilities.GetAndUpdateRiskScore(ref contact);

                    if (contact.Attributes.Contains("mortage_riskscore"))
                    {
                        contact.Attributes["mortage_riskscore"] = riskScore;
                    }
                    else
                    {
                        contact.Attributes.Add("mortage_riskscore", riskScore);
                    }

                    // Final APR for Mortgage = (Base APR + Margin) + Log (Risk Score)  + Sales Tax based on State;
                    decimal apr = ((baseApr + margin) + (decimal)Math.Log(riskScore) + taxRate)/100;

                    tracingService.Trace($"Got mortgage info. Term length is {mortgage.Attributes["mortage_termmonths"]}.");

                    if ((int)mortgage.Attributes["mortage_termmonths"] < 1)
                    {
                        throw new InvalidPluginExecutionException("Mortgage term must be at least one month.");
                    }

                    decimal periodicInterest = (apr / 12);

                    // Calculate the monthly payment
                    Money monthlyPayment = new Money(
                        (decimal)
                            (
                            ((double)((((Money)mortgage.Attributes["mortage_amount"]).Value * periodicInterest)) /
                                (
                                    1 - Math.Pow((double)(1 + periodicInterest), -Convert.ToDouble(mortgage.Attributes["mortage_termmonths"]))
                                )
                            )
                            )
                        );

                    tracingService.Trace($"Calculated payment. Payment is {monthlyPayment}");

                    mortgage.Attributes.Add("mortage_minpayment", monthlyPayment);

                    tracingService.Trace("Updated mortgage with payment.");
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in Mortage On Create Plugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("Mortgage Creation: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
