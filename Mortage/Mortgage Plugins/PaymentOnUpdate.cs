using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Mortgage_Plugins
{
    public class PaymentOnUpdate : IPlugin
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
                Entity payment = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for
                // web service calls.
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    if(context.PreEntityImages["due"].Attributes.Contains("mortage_amountdue") && payment.Attributes.Contains("mortage_amountpaid"))
                    {
                        // If the amount paid is greater than the amount due
                        if (((Money)payment.Attributes["mortage_amountpaid"]).Value >= ((Money)context.PreEntityImages["due"].Attributes["mortage_amountdue"]).Value)
                        {
                            payment.Attributes["statecode"] = new OptionSetValue(1);
                            payment.Attributes["statuscode"] = new OptionSetValue(2);
                        }
                    }           
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException($"An error occurred in Mortage On Create Plugin! {ex.Message}");
                }

                catch (Exception ex)
                {
                    tracingService.Trace($"Mortgage Creation: {ex.ToString()}");
                    throw;
                }
            }
        }
    }
}
