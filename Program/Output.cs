using System;

namespace Kundbilden
{
    public abstract class Output
    {
        public static void RedColor(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{text}\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        public static void GreenColor(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{text}\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        public static void WhiteColor(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n{text}\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }
    }
}