using AccessControlModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountsWebApi.Models
{
    public class UserRepository
    {
        private AppUsersContext db;
        public UserRepository(AppUsersContext context)
        {
            db = context;
        }

        public User GetUser(string username, string passwordHash)
        {
            return db.Users.Where(u => (u.UserName == username) && (u.PasswordHash == passwordHash)).FirstOrDefault();
        }
    }

}
