using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CG_TechPro.Models
{
    public class Inventory
    {
        [Key]
        public Guid I_Id { get; set; }

        public Guid D_Id { get; set; }

        public required string  Serial_No { get; set; } 

        public int CreatedBy { get; set; }

        public char D_State { get; set; }
    }
}