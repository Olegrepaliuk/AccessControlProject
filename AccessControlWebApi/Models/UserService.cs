using AccessControlModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class UserService
    {
        private UsersRepository repo;
        private UserManager<User> userManager;
        public UserService(UsersRepository repository, UserManager<User> manager)
        {
            repo = repository;
            userManager = manager;
        }

        public User FindUser(string username, string password, bool hashed = false)
        {
            /*
            string hashedPass;
            if(hashed == false)
            {
                userManager.PasswordHasher.HashPassword()
            }
            return repo.GetUser(username, password);
            var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Success) successAuth = true;
            */
            return null;
        }
    }

}
