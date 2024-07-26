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
  public class MitigantController : Controller
  {
    //
    // GET: /Mitigant/

    RcsaDb db = new RcsaDb();
    public ActionResult MitigantDetails()
    {
      //var list = db.RiskMasters.OrderByDescending(x => x.CreatedOn).ToList();
      var list = db.MitigantsMaster.ToList();
      return View(list);
    }

    [HttpPost]
    public ActionResult MitigantDetails(string keyword, string command)
    {
      var list = new List<MitigantMaster>();
      if (!String.IsNullOrEmpty(keyword))
      {
        list = db.MitigantsMaster.Where(x => x.MitigantName.Contains(keyword)).ToList();
      }
      if (command == "Reset")
      {
        list = db.MitigantsMaster.ToList();
      }
      ViewBag.Mode = "Search";
      return View(list.OrderBy(x => x.MitigantName));
    }
    //
    // GET: /Mitigant/Details/5

    public ActionResult Details(int id)
    {
      var mitigant = db.MitigantsMaster.FirstOrDefault(x => x.MitigantId == id);
      return View(mitigant ?? new MitigantMaster());
    }

    //
    // GET: /Mitigant/Create

    public ActionResult Create()
    {
      return View();
    }

    //
    // POST: /Mitigant/Create


    public bool IsExist(string mitigantName, int id = 0)
    {
      if (id == 0)
        return db.MitigantsMaster.Any(x => x.MitigantName == mitigantName);
      else
        return db.MitigantsMaster.Any(x => x.MitigantName == mitigantName && x.SubRiskId == id);
    }

    public bool IsExist(string mitigantName, int subRiskId = 0, int id = 0)
    {
      if (id == 0)
        return db.MitigantsMaster.Any(x => x.MitigantName == mitigantName);
      else
        return db.MitigantsMaster.Any(x => x.MitigantName == mitigantName && x.SubRiskId == subRiskId && x.MitigantId != id);
    }



    [HttpPost]
    public ActionResult Create(MitigantMaster mitigant)
    {
      try
      {
        var riskid = Request["ddlRisk"] + "";

        ViewBag.SubRiskSelected = mitigant.SubRiskId + "";
        ViewBag.RiskSelected = riskid;
        ViewBag.MitigantSelected = mitigant.MitigantId + "";
        //  ViewBag.SubRiskSelected = mitigant.SubRiskId + "";

        if (IsExist(mitigant.MitigantName.Trim(), mitigant.SubRiskId))
        {
          ViewBag.Error = "Mitigant name is already exist!";
          return View();
        }

        string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (String.IsNullOrWhiteSpace(ip))
        {
          ip = Request.ServerVariables["REMOTE_ADDR"];
        }
        mitigant.InsertedBy = WebSecurity.CurrentUserId;
        mitigant.Insertedon = DateTime.Now;
        mitigant.Insertedmachineinfo = ip;
        db.MitigantsMaster.Add(mitigant);
        db.SaveChanges();

        return RedirectToAction("MitigantDetails");
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /Mitigant/Edit/5

    public ActionResult Edit(int id)
    {
      MitigantMaster list = db.MitigantsMaster.Where(x => x.MitigantId == id).SingleOrDefault();
      return View(list);
    }

    //
    // POST: /Mitigant/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection, MitigantMaster objMitigant)
    {
      string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      if (String.IsNullOrWhiteSpace(ip))
      {
        ip = Request.ServerVariables["REMOTE_ADDR"];
      }
      try
      {
        MitigantMaster list = db.MitigantsMaster.Where(x => x.MitigantId == id).SingleOrDefault();
        list.MitigantName = objMitigant.MitigantName;
        list.SubRiskId = objMitigant.SubRiskId;
        list.MitigantDesc = objMitigant.MitigantDesc;
        list.Updatedby = WebSecurity.CurrentUserId;
        list.Updatedon = DateTime.Now;
        list.Updatemachineinfo = ip;
        if (ModelState.IsValid)
        {
          if (objMitigant.SubRiskId != null && objMitigant.MitigantName != null)
          {
            if (IsExist(list.MitigantName.Trim(), list.SubRiskId, list.MitigantId))
            {
              // ViewBag.Error = "Mitigant name is already exist!";
              return RedirectToAction("MitigantDetails");
            }

            UpdateModel(list);
            db.SaveChanges();

            return RedirectToAction("MitigantDetails");
          }
          else
          {
            ModelState.AddModelError("", "Name is required!");
            return View(objMitigant);
          }
        }
        else
        {

          return RedirectToAction("MitigantDetails");

        }
      }
      catch
      {
        return View(objMitigant);
      }
    }

    //
    // GET: /Mitigant/Delete/5

    public ActionResult Delete(int id)
    {
      var mitigant = db.MitigantsMaster.Where(x => x.MitigantId == id);
      if (mitigant.Any())
      {
        db.MitigantsMaster.Remove(mitigant.First());
        db.SaveChanges();
      }
      return RedirectToAction("MitigantDetails");
    }

    //
    // POST: /Mitigant/Delete/5

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



    #region JSON Response Functions

    [HttpGet]
    public JsonResult Mitigants(int subRiskId)
    {
      List<MitigantMaster> mitigants = db.MitigantsMaster.Where(x => x.SubRiskId == subRiskId).ToList();
      return Json(mitigants, JsonRequestBehavior.AllowGet);
    }

    #endregion
  }
}
