using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using final.Models;
using CG_TechPro.Data;
using System.Security.Claims;
using CG_TechPro.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;

namespace final.Controllers{
    public class HomeController : Controller
    {
        
        private readonly DataContext _context;
   private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(DataContext context,IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;

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
 public IActionResult Admin()
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


        [HttpPost("/Home/Index")]

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
                return View("Index");
            }
            bool VerifyPass = BCrypt.Net.BCrypt.Verify(users.Password, BCrypt.Net.BCrypt.HashPassword(user.Password));
            if (users != null && VerifyPass)
            {
                var token = GenerateJwtToken(users);
                HttpContext.Session.SetString("jwtToken", token);
                return RedirectToAction("User");
            }
            return View("Index");
        }

         [HttpPost("/Home/Admin")]
    public  ActionResult<string> addDevice(string deviceType, int count, string specifications)
    {
        var data = new Devices { D_Type = deviceType,CreatedAtUTC=DateTime.Now,UpdatedAtUTC=DateTime.Now, CreatedBy=123};
        _context.Devices.Add(data);
     for (int i = 0; i < count; i++)
    {
                       
        var deviceData = _context.Devices.Find(data.D_Id);
        if(deviceData != null){
          
            var data1=new Inventory { Specifications=specifications ,CreatedBy=deviceData.CreatedBy, D_State='U', };
            data1.D_Id = deviceData.D_Id;
            _context.Inventory.Add(data1);          
        }
        _context.SaveChanges();
        }
        return ("added success");
    }
<<<<<<< HEAD
    
  
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
=======

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Admin()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
>>>>>>> 73e46b73d1b515080faca1d39ceb332b13c82019
    }
}

    

    //         // Create HttpClient
    //         using (var client = _httpClientFactory.CreateClient())
    //         {
    //             client.BaseAddress = new Uri("http://localhost:5188"); // Replace with your API base URL
    //             // The above URL assumes the API is running on localhost and port 5000. Change it as needed.

    //             // Convert data to JSON
    //             var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

    //             // Make a POST request to the API endpoint
    //             var response = await client.PostAsync("/Home/Admin", jsonContent);
    //             response.EnsureSuccessStatusCode();

    //             var result = await response.Content.ReadFromJsonAsync<dynamic>();
    //             if (result.success)
    //             {
    //                 Console.Write("added");
    //             }
    //             else
    //             {
    //                 // TempData["ErrorMessage"] = "Error: " + result.message;
    //                 Console.Write("not added");
    //             }
    //         }
    //     }
    //     catch (HttpRequestException)
    //     {
    //         TempData["ErrorMessage"] = "Error adding device. API request failed.";
    //         return RedirectToAction(nameof(Index));
    //     }
    // }
    
    