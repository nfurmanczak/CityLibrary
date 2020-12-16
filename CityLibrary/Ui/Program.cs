/*
 * www.gso-koeln.de 2020
 */
using CityLibrary.Model;
using CityLibrary.Svc;
using CityLibrary.Ui;
using Gso.FS.EFCore.Logging;
using Microsoft.EntityFrameworkCore; // Include()
using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

namespace CityLibrary.Ui
{
    class Program
    {
        static void Main(string[] args) {
            new Program().Run();
        }

        public void Run() {
            UiHelpers.InitConsole();

            // drop database file for testing purpose
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                File.Delete(ModelContext.DataBaseFileWinOS);
            else
                File.Delete(ModelContext.DataBaseFilemacOS);

            using (var db = new ModelContext()) {
                // create DB if not exists
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                GsoLogger.LoggingEnabled = false;

                // populate DB
                if (db.Persons.OfType<Admin>().Count() == 0) {
                    db.Add(new Admin() { FirstName = "Adalbert", LastName = "Acker", Department = "Service" });
                    db.Add(new Admin() { FirstName = "Agate", LastName = "Ahrweiler", Department = "Service" });
                    db.SaveChanges();
                }
                if (db.Persons.OfType<Employee>().Where(p => !(p is Admin)).Count() == 0) {
                    var boss = new Employee() { FirstName = "Egon", LastName = "Boss", Department = "Customer" };
                    db.Add(boss);
                    db.Add(new Employee() { FirstName = "Ernst", LastName = "Ecker", LineManager = boss, Department = "Customer" });
                    db.Add(new Employee() { FirstName = "Emilia", LastName = "Eichberg", LineManager = boss, Department = "Customer" });
                    db.SaveChanges();
                }
                if (db.Persons.OfType<Member>().Count() == 0) {
                    db.AddRange(SvcImport.ImportMembers3(@"../../../Data/fake-persons.txt"));
                    db.SaveChanges();
                }
                if (db.Mediums.Count() == 0) {
                    var mediums = SvcImport.ImportMediums3(@"../../../Data/spiegel-bestseller.txt");
                    db.AddRange(mediums);
                    db.SaveChanges();
                }
                if (db.Items.Count() == 0) {
                    // 9783406745713 Kapital und Ideologie
                    db.Add(new Item() { Available = ItemAvailable.InStock, State = ItemState.Usable, StorageLocation = "Regal 1", MediumId = db.Mediums.Where(i => i.Identifier.Contains("9783406745713")).SingleOrDefault() });
                    db.Add(new Item() { Available = ItemAvailable.InStock, State = ItemState.Usable, StorageLocation = "Regal 1", MediumId = db.Mediums.Where(i => i.Identifier.Contains("9783406745713")).SingleOrDefault() });
                    db.Add(new Item() { Available = ItemAvailable.borrowed, State = ItemState.Usable, StorageLocation = "Regal 1", MediumId = db.Mediums.Where(i => i.Identifier.Contains("9783406745713")).SingleOrDefault() });
                    db.Add(new Item() { Available = ItemAvailable.borrowed, State = ItemState.Usable, StorageLocation = "Regal 2", MediumId = db.Mediums.Where(i => i.Identifier.Contains("9783406745713")).SingleOrDefault() });
                    // 9783551556912 Die Abenteuer des Apollo. Die Gruft des Tyranne
                    db.Add(new Item() { Available = ItemAvailable.InStock, State = ItemState.Usable, StorageLocation = "Regal 2", MediumId = db.Mediums.Where(i => i.Identifier.Contains("9783551556912")).SingleOrDefault() });
                    db.Add(new Item() { Available = ItemAvailable.InStock, State = ItemState.Defect, StorageLocation = "Regal 2", MediumId = db.Mediums.Where(i => i.Identifier.Contains("9783551556912")).SingleOrDefault() });
                    // 9783789115172 Mein Weg zum Fußballprofi
                    db.Add(new Item() { Available = ItemAvailable.InStock, State = ItemState.Usable, StorageLocation = "Regal 3", MediumId = db.Mediums.Where(i => i.Identifier.Contains("9783789115172")).SingleOrDefault() });
                    db.Add(new Item() { Available = ItemAvailable.InStock, State = ItemState.Ordered, StorageLocation = "Regal 3", MediumId = db.Mediums.Where(i => i.Identifier.Contains("9783789115172")).SingleOrDefault() });
                    db.Add(new Item() { Available = ItemAvailable.borrowed, State = ItemState.Defect, StorageLocation = "Regal 3", MediumId = db.Mediums.Where(i => i.Identifier.Contains("9783789115172")).SingleOrDefault() });
                    db.SaveChanges();
                }

                GsoLogger.LoggingEnabled = true;

                //ShowStuff();

                var svc = new SvcCityLibrary(db);
                var menu = new UiMenu(svc);
                while (true) {
                    menu.ShowMainMenu();
                    Console.ReadLine();
                }

                Console.WriteLine("\nDone.");
                Console.ReadLine();

                foreach (var item in db.Persons.ToList())
                {
                    Console.WriteLine(item.FirstName);
                    Console.WriteLine(item.LastName);
                }
                
            }
        }

        void ShowStuff() {
            using (var db = new ModelContext()) {
                // show data
                int i = 0;

                Console.WriteLine("\n***** persons");
                i = 0;
                foreach (var p in db.Persons) {
                    Console.WriteLine($"{i++:d2}: {p.FullName,-22} is '{p.GetType().Name}' ");
                }

                Console.WriteLine("\n***** teams by line manager");
                i = 0;
                foreach (var e in db.Persons.OfType<Employee>()
                    .Where(e => e.Team.Count > 0)
                    .Include(nameof(Employee.Team))) {
                    Console.WriteLine($"Team of {e.FullName}");
                    foreach (var p in e.Team)
                        Console.WriteLine($" * {p.FullName}");
                }

                Console.WriteLine("\n***** employees sorted by department");
                i = 0;
                foreach (var e in db.Persons
                    .OfType<Employee>()
                    .OrderBy(e => e.Department)
                    .OrderBy(e => e.LastName)
                    .OrderBy(e => e.FirstName)) {
                    Console.WriteLine($"{i++:d2}: {e.FullName,-22} at '{e.Department}' ");
                }

                Console.WriteLine("\n***** employees by department");
                var depts = db.Persons
                    .OfType<Employee>()
                    .Select(e => e.Department)
                    .Distinct()
                    .ToList();
                foreach (var d in depts) {
                    i = 0;
                    Console.WriteLine($"Department '{d}'");
                    foreach (var e in db.Persons
                        .OfType<Employee>()
                        .Where(e => e.Department == d)
                        .OrderBy(e => e.LastName)
                        .OrderBy(e => e.FirstName)) {
                        Console.WriteLine($" * {i++:d2}: {e.FullName}");
                    }
                }

                Console.WriteLine("\n***** administrators");
                i = 0;
                foreach (var a in db.Persons.OfType<Admin>()) {
                    Console.WriteLine($"{i++:d2}: {a.FullName,-22} is '{a.GetType().Name}' ");
                }

                Console.WriteLine("\n***** media");
                i = 0;
                foreach (var medium in db.Mediums.Where(m => m.Title.StartsWith("Da"))) {
                    Console.WriteLine($"{i++:d2}: {medium.Identifier} {medium.Title} [{medium.Kind}] {medium.Price}");
                }

                Console.WriteLine("\n***** items");
                i = 0;
                foreach (var item in db.Items) {
                    //Console.WriteLine("Item: "+ item.Id + "");
                    //Console.WriteLine("Item: {0}", item.Id);
                    Console.WriteLine($"{i++:d2}: {item.Id}");
                }
            }
        }
    }
}
