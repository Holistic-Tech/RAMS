using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rcsa.Web.Models;
using WebMatrix.WebData;

namespace Rcsa.Web.Controllers
{
  [Authorize]
  public class SubRiskController : Controller
  {
    //
    // GET: /SubRisk/

    RcsaDb db = new RcsaDb();

    public ActionResult Index()
    {
      return View();
    }


    #region JSON Response Functions

    [HttpGet]
    public JsonResult SubRisks(int riskId)
    {
      List<SubRiskMaster> subrisks = db.SubRisksMaster.Where(x => x.RiskId == riskId).ToList();
      return Json(subrisks, JsonRequestBehavior.AllowGet);
    }

    #endregion


    public ActionResult SubRiskDetails()
    {
      var list = db.SubRisksMaster.ToList();
      return View(list);
    }

    [HttpPost]
    public ActionResult SubRiskDetails(string keyword, string command)
    {
      List<SubRiskMaster> list = new List<SubRiskMaster>();
      if (keyword != "" && keyword != null)
      {
        list = db.SubRisksMaster.ToList();
        list = list.Where(x => x.SubRiskName.Contains(keyword)).ToList();// || x.RisksMaster.RiskName.Contains(keyword) || x.SubRiskDesc.Contains(keyword)).ToList();
      }
      if (command == "Reset")
      {
        list = db.SubRisksMaster.ToList();
      }
      ViewBag.Mode = "Search";
      return View(list.OrderBy(x => x.SubRiskName));
    }

    //
    // GET: /SubRisk/Details/5

    public ActionResult Details(int id)
    {
      SubRiskMaster list = db.SubRisksMaster.FirstOrDefault(x => x.SubRiskId == id);
      return View(list);
    }

    //
    // GET: /SubRisk/Create

    public ActionResult Create()
    {
      return View();
    }

    //
    // POST: /SubRisk/Create

    [HttpPost]
    public ActionResult Create(SubRiskMaster subrisk)
    {
      string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      if (String.IsNullOrWhiteSpace(ip))
      {
        ip = Request.ServerVariables["REMOTE_ADDR"];
      }
      try
      {
        bool list = db.SubRisksMaster.Any(x => x.SubRiskName == subrisk.SubRiskName && x.RiskId == subrisk.RiskId);
        if (list)
        {
          ViewBag.Error = "Sub risk name already exist!";
          return View();
        }
        subrisk.InsertedBy = WebSecurity.CurrentUserId;
        subrisk.Insertedon = DateTime.Now;
        subrisk.Insertedmachineinfo = ip;

        db.SubRisksMaster.Add(subrisk);
        db.SaveChanges();
        return RedirectToAction("SubRiskDetails");
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /SubRisk/Edit/5

    public ActionResult Edit(int id)
    {
      SubRiskMaster list = db.SubRisksMaster.FirstOrDefault(x => x.SubRiskId == id);
      return View(list);
    }

    //
    // POST: /SubRisk/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection, SubRiskMaster subrisk)
    {
      string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      if (String.IsNullOrWhiteSpace(ip))
      {
        ip = Request.ServerVariables["REMOTE_ADDR"];
      }
      try
      {
        SubRiskMaster list = db.SubRisksMaster.FirstOrDefault(x => x.SubRiskId == id);
        if (ModelState.IsValid)
        {

          bool exists = db.SubRisksMaster.Any(x => x.SubRiskName == subrisk.SubRiskName && x.RiskId == subrisk.RiskId && x.SubRiskId != subrisk.SubRiskId);
          if (exists)
          {
            ViewBag.Error = "Sub risk already exist!";
            return View();
          }
          list.SubRiskName = subrisk.SubRiskName;
          list.RiskId = subrisk.RiskId;
          list.SubRiskDesc = subrisk.SubRiskDesc;
          list.Updatedby = WebSecurity.CurrentUserId;
          list.Updatedon = DateTime.Now;
          list.Updatemachineinfo = ip;
          UpdateModel(list);
          db.SaveChanges();
          return RedirectToAction("SubRiskDetails");
        }
        return View();

      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /SubRisk/Delete/5

    public ActionResult Delete(int id)
    {
      var subrisk = db.SubRisksMaster.Where(x => x.SubRiskId == id);
      if (subrisk.Any())
      {
        db.SubRisksMaster.Remove(subrisk.First());
        db.SaveChanges();
      }
      return RedirectToAction("SubRiskDetails");
    }

    //
    // POST: /SubRisk/Delete/5

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

  }
}
