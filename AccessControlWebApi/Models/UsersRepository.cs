using AccessControlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class UsersRepository
    {
        private AppUsersContext db;
        public UsersRepository(AppUsersContext context)
        {
            db = context;
        }
        
        public User GetUser(string username, string passwordHash)
        {
            return db.Users.Where(u => (u.UserName == username) && (u.PasswordHash == passwordHash)).FirstOrDefault();
        }

    }
}
