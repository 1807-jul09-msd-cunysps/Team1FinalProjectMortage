using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;

namespace Console_Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            decimal periodicInterest = (0.09M / 12);
            int mortgageTerm = 144;
            Money mortgageAmount = new Money(100000);

            // Calculate the monthly payment

            // Amount with interest
            var amountWithInterest = 100000.0M * periodicInterest;

            Money monthlyPayment = new Money(
                        (decimal)
                            (
                            ((double)((mortgageAmount.Value * periodicInterest)) /
                                (
                                    1 - Math.Pow((double)(1 + periodicInterest), -Convert.ToDouble(mortgageTerm))
                                )
                            )
                            )
                        );

            Console.WriteLine(monthlyPayment.Value);
            Console.Read();
        }
    }
}
