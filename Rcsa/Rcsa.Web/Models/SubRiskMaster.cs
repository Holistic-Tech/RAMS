using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Rcsa.Web.Models
{

  [Table("SubRiskMaster")]
  public class SubRiskMaster
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SubRiskId { get; set; }
    [Required(ErrorMessage = "Please enter sub risk name")]
    [StringLength(300)]
    public string SubRiskName { get; set; }
    public string SubRiskDesc { get; set; }

    [ForeignKey("RisksMaster")]
    public int RiskId { get; set; }
    [ForeignKey("RiskId")]
    public virtual RiskMaster RisksMaster { get; set; }

    public int? InsertedBy { get; set; }
    public DateTime? Insertedon { get; set; }
    public string Insertedmachineinfo { get; set; }
    public int? Updatedby { get; set; }
    public DateTime? Updatedon { get; set; }
    public string Updatemachineinfo { get; set; }
  }
}