using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Kundbilden
{
    public class Customer: BasicInfo
    {
        // Mandatory
        //public int Id { get; set; }
        public string Name { get; set; }
        public string OrganisationNumber { get; set; }
        public string Adress { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public List<Account> Accounts = new List<Account>();

        // Optionally
        public string Region { get; set; }
        public string Country { get; set; }
        public string Telephone { get; set; }


        public Customer()
        {

        }

        public Customer(Bank bank)
        {
            Id = CreateUniqueId(bank);
        }

        public override int CreateUniqueId(Bank bank)
        {
            var count = bank.ListOfCustomers.Count();
            if (count == 0) return 1001;

            // Set unique customer id
            var sortIdDesc = bank.ListOfCustomers.OrderByDescending(x => x.Id);

            return sortIdDesc.First().Id + 1;
        }



        public override string ToString()
        {
            return Id.ToString();
        }
    }
}