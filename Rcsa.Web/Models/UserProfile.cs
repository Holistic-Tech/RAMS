using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rcsa.Web.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter user name")]
        [MaxLength(20)]
        public string UserName { get; set; }
        // [Required(ErrorMessage = "Please enter first name")]
        [MaxLength(50)]
        public string FirstName { get; set; }
        //  [Required(ErrorMessage = "Please enter last name")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter email id")]
        [StringLength(50)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter valid email id.")]
        public string Email { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsCompanySupervisor { get; set; }
        public bool? IsCompanyStaff { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateOn { get; set; }
    }
}