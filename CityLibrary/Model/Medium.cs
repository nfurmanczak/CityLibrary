/*
 * www.gso-koeln.de 2020
 */
using System;

namespace CityLibrary.Model
{
    public class Medium
    {
        protected string identifier;

        public Medium(string identifier) {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException("invalid identifier");
            this.identifier = identifier;
        }
        public string Identifier { get { return identifier; } }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime? Date { get; set; }
        public string Kind { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public Decimal Price { get; set; }
    }
}
