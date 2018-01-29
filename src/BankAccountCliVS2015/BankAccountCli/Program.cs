using Fclp;
using System;

namespace BankAccountCli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Read and parse command line arguments.
            FluentCommandLineParser<ApplicationArguments> parser = new FluentCommandLineParser<ApplicationArguments>();

            parser.Setup(arg => arg.AccountName)
                .As('a', "account")
                .Required();
            parser.Setup(arg => arg.IsCredit)
                .As('c', "credit");
            parser.Setup(arg => arg.IsDebit)
                .As('d', "debit");
            parser.Setup(arg => arg.Ammount)
                .As('m', "ammount")
                .Required();
            parser.Setup(arg => arg.Balance)
                .As('b', "balance")
                .Required();

            parser.Parse(args);
            ApplicationArguments arguments = parser.Object;

            // Validate exclusive options.
            if (arguments.IsCredit && arguments.IsDebit)
            {
                throw new ArgumentException("Credit and Debit arguments cannot be passed together.");
            }

            // Run bank account simulation.
            BankAccount ba = new BankAccount(arguments.AccountName, arguments.Balance);

            if (arguments.IsCredit)
            {
                ba.Credit(arguments.Ammount);
            }
            else if (arguments.IsDebit)
            {
                ba.Debit(arguments.Ammount);
            }
            else
            {
                throw new ArgumentException("Invalid account operation.");
            }

            Console.WriteLine("Current balance is ${0}", ba.Balance);

            // Wait key for exit in debug mode.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.ReadKey();
            }
        }
    }
}
