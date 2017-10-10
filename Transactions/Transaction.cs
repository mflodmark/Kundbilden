using System;
using System.Linq;
using Kundbilden.ExtentionsMethods;

namespace Kundbilden
{
    public class Transaction: BasicInfo
    {
        private decimal _amount;

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (value > 0)
                {
                    _amount = value;
                }
                else
                {
                    //Output.RedColor("Beloppet måste vara positivt. Försök igen!");
                }
            }
        }

        //public int Id { get; set; }
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public decimal FromAccountBalance { get; set; }
        public decimal ToAccountBalance { get; set; }

        public Transaction()
        {
            Date = DateTime.Now;           
        }

        public Transaction(Bank bank): this()
        {
            Id = CreateUniqueId(bank);
        }

        public override int CreateUniqueId(Bank bank)
        {
            var count = bank.ListOfTransactions.Count();
            if (count == 0) return 101;

            var sortIdDesc = bank.ListOfTransactions.OrderByDescending(x => x.Id);
            
            return sortIdDesc.First().Id + 1;
        }


    }



    public enum TransactionType
    {
        Contribution,
        Withdrawl,
        Transfer,
        Income,
        Expense
    }
}