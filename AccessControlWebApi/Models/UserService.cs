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
        private UserRepository repo;
        private UserManager<User> userManager;
        public UserService(UserRepository repository, UserManager<User> manager)
        {
            repo = repository;
            userManager = manager;
        }

        public  async Task<User> FindUserAsync(string username, string password, bool hash = true)
        {
            var user = await userManager.FindByNameAsync(username);
            bool successAuth = false;
            if (user != null)
            {
                if (hash)
                {
                    successAuth = (password == user.PasswordHash);
                }
                else
                {
                    var result = userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                    if (result == PasswordVerificationResult.Success) successAuth = true;
                }
            }
            if (successAuth) return user;
            return null;
        }

        public async Task<bool> CheckAdminRights(User user)
        {
            var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            foreach (var role in rolesList)
            {
                if (role.ToUpper() == "ADMIN") return true;
            }
            return false;
        }
    }

}
