﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using final.Models;
using CG_TechPro.Data;
using System.Security.Claims;
using CG_TechPro.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace final.Controllers{
    public class HomeController : Controller
    {
        
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _accessor;

        public HomeController(DataContext context, IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult User()
        {
            return View();
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

        [HttpPost]
        [Route("/Home/Index")]
        public ActionResult Login(Users user)
        {
            if(string.IsNullOrEmpty(user.UserName)|| string.IsNullOrEmpty(user.Password))
            {
                return View("Index");
            }
            string desiredDomain = "@cginfinity.com";
            if (!user.UserName.EndsWith(desiredDomain, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid domain.");
            }
            var users = _context.Users.FirstOrDefault(u=>u.UserName==user.UserName);
            if(users == null){
                return BadRequest("Invalid user");
            }
            bool VerifyPass = BCrypt.Net.BCrypt.Verify(user.Password, users.Password);
            if (users != null && VerifyPass)
            {
                var token = GenerateJwtToken(users);
                _accessor.HttpContext.Session.SetString("jwtToken", token);
                var Id = users.U_Id;
                _accessor.HttpContext.Session.SetString("UserId" , Id.ToString());
                return RedirectToAction("User");
            }
            return BadRequest("Invalid");
        }

         [HttpPost]
        [Route("Admit")]
        public IActionResult PostUser([FromBody] Users user){
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string HashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password , salt);
            var users = new Users {
            UserName = user.UserName,
            Password = HashedPassword,
            };
            _context.Users.Add(users);
            _context.SaveChanges();
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
