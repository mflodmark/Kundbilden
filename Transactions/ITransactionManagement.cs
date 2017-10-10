namespace Kundbilden
{
    interface ITransactionManagement
    {
        void CreateTransaction(Account fromAcc, Account toAcc, decimal amount, Bank bank, TransactionType type);
    }
}