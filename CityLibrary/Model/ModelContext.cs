/*
 * www.gso-koeln.de 2020
 */
using Gso.FS.EFCore.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CityLibrary.Model
{
    public class ModelContext : DbContext
    {
        // Sqlite database file to utilize
        public const string DataBaseFile = @"../../../CityLibrary.Sqlite";
        public static ILoggerFactory loggerFactory = null;
        public DbSet<Person> Persons { get; set; } // Person Cache
        public DbSet<Medium> Mediums { get; set; } // Medium Cache
        public DbSet<Item> Items { get; set; } // Item Cache

        protected override void OnConfiguring(DbContextOptionsBuilder dcob) {
            // enable logging
            if (loggerFactory == null) {
                //loggerFactory = LoggerFactory.Create(lbldr => lbldr.AddDebug());
                //loggerFactory = LoggerFactory.Create(lbldr => lbldr.AddConsole());
                //loggerFactory = LoggerFactory.Create(lbldr => lbldr.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information).AddConsole());
                loggerFactory = new LoggerFactory(new ILoggerProvider[] {
                    new GsoLoggerProvider()
                    .SetLogLevel(LogLevel.Information)
                    .AddCategories("Microsoft.EntityFrameworkCore.Database.Command")
                    .AddEventIDs(20101) // CommandExecuted
                    .AddEventIDs(0) // unspecified event id
                });
            }
            dcob = dcob.UseLoggerFactory(loggerFactory);

            // inject Sqlite usage
            dcob.UseSqlite("Data Source=" + DataBaseFile);
        }
        protected override void OnModelCreating(ModelBuilder builder) {
            // the class hierarchy subtypes
            builder.Entity<Admin>().Property(b => b.Id).HasField("id");
            builder.Entity<Employee>().Property(b => b.Id).HasField("id");
            builder.Entity<Member>().Property(b => b.Id).HasField("id");

            // the class hierarchy base type
            builder.Entity<Person>().Property(b => b.Id).HasField("id");
            //builder.Entity<Person>().HasDiscriminator<int>("PersonType")
            //    .HasValue<Member>(1)
            //    .HasValue<Employee>(2)
            //    .HasValue<Admin>(3);

            builder.Entity<Medium>().HasKey("Identifier");
            builder.Entity<Medium>().Property(b => b.Identifier).HasField("identifier");
            builder.Entity<Item>().Property(b => b.Id).HasField("id");
        }
    }
}
