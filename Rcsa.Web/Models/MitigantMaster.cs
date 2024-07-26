using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rcsa.Web.Models
{
  [Table("MitigantMaster")]
  public class MitigantMaster
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MitigantId { get; set; }
    [Required(ErrorMessage = "Please enter mitigant name")]
    [StringLength(300)]
    public string MitigantName { get; set; }
    public string MitigantDesc { get; set; }


    [ForeignKey("SubRisksMaster")]
    public int SubRiskId { get; set; }
    [ForeignKey("SubRiskId")]
    public virtual SubRiskMaster SubRisksMaster { get; set; }

    public int? InsertedBy { get; set; }
    public DateTime? Insertedon { get; set; }
    public string Insertedmachineinfo { get; set; }
    public int? Updatedby { get; set; }
    public DateTime? Updatedon { get; set; }
    public string Updatemachineinfo { get; set; }
  }
}