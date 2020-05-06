﻿using AccessControlModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountsWebApi.Models
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
        public async Task<User> FindUserAsync(string username, string password, bool hash = true)
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

        public async Task CreateUserWithRole(dynamic userWithRole)
        {
            string userName = userWithRole.UserName;
            string fullName = userWithRole.FullName;
            string password = userWithRole.Password;
            string role = userWithRole.Role;
            User user = new User { UserName = userName, FullName = fullName};
            try
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, new List<string> { role });
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }            
        }

        public async Task DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            await userManager.DeleteAsync(user);
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
        
        public async Task<List<Tuple<User, string>>> GetAllUsersWithRole()
        {
            var usersWithRole = new List<Tuple<User, string>>();
            var users = userManager.Users.ToList();
            foreach(var user in users)
            {
                bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");
                string role = isAdmin ? "Admin" : "User";
                usersWithRole.Add(new Tuple<User, string>(user, role));
            }
            return usersWithRole;
        }

    }
}