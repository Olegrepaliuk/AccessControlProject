﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControlWebApi.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "AccessControlWebApiIssuer";
        public const string AUDIENCE = "AccessControlWebApiClient";
        const string KEY = "api_access_control_secretkey2020";
        public const int LIFETIME = 525600;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
