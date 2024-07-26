using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Rcsa.Web.Models;
using WebMatrix.WebData;
using System.Data.Objects.SqlClient;
using System.ComponentModel.DataAnnotations;
using Rcsa.Web.ReportForms;
using LinqKit;
using System.Linq.Expressions;
using System.Web.Security;

namespace Rcsa.Web.Controllers
{
  [Authorize]
  public class StandardController : Controller
  {
    //
    // GET: /Standard/
    RcsaDb db = new RcsaDb();
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult StandardReports()
    {
      var reportList = new SelectList(new[]
                                          {
                                              new {ID="1",Name="Has Action Plan"},
                                              new{ID="2",Name="Very High or High Inherent Risk Rating and Mitigant Importance"},
                                              new{ID="3",Name="Issue Severity: High"},
                                              new{ID="4",Name="Issue Severity:Very High"},
                                              new{ID="5",Name="Mitigant Effectiveness: Effective"},
                                              new{ID="6",Name="Subrisk Impact: High"},
                                              new{ID="7",Name="Subrisk Impact: Very High"},
                                              new{ID="8",Name="Subrisk Likelihood: High"},
                                              new{ID="9",Name="Subrisk Likelihood: Very High"},
                                              new{ID="10",Name="Accepted Risk Report"},
                                              new{ID="11",Name="Loss Reporting Report"},
                                              new{ID="12",Name="Risk Assessment Report"},
                                          },
                    "ID", "Name", 1);
      ViewData["list"] = reportList;
      List<RiskManagementModel> list = new List<RiskManagementModel>();
      var roles = Roles.GetRolesForUser().Single();
      if (roles.ToLower() == "Company")
      {



        var companyUserId = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();
        var riskDetail = db.RiskDetails.Where(x => x.CompanyID == companyUserId.CompanyId).ToList();
        foreach (var riskDetailResult in riskDetail)
        {
          if (riskDetailResult != null && riskDetailResult.RiskDetailID > 0)
          {
            var model = new RiskManagementModel();
            model.RiskDetailID = riskDetailResult.RiskDetailID;
            model.CompanyId = riskDetailResult.CompanyID ?? 0;
            model.DepartmentID = riskDetailResult.DepartmentID ?? 0;
            model.RiskID = riskDetailResult.RiskID ?? 0;
            model.RiskName = riskDetailResult.RiskMaster.RiskName;
            model.SubRiskID = riskDetailResult.SubRiskID ?? 0;
            model.SubRiskName = riskDetailResult.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetailResult.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetailResult.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetailResult.Inherenet_risk_rating;
            model.MitigantID = riskDetailResult.MitigantID ?? 0;
            model.MitigantName = riskDetailResult.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetailResult.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetailResult.Mitigant_effectiveness;
            model.Issue = riskDetailResult.Issue;
            model.Issue_Severity = riskDetailResult.Issue_Severity;
            model.ActionPlanAvailable = riskDetailResult.ActionPlanAvailable;
            model.ActionPlan = riskDetailResult.ActionPlan;
            model.TargetDate = riskDetailResult.TargetDate.ToString();
            model.ActionPlan_Status = riskDetailResult.ActionPlan_Status;
            model.List_risk_associated = riskDetailResult.List_risk_associated;
            model.Owner = riskDetailResult.Owner;

            list.Add(model);
          }
        }
      }
      else
      {
        var riskDetails = db.RiskDetails.ToList();
        foreach (var riskDetail in riskDetails)
        {
          if (riskDetails.Count > 0)
          {
            var model = new RiskManagementModel();
            model = new RiskManagementModel();
            model.RiskDetailID = riskDetail.RiskDetailID;
            model.CompanyId = riskDetail.CompanyID ?? 0;
            model.DepartmentID = riskDetail.DepartmentID ?? 0;
            model.RiskID = riskDetail.RiskID ?? 0;
            model.RiskName = riskDetail.RiskMaster.RiskName;
            model.SubRiskID = riskDetail.SubRiskID ?? 0;
            model.SubRiskName = riskDetail.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetail.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
            model.MitigantID = riskDetail.MitigantID ?? 0;
            model.MitigantName = riskDetail.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetail.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
            model.Issue = riskDetail.Issue;
            model.Issue_Severity = riskDetail.Issue_Severity;
            model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
            model.ActionPlan = riskDetail.ActionPlan;
            model.TargetDate = riskDetail.TargetDate.ToString();
            model.ActionPlan_Status = riskDetail.ActionPlan_Status;
            model.List_risk_associated = riskDetail.List_risk_associated;
            model.Owner = riskDetail.Owner;
            list.Add(model);
          }
        }
      }
      return View();
    }

    [HttpPost]
    public string ReportResult(string compId, string deptId, string reportType, string value)
    {
      string res = "0";

      int companyId = 0;
      if (compId != "Select Company" && compId != "")
      {
        companyId = Convert.ToInt32(compId);
      }
      int departmentId = 0;
      if (deptId != "Select Department" && deptId != "")
      {
        departmentId = Convert.ToInt32(deptId);
      }
      List<StandardReportModel> list = new List<StandardReportModel>();
      var roles = Roles.GetRolesForUser().Single();
      var lstRiskDetalModel = new List<RiskDetailsModel>();

      List<RiskDetailsModel> riskDetailsData;
      List<CompanyMaster> companiesList = new List<CompanyMaster>();
      if (User.IsInRole("Admin"))
      {

        if (reportType == "1")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.ActionPlanAvailable == "Yes").ToList();

        }
        else if (reportType == "2")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId &&
            (x.Mitigant_Importance == "VH" || x.Mitigant_Importance == "H") && (x.Inherenet_risk_rating == "VH" || x.Inherenet_risk_rating == "H")).ToList();
        }
        else if (reportType == "3")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Issue_Severity == "H").ToList();
        }
        else if (reportType == "4")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Issue_Severity == "VH").ToList();
        }
        else if (reportType == "5")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId
              && x.Mitigant_effectiveness == "Effective").ToList();
        }
        else if (reportType == "6")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Impact == "H").ToList();
        }
        else if (reportType == "7")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Impact == "VH").ToList();
        }
        else if (reportType == "8")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "H").ToList();
        }
        else if (reportType == "9")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "VH").ToList();
        }
        else if (reportType == "10")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "C").ToList();
        }
        else if (reportType == "11")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
        }
        else if (reportType == "12")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        }
        else
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        }

        if (riskDetailsData.Count == 0 && companyId == 0 && departmentId != 0)
        {


          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId).ToList();

          foreach (var items1 in riskDetailsData)
          {
            //items1.Issue_Severity
            lstRiskDetalModel.Add(items1);
          }
        }

        if (riskDetailsData.Count == 0 && companyId != 0 && departmentId == 0)
        {


          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId).ToList();

          foreach (var items1 in riskDetailsData)
          {
            //items1.Issue_Severity
            lstRiskDetalModel.Add(items1);
          }
        }



        if (riskDetailsData.Count == 0)
        {
          var companies = db.CompaniesMaster.ToList();
          foreach (var item in companies)
          {
            riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId).ToList();

            foreach (var items1 in riskDetailsData)
            {
              //items1.Issue_Severity
              lstRiskDetalModel.Add(items1);
            }
          }
        }
        else//if (riskDetailsData != null)
        {
          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);
          }
        }
        //}
      }
      if (User.IsInRole("Consultant"))
      {

        if (reportType == "1")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.ActionPlanAvailable == "Yes").ToList();
        }
        else if (reportType == "2")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId
              && (x.Mitigant_Importance == "VH" || x.Mitigant_Importance == "H")
              && (x.Inherenet_risk_rating == "VH" || x.Inherenet_risk_rating == "H")).ToList();
        }
        else if (reportType == "3")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Issue_Severity == "H").ToList();
        }
        else if (reportType == "4")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Issue_Severity == "VH").ToList();
        }
        else if (reportType == "5")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Mitigant_effectiveness == "Effective").ToList();
        }
        else if (reportType == "6")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Impact == "H").ToList();
        }
        else if (reportType == "7")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Impact == "VH").ToList();
        }
        else if (reportType == "8")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "H").ToList();
        }
        else if (reportType == "9")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "VH").ToList();
        }
        else if (reportType == "10")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "C").ToList();
        }
        else if (reportType == "11")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
        }
        else if (reportType == "12")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        }
        else
        { riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList(); }


        if (riskDetailsData.Count == 0 && companyId == 0 && departmentId != 0)
        {


          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId).ToList();

          foreach (var items1 in riskDetailsData)
          {
            //items1.Issue_Severity
            lstRiskDetalModel.Add(items1);
          }
        }

        if (riskDetailsData.Count == 0 && companyId != 0 && departmentId == 0)
        {


          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId).ToList();

          foreach (var items1 in riskDetailsData)
          {
            //items1.Issue_Severity
            lstRiskDetalModel.Add(items1);
          }
        }



        if (riskDetailsData.Count == 0)
        {
          var companies = db.CompaniesMaster.ToList();
          foreach (var item in companies)
          {
            riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId).ToList();

            foreach (var items1 in riskDetailsData)
            {
              //items1.Issue_Severity
              lstRiskDetalModel.Add(items1);
            }
          }
        }
        else//if (riskDetailsData != null)
        {
          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);
          }
        }
        // }
      }

      if (User.IsInRole("Company"))
      {

        if (reportType == "1")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.ActionPlanAvailable == "Yes" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "2")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && (x.Mitigant_Importance == "VH" || x.Mitigant_Importance == "H") && (x.Inherenet_risk_rating == "VH" || x.Inherenet_risk_rating == "H") && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "3")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.Issue_Severity == "H" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "4")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.Issue_Severity == "VH" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "5")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId &&
              x.Mitigant_effectiveness == "Effective" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "6")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.SubRisk_Impact == "H" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "7")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.SubRisk_Impact == "VH" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "8")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "H" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "9")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "VH" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "10")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "C").ToList();
        }
        else if (reportType == "11")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
        }
        else if (reportType == "12")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        }
        else
        { riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.CompanyID == companyId).ToList(); }
        if (riskDetailsData.Count == 0)
        {
          var companies = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).ToList();
          foreach (var item in companies)
          {
            riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId).ToList();
            foreach (var items in riskDetailsData)
            {
              lstRiskDetalModel.Add(items);
            }
          }
        }
        else//    if (riskDetailsData != null)
        {
          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);
          }
        }
        // }
      }

      if (lstRiskDetalModel != null && lstRiskDetalModel.Count > 0)
      {
        foreach (var item in lstRiskDetalModel)
        {
          var lstDetails = new StandardReportModel();
          lstDetails.RiskDetailID = Convert.ToInt32(item.RiskDetailID);
          lstDetails.CompanyId = Convert.ToInt32(item.CompanyID);
          lstDetails.CompanyName = item.CompanyMaster.ComapnyName;
          lstDetails.DepartmentID = Convert.ToInt32(item.DepartmentID);
          lstDetails.DepartmentName = item.DepartmentMaster.DepartmentName;
          lstDetails.SubRisk_Impact = item.SubRisk_Impact;
          lstDetails.SubRisk_Likelyhood = item.SubRisk_Likelyhood;
          lstDetails.Inherenet_risk_rating = item.Inherenet_risk_rating;
          lstDetails.MitigantID = Convert.ToInt32(item.MitigantID);
          lstDetails.Mitigant_whyEffective = item.Mitigant_whyEffective;
          lstDetails.Issue_Severity = item.Issue_Severity;
          lstDetails.ActionPlanAvailable = item.ActionPlanAvailable;
          lstDetails.Shared_process_Department = item.Shared_process_Department;
          lstDetails.Reason_Risk_acceptance = item.Reason_Risk_acceptance;
          lstDetails.Shared_process_Description = item.Shared_process_Description;
          lstDetails.CompanyObjectives = item.CompanyObjectives;
          lstDetails.Mitigant_Importance = item.Mitigant_Importance;
          lstDetails.RiskID = Convert.ToInt32(item.RiskID);
          lstDetails.SubRiskID = Convert.ToInt32(item.SubRiskID);
          lstDetails.Risk = item.RiskMaster.RiskName;
          lstDetails.subRisk = item.SubRiskMaster.SubRiskName;
          lstDetails.Mitigant = item.MitigantMaster.MitigantName;
          lstDetails.MitigantEffectivness = item.Mitigant_effectiveness;
          lstDetails.Issues = item.Issue;
          lstDetails.ActionPlan = item.ActionPlan;
          lstDetails.ActionPlanStatus = item.ActionPlan_Status;
          lstDetails.Owner = item.Owner;
          lstDetails.TargetDate = item.TargetDate.ToString();
          lstDetails.IsThisRiskBeingAccepted = item.IsThisRiskBeingAccepted;
          lstDetails.List_risk_associated = item.List_risk_associated;

          if (value == "Has Action Plan")
          {
            value = "Action Plan";
          }
          if (item.Status == "C")
          {
            lstDetails.Status = "Approved";
          }
          else
          {
            lstDetails.Status = "In Progress";
          }
          lstDetails.ReportType = value;

          list.Add(lstDetails);
        }
      }

      if (lstRiskDetalModel.Count > 0)
      {
        if (reportType == "10")
        {
          this.HttpContext.Session["ReportName"] = "AcceptedReport.rpt";
          this.HttpContext.Session["rptSource"] = list;
          res = "1";
        }
        else if (reportType == "11")
        {
          this.HttpContext.Session["ReportName"] = "LossReport.rpt";
          this.HttpContext.Session["rptSource"] = list;
          res = "1";

        }
        else if (reportType == "12")
        {
          this.HttpContext.Session["ReportName"] = "RiskAssessmentReport.rpt";
          this.HttpContext.Session["rptSource"] = list;
          res = "1";

        }
        else
        {
          this.HttpContext.Session["ReportName"] = "ReportType.rpt";
          this.HttpContext.Session["rptSource"] = list;
          res = "1";

        }
      }
      return res;
    }

    //public ActionResult LossReport()
    //{
    //  List<RiskManagementModel> list = new List<RiskManagementModel>();
    //  var roles = Roles.GetRolesForUser().Single();
    //  if (roles.ToLower() == "Company")
    //  {
    //    var companyUserId = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();
    //    var riskDetail = db.RiskDetails.Where(x => x.CompanyID == companyUserId.CompanyId).ToList();
    //    foreach (var riskDetailResult in riskDetail)
    //    {
    //      if (riskDetailResult != null && riskDetailResult.RiskDetailID > 0)
    //      {
    //        var model = new RiskManagementModel();
    //        model.RiskDetailID = riskDetailResult.RiskDetailID;
    //        model.CompanyId = riskDetailResult.CompanyID ?? 0;
    //        model.DepartmentID = riskDetailResult.DepartmentID ?? 0;
    //        model.RiskID = riskDetailResult.RiskID ?? 0;
    //        model.RiskName = riskDetailResult.RiskMaster.RiskName;
    //        model.SubRiskID = riskDetailResult.SubRiskID ?? 0;
    //        model.SubRiskName = riskDetailResult.SubRiskMaster.SubRiskName;
    //        model.SubRisk_Impact = riskDetailResult.SubRisk_Impact;
    //        model.SubRisk_Likelyhood = riskDetailResult.SubRisk_Likelyhood;
    //        model.Inherenet_risk_rating = riskDetailResult.Inherenet_risk_rating;
    //        model.MitigantID = riskDetailResult.MitigantID ?? 0;
    //        model.MitigantName = riskDetailResult.MitigantMaster.MitigantName;
    //        model.Mitigant_Importance = riskDetailResult.Mitigant_Importance;
    //        model.Mitigant_effectiveness = riskDetailResult.Mitigant_effectiveness;
    //        model.Issue = riskDetailResult.Issue;
    //        model.Issue_Severity = riskDetailResult.Issue_Severity;
    //        model.ActionPlanAvailable = riskDetailResult.ActionPlanAvailable;
    //        model.ActionPlan = riskDetailResult.ActionPlan;
    //        model.TargetDate = riskDetailResult.TargetDate;
    //        model.ActionPlan_Status = riskDetailResult.ActionPlan_Status;
    //        model.List_risk_associated = riskDetailResult.List_risk_associated;
    //        model.Owner = riskDetailResult.Owner;
    //        list.Add(model);
    //      }
    //    }
    //  }
    //  else
    //  {
    //    var riskDetails = db.RiskDetails.ToList();
    //    foreach (var riskDetail in riskDetails)
    //    {
    //      if (riskDetails.Count > 0)
    //      {
    //        var model = new RiskManagementModel();
    //        model = new RiskManagementModel();
    //        model.RiskDetailID = riskDetail.RiskDetailID;
    //        model.CompanyId = riskDetail.CompanyID ?? 0;
    //        model.DepartmentID = riskDetail.DepartmentID ?? 0;
    //        model.RiskID = riskDetail.RiskID ?? 0;
    //        model.RiskName = riskDetail.RiskMaster.RiskName;
    //        model.SubRiskID = riskDetail.SubRiskID ?? 0;
    //        model.SubRiskName = riskDetail.SubRiskMaster.SubRiskName;
    //        model.SubRisk_Impact = riskDetail.SubRisk_Impact;
    //        model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
    //        model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
    //        model.MitigantID = riskDetail.MitigantID ?? 0;
    //        model.MitigantName = riskDetail.MitigantMaster.MitigantName;
    //        model.Mitigant_Importance = riskDetail.Mitigant_Importance;
    //        model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
    //        model.Issue = riskDetail.Issue;
    //        model.Issue_Severity = riskDetail.Issue_Severity;
    //        model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
    //        model.ActionPlan = riskDetail.ActionPlan;
    //        model.TargetDate = riskDetail.TargetDate;
    //        model.ActionPlan_Status = riskDetail.ActionPlan_Status;
    //        model.List_risk_associated = riskDetail.List_risk_associated;
    //        model.Owner = riskDetail.Owner;
    //        list.Add(model);
    //      }
    //    }
    //  }
    //  //var query = list.GroupBy(x => new { x.RiskID, x.SubRiskID }); 

    //  return View(list);
    //}

    //[HttpPost]
    //public string LossReport(string compId, string deptId)
    //{
    //  string res = "0";
    //  int companyId = 0;
    //  if (compId != "Select Company" && compId != "")
    //  {
    //    companyId = Convert.ToInt32(compId);
    //  }
    //  int departmentId = 0;
    //  if (deptId != "Select Department" && deptId != "")
    //  {
    //    departmentId = Convert.ToInt32(deptId);
    //  }
    //  List<StandardReportModel> list = new List<StandardReportModel>();
    //  var roles = Roles.GetRolesForUser().Single();
    //  var lstRiskDetalModel = new List<RiskDetailsModel>();
    //  //var predicate = PredicateBuilder.True<RiskDetailsModel>();
    //  List<CompanyMaster> companiesList = new List<CompanyMaster>();
    //  if (User.IsInRole("Admin"))
    //  {
    //    //var companies = db.CompaniesMaster.ToList();
    //    //foreach (var item in companies)
    //    //{
    //    var riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
    //    if (riskDetailsData.Count == 0 && deptId == "Select Department")
    //    {
    //      riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);
    //      }
    //    }
    //    if (riskDetailsData.Count == 0 && companyId == 0)
    //    {
    //      var companies = db.CompaniesMaster.ToList();
    //      foreach (var item in companies)
    //      {
    //        riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
    //        foreach (var items in riskDetailsData)
    //        {
    //          lstRiskDetalModel.Add(items);
    //        }
    //      }
    //    }
    //    else//(riskDetailsData != null)
    //    {
    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);
    //      }
    //    }
    //    //}
    //  }
    //  if (User.IsInRole("Consultant"))
    //  {
    //    //  var companyUsers = db.CompanyUsers.Where(x => x.CreatedBy == WebSecurity.CurrentUserId).ToList();
    //    //foreach (var users in companyUsers)
    //    // {
    //    var riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
    //    if (riskDetailsData.Count == 0 && deptId == "Select Department")
    //    {
    //      riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);
    //      }
    //    }
    //    if (riskDetailsData.Count == 0 && companyId == 0)
    //    {
    //      var companies = db.CompaniesMaster.ToList();
    //      foreach (var item in companies)
    //      {
    //        riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
    //        foreach (var items in riskDetailsData)
    //        {
    //          lstRiskDetalModel.Add(items);
    //        }
    //      }
    //    }
    //    else//if (riskDetailsData != null)
    //    {
    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);
    //      }
    //    }
    //    // }
    //  }

    //  if (User.IsInRole("Company"))
    //  {
    //    //  var companyUsers = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).ToList();
    //    // foreach (var users in companyUsers)
    //    //  {
    //    var riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.LossesAssociatedWithThisRisk == "Yes").ToList();

    //    if (riskDetailsData.Count == 0)
    //    {
    //      var companies = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).ToList();
    //      foreach (var item in companies)
    //      {
    //        riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
    //        foreach (var items in riskDetailsData)
    //        {
    //          lstRiskDetalModel.Add(items);
    //        }
    //      }
    //    }
    //    else//    if (riskDetailsData != null)
    //    {
    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);
    //      }
    //    }
    //    // }
    //  }


    //  if (lstRiskDetalModel != null && lstRiskDetalModel.Count > 0)
    //  {
    //    foreach (var item in lstRiskDetalModel)
    //    {
    //      var lstDetails = new StandardReportModel();
    //      lstDetails.RiskDetailID = Convert.ToInt32(item.RiskDetailID);
    //      lstDetails.CompanyId = Convert.ToInt32(item.CompanyID);
    //      lstDetails.CompanyName = item.CompanyMaster.ComapnyName;
    //      lstDetails.DepartmentID = Convert.ToInt32(item.DepartmentID);
    //      lstDetails.DepartmentName = item.DepartmentMaster.DepartmentName;
    //      lstDetails.SubRisk_Impact = item.SubRisk_Impact;
    //      lstDetails.SubRisk_Likelyhood = item.SubRisk_Likelyhood;
    //      lstDetails.Inherenet_risk_rating = item.Inherenet_risk_rating;
    //      lstDetails.MitigantID = Convert.ToInt32(item.MitigantID);
    //      lstDetails.Mitigant_whyEffective = item.Mitigant_whyEffective;
    //      lstDetails.Issue_Severity = item.Issue_Severity;
    //      lstDetails.ActionPlanAvailable = item.ActionPlanAvailable;
    //      lstDetails.Shared_process_Department = item.Shared_process_Department;
    //      lstDetails.Reason_Risk_acceptance = item.Reason_Risk_acceptance;
    //      lstDetails.Shared_process_Description = item.Shared_process_Description;
    //      lstDetails.CompanyObjectives = item.CompanyObjectives;
    //      lstDetails.Mitigant_Importance = item.Mitigant_Importance;
    //      lstDetails.RiskID = Convert.ToInt32(item.RiskID);
    //      lstDetails.SubRiskID = Convert.ToInt32(item.SubRiskID);
    //      lstDetails.Risk = item.RiskMaster.RiskName;
    //      lstDetails.subRisk = item.SubRiskMaster.SubRiskName;
    //      lstDetails.Mitigant = item.MitigantMaster.MitigantName;
    //      lstDetails.MitigantEffectivness = item.Mitigant_effectiveness;
    //      lstDetails.Issues = item.Issue;
    //      lstDetails.ActionPlan = item.ActionPlan;
    //      lstDetails.ActionPlanStatus = item.ActionPlan_Status;
    //      lstDetails.Owner = item.Owner;
    //      lstDetails.TargetDate = item.TargetDate.ToString("dd-MM-yyyy");
    //      lstDetails.Lossess = item.Losses;
    //      list.Add(lstDetails);
    //    }
    //  }

    //  if (lstRiskDetalModel.Count > 0)
    //  {
    //    this.HttpContext.Session["ReportName"] = "LossReport.rpt";
    //    this.HttpContext.Session["rptSource"] = list;
    //    res = "1";
    //  }
    //  return res;

    //}

    //public ActionResult AcceptedRisk()
    //{
    //  List<RiskManagementModel> list = new List<RiskManagementModel>();
    //  var roles = Roles.GetRolesForUser().Single();
    //  if (roles.ToLower() == "Company")
    //  {
    //    var companyUserId = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();
    //    var riskDetail = db.RiskDetails.Where(x => x.CompanyID == companyUserId.CompanyId).ToList();
    //    foreach (var riskDetailResult in riskDetail)
    //    {
    //      if (riskDetailResult != null && riskDetailResult.RiskDetailID > 0)
    //      {
    //        var model = new RiskManagementModel();
    //        model.RiskDetailID = riskDetailResult.RiskDetailID;
    //        model.CompanyId = riskDetailResult.CompanyID ?? 0;
    //        model.DepartmentID = riskDetailResult.DepartmentID ?? 0;
    //        model.RiskID = riskDetailResult.RiskID ?? 0;
    //        model.RiskName = riskDetailResult.RiskMaster.RiskName;
    //        model.SubRiskID = riskDetailResult.SubRiskID ?? 0;
    //        model.SubRiskName = riskDetailResult.SubRiskMaster.SubRiskName;
    //        model.SubRisk_Impact = riskDetailResult.SubRisk_Impact;
    //        model.SubRisk_Likelyhood = riskDetailResult.SubRisk_Likelyhood;
    //        model.Inherenet_risk_rating = riskDetailResult.Inherenet_risk_rating;
    //        model.MitigantID = riskDetailResult.MitigantID ?? 0;
    //        model.MitigantName = riskDetailResult.MitigantMaster.MitigantName;
    //        model.Mitigant_Importance = riskDetailResult.Mitigant_Importance;
    //        model.Mitigant_effectiveness = riskDetailResult.Mitigant_effectiveness;
    //        model.Issue = riskDetailResult.Issue;
    //        model.Issue_Severity = riskDetailResult.Issue_Severity;
    //        model.ActionPlanAvailable = riskDetailResult.ActionPlanAvailable;
    //        model.ActionPlan = riskDetailResult.ActionPlan;
    //        model.TargetDate = riskDetailResult.TargetDate;
    //        model.ActionPlan_Status = riskDetailResult.ActionPlan_Status;
    //        model.List_risk_associated = riskDetailResult.List_risk_associated;
    //        model.Owner = riskDetailResult.Owner;
    //        list.Add(model);
    //      }
    //    }
    //  }
    //  else
    //  {
    //    var riskDetails = db.RiskDetails.ToList();
    //    foreach (var riskDetail in riskDetails)
    //    {
    //      if (riskDetails.Count > 0)
    //      {
    //        var model = new RiskManagementModel();
    //        model = new RiskManagementModel();
    //        model.RiskDetailID = riskDetail.RiskDetailID;
    //        model.CompanyId = riskDetail.CompanyID ?? 0;
    //        model.DepartmentID = riskDetail.DepartmentID ?? 0;
    //        model.RiskID = riskDetail.RiskID ?? 0;
    //        model.RiskName = riskDetail.RiskMaster.RiskName;
    //        model.SubRiskID = riskDetail.SubRiskID ?? 0;
    //        model.SubRiskName = riskDetail.SubRiskMaster.SubRiskName;
    //        model.SubRisk_Impact = riskDetail.SubRisk_Impact;
    //        model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
    //        model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
    //        model.MitigantID = riskDetail.MitigantID ?? 0;
    //        model.MitigantName = riskDetail.MitigantMaster.MitigantName;
    //        model.Mitigant_Importance = riskDetail.Mitigant_Importance;
    //        model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
    //        model.Issue = riskDetail.Issue;
    //        model.Issue_Severity = riskDetail.Issue_Severity;
    //        model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
    //        model.ActionPlan = riskDetail.ActionPlan;
    //        model.TargetDate = riskDetail.TargetDate;
    //        model.ActionPlan_Status = riskDetail.ActionPlan_Status;
    //        model.List_risk_associated = riskDetail.List_risk_associated;
    //        model.Owner = riskDetail.Owner;
    //        list.Add(model);
    //      }
    //    }
    //  }
    //  //var query = list.GroupBy(x => new { x.RiskID, x.SubRiskID }); 

    //  return View(list);
    //}

    //[HttpPost]
    //public string AcceptedRisk(string compId, string deptId)
    //{
    //  string res = "0";
    //  int companyId = 0;
    //  if (compId != "Select Company" && compId != "")
    //  {
    //    companyId = Convert.ToInt32(compId);
    //  }
    //  int departmentId = 0;
    //  if (deptId != "Select Department" && deptId != "")
    //  {
    //    departmentId = Convert.ToInt32(deptId);
    //  }
    //  List<StandardReportModel> list = new List<StandardReportModel>();
    //  var roles = Roles.GetRolesForUser().Single();
    //  var lstRiskDetalModel = new List<RiskDetailsModel>();
    //  //var predicate = PredicateBuilder.True<RiskDetailsModel>();
    //  List<CompanyMaster> companiesList = new List<CompanyMaster>();
    //  if (User.IsInRole("Admin"))
    //  {
    //    //var companies = db.CompaniesMaster.ToList();
    //    //foreach (var item in companies)
    //    //{
    //    var riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "C").ToList();
    //    if (riskDetailsData.Count == 0 && deptId == "Select Department")
    //    {
    //      riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.Status == "C").ToList();

    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);

    //      }
    //    }
    //    if (riskDetailsData.Count == 0 && companyId == 0)
    //    {
    //      var companies = db.CompaniesMaster.ToList();
    //      foreach (var item in companies)
    //      {
    //        riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId && x.Status == "C").ToList();

    //        foreach (var items in riskDetailsData)
    //        {
    //          lstRiskDetalModel.Add(items);

    //        }
    //      }
    //    }
    //    else//(riskDetailsData != null)
    //    {

    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);

    //      }
    //    }
    //    //}
    //  }
    //  if (User.IsInRole("Consultant"))
    //  {

    //    var riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "C").ToList();
    //    if (riskDetailsData.Count == 0 && deptId == "Select Department")
    //    {

    //      riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.Status == "C").ToList();
    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);

    //      }
    //    }
    //    if (riskDetailsData.Count == 0 && companyId == 0)
    //    {
    //      var companies = db.CompaniesMaster.ToList();

    //      foreach (var item in companies)
    //      {
    //        riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId && x.Status == "C").ToList();
    //        foreach (var items in riskDetailsData)
    //        {
    //          lstRiskDetalModel.Add(items);
    //        }

    //      }
    //    }
    //    else//if (riskDetailsData != null)
    //    {

    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);

    //      }
    //    }
    //    // }
    //  }

    //  if (User.IsInRole("Company"))
    //  {
    //    //  var companyUsers = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).ToList();
    //    // foreach (var users in companyUsers)
    //    //  {
    //    var riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.Status == "C").ToList();

    //    if (riskDetailsData.Count == 0)
    //    {
    //      var companies = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).ToList();
    //      foreach (var item in companies)
    //      {
    //        riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId && x.Status == "C").ToList();

    //        foreach (var items in riskDetailsData)
    //        {
    //          lstRiskDetalModel.Add(items);

    //        }
    //      }
    //    }
    //    else//    if (riskDetailsData != null)
    //    {

    //      foreach (var items in riskDetailsData)
    //      {
    //        lstRiskDetalModel.Add(items);

    //      }
    //    }
    //    // }
    //  }


    //  if (lstRiskDetalModel != null && lstRiskDetalModel.Count > 0)
    //  {
    //    foreach (var item in lstRiskDetalModel)
    //    {
    //      var lstDetails = new StandardReportModel();
    //      lstDetails.RiskDetailID = Convert.ToInt32(item.RiskDetailID);
    //      lstDetails.CompanyId = Convert.ToInt32(item.CompanyID);
    //      lstDetails.CompanyName = item.CompanyMaster.ComapnyName;
    //      lstDetails.DepartmentID = Convert.ToInt32(item.DepartmentID);
    //      lstDetails.DepartmentName = item.DepartmentMaster.DepartmentName;
    //      lstDetails.SubRisk_Impact = item.SubRisk_Impact;
    //      lstDetails.SubRisk_Likelyhood = item.SubRisk_Likelyhood;
    //      lstDetails.Inherenet_risk_rating = item.Inherenet_risk_rating;
    //      lstDetails.MitigantID = Convert.ToInt32(item.MitigantID);
    //      lstDetails.Mitigant_whyEffective = item.Mitigant_whyEffective;
    //      lstDetails.Issue_Severity = item.Issue_Severity;
    //      lstDetails.ActionPlanAvailable = item.ActionPlanAvailable;
    //      lstDetails.Shared_process_Department = item.Shared_process_Department;
    //      lstDetails.Reason_Risk_acceptance = item.Reason_Risk_acceptance;
    //      lstDetails.Shared_process_Description = item.Shared_process_Description;
    //      lstDetails.CompanyObjectives = item.CompanyObjectives;
    //      lstDetails.Mitigant_Importance = item.Mitigant_Importance;
    //      lstDetails.RiskID = Convert.ToInt32(item.RiskID);
    //      lstDetails.SubRiskID = Convert.ToInt32(item.SubRiskID);
    //      lstDetails.Risk = item.RiskMaster.RiskName;
    //      lstDetails.subRisk = item.SubRiskMaster.SubRiskName;
    //      lstDetails.Mitigant = item.MitigantMaster.MitigantName;
    //      lstDetails.MitigantEffectivness = item.Mitigant_effectiveness;
    //      lstDetails.Issues = item.Issue;
    //      lstDetails.ActionPlan = item.ActionPlan;
    //      lstDetails.ActionPlanStatus = item.ActionPlan_Status;
    //      lstDetails.Owner = item.Owner;
    //      lstDetails.TargetDate = item.TargetDate.ToString("dd-MM-yyyy");
    //      lstDetails.Lossess = item.Losses;
    //      list.Add(lstDetails);
    //    }
    //  }

    //  if (lstRiskDetalModel.Count > 0)
    //  {
    //    this.HttpContext.Session["ReportName"] = "AcceptedReport.rpt";
    //    this.HttpContext.Session["rptSource"] = list;
    //    res = "1";
    //  }
    //  return res;


    //}

    public ActionResult RiskAssessment()
    {
      List<RiskManagementModel> list = new List<RiskManagementModel>();
      var roles = Roles.GetRolesForUser().Single();
      if (roles.ToLower() == "Company")
      {
        var companyUserId = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();
        var riskDetail = db.RiskDetails.Where(x => x.CompanyID == companyUserId.CompanyId).ToList();
        foreach (var riskDetailResult in riskDetail)
        {
          if (riskDetailResult != null && riskDetailResult.RiskDetailID > 0)
          {
            var model = new RiskManagementModel();
            model.RiskDetailID = riskDetailResult.RiskDetailID;
            model.CompanyId = riskDetailResult.CompanyID ?? 0;
            model.DepartmentID = riskDetailResult.DepartmentID ?? 0;
            model.RiskID = riskDetailResult.RiskID ?? 0;
            model.RiskName = riskDetailResult.RiskMaster.RiskName;
            model.SubRiskID = riskDetailResult.SubRiskID ?? 0;
            model.SubRiskName = riskDetailResult.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetailResult.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetailResult.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetailResult.Inherenet_risk_rating;
            model.MitigantID = riskDetailResult.MitigantID ?? 0;
            model.MitigantName = riskDetailResult.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetailResult.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetailResult.Mitigant_effectiveness;
            model.Issue = riskDetailResult.Issue;
            model.Issue_Severity = riskDetailResult.Issue_Severity;
            model.ActionPlanAvailable = riskDetailResult.ActionPlanAvailable;
            model.ActionPlan = riskDetailResult.ActionPlan;
            model.TargetDate = riskDetailResult.TargetDate.ToString();
            model.ActionPlan_Status = riskDetailResult.ActionPlan_Status;
            model.List_risk_associated = riskDetailResult.List_risk_associated;
            model.Owner = riskDetailResult.Owner;
            list.Add(model);
          }
        }
      }
      else
      {
        var riskDetails = db.RiskDetails.ToList();
        foreach (var riskDetail in riskDetails)
        {
          if (riskDetails.Count > 0)
          {
            var model = new RiskManagementModel();
            model = new RiskManagementModel();
            model.RiskDetailID = riskDetail.RiskDetailID;
            model.CompanyId = riskDetail.CompanyID ?? 0;
            model.DepartmentID = riskDetail.DepartmentID ?? 0;
            model.RiskID = riskDetail.RiskID ?? 0;
            model.RiskName = riskDetail.RiskMaster.RiskName;
            model.SubRiskID = riskDetail.SubRiskID ?? 0;
            model.SubRiskName = riskDetail.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetail.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
            model.MitigantID = riskDetail.MitigantID ?? 0;
            model.MitigantName = riskDetail.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetail.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
            model.Issue = riskDetail.Issue;
            model.Issue_Severity = riskDetail.Issue_Severity;
            model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
            model.ActionPlan = riskDetail.ActionPlan;
            model.TargetDate = riskDetail.TargetDate.ToString();
            model.ActionPlan_Status = riskDetail.ActionPlan_Status;
            model.List_risk_associated = riskDetail.List_risk_associated;
            model.Owner = riskDetail.Owner;
            list.Add(model);
          }
        }
      }
      //var query = list.GroupBy(x => new { x.RiskID, x.SubRiskID }); 

      return View(list);
    }

    [HttpPost]
    public string RiskAssessment(string compId, string riskId)
    {
      string res = "0";
      int companyId = 0;
      if (compId != "Select Company" && compId != "")
      {
        companyId = Convert.ToInt32(compId);
      }
      int riskAssessmentId = 0;
      if (riskId != "Select Risk Assessment" && riskId != "")
      {
        riskAssessmentId = Convert.ToInt32(riskId);
      }
      List<StandardReportModel> list = new List<StandardReportModel>();
      var roles = Roles.GetRolesForUser().Single();
      var lstRiskDetalModel = new List<RiskDetailsModel>();
      //var predicate = PredicateBuilder.True<RiskDetailsModel>();
      List<CompanyMaster> companiesList = new List<CompanyMaster>();
      if (User.IsInRole("Admin"))
      {
        //var companies = db.CompaniesMaster.ToList();
        //foreach (var item in companies)
        //{
        var riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.RiskAssessmentID == riskAssessmentId).ToList();
        if (riskDetailsData.Count == 0 && riskId == "Select Risk Assessment")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId).ToList();

          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);

          }
        }
        if (riskDetailsData.Count == 0 && companyId == 0)
        {
          var companies = db.CompaniesMaster.ToList();
          foreach (var item in companies)
          {
            riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId).ToList();

            foreach (var items in riskDetailsData)
            {
              lstRiskDetalModel.Add(items);

            }
          }
        }
        else//(riskDetailsData != null)
        {

          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);

          }
        }
        //}
      }
      if (User.IsInRole("Consultant"))
      {

        var riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.RiskAssessmentID == riskAssessmentId).ToList();
        if (riskDetailsData.Count == 0 && riskId == "Select Risk Assessment")
        {

          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId).ToList();
          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);

          }
        }
        if (riskDetailsData.Count == 0 && companyId == 0)
        {
          var companies = db.CompaniesMaster.ToList();

          foreach (var item in companies)
          {
            riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId).ToList();
            foreach (var items in riskDetailsData)
            {
              lstRiskDetalModel.Add(items);
            }

          }
        }
        else//if (riskDetailsData != null)
        {

          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);

          }
        }
        // }
      }

      if (User.IsInRole("Company"))
      {
        //  var companyUsers = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).ToList();
        // foreach (var users in companyUsers)
        //  {
        var riskDetailsData = db.RiskDetails.Where(x => x.RiskAssessmentID == riskAssessmentId).ToList();

        if (riskDetailsData.Count == 0)
        {
          var companies = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).ToList();
          foreach (var item in companies)
          {
            riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId).ToList();

            foreach (var items in riskDetailsData)
            {
              lstRiskDetalModel.Add(items);

            }
          }
        }
        else//    if (riskDetailsData != null)
        {

          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);

          }
        }
        // }
      }


      if (lstRiskDetalModel != null && lstRiskDetalModel.Count > 0)
      {
        foreach (var item in lstRiskDetalModel)
        {
          var lstDetails = new StandardReportModel();
          lstDetails.RiskDetailID = Convert.ToInt32(item.RiskDetailID);
          lstDetails.CompanyId = Convert.ToInt32(item.CompanyID);
          lstDetails.CompanyName = item.CompanyMaster.ComapnyName;
          lstDetails.DepartmentID = Convert.ToInt32(item.DepartmentID);
          lstDetails.DepartmentName = item.DepartmentMaster.DepartmentName;
          lstDetails.SubRisk_Impact = item.SubRisk_Impact;
          lstDetails.SubRisk_Likelyhood = item.SubRisk_Likelyhood;
          lstDetails.Inherenet_risk_rating = item.Inherenet_risk_rating;
          lstDetails.MitigantID = Convert.ToInt32(item.MitigantID);
          lstDetails.Mitigant_whyEffective = item.Mitigant_whyEffective;
          lstDetails.Issue_Severity = item.Issue_Severity;
          lstDetails.ActionPlanAvailable = item.ActionPlanAvailable;
          lstDetails.Shared_process_Department = item.Shared_process_Department;
          lstDetails.Reason_Risk_acceptance = item.Reason_Risk_acceptance;
          lstDetails.Shared_process_Description = item.Shared_process_Description;
          lstDetails.CompanyObjectives = item.CompanyObjectives;
          lstDetails.Mitigant_Importance = item.Mitigant_Importance;
          lstDetails.RiskID = Convert.ToInt32(item.RiskID);
          lstDetails.SubRiskID = Convert.ToInt32(item.SubRiskID);
          lstDetails.Risk = item.RiskMaster.RiskName;
          lstDetails.subRisk = item.SubRiskMaster.SubRiskName;
          lstDetails.Mitigant = item.MitigantMaster.MitigantName;
          lstDetails.MitigantEffectivness = item.Mitigant_effectiveness;
          lstDetails.Issues = item.Issue;
          lstDetails.ActionPlan = item.ActionPlan;
          lstDetails.ActionPlanStatus = item.ActionPlan_Status;
          lstDetails.Owner = item.Owner;
          lstDetails.TargetDate = item.TargetDate.ToString();
          lstDetails.Lossess = item.Losses;
          if (item.Status == "C")
          {
            lstDetails.Status = "Approved";
          }
          else
          {
            lstDetails.Status = "In Progress";
          }

          list.Add(lstDetails);
        }
      }

      if (lstRiskDetalModel.Count > 0)
      {
        this.HttpContext.Session["ReportName"] = "RiskAssessmentReport.rpt";
        this.HttpContext.Session["rptSource"] = list;
        res = "1";
      }
      return res;
    }

    //public JsonResult AssessmentList(string CompanyId)
    //{
    //  if (CompanyId == "")
    //  {
    //    return Json(new SelectList("", 0));
    //  }
    //  else
    //  {
    //    int companyId = Convert.ToInt32(CompanyId);
    //    List<RiskAssessmentMaster> riskAssessment = db.RiskAssessmentMaster.Where(x => x.CompanyID == companyId).ToList();
    //    if (HttpContext.Request.IsAjaxRequest())
    //      return Json(new SelectList(
    //                          riskAssessment.ToArray(),
    //                          "RiskAssessmentId",
    //                          "RiskAssessmentName"), JsonRequestBehavior.AllowGet);
    //    return Json(new SelectList(
    //                        riskAssessment.ToArray(),
    //                        "RiskAssessmentId",
    //                        "RiskAssessmentName"), JsonRequestBehavior.AllowGet);
    //  }
    //}

    public ActionResult DepartmentRisks(int? cId, int? dId)
    {
      var companyId = Session["compId"] + "";
      var departmentId = Session["deptId"] + "";
      if ((cId == null || cId == 0) && !String.IsNullOrWhiteSpace(companyId) && companyId != "0")
      {
        cId = Convert.ToInt32(companyId);
      }

      if ((dId == null || dId == 0) && !String.IsNullOrWhiteSpace(departmentId) && departmentId != "0")
      {
        dId = Convert.ToInt32(departmentId);
      }

      var riskAssesmentList = new List<RiskAssessmentMaster>();
      if (cId != null && dId != null)
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
      }
      else if (cId != null)
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId).ToList();
      }
      else if (dId != null)
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == dId).ToList();
      }
      else
      {
        riskAssesmentList = db.RiskAssessmentMaster.ToList();
      }
      //ViewBag.compId = cId;
      //ViewBag.deptId = dId;
      //var riskAssesmentList = new List<RiskAssessmentMaster>();
      //if (User.IsInRole("Consultant"))
      //{
      //  if (cId != null && dId != null)
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
      //  }
      //  else
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.ToList();
      //  }
      //}

      //if (User.IsInRole("Admin"))
      //{
      //  if (cId != null && dId != null)
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
      //  }
      //  else
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.ToList();
      //  }
      //}
      //if (User.IsInRole("Company"))
      //{
      //  var userId = WebSecurity.CurrentUserId;
      //  var list = db.CompanyUsers.Where(x => x.UserId == userId).FirstOrDefault();

      //  if (cId != null && dId != null)
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
      //  }
      //  else
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == list.CompanyId).ToList();
      //  }
      //}
      return View(riskAssesmentList.OrderByDescending(x => x.InsertedDate).ThenBy(x => x.Status));
    }

    [HttpPost]
    public ActionResult DepartmentRisks(int? ids)
    {

      int deptId = Request["hfDeptId"] != "" ? Convert.ToInt32(Request["hfDeptId"]) : 0;
      int compId = Request["hfCompId"] != "" ? Convert.ToInt32(Request["hfCompId"]) : 0;
      Session["compId"] = compId;
      Session["deptId"] = deptId;
      string riskAssesment = Request["txtRiskAssesmentName"] != "" ? Request["txtRiskAssesmentName"] : "";
      var riskAssesmentList = new List<RiskAssessmentMaster>();
      ViewBag.CompId = compId;
      ViewBag.DeptId = deptId;
      if (riskAssesment != null && riskAssesment != "")
      {
        string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (String.IsNullOrWhiteSpace(ip))
        {
          ip = Request.ServerVariables["REMOTE_ADDR"];
        }
        var riskAssesmentMaster = new RiskAssessmentMaster();
        riskAssesmentMaster.RiskAssessmentName = riskAssesment;
        riskAssesmentMaster.CompanyID = compId;
        riskAssesmentMaster.DepartmentID = deptId;
        riskAssesmentMaster.Status = "P";
        riskAssesmentMaster.InsertedBy = WebSecurity.CurrentUserId;
        riskAssesmentMaster.Insertedmachineinfo = ip;
        riskAssesmentMaster.InsertedDate = DateTime.Now;
        db.RiskAssessmentMaster.Add(riskAssesmentMaster);
        db.SaveChanges();
        int riskAssesmentId = riskAssesmentMaster.RiskAssessmentId;
        if (User.IsInRole("Company"))
        {
          return RedirectToAction("RiskDetails", "Orm", new
          {
            id = riskAssesmentId,
            compId = riskAssesmentMaster.CompanyID,
            deptId = riskAssesmentMaster.DepartmentID
          });
        }
        {
          return RedirectToAction("RiskDetails", "Orm", new
          {
            id = riskAssesmentId,
            compId = riskAssesmentMaster.CompanyID,
            deptId = riskAssesmentMaster.DepartmentID
          });
        }
        //return View(db.RiskAssessmentMaster.OrderByDescending(x => x.Status).ToList());   
      }

      //var listDepartmentRisks = new List<RiskDetailsModel>();
      if (User.IsInRole("Consultant"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}                
      }
      if (User.IsInRole("Admin"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}
      }
      if (User.IsInRole("Company"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}
      }

      return View(riskAssesmentList.OrderByDescending(x => x.Status));
    }

    public ActionResult RiskDetails(int? id, int? compId, int? deptId)
    {
      ViewBag.compId = compId;
      ViewBag.deptId = deptId;
      ViewBag.RiskAssesmentId = id;
      if (compId == 0 && deptId == 0)
      {
        var riskList = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == id).FirstOrDefault();
        compId = riskList.CompanyID;
        deptId = riskList.DepartmentID;
        ViewBag.compId = compId;
        ViewBag.deptId = deptId;
        ViewBag.riskheaderName = riskList.RiskAssessmentName;
      }
      var companyName = db.CompaniesMaster.Where(x => x.CompanyId == compId).FirstOrDefault();
      if (deptId == 0)
      {
        var riskList1 = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == id).FirstOrDefault();
        deptId = riskList1.DepartmentID;
        ViewBag.deptId = deptId;
        ViewBag.riskheaderName = riskList1.RiskAssessmentName;
      }
      var deptName = db.DepartmentsMaster.Where(x => x.DepartmentId == deptId).FirstOrDefault();
      ViewBag.companyName = companyName.ComapnyName;
      ViewBag.departmentName = deptName.DepartmentName;

      List<RiskManagementModel> list = new List<RiskManagementModel>();
      var roles = Roles.GetRolesForUser().Single();
      if (roles.ToLower() == "Company")
      {
        //  var companyUserId = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();
        var riskDetail = db.RiskDetails.Where(x => x.CompanyID == compId).ToList();
        foreach (var riskDetailResult in riskDetail)
        {
          if (riskDetailResult != null && riskDetailResult.RiskDetailID > 0)
          {
            var model = new RiskManagementModel();
            model.RiskDetailID = riskDetailResult.RiskDetailID;
            model.CompanyId = riskDetailResult.CompanyID ?? 0;
            model.DepartmentID = riskDetailResult.DepartmentID ?? 0;
            model.RiskID = riskDetailResult.RiskID ?? 0;
            model.RiskName = riskDetailResult.RiskMaster.RiskName;
            model.SubRiskID = riskDetailResult.SubRiskID ?? 0;
            model.SubRiskName = riskDetailResult.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetailResult.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetailResult.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetailResult.Inherenet_risk_rating;
            model.MitigantID = riskDetailResult.MitigantID ?? 0;
            model.MitigantName = riskDetailResult.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetailResult.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetailResult.Mitigant_effectiveness;
            model.Issue = riskDetailResult.Issue;
            model.Issue_Severity = riskDetailResult.Issue_Severity;
            model.ActionPlanAvailable = riskDetailResult.ActionPlanAvailable;
            model.ActionPlan = riskDetailResult.ActionPlan;
            if (riskDetailResult.TargetDate != "N/A")
            {
              DateTime dt = Convert.ToDateTime(riskDetailResult.TargetDate);
              model.TargetDate = String.Format("{0:MM/dd/yyyy}", dt);
            }
            else
            {
              model.TargetDate = riskDetailResult.TargetDate.ToString();
            }
            model.ActionPlan_Status = riskDetailResult.ActionPlan_Status;
            model.List_risk_associated = riskDetailResult.List_risk_associated;
            model.Owner = riskDetailResult.Owner;
            list.Add(model);
          }
        }
      }
      else
      {
        var riskDetails = db.RiskDetails.ToList();
        foreach (var riskDetail in riskDetails)
        {
          if (riskDetails.Count > 0)
          {
            var model = new RiskManagementModel();
            model = new RiskManagementModel();
            model.RiskDetailID = riskDetail.RiskDetailID;
            model.CompanyId = riskDetail.CompanyID ?? 0;
            model.DepartmentID = riskDetail.DepartmentID ?? 0;
            model.RiskID = riskDetail.RiskID ?? 0;
            model.RiskName = riskDetail.RiskMaster.RiskName;
            model.SubRiskID = riskDetail.SubRiskID ?? 0;
            model.SubRiskName = riskDetail.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetail.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
            model.MitigantID = riskDetail.MitigantID ?? 0;
            model.MitigantName = riskDetail.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetail.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
            model.Issue = riskDetail.Issue;
            model.Issue_Severity = riskDetail.Issue_Severity;
            model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
            model.ActionPlan = riskDetail.ActionPlan;
            if (riskDetail.TargetDate != "N/A")
            {
              DateTime dt = Convert.ToDateTime(riskDetail.TargetDate);
              model.TargetDate = String.Format("{0:MM/dd/yyyy}", dt);
            }
            else
            {
              model.TargetDate = riskDetail.TargetDate.ToString();
            }
            model.ActionPlan_Status = riskDetail.ActionPlan_Status;
            model.List_risk_associated = riskDetail.List_risk_associated;
            model.Owner = riskDetail.Owner;
            list.Add(model);
          }
        }
      }
      //var query = list.GroupBy(x => new { x.RiskID, x.SubRiskID }); 

      return View(list);
    }

    [HttpPost]
    public string RiskDetails(string compId, string deptId, string riskRating, string issueSaverty, string subriskImpact,
    string subriskMethod, string subriskLikehood, string mitigantImportance, string riskAssessmentName)
    {
      string res = "0";
      int companyId = 0;
      if (compId != "Select Company" && compId != "")
      {
        companyId = Convert.ToInt32(compId);
      }
      int departmentId = 0;
      if (deptId != "Select Department" && deptId != "")
      {
        departmentId = Convert.ToInt32(deptId);
      }

      if (departmentId == 0 && riskAssessmentName != "")
      {
        int riskId = Convert.ToInt32(riskAssessmentName);
        var riskList = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == riskId).FirstOrDefault();
        companyId = Convert.ToInt32(riskList.CompanyID);
        departmentId = Convert.ToInt32(riskList.DepartmentID);
      }

      List<StandardReportModel> list = new List<StandardReportModel>();
      var roles = Roles.GetRolesForUser().Single();
      var listRiskDetails = db.RiskDetails.Where(x => x.Status == "C").ToList();
      var lstRiskDetalModel = new List<RiskDetailsModel>();

      var predicate = PredicateBuilder.True<RiskDetailsModel>();
      var orPredicate = PredicateBuilder.False<RiskDetailsModel>();


      bool andFlag = false;
      bool orFlag = false;

      if (compId != null && compId != "" && compId != "Select Company")
      {
        andFlag = true;
        predicate = predicate.And(v => v.CompanyID == companyId);
      }
      if (deptId != null && deptId != "" && deptId != "Select Department")
      {
        andFlag = true;
        predicate = predicate.And(v => v.DepartmentID == departmentId);
      }
      if (riskRating != "" && riskRating != null && riskRating != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.Inherenet_risk_rating == riskRating);
      }
      if (issueSaverty != "" && issueSaverty != null && issueSaverty != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.Issue_Severity == issueSaverty);
      }
      if (subriskImpact != "" && subriskImpact != null && subriskImpact != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.SubRisk_Impact == subriskImpact);
      }
      if (subriskLikehood != "" && subriskLikehood != null && subriskLikehood != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.SubRisk_Likelyhood == subriskLikehood);
      }
      if (mitigantImportance != "" && mitigantImportance != null && mitigantImportance != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.Mitigant_Importance == mitigantImportance);
      }
      lstRiskDetalModel = GetPredicate(predicate, orPredicate, andFlag, orFlag);

      if (lstRiskDetalModel != null && lstRiskDetalModel.Count > 0)
      {
        foreach (var item in lstRiskDetalModel)
        {
          var lstDetails = new StandardReportModel();
          lstDetails.RiskDetailID = Convert.ToInt32(item.RiskDetailID);
          lstDetails.CompanyId = Convert.ToInt32(item.CompanyID);
          lstDetails.CompanyName = item.CompanyMaster.ComapnyName;
          lstDetails.DepartmentID = Convert.ToInt32(item.DepartmentID);
          lstDetails.DepartmentName = item.DepartmentMaster.DepartmentName;
          lstDetails.SubRisk_Impact = item.SubRisk_Impact;
          lstDetails.SubRisk_Likelyhood = item.SubRisk_Likelyhood;
          lstDetails.Inherenet_risk_rating = item.Inherenet_risk_rating;
          lstDetails.MitigantID = Convert.ToInt32(item.MitigantID);
          lstDetails.Mitigant_whyEffective = item.Mitigant_whyEffective;
          lstDetails.Issue_Severity = item.Issue_Severity;
          lstDetails.ActionPlanAvailable = item.ActionPlanAvailable;
          lstDetails.Shared_process_Department = item.Shared_process_Department;
          lstDetails.Reason_Risk_acceptance = item.Reason_Risk_acceptance;
          lstDetails.Shared_process_Description = item.Shared_process_Description;
          lstDetails.CompanyObjectives = item.CompanyObjectives;

          lstDetails.RiskID = Convert.ToInt32(item.RiskID);
          lstDetails.SubRiskID = Convert.ToInt32(item.SubRiskID);
          lstDetails.Risk = item.RiskMaster.RiskName;
          lstDetails.subRisk = item.SubRiskMaster.SubRiskName;
          lstDetails.Mitigant = item.MitigantMaster.MitigantName;
          lstDetails.MitigantEffectivness = item.Mitigant_effectiveness;
          lstDetails.Mitigant_description = item.MitigantMaster.MitigantDesc;
          lstDetails.Issues = item.Issue;
          lstDetails.ActionPlan = item.ActionPlan;
          lstDetails.ActionPlanStatus = item.ActionPlan_Status;
          lstDetails.Owner = item.Owner;
          if (item.TargetDate != "N/A")
          {
            DateTime dt = Convert.ToDateTime(item.TargetDate);
            lstDetails.TargetDate = String.Format("{0:MM/dd/yyyy}", dt);
          }
          else
          {
            lstDetails.TargetDate = item.TargetDate.ToString();
          }
          lstDetails.Mitigant_Importance = item.Mitigant_Importance;
          lstDetails.IsThisRiskBeingAccepted = item.IsThisRiskBeingAccepted;
          lstDetails.List_risk_associated = item.List_risk_associated;

          if (item.Status == "C")
          {
            lstDetails.Status = "Approved";
          }
          else
          {
            lstDetails.Status = "In Progress";
          }
          if (item.RiskAssessmentMaster != null)
          {
            lstDetails.RiskAssesmentName = item.RiskAssessmentMaster.RiskAssessmentName;
            //lstDetails.Status = item.RiskAssessmentMaster.Status;
            lstDetails.InsertDate = Convert.ToDateTime(item.RiskAssessmentMaster.InsertedDate);
          }
          list.Add(lstDetails);
        }
      }

      //Temp solution
      //if (compId != null && compId != "" && compId != "Select Company")
      //{
      //  list = list.Where(x => x.CompanyId == companyId).ToList();
      //}
      if (riskAssessmentName != "")
      {
        int id = Convert.ToInt32(riskAssessmentName);
        var newList = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == id).FirstOrDefault();
        list = list.Where(x => x.RiskAssesmentName == newList.RiskAssessmentName).ToList();
      }

      if (lstRiskDetalModel.Count > 0)
      {
        this.HttpContext.Session["ReportName"] = "RiskAssessmentReport.rpt";
        this.HttpContext.Session["rptSource"] = list;
        res = "1";
      }
      return res;
    }

    public static List<RiskDetailsModel> GetPredicate(Expression<Func<RiskDetailsModel, bool>> predicate, Expression<Func<RiskDetailsModel, bool>> orPredicate,
                                                    bool andFlag, bool orFlag)
    {
      RcsaDb db = new RcsaDb();
      IQueryable<RiskDetailsModel> list = db.RiskDetails;

      if (andFlag)
      {
        list = db.RiskDetails.AsExpandable().Where(predicate);
      }

      if (orFlag)
      {
        list = list.AsExpandable().Where(orPredicate);
      }
      return list.ToList();
      //      return db.RiskDetails.AsExpandable().Where(predicate).ToList();
    }

    [HttpPost]
    public string GetAllRiskAssessmentList(string compId, string deptId)
    {
      string res = "1";
      int companyId = 0;
      int departmentId = 0;

      var riskAssesmentAtTime = db.RiskDetails.ToList();

      if (!String.IsNullOrWhiteSpace(compId))
      {
        companyId = Convert.ToInt32(compId);
        riskAssesmentAtTime = riskAssesmentAtTime.Where(x => x.CompanyID == companyId).ToList();
      }
      if (!String.IsNullOrWhiteSpace(deptId))
      {
        departmentId = Convert.ToInt32(deptId);
        riskAssesmentAtTime = riskAssesmentAtTime.Where(x => x.DepartmentID == departmentId).ToList();
      }


      //var RiskAssesmentAtTime = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "P").ToList();
      if (riskAssesmentAtTime != null && riskAssesmentAtTime.Count > 0)
      {
        res = "0";
      }
      return res;
    }

    public ActionResult SpecificTypeRisk(int? cId, int? dId)
    {
      var companyId = Session["compId"] + "";
      var departmentId = Session["deptId"] + "";
      if ((cId == null || cId == 0) && !String.IsNullOrWhiteSpace(companyId) && companyId != "0")
      {
        cId = Convert.ToInt32(companyId);
      }

      if ((dId == null || dId == 0) && !String.IsNullOrWhiteSpace(departmentId) && departmentId != "0")
      {
        dId = Convert.ToInt32(departmentId);
      }

      var riskAssesmentList = new List<RiskAssessmentMaster>();
      if (cId != null && dId != null)
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
      }
      else if (cId != null)
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId).ToList();
      }
      else if (dId != null)
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == dId).ToList();
      }
      else
      {
        riskAssesmentList = db.RiskAssessmentMaster.ToList();
      }
    
      return View(riskAssesmentList.OrderByDescending(x => x.InsertedDate).ThenBy(x => x.Status));
    }

    [HttpPost]
    public ActionResult SpecificTypeRisk(int? ids)
    {

      int deptId = Request["hfDeptId"] != "" ? Convert.ToInt32(Request["hfDeptId"]) : 0;
      int compId = Request["hfCompId"] != "" ? Convert.ToInt32(Request["hfCompId"]) : 0;

      Session["compId"] = compId;
      Session["deptId"] = deptId;
      string riskAssesment = Request["txtRiskAssesmentName"] != "" ? Request["txtRiskAssesmentName"] : "";
      var riskAssesmentList = new List<RiskAssessmentMaster>();
      ViewBag.CompId = compId;
      ViewBag.DeptId = deptId;
      if (riskAssesment != null && riskAssesment != "")
      {
        string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (String.IsNullOrWhiteSpace(ip))
        {
          ip = Request.ServerVariables["REMOTE_ADDR"];
        }
        var riskAssesmentMaster = new RiskAssessmentMaster();
        riskAssesmentMaster.RiskAssessmentName = riskAssesment;
        riskAssesmentMaster.CompanyID = compId;
        riskAssesmentMaster.DepartmentID = deptId;
        riskAssesmentMaster.Status = "P";
        riskAssesmentMaster.InsertedBy = WebSecurity.CurrentUserId;
        riskAssesmentMaster.Insertedmachineinfo = ip;
        riskAssesmentMaster.InsertedDate = DateTime.Now;
        db.RiskAssessmentMaster.Add(riskAssesmentMaster);
        db.SaveChanges();
        int riskAssesmentId = riskAssesmentMaster.RiskAssessmentId;
        if (User.IsInRole("Company"))
        {
          return RedirectToAction("RiskDetails", "Orm", new
          {
            id = riskAssesmentId,
            compId = riskAssesmentMaster.CompanyID,
            deptId = riskAssesmentMaster.DepartmentID
          });
        }
        //{
        //  return RedirectToAction("RiskDetails", "Orm", new
        //  {
        //    id = riskAssesmentId,
        //    compId = riskAssesmentMaster.CompanyID,
        //    deptId = riskAssesmentMaster.DepartmentID
        //  });
        //}
        //return View(db.RiskAssessmentMaster.OrderByDescending(x => x.Status).ToList());   
      }

      if (compId == 0 && deptId == 0)
      {
        riskAssesmentList = db.RiskAssessmentMaster.OrderByDescending(x => x.InsertedDate).ThenBy(x => x.Status).ToList();
      }
      else
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
      }

      #region Dummy Code
      /*if (User.IsInRole("Consultant"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}                
      }
      if (User.IsInRole("Admin"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
      }
      if (User.IsInRole("Company"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}
      }*/
      #endregion

      return View(riskAssesmentList.OrderByDescending(x => x.Status));
    }

    public ActionResult SpecificRiskDetails(int? id, int? compId, int? deptId)
    {
      ViewBag.compId = compId;
      ViewBag.deptId = deptId;
      ViewBag.RiskAssesmentId = id;

      if (compId == 0 && deptId == 0)
      {
        var riskList = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == id).FirstOrDefault();
        compId = riskList.CompanyID;
        deptId = riskList.DepartmentID;
        ViewBag.compId = compId;
        ViewBag.deptId = deptId;
        ViewBag.riskheaderName = riskList.RiskAssessmentName;
      }
      var companyName = db.CompaniesMaster.Where(x => x.CompanyId == compId).FirstOrDefault();
      if (deptId == 0)
      {
        var riskList1 = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == id).FirstOrDefault();
        deptId = riskList1.DepartmentID;
        ViewBag.deptId = deptId;
        ViewBag.riskheaderName = riskList1.RiskAssessmentName;
      }
      var deptName = db.DepartmentsMaster.Where(x => x.DepartmentId == deptId).FirstOrDefault();
      ViewBag.companyName = companyName.ComapnyName;
      ViewBag.departmentName = deptName.DepartmentName;

      var reportList = new SelectList(new[]
                                          {
                                              new {ID="1",Name="Has Action Plan"},
                                              new{ID="2",Name="Very High or High Mitigant Importance"},
                                              new{ID="3",Name="Issue Severity: High"},
                                              new{ID="4",Name="Issue Severity:Very High"},
                                              new{ID="5",Name="Mitigant Effectiveness: Effective"},
                                              new{ID="6",Name="Subrisk Impact: High"},
                                              new{ID="7",Name="Subrisk Impact: Very High"},
                                              new{ID="8",Name="Subrisk Likelihood: High"},
                                              new{ID="9",Name="Subrisk Likelihood: Very High"},
                                              new{ID="10",Name="Accepted Risk Report"},
                                              new{ID="11",Name="Loss Reporting Report"},
                                              new{ID="12",Name="Risk Assessment Report"},
                                          },
                   "ID", "Name", 1);
      ViewData["list"] = reportList;

      List<RiskManagementModel> list = new List<RiskManagementModel>();
      var roles = Roles.GetRolesForUser().Single();
      if (roles.ToLower() == "Company")
      {
        //  var companyUserId = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();
        var riskDetail = db.RiskDetails.Where(x => x.CompanyID == compId).ToList();
        foreach (var riskDetailResult in riskDetail)
        {
          if (riskDetailResult != null && riskDetailResult.RiskDetailID > 0)
          {
            var model = new RiskManagementModel();
            model.RiskDetailID = riskDetailResult.RiskDetailID;
            model.CompanyId = riskDetailResult.CompanyID ?? 0;
            model.DepartmentID = riskDetailResult.DepartmentID ?? 0;
            model.RiskID = riskDetailResult.RiskID ?? 0;
            model.RiskName = riskDetailResult.RiskMaster.RiskName;
            model.SubRiskID = riskDetailResult.SubRiskID ?? 0;
            model.SubRiskName = riskDetailResult.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetailResult.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetailResult.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetailResult.Inherenet_risk_rating;
            model.MitigantID = riskDetailResult.MitigantID ?? 0;
            model.MitigantName = riskDetailResult.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetailResult.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetailResult.Mitigant_effectiveness;
            model.Issue = riskDetailResult.Issue;
            model.Issue_Severity = riskDetailResult.Issue_Severity;
            model.ActionPlanAvailable = riskDetailResult.ActionPlanAvailable;
            model.ActionPlan = riskDetailResult.ActionPlan;
            if (riskDetailResult.TargetDate != "N/A")
            {
              DateTime dt = Convert.ToDateTime(riskDetailResult.TargetDate);
              model.TargetDate = String.Format("{0:MM/dd/yyyy}", dt);
            }
            else
            {
              model.TargetDate = riskDetailResult.TargetDate.ToString();
            }
            model.ActionPlan_Status = riskDetailResult.ActionPlan_Status;
            model.List_risk_associated = riskDetailResult.List_risk_associated;
            model.Owner = riskDetailResult.Owner;
            list.Add(model);
          }
        }
      }
      else
      {
        var riskDetails = db.RiskDetails.ToList();
        foreach (var riskDetail in riskDetails)
        {
          if (riskDetails.Count > 0)
          {
            var model = new RiskManagementModel();
            model = new RiskManagementModel();
            model.RiskDetailID = riskDetail.RiskDetailID;
            model.CompanyId = riskDetail.CompanyID ?? 0;
            model.DepartmentID = riskDetail.DepartmentID ?? 0;
            model.RiskID = riskDetail.RiskID ?? 0;
            model.RiskName = riskDetail.RiskMaster.RiskName;
            model.SubRiskID = riskDetail.SubRiskID ?? 0;
            model.SubRiskName = riskDetail.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetail.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
            model.MitigantID = riskDetail.MitigantID ?? 0;
            model.MitigantName = riskDetail.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetail.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
            model.Issue = riskDetail.Issue;
            model.Issue_Severity = riskDetail.Issue_Severity;
            model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
            model.ActionPlan = riskDetail.ActionPlan;
            if (riskDetail.TargetDate != "N/A")
            {
              DateTime dt = Convert.ToDateTime(riskDetail.TargetDate);
              model.TargetDate = String.Format("{0:MM/dd/yyyy}", dt);
            }
            else
            {
              model.TargetDate = riskDetail.TargetDate.ToString();
            }
            model.ActionPlan_Status = riskDetail.ActionPlan_Status;
            model.List_risk_associated = riskDetail.List_risk_associated;
            model.Owner = riskDetail.Owner;
            list.Add(model);
          }
        }
      }
      //var query = list.GroupBy(x => new { x.RiskID, x.SubRiskID }); 

      return View(list);
    }

    [HttpPost]
    public string SpecificRiskDetails(string compId, string deptId, string riskAssessmentName, string reportType, string value)
    {
      string res = "0";

      int companyId = 0;
      if (compId != "Select Company" && compId != "")
      {
        companyId = Convert.ToInt32(compId);
      }
      int departmentId = 0;
      if (deptId != "Select Department" && deptId != "")
      {
        departmentId = Convert.ToInt32(deptId);
      }
      if (companyId == 0 && departmentId == 0)
      {
        int riskId = Convert.ToInt32(riskAssessmentName);
        var riskList = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == riskId).FirstOrDefault();
        companyId = Convert.ToInt32(riskList.CompanyID);
        departmentId = Convert.ToInt32(riskList.DepartmentID);
      }

      List<StandardReportModel> list = new List<StandardReportModel>();
      var roles = Roles.GetRolesForUser().Single();
      var lstRiskDetalModel = new List<RiskDetailsModel>();

      List<RiskDetailsModel> riskDetailsData;
      List<CompanyMaster> companiesList = new List<CompanyMaster>();
      if (User.IsInRole("Admin"))
      {

        if (reportType == "1")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.ActionPlanAvailable == "Yes").ToList();

        }
        else if (reportType == "2")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId &&
            (x.Mitigant_Importance == "VH" || x.Mitigant_Importance == "H") && (x.Inherenet_risk_rating == "VH" || x.Inherenet_risk_rating == "H")).ToList();
        }
        else if (reportType == "3")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Issue_Severity == "H").ToList();
        }
        else if (reportType == "4")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Issue_Severity == "VH").ToList();
        }
        else if (reportType == "5")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId
              && x.Mitigant_effectiveness == "Effective").ToList();
        }
        else if (reportType == "6")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Impact == "H").ToList();
        }
        else if (reportType == "7")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Impact == "VH").ToList();
        }
        else if (reportType == "8")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "H").ToList();
        }
        else if (reportType == "9")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "VH").ToList();
        }
        else if (reportType == "10")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "C").ToList();
        }
        else if (reportType == "11")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
        }
        else if (reportType == "12")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        }
        else
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        }

        if (riskDetailsData.Count == 0 && companyId == 0 && departmentId != 0)
        {


          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId).ToList();

          foreach (var items1 in riskDetailsData)
          {
            //items1.Issue_Severity
            lstRiskDetalModel.Add(items1);
          }
        }

        if (riskDetailsData.Count == 0 && companyId != 0 && departmentId == 0)
        {


          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId).ToList();

          foreach (var items1 in riskDetailsData)
          {
            //items1.Issue_Severity
            lstRiskDetalModel.Add(items1);
          }
        }



        if (riskDetailsData.Count == 0)
        {
          return res;
          //var companies = db.CompaniesMaster.ToList();
          //foreach (var item in companies)
          //{
          //  riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId).ToList();

          //  foreach (var items1 in riskDetailsData)
          //  {
          //    //items1.Issue_Severity
          //    lstRiskDetalModel.Add(items1);
          //  }
          //}
        }
        else//if (riskDetailsData != null)
        {
          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);
          }
        }
        //}
      }
      if (User.IsInRole("Consultant"))
      {

        if (reportType == "1")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.ActionPlanAvailable == "Yes").ToList();
        }
        else if (reportType == "2")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId
              && (x.Mitigant_Importance == "VH" || x.Mitigant_Importance == "H")
              && (x.Inherenet_risk_rating == "VH" || x.Inherenet_risk_rating == "H")).ToList();
        }
        else if (reportType == "3")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Issue_Severity == "H").ToList();
        }
        else if (reportType == "4")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Issue_Severity == "VH").ToList();
        }
        else if (reportType == "5")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Mitigant_effectiveness == "Effective").ToList();
        }
        else if (reportType == "6")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Impact == "H").ToList();
        }
        else if (reportType == "7")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Impact == "VH").ToList();
        }
        else if (reportType == "8")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "H").ToList();
        }
        else if (reportType == "9")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "VH").ToList();
        }
        else if (reportType == "10")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "C").ToList();
        }
        else if (reportType == "11")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
        }
        else if (reportType == "12")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        }
        else
        { riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList(); }


        if (riskDetailsData.Count == 0 && companyId == 0 && departmentId != 0)
        {


          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId).ToList();

          foreach (var items1 in riskDetailsData)
          {
            //items1.Issue_Severity
            lstRiskDetalModel.Add(items1);
          }
        }

        if (riskDetailsData.Count == 0 && companyId != 0 && departmentId == 0)
        {


          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId).ToList();

          foreach (var items1 in riskDetailsData)
          {
            //items1.Issue_Severity
            lstRiskDetalModel.Add(items1);
          }
        }



        if (riskDetailsData.Count == 0)
        {
          return res;
          //var companies = db.CompaniesMaster.ToList();
          //foreach (var item in companies)
          //{
          //  riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId).ToList();

          //  foreach (var items1 in riskDetailsData)
          //  {
          //    //items1.Issue_Severity
          //    lstRiskDetalModel.Add(items1);
          //  }
          //}
        }
        else//if (riskDetailsData != null)
        {
          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);
          }
        }
        // }
      }

      if (User.IsInRole("Company"))
      {

        if (reportType == "1")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.ActionPlanAvailable == "Yes" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "2")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && (x.Mitigant_Importance == "VH" || x.Mitigant_Importance == "H") && (x.Inherenet_risk_rating == "VH" || x.Inherenet_risk_rating == "H") && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "3")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.Issue_Severity == "H" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "4")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.Issue_Severity == "VH" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "5")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId &&
              x.Mitigant_effectiveness == "Effective" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "6")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.SubRisk_Impact == "H" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "7")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.SubRisk_Impact == "VH" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "8")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "H" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "9")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.SubRisk_Likelyhood == "VH" && x.CompanyID == companyId).ToList();
        }
        else if (reportType == "10")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "C").ToList();
        }
        else if (reportType == "11")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.LossesAssociatedWithThisRisk == "Yes").ToList();
        }
        else if (reportType == "12")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        }
        else
        { riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId && x.CompanyID == companyId).ToList(); }
        if (riskDetailsData.Count == 0)
        {
          return res;
          //var companies = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).ToList();
          //foreach (var item in companies)
          //{
          //  riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == item.CompanyId).ToList();
          //  foreach (var items in riskDetailsData)
          //  {
          //    lstRiskDetalModel.Add(items);
          //  }
          //}
        }
        else//    if (riskDetailsData != null)
        {
          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);
          }
        }
        // }
      }


      if (lstRiskDetalModel != null && lstRiskDetalModel.Count > 0)
      {
        foreach (var item in lstRiskDetalModel)
        {
          var lstDetails = new StandardReportModel();
          lstDetails.RiskDetailID = Convert.ToInt32(item.RiskDetailID);
          lstDetails.CompanyId = Convert.ToInt32(item.CompanyID);
          lstDetails.CompanyName = item.CompanyMaster.ComapnyName;
          lstDetails.DepartmentID = Convert.ToInt32(item.DepartmentID);
          lstDetails.DepartmentName = item.DepartmentMaster.DepartmentName;
          lstDetails.SubRisk_Impact = item.SubRisk_Impact;
          lstDetails.SubRisk_Likelyhood = item.SubRisk_Likelyhood;
          lstDetails.Inherenet_risk_rating = item.Inherenet_risk_rating;
          lstDetails.MitigantID = Convert.ToInt32(item.MitigantID);
          lstDetails.Mitigant_whyEffective = item.Mitigant_whyEffective;
          lstDetails.Issue_Severity = item.Issue_Severity;
          lstDetails.ActionPlanAvailable = item.ActionPlanAvailable;
          lstDetails.Shared_process_Department = item.Shared_process_Department;
          lstDetails.Reason_Risk_acceptance = item.Reason_Risk_acceptance;
          lstDetails.Shared_process_Description = item.Shared_process_Description;
          lstDetails.CompanyObjectives = item.CompanyObjectives;
          lstDetails.Mitigant_Importance = item.Mitigant_Importance;
          lstDetails.RiskID = Convert.ToInt32(item.RiskID);
          lstDetails.SubRiskID = Convert.ToInt32(item.SubRiskID);
          lstDetails.Risk = item.RiskMaster.RiskName;
          lstDetails.subRisk = item.SubRiskMaster.SubRiskName;
          lstDetails.Mitigant = item.MitigantMaster.MitigantName;
          lstDetails.MitigantEffectivness = item.Mitigant_effectiveness;
          lstDetails.Issues = item.Issue;
          lstDetails.ActionPlan = item.ActionPlan;
          lstDetails.ActionPlanStatus = item.ActionPlan_Status;
          lstDetails.Owner = item.Owner;
          if (item.TargetDate != "N/A")
          {
            DateTime dt = Convert.ToDateTime(item.TargetDate);
            lstDetails.TargetDate = String.Format("{0:MM/dd/yyyy}", dt);
          }
          else
          {
            lstDetails.TargetDate = item.TargetDate.ToString();
          }
          lstDetails.RiskAssesmentName = item.RiskAssessmentMaster.RiskAssessmentName;
          lstDetails.IsThisRiskBeingAccepted = item.IsThisRiskBeingAccepted;
          lstDetails.List_risk_associated = item.List_risk_associated;
          lstDetails.Lossess = item.Losses;
          if (value == "Has Action Plan")
          {
            value = "Action Plan";
          }
          if (item.Status == "C")
          {
            lstDetails.Status = "Approved";
          }
          else
          {
            lstDetails.Status = "In Progress";
          }
          lstDetails.ReportType = value;

          list.Add(lstDetails);
        }
      }

      if (riskAssessmentName != "")
      {
        var riskId = Convert.ToInt32(riskAssessmentName);
        var newList = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == riskId).FirstOrDefault();
        list = list.Where(x => x.RiskAssesmentName == newList.RiskAssessmentName).ToList();
      }

      if (lstRiskDetalModel.Count > 0)
      {
        if (reportType == "10")
        {
          this.HttpContext.Session["ReportName"] = "AcceptedReport.rpt";
          this.HttpContext.Session["rptSource"] = list;
          res = "1";
        }
        else if (reportType == "11")
        {
          this.HttpContext.Session["ReportName"] = "LossReport.rpt";
          this.HttpContext.Session["rptSource"] = list;
          res = "1";

        }
        else if (reportType == "12")
        {
          this.HttpContext.Session["ReportName"] = "AssessmentReport.rpt";
          this.HttpContext.Session["rptSource"] = list;
          res = "1";

        }
        else
        {
          this.HttpContext.Session["ReportName"] = "ReportType.rpt";
          this.HttpContext.Session["rptSource"] = list;
          res = "1";

        }
      }
      return res;
    }

    public ActionResult BoardRisk(int? cId, int? dId)
    {
      var companyId = Session["compId"] + "";
      var departmentId = Session["deptId"] + "";
      if ((cId == null || cId == 0) && !String.IsNullOrWhiteSpace(companyId) && companyId != "0")
      {
        cId = Convert.ToInt32(companyId);
      }

      if ((dId == null || dId == 0) && !String.IsNullOrWhiteSpace(departmentId) && departmentId != "0")
      {
        dId = Convert.ToInt32(departmentId);
      }

      var riskAssesmentList = new List<RiskAssessmentMaster>();
      if (cId != null && dId != null)
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
      }
      else if (cId != null)
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId).ToList();
      }
      else if (dId != null)
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == dId).ToList();
      }
      else
      {
        riskAssesmentList = db.RiskAssessmentMaster.ToList();
      }
      //ViewBag.compId = cId;
      //ViewBag.deptId = dId;
      //var riskAssesmentList = new List<RiskAssessmentMaster>();
      //if (User.IsInRole("Consultant"))
      //{
      //  if (cId != null && dId != null)
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
      //  }
      //  else
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.ToList();
      //  }
      //}

      //if (User.IsInRole("Admin"))
      //{
      //  if (cId != null && dId != null)
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
      //  }
      //  else
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.ToList();
      //  }
      //}
      //if (User.IsInRole("Company"))
      //{
      //  var userId = WebSecurity.CurrentUserId;
      //  var list = db.CompanyUsers.Where(x => x.UserId == userId).FirstOrDefault();

      //  if (cId != null && dId != null)
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
      //  }
      //  else
      //  {
      //    riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == list.CompanyId).ToList();
      //  }
      //}
      return View(riskAssesmentList.OrderByDescending(x => x.InsertedDate).ThenBy(x => x.Status));
    }

    [HttpPost]
    public ActionResult BoardRisk(int? ids)
    {

      int deptId = Request["hfDeptId"] != "" ? Convert.ToInt32(Request["hfDeptId"]) : 0;
      int compId = Request["hfCompId"] != "" ? Convert.ToInt32(Request["hfCompId"]) : 0;
      Session["compId"] = compId;
      Session["deptId"] = deptId;
      string riskAssesment = Request["txtRiskAssesmentName"] != "" ? Request["txtRiskAssesmentName"] : "";
      var riskAssesmentList = new List<RiskAssessmentMaster>();
      ViewBag.CompId = compId;
      ViewBag.DeptId = deptId;
      if (riskAssesment != null && riskAssesment != "")
      {
        string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (String.IsNullOrWhiteSpace(ip))
        {
          ip = Request.ServerVariables["REMOTE_ADDR"];
        }
        var riskAssesmentMaster = new RiskAssessmentMaster();
        riskAssesmentMaster.RiskAssessmentName = riskAssesment;
        riskAssesmentMaster.CompanyID = compId;
        riskAssesmentMaster.DepartmentID = deptId;
        riskAssesmentMaster.Status = "P";
        riskAssesmentMaster.InsertedBy = WebSecurity.CurrentUserId;
        riskAssesmentMaster.Insertedmachineinfo = ip;
        riskAssesmentMaster.InsertedDate = DateTime.Now;
        db.RiskAssessmentMaster.Add(riskAssesmentMaster);
        db.SaveChanges();
        int riskAssesmentId = riskAssesmentMaster.RiskAssessmentId;
        if (User.IsInRole("Company"))
        {
          return RedirectToAction("RiskDetails", "Orm", new
          {
            id = riskAssesmentId,
            compId = riskAssesmentMaster.CompanyID,
            deptId = riskAssesmentMaster.DepartmentID
          });
        }
        {
          return RedirectToAction("RiskDetails", "Orm", new
          {
            id = riskAssesmentId,
            compId = riskAssesmentMaster.CompanyID,
            deptId = riskAssesmentMaster.DepartmentID
          });
        }
        //return View(db.RiskAssessmentMaster.OrderByDescending(x => x.Status).ToList());   
      }

      //var listDepartmentRisks = new List<RiskDetailsModel>();
      if (User.IsInRole("Consultant"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}                
      }
      if (User.IsInRole("Admin"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}
      }
      if (User.IsInRole("Company"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        if (deptId > 0)
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}
      }

      return View(riskAssesmentList.OrderByDescending(x => x.Status));
    }

    public ActionResult BoardRiskDetails(int? id, int? compId, int? deptId)
    {
      ViewBag.compId = compId;
      ViewBag.deptId = deptId;
      ViewBag.RiskAssesmentId = id;
      if (compId == 0 && deptId == 0)
      {
        var riskList = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == id).FirstOrDefault();
        compId = riskList.CompanyID;
        deptId = riskList.DepartmentID;
        ViewBag.compId = compId;
        ViewBag.deptId = deptId;
        ViewBag.riskheaderName = riskList.RiskAssessmentName;
      }
      var companyName = db.CompaniesMaster.Where(x => x.CompanyId == compId).FirstOrDefault();
      if (deptId == 0)
      {
        var riskList1 = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == id).FirstOrDefault();
        deptId = riskList1.DepartmentID;
        ViewBag.deptId = deptId;
        ViewBag.riskheaderName = riskList1.RiskAssessmentName;
      }
      var deptName = db.DepartmentsMaster.Where(x => x.DepartmentId == deptId).FirstOrDefault();
      ViewBag.companyName = companyName.ComapnyName;
      ViewBag.departmentName = deptName.DepartmentName;

      List<RiskManagementModel> list = new List<RiskManagementModel>();
      var roles = Roles.GetRolesForUser().Single();
      if (roles.ToLower() == "Company")
      {
        //  var companyUserId = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();
        var riskDetail = db.RiskDetails.Where(x => x.CompanyID == compId).ToList();
        foreach (var riskDetailResult in riskDetail)
        {
          if (riskDetailResult != null && riskDetailResult.RiskDetailID > 0)
          {
            var model = new RiskManagementModel();
            model.RiskDetailID = riskDetailResult.RiskDetailID;
            model.CompanyId = riskDetailResult.CompanyID ?? 0;
            model.DepartmentID = riskDetailResult.DepartmentID ?? 0;
            model.RiskID = riskDetailResult.RiskID ?? 0;
            model.RiskName = riskDetailResult.RiskMaster.RiskName;
            model.SubRiskID = riskDetailResult.SubRiskID ?? 0;
            model.SubRiskName = riskDetailResult.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetailResult.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetailResult.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetailResult.Inherenet_risk_rating;
            model.MitigantID = riskDetailResult.MitigantID ?? 0;
            model.MitigantName = riskDetailResult.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetailResult.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetailResult.Mitigant_effectiveness;
            model.Issue = riskDetailResult.Issue;
            model.Issue_Severity = riskDetailResult.Issue_Severity;
            model.ActionPlanAvailable = riskDetailResult.ActionPlanAvailable;
            model.ActionPlan = riskDetailResult.ActionPlan;
            if (riskDetailResult.TargetDate != "N/A")
            {
              DateTime dt = Convert.ToDateTime(riskDetailResult.TargetDate);
              model.TargetDate = String.Format("{0:MM/dd/yyyy}", dt);
            }
            else
            {
              model.TargetDate = riskDetailResult.TargetDate.ToString();
            }
            model.ActionPlan_Status = riskDetailResult.ActionPlan_Status;
            model.List_risk_associated = riskDetailResult.List_risk_associated;
            model.Owner = riskDetailResult.Owner;
            list.Add(model);
          }
        }
      }
      else
      {
        var riskDetails = db.RiskDetails.ToList();
        foreach (var riskDetail in riskDetails)
        {
          if (riskDetails.Count > 0)
          {
            var model = new RiskManagementModel();
            model = new RiskManagementModel();
            model.RiskDetailID = riskDetail.RiskDetailID;
            model.CompanyId = riskDetail.CompanyID ?? 0;
            model.DepartmentID = riskDetail.DepartmentID ?? 0;
            model.RiskID = riskDetail.RiskID ?? 0;
            model.RiskName = riskDetail.RiskMaster.RiskName;
            model.SubRiskID = riskDetail.SubRiskID ?? 0;
            model.SubRiskName = riskDetail.SubRiskMaster.SubRiskName;
            model.SubRisk_Impact = riskDetail.SubRisk_Impact;
            model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
            model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
            model.MitigantID = riskDetail.MitigantID ?? 0;
            model.MitigantName = riskDetail.MitigantMaster.MitigantName;
            model.Mitigant_Importance = riskDetail.Mitigant_Importance;
            model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
            model.Issue = riskDetail.Issue;
            model.Issue_Severity = riskDetail.Issue_Severity;
            model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
            model.ActionPlan = riskDetail.ActionPlan;
            if (riskDetail.TargetDate != "N/A")
            {
              DateTime dt = Convert.ToDateTime(riskDetail.TargetDate);
              model.TargetDate = String.Format("{0:MM/dd/yyyy}", dt);
            }
            else
            {
              model.TargetDate = riskDetail.TargetDate.ToString();
            }
            model.ActionPlan_Status = riskDetail.ActionPlan_Status;
            model.List_risk_associated = riskDetail.List_risk_associated;
            model.Owner = riskDetail.Owner;
            list.Add(model);
          }
        }
      }
      //var query = list.GroupBy(x => new { x.RiskID, x.SubRiskID }); 

      return View(list);
    }

    [HttpPost]
    public string BoardRiskDetails(string compId, string deptId, string riskRating, string issueSaverty, string subriskImpact,
    string subriskMethod, string subriskLikehood, string mitigantImportance, string riskAssessmentName)
    {
      string res = "0";
      int companyId = 0;
      if (compId != "Select Company" && compId != "")
      {
        companyId = Convert.ToInt32(compId);
      }
      int departmentId = 0;
      if (deptId != "Select Department" && deptId != "")
      {
        departmentId = Convert.ToInt32(deptId);
      }

      if (departmentId == 0 && riskAssessmentName != "")
      {
        int riskId = Convert.ToInt32(riskAssessmentName);
        var riskList = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == riskId).FirstOrDefault();
        companyId = Convert.ToInt32(riskList.CompanyID);
        departmentId = Convert.ToInt32(riskList.DepartmentID);
      }

      List<StandardReportModel> list = new List<StandardReportModel>();
      var roles = Roles.GetRolesForUser().Single();
      var listRiskDetails = db.RiskDetails.Where(x => x.Status == "C").ToList();
      var lstRiskDetalModel = new List<RiskDetailsModel>();

      var predicate = PredicateBuilder.True<RiskDetailsModel>();
      var orPredicate = PredicateBuilder.False<RiskDetailsModel>();


      bool andFlag = false;
      bool orFlag = false;

      if (compId != null && compId != "" && compId != "Select Company")
      {
        andFlag = true;
        predicate = predicate.And(v => v.CompanyID == companyId);
      }
      if (deptId != null && deptId != "" && deptId != "Select Department")
      {
        andFlag = true;
        predicate = predicate.And(v => v.DepartmentID == departmentId);
      }
      if (riskRating != "" && riskRating != null && riskRating != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.Inherenet_risk_rating == riskRating);
      }
      if (issueSaverty != "" && issueSaverty != null && issueSaverty != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.Issue_Severity == issueSaverty);
      }
      if (subriskImpact != "" && subriskImpact != null && subriskImpact != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.SubRisk_Impact == subriskImpact);
      }
      if (subriskLikehood != "" && subriskLikehood != null && subriskLikehood != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.SubRisk_Likelyhood == subriskLikehood);
      }
      if (mitigantImportance != "" && mitigantImportance != null && mitigantImportance != "Select")
      {
        orFlag = false;
        orPredicate = orPredicate.Or(v => v.Mitigant_Importance == mitigantImportance);
      }
      lstRiskDetalModel = GetPredicate(predicate, orPredicate, andFlag, orFlag);

      if (lstRiskDetalModel != null && lstRiskDetalModel.Count > 0)
      {
        foreach (var item in lstRiskDetalModel)
        {
          var lstDetails = new StandardReportModel();
          lstDetails.RiskDetailID = Convert.ToInt32(item.RiskDetailID);
          lstDetails.CompanyId = Convert.ToInt32(item.CompanyID);
          lstDetails.CompanyName = item.CompanyMaster.ComapnyName;
          lstDetails.DepartmentID = Convert.ToInt32(item.DepartmentID);
          lstDetails.DepartmentName = item.DepartmentMaster.DepartmentName;
          lstDetails.SubRisk_Impact = item.SubRisk_Impact;
          lstDetails.SubRisk_Likelyhood = item.SubRisk_Likelyhood;
          lstDetails.Inherenet_risk_rating = item.Inherenet_risk_rating;
          lstDetails.MitigantID = Convert.ToInt32(item.MitigantID);
          lstDetails.Mitigant_whyEffective = item.Mitigant_whyEffective;
          lstDetails.Issue_Severity = item.Issue_Severity;
          lstDetails.ActionPlanAvailable = item.ActionPlanAvailable;
          lstDetails.Shared_process_Department = item.Shared_process_Department;
          lstDetails.Reason_Risk_acceptance = item.Reason_Risk_acceptance;
          lstDetails.Shared_process_Description = item.Shared_process_Description;
          lstDetails.CompanyObjectives = item.CompanyObjectives;

          lstDetails.RiskID = Convert.ToInt32(item.RiskID);
          lstDetails.SubRiskID = Convert.ToInt32(item.SubRiskID);
          lstDetails.Risk = item.RiskMaster.RiskName;
          lstDetails.subRisk = item.SubRiskMaster.SubRiskName;
          lstDetails.Mitigant = item.MitigantMaster.MitigantName;
          lstDetails.MitigantEffectivness = item.Mitigant_effectiveness;
          lstDetails.Issues = item.Issue;
          lstDetails.ActionPlan = item.ActionPlan;
          lstDetails.ActionPlanStatus = item.ActionPlan_Status;
          lstDetails.Owner = item.Owner;
          if (item.TargetDate != "N/A")
          {
            DateTime dt = Convert.ToDateTime(item.TargetDate);
            lstDetails.TargetDate = String.Format("{0:MM/dd/yyyy}", dt);
          }
          else
          {
            lstDetails.TargetDate = item.TargetDate.ToString();
          }

          lstDetails.Mitigant_Importance = item.Mitigant_Importance;
          lstDetails.IsThisRiskBeingAccepted = item.IsThisRiskBeingAccepted;
          lstDetails.List_risk_associated = item.List_risk_associated;

          if (item.Status == "C")
          {
            lstDetails.Status = "Approved";
          }
          else
          {
            lstDetails.Status = "In Progress";
          }
          if (item.RiskAssessmentMaster != null)
          {
            lstDetails.RiskAssesmentName = item.RiskAssessmentMaster.RiskAssessmentName;
            //lstDetails.Status = item.RiskAssessmentMaster.Status;
            lstDetails.InsertDate = Convert.ToDateTime(item.RiskAssessmentMaster.InsertedDate);
          }
          list.Add(lstDetails);
        }
      }

      //Temp solution
      //if (compId != null && compId != "" && compId != "Select Company")
      //{
      //  list = list.Where(x => x.CompanyId == companyId).ToList();
      //}
      if (riskAssessmentName != "")
      {
        int id = Convert.ToInt32(riskAssessmentName);
        var newList = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == id).FirstOrDefault();
        list = list.Where(x => x.RiskAssesmentName == newList.RiskAssessmentName).ToList();
      }

      if (lstRiskDetalModel.Count > 0)
      {
        this.HttpContext.Session["ReportName"] = "NewBoardReport.rpt";
        this.HttpContext.Session["rptSource"] = list;
        res = "1";
      }
      return res;
    }

  }
}
