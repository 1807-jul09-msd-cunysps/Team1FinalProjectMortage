using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;

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
                    decimal taxRate = 0.05M;

                    // @TODO: Actually get the APR
                    // Final APR for Mortgage = (Base APR + Margin) + Log (Risk Score)  + Sales Tax based on State;
                    decimal apr = 0.1M + taxRate;

                    // Get the generated number
                    string mortgageNumber = mortgage.Attributes["mortage_name"].ToString();
                    // Get the creating user
                    EntityReference creatorId = (EntityReference)mortgage.Attributes["ownerid"];
                    // Get total payment amount
                    Money totalAmount = (Money)mortgage.Attributes["mortage_amount"];

                    // Get the length of the mortgage
                    int mortageTerm = (int)mortgage.Attributes["mortage_termmonths"];

                    tracingService.Trace($"Got mortgage info. Term length is {mortageTerm}.");

                    if(mortageTerm < 1)
                    {
                        throw new InvalidPluginExecutionException("Mortgage term must be at least one month.");
                    }

                    decimal periodicInterest = (apr / 12);

                    // Calculate the monthly payment
                    Money monthlyPayment = new Money(
                        (decimal)
                            (
                            (double)(totalAmount.Value * periodicInterest) /
                                (
                                    1 - Math.Pow((double)(1 + periodicInterest), -(double)mortageTerm)
                                )
                            )
                        );

                    tracingService.Trace($"Calculated payment. Payment is {monthlyPayment}");

                    if (mortgage.Attributes.Contains("mortage_minpayment"))
                    {
                        mortgage.Attributes["mortage_minpayment"] = monthlyPayment;
                    }
                    else
                    {
                        mortgage.Attributes.Add("mortage_minpayment", monthlyPayment);
                    }
                    

                    service.Update(mortgage);

                    tracingService.Trace("Updated mortgage with payment.");

                    EntityReferenceCollection payments = new EntityReferenceCollection();

                    for (int i = 0; i < mortageTerm; i++)
                    {
                        Entity payment = new Entity("mortage_payment");

                        string paymentNumber = Convert.ToString(i + 1).PadLeft(3, '0');

                        // Make sure there aren't too many payments
                        if(paymentNumber.Length > 3)
                        {
                            throw new InvalidPluginExecutionException("Too many payments for this mortgage. Must not exceed 999.");
                        }

                        payment.Attributes.Add("mortage_name", $"Payment {paymentNumber} - {mortgageNumber}");

                        payment.Attributes.Add("mortage_amountdue", monthlyPayment);
                        payment.Attributes.Add("mortage_amountpaid", 0.0M);
                        payment.Attributes.Add("ownerid", creatorId);

                        // Create the payment but maintain a reference to it
                        payments.Add(new EntityReference("mortage_payment", service.Create(payment)));

                        tracingService.Trace($"Created payment {i + 1}.");
                    }

                    // Associate all the payments with the mortgage
                    Relationship mortgageRelationship = new Relationship("mortage_mortgage_payment");
                    service.Associate("mortage_mortgage", context.PrimaryEntityId, mortgageRelationship, payments);
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in Mortgage On Create Plugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("Mortage On Create: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
