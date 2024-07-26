using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rcsa.Web.ReportForms
{
  public class StandardReportModel
  {
    public int RiskDetailID { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; }
    public int RiskID { get; set; }
    public int SubRiskID { get; set; }
    public string Risk { get; set; }
    public string subRisk { get; set; }
    public string Mitigant { get; set; }
    public string Mitigant_Importance { get; set; }
    public string MitigantEffectivness { get; set; }
    public string Issues { get; set; }
    public string ActionPlan { get; set; }
    public string TargetDate { get; set; }
    public string ActionPlanStatus { get; set; }
    public string Owner { get; set; }
    public string SubRisk_Impact { get; set; }
    public string SubRisk_Likelyhood { get; set; }
    public string Inherenet_risk_rating { get; set; }
    public int MitigantID { get; set; }
    public string Mitigant_whyEffective { get; set; }
    public string Issue_Severity { get; set; }
    public string ActionPlanAvailable { get; set; }
    public string Shared_process_Department { get; set; }
    public string Reason_Risk_acceptance { get; set; }
    public string Shared_process_Description { get; set; }
    public string CompanyObjectives { get; set; }
    public string RiskAssesmentName { get; set; }
    public string Status { get; set; }
    public DateTime InsertDate { get; set; }
    public string ReportType { get; set; }
    public string Lossess { get; set; }
    public string IsThisRiskBeingAccepted { get; set; }
    public string List_risk_associated { get; set; }
    public string Mitigant_description { get; set; }


  }
}