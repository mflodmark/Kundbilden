using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kundbilden
{
    public abstract class Input
    {
        public static bool CheckForAccountWithAccountId(int id, Bank bank)
        {
            var selected = bank.ListOfAccounts.SingleOrDefault(x => x.Id == id);
            if (selected == null)
            {
                return false;
            }

            return true;
        }

        public static bool CheckForCustomerWithCustomerId(int id, Bank bank)
        {
            var selected = bank.ListOfCustomers.SingleOrDefault(x => x.Id == id);
            if (selected == null)
            {
                return false;
            }

            return true;
        }

        public static bool CheckForCustomerWithCustomerIdOrAccountId(int id, Bank bank, out int customerId)
        {
            var selected = bank.ListOfCustomers.SingleOrDefault(x => x.Id == id || x.Accounts.Any(y => y.Id == id));

            if (selected == null)
            {
                customerId = 0;
                return false;
            }

            customerId = selected.Id;
            
            return true;
        }

        public static int AskForAccount()
        {

            while (true)
            {
                Console.Write("Ange kontonummer: ");

                if (int.TryParse(Console.ReadLine(), out int accountId))
                {
                    return accountId;
                }
                else
                {
                    Output.RedColor("Mata in kontonummer med siffror. Försök igen.");

                }
            }
        }

        public static int AskForCustomer()
        {
            while (true)
            {

                Console.Write("Ange kundnummer: ");

                if (int.TryParse(Console.ReadLine(), out int customerId))
                {
                    return customerId;
                }
                else
                {
                    Output.RedColor("Mata in kundnummer med siffror. Försök igen.");
                }

            }

        }

        public static int AskForAccountIdOrCustomerId()
        {
            while (true)
            {
                Console.Write("Kundnummer eller kontonummer? > ");

                if (int.TryParse(Console.ReadLine(), out int customerNrOrAccountNr))
                {
                    return customerNrOrAccountNr;
                }
                else
                {
                    Output.RedColor("Mata in kundnummer eller kontonummer med siffror. Försök igen.");
                }

            }


        }

        public static decimal AskForAmount()
        {
            var amount = ConvertStringToDecimalWithComma("Ange belopp: ");
            
            return amount;
        }

        public static Customer AskForCustomerInfo()
        {
            Console.WriteLine("* Lägg till ny kund *");

            Output.WhiteColor("Obligatoriskt");
            var name = AskFor("Namn");
            var adress = AskFor("Adress");
            var zipCode = AskFor("Postnummer");
            var city = AskFor("Postort");
            var orgNr = AskFor("Organisationsnummer");

            Output.WhiteColor("Valfritt");
            Console.Write($"Country:");
            var country = Console.ReadLine();
            Console.Write($"Region: ");
            var region = Console.ReadLine();
            Console.Write($"Telefon: ");
            var tel = Console.ReadLine();

            return new Customer()
            {
                Name = name,
                Adress = adress,
                City = city,
                Country = country,
                ZipCode = zipCode,
                Region = region,
                OrganisationNumber = orgNr,
                Telephone = tel
            };
        }

        public static string AskFor(string input)
        {
            
            Console.Write($"{input}: ");
            var text = Console.ReadLine();

            if (text != null && !text.Equals("")) return text.UpperFirstLetter();

            // Ask the same question again
            Output.RedColor($"{input} är obligatoriskt");
            AskFor(input);

            // Return text if text is not equal to null or ""
            return text;
        }

 
        public static decimal ConvertStringToDecimalWithComma(string text)
        {

            while (true)
            {
                Console.Write($"{text}");

                if (decimal.TryParse(Console.ReadLine(), out var nr))
                {
                    if (nr >= 0)
                    {
                        return nr;
                    }
                    else
                    {
                        Output.RedColor("Belopp måste vara positivt");
                        continue;
                    }

                }
                else
                {
                    Output.RedColor("Try again!");
                }
            }
        }


        public static decimal AskForRate()
        {
            var account = Input.ConvertStringToDecimalWithComma("Ange ränta(decimalform): ");

            return account;
        }
    }
}