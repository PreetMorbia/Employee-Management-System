using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Enterprise.EmployeeManagement.BAL.Enums;


namespace Enterprise.EmployeeManagement.BAL.CustomModels
{
    public class CustomSiteUserModel
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
        public string RoleName
        {
            get
            {
                return Enum.GetName(typeof(CustomEnums.RoleEnum), this.RoleId);
            }
        }
        public DateTime CreateDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        [StringLength(45)]
        public string LogInIpAddress { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        [Required]
        [PasswordPropertyText]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        public string FormattedCreateDate
        {
            get
            {
                return this.CreateDate.ToString("dd MMM yyy");
            }
        }
    }
}

