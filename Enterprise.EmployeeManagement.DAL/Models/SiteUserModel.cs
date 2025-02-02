using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.EmployeeManagement.DAL.Models
{
    
    public class SiteUserModel
    {
        [Key]
        public int SiteUserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [ForeignKey("RoleModel")]
        [Required]
        public int RoleId { get; set; }
        //public virtual RoleModel Role { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public DateTime LastLoginDate { get; set; }

        [StringLength(45)]
        public string LogInIpAddress { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        [Required]
        [PasswordPropertyText]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).+$", ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one special character.")]
        public string Password { get; set; }
    }
}
