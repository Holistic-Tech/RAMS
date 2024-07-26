using System;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Collections.Generic;

namespace Rcsa.Web.Models
{
  [Table("RiskDetails")]
    public class RiskDetailsModel
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RiskDetailID { get; set;}

    [ForeignKey("RiskAssessmentMaster")]
    public int? RiskAssessmentID { get; set; }
    [ForeignKey("RiskAssessmentId")]
    public virtual RiskAssessmentMaster RiskAssessmentMaster { get; set; }

    [ForeignKey("CompanyMaster")]
    public int? CompanyID { get; set; }
    [ForeignKey("CompanyId")]
    public virtual CompanyMaster CompanyMaster { get; set; }

    [ForeignKey("DepartmentMaster")]
    public int? DepartmentID { get; set;}
    [ForeignKey("DepartmentId")]
    public virtual DepartmentMaster DepartmentMaster { get; set; }

    [ForeignKey("RiskMaster")]
    public int? RiskID { get; set;}
    [ForeignKey("RiskId")]
    public virtual RiskMaster RiskMaster { get; set; }

    [ForeignKey("SubRiskMaster")]
    public int? SubRiskID { get; set;}
    [ForeignKey("SubRiskId")]
    public virtual SubRiskMaster SubRiskMaster { get; set; }

    [MaxLength(50)]
    public string SubRisk_Impact { get; set;}
    [MaxLength(50)]
    public string SubRisk_Likelyhood { get; set;}
    [MaxLength(50)]
    public string Inherenet_risk_rating { get; set;}

    [ForeignKey("MitigantMaster")]
    public int? MitigantID { get; set;}
    [ForeignKey("MitigantId")]
    public virtual MitigantMaster MitigantMaster { get; set; } 
    [MaxLength(10)]    
    public string Mitigant_Importance { get; set;}
    [MaxLength(8000)]    
    public string Mitigant_effectiveness { get; set;}
    [MaxLength(8000)]    
    public string Mitigant_whyEffective { get; set;}
    [MaxLength(8000)]    
    public string Issue { get; set;}
    [MaxLength(8000)]    
    public string Issue_Severity { get; set;}

    [MaxLength(3)]
    public string ActionPlanAvailable { get; set; }

    [MaxLength(3)]
    public string IsThisRiskBeingAccepted { get; set; }

    [MaxLength(3)]
    public string LossesAssociatedWithThisRisk { get; set; }

    [MaxLength(8000)]
    public string Losses { get; set; }

    [MaxLength(8000)]    
    public string ActionPlan { get; set;}

    public string TargetDate { get; set;}
    [MaxLength(8000)]    
    public string ActionPlan_Status { get; set;}
    [MaxLength(8000)]    
    public string Reason_Risk_acceptance { get; set;}
    [MaxLength(8000)]    
    public string List_risk_associated { get; set;}
    [MaxLength(8000)]    
    public string Owner { get; set;}
    [MaxLength(8000)]    
    public string Shared_process_Department { get; set;}
    [MaxLength(8000)]    
    public string Shared_process_Description { get; set;}

    public string CompanyObjectives { get; set; }
    [MaxLength(1)]
    public string Status { get; set; }

    public DateTime? CompletionDate { get; set; }

    public int? insertedBy { get; set;}
    public DateTime Insertedon { get; set;}
    [MaxLength(8000)]
    public string insertedmachineinfo { get; set; }
    public int? Updatedby { get; set;}
    public DateTime updatedon { get; set;}
    [MaxLength(8000)]    
    public string updatemachineinfo { get; set;}
  }
}