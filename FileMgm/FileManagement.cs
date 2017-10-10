using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Kundbilden.ExtentionsMethods;

namespace Kundbilden
{
    public sealed class FileManagement: IFileManagement
    {
        private const string Path = @"C:\Users\marku\Documents\Visual Studio 2017\Projects\PROG17\ProgrammeringC#\Kundbilden\FileArchive\";
        private const string CustomerAndAccountFile = @"bankdata.txt";
        private const string TranFile = "transaktioner.txt";
        private int _customerAndAccountFileCounter = 0;
        private int _tranFileCounter = 0;

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        public void PrintCurrentDirectory()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine("\n");
        }

        public void GetFiles(Bank bank)
        {
            // Get customer and accounts
            try
            {
                // Get customer
                try
                {
                    GetCustomersAndAccounts(bank);
                }
                catch (FormatException)
                {
                    // Tar hand om övriga fel
                    Console.WriteLine($"Försöker hämta kunder från {CustomerAndAccountFile}, men formatering misslyckades.\n" +
                                      $"Fel uppkom på rad {_customerAndAccountFileCounter}");
                    throw;
                }
            }
            catch (IOException)
            {
                // Tar hand om fel vid inläsning
                Output.RedColor($"Kunde inte läsa in {CustomerAndAccountFile}...");
                Console.WriteLine($"Se till att filen finns i:{Path}");
                throw;
            }

            
            AddAccountsToCustomer(bank);

            // Get transactions
            //try
            //{
            //    GetTransactions(bank);
            //}
            //catch (IOException)
            //{
            //    // Tar hand om fel vid inläsning
            //    Output.RedColor($"Kunde inte läsa in {TranFile}...");
            //    Console.WriteLine($"Se till att filen finns i:{Path}");
            //    throw;
            //}
            //catch (FormatException)
            //{
            //    // Tar hand om övriga fel
            //    Console.WriteLine($"Försöker hämta transaktioner från {TranFile}, men formatering misslyckades.\n" +
            //                      $"Fel uppkom på rad {_tranFileCounter}");
            //    throw;
            //}
        }

        private static void AddAccountsToCustomer(Bank bank)
        {
            foreach (var customer in bank.ListOfCustomers)
            {
                if (bank.ListOfAccounts.Any(x => x.CustomerId == customer.Id))
                {
                    customer.Accounts = bank.ListOfAccounts.Where(x => x.CustomerId == customer.Id).ToList();
                }
            }
        }


        private void GetCustomersAndAccounts(Bank bank)
        {
            using (var reader = new StreamReader(Path + CustomerAndAccountFile))
            {
                _customerAndAccountFileCounter = 0;
                var startingLineOfAccounts = 0;

                var textfromfile = reader.ReadLine();

                while (textfromfile != null)
                {
                    _customerAndAccountFileCounter++;

                    var columns = textfromfile.Split(';');

                    // Check next row
                    textfromfile = reader.ReadLine();

                    // Check first line add att the value to lines of customers
                    if (_customerAndAccountFileCounter == 1)
                    {
                        var linesOfCustomers = int.Parse(columns[0]);
                        startingLineOfAccounts = linesOfCustomers + 2;
                        continue;
                    }


                    /*
                     Varje kund står på en rad. Semikolon separerar värdena åt.
                        1015                   - Kundnummer Column[0]
                        552060-3431            - Organisationsnummer Column[1]
                        Comércio Mineiro       - Företagsnamn Column[2]
                        Av. dos Lusíadas, 23   - Adress Column[3]
                        São Paulo              - Stad Column[4]
                        SP                     - Region Column[5]
                        05432-043              - Postnummer Column[6]
                        Brazil                 - Land Column[7]
                        (11) 555-7647          - Telefonnummer Column[8]
                    */

                    // Check customers
                    if (_customerAndAccountFileCounter < startingLineOfAccounts)
                    {
                        bank.ListOfCustomers.Add(new Customer()
                        {
                            Id = int.Parse(columns[0]),
                            OrganisationNumber = columns[1],
                            Name = columns[2],
                            Adress = columns[3],
                            City = columns[4],
                            Region = columns[5],
                            ZipCode = columns[6],
                            Country = columns[7],
                            Telephone = columns[8]
                        });                          
                    }

                    if (_customerAndAccountFileCounter > startingLineOfAccounts)
                    {

                        new Account(bank, int.Parse(columns[0]), 
                            int.Parse(columns[1]), 
                            ConvertStringToDecimalWithDecimal(columns[2]));
                        
                        //bank.ListOfAccounts.Add(new Account()
                        //{
                        //    Id = int.Parse(columns[0]),
                        //    CustomerId = int.Parse(columns[1]),
                        //    Balance = ConvertStringToDecimalWithDecimal(columns[2])
                        //});
                    }
                }
            } 

            Console.WriteLine($"Antal kunder: {bank.ListOfCustomers.Count}");
            Console.WriteLine($"Antal konton: {bank.ListOfAccounts.Count}");
            Console.WriteLine($"Totalt saldo: {bank.ListOfAccounts.Select(x => x.Balance).Sum().GetSwedishKr()}");
        }

        public void QuitAndSave(Bank bank)
        {
            var countCustomers = bank.ListOfCustomers.Count;
            var countAccounts = bank.ListOfAccounts.Count;
            var sumBalance = bank.ListOfAccounts.Select(x => x.Balance).Sum();
            var countTrans = bank.ListOfTransactions.Count;

            Output.WhiteColor("* Avsluta och spara *");
            Console.WriteLine($"Antal kunder: {countCustomers}");
            Console.WriteLine($"Antal konton: {countAccounts}");
            Console.WriteLine($"Totalt saldo på konton: {sumBalance.GetSwedishKr()}");
            Console.WriteLine($"Antal transaktioner: {countTrans}");

            // Create files
            CreateCustomersAndAccountsFile(bank);
            CreateTransactionsFile(bank);

            // Close the console
            Console.ReadLine();
            Environment.Exit(0);
        }

        private void CreateCustomersAndAccountsFile(Bank bank)
        {
            var date = DateTime.Now;

            // Date format
            var newFileName = date.ToString("yyyyMMdd-HHmm") + ".txt";
            Console.WriteLine($"Sparar till {newFileName}...");

            using (var sw = new StreamWriter(Path + newFileName))
            {
                // Count of customers
                sw.WriteLine(bank.ListOfCustomers.Count);

                // Get values from list of customers
                foreach (var item in bank.ListOfCustomers)
                {
                    sw.WriteLine($"{item.Id};{item.OrganisationNumber};{item.Name};" +
                                 $"{item.Adress};{item.City};{item.Region};{item.ZipCode};" +
                                 $"{item.Country};{item.Telephone}");
                }

                // Count of accounts
                sw.WriteLine(bank.ListOfAccounts.Count);

                // Get values from list of accounts
                foreach (var item in bank.ListOfAccounts)
                {
                    // Convert to . on balance
                    var balance = item.Balance.ToString(Culture);
                    var interestRate = item.InterestRate.ToString(Culture);
                    var creditLimit = item.CreditLimit.ToString(Culture);
                    var creditInterestRate = item.CreditInterestRate.ToString(Culture);

                    // Write values
                    sw.WriteLine($"{item.Id};{item.CustomerId};{balance};" +
                                 $"{interestRate};{creditLimit};{creditInterestRate}");
                }
            } 
        }

        public void CreateTransactionsFile(Bank bank)
        {

            Console.WriteLine($"Sparar till {TranFile}...");

            using (var sw = new StreamWriter(Path + TranFile))
            {
                // Number of transactions
                sw.WriteLine(bank.ListOfTransactions.Count);

                // Get values from list of transactions
                foreach (var item in bank.ListOfTransactions)
                {
                    // Convert culture
                    var amount = item.Amount.ToString(Culture);

                    // from and to account/balance are empty if they dont' have any value
                    var from = "";
                    var to = "";
                    if (item.FromAccountId != 0) from = item.FromAccountId.ToString();
                    if (item.ToAccountId != 0) to = item.ToAccountId.ToString();

                    var fromBalance = ""; 
                    var toBalance = ""; 

                    if (item.FromAccountBalance != 0) fromBalance = item.FromAccountBalance.ToString(Culture);
                    if (item.ToAccountBalance != 0) toBalance = item.ToAccountBalance.ToString(Culture);

                    // Write transactions
                    sw.WriteLine($"{item.Id};{amount};" +
                                 $"{from};{fromBalance};" +
                                 $"{to};{toBalance};" +
                                 $"{item.Date};{item.Type}");
                }
            }
        }

        private decimal ConvertStringToDecimalWithDecimal(string text)
        {
            var style = NumberStyles.AllowDecimalPoint;
            var invC = CultureInfo.InvariantCulture;

            if (decimal.TryParse(text, style, invC, out var number))
            {
                return number;
            }

            // Guard
            throw new FormatException();
        }

        private void GetTransactions(Bank bank)
        {

            using (var reader = new StreamReader(Path + TranFile))
            {
                _tranFileCounter = 0;

                var textfromfile = reader.ReadLine();

                while (textfromfile != null )
                {
                    _tranFileCounter++;

                    var columns = textfromfile.Split(';');

                    // Check next row
                    textfromfile = reader.ReadLine();

                    // Ignore the first line
                    if (_tranFileCounter == 1)
                    {
                        continue;
                    }

                    /* How the writeline looks like
                    sw.WriteLine($"{item.TransactionId};{balance};{amount};" +
                                 $"{item.CustomerId};{from};{to};" +
                                 $"{item.TrnDate};{item.Type}");
                                 */

                    // Get enum value
                    if (Enum.TryParse(columns[7], out TransactionType type))
                    {
                        
                    }
                    else
                    {
                        // Throw exception instead?
                        Output.RedColor("Kunde inte omvandla transaktionstypen från filen");
                    }

                    // Check if value is ""
                    int? fromAcc = null;
                    int? toAcc = null;
                    if (columns[4] != "" && columns[4] != null) fromAcc = int.Parse(columns[4]);
                    if (columns[5] != "" && columns[5] != null) toAcc = int.Parse(columns[5]);

                    // Check transactions
                    bank.ListOfTransactions.Add(new Transaction()
                    {
                        Id = int.Parse(columns[0]),
                        FromAccountBalance = ConvertStringToDecimalWithDecimal(columns[1]),
                        Amount = ConvertStringToDecimalWithDecimal(columns[2]),
                        FromAccountId = fromAcc,
                        ToAccountId = toAcc,
                        Date = DateTime.Parse(columns[6]),
                        Type = type

                    });
                }
            }

            Console.WriteLine($"Antal transaktioner: {bank.ListOfTransactions.Count}");
        }
    }
}
