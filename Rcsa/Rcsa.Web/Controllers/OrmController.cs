using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using Rcsa.Web.Models;
using WebMatrix.WebData;


namespace Rcsa.Web.Controllers
{
  [Authorize]
  public class OrmController : Controller
  {
    //
    // GET: /Orm/
    RcsaDb db = new RcsaDb();

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult RiskManagement(int id, int? compId, int? deptId)
    {
      ViewBag.compId = compId;
      ViewBag.deptId = deptId;

      var model = new RiskManagementModel();
      var company = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId);
      if (!company.Any())
      {
        return View(model);
      }

      var companyId = company.First().CompanyId;
      RiskDetailsModel riskDetail = db.RiskDetails.FirstOrDefault(x => x.RiskDetailID == id && x.CompanyID == companyId);
      if (riskDetail != null && riskDetail.RiskDetailID > 0)
      {
        ViewBag.Mode = "Edit";
        model.RiskDetailID = riskDetail.RiskDetailID;
        model.CompanyId = riskDetail.CompanyID ?? 0;
        model.DepartmentID = riskDetail.DepartmentID ?? 0;
        model.RiskID = riskDetail.RiskID ?? 0;
        RiskMaster objRiskMaster = riskDetail.RiskMaster;
        if (objRiskMaster != null)
        {
          model.RiskName = objRiskMaster.RiskName;
          model.RiskDescription = objRiskMaster.Description;
          objRiskMaster = null;
        }
        model.SubRiskID = riskDetail.SubRiskID ?? 0;
        SubRiskMaster objSubRiskMaster = riskDetail.SubRiskMaster;
        if (objSubRiskMaster != null)
        {
          model.SubRiskName = objSubRiskMaster.SubRiskName;
          model.SubRiskDescription = objSubRiskMaster.SubRiskDesc;
          objSubRiskMaster = null;
        }
        model.SubRisk_Impact = riskDetail.SubRisk_Impact;
        model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
        model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
        model.MitigantID = riskDetail.MitigantID ?? 0;
        MitigantMaster objMitigantMaster = riskDetail.MitigantMaster;
        if (objMitigantMaster != null)
        {
          model.MitigantName = objMitigantMaster.MitigantName;
          model.MitigantDescription = objMitigantMaster.MitigantDesc;
          objMitigantMaster = null;
        }
        model.Mitigant_Importance = riskDetail.Mitigant_Importance;
        model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
        model.Mitigant_whyEffective = riskDetail.Mitigant_whyEffective;
        model.Issue = riskDetail.Issue;
        model.Issue_Severity = riskDetail.Issue_Severity;
        model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
        model.IsThisRiskBeingAccepted = riskDetail.IsThisRiskBeingAccepted;
        model.LossesAssociatedWithThisRisk = riskDetail.LossesAssociatedWithThisRisk;
        model.Losses = riskDetail.Losses;

        model.ActionPlan = riskDetail.ActionPlan;
        model.TargetDate = riskDetail.TargetDate.ToString();
        model.ActionPlan_Status = riskDetail.ActionPlan_Status;
        model.Reason_Risk_acceptance = riskDetail.Reason_Risk_acceptance;
        model.List_risk_associated = riskDetail.List_risk_associated;
        model.Owner = riskDetail.Owner;
        model.Shared_process_Department = riskDetail.Shared_process_Department;
        model.Shared_process_Description = riskDetail.Shared_process_Description;
        model.insertedBy = riskDetail.insertedBy ?? 0;
        model.Insertedon = DateTime.Now;
        model.insertedmachineinfo = riskDetail.insertedmachineinfo;
        model.Updatedby = riskDetail.Updatedby ?? 0;
        model.updatedon = DateTime.Now;
        model.updatemachineinfo = riskDetail.updatemachineinfo;
        model.CompanyObjectives = riskDetail.CompanyObjectives;
      }
      else
      {
        var com = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId);
        if (com.Any())
        {
          model.CompanyId = com.First().CompanyId;
          model.TargetDate = DateTime.Now.ToString(); ;
        }
        model.TargetDate = DateTime.Now.ToString();
      }

      model.Departments = (from x in db.UserDepartments
                           join u in db.DepartmentsMaster on x.DepartmentId equals u.DepartmentId
                           where x.UserId == WebSecurity.CurrentUserId
                           select u).ToList();



      if (model.RiskID > 0)
      {
        model.Risks = db.RiskMasters.ToList();
        model.SubRisks = db.SubRisksMaster.Where(x => x.RiskId == model.RiskID).ToList();
      }
      else
      {
        model.Risks = new List<RiskMaster>();
        model.SubRisks = new List<SubRiskMaster>();
      }

      if (model.SubRiskID > 0)
        model.Mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == model.SubRiskID).ToList();
      else
        model.Mitigants = new List<MitigantMaster>();

      //model.Shared_process_Departments = db.DepartmentsMaster.ToList();
      model.Shared_process_Departments = model.Departments;

      return View(model);
    }

    [HttpPost]
    public ActionResult RiskManagement(RiskManagementModel riskModel, string command)
    {
      string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      if (String.IsNullOrWhiteSpace(ip))
      {
        ip = Request.ServerVariables["REMOTE_ADDR"];
      }
      int? cid = Request["hfCompId"] != "" ? Convert.ToInt32(Request["hfCompId"]) : 0;
      int? did = Request["hfDeptId"] != "" ? Convert.ToInt32(Request["hfDeptId"]) : 0;
      int? rid = Request["hfAssesmentId"] != "" ? Convert.ToInt32(Request["hfAssesmentId"]) : 0;
      int riskId = Request["hfRiskId"] != "" ? Convert.ToInt32(Request["hfRiskId"]) : 0;
      int subRiskId = Request["hfSubRiskId"] != "" ? Convert.ToInt32(Request["hfSubRiskId"]) : 0;
      int mitigantId = Request["hfMitigantId"] != "" ? Convert.ToInt32(Request["hfMitigantId"]) : 0;
      if (ModelState.IsValid)
      {
        var riskExists = db.RiskDetails.Where(x => x.RiskAssessmentID == rid && x.RiskID == riskId && x.SubRiskID == subRiskId && x.MitigantID == mitigantId).ToList();
        if (riskExists.Count > 0 && riskExists != null)
        {
          riskModel.Departments = db.DepartmentsMaster.ToList();
          riskModel.Risks = db.RiskMasters.ToList();
          if (riskModel.RiskID > 0)
            riskModel.SubRisks = db.SubRisksMaster.Where(x => x.RiskId == riskModel.RiskID).ToList();
          else
            riskModel.SubRisks = new List<SubRiskMaster>();

          if (riskModel.SubRiskID > 0)
            riskModel.Mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == riskModel.SubRiskID).ToList();
          else
            riskModel.Mitigants = new List<MitigantMaster>();

          riskModel.Shared_process_Departments = db.DepartmentsMaster.ToList();
          ViewBag.RiskDetailsId = riskExists.FirstOrDefault().RiskDetailID;
          ViewBag.RiskExists = "Risk Already Exists";
          return View(riskModel);
        }
        RiskDetailsModel riskDetail = db.RiskDetails.FirstOrDefault(x => x.RiskDetailID == riskModel.RiskDetailID) ??
                                      new RiskDetailsModel();
        //cid = riskModel.CompanyId;
        //did = riskModel.DepartmentID;
        riskDetail.RiskDetailID = riskModel.RiskDetailID;
        riskDetail.RiskAssessmentID = rid;//riskModel.RiskAssessmentId;
        riskDetail.CompanyID = riskModel.CompanyId;
        riskDetail.DepartmentID = riskModel.DepartmentID;
        riskDetail.RiskID = riskModel.RiskID != 0 ? riskModel.RiskID : riskId;
        //riskDetail.RiskID = riskModel.RiskID;
        riskDetail.SubRiskID = riskModel.SubRiskID != 0 ? riskModel.SubRiskID : subRiskId;
        //riskDetail.SubRiskID = riskModel.SubRiskID;
        riskDetail.SubRisk_Impact = riskModel.SubRisk_Impact;
        riskDetail.SubRisk_Likelyhood = riskModel.SubRisk_Likelyhood;
        riskDetail.Inherenet_risk_rating = riskModel.Inherenet_risk_rating;
        riskDetail.MitigantID = riskModel.MitigantID != 0 ? riskModel.MitigantID : mitigantId;
        //riskDetail.MitigantID = riskModel.MitigantID;
        riskDetail.Mitigant_Importance = riskModel.Mitigant_Importance;
        riskDetail.Mitigant_effectiveness = riskModel.Mitigant_effectiveness;
        riskDetail.Mitigant_whyEffective = riskModel.Mitigant_whyEffective;
        riskDetail.Issue = riskModel.Issue;
        riskDetail.Issue_Severity = riskModel.Issue_Severity;
        riskDetail.ActionPlanAvailable = riskModel.ActionPlanAvailable;
        riskDetail.IsThisRiskBeingAccepted = riskModel.IsThisRiskBeingAccepted;
        riskDetail.LossesAssociatedWithThisRisk = riskModel.LossesAssociatedWithThisRisk;
        riskDetail.Losses = riskModel.Losses;
        riskDetail.ActionPlan = riskModel.ActionPlan;
        DateTime targetDate;
        if (DateTime.TryParse(riskModel.TargetDate + "", out  targetDate))
        {

          riskDetail.TargetDate = riskModel.TargetDate.ToString();
        }
        else
        {
          riskDetail.TargetDate = DateTime.Now.ToString();
        }

        riskDetail.ActionPlan_Status = riskModel.ActionPlan_Status;
        riskDetail.Reason_Risk_acceptance = riskModel.Reason_Risk_acceptance;
        riskDetail.List_risk_associated = riskModel.List_risk_associated;
        riskDetail.Owner = riskModel.Owner;
        riskDetail.Shared_process_Department = riskModel.Shared_process_Department;
        riskDetail.Shared_process_Description = riskModel.Shared_process_Description;
        if (riskDetail.RiskDetailID == 0)
        {
          riskDetail.insertedmachineinfo = ip;
          riskDetail.insertedBy = WebSecurity.CurrentUserId;
          riskDetail.Insertedon = DateTime.Now;
          if (riskDetail.updatedon == DateTime.MinValue)
          {
            riskDetail.updatedon = DateTime.Now;
          }
        }

        else
        {
          riskDetail.Updatedby = riskModel.Updatedby;
          riskDetail.updatedon = DateTime.Now;
          riskDetail.updatemachineinfo = ip;
        }
        riskDetail.CompanyObjectives = riskModel.CompanyObjectives;


        riskDetail.Status = "P";
        if (command != "Save")
        {
          riskDetail.Status = "C";
          riskDetail.CompletionDate = DateTime.Now;
        }
        if (riskDetail.RiskDetailID <= 0)
        {
          db.RiskDetails.Add(riskDetail);
          ViewBag.Mode = "Save";
        }
        else
        {
          UpdateModel(riskDetail);
          ViewBag.Mode = "Save";
        }
        db.SaveChanges();
      }
      else
      {
        riskModel.Departments = db.DepartmentsMaster.ToList();
        riskModel.Risks = db.RiskMasters.ToList();
        if (riskModel.RiskID > 0)
          riskModel.SubRisks = db.SubRisksMaster.Where(x => x.RiskId == riskModel.RiskID).ToList();
        else
          riskModel.SubRisks = new List<SubRiskMaster>();

        if (riskModel.SubRiskID > 0)
          riskModel.Mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == riskModel.SubRiskID).ToList();
        else
          riskModel.Mitigants = new List<MitigantMaster>();

        riskModel.Shared_process_Departments = db.DepartmentsMaster.ToList();

        return View(riskModel);
      }
      return RedirectToAction("RiskDetails", "Orm", new { id = rid, compId = cid, deptId = did });
    }

    [HttpPost]
    public string GetRiskAssessmentList(string deptId)
    {
      string res = "1";
      int departmentId = 0;
      if (deptId != null)
      {
        departmentId = Convert.ToInt32(deptId);
      }
      var company = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).SingleOrDefault();
      var RiskAssesmentAtTime = db.RiskDetails.Where(x => x.CompanyID == company.CompanyId && x.DepartmentID == departmentId && x.Status == "P").ToList();
      if (RiskAssesmentAtTime != null && RiskAssesmentAtTime.Count > 0)
      {
        res = "0";
      }
      return res;
    }

    [HttpPost]
    public string GetAllRiskAssessmentList(string compId, string deptId)
    {
      string res = "1";
      int companyId = 0;
      int departmentId = 0;
      if (compId != null)
      {
        companyId = Convert.ToInt32(compId);
      }
      if (deptId != null)
      {
        departmentId = Convert.ToInt32(deptId);
      }
      var RiskAssesmentAtTime = db.RiskDetails.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "P").ToList();
      if (RiskAssesmentAtTime != null && RiskAssesmentAtTime.Count > 0)
      {
        res = "0";
      }
      return res;
    }

    [HttpPost]
    public string GetAllCompletedList(string compId, string deptId)
    {
      string res = "1";
      int companyId = 0;
      int departmentId = 0;
      if (compId != null)
      {
        companyId = Convert.ToInt32(compId);
      }
      if (deptId != null)
      {
        departmentId = Convert.ToInt32(deptId);
      }
      var RiskAssesmentAtTime = db.RiskAssessmentMaster.Where(x => x.CompanyID == companyId && x.DepartmentID == departmentId && x.Status == "P").ToList();
      if (RiskAssesmentAtTime != null && RiskAssesmentAtTime.Count > 0)
      {
        res = "0";
      }
      return res;
    }


    public ActionResult UpdateRisk()
    {
      var company = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId);
      if (company.Any())
      {
        var listDepartmentRisks = db.RiskDetails
                  .Where(x => x.CompanyID == company.FirstOrDefault().CompanyId && x.Status == "P").ToList();
        return View(listDepartmentRisks);
      }

      return View(new List<RiskDetailsModel>());
    }

    public ActionResult DepartmentRisks(int? cId, int? dId)
    {
      ViewBag.compId = cId;
      ViewBag.deptId = dId;
      var riskAssesmentList = new List<RiskAssessmentMaster>();
      if (User.IsInRole("Consultant"))
      {
        if (cId != null && dId != null)
        {
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
        }
        else
        {
          riskAssesmentList = db.RiskAssessmentMaster.ToList();
        }
      }

      if (User.IsInRole("Admin"))
      {
        if (cId != null && dId != null)
        {
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
        }
        else
        {
          riskAssesmentList = db.RiskAssessmentMaster.ToList();
        }
      }
      if (User.IsInRole("Company"))
      {
        if (cId != null && dId != null)
        {
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId).ToList();
        }
        else
        {
          riskAssesmentList = db.RiskAssessmentMaster.ToList();
        }
      }
      return View(riskAssesmentList.OrderByDescending(x => x.Status));
    }

    [HttpPost]
    public ActionResult DepartmentRisks(int? ids)
    {

      int deptId = Request["hfDeptId"] != "" ? Convert.ToInt32(Request["hfDeptId"]) : 0;
      int compId = Request["hfCompId"] != "" ? Convert.ToInt32(Request["hfCompId"]) : 0;
      string riskAssesment = Request["txtRiskAssesmentName"] != "" ? Request["txtRiskAssesmentName"] : "";
      var riskAssesmentList = new List<RiskAssessmentMaster>();
      ViewBag.CompId = compId;
      ViewBag.DeptId = deptId;
      if (riskAssesment != null && riskAssesment != "")
      {
        var list = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentName == riskAssesment && x.CompanyID == compId).ToList();
        if (list.Count > 0)
        {
          ViewBag.Error = "Risk Assessment name already exist!";
          // return View();

        }
        else
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
      }

      //var listDepartmentRisks = new List<RiskDetailsModel>();
      if (User.IsInRole("Consultant"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}                
      }
      if (User.IsInRole("Admin"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}
      }
      if (User.IsInRole("Company"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}
      }

      return View(riskAssesmentList.OrderByDescending(x => x.Status));
    }


    public ActionResult RiskManagementDetail(int id, int? rid, int? compId, int? deptId, string view)
    {
      ViewBag.compId = compId;
      ViewBag.deptId = deptId;
      ViewBag.RiskAssesmentId = rid;
      ViewBag.ViewType = view;
      var model = new RiskManagementModel();
      var companyUser = db.CompanyUsers.FirstOrDefault(x => x.UserId == WebSecurity.CurrentUserId);
      if (companyUser == null)
      {
        if (User.IsInRole("Consultant"))
        {
          int userIds = 0;
          if (Request["hfInsertedBy"] != null)
          {
            userIds = Convert.ToInt32(Request["hfInsertedBy"]);
          }
          companyUser = db.CompanyUsers.FirstOrDefault(x => x.UserId == userIds);
        }
        else
        {
          return View();
        }
      }

      var riskDetail = db.RiskDetails.FirstOrDefault(x => x.RiskDetailID == id && x.CompanyID == companyUser.CompanyId);
      if (riskDetail != null && riskDetail.RiskDetailID > 0)
      {
        model.RiskDetailID = riskDetail.RiskDetailID;
        model.CompanyId = riskDetail.CompanyID ?? 0;
        model.DepartmentID = riskDetail.DepartmentID ?? 0;
        model.DepartmentName = riskDetail.DepartmentMaster.DepartmentName;
        model.RiskID = riskDetail.RiskID ?? 0;

        var objRiskMaster = riskDetail.RiskMaster;
        if (objRiskMaster != null)
        {
          model.RiskName = objRiskMaster.RiskName;
          model.RiskDescription = objRiskMaster.Description;
        }
        //model.RiskName = riskDetail.RiskMaster.RiskName;
        //model.RiskDescription = riskDetail.RiskMaster.Description;
        model.SubRiskID = riskDetail.SubRiskID ?? 0;

        var objSubRiskMaster = riskDetail.SubRiskMaster;
        if (objSubRiskMaster != null)
        {
          model.SubRiskName = objSubRiskMaster.SubRiskName;
          model.SubRiskDescription = objSubRiskMaster.SubRiskDesc;
        }
        //model.SubRiskName = riskDetail.SubRiskMaster.SubRiskName;
        //model.SubRiskDescription = riskDetail.SubRiskMaster.SubRiskDesc;
        model.SubRisk_Impact = riskDetail.SubRisk_Impact;
        model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
        model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
        model.MitigantID = riskDetail.MitigantID ?? 0;

        var objMitigantMaster = riskDetail.MitigantMaster;
        if (objMitigantMaster != null)
        {
          model.MitigantName = objMitigantMaster.MitigantName;
          model.MitigantDescription = objMitigantMaster.MitigantDesc;
          objMitigantMaster = null;
        }
        //model.MitigantName = riskDetail.MitigantMaster.MitigantName;
        //model.MitigantDescription = riskDetail.MitigantMaster.MitigantDesc;
        model.Mitigant_Importance = riskDetail.Mitigant_Importance;
        model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
        model.Mitigant_whyEffective = riskDetail.Mitigant_whyEffective;
        model.Issue = riskDetail.Issue;
        model.Issue_Severity = riskDetail.Issue_Severity;
        model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
        model.IsThisRiskBeingAccepted = riskDetail.IsThisRiskBeingAccepted;
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
        model.Reason_Risk_acceptance = riskDetail.Reason_Risk_acceptance;
        model.List_risk_associated = riskDetail.List_risk_associated;
        model.LossesAssociatedWithThisRisk = riskDetail.LossesAssociatedWithThisRisk;
        model.Losses = riskDetail.Losses;
        model.Owner = riskDetail.Owner;
        model.Shared_process_Department = riskDetail.Shared_process_Department;
        model.Shared_process_Description = riskDetail.Shared_process_Description;
        model.insertedBy = riskDetail.insertedBy ?? 0;
        model.Insertedon = DateTime.Now;
        model.insertedmachineinfo = riskDetail.insertedmachineinfo;
        model.Updatedby = riskDetail.Updatedby ?? 0;
        model.updatedon = DateTime.Now;
        model.updatemachineinfo = riskDetail.updatemachineinfo;
        model.CompanyObjectives = riskDetail.CompanyObjectives;
      }
      else
      {
        model.CompanyId = db.CompanyUsers.FirstOrDefault(x => x.CompanyUserId == WebSecurity.CurrentUserId).CompanyId;
      }

      model.Departments = db.DepartmentsMaster.ToList();
      model.Risks = new List<RiskMaster>();
      model.SubRisks = new List<SubRiskMaster>();
      model.Mitigants = new List<MitigantMaster>();
      model.Shared_process_Departments = db.DepartmentsMaster.ToList();
     // model.TargetDate = DateTime.Now.ToString();

      return View(model);
    }

    public ActionResult RiskDetails(int? id, int? compId, int? deptId, string view, int? editId)
    {
      ViewBag.compId = compId;
      ViewBag.deptId = deptId;
      ViewBag.RiskAssesmentId = id;
      ViewBag.ViewType = view;
      string value = Request.QueryString["editId"];
      ViewBag.newValue = value;
      var riskDetail = db.RiskDetails.Where(x => x.RiskAssessmentID == id).ToList();
      var newriskDetail = db.RiskDetails.OrderByDescending(x => x.updatedon).FirstOrDefault();
      //ViewBag.lastId = newriskDetail.RiskDetailID;
      var companyName = db.CompaniesMaster.Where(x => x.CompanyId == compId).FirstOrDefault();
      var departmentName = db.DepartmentsMaster.Where(x => x.DepartmentId == deptId).FirstOrDefault();
      if (companyName != null)
      {
        @ViewBag.companyName = companyName.ComapnyName;
      }
      if (departmentName != null)
      {
        @ViewBag.departmentName = departmentName.DepartmentName;
      }
      return View(riskDetail);
    }

    [HttpPost]
    public ActionResult RiskDetails()
    {
      string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      if (String.IsNullOrWhiteSpace(ip))
      {
        ip = Request.ServerVariables["REMOTE_ADDR"];
      }

      int riskAssesmentId = Request["hfRiskAssesmentId"] != "" ? Convert.ToInt32(Request["hfRiskAssesmentId"]) : 0;
      int compId = Request["hfCompId"] != "" ? Convert.ToInt32(Request["hfCompId"]) : 0;
      int deptId = Request["hfDeptId"] != "" ? Convert.ToInt32(Request["hfDeptId"]) : 0;
      ViewBag.compId = compId;
      ViewBag.deptId = deptId;
      var riskDetails = db.RiskDetails.Where(x => x.RiskAssessmentID == riskAssesmentId).ToList();
      var assesmentDetails = db.RiskAssessmentMaster.Where(x => x.RiskAssessmentId == riskAssesmentId).SingleOrDefault();
      foreach (var item in riskDetails)
      {
        item.Status = "C";
        item.Updatedby = WebSecurity.CurrentUserId;
        item.updatedon = DateTime.Now;
        item.updatemachineinfo = ip;
        UpdateModel(item);
        db.SaveChanges();
      }
      assesmentDetails.Updatedby = WebSecurity.CurrentUserId;
      assesmentDetails.Updatedon = DateTime.Now;
      assesmentDetails.Updatemachineinfo = ip;
      assesmentDetails.Status = "C";
      UpdateModel(assesmentDetails);
      db.SaveChanges();
      riskDetails = riskDetails != null ? riskDetails : new List<RiskDetailsModel>();
      return View(riskDetails);
    }

    public ActionResult RiskAssesmentsManagement(int? id, int? rid, int? compId, int? deptId)
    {
      var model = new RiskManagementModel();
      ViewBag.compId = compId;
      ViewBag.deptId = deptId;
      ViewBag.RiskAssesmentId = rid;

      var companyName = db.CompaniesMaster.Where(x => x.CompanyId == compId).FirstOrDefault();
      var departmentName = db.DepartmentsMaster.Where(x => x.DepartmentId == deptId).FirstOrDefault();
      if (companyName != null)
      {
        @ViewBag.companyName = companyName.ComapnyName;
      }
      if (departmentName != null)
      {
        @ViewBag.departmentName = departmentName.DepartmentName;
      }



      RiskDetailsModel riskDetail = db.RiskDetails.FirstOrDefault(x => x.RiskDetailID == id);

      if (riskDetail != null && riskDetail.RiskAssessmentID > 0)
      {
        ViewBag.Mode = "Edit";

        model.RiskDetailID = riskDetail.RiskDetailID;
        model.CompanyId = riskDetail.CompanyID ?? compId;
        model.DepartmentID = riskDetail.DepartmentID ?? deptId;
        model.RiskAssessmentId = riskDetail.RiskAssessmentID ?? rid;
        model.RiskID = riskDetail.RiskID ?? 0;

        RiskMaster objRiskMaster = riskDetail.RiskMaster;
        if (objRiskMaster != null)
        {
          model.RiskName = objRiskMaster.RiskName;
          model.RiskDescription = objRiskMaster.Description;
          objRiskMaster = null;
        }
        model.SubRiskID = riskDetail.SubRiskID ?? 0;
        SubRiskMaster objSubRiskMaster = riskDetail.SubRiskMaster;
        if (objSubRiskMaster != null)
        {
          model.SubRiskName = objSubRiskMaster.SubRiskName;
          model.SubRiskDescription = objSubRiskMaster.SubRiskDesc;
          objSubRiskMaster = null;
        }
        model.SubRisk_Impact = riskDetail.SubRisk_Impact;
        model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
        model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
        model.MitigantID = riskDetail.MitigantID ?? 0;
        MitigantMaster objMitigantMaster = riskDetail.MitigantMaster;
        if (objMitigantMaster != null)
        {
          model.MitigantName = objMitigantMaster.MitigantName;
          model.MitigantDescription = objMitigantMaster.MitigantDesc;
          objMitigantMaster = null;
        }
        model.Mitigant_Importance = riskDetail.Mitigant_Importance;
        model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
        model.Mitigant_whyEffective = riskDetail.Mitigant_whyEffective;
        model.Issue = riskDetail.Issue;
        model.Issue_Severity = riskDetail.Issue_Severity;
        model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
        model.IsThisRiskBeingAccepted = riskDetail.IsThisRiskBeingAccepted;
        model.LossesAssociatedWithThisRisk = riskDetail.LossesAssociatedWithThisRisk;
        model.Losses = riskDetail.Losses;
        model.ActionPlan = riskDetail.ActionPlan;
        model.TargetDate = riskDetail.TargetDate == null ? null : riskDetail.TargetDate.ToString(); ;
        model.ActionPlan_Status = riskDetail.ActionPlan_Status;
        //   model.Reason_Risk_acceptance = riskDetail.Reason_Risk_acceptance;
        model.List_risk_associated = riskDetail.List_risk_associated;
        model.Owner = riskDetail.Owner;
        model.Shared_process_Department = riskDetail.Shared_process_Department;
        model.Shared_process_Description = riskDetail.Shared_process_Description;
        model.insertedBy = riskDetail.insertedBy ?? 0;
        model.Insertedon = DateTime.Now;
        model.insertedmachineinfo = riskDetail.insertedmachineinfo;
        model.Updatedby = riskDetail.Updatedby ?? 0;
        model.updatedon = DateTime.Now;
        model.updatemachineinfo = riskDetail.updatemachineinfo;
        model.CompanyObjectives = riskDetail.CompanyObjectives;
        ViewBag.SharedProcessDepartment = riskDetail.Shared_process_Department;
      }
      else
      {
        var com = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId);
        if (com.Any())
        {
          model.CompanyId = com.First().CompanyId;
          model.TargetDate = DateTime.Now.ToString();
        }
        model.TargetDate = null;
      }

      model.Departments = (from x in db.UserDepartments
                           join u in db.DepartmentsMaster on x.DepartmentId equals u.DepartmentId
                           where x.UserId == WebSecurity.CurrentUserId
                           select u).ToList();


      if (model.RiskID > 0)
      {
        model.Risks = db.RiskMasters.ToList();
        model.SubRisks = db.SubRisksMaster.Where(x => x.RiskId == model.RiskID).ToList();
      }
      else if (TempData["riskId"] != null)
      {
        var riskId = Convert.ToInt32(TempData["riskId"]);
        var subriskId = Convert.ToInt32(TempData["subRiskId"]);
        model.Risks = db.RiskMasters.ToList();
        model.SubRisks = db.SubRisksMaster.Where(x => x.RiskId == riskId).ToList();
        model.RiskID = riskId;
        model.SubRiskID = subriskId;
        model.SubRiskDescription = db.SubRisksMaster.Where(x => x.SubRiskId == subriskId).Select(x => x.SubRiskDesc).FirstOrDefault();
        model.RiskDescription = db.RiskMasters.Where(x => x.RiskId == riskId).Select(x => x.Description).FirstOrDefault();
        // ViewBag.Mode = "New";
        ViewBag.newRiskId = riskId;
      }
      else
      {
        model.Risks = new List<RiskMaster>();
        model.SubRisks = new List<SubRiskMaster>();
      }

      //if (model.SubRiskID > 0 && TempData["mitigantId"] == null)
      //{
      //  model.Mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == model.SubRiskID).ToList();
      //}
      if (model.SubRiskID > 0)
      {
        model.Mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == model.SubRiskID).ToList();
      }

      //else if (TempData["mitigantId"] != null)
      //{
      //  var subriskId = Convert.ToInt32(TempData["subRiskId"]);
      //  var mitigantId = Convert.ToInt32(TempData["mitigantId"]);
      //  model.Mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == subriskId).ToList();
      //  model.MitigantID = mitigantId;
      //  model.MitigantDescription = db.MitigantsMaster.Where(x => x.MitigantId == mitigantId).Select(x => x.MitigantDesc).FirstOrDefault();
      //}
      else
      { model.Mitigants = new List<MitigantMaster>(); }

      model.Shared_process_Departments = model.Departments;
      return View(model);
    }

    public void RiskAssesmentsDelete(int? Id)
    {
      int cId = Convert.ToInt32(Id);
      var RiskAssesmentsList = db.RiskDetails.Where(x => x.RiskAssessmentID == cId).ToList();
      foreach (var item in RiskAssesmentsList)
      {
        db.RiskDetails.Remove(item);
        db.SaveChanges();
      }
    }

    [HttpPost]
    public ActionResult RiskAssesmentsManagement(RiskManagementModel riskModel, string command)
    {
      int riskdetailsId = 0;
      string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      if (String.IsNullOrWhiteSpace(ip))
      {
        ip = Request.ServerVariables["REMOTE_ADDR"];
      }

      int? cid = Request["hfCompId"] != "" ? Convert.ToInt32(Request["hfCompId"]) : 0;
      int? did = Request["hfDeptId"] != "" ? Convert.ToInt32(Request["hfDeptId"]) : 0;
      int? rid = Request["hfAssesmentId"] != "" ? Convert.ToInt32(Request["hfAssesmentId"]) : 0;
      int riskId = Request["hfRiskId"] != "" ? Convert.ToInt32(Request["hfRiskId"]) : 0;
      int subRiskId = Request["hfSubRiskId"] != "" ? Convert.ToInt32(Request["hfSubRiskId"]) : 0;
      int mitigantId = Request["hfMitigantId"] != "" ? Convert.ToInt32(Request["hfMitigantId"]) : 0;
      int IsExists = Request["hfIsExists"] != "" ? Convert.ToInt32(Request["hfIsExists"]) : 0;
      int RiskDetailsId = Request["RiskDetailsId"] != "" ? Convert.ToInt32(Request["RiskDetailsId"]) : 0;
      int Isoverwrite = Request["hfOverWrite"] != "" ? Convert.ToInt32(Request["hfOverWrite"]) : 0;
      int selectedButton = Convert.ToInt32(Request["hfSelectedButton"]);
      ViewBag.compId = cid;
      ViewBag.deptId = did;
      ViewBag.RiskAssesmentId = rid;
      ViewBag.MitigantID = mitigantId;
      ViewBag.SubRiskID = subRiskId;
      ViewBag.RiskId = riskId;



      if (ModelState.IsValid)
      {

        var riskExists = db.RiskDetails.Where(x => x.RiskAssessmentID == rid && x.RiskID == riskId && x.SubRiskID == subRiskId && x.MitigantID == mitigantId && x.Status == "P").ToList();
        if (riskExists.Count > 0 && riskExists != null && IsExists == 0)
        {
          ViewBag.RiskDetailsId = riskExists.FirstOrDefault().RiskDetailID;
          riskModel.Departments = db.DepartmentsMaster.ToList();
          riskModel.Risks = db.RiskMasters.ToList();
          if (riskModel.RiskID > 0)
            riskModel.SubRisks = db.SubRisksMaster.Where(x => x.RiskId == riskModel.RiskID).ToList();
          else
            riskModel.SubRisks = new List<SubRiskMaster>();

          if (riskModel.SubRiskID > 0)
            riskModel.Mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == riskModel.SubRiskID).ToList();
          else
            riskModel.Mitigants = new List<MitigantMaster>();

          riskModel.Shared_process_Departments = db.DepartmentsMaster.ToList();
          ViewBag.RiskDetailsId = riskExists.FirstOrDefault().RiskDetailID;
          ViewBag.RiskExists = "Risk Already Exists";
          return View(riskModel);
        }

        riskdetailsId = riskModel.RiskDetailID != 0 ? riskModel.RiskDetailID : RiskDetailsId;

        RiskDetailsModel riskDetail = db.RiskDetails.FirstOrDefault(x => x.RiskDetailID == riskdetailsId) ??
                                      new RiskDetailsModel();
        riskModel.RiskDetailID = riskModel.RiskDetailID != 0 ? riskModel.RiskDetailID : RiskDetailsId;
        if (Isoverwrite == 0)
        {
          riskDetail.RiskDetailID = riskModel.RiskDetailID != 0 ? riskModel.RiskDetailID : RiskDetailsId;
        }
        riskDetail.RiskAssessmentID = rid;
        riskDetail.CompanyID = riskModel.CompanyId;
        riskDetail.DepartmentID = riskModel.DepartmentID;
        riskDetail.RiskID = riskModel.RiskID != 0 ? riskModel.RiskID : riskId;
        //riskDetail.RiskID = riskModel.RiskID;
        riskDetail.SubRiskID = riskModel.SubRiskID != 0 ? riskModel.SubRiskID : subRiskId;
        //riskDetail.SubRiskID = riskModel.SubRiskID;
        riskDetail.SubRisk_Impact = riskModel.SubRisk_Impact;
        riskDetail.SubRisk_Likelyhood = riskModel.SubRisk_Likelyhood;
        riskDetail.Inherenet_risk_rating = riskModel.Inherenet_risk_rating;
        riskDetail.MitigantID = riskModel.MitigantID != 0 ? riskModel.MitigantID : mitigantId;
        //riskDetail.MitigantID = riskModel.MitigantID;
        riskDetail.Mitigant_Importance = riskModel.Mitigant_Importance;
        riskDetail.Mitigant_effectiveness = riskModel.Mitigant_effectiveness;
        riskDetail.Mitigant_whyEffective = riskModel.Mitigant_whyEffective;
        riskDetail.Issue = riskModel.Issue;
        riskDetail.Issue_Severity = riskModel.Issue_Severity;
        riskDetail.ActionPlanAvailable = riskModel.ActionPlanAvailable;
        riskDetail.IsThisRiskBeingAccepted = riskModel.IsThisRiskBeingAccepted;
        riskDetail.LossesAssociatedWithThisRisk = riskModel.LossesAssociatedWithThisRisk;
        riskDetail.Losses = riskModel.Losses;
        riskDetail.ActionPlan = riskModel.ActionPlan;
        DateTime targetDate;
        if (DateTime.TryParse(riskModel.TargetDate + "", out  targetDate))
        {

          riskDetail.TargetDate = riskModel.TargetDate.ToString();
        }
        else
        {
          riskDetail.TargetDate = null;
        }

        riskDetail.ActionPlan_Status = riskModel.ActionPlan_Status;
        //   riskDetail.Reason_Risk_acceptance = riskModel.Reason_Risk_acceptance;
        riskDetail.List_risk_associated = riskModel.List_risk_associated;
        riskDetail.Owner = riskModel.Owner;
        riskDetail.Shared_process_Department = riskModel.Shared_process_Department;
        riskDetail.Shared_process_Description = riskModel.Shared_process_Description;
        if (riskdetailsId == 0)
        {
          riskDetail.insertedmachineinfo = ip;
          riskDetail.insertedBy = WebSecurity.CurrentUserId;
          riskDetail.Insertedon = DateTime.Now;
          if (riskDetail.updatedon == DateTime.MinValue)
          {
            riskDetail.updatedon = DateTime.Now;
          }
        }

        else
        {
          riskDetail.Updatedby = riskModel.Updatedby;
          riskDetail.updatedon = DateTime.Now;
          riskDetail.updatemachineinfo = ip;
        }
        riskDetail.CompanyObjectives = riskModel.CompanyObjectives;
        riskDetail.Status = "P";

        //if (command != "Save")
        //{
        //  riskDetail.Status = "C";
        //  riskDetail.CompletionDate = DateTime.Now;
        //}
        if (riskdetailsId <= 0)
        {
          db.RiskDetails.Add(riskDetail);
          ViewBag.Mode = "Save";
        }
        else
        {
          if (Isoverwrite == 0)
          {
            UpdateModel(riskDetail);
            ViewBag.Mode = "Save";
          }
        }
        db.SaveChanges();
      }
      else
      {
        riskModel.Departments = db.DepartmentsMaster.ToList();
        riskModel.Risks = db.RiskMasters.ToList();
        if (riskModel.RiskID > 0)
          riskModel.SubRisks = db.SubRisksMaster.Where(x => x.RiskId == riskModel.RiskID).ToList();
        else
          riskModel.SubRisks = new List<SubRiskMaster>();

        if (riskModel.SubRiskID > 0)
          riskModel.Mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == riskModel.SubRiskID).ToList();
        else
          riskModel.Mitigants = new List<MitigantMaster>();

        riskModel.Shared_process_Departments = db.DepartmentsMaster.ToList();

        return View(riskModel);
      }
      if (Request.Form["exit"] != null)
      {
        riskModel.RiskID = ViewBag.RiskId;
        riskModel.Risks = db.RiskMasters.ToList();
        if (riskModel.RiskID > 0)
          riskModel.SubRisks = db.SubRisksMaster.Where(x => x.RiskId == riskModel.RiskID).ToList();
        else
          riskModel.SubRisks = new List<SubRiskMaster>();

        if (riskModel.SubRiskID > 0)
          riskModel.Mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == riskModel.SubRiskID).ToList();
        else
          riskModel.Mitigants = new List<MitigantMaster>();
        // return View(riskModel);
        return RedirectToAction("DepartmentRisks", "Orm");//, new { cId = cid, dId = did}
      }

      TempData["riskId"] = riskId;
      TempData["subRiskId"] = subRiskId;
      TempData["mitigantId"] = mitigantId;


      return RedirectToAction("RiskDetails", "Orm", new { id = rid, compId = cid, deptId = did, editId = riskdetailsId });
    }

    public ActionResult RiskAssesmentsManagementDetail(int id, int? rid, int? compId, int? deptId, string view)
    {
      ViewBag.compId = compId;
      ViewBag.deptId = deptId;
      ViewBag.RiskAssesmentId = rid;
      ViewBag.ViewType = view;

      var companyName = db.CompaniesMaster.Where(x => x.CompanyId == compId).FirstOrDefault();
      var departmentName = db.DepartmentsMaster.Where(x => x.DepartmentId == deptId).FirstOrDefault();
      if (companyName != null)
      {
        @ViewBag.companyName = companyName.ComapnyName;
      }
      if (departmentName != null)
      {
        @ViewBag.departmentName = departmentName.DepartmentName;
      }


      var model = new RiskManagementModel();
      var riskDetail = db.RiskDetails.FirstOrDefault(x => x.RiskDetailID == id);
      if (riskDetail != null && riskDetail.RiskDetailID > 0)
      {
        model.RiskDetailID = riskDetail.RiskDetailID;
        model.CompanyId = riskDetail.CompanyID ?? compId;
        model.DepartmentID = riskDetail.DepartmentID ?? deptId;
        model.RiskAssessmentId = riskDetail.RiskAssessmentID ?? rid;
        model.DepartmentName = riskDetail.DepartmentMaster.DepartmentName;
        model.RiskID = riskDetail.RiskID ?? 0;

        var objRiskMaster = riskDetail.RiskMaster;
        if (objRiskMaster != null)
        {
          model.RiskName = objRiskMaster.RiskName;
          model.RiskDescription = objRiskMaster.Description;
        }
        model.SubRiskID = riskDetail.SubRiskID ?? 0;

        var objSubRiskMaster = riskDetail.SubRiskMaster;
        if (objSubRiskMaster != null)
        {
          model.SubRiskName = objSubRiskMaster.SubRiskName;
          model.SubRiskDescription = objSubRiskMaster.SubRiskDesc;
        }
        model.SubRisk_Impact = riskDetail.SubRisk_Impact;
        model.SubRisk_Likelyhood = riskDetail.SubRisk_Likelyhood;
        model.Inherenet_risk_rating = riskDetail.Inherenet_risk_rating;
        model.MitigantID = riskDetail.MitigantID ?? 0;

        var objMitigantMaster = riskDetail.MitigantMaster;
        if (objMitigantMaster != null)
        {
          model.MitigantName = objMitigantMaster.MitigantName;
          model.MitigantDescription = objMitigantMaster.MitigantDesc;
          objMitigantMaster = null;
        }
        model.Mitigant_Importance = riskDetail.Mitigant_Importance;
        model.Mitigant_effectiveness = riskDetail.Mitigant_effectiveness;
        model.Mitigant_whyEffective = riskDetail.Mitigant_whyEffective;
        model.Issue = riskDetail.Issue;
        model.Issue_Severity = riskDetail.Issue_Severity;
        model.ActionPlanAvailable = riskDetail.ActionPlanAvailable;
        model.IsThisRiskBeingAccepted = riskDetail.IsThisRiskBeingAccepted;
        model.LossesAssociatedWithThisRisk = riskDetail.LossesAssociatedWithThisRisk;
        model.Losses = riskDetail.Losses;
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
        model.Reason_Risk_acceptance = riskDetail.Reason_Risk_acceptance;
        model.List_risk_associated = riskDetail.List_risk_associated;
        model.Owner = riskDetail.Owner;
        model.Shared_process_Department = riskDetail.Shared_process_Department;
        model.Shared_process_Description = riskDetail.Shared_process_Description;
        model.insertedBy = riskDetail.insertedBy ?? 0;
        model.Insertedon = DateTime.Now;
        model.insertedmachineinfo = riskDetail.insertedmachineinfo;
        model.Updatedby = riskDetail.Updatedby ?? 0;
        model.updatedon = DateTime.Now;
        model.updatemachineinfo = riskDetail.updatemachineinfo;
        model.CompanyObjectives = riskDetail.CompanyObjectives;

      }

      model.Departments = db.DepartmentsMaster.ToList();
      model.Risks = new List<RiskMaster>();
      model.SubRisks = new List<SubRiskMaster>();
      model.Mitigants = new List<MitigantMaster>();
      model.Shared_process_Departments = db.DepartmentsMaster.ToList();
      // model.TargetDate = riskDetail.TargetDate.ToString();

      return View(model);
    }

    public ActionResult StartNewRiskAssesment()
    {
      return View();
    }

    public ActionResult ViewRisks(int? cId, int? dId)
    {
      ViewBag.compId = cId;
      ViewBag.deptId = dId;
      var riskAssesmentList = new List<RiskAssessmentMaster>();
      if (User.IsInRole("Consultant"))
      {
        if (cId != null && dId != null)
        {
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId && x.Status == "C").ToList();
        }
        else
        {
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.Status == "C").ToList();
        }
      }

      if (User.IsInRole("Admin"))
      {
        if (cId != null && dId != null)
        {
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId && x.Status == "C").ToList();
        }
        else
        {
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId && x.Status == "C").ToList();
        }
      }
      if (User.IsInRole("Company"))
      {
        if (cId != null && dId != null)
        {
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == cId && x.DepartmentID == dId && x.Status == "C").ToList();
        }
        else
        {
          riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.Status == "C").ToList();
        }
      }
      return View(riskAssesmentList);
    }

    [HttpPost]
    public ActionResult ViewRisks(int? ids)
    {

      int deptId = Request["hfDeptId"] != "" ? Convert.ToInt32(Request["hfDeptId"]) : 0;
      int compId = Request["hfCompId"] != "" ? Convert.ToInt32(Request["hfCompId"]) : 0;
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
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId && x.DepartmentID == deptId && x.Status == "C").ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}                
      }
      if (User.IsInRole("Admin"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId && x.DepartmentID == deptId && x.Status == "C").ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}
      }
      if (User.IsInRole("Company"))
      {
        riskAssesmentList = db.RiskAssessmentMaster.Where(x => x.CompanyID == compId && x.DepartmentID == deptId && x.Status == "C").ToList();
        //if (deptId != 0)
        //{
        //  listDepartmentRisks = db.RiskDetails.Where(x => x.CompanyID == compId && x.DepartmentID == deptId).ToList();
        //}
      }

      return View(riskAssesmentList);
    }

  }
}
