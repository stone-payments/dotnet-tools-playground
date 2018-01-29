using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccountCli
{
    public class ApplicationArguments
    {
        public double Ammount { get; set; }
        public bool IsCredit { get; set; }
        public bool IsDebit { get; set; }
        public string AccountName { get; set; }
        public double Balance { get; set; }
    }
}
