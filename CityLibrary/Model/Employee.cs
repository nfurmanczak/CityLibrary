/*
 * www.gso-koeln.de 2020
 */

using System.Collections.Generic;

namespace CityLibrary.Model
{
    public class Employee : Person
    {
        public string Department { get; set; }
        public Employee LineManager { get; set; }
        public IList<Employee> Team { get; set; }
    }
}

