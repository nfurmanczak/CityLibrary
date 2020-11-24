using CityLibrary.Model;
using CityLibrary.Svc;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace CityLibrary.Ui
{
    public class UiMenu
    {
        private SvcCityLibrary svc = null;

        public UiMenu(SvcCityLibrary svc) {
            this.svc = svc;
        }

        public void ShowMainMenu() {
            UiHelpers.ShowHeader("Willkommen in der Stadtbücherei");

            // setup menu items
            var menuItems = new UiMenuItem[] {
                new UiMenuItem('A', "Anmelden", Login),
                new UiMenuItem('S', "Medium suchen", FindMedium),
            };

            // show menu items
            foreach (var mi in menuItems) {
                Console.WriteLine($"{mi.Key} {mi.Info}");
            }

            // ask user an invoke menu action
            for (bool ende = false; !ende; /**/) {
                var key = UiHelpers.AskKey("\nIhre Wahl");
                foreach (var mi in menuItems) {
                    if (mi.Key == key) {
                        mi.Action();
                        ende = true;
                        break;
                    }
                }
            }
        }

        public void Login() {
            // ask user for credentials
            UiHelpers.ShowHeader("Anmelden");
            
            string user; 

            do
            {
                user = UiHelpers.AskValue("Benutzername");
                if (user.Length != 4)
                    Console.WriteLine("Benutername muss 4 Buchstaben enthalten.");
            } while (user.Length != 4 );

            
            var pwHash = UiHelpers.AskValue("Kennwort").GetHashCode();

            // authenticate user
            if (svc.FindPersonByCredentials(user, pwHash, out Person person)) {
                Console.WriteLine("Benutzername: " + user);
                Console.WriteLine("Password: " + pwHash);
                Console.WriteLine("Fake result:");

                if (pwHash == "4711".GetHashCode() )
                    Console.WriteLine("Password OK.");
                else
                    Console.WriteLine("Password Falsch!");
            }
            else 
                Console.WriteLine("Benutzer nicht gefunden.");

            // TODO
            /*
            Console.WriteLine("\n!!!To do:");
            Console.WriteLine("Select concrete Person");
            Console.WriteLine("User name shall be for all persons:");
            Console.WriteLine("  first letter of first name plus");
            Console.WriteLine("  3 fist letters of last name.");
            Console.WriteLine("Example: Hugo Müller --> hmül");
            Console.WriteLine("User password shall be \"4711\" for all persons:");
            */
        }

        public void FindMedium() {
            UiHelpers.ShowHeader("Medium suchen");
            List<string> options = new List<string> {"Titel", "Format", "Kategorie", "Author" };

            PrintCategory(options);
            bool loopcheck; 
            
            do
            {
                Console.Write("Eingabe: ");
                string userSelectOptionString = Console.ReadLine();
                if (Byte.TryParse(userSelectOptionString, out _))
                {
                    if (Convert.ToByte(userSelectOptionString) <= options.Count())
                    {
                        Console.Write("Suchmuster: ");
                        IList<Medium> result = svc.FindMedium(Console.ReadLine(), 10, Convert.ToByte(userSelectOptionString));
                        PrintResult(result);

                        loopcheck = false;
                    }
                    else
                    {
                        Console.WriteLine("Kein gültiger Menüpunkt.");
                        loopcheck = true; 
                    } 
                }
                else
                {
                    Console.WriteLine("Ungültige Eingabe");
                    loopcheck = true; 
                }
                
            } while (loopcheck);

            // TODO
            /*
            Console.WriteLine("\n!!!To do:");
            Console.WriteLine("The search pattern shall also be used");
            Console.WriteLine("to match category, kind and author.");
            */
        }

        //     
        public void PrintResult(IList<Medium> result)
        {
            int i = 0;

            if (result.Count == 0)
                Console.WriteLine("Nichts gefunden");
            else
            {
                foreach (var medium in result)
                {
                    Console.WriteLine($"{++i:d2} {medium.Title}");
                    Console.WriteLine($"     [{medium.Category}] [{medium.Kind}] {medium.Author}");
                }
            }
        }

        public void PrintCategory<T>(List<T> menulist)
        {
            foreach (var mitem in menulist.Select((value, index) => new { value, index }))
                Console.WriteLine("{0}. {1}", mitem.index + 1, mitem.value);
        }
    }
}
