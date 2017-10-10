using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Kundbilden.ExtentionsMethods;

namespace Kundbilden
{

    public sealed class Bank: IBank
    {
        public List<Customer> ListOfCustomers { get; set; } = new List<Customer>();
        public List<Account> ListOfAccounts { get; set; } = new List<Account>();
        public List<Transaction> ListOfTransactions { get; set; } = new List<Transaction>();

        public void ShowCustomer(Customer customer)
        {
    
            // Query for sum of accounts
            var sumOfAccounts = customer.Accounts.Select(x => x.Balance).Sum();

            // Customer
            Console.WriteLine($"\nCustomer Id: {customer.Id}");
            Console.WriteLine($"Name: {customer.Name}");
            Console.WriteLine($"Adress: {customer.Adress}");
            Console.WriteLine($"Zip Code: {customer.ZipCode}");
            Console.WriteLine($"City: {customer.City}");
            Console.WriteLine($"Country: {customer.Country}");
            Console.WriteLine($"Region: {customer.Region}");
            Console.WriteLine($"Organistionsnummer: {customer.OrganisationNumber}");
            Console.WriteLine($"Telefon: {customer.Telephone}");

            // Accounts
            Console.WriteLine($"\nKonto information:");

            foreach (var item in customer.Accounts)
            {
                Console.WriteLine(
                    $"Account: {item.Id}, Balance: {item.Balance.GetSwedishKr()}, Interest Rate: {item.InterestRate.GetProcent()}, " +
                    $"Credit: {item.CreditLimit.GetSwedishKr()}, Credit debit rate: {item.CreditInterestRate.GetProcent()}");
            }

            Console.WriteLine($"Totalt saldo: {sumOfAccounts.GetSwedishKr()}");

            // Transactions
            Console.WriteLine("\nTransaktionshistorik:");
            PrintTransactionsFor(customer);
        }

        public void PrintTransactionsFor(Account account)
        {
            var transactions = account.Transactions;

            PrintTranscations(transactions);

        }

        private void PrintTranscations(IEnumerable<Transaction> transactions)
        {
            foreach (var item in transactions)
            {
                //Check if item is of type transfer
                if (item.Type == TransactionType.Transfer)
                {
                    Console.WriteLine($"TrnId: {item.Id}, Date: {item.Date}, " +
                                      $"Amount: {item.Amount.GetSwedishKr()}, " +
                                      $"From account: {item.FromAccountId}, " +
                                      $"Balance: {item.FromAccountBalance.GetSwedishKr()}, " +
                                      $"To account: {item.ToAccountId}, " +
                                      $"Balance: {item.ToAccountBalance.GetSwedishKr()}, " +
                                      $"Typ: {item.Type.ToString()}");
                }
                else if (item.Type == TransactionType.Income || item.Type == TransactionType.Contribution)
                {
                    Console.WriteLine($"TrnId: {item.Id}, Date: {item.Date}, " +
                                      $"Amount: {item.Amount.GetSwedishKr()}, " +
                                      $"To account: {item.ToAccountId}, " +
                                      $"Saldo: {item.ToAccountBalance.GetSwedishKr()}, " +
                                      $"Typ: {item.Type.ToString()}");

                }
                else if (item.Type == TransactionType.Expense || item.Type == TransactionType.Withdrawl)
                {
                    Console.WriteLine($"TrnId: {item.Id}, Date: {item.Date}, " +
                                      $"Amount: {item.Amount.GetSwedishKr()}, " +
                                      $"From account: {item.FromAccountId}, " +
                                      $"Saldo: {item.FromAccountBalance.GetSwedishKr()}, " + 
                                      $"Typ: {item.Type.ToString()}");

                }
            }

            if (transactions.Count() == 0) Console.WriteLine("Inga transaktioner finns på detta konto");

        }

        public void PrintTransactionsFor(Customer customer)
        {
            var transactions = customer.Accounts.SelectMany(x => x.Transactions);

            PrintTranscations(transactions);
           
        }

        public void AddNewCustomer(Customer customer)
        {
            ListOfCustomers.Add(new Customer(this)
            {
                Name = customer.Name,
                Adress = customer.Adress,
                City = customer.City,
                Country = customer.Country,
                ZipCode = customer.ZipCode,
                Region = customer.Region,
                OrganisationNumber = customer.OrganisationNumber,
                Telephone = customer.Telephone,
            });

            // Add account to new customer
            AddNewAccount(ListOfCustomers.Last());

            Console.WriteLine($"Ny kund skapad med kundnummer: {ListOfCustomers.Last().Id}");
            Console.WriteLine($"Nytt kontonummer: {ListOfAccounts.Last().Id}");


        }

        public void AddNewAccount(Customer customer)
        {
            var acc = new Account(this) {
                CustomerId = customer.Id
            };

            ListOfAccounts.Add(acc);
            customer.Accounts.Add(acc);
        }

        public void DeleteCustomer(Customer customer)
        {
            var checkSumOfAccounts =
                ListOfAccounts.Where(x => x.CustomerId == customer.Id).Select(y => y.Balance).Sum();

            if (checkSumOfAccounts == 0)
            {
                ListOfAccounts.Remove(customer.Accounts.FirstOrDefault());
                ListOfCustomers.Remove(customer);
                Output.GreenColor($"Borttaget kundnummer: {customer.Id}");
            }
            else
            {
                Output.RedColor("Det går inte att ta bort en kund som har saldo större en noll på något av sina konton.");
            }
        }

        public void DeleteAccount(Account account)
        {
            // Check sum of accounts
            var checkSumOfAccounts =
                ListOfAccounts.Where(x => x.Id == account.Id).Select(y => y.Balance).Sum();

            // Count accounts
            var countAccounts = ListOfAccounts.Count(x => x.CustomerId == account.CustomerId);
            var customer = ListOfCustomers.FirstOrDefault(x => x.Id == account.CustomerId);

            // Can not delete account if sum is not zero or there is only one account 
            // (customer always need an account)
            if (checkSumOfAccounts == 0 && countAccounts > 1)
            {
                ListOfAccounts.Remove(account);
                customer.Accounts.Remove(account);
                Output.GreenColor($"Borttaget kontonummer: {account.Id}");
            }
            else
            {
                Output.RedColor("Kan ej ta bort kontot om:\n- det finns saldo\n- det är kundens enda konto");
            }

        }

        public Account GetSingleAccount(int id)
        {

            var selected = ListOfAccounts.SingleOrDefault(x => x.Id == id);

            return selected;
        }


        public Customer GetSingleCustomer(int id)
        {
            var selected = ListOfCustomers.SingleOrDefault(x => x.Id == id);

            return selected;
        }

        public IEnumerable<Customer> GetCustomers(string nameOrCity)
        {

            var result = ListOfCustomers.Where(x =>
                x.Name.ToLower().Contains(nameOrCity.ToLower()) ||
                x.City.ToLower() == nameOrCity.ToLower());

            return result;

        }
    }
}