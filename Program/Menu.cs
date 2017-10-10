using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Kundbilden.ExtentionsMethods;
using Console = System.Console;

namespace Kundbilden
{
    public sealed class Menu
    {
        public void PrintMenu()
        {
            Console.WriteLine("Huvudmeny");
            Console.WriteLine("0) Avsluta och spara");
            Console.WriteLine("1) Sök kund");
            Console.WriteLine("2) Visa kundbild");
            Console.WriteLine("3) Skapa kund");
            Console.WriteLine("4) Ta bort kund");
            Console.WriteLine("5) Skapa konto");
            Console.WriteLine("6) Ta bort konto");
            Console.WriteLine("7) Insättning");
            Console.WriteLine("8) Uttag");
            Console.WriteLine("9) Överföring");
            Console.WriteLine("10) Se transaktioner");
            Console.WriteLine("11) Lägg på dagens in/utlåningsränta");
            Console.WriteLine("12) Ändra inlåningsränta på konto");
            Console.WriteLine("13) Ändra kredit på konto");
            Console.WriteLine("14) Ändra utlåningsränta på konto");
            Console.WriteLine("100) Rensa sidan och gå tillbaka till main");

        }

        public void GetChoiceAndExecute(Bank bank)
        {
            try
            {
                var file = new FileManagement();
                var account = new Account();
                //var trnMgm = new TransactionManagement();

                while (true)
                {
                    Console.Write("\n> ");
                    if (int.TryParse(Console.ReadLine(), out var nr))
                    {
                        switch (nr)
                        {
                            case 0:
                                file.QuitAndSave(bank);
                                break;
                            case 1:
                                Search(bank);
                                break;
                            case 2:
                                ShowCustomer(bank);
                                break;
                            case 3:
                                AddNewCustomer(bank);
                                break;
                            case 4:
                                DeleteCustomer(bank);
                                break;
                            case 5:
                                AddNewAccount(bank);
                                break;
                            case 6:
                                DeleteAccount(bank);
                                break;
                            case 7:
                                CreateContribution(bank, account);
                                break;
                            case 8:
                                CreateWithdrawl(bank);
                                break;
                            case 9:
                                CreateTransfer(bank);
                                break;
                            case 10:
                                PrintTransactions(bank);
                                break;
                            case 11:
                                account.CalculateDailyInterest(bank);
                                PrintStatistics(bank);
                                break;
                            case 12:
                                AmendInterestRate(bank);
                                break;
                            case 13:
                                AmendCreditLimit(bank);
                                break;
                            case 14:
                                AmendCreditInterestRate(bank);
                                break;
                            case 100:
                                Console.Clear();
                                PrintMenu();
                                break;
                            default:
                                Console.WriteLine("Try with a valid number instead!");
                                continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Try with numbers instead");

                    }
                }
            }
            catch (IOException)
            {
                Output.RedColor("Problem vid läsning eller sparande till fil. Kontakta admin!");
                GetChoiceAndExecute(bank);
            }
            catch (FormatException)
            {
                Output.RedColor("Problem vid konvertering från text till nummer. Försök igen!");
                GetChoiceAndExecute(bank);
            }
            catch (ArgumentNullException)
            {
                Output.RedColor("Input existerar ej i listan. Försök igen!");
                GetChoiceAndExecute(bank);
            }
            catch (Exception)
            {
                Output.RedColor("Någonting gick fel... Försök igen!");
                GetChoiceAndExecute(bank);
            }
        }

        private static bool AskForAccount(Bank bank, out int id)
        {
            id = Input.AskForAccount();
            var check = Input.CheckForAccountWithAccountId(id, bank);

            return check;
        }

        private static void AmendInterestRate(Bank bank)
        {
            Output.WhiteColor("* Ändra inlåningsränta *");
            while (true)
            {
                if (AskForAccount(bank, out var id))
                {
                    var account = bank.GetSingleAccount(id);


                    // Multiply rate with 100
                    var procentBefore = account.InterestRate * 100;

                    Console.WriteLine($"Nuvarande inlåningsränta (årsbasis): {procentBefore.GetProcent()}");

                    // Get new interest rate and update it on the specific account
                    var rate = Input.AskForRate();

                    if (account.ValidateInterestRate(account, rate)) 
                    {
                        account.AmendInterestRate(account, rate);
                        var procentAfter = account.InterestRate * 100;

                        Output.GreenColor($"Ny inlåningsränta: {procentAfter.GetProcent()}");
                    }
                    else
                    {
                        Output.RedColor("\nInlåningsräntan kan inte vara mindre än noll.\n");
                    }



                    break;
                }
                else
                {
                    Output.RedColor("Kontot existerar inte. Försök igen.");
                }
            }
        }

        private static void AmendCreditInterestRate(Bank bank)
        {
            Output.WhiteColor("* Ändra utlåningsränta *");
            while (true)
            {
                if (AskForAccount(bank, out var id))
                {
                    var account = bank.GetSingleAccount(id);


                    // Multiply rate with 100
                    var procentBefore = account.CreditInterestRate * 100;

                    Console.WriteLine($"Nuvarande utlåningsränta (årsbasis): {procentBefore.GetProcent()}");

                    var rate = Input.AskForRate();

                    if (account.validateCreditRate(account, rate))
                    {
                        account.AmendCreditInterestRate(account, rate);
                        var procentAfter = account.CreditInterestRate * 100;

                        Output.GreenColor($"Ny utlåningsränta: {procentAfter.GetProcent()}");
                    }
                    else
                    {
                        Output.RedColor("\nUtlåningsräntan kan inte vara mindre än noll.\n");
                    }


                    //if (account.validateCreditRate())
                    //{
                    //    account.AmendCreditInterestRate(account);
                    //}
                    //else
                    //{
                    //    Output.RedColor("\nUtlåningsräntan kan inte vara mindre än noll.\n");
                    //}
                    break;
                }
                else
                {
                    Output.RedColor("Kontot existerar inte. Försök igen.");
                }
            }
        }

        private static void AmendCreditLimit(Bank bank)
        {
            Output.WhiteColor("* Ändra kreditlimit *");
            while (true)
            {
                if (AskForAccount(bank, out var id))
                {
                    var account = bank.GetSingleAccount(id);
                    account.AmendCreditLimit(account);
                    break;
                }
                else
                {
                    Output.RedColor("Kontot existerar inte. Försök igen.");
                }
            }
        }

        private static void PrintTransactions(Bank bank)
        {
            Output.WhiteColor("* Sök Transaktioner *");

            while (true)
            {
                if (AskForAccount(bank, out var id))
                {
                    var account = bank.GetSingleAccount(id);
                    bank.PrintTransactionsFor(account);
                    break;
                }
                else
                {
                    Output.RedColor("Kontot existerar inte. Försök igen.");
                }
            }

        }

        private static void CreateTransfer(Bank bank)
        {
            Output.WhiteColor("* Överföring *");

            while (true)
            {
                Console.Write($"[Från] ");
                var fromId = Input.AskForAccount();
                var fromCheck = Input.CheckForAccountWithAccountId(fromId, bank);

                if (fromCheck == false)
                {
                    Output.RedColor("[Från] kontot existerar inte. Försök igen.");
                    continue;
                }

                Console.Write("[Till] ");
                var toId = Input.AskForAccount();
                var toCheck = Input.CheckForAccountWithAccountId(toId, bank);

                if (toCheck)
                {
                    // if both from account and to account exists
                    var fromAcc = bank.GetSingleAccount(fromId);
                    var toAcc = bank.GetSingleAccount(toId);
                    var amount = Input.AskForAmount();

                    // Print before change
                    Console.WriteLine($"Saldo på konto ({fromAcc.Id}) innan överföring: {fromAcc.Balance.GetSwedishKr()}");
                    Console.WriteLine($"Saldo på konto ({toAcc.Id}) innan överföring: {toAcc.Balance.GetSwedishKr()}");

                    if (fromAcc.ValidateWithdrawl(fromAcc, amount, bank, TransactionType.Transfer))
                    {
                        fromAcc.CreateTransaction(fromAcc, toAcc, amount, bank, TransactionType.Transfer);
                    }
                    else
                    {
                        Output.RedColor("\nTäckning saknas på kontot\n");

                    }


                    // Print after change
                    Console.WriteLine($"Saldo på konto ({fromAcc.Id}) efter överföring: {fromAcc.Balance.GetSwedishKr()}");
                    Console.WriteLine($"Saldo på konto ({toAcc.Id}) efter överföring: {toAcc.Balance.GetSwedishKr()}");

                    break;
                }
                else
                {
                    Output.RedColor("[Till] kontot existerar inte. Försök igen.");
                }


                PrintStatistics(bank);
            }

        }

        private static void CreateWithdrawl(Bank bank)
        {
            Output.WhiteColor("* Uttag *");

            while (true)
            {
                if (AskForAccount(bank, out var id))
                {
                    var account = bank.GetSingleAccount(id);
                    Console.WriteLine($"Saldo innan uttag: {account.Balance.GetSwedishKr()}");

                    var amount = Input.AskForAmount();

                    if (account.ValidateWithdrawl(account, amount, bank, TransactionType.Withdrawl))
                    {
                        account.CreateTransaction(account, null, amount, bank, TransactionType.Withdrawl);
                    }
                    else
                    {
                        Output.RedColor("\nTäckning saknas på kontot\n");

                    }


                    Console.WriteLine($"Saldo efter uttag: {account.Balance.GetSwedishKr()}");

                    break;
                }
                else
                {
                    Output.RedColor("Kontot existerar inte. Försök igen.");
                }
            }

            PrintStatistics(bank);
        }

        private static void CreateContribution(Bank bank, Account acc)
        {
            Output.WhiteColor("* Insättning *");

            while (true)
            {
                if (AskForAccount(bank, out var id))
                {

                    var account = bank.GetSingleAccount(id);
                    Console.WriteLine($"Saldo innan insättning: {account.Balance.GetSwedishKr()}");

                    var amount = Input.AskForAmount();
                    acc.CreateTransaction(null, account, amount, bank, TransactionType.Contribution);

                    Console.WriteLine($"Saldo efter insättning: {account.Balance.GetSwedishKr()}");
                    break;
                }
                else
                {
                    Output.RedColor("Kontot existerar inte. Försök igen.");
                }
            }
            PrintStatistics(bank);
        }

        private static void DeleteAccount(Bank bank)
        {
            Output.WhiteColor("* Ta bort konto *");

            while (true)
            {
                if (AskForAccount(bank, out var id))
                {
                    var account = bank.GetSingleAccount(id);
                    bank.DeleteAccount(account);
                    break;
                }
                else
                {
                    Output.RedColor("Kunden existerar inte. Försök igen.");
                }
            }
            PrintStatistics(bank);
        }

        private static void AddNewAccount(Bank bank)
        {
            Output.WhiteColor("* Lägg till konto *");

            while (true)
            {
                var id = Input.AskForCustomer();
                var check = Input.CheckForCustomerWithCustomerId(id, bank);

                if (check)
                {
                    var customer = bank.GetSingleCustomer(id);
                    bank.AddNewAccount(customer);

                    Console.WriteLine($"Nytt kontonummer: {customer.Accounts.Last().Id}");
                    break;
                }
                else
                {
                    Output.RedColor("Kunden existerar inte. Försök igen.");
                }
            }
            PrintStatistics(bank);
        }

        private static void DeleteCustomer(Bank bank)
        {
            Output.WhiteColor("* Ta bort kund *");

            while (true)
            {
                var id = Input.AskForCustomer();
                var check = Input.CheckForCustomerWithCustomerId(id, bank);

                if (check)
                {
                    var customer = bank.GetSingleCustomer(id);
                    bank.DeleteCustomer(customer);
                    break;
                }
                else
                {
                    Output.RedColor("Kunden existerar inte. Försök igen.");
                }
            }
            PrintStatistics(bank);
        }

        private static void AddNewCustomer(Bank bank)
        {
            var customer = Input.AskForCustomerInfo();
            bank.AddNewCustomer(customer);
            PrintStatistics(bank);
        }

        private static void Search(Bank bank)
        {
            Output.WhiteColor("* Sök Kund *");
            var nameOrCity = Input.AskFor("Namn eller postort");

            var result = bank.GetCustomers(nameOrCity);

            foreach (var item in result)
            {
                Console.WriteLine($"{item.Id}: {item.Name}");
            }
        }

        private static void ShowCustomer(Bank bank)
        {
            Output.WhiteColor("* Visa kundbild *");

            while (true)
            {
                var id = Input.AskForAccountIdOrCustomerId();
                var check = Input.CheckForCustomerWithCustomerIdOrAccountId(id, bank, out var customerId);

                if (check)
                {
                    var customer = bank.GetSingleCustomer(customerId);
                    bank.ShowCustomer(customer);
                    break;
                }
                else
                {
                    Output.RedColor("Kundnumret eller kontot existerar inte. Försök igen.");
                }
            }

        }

        private static void PrintStatistics(Bank bank)
        {
            Output.WhiteColor("Statistik");
            Console.WriteLine($"Antal kunder: {bank.ListOfCustomers.Count}");
            Console.WriteLine($"Antal konton: {bank.ListOfAccounts.Count}");
            Console.WriteLine($"Totalt saldo: {bank.ListOfAccounts.Select(x => x.Balance).Sum().GetSwedishKr()}");
            Console.WriteLine($"Antal transaktioner: {bank.ListOfTransactions.Count()}");

        }


    }
}