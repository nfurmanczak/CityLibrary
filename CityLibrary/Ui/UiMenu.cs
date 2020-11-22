using CityLibrary.Model;
using CityLibrary.Svc;
using System;

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
            var user = UiHelpers.AskValue("Benutzername");
            var pwHash = UiHelpers.AskValue("Kennwort").GetHashCode();

            // authenticate user
            if (svc.FindPersonByCredentials(user, pwHash, out Person person)) {
                Console.WriteLine("Fake result:");
                Console.WriteLine(person.FirstName + " " + person.LastName);
            }

            // TODO
            Console.WriteLine("\n!!!To do:");
            Console.WriteLine("Select concrete Person");
            Console.WriteLine("User name shall be for all persons:");
            Console.WriteLine("  first letter of first name plus");
            Console.WriteLine("  3 fist letters of last name.");
            Console.WriteLine("Example: Hugo Müller --> hmül");
            Console.WriteLine("User password shall be \"4711\" for all persons:");
        }

        public void FindMedium() {
            UiHelpers.ShowHeader("Medium suchen");

            // ask search pattern
            Console.Write("Suchbedingung: ");
            var pattern = Console.ReadLine();
            int i = 0;
            var result = svc.FindMedium(pattern, 10);

            // show search result
            if (result.Count == 0) {
                Console.WriteLine("Nichts gefunden");
            } else {
                foreach (var medium in result) {
                    Console.WriteLine($"{++i:d2} {medium.Title}");
                    Console.WriteLine($"     [{medium.Category}] [{medium.Kind}] {medium.Author}");
                }
            }

            // TODO
            Console.WriteLine("\n!!!To do:");
            Console.WriteLine("The search pattern shall also be used");
            Console.WriteLine("to match category, kind and author.");
        }
    }
}
