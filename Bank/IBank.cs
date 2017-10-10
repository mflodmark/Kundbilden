namespace Kundbilden
{
    public interface IBank
    {
        void AddNewCustomer(Customer customer);
        void AddNewAccount(Customer customer);
        void DeleteCustomer(Customer customer);
        void DeleteAccount(Account account);

    }
}