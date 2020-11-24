/*
 * www.gso-koeln.de 2020
 */
using System; // DateTime

namespace CityLibrary.Model
{
    public abstract class Person
    {
        protected int id; // --> see OnModelCreating(...)
        public int Id { get { return id; } } // readonly access
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }

        // derived properties
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        // dummy properties to be supported later

        public string UserName { get { return "FAKE"; } }
        public int PwdHash { get { return "4711".GetHashCode(); } }
    }
}
