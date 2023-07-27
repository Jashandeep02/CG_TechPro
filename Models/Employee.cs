using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CG_TechPro.Models
{
    public class Employee
    {
        [Key]
        public Guid Emp_Code { get; set; }

        public Guid U_Id { get; set; }

        public required string Name { get; set; }

        public DateTime CreatedAtUTC { get; set; }

        public DateTime UpdatedAtUTC { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public bool IsAdmin { get; set; }
    }
}