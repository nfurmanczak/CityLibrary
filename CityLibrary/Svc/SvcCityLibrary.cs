/*
 * www.gso-koeln.de 2020
 */
using CityLibrary.Model;
using System.Collections.Generic;
using System.Linq;

namespace CityLibrary.Svc
{
    public class SvcCityLibrary
    {
        private ModelContext ctx = null;

        public SvcCityLibrary(ModelContext ctx) { this.ctx = ctx; }

        public bool FindPersonByCredentials(string user, int pwHash, out Person person) {
            //TODO: real search using user and password hash
            person = ctx.Persons.FirstOrDefault();
            return true;
        }

        public IList<Medium> FindMedium(string pattern, int max) {
            var comp = System.StringComparison.OrdinalIgnoreCase;
            var query = ctx.Mediums.Where(m => m.Title.Contains(pattern, comp));
            return query.Take(max).ToList();
        }
    }
}
