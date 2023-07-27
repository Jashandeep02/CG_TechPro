// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IdentityModel.Tokens.Jwt;
// using System.Linq;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using CG_TechPro.Data;
// using CG_TechPro.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Microsoft.IdentityModel.Tokens;

// namespace CG_TechPro.Controllers
// {
//     [ApiController]
//     [Route("Login")]
//     public class LoginController : Controller
//     {
        
//         private readonly DataContext _context;

//         public LoginController(DataContext context)
//         {
//             _context = context;
//         }
//         public IActionResult User()
//         {
//             return View();
//         }
//         private static string GenerateJwtToken(Users user)
//             {
//                 var claims = new List<Claim>
//                 {
//                     new Claim(ClaimTypes.Name, user.UserName)
//                 };

//                 var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"));
//                 var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//                 var expirationMinutes = 1; 
//                 var expirationTime = DateTime.UtcNow.AddMinutes(expirationMinutes);

//             var token = new JwtSecurityToken(
//                     issuer: "your_issuer",
//                     audience: "your_audience",
//                     claims: claims,
//                     expires: expirationTime,
//                     signingCredentials: credentials
//                 );

//                 return new JwtSecurityTokenHandler().WriteToken(token);
//             }

        // [HttpPost("/Home/Index")]
        // public IActionResult Login([FromBody] Users user)
        // {
        //     if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
        //     {
        //         return BadRequest("Username and password are required.");
        //     }

        //     string desiredDomain = "@cginfinity.com";
        //     if (!user.UserName.EndsWith(desiredDomain, StringComparison.OrdinalIgnoreCase))
        //     {
        //         return BadRequest("Invalid domain.");
        //     }

        //     var storedUser = _context.Users.FirstOrDefault(u => u.UserName == user.UserName);
        //     if (storedUser != null && BCrypt.Net.BCrypt.Verify(user.Password, (storedUser.Password)))
        //     {
        //         var token = GenerateJwtToken(storedUser);
        //         //HttpContext.Session.SetString("jwtToken", token);
        //         return View("User");
        //     }

        //     return BadRequest("Invalid username or password.");
        // }

//         [HttpPost("/Home/Index")]

//         public ActionResult Login(Users Loginmodel)
//         {
//             if(string.IsNullOrEmpty(Loginmodel.UserName)|| string.IsNullOrEmpty(Loginmodel.Password))
//         {
//             return View("Index");
//         }
//             //  string Passwordhash=HashPassword(Loginmodel.Password);

//             var user = _context.Users.FirstOrDefault(u=>u.UserName==Loginmodel.UserName && u.Password==Loginmodel.Password);

//             if (user != null)
//             {
//                 return RedirectToAction("User");
//             }
//             return View("Index");
//         }

//     }
// }