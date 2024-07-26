using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
    public ActionResult MitigantDetails(string page)
    {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            var list = new List<MitigantMaster>();
            if (User.IsInRole("Admin") || User.IsInRole("Consultant"))
            {
                list = db.MitigantsMaster.ToList();
            }
            else
            {
                var adminConsultantUsers = new List<string>();
                var ids = new List<int>();
                adminConsultantUsers.AddRange(Roles.GetUsersInRole("Admin"));
                adminConsultantUsers.AddRange(Roles.GetUsersInRole("Consultant"));
                ids = db.UserProfiles.Where(x => adminConsultantUsers.Contains(x.UserName)).Select(x => x.UserId).ToList();
                var currentComp = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).Select(x => x.CompanyId).FirstOrDefault();
                var companyUsers = db.CompanyUsers.Where(x => x.CompanyId == currentComp).Select(x => x.UserId).ToList();
                //var list = db.RiskMasters.OrderByDescending(x => x.CreatedOn).ToList();
                list = db.MitigantsMaster.Where(x => companyUsers.Contains((int)x.InsertedBy) || x.InsertedBy == 1 || ids.Contains((int)x.InsertedBy)).ToList();
            }
            
      return View(list);
    }

    [HttpPost]
    public ActionResult MitigantDetails(string keyword, string command)
    {
      var list = new List<MitigantMaster>();
            if (User.IsInRole("Admin") || User.IsInRole("Consultant"))
            {
                if (!String.IsNullOrEmpty(keyword))
                {
                    list = db.MitigantsMaster.Where(x => x.MitigantName.Contains(keyword)).ToList();
                }
                if (command == "Reset")
                {
                    list = db.MitigantsMaster.ToList();
                }
                ViewBag.Mode = "Search";
            }
            else
            {
            var currentComp = db.CompanyUsers.Where(x => x.UserId == WebSecurity.CurrentUserId).Select(x => x.CompanyId).FirstOrDefault();
            var companyUsers = db.CompanyUsers.Where(x => x.CompanyId == currentComp).Select(x => x.UserId).ToList();
                var adminConsultantUsers = new List<string>();
                var ids = new List<int>();
                adminConsultantUsers.AddRange(Roles.GetUsersInRole("Admin"));
                adminConsultantUsers.AddRange(Roles.GetUsersInRole("Consultant"));
                ids = db.UserProfiles.Where(x => adminConsultantUsers.Contains(x.UserName)).Select(x => x.UserId).ToList();
                if (!String.IsNullOrEmpty(keyword))
              {
                list = db.MitigantsMaster.Where(x => (companyUsers.Contains((int)x.InsertedBy) || x.InsertedBy == 1 || ids.Contains((int)x.InsertedBy)) && x.MitigantName.Contains(keyword)).ToList();
              }
              if (command == "Reset")
              {
                list = db.MitigantsMaster.Where(x => companyUsers.Contains((int)x.InsertedBy) || x.InsertedBy == 1 || ids.Contains((int)x.InsertedBy)).ToList();
              }
              ViewBag.Mode = "Search";
            }

      return View(list.OrderBy(x => x.MitigantName));
    }
    //
    // GET: /Mitigant/Details/5

    public ActionResult Details(int id, string page)
    {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;

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
                string page = collection["page"];
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
              return RedirectToAction("MitigantDetails", new { page = page });
            }

            //UpdateModel(list);
            db.SaveChanges();

            return RedirectToAction("MitigantDetails", new { page = page });
          }
          else
          {
            ModelState.AddModelError("", "Name is required!");
            return View(objMitigant);
          }
        }
        else
        {

          return RedirectToAction("MitigantDetails", new { page = page });

        }
      }
      catch
      {
        return View(objMitigant);
      }
    }

        //
        // GET: /Mitigant/Delete/5

        public ActionResult Delete(int id, string page)
        {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            if (User.IsInRole("Admin") || User.IsInRole("Consultant") || User.IsInRole("Supervisor") || User.IsInRole("Managers"))
            {
                var mitigant = db.MitigantsMaster.Where(x => x.MitigantId == id);
                if (mitigant.Any())
                {
                    var mt = mitigant.FirstOrDefault();
                    var rd = db.RiskDetails.Where(r => r.MitigantID == mt.MitigantId);

                    foreach (var r in rd)
                    {
                        db.RiskDetails.Remove(r);
                    }
                    db.SaveChanges();
                    db.MitigantsMaster.Remove(mitigant.First());
                    db.SaveChanges();
                }
            }
            return RedirectToAction("MitigantDetails", new { page = page });
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
