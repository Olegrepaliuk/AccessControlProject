using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccessControlModels;
using AccountsWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AccountsWebApi.Controllers
{
    [Route("identity/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserService userService;
        public AccountController(UserService service)
        {
            userService = service;
        }
        [HttpPost("token")]
        public ActionResult<object> Token(dynamic obj)
        {
            string username = obj.username;
            string password = obj.password;
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return response;
        }
        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var user = AsyncHelper.RunSync<User>(() => userService.FindUserAsync(username, password, false));
            if (user != null)
            {
                bool isAdmin = AsyncHelper.RunSync<bool>(() => userService.CheckAdminRights(user));
                string userRole = isAdmin ? "Admin" : "User";
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}