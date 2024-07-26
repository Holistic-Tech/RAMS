using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rcsa.Web.ReportForms
{
  public class CustomizedReportModel
  {
    public string Risk { get; set; }
    public string subRisk { get; set; }
    public string Department { get; set; }
    public string Company { get; set; }
    public string Mitigant { get; set; }
    public string InherentRiskRating { get; set; }
    public string Issues { get; set; }
    public string IssueSaverity { get; set; }
    public string ActionPlan { get; set; }
    public string ActionPlanStatus { get; set; }
    public string Owner { get; set; }
    public string TargetDate { get; set; }

  }
}