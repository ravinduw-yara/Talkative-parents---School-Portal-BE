using CommonUtility.RequestModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text;
using Utility.Models;

namespace CommonUtility
{
    public interface IUtilityService
    {
        string GetJwtToken(MUserInfo user);
    }
    public  class UtilityService : IUtilityService
    {
        private readonly AppSettings appSettings;
        
        public UtilityService(IOptions<AppSettings> options)
        {
            appSettings = options.Value;
        }
  

        public string GetJwtToken(MUserInfo user)
        {
            var tokenHandeler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.JwtKey);

            var tokenDescribtor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                  new Claim(ClaimTypes.Name,user.Id.ToString()),
                  new Claim(ClaimTypes.Role,"Admin"),
                  new Claim(ClaimTypes.Version,"v3.1")
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(appSettings.TokenLifetime)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandeler.CreateToken(tokenDescribtor);
            
            return tokenHandeler.WriteToken(token);
        }
    }
}
