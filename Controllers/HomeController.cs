using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using final.Models;
using CG_TechPro.Data;
using System.Security.Claims;
using CG_TechPro.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Authentication;

namespace final.Controllers
{
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

    [HttpPost]
    [Route("/Home/Index")]
    public ActionResult Login(Users user)
    {
        if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
        {
            return View("Index");
        }

        string desiredDomain = "@cginfinity.com";
        if (!user.UserName.EndsWith(desiredDomain, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid domain.");
        }

        var users = _context.Users.FirstOrDefault(u => u.UserName == user.UserName);
        if (users == null)
        {
            return BadRequest("Invalid user");
        }

        bool VerifyPass = BCrypt.Net.BCrypt.Verify(user.Password, users.Password);
        if (!VerifyPass)
        {
            return BadRequest("Invalid password");
        }

        var employee = _context.Employee.FirstOrDefault(e => e.U_Id == users.U_Id);
        if (employee == null)
        {
            return BadRequest("Employee record not found");
        }

        var token = GenerateJwtToken(users);
        _accessor.HttpContext.Session.SetString("jwtToken", token);
        var Id = users.U_Id;
        _accessor.HttpContext.Session.SetString("UserId", Id.ToString());
        var Name = users.UserName;
        _accessor.HttpContext.Session.SetString("Name", Name);

        if (employee.IsAdmin)
        {
            return RedirectToAction("Admin");
        }
        else
        {
            return RedirectToAction("User");
        }
    }

        [HttpPost]
        [Route("Admit")]
        public IActionResult PostUser([FromBody] Users user)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string HashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);
            var users = new Users
            {
                UserName = user.UserName,
                Password = HashedPassword,
            };
            _context.Users.Add(users);
            _context.SaveChanges();
            return Ok();
        }


        [HttpPost]
        [Route("employee")]
        public IActionResult PostE(string name, string createdBy, bool isAdmin)
        {
            var data = new Employee { Name = name, CreatedBy = createdBy, IsAdmin = isAdmin };
            _context.Employee.Add(data);
            _context.SaveChanges();
            return Ok();

        }

        [HttpPost("/Home/Admin")]
        public JsonResult addDevice(string deviceType, int count, string specifications)
        {
            string Id = _accessor.HttpContext.Session.GetString("UserId");
            var data = new Devices { D_Type = deviceType, CreatedAtUTC = DateTime.Now, UpdatedAtUTC = DateTime.Now, CreatedBy = Id };
            _context.Devices.Add(data);
            for (int i = 0; i < count; i++)
            {
                var deviceData = _context.Devices.Find(data.D_Id);
                if (deviceData != null)
                {
                    Guid serial = Guid.NewGuid();
                    Guid Iid = Guid.NewGuid();
                    var data1 = new Inventory { I_Id = Iid, Specifications = specifications, CreatedBy = deviceData.CreatedBy, D_State = 'U', Serial = serial };
                    data1.D_Id = deviceData.D_Id;
                    _context.Inventory.Add(data1);
                }
                _context.SaveChanges();
            }
            return Json(new { success = true });
        }

        [HttpGet]
        [Route("/Home/Admin/ShowDevices")]
        public IActionResult GetDevices()
        {
            var devices = (from d in _context.Devices
                           join i in _context.Inventory on d.D_Id equals i.D_Id
                           select new
                           {
                               serialNo = i.Serial,
                               inventoryId = i.Id,
                               specification = i.Specifications,
                               deviceType = d.D_Type,
                           }).ToList();
            return Json(devices);

        }

        [HttpGet]
        [Route("/Home/Admin/ShowAssignedDevices")]
        public IActionResult getAssignedDevices()
        {
            var assignedDevices = (from i in _context.Inventory
                                join d in _context.Devices on i.D_Id equals d.D_Id
                                where i.D_State == 'A' // Filter only assigned devices
                                select new
                                {
                                        serialNo = i.Serial,
                                        inventoryId = i.Id,
                                        specification=i.Specifications,
                                        deviceType = d.D_Type,
                                }).ToList();
            return Json(assignedDevices);
        }

        [HttpGet]
        [Route("/Home/Admin/ShowUnassignedDevices")]
        public IActionResult getUnassignedDevices()
        {
            var assignedDevices = (from i in _context.Inventory
                                join d in _context.Devices on i.D_Id equals d.D_Id
                                where i.D_State == 'U' // Filter only assigned devices
                                select new
                                {
                                    serialNo = i.Serial,
                                    inventoryId = i.Id,
                                    specification=i.Specifications,
                                    deviceType = d.D_Type,
                                }).ToList();
            return Json(assignedDevices);
        }
        [HttpGet]
        [Route("/Home/Admin/ShowRepairDevices")]
        public IActionResult getRepairDevices()
        {
            var assignedDevices = (from i in _context.Inventory
                                join d in _context.Devices on i.D_Id equals d.D_Id
                                where i.D_State == 'R' // Filter only assigned devices
                                select new
                                {
                                    serialNo = i.Serial,
                                    inventoryId = i.Id,
                                    specification=i.Specifications,
                                    deviceType = d.D_Type,
                                }).ToList();
            return Json(assignedDevices);
        }

        [HttpGet]
        [Route("/Home/Admin/ShowDevicesByType")]
        public IActionResult GetDevicesByType(string deviceType)
        {
            if (string.IsNullOrEmpty(deviceType))
            {
                return BadRequest("Device type cannot be empty.");
            }
            var devices = (from i in _context.Inventory
                    join d in _context.Devices on i.D_Id equals d.D_Id
                    where d.D_Type.ToLower() == deviceType.ToLower()
                    select new
                    {
                            deviceType = d.D_Type,
                        specification = i.Specifications,
                        inventoryId = i.Id,
                        serialNo = i.Serial
                    }).ToList();
            if (devices.Count == 0)
            {
                return NotFound("No devices found for the specified device type.");
            }
            return Json(devices);
        }

        [HttpGet]
        [Route("/Home/Admin/ShowAssignedDevicesByType")]
        public IActionResult getAssignedDevicesByType(string deviceType)
        {
            if (string.IsNullOrEmpty(deviceType))
            {
               return BadRequest("Device type cannot be empty.");
            }
            var assignedDevices = (from i in _context.Inventory
                            join d in _context.Devices on i.D_Id equals d.D_Id
                            where d.D_Type.ToLower() == deviceType.ToLower()
                                    && i.D_State == 'A' // Filter only assigned devices
                            select new
                            {
                                serialNo = i.Serial,
                                inventoryId = i.Id,
                                specification = i.Specifications,
                                deviceType = d.D_Type,
                            }).ToList();
            return Json(assignedDevices);
        }
        
        [HttpGet]
        [Route("/Home/Admin/ShowUnassignedDevicesByType")]
        public IActionResult getUnassignedDevicesByType(string deviceType)
        {
            if (string.IsNullOrEmpty(deviceType))
            {
               return BadRequest("Device type cannot be empty.");
            }
            var unassignedDevices = (from i in _context.Inventory
                                join d in _context.Devices on i.D_Id equals d.D_Id
                                where d.D_Type.ToLower() == deviceType.ToLower()
                                    && i.D_State == 'U' // Filter only unassigned devices
                                select new
                                {
                                    serialNo = i.Serial,
                                    inventoryId = i.Id,
                                    specification = i.Specifications,
                                    deviceType = d.D_Type,
                                }).ToList();
            return Json(unassignedDevices);
        }
        
        [HttpGet]
        [Route("/Home/Admin/ShowRepairDevicesByType")]
        public IActionResult getRepairDevicesByType(string deviceType)
        {
            if (string.IsNullOrEmpty(deviceType))
            {
                return BadRequest("Device type cannot be empty.");
            }
        var repairDevices = (from i in _context.Inventory
                            join d in _context.Devices on i.D_Id equals d.D_Id
                            where d.D_Type.ToLower() == deviceType.ToLower()
                                && i.D_State == 'R' // Filter only repair devices
                            select new
                            {
                                serialNo = i.Serial,
                                inventoryId = i.Id,
                                specification = i.Specifications,
                                deviceType = d.D_Type,
                            }).ToList();
            return Json(repairDevices);
        }

 


        [HttpPost]
        [Route("/Home/Admin/Assigned")]
        public JsonResult AssignDeviceToEmployee([FromBody] AssignDeviceRequest assign)
        {
            string user = _accessor.HttpContext.Session.GetString("UserId");
            var employee = _context.Employee.Find(assign.empCode);
            var inventory = _context.Inventory.FirstOrDefault(i => i.Id == assign.id && i.D_State == 'U');

            if (employee == null || inventory == null)
            {
                return Json(new { success = false, message = "Employee or inventory not found" });
            }
            // Create an entry in the Assigned table to track the assignment
            inventory.D_State = 'A';
            var assigned = new Assigned
            {
                Emp_Code = assign.empCode,
                Id = assign.id,
                AssignedBy = user,
                AssignedAtUTC = DateTime.Now
            };
            _context.Inventory.Update(inventory);
            _context.Assigned.Add(assigned);
            _context.SaveChanges();

            return Json(new { success = true }); ;
        }

        

        [HttpGet]
        [Route("/Home/Admin/GetAssignedDevices/{empCode}")]
        public IActionResult GetAssignedDevices(int empCode)
        {
            var assignedDevices = (from a in _context.Assigned
                                   join i in _context.Inventory on a.Id equals i.Id
                                   join d in _context.Devices on i.D_Id equals d.D_Id
                                   where a.Emp_Code == empCode
                                   select new
                                   {
                                       DeviceType = d.D_Type,
                                       Specifications = i.Specifications,
                                       InventoryId = i.Id,
                                       SerialNo = i.Serial
                                   }).ToList();
            return Json(assignedDevices);
        }


        [HttpGet]
        [Route("/Home/Admin/logout")]
        public IActionResult Logout()
 
        {
            _accessor.HttpContext.Session.Clear();
            return Ok(new { message = "Logout successful." });
        }


        [HttpGet]
        [Route("/Home/Admin/GetUserDevices")]
        public IActionResult GetUserDevices()
        {
           string userIdStr = _accessor.HttpContext.Session.GetString("UserId");
    // Convert the user ID to an integer
        if (!int.TryParse(userIdStr, out int userId))
        {
            // Handle invalid user ID if necessary
            return BadRequest("Invalid user ID");
        }
        // Fetch the devices assigned to the user
        var devices = (from e in _context.Employee
                    join a in _context.Assigned on e.Emp_Code equals a.Emp_Code
                    join i in _context.Inventory on a.Id equals i.Id
                    join d in _context.Devices on i.D_Id equals d.D_Id
                    where e.U_Id == userId
                    select new
                    {
                        serialNo = i.Serial,
                        deviceType = d.D_Type,
                        specification = i.Specifications
                    }).ToList();
                    return Json(devices);
        }

        [HttpGet]
        [Route("/Home/test/devices")]
        public ActionResult<IEnumerable<string>> GetDevices(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                // If the search keyword is empty or null, return an empty list
                return new List<string>();
            }
            // Perform a case-insensitive search on the mock devices data
          var matchingDevices = _context.Devices
                .AsEnumerable()
                .Where(device => device.D_Type.Contains(search, StringComparison.OrdinalIgnoreCase) )
                .Select(device => device.D_Type)
                .ToList();
            return Json(matchingDevices);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
