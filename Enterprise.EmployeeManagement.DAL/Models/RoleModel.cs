using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Enterprise.EmployeeManagement.DAL.Models
{
    public class RoleModel
    {
        [Key]
        public int RoleId { get; set; } // Primary key
        public string RoleName { get; set; } // Additional properties
    }
}
