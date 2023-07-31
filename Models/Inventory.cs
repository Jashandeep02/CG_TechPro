using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CG_TechPro.Models
{
    public class Inventory
    {
        public Guid I_Id { get; set; }
        [Key]
        public int Id { get; set; }

        public Guid D_Id { get; set; }

        public Guid Serial { get; set; } 

        public required string Specifications { get; set; }
        public required string CreatedBy { get; set; }

        public char D_State { get; set; }
    }
}