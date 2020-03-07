using AccessControl.Models;
using AccessControlModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class UsersInfo
    {
        public static async Task<User> FindUser(string username, string password, UserManager<User> _userManager, bool hash = true)
        {
            var user = await _userManager.FindByNameAsync(username);
            bool successAuth = false;
            if (user != null)
            {
                if (hash)
                {
                    successAuth = (password == user.PasswordHash);
                }
                else
                {
                    var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                    if (result == PasswordVerificationResult.Success) successAuth = true;
                }
            }
            if (successAuth) return user;
            return null;
        }
        public static async Task<bool> CheckAdminRights(User user, UserManager<User> _userManager)
        {
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            foreach (var role in rolesList)
            {
                if (role.ToUpper() == "ADMIN") return true;
            }
            return false;
        }
    }
}
