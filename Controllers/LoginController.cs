using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CG_TechPro.Data;
using CG_TechPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CG_TechPro.Controllers
{
    [ApiController]
    [Route("Login")]
    public class LoginController : Controller
    {
        
        private readonly DataContext _context;

        public LoginController(DataContext context)
        {
            _context = context;
        }
        private static string GenerateJwtToken(Users user)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expirationMinutes = 1; 
                var expirationTime = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var token = new JwtSecurityToken(
                    issuer: "your_issuer",
                    audience: "your_audience",
                    claims: claims,
                    expires: expirationTime,
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
         public IActionResult GetUser([FromBody]Users user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return View("Login");
            }
            string desiredDomain = "@cginfinity.com";
            if (!user.UserName.EndsWith(desiredDomain, StringComparison.OrdinalIgnoreCase))
            {
                return View("InvalidDomainError");
            }
            bool VerifyPass = BCrypt.Net.BCrypt.Verify(user.Password, BCrypt.Net.BCrypt.HashPassword(user.Password));
            var users = _context.Users.FirstOrDefault(u => u.UserName == user.UserName);
            if (user != null && VerifyPass)
            {
                var token = GenerateJwtToken(users);
                return Json(new { token });
            }
            return View("Login");
        }
    }
}