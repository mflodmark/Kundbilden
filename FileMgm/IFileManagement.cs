namespace Kundbilden
{
    interface IFileManagement
    {
        void CreateTransactionsFile(Bank bank);
        void GetFiles(Bank bank);
        void QuitAndSave(Bank bank);
    }
}