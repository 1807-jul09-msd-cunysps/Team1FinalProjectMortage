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
using System.ServiceModel.Activities;
using System.Activities;
using System.Activities.Statements;
using Microsoft.Xrm.Sdk.Workflow;

namespace Mortgage_Plugins
{
    public class CreateMortgagePayments : CodeActivity
    {
        [RequiredArgument]
        [Input("Mortgage")]
        [ReferenceTarget("mortage_mortgage")]
        public InArgument<EntityReference> MortgageReference { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                Entity mortgage = service.Retrieve("mortage_mortgage", MortgageReference.Get(executionContext).Id, new ColumnSet
                (
                    "mortage_name",
                    "mortage_termmonths",
                    "mortage_minpayment",
                    "ownerid"
                ));

                EntityReferenceCollection payments = new EntityReferenceCollection();

                for (int i = 0; i < (int)mortgage.Attributes["mortage_termmonths"]; i++)
                {
                    Entity payment = new Entity("mortage_payment");

                    string paymentNumber = Convert.ToString(i + 1).PadLeft(3, '0');

                    // Make sure there aren't too many payments
                    if (paymentNumber.Length > 3)
                    {
                        throw new FaultException("Too many payments for this mortgage. Must not exceed 999.");
                    }

                    payment.Attributes.Add("mortage_name", $"Payment {paymentNumber} - {mortgage.Attributes["mortage_name"]}");

                    payment.Attributes.Add("mortage_amountdue", mortgage.Attributes["mortage_minpayment"]);
                    payment.Attributes.Add("mortage_amountpaid", 0.0M);
                    payment.Attributes.Add("ownerid", mortgage.Attributes["ownerid"]);

                    // Create the payment but maintain a reference to it
                    payments.Add(new EntityReference("mortage_payment", service.Create(payment)));

                    tracingService.Trace($"Created payment {i + 1}.");
                }

                // Associate all the payments with the mortgage
                Relationship mortgageRelationship = new Relationship("mortage_mortgage_payment");
                service.Associate("mortage_mortgage", mortgage.Id, mortgageRelationship, payments);
            }
            
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in Payment Creation Workflow.{ex.StackTrace}{ex.Message}", ex);
            }

            catch (Exception ex)
            {
                tracingService.Trace("Payment Creation: {0}", ex.ToString());
                throw;
            }
        }
    }
}
