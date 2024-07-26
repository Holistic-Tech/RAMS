using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rcsa.Web.Models
{
  [Table("CompanyUser")]
  public class CompanyUser
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CompanyUserId { get; set; }
    //public int UserId { get; set; }
    //public int CompanyId { get; set; }

    [ForeignKey("UsersProfile")]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual UserProfile UsersProfile { get; set; }

    [ForeignKey("CompanyMasters")]
    public int CompanyId { get; set; }
    [ForeignKey("CompanyId")]
    public virtual CompanyMaster CompanyMasters { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
  }
}