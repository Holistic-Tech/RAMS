using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rcsa.Web.Models
{
  [Table("RiskMaster")]
  public class RiskMaster
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RiskId { get; set; }

    [Required(ErrorMessage = "Please enter risk name")]
    [StringLength(300)]
    public string RiskName { get; set; }

    public string Description { get; set; }

    public int? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public int? UpdateBy { get; set; }
    public DateTime? UpdateOn { get; set; }
  }
}