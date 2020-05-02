using AccessControlModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccessControl.Models
{
    public class AccountService
    {
        private string baseAddress = "https://localhost:44381/identity/account";
        private static HttpClient client;
        public AccountService(HttpClient cl)
        {
            client = cl;
        }
        public async Task<string> GetAuthenticationToken(string username, string password)
        {            
            var user = new
            {
                username = username,
                password = password
            };
            string address = $"{baseAddress}/token";
            var response = await client.PostAsJsonAsync($"{baseAddress}/token", user);
            if(response.IsSuccessStatusCode)
            {
                var tokenModel = await response.Content.ReadAsAsync<dynamic>();
                return tokenModel.access_token;
            }
            return null;
        }
        public ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.AUDIENCE,
                    ValidateLifetime = false,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                };
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
