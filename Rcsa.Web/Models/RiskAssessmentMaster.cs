using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rcsa.Web.Models
{
  [Table("RiskAssessmentHeader")]
  public class RiskAssessmentMaster
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RiskAssessmentId { get; set; }

    [Required(ErrorMessage = "Please enter Risk Assessment name")]
    [StringLength(500)]
    public string RiskAssessmentName { get; set; }

    [ForeignKey("CompanyMaster")]
    public int? CompanyID { get; set; }
    [ForeignKey("CompanyId")]
    public virtual CompanyMaster CompanyMaster { get; set; }

    [ForeignKey("DepartmentMaster")]
    public int? DepartmentID { get; set; }
    [ForeignKey("DepartmentId")]
    public virtual DepartmentMaster DepartmentMaster { get; set; }

    [MaxLength(1)]
    public string Status { get; set; }

    public DateTime? InsertedDate { get; set; }
    public int? InsertedBy { get; set; }    
    public string Insertedmachineinfo { get; set; }
    public int? Updatedby { get; set; }
    public DateTime? Updatedon { get; set; }
    public string Updatemachineinfo { get; set; }
    

  }
}