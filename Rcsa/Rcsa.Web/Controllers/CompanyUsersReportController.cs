using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
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

namespace Rcsa.Web.Controllers
{
 
  [Authorize]
  public class CompanyUsersReportController : Controller
  {
    //
    // GET: /CompanyUsersReport/
    RcsaDb db = new RcsaDb();
    public ActionResult Index()
    {
      return View();
    }

   
    public ActionResult UsersReport()
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
    public ActionResult UsersReport(RiskManagementModel model)
    {

      string companyId = Request["CompanyId"] != null ? Request["CompanyId"] : null;//model.CompanyId.ToString();
      string departmentId = Request["DepartmentID"] != null ? Request["DepartmentID"] : null;//model.DepartmentID.ToString();
      string RiskRating = Request["Inherenet_risk_rating"] != null ? Request["Inherenet_risk_rating"] : null;
      string IssueSaverity = Request["Issue_Severity"] != null ? Request["Issue_Severity"] : null;
      string SubRiskImpact = Request["SubRisk_Impact"] != null ? Request["SubRisk_Impact"] : null;
      string SubRiskLikeHood = Request["SubRisk_Likelyhood"] != null ? Request["SubRisk_Likelyhood"] : null;
      string MitigantImportance = Request["Mitigant_Importance"] != null ? Request["Mitigant_Importance"] : null;

      List<RiskManagementModel> list = new List<RiskManagementModel>();
      var roles = Roles.GetRolesForUser().Single();
      var listRiskDetails = db.RiskDetails.Where(x => x.Status == "C").ToList();

      listRiskDetails = listRiskDetails.Where(x => companyId.Contains(x.CompanyID.ToString()) && departmentId.Contains(x.DepartmentID.ToString())
      && RiskRating.Contains(x.Inherenet_risk_rating) && IssueSaverity.Contains(x.Issue_Severity) && SubRiskImpact.Contains(x.SubRisk_Impact)
      && SubRiskLikeHood.Contains(x.SubRisk_Likelyhood) && MitigantImportance.Contains(x.Mitigant_Importance)).ToList();

      if (listRiskDetails.Count > 0)
      {
        if (roles.ToLower() == "admin")
        {
          foreach (var data in listRiskDetails)
          {
            RiskManagementModel listModel = new RiskManagementModel();

            listModel.RiskID = Convert.ToInt32(data.RiskID);
            listModel.SubRiskID = Convert.ToInt32(data.SubRiskID);
            listModel.RiskName = data.RiskMaster.RiskName;
            listModel.SubRiskName = data.SubRiskMaster.SubRiskName;
            listModel.MitigantName = data.MitigantMaster.MitigantName;
            listModel.Mitigant_Importance = data.Mitigant_Importance;
            listModel.Mitigant_effectiveness = data.Mitigant_effectiveness;
            listModel.Issue = data.Issue;
            listModel.ActionPlan = data.ActionPlan;
            listModel.TargetDate = data.TargetDate.ToString();
            listModel.ActionPlan_Status = data.ActionPlan_Status;
            listModel.Owner = data.Owner;
            list.Add(listModel);
          }
          return View(list);
        }
        else
        {
          var companyUserId = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();
          var riskDetail = listRiskDetails.Where(x => x.CompanyID == companyUserId.CompanyId).ToList();
          foreach (var data in riskDetail)
          {
            RiskManagementModel listModel = new RiskManagementModel();
            listModel.RiskID = Convert.ToInt32(data.RiskID);
            listModel.SubRiskID = Convert.ToInt32(data.SubRiskID);
            listModel.RiskName = data.RiskMaster.RiskName;
            listModel.SubRiskName = data.SubRiskMaster.SubRiskName;
            listModel.MitigantName = data.MitigantMaster.MitigantName;
            listModel.Mitigant_Importance = data.Mitigant_Importance;
            listModel.Mitigant_effectiveness = data.Mitigant_effectiveness;
            listModel.Issue = data.Issue;
            listModel.ActionPlan = data.ActionPlan;
            listModel.TargetDate = data.TargetDate.ToString();
            listModel.ActionPlan_Status = data.ActionPlan_Status;
            listModel.Owner = data.Owner;
            list.Add(listModel);
          }
          return View(list);
        }
      }
      else
      {
        return View(list);
      }
    }

    //
    // GET: /CompanyUsersReport/Details/5

    public ActionResult Details(int id)
    {
      return View();
    }

    //
    // GET: /CompanyUsersReport/Create

    public ActionResult Create()
    {
      return View();
    }

    //
    // POST: /CompanyUsersReport/Create

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
    // GET: /CompanyUsersReport/Edit/5

    public ActionResult Edit(int id)
    {
      return View();
    }

    //
    // POST: /CompanyUsersReport/Edit/5

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
    // GET: /CompanyUsersReport/Delete/5

    public ActionResult Delete(int id)
    {
      return View();
    }

    //
    // POST: /CompanyUsersReport/Delete/5

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


    public JsonResult DepartmentList(string CompanyId)
    {
      if (CompanyId == "")
      {
        //return Json(new SelectList("", 0));
        List<DepartmentMaster> department = db.DepartmentsMaster.ToList();
        return Json(new SelectList(
                          department.ToArray(),
                          "DepartmentId",
                          "DepartmentName"), JsonRequestBehavior.AllowGet);


      }
      else
      {
        int companyId = Convert.ToInt32(CompanyId);
        List<DepartmentMaster> department = db.DepartmentsMaster.Where(x => x.CompanyId == companyId).ToList();
        if (HttpContext.Request.IsAjaxRequest())
          return Json(new SelectList(
                              department.ToArray(),
                              "DepartmentId",
                              "DepartmentName"), JsonRequestBehavior.AllowGet);
        return Json(new SelectList(
                            department.ToArray(),
                            "DepartmentId",
                            "DepartmentName"), JsonRequestBehavior.AllowGet);
      }
    }

    public JsonResult RiskAssessmentList(string departmentId)
    {
      if (departmentId == "")
      {
        List<RiskAssessmentMaster> risk = new List<RiskAssessmentMaster>();
        if (User.IsInRole("Company"))
        {
          var compId = WebSecurity.CurrentUserId;
          risk = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId).ToList();
        }
        else
        {
          //return Json(new SelectList("", 0));
          risk = db.RiskAssessmentMaster.ToList();
        }
        return Json(new SelectList(
                          risk.ToArray(),
                          "RiskAssessmentId",
                          "RiskAssessmentName"), JsonRequestBehavior.AllowGet);


      }
      else
      {
        int depttId = Convert.ToInt32(departmentId);
        List<RiskAssessmentMaster> risk = db.RiskAssessmentMaster.Where(x => x.DepartmentID == depttId).ToList();
        if (HttpContext.Request.IsAjaxRequest())
          return Json(new SelectList(
                              risk.ToArray(),
                              "RiskAssessmentId",
                              "RiskAssessmentName"), JsonRequestBehavior.AllowGet);
        return Json(new SelectList(
                            risk.ToArray(),
                            "RiskAssessmentId",
                            "RiskAssessmentName"), JsonRequestBehavior.AllowGet);
      }
    }

    [HttpPost]
    public string ReportResult(string compId, string deptId, string riskRating, string issueSaverty, string subriskImpact,
    string subriskMethod, string subriskLikehood, string mitigantImportance)
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
      if (companyId == 0 && departmentId == 0 && riskRating != "All Risk Assessment")
      {
        list = list.Where(x => x.RiskAssesmentName == riskRating).ToList();
      }
      if (riskRating != "All Risk Assessment" && companyId != 0 && departmentId != 0)
      {
        list = list.Where(x => x.RiskAssesmentName == riskRating).ToList();
      }
      if (riskRating != "All Risk Assessment" && companyId == 0 && departmentId != 0)
      {
        list = list.Where(x => x.RiskAssesmentName == riskRating).ToList();
      }
      if (riskRating == "All Risk Assessment" && departmentId != 0)
      {
        list = list.Where(x => x.DepartmentID == departmentId).ToList();
      }
      if (lstRiskDetalModel.Count > 0)
      {
        this.HttpContext.Session["ReportName"] = "Department.rpt";//"LossReport.rpt";
        this.HttpContext.Session["rptSource"] = list;
        res = "1";
      }
      return res;
    }

    public ActionResult CustomizedReport()
    {
      return View();
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

    // [HttpPost]
    //public string CustomizedReportResult(string compId, string deptId, string riskRating, string issueSaverty, string subriskImpact,
    //string subriskMethod, string subriskLikehood, string mitigantImportance)
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
    //  var listRiskDetails = db.RiskDetails.Where(x => x.Status == "C").ToList();
    //  var lstRiskDetalModel = new List<RiskDetailsModel>();

    //  var predicate = PredicateBuilder.True<RiskDetailsModel>();
    //  if (compId != null && compId != "" && compId != "Select Company")
    //  {
    //    predicate = predicate.And(v => v.CompanyID == companyId);
    //  }
    //  if (deptId != null && deptId != "" && deptId != "Select Department")
    //  {
    //    predicate = predicate.And(v => v.DepartmentID == departmentId);
    //  }
    //  if (riskRating != "" && riskRating != null && riskRating != "Select")
    //  {
    //    predicate = predicate.And(v => v.Inherenet_risk_rating == riskRating);
    //  }
    //  if (issueSaverty != "" && issueSaverty != null && issueSaverty != "Select")
    //  {
    //    predicate = predicate.And(v => v.Issue_Severity == issueSaverty);
    //  }
    //  if (subriskImpact != "" && subriskImpact != null && subriskImpact != "Select")
    //  {
    //    predicate = predicate.And(v => v.SubRisk_Impact == subriskImpact);
    //  }
    //  if (subriskLikehood != "" && subriskLikehood != null && subriskLikehood != "Select")
    //  {
    //    predicate = predicate.And(v => v.SubRisk_Likelyhood == subriskLikehood);
    //  }
    //  if (mitigantImportance != "" && mitigantImportance != null && mitigantImportance != "Select")
    //  {
    //    predicate = predicate.And(v => v.Mitigant_Importance == mitigantImportance);
    //  }


    //  lstRiskDetalModel = GetPredicate(predicate);

    //  //listRiskDetails = listRiskDetails.Where(x => companyId.Contains(x.CompanyID.ToString()) && departmentId.Contains(x.DepartmentID.ToString())
    //  //&& RiskRating.Contains(x.Inherenet_risk_rating) && IssueSaverity.Contains(x.Issue_Severity) && SubRiskImpact.Contains(x.SubRisk_Impact)
    //  //&& SubRiskLikeHood.Contains(x.SubRisk_Likelyhood) && MitigantImportance.Contains(x.Mitigant_Importance)).ToList();


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
    //      lstDetails.RiskAssesmentName = item.RiskAssessmentMaster.RiskAssessmentName;
    //      lstDetails.Status = item.RiskAssessmentMaster.Status;
    //      lstDetails.InsertDate = Convert.ToDateTime(item.RiskAssessmentMaster.InsertedDate);
    //      list.Add(lstDetails);
    //    }
    //  }

    //  if (lstRiskDetalModel.Count > 0)
    //  {
    //    this.HttpContext.Session["ReportName"] = "CustomizedReport.rpt";
    //    this.HttpContext.Session["rptSource"] = list;
    //    res = "1";
    //  }
    //  return res;
    //}
    // }
  }
}
