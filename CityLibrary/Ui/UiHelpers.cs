using CityLibrary.Svc;
using System;

namespace CityLibrary.Ui
{
    public static class UiHelpers
    {
        public static void InitConsole() {
            //Console.WindowWidth = 80;
            //Console.WindowHeight = 30;
            //Console.BufferWidth = 80;
            //Console.BufferHeight = 1000;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Clear();
        }

        public static void ShowHeader(string title) {
            Console.Clear();
            Console.WriteLine(new String('-', 30));
            Console.WriteLine($"* {title}");
            Console.WriteLine(new String('-', 30));
        }

        public static string AskValue(string prompt) {
            Console.Write($"{prompt,-15}: ");
            return Console.ReadLine();
        }
        public static char AskKey(string prompt) {
            Console.Write($"{prompt}: ");
            return Char.ToUpper(Console.ReadKey().KeyChar);
        }
    }
}
