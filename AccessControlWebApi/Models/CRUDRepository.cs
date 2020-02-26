using AccessControl.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class CRUDRepository
    {
        private AccessCtrlContext db;

        public CRUDRepository(AccessCtrlContext context)
        {
            db = context;
        }

        public IEnumerable<Person> People
        {
            get
            {
                return db.People;
            }

        }

        public void PutPerson(Person person)
        {
            db.Entry(person).State = EntityState.Modified;
            db.SaveChanges();
        }

        public Person GetPersonById(int id)
        {
            return db.People.Where(p => p.Id == id).FirstOrDefault();
        }

        public void AddPerson(Person person)
        {
            db.People.Add(person);
            db.SaveChanges();
        }

        public string DeletePerson(int id)
        {
            var person = db.People.Where(p => p.Id == id).FirstOrDefault();
            if(person != null)
            {
                db.People.Remove(person);
                db.SaveChanges();
                return "deleted";
            }
            else
            {
                return "NotFound";
            }
            
        }
    }
}
