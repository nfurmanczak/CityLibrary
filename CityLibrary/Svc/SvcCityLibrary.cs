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

            if (ctx.Persons.Where(f => f.FirstName.StartsWith(user.Substring(0, 1).ToUpper())).Where(l => l.LastName.Contains(user.Substring(1, 1).ToUpper() + user.Substring(2, 2).ToLower())).Count() > 0)
            {
                person = ctx.Persons.Where(f => f.FirstName.StartsWith(user.Substring(0, 1).ToUpper())).Where(l => l.LastName.Contains(user.Substring(1, 1).ToUpper() + user.Substring(2, 2).ToLower())).First();
                return true; 
            }
            else
            {
                person = null;
                return false; 
            }
        }

        public IList<Medium> FindMedium(string pattern, int max, byte UserSelectOption)
        {
            var comp = System.StringComparison.OrdinalIgnoreCase;
            IQueryable<Medium> query; 

            if(UserSelectOption == 1)
                query = ctx.Mediums.Where(m => m.Title.Contains(pattern, comp) && m.Title != null);
            else if (UserSelectOption == 2)
                query = ctx.Mediums.Where(k => k.Kind.Contains(pattern, comp) && k.Kind != null);
            else if (UserSelectOption == 3)
                query = ctx.Mediums.Where(c => c.Category.Contains(pattern, comp) && c.Category != null);
            else 
                query = ctx.Mediums.Where(a => a.Author.Contains(pattern, comp) && a.Author != null); 
                
            return query.Take(max).ToList();
        }
    }
}
