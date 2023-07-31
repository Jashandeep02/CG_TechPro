using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CG_TechPro.Models
{
    public class Users
    {
        [Key]
        public int U_Id { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }

    }
}