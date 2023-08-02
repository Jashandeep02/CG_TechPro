using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CG_TechPro.Models
{
    public class Assigned
    {
        [Key]
        public int A_Id { get; set; }

        public int Emp_Code { get; set; }

        public  int Id { get; set; }

         public DateTime? AssignedAtUTC { get; set; }

         public required string AssignedBy { get; set; }
    }
}