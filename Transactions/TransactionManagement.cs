using System;
using System.Linq;

//namespace Kundbilden
//{
    //public sealed class TransactionManagement: ITransactionManagement
    //{
    //    public void CreateTransaction(Account fromAcc, Account toAcc, decimal amount, Bank bank, TransactionType type)
    //    {

//        var fileManagement = new FileManagement();

//        if (type == TransactionType.Withdrawl)
//        {
//            var before = fromAcc.Balance;
//            fromAcc.Balance -= amount;
//            var after = fromAcc.Balance;


//            var transaction = new Transaction(bank)
//            {
//                FromAccountId = fromAcc.Id,
//                Amount = amount,
//                //ToAccountId = toAcc.AccountId,
//                FromAccountBalance = fromAcc.Balance,
//                Type = TransactionType.Withdrawl

//            };

//            if (before != after)
//            { 
//                bank.ListOfTransactions.Add(transaction);
//                fromAcc.Transactions.Add(transaction);

//                fileManagement.CreateTransactionsFile(bank);
//            }

//        }

//        if (type == TransactionType.Contribution)
//        {
//            var before = toAcc.Balance;
//            toAcc.Balance += amount;
//            var after = toAcc.Balance;

//            var transaction = new Transaction(bank)
//            {
//                //FromAccountId = fromAcc.AccountId,
//                Amount = amount,
//                ToAccountId = toAcc.Id,
//                ToAccountBalance = toAcc.Balance,
//                Type = TransactionType.Contribution

//            };

//            if (before != after)
//            {
//                bank.ListOfTransactions.Add(transaction);
//                toAcc.Transactions.Add(transaction);

//                fileManagement.CreateTransactionsFile(bank);
//            }
//        }

//        if (type == TransactionType.Transfer)
//        {
//            var before = fromAcc.Balance;
//            fromAcc.Balance -= amount;
//            toAcc.Balance += amount;
//            var after = fromAcc.Balance;

//            var transaction = new Transaction(bank)
//            {
//                FromAccountId = fromAcc.Id,
//                Amount = amount,
//                ToAccountId = toAcc.Id,
//                FromAccountBalance = fromAcc.Balance,
//                ToAccountBalance = toAcc.Balance,
//                Type = TransactionType.Transfer

//            };

//            if (before != after)
//            {
//                bank.ListOfTransactions.Add(transaction);
//                fromAcc.Transactions.Add(transaction);
//                toAcc.Transactions.Add(transaction);

//                fileManagement.CreateTransactionsFile(bank);
//            }        

//        }
//    }
//    }
//}