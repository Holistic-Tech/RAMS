using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Rcsa.Web.Models
{
  [Table("UserDepartment")]
  public class UserDepartment
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserDepartmentId { get; set; }

    [ForeignKey("UsersProfile")]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual UserProfile UsersProfile { get; set; }

    [ForeignKey("DepartmentMasters")]
    public int DepartmentId { get; set; }
    [ForeignKey("DepartmentId")]
    public virtual DepartmentMaster DepartmentMasters { get; set; }


    //[ForeignKey("CompaniesMasters")]
    public int CompanyId { get; set; }
    //[ForeignKey("CompanyId")]
    //public virtual CompanyMaster CompaniesMasters { get; set; }

  }
}