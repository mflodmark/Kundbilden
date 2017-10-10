using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using Kundbilden.ExtentionsMethods;
using System.Collections.Generic;

namespace Kundbilden
{
    public class Account: BasicInfo, IAccounts
    {
        private decimal _balance;
        private decimal _interestRate;
        private decimal _creditInterestRate;
        public List<Transaction> Transactions = new List<Transaction>();


        //public AccountType Type { get; set; }
        //public int Id { get; set; }
        public int CustomerId { get; set; }

        public decimal Balance
        {
            get => _balance;
            private set
            {
                if (value < 0 && CreditLimit == 0)
                {
                    // Don't update
                    //Output.RedColor("\nTäckning saknas på kontot\n");
                }
                else if (value < -CreditLimit && CreditLimit > 0)
                {
                    // Don't update
                    //Output.RedColor("\nBalansen kan inte vara mindre än kreditgränsen på detta konto.\n");
                }
                else
                {
                    _balance = value;
                }
            }
        }

        public decimal InterestRate
        {
            get => _interestRate;
            set
            {
                if (value < 0)
                {
                    //Output.RedColor("\nInlåningsräntan kan inte vara mindre än noll.\n");
                }
                else
                {
                    _interestRate = value;
                }
            }
        }

        public decimal CreditLimit { get; set; }

        public decimal CreditInterestRate
        {
            get => _creditInterestRate;
            set
            {
                if (value < 0)
                {
                    //Output.RedColor("\nUtlåningsräntan kan inte vara mindre än noll.\n");
                }
                else
                {
                    _creditInterestRate = value;
                }
            }
        }

        public Account()
        {
            this.Balance = 0;
            this.InterestRate = 0;
            this.CreditLimit = 0;
            this.CreditInterestRate = 0;
        }

        public Account(Bank bank) : this()
        {
            Id = CreateUniqueId(bank);
        }

        public Account(decimal balance)
        {
            this.Balance = balance;
        }

        public Account(int credit, decimal balance)
        {
            this.CreditLimit = credit;
            this.Balance = balance;
        }

        public Account(Bank bank, int id, int customerId, decimal balance)
        {
            bank.ListOfAccounts.Add(new Account()
            {
                Id = id,
                CustomerId = customerId,
                Balance = balance
            });
        }

        public bool ValidateWithdrawl(Account fromAcc, decimal amount, Bank bank, TransactionType type)  
        {
            var before = fromAcc.Balance;
            fromAcc.Balance -= amount;
            var after = fromAcc.Balance;

            if (before != after)
            {
                fromAcc.Balance += amount;
                return true;
            }

            return false;
        }

        public bool ValidateInterestRate(Account account, decimal rate)
        {
            account.InterestRate = rate;
            if (account.InterestRate == 0) return false;

            return true;
        }

        public bool validateCreditRate(Account account, decimal rate)
        {
            account.CreditInterestRate = rate;
            if (account.CreditInterestRate == 0) return false;

            return true;
        }

        public void CreateTransaction(Account fromAcc, Account toAcc, decimal amount, Bank bank, TransactionType type)
        {


            var fileManagement = new FileManagement();

            if (type == TransactionType.Withdrawl)
            {
                var before = fromAcc.Balance;
                fromAcc.Balance -= amount;
                var after = fromAcc.Balance;


                var transaction = new Transaction(bank)
                {
                    FromAccountId = fromAcc.Id,
                    Amount = amount,
                    //ToAccountId = toAcc.AccountId,
                    FromAccountBalance = fromAcc.Balance,
                    Type = TransactionType.Withdrawl

                };

                if (before != after)
                {
                    bank.ListOfTransactions.Add(transaction);
                    fromAcc.Transactions.Add(transaction);

                    fileManagement.CreateTransactionsFile(bank);
                }

            }

            if (type == TransactionType.Contribution)
            {
                var before = toAcc.Balance;
                toAcc.Balance += amount;
                var after = toAcc.Balance;

                var transaction = new Transaction(bank)
                {
                    //FromAccountId = fromAcc.AccountId,
                    Amount = amount,
                    ToAccountId = toAcc.Id,
                    ToAccountBalance = toAcc.Balance,
                    Type = TransactionType.Contribution

                };

                if (before != after)
                {
                    bank.ListOfTransactions.Add(transaction);
                    toAcc.Transactions.Add(transaction);

                    fileManagement.CreateTransactionsFile(bank);
                }
            }

            if (type == TransactionType.Transfer)
            {
                var before = fromAcc.Balance;
                fromAcc.Balance -= amount;
                var after = fromAcc.Balance;

                if (before != after)
                {
                    toAcc.Balance += amount;


                    var transaction = new Transaction(bank)
                    {
                        FromAccountId = fromAcc.Id,
                        Amount = amount,
                        ToAccountId = toAcc.Id,
                        FromAccountBalance = fromAcc.Balance,
                        ToAccountBalance = toAcc.Balance,
                        Type = TransactionType.Transfer

                    };


                    bank.ListOfTransactions.Add(transaction);
                    fromAcc.Transactions.Add(transaction);
                    toAcc.Transactions.Add(transaction);

                    fileManagement.CreateTransactionsFile(bank);
                }

            }
        }


        public override int CreateUniqueId(Bank bank)
        { 
            var count = bank.ListOfAccounts.Count();
            if (count == 0) return 13001;

            var sortIdDesc = bank.ListOfAccounts.OrderByDescending(x => x.Id);

            return sortIdDesc.First().Id + 1;
        }

        public void CalculateDailyInterest(Bank bank)
        {
            int counter = 0;
            foreach (var item in bank.ListOfAccounts)
            {
                // Interest rate when account is greater than zero
                if (item._interestRate > 0 && item.Balance > 0)
                {
                    counter++;
                    decimal interest = 0;  

                    // Check for leap year
                    if (DateTime.IsLeapYear(DateTime.Now.Year))
                    {
                        interest = item.Balance * item.InterestRate / 366;
                        item.Balance += interest;
                    }
                    else
                    {
                        interest = item.Balance * item.InterestRate / 365;
                        item.Balance += interest;
                    }

                    // Add transaction
                    var trn = new Transaction(bank)
                    {
                        ToAccountId = item.Id,
                        Amount = interest,
                        FromAccountBalance = item.Balance,
                        Type = TransactionType.Income
                    };

                    bank.ListOfTransactions.Add(trn);
                    item.Transactions.Add(trn);
                }

                // Debit interest rate for accounts that are using the credit on the account
                if (item.CreditLimit > 0 && item.CreditInterestRate > 0 && item.Balance < 0)
                {
                    counter++;
                    var interest = 0M;

                    // Check for leap year
                    if (DateTime.IsLeapYear(DateTime.Now.Year))
                    {
                        interest = -item.Balance * item.CreditInterestRate / 366;
                        
                        // Set balance without the set method stopping it
                        item._balance -= interest;
                    }
                    else
                    {
                        interest = -item.Balance * item.CreditInterestRate / 365;
                        
                        // Set balance without the set method stopping it
                        item._balance -= interest;
                    }

                    // Add transaction
                    var trn = new Transaction(bank)
                    {
                        FromAccountId = item.Id,
                        Amount = interest,
                        FromAccountBalance = item.Balance,
                        Type = TransactionType.Expense
                    };

                    bank.ListOfTransactions.Add(trn);
                    item.Transactions.Add(trn);

                }
            }

            Console.WriteLine($"Antal konton som fått in/utlåningsränta idag: {counter}");

            // Save file
            var fileManagement = new FileManagement();
            if (counter > 0) fileManagement.CreateTransactionsFile(bank);
        }

        public void AmendInterestRate(Account account, decimal rate)
        {

            account.InterestRate = rate;

            //// Multiply rate with 100
            //var procentBefore = account.InterestRate * 100;

            //Console.WriteLine($"Nuvarande inlåningsränta (årsbasis): {procentBefore.GetProcent()}");

            //// Get new interest rate and update it on the specific account
            //account.InterestRate = Input.AskForRate();
            //var procentAfter = account.InterestRate * 100;

            //Output.GreenColor($"Ny inlåningsränta: {procentAfter.GetProcent()}");
            
        }

        public void AmendCreditInterestRate(Account account, decimal rate)
        {
            account.CreditInterestRate = rate;

            //// Multiply rate with 100
            //var procentBefore = account.CreditInterestRate * 100;

            //Console.WriteLine($"Nuvarande utlåningsränta (årsbasis): {procentBefore.GetProcent()}");

            //// Ask for new rate
            //account.CreditInterestRate = Input.AskForRate();
            //var procentAfter = account.CreditInterestRate * 100;

            //Output.GreenColor("Utlåningsräntan har uppdaterats.");
            //Console.WriteLine($"Ny utlåningsränta: {procentAfter.GetProcent()}");
        }

        public void AmendCreditLimit(Account account)
        {
            Console.WriteLine($"Nuvarande kreditlimit: {account.CreditLimit}");

            // Ask for new limit
            account.CreditLimit = Input.AskForAmount();

            Output.GreenColor("Kreditlimit har uppdaterats.");
            Console.WriteLine($"Ny kreditlimit: {account.CreditLimit}");

        }



        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public enum AccountType
    {
        CreditAccount,
        Account
    }
}