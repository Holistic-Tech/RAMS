using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Linq;
using System.Web;

namespace Rcsa.Web.Models
{
  public class RiskManagementModel
  {
    public int RiskDetailID { get; set; }

    [ForeignKey("RiskAssessmentHeader")]
    public int? RiskAssessmentId { get; set; }
    [ForeignKey("RiskAssessmentId")]
    public virtual RiskAssessmentMaster RiskAssessmentHeader { get; set; }

    public int? CompanyId { get; set; }

    //[Required(ErrorMessage = "Please select Department...")]
    public int? DepartmentID { get; set; }
    public List<DepartmentMaster> Departments { get; set; }
    public string DepartmentName { get; set; }

    [Required(ErrorMessage = "Please select Risk...")]
    public int RiskID { get; set; }
    public List<RiskMaster> Risks { get; set; }

    public string RiskName { get; set; }
    public string RiskDescription { get; set; }

    [Required(ErrorMessage = "Please select Subrisk...")]
    public int SubRiskID { get; set; }
    public List<SubRiskMaster> SubRisks { get; set; }
    public string SubRiskName { get; set; }
    public string SubRiskDescription { get; set; }

    //[Required(ErrorMessage = "Please enter Subrisk Impact...")]
    [MaxLength(50)]
    public string SubRisk_Impact { get; set; }

    //[Required(ErrorMessage = "Please enter Subrisk Likelyhood...")]
    [MaxLength(50)]
    public string SubRisk_Likelyhood { get; set; }

    //[Required(ErrorMessage = "Please enter Inherent Risk Rating...")]
    [MaxLength(50)]
    public string Inherenet_risk_rating { get; set; }

    [Required(ErrorMessage = "Please select Mitigant...")]
    public int MitigantID { get; set; }
    public List<MitigantMaster> Mitigants { get; set; }
    public string MitigantName { get; set; }
    public string MitigantDescription { get; set; }

    [DataType(DataType.Date)]
    public string CompletionDate { get; set; }

    //[Required(ErrorMessage = "Please enter Mitigant Importance...")]
    [MaxLength(10)]
    public string Mitigant_Importance { get; set; }

    //[Required(ErrorMessage = "Please enter Mitigant Effectiveness...")]
    [MaxLength(8000)]
    public string Mitigant_effectiveness { get; set; }

    //[Required(ErrorMessage = "Please enter Why Mitigant Effective...")]
    [MaxLength(8000)]
    public string Mitigant_whyEffective { get; set; }

    //[Required(ErrorMessage = "Please enter Issue...")]
    [MaxLength(8000)]
    public string Issue { get; set; }

    //[Required(ErrorMessage = "Please enter Issue Severity...")]
    [MaxLength(8000)]
    public string Issue_Severity { get; set; }

    //[Required(ErrorMessage = "Please select if Action-Plan Available...")]
    [MaxLength(3)]
    public string ActionPlanAvailable { get; set; }

    [MaxLength(3)]
    public string IsThisRiskBeingAccepted { get; set; }

    [MaxLength(3)]
    public string LossesAssociatedWithThisRisk { get; set; }

    [MaxLength(8000)]
    public string Losses { get; set; }

    //[Required(ErrorMessage = "Please enter Action Plan...")]
    [MaxLength(8000)]
    public string ActionPlan { get; set; }

    //[Required(ErrorMessage = "Please select Target Date...")]
    [DataType(DataType.Date)]
    public string TargetDate { get; set; }

    //[Required(ErrorMessage = "Please enter Status of Action Plan...")]
    [MaxLength(8000)]
    public string ActionPlan_Status { get; set; }

    //[Required(ErrorMessage = "Please enter Reason of Risk Acceptance...")]
    [MaxLength(8000)]
    public string Reason_Risk_acceptance { get; set; }

    //[Required(ErrorMessage = "Please enter Associated Risk...")]
    [MaxLength(8000)]
    public string List_risk_associated { get; set; }

    //[Required(ErrorMessage = "Please enter Owner...")]
    [MaxLength(8000)]
    public string Owner { get; set; }

    //[Required(ErrorMessage = "Please select Shared Process Department...")]
    //[MaxLength(8000)]
    public string Shared_process_Department { get; set; }
    public List<DepartmentMaster> Shared_process_Departments { get; set; }

    //[Required(ErrorMessage = "Please enter Shared Process Description...")]
    [MaxLength(8000)]
    public string Shared_process_Description { get; set; }

    public string CompanyObjectives { get; set; }

    public int insertedBy { get; set; }
    public DateTime Insertedon { get; set; }
    public string insertedmachineinfo { get; set; }
    public int? Updatedby { get; set; }
    public DateTime updatedon { get; set; }
    public string updatemachineinfo { get; set; }



    public IEnumerable<SelectListItem> LevelOfSeverityImpacts = new List<SelectListItem>{
                            new SelectListItem { Value = "VH", Text = "VH" },
                            new SelectListItem { Value = "H", Text = "H" },
                            new SelectListItem { Value = "M", Text = "M" },
                            new SelectListItem { Value = "L", Text = "L" },
                            new SelectListItem { Value = "VL", Text = "VL" }
                          };
    public IEnumerable<SelectListItem> MitigantEffectivenessOptions = new List<SelectListItem>{
                            new SelectListItem { Value = "Effective", Text = "Effective" },
                            new SelectListItem { Value = "Opportunity for Improvement", Text = "Opportunity for Improvement" },
                            new SelectListItem { Value = "Not Effective", Text = "Not Effective" },
                            new SelectListItem { Value = "Not Existent", Text = "Not Existent" }
                          };
    public IEnumerable<SelectListItem> ActionPlanAvailableOptions = new List<SelectListItem>{
                            new SelectListItem { Value = "Yes", Text = "Yes" },
                            new SelectListItem { Value = "No", Text = "No" }
                          };

    public IEnumerable<SelectListItem> IsThisRiskBeingAcceptedOption = new List<SelectListItem>{
                            new SelectListItem { Value = "N/A", Text = "N/A" },
                            new SelectListItem { Value = "Yes", Text = "Yes" },
                            new SelectListItem { Value = "No", Text = "No" }
                          };

    public IEnumerable<SelectListItem> InherentRiskRatingOptions = new List<SelectListItem>{
                            new SelectListItem { Value = "VH", Text = "VH" },
                            new SelectListItem { Value = "H", Text = "H" },
                            new SelectListItem { Value = "M", Text = "M" },
                           new SelectListItem { Value = "L", Text = "L" },
                            new SelectListItem { Value = "VL", Text = "VL" }
                          };

    public IEnumerable<SelectListItem> issueSeverity = new List<SelectListItem>{
                             new SelectListItem { Value = "N/A", Text = "N/A" },
                            new SelectListItem { Value = "VH", Text = "VH" },
                            new SelectListItem { Value = "H", Text = "H" },
                            new SelectListItem { Value = "M", Text = "M" },
                            new SelectListItem { Value = "L", Text = "L" },
                            new SelectListItem { Value = "VL", Text = "VL" }
                          };
  }
}