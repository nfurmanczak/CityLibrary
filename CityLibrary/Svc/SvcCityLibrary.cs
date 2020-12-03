/*
 * www.gso-koeln.de 2020
 */
using CityLibrary.Model;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;

namespace CityLibrary.Svc
{
    public class SvcCityLibrary
    {
        private ModelContext ctx = null;

        public SvcCityLibrary(ModelContext ctx) { this.ctx = ctx; }

        public bool FindPersonByCredentials(string user, int pwHash, out Person person) {
            //TODO: real search using user and password hash

            if (ctx.Persons.Where(a => a.UserName == user).Count() > 0)
            {
                Console.WriteLine(); 
                //person = ctx.Persons.Where(f => f.FirstName.StartsWith(user.Substring(0, 1).ToUpper())).Where(l => l.LastName.Contains(user.Substring(1, 1).ToUpper() + user.Substring(2, 2).ToLower())).First();
                person = ctx.Persons.Where(a => a.UserName == user).FirstOrDefault(); 
                return true; 
            }
            else
            {
                person = null;
                return false; 
            }
        }

        public IList<Medium> FindMedium(string pattern, int max, byte UserSelectOption) {
            var comp = System.StringComparison.OrdinalIgnoreCase;
            IQueryable<Medium> query; 

            if(UserSelectOption == 1)
                query = ctx.Mediums.Where(m => m.Title != null && m.Title.Contains(pattern, comp));
            else if (UserSelectOption == 2)
                query = ctx.Mediums.Where(k => k.Kind != null && k.Kind.Contains(pattern, comp));
            else if (UserSelectOption == 3)
                query = ctx.Mediums.Where(c => c.Category != null && c.Category.Contains(pattern, comp));
            else 
                query = ctx.Mediums.Where(a => a.Author != null && a.Author.Contains(pattern, comp)); 
                
            return query.Take(max).ToList();
 
        }



        public IList<Item> FindItem(string pattern)
        {
            var comp = System.StringComparison.OrdinalIgnoreCase;
            IQueryable<Item> query;
           
            query = ctx.Items.Include(nameof(Item.MediumId))
                    .Where(a => a.MediumId.Title.Contains(pattern, comp))
                    .Where(b => b.State == ItemState.Usable)
                    .Where(c => c.Available == ItemAvailable.InStock);

            return query.ToList(); 

        }

        public void BorrowItem(Item i1)
        {
            /* 
             * var f = db.Mediums.SingleOrDefault(x => x.Id == id);
                f.Borrow = 1;
                db.SaveChanges(); 
             * 
             */

            var f = ctx.Items.SingleOrDefault(x => x.Id == i1.Id);
            f.Available = ItemAvailable.borrowed;
            ctx.SaveChanges(); 
        }

    }
}
