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
  public class RiskController : Controller
  {
    //
    // GET: /Risk/

    RcsaDb db = new RcsaDb();
    public ActionResult Index()
    {
      return View();
    }

        //
        // GET: /Risk/Details/5
        public ActionResult RiskDetail(string page)
        {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;

            var list = db.RiskMasters.ToList();
            return View(list);
        }

    [HttpPost]
    public ActionResult RiskDetail(string keyword, string command)
    {
      List<RiskMaster> list = new List<RiskMaster>();
      if (keyword != "" && keyword != null)
      {
        list = db.RiskMasters.ToList();
        list = list.Where(x => x.RiskName.Contains(keyword)).ToList();// || x.Description.Contains(keyword)).ToList();
      }
      if (command == "Reset")
      {
        list = db.RiskMasters.ToList();

      }
      ViewBag.Mode = "Search";
      return View(list.OrderBy(x => x.RiskName));

    }

        public ActionResult Details(int id, string page)
        {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            RiskMaster list = db.RiskMasters.FirstOrDefault(x => x.RiskId == id);
            return View(list);
            //return View();
        }


        //
        // GET: /Risk/Create

        public ActionResult Create()
    {
      return View();
    }

    //
    // POST: /Risk/Create

    [HttpPost]
    public ActionResult Create(RiskMaster risk)
    {
      try
      {
        bool list = db.RiskMasters.Any(x => x.RiskName == risk.RiskName);
        if (list)
        {
          ViewBag.Error = "Risk name already exist!";
          return View();
        }
        risk.CreatedBy = WebSecurity.CurrentUserId;
        risk.CreatedOn = DateTime.Now;

        db.RiskMasters.Add(risk);
        db.SaveChanges();
        // TODO: Add insert logic here
        return RedirectToAction("RiskDetail");
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /Risk/Edit/5

    public ActionResult Edit(int id)
    {
      RiskMaster list = db.RiskMasters.FirstOrDefault(x => x.RiskId == id);
      return View(list);
    }

    //
    // POST: /Risk/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection, RiskMaster risk)
    {
      try
      {
        string page = collection["page"];
        // TODO: Add update logic here
        RiskMaster list = db.RiskMasters.FirstOrDefault(x => x.RiskId == id);
        if (ModelState.IsValid)
        {

          var exists = db.RiskMasters.Any(x => x.RiskName == risk.RiskName && x.RiskId != id);
          if (exists)
          {
            ViewBag.Error = "Risk name already exist!";
            return View();
          }

          if (Request.Form["delete"] != null)
          {
            var list1 = db.RiskMasters.Where(x => x.RiskId == id);
            db.RiskMasters.Remove(list1.First());
            db.SaveChanges();
            return RedirectToAction("RiskDetail", new { page = page });
          }
          list.RiskName = risk.RiskName;
          list.Description = risk.Description;
          list.UpdateBy = WebSecurity.CurrentUserId;
          list.UpdateOn = DateTime.Now;
          //UpdateModel(list);
          db.SaveChanges();
        }
        return RedirectToAction("RiskDetail", new { page = page });
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /Risk/Delete/5

    public ActionResult Delete(int id, string page)
    {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            if (User.IsInRole("Admin") || User.IsInRole("Consultant") || User.IsInRole("Supervisor") || User.IsInRole("Managers"))
            {
                var risk = db.RiskMasters.Where(x => x.RiskId == id);
                if (risk.Any())
                {
                    var rk = risk.FirstOrDefault();
                    var rd = db.RiskDetails.Where(r => r.RiskID == rk.RiskId);

                    foreach(var r in rd)
                    {
                        db.RiskDetails.Remove(r);
                    }
                    db.SaveChanges();
                    db.RiskMasters.Remove(risk.First());
                    db.SaveChanges();
                }
            }
                
      return RedirectToAction("RiskDetail", new { page = page });
      //return View();
    }

    //
    // POST: /Risk/Delete/5

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
    public JsonResult Risks()
    {
      List<RiskMaster> risks = db.RiskMasters.ToList();
      return Json(risks, JsonRequestBehavior.AllowGet);
    }

    #endregion
  }
}