using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rcsa.Web.Models;
using WebMatrix.WebData;
using Rcsa.Web.Helpers;

namespace Rcsa.Web.Controllers
{
  [Authorize]
  public class CompanyController : Controller
  {
    //
    // GET: /Company/

    RcsaDb db = new RcsaDb();
    public ActionResult Index()
    {
      return View();
    }

        public ActionResult CompanyDetails(string page)
        {
            bool isUserAConsultant = UsersHelper.isUserACompany();

            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            var list = db.CompaniesMaster.OrderBy(x => x.ComapnyName).ToList();
            return View(list);
        }


        [HttpPost]
    public ActionResult CompanyDetails(string keyword, string command)
    {
      var master = new List<CompanyMaster>();
      if (keyword != "" && keyword != null)
      {
        master = db.CompaniesMaster.Where(x => x.ComapnyName.Contains(keyword)).ToList();// || x.SubRisksMaster.SubRiskName.Contains(keyword) || x.MitigantDesc.Contains(keyword)).ToList();
      }
      if (command == "Reset")
      {
        master = db.CompaniesMaster.OrderBy(x => x.ComapnyName).ToList();
      }
      ViewBag.Mode = "Search";
      return View(master);
    }

    //
    // GET: /Company/Details/5

    public ActionResult Details(int id, string page)
    {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;

            CompanyMaster list = db.CompaniesMaster.Where(x => x.CompanyId == id).SingleOrDefault();
      return View(list);
    }

    //
    // GET: /Company/Create

    public ActionResult Create()
    {
      return View();
    }

    //
    // POST: /Company/Create

    [HttpPost]
    public ActionResult Create(CompanyMaster company)
    {
      try
      {
        string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (String.IsNullOrWhiteSpace(ip))
        {
          ip = Request.ServerVariables["REMOTE_ADDR"];
        }
        company.InsertedBy = WebSecurity.CurrentUserId;
        company.Insertedon = DateTime.Now;
        company.Insertedmachineinfo = ip;

        db.CompaniesMaster.Add(company);
        db.SaveChanges();
        return RedirectToAction("CompanyDetails");
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /Company/Edit/5

    public ActionResult Edit(int id, string page)
    {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            var company = db.CompaniesMaster.FirstOrDefault(x => x.CompanyId == id);
      return View(company);
    }

    //
    // POST: /Company/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection, CompanyMaster objCompany)
    {
      try
      {
                string page = collection["page"];

        if (ModelState.IsValid)
        {
          var company = db.CompaniesMaster.FirstOrDefault(x => x.CompanyId == id);
          string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
          if (String.IsNullOrWhiteSpace(ip))
          {
            ip = Request.ServerVariables["REMOTE_ADDR"];
          }

          if (Request.Form["delete"] != null)
          {
            var list = db.CompaniesMaster.Where(x => x.CompanyId == objCompany.CompanyId);
            db.CompaniesMaster.Remove(list.First());
            db.SaveChanges();
            return RedirectToAction("CompanyDetails", new { page = page });
          }


          bool exists = db.CompaniesMaster.Any(x => x.ComapnyName == objCompany.ComapnyName && x.CompanyId != objCompany.CompanyId);
          if (exists)
          {
            ViewBag.Error = "Company Name already exist!";
            return View();
          }
          company.ComapnyName = objCompany.ComapnyName;
          company.CompanyNo = objCompany.CompanyNo;
          company.CompanyDescription = objCompany.CompanyDescription;
          company.Phone = objCompany.Phone;
          company.Updatedby = WebSecurity.CurrentUserId;
          company.Updatedon = DateTime.Now;
          company.Updatemachineinfo = ip;
          //UpdateModel(company);
          db.SaveChanges();
          return RedirectToAction("CompanyDetails", new { page = page });
        }
        else
        {
          return View();
        }
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /Company/Delete/5

    public ActionResult Delete(int id, string page)
    {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            if (User.IsInRole("Admin") || User.IsInRole("Consultant") || User.IsInRole("Supervisor") || User.IsInRole("Managers"))
            {
                var company = db.CompaniesMaster.Where(x => x.CompanyId == id);
                if (company.Any())
                {
                    var comp = company.First();
                    var riskHeaders = db.RiskAssessmentMaster.Where(x => x.CompanyID == comp.CompanyId).ToList();
                    foreach(var risk in riskHeaders)
                    {
                        var riskDetails = db.RiskDetails.Where(y => y.RiskAssessmentID == risk.RiskAssessmentId).ToList();
                        foreach(var rd in riskDetails)
                        {
                            db.RiskDetails.Remove(rd);
                        }
                        db.SaveChanges();
                        db.RiskAssessmentMaster.Remove(risk);
                    }
                    db.SaveChanges();
                    db.CompaniesMaster.Remove(company.First());
                    db.SaveChanges();
                }
            }
      return RedirectToAction("CompanyDetails", new { page = page });
    }

    //  try
    //  {
    //    // TODO: Add delete logic here

    //   // var q = db.CompaniesMaster.Where(x => x.ComapnyName == Name);
    //    //var user = q.Any() ? q.First() : new CompanyMaster();
    //    List<CompanyMaster> user = db.CompaniesMaster.Where(x => x.ComapnyName == Name).ToList();
    //    List<CompanyMaster> userlist=db.CompaniesMaster.Where(x => x.ComapnyName == Name).ToList();
    //    foreach (var list in user)
    //    {
    //      userlist.Add(list);

    //    }
    //    return View();
    //  }
    //  catch
    //  {
    //    return View();
    //  }
    //}  return View(list);
    //
    // POST: /Company/Delete/5

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
