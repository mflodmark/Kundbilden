using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Kundbilden
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                var menu = new Menu();
                var bank = new Bank();
                var file = new FileManagement();

                // Get current files
                //file.PrintCurrentDirectory();
                file.GetFiles(bank);

                Console.WriteLine("\n*********************************");
                Console.WriteLine("* Välkommen till kundbilden 2.0 *");
                Console.WriteLine("*********************************\n");

                menu.PrintMenu();
                menu.GetChoiceAndExecute(bank);
            }
            catch (Exception)
            {
                Output.RedColor("Någonting gick helt galet!\nRing supporten på (08-701) 125 80");
            }




            Console.ReadLine();

        }


    }
}
