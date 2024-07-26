using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Rcsa.Web.Models;
using WebMatrix.WebData;

namespace Rcsa.Web.Controllers
{
  [Authorize]
  public class DepartmentController : Controller
  {
    //
    // GET: /Department/

    RcsaDb db = new RcsaDb();
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult DepartmentDetails()
    {

      int currentUserId = WebSecurity.CurrentUserId;
      string role = System.Web.Security.Roles.GetRolesForUser().Single();
      var _res = new List<DepartmentMaster>();

      if (role.ToLower() == "company")
      {
        var user = db.CompanyUsers.FirstOrDefault(x => x.UserId == currentUserId);
        _res = db.DepartmentsMaster.Where(x => x.CompanyId == user.CompanyId).ToList();
      }
      else
      {
        _res = db.DepartmentsMaster.ToList();
      }
      return View(_res);
    }
    //
    // GET: /Department/Details/5

    public ActionResult Details(int id)
    {
      DepartmentMaster list = db.DepartmentsMaster.FirstOrDefault(x => x.DepartmentId == id);
      return View(list);
    }

    //
    // GET: /Department/Create

    public ActionResult Create()
    {
      return View();
    }

    //
    // POST: /Department/Create

    [HttpPost]
    public ActionResult Create(DepartmentMaster department)
    {
      try
      {
        string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (String.IsNullOrWhiteSpace(ip))
        {
          ip = Request.ServerVariables["REMOTE_ADDR"];
        }
        department.InsertedBy = WebSecurity.CurrentUserId;
        department.Insertedon = DateTime.Now;
        department.Insertedmachineinfo = ip;
        db.DepartmentsMaster.Add(department);
        db.SaveChanges();
        return RedirectToAction("DepartmentDetails");
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /Department/Edit/5

    public ActionResult Edit(int id)
    {
      var list = db.DepartmentsMaster.FirstOrDefault(x => x.DepartmentId == id);

      return View(list ?? new DepartmentMaster());
    }

    //
    // POST: /Department/Edit/5

    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection, DepartmentMaster department)
    {
      string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      if (String.IsNullOrWhiteSpace(ip))
      {
        ip = Request.ServerVariables["REMOTE_ADDR"];
      }      
      try
      {
        if (ModelState.IsValid)
        {
          var list = db.DepartmentsMaster.FirstOrDefault(x => x.DepartmentId == id);
          list.DepartmentName = department.DepartmentName;
          list.CompanyId = department.CompanyId;
          list.Updatedby =WebSecurity.CurrentUserId ;
          list.Updatedon = DateTime.Now;
          list.Updatemachineinfo = ip;
          UpdateModel(list);
          db.SaveChanges();
        }
        return RedirectToAction("DepartmentDetails");
      }
      catch
      {
        return View();
      }
    }

    //
    // GET: /Department/Delete/5

    public ActionResult Delete(int id)
    {
      var department = db.DepartmentsMaster.Where(x => x.DepartmentId == id);
      if (department.Any())
      {
        db.DepartmentsMaster.Remove(department.First());
        db.SaveChanges();
      }
      return RedirectToAction("DepartmentDetails");
    }

    //
    // POST: /Department/Delete/5

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
    public JsonResult UserDepartments()
    {
      var list = (from x in db.DepartmentsMaster
                  join u in db.UserDepartments on x.DepartmentId equals u.DepartmentId
                  where u.UserId == WebSecurity.CurrentUserId && x.CompanyId == u.CompanyId
                  select x).ToList();

      return Json(list, JsonRequestBehavior.AllowGet);
    }    

    #endregion
  }
}
