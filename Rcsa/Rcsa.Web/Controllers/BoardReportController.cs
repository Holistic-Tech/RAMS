using LinqKit;
using Rcsa.Web.Models;
using Rcsa.Web.ReportForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using System.Linq.Expressions;

namespace Rcsa.Web.Controllers
{
  [Authorize]
  public class BoardReportController : Controller
  {
    //
    // GET: /BoardReport/
    RcsaDb db = new RcsaDb();
    public ActionResult Index()
    {
      return View();
    }

    //
    // GET: /BoardReport/Details/5

    public ActionResult BoardReports()
    {
      return View();
    }

    [HttpPost]
    public string BoardReportAll(string compId, string deptId)
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
      //var predicate = PredicateBuilder.True<RiskDetailsModel>();
      List<CompanyMaster> companiesList = new List<CompanyMaster>();
      if (User.IsInRole("Admin"))
      {
        //var companies = db.CompaniesMaster.ToList();
        //foreach (var item in companies)
        //{
        var riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        if (riskDetailsData.Count == 0 && deptId == "Select Department")
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
        //  var companyUsers = db.CompanyUsers.Where(x => x.CreatedBy == WebSecurity.CurrentUserId).ToList();
        //foreach (var users in companyUsers)
        // {
        var riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId).ToList();
        if (riskDetailsData.Count == 0 && deptId == "Select Department")
        {
          riskDetailsData = db.RiskDetails.Where(x => x.CompanyID == companyId).ToList();
          foreach (var items in riskDetailsData)
          {
            lstRiskDetalModel.Add(items);
          }
        }
        if (riskDetailsData.Count == 0)
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
        var riskDetailsData = db.RiskDetails.Where(x => x.DepartmentID == departmentId).ToList();

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
          list.Add(lstDetails);
        }
      }

      if (lstRiskDetalModel.Count > 0)
      {
        this.HttpContext.Session["ReportName"] = "BoardReport.rpt";
        this.HttpContext.Session["rptSource"] = list;
        res = "1";
      }
      return res;
    }


    [HttpPost]
    public string BoardReport(string compId, string deptId, string riskRating, string issueSaverty, string subriskImpact,
      string subriskMethod, string subriskLikehood, string mitigantImportance)
    {
      string res = "0";
      int companyId = 0;
      if (compId != "Select Company" && compId != "")
      {
        companyId = Convert.ToInt32(compId);
      }
      int departmentId = 0;
      if (deptId != "Select Departments" && deptId != "" && deptId != "All Departments")
      {
        departmentId = Convert.ToInt32(deptId);
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
     
      if (deptId != null && deptId != "" && deptId != "All Departments")
      {
        andFlag = true;
        predicate = predicate.And(v => v.DepartmentID == departmentId);
      }

      if (riskRating != "" && riskRating != null && riskRating != "Select")
      {
        orFlag = true;
        orPredicate = orPredicate.Or(v => v.Inherenet_risk_rating == riskRating);
      }
      if (issueSaverty != "" && issueSaverty != null && issueSaverty != "Select")
      {
        orFlag = true;
        orPredicate = orPredicate.Or(v => v.Issue_Severity == issueSaverty);
      }
      if (subriskImpact != "" && subriskImpact != null && subriskImpact != "Select")
      {
        orFlag = true;
        orPredicate = orPredicate.Or(v => v.SubRisk_Impact == subriskImpact);
      }
      if (subriskLikehood != "" && subriskLikehood != null && subriskLikehood != "Select")
      {
        orFlag = true;
        orPredicate = orPredicate.Or(v => v.SubRisk_Likelyhood == subriskLikehood);
      }
      if (mitigantImportance != "" && mitigantImportance != null && mitigantImportance != "Select")
      {
        orFlag = true;
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
          lstDetails.TargetDate = item.TargetDate.ToString();
          if (item.RiskAssessmentMaster != null)
          {
            lstDetails.RiskAssesmentName = item.RiskAssessmentMaster.RiskAssessmentName;
            lstDetails.Status = item.RiskAssessmentMaster.Status;
            lstDetails.InsertDate = Convert.ToDateTime(item.RiskAssessmentMaster.InsertedDate);
          }
          list.Add(lstDetails);
        }
      }

      //Temp solution
      if (compId != null && compId != "" && compId != "Select Company")
      {
        list = list.Where(x => x.CompanyId == companyId).ToList();
      }
      if (lstRiskDetalModel.Count > 0)
      {
        this.HttpContext.Session["ReportName"] = "Department.rpt";// "LossReport.rpt";
        this.HttpContext.Session["rptSource"] = list;
        res = "1";
      }
      return res;

    }
    //
    // GET: /BoardReport/Create

    public ActionResult Create()
    {
      return View();
    }

    //
    // POST: /BoardReport/Create

    [HttpPost]
    public ActionResult Create(FormCollection collection)
    {
      try
      {
        // TODO: Add insert logic here

        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /BoardReport/Edit/5

    public ActionResult Edit(int id)
    {
      return View();
    }

    //
    // POST: /BoardReport/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection)
    {
      try
      {
        // TODO: Add update logic here

        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /BoardReport/Delete/5

    public ActionResult Delete(int id)
    {
      return View();
    }

    //
    // POST: /BoardReport/Delete/5

    [HttpPost]
    public ActionResult Delete(int id, FormCollection collection)
    {
      try
      {
        // TODO: Add delete logic here

        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }

    public static List<RiskDetailsModel> GetPredicate(Expression<Func<RiskDetailsModel, bool>> predicate, Expression<Func<RiskDetailsModel, bool>> orPredicate,
                                                  bool andFlag, bool orFlag)
    {
      RcsaDb db = new RcsaDb();
      IQueryable<RiskDetailsModel> list = db.RiskDetails ;

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
  }
}
