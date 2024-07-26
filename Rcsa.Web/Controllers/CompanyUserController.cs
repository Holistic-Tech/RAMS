using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Rcsa.Web.Models;
using WebMatrix.WebData;
using Rcsa.Web.ViewModel;

namespace Rcsa.Web.Controllers
{

  [Authorize]
  public class CompanyUserController : Controller
  {
    //
    // GET: /CompanyUser/

    RcsaDb db = new RcsaDb();

    public ActionResult CompanyUsers(int? id, string compid, string page)
    {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            ViewBag.compId = Convert.ToInt32(compid);
      var list = new List<CompanyUser>();
      if (User.IsInRole("Admin") || User.IsInRole("Consultant"))
      {
        if (id != null)
        {
          list = db.CompanyUsers.Where(x => x.CompanyId == id).OrderBy(x => x.UsersProfile.UserName).ToList();
        }
        else
        {
          list = db.CompanyUsers.OrderBy(x => x.UsersProfile.UserName).ToList();
        }
      }
      else
      {
        var user = db.CompanyUsers.FirstOrDefault(x => x.UserId == WebSecurity.CurrentUserId);
        if (user != null)
          list = db.CompanyUsers.Where(x => x.CompanyId == user.CompanyId).OrderBy(x => x.UsersProfile.UserName).ToList();
        ViewBag.compId = Convert.ToInt32(user.CompanyId);
      }

      return View(list);
    }

    [HttpPost]
    public ActionResult CompanyUsers(string keyword, string command)
    {
      string userId = "";
      var userName = db.UserProfiles.Where(x => x.UserName == keyword).SingleOrDefault();
      if (userName != null)
      {
        userId = Convert.ToString(userName.UserId);
      }
      var FirstName = db.UserProfiles.Where(x => x.FirstName == keyword).SingleOrDefault();
      if (FirstName != null)
      {
        userId = Convert.ToString(FirstName.UserId);
      }
      var SecondName = db.UserProfiles.Where(x => x.LastName == keyword).SingleOrDefault();
      if (SecondName != null)
      {
        userId = Convert.ToString(SecondName.UserId);
      }
      var Email = db.UserProfiles.Where(x => x.Email == keyword).SingleOrDefault();
      if (Email != null)
      {
        userId = Convert.ToString(Email.UserId);
      }

      var list = new List<CompanyUser>();
      if (User.IsInRole("Admin") || User.IsInRole("Consultant"))
      {
        list = db.CompanyUsers.OrderBy(x => x.UsersProfile.UserName).ToList();
      }
      else
      {
        var user = db.CompanyUsers.FirstOrDefault(x => x.UserId == WebSecurity.CurrentUserId);
        if (user != null)
          list = db.CompanyUsers.Where(x => x.CompanyId == user.CompanyId).OrderBy(x => x.UsersProfile.UserName).ToList();
      }

      if (keyword != "" && keyword != null)
      {
        list = list.Where(x => userId.Contains(x.UserId.ToString())).ToList();
      }
      if (command == "Reset")
      {
        list = db.CompanyUsers.OrderBy(x => x.UsersProfile.UserName).ToList();
      }
      ViewBag.Mode = "Search";
      return View(list);
    }
    //
    // GET: /CompanyUser/Create

    public ActionResult Create(int? compId)
    {
      ViewBag.compId = 0;
      if (compId == null)
      {
        // compId = Convert.ToInt32(Request["hfcompid"]);
        var userID = db.CompanyUsers.FirstOrDefault(x => x.UserId == WebSecurity.CurrentUserId);
        if (userID != null)
          ViewBag.compId = userID.CompanyId;
      }
      else
      {
        ViewBag.compId = compId;
      }
      return View();
    }

    //
    // POST: /CompanyUser/Create

    public ActionResult UserDetail(int userId, string page)
    {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            var q = db.UserProfiles.Where(x => x.UserId == userId);
      var user = q.Any() ? q.First() : new UserProfile();
      return View(user);
    }

    [HttpPost]
    public ActionResult Create(RegisterModel model)//CompanyUser companyuser
    {
      try
      {
        string comUserType = "";
        int companyId = 0;
        if (Request["hfCompanyId"] != null && Request["hfCompanyId"].ToString().Length > 0)
          companyId = Convert.ToInt32(Request["hfCompanyId"]);
        ViewBag.compId = companyId;
        if (!User.IsInRole("Manager") || User.IsInRole("User"))
        {
          comUserType = Request["hfuType"] + "";
          if (comUserType == null || comUserType == "" || comUserType == "Select")
          {
            ModelState.AddModelError("", "Please select User Type for user");
            return View();
          }
        }
        else comUserType = "User";

        #region my OLDCode
        //UserProfile profile = new UserProfile();
        //if (ModelState.IsValid)
        //{
        //  profile.UserName = companyuser.UsersProfile.UserName;
        //  profile.FirstName = companyuser.UsersProfile.FirstName;
        //  profile.LastName = companyuser.UsersProfile.LastName;
        //  profile.Email = companyuser.UsersProfile.Email;
        //  profile.UpdateOn = DateTime.Now;
        //  db.UserProfiles.Add(profile);
        //  db.SaveChanges();
        //  int userid = profile.UserId;
        //  if (userid > 0)
        //  {
        //    companyuser.CompanyId = companyId;
        //    companyuser.UserId = userid;
        //    companyuser.UpdatedOn = DateTime.Now;
        //    db.CompanyUsers.Add(companyuser);
        //    db.SaveChanges();
        //  }
        //}
        //return RedirectToAction("CompanyUsers");
        #endregion
        if (ModelState.IsValid)
        {
          if (WebSecurity.UserExists(model.UserName))
          {
            ModelState.AddModelError("", "User name already exist!");
            return View(model);
          }

          var user = db.UserProfiles.Where(x => x.Email == model.Email);
          if (user.Any())
          {
            ModelState.AddModelError("", "Email already exist!");
            return View(model);
          }

          string confirmationToken =
            WebSecurity.CreateUserAndAccount(model.UserName, model.Password,
                                             new
                                             {
                                               FirstName = model.FirstName,
                                               LastName = model.LastName,
                                               Email = model.Email,
                                               IsActive = true
                                             },
                                             true);


          bool result = WebSecurity.ConfirmAccount(confirmationToken);
          if (result == true)
          {
            var currentuser = db.UserProfiles.Where(x => x.UserName == model.UserName).Select(i => new { i.UserId }).FirstOrDefault();
            var userdetails = db.UserProfiles.Where(x => x.UserId == currentuser.UserId).SingleOrDefault();

            userdetails.IsCompanyStaff = comUserType == "User" ? true : false;
            userdetails.IsCompanySupervisor = comUserType == "Manager" ? true : false;

            if(!User.IsInRole("Manager") || User.IsInRole("User"))
            {
                if (comUserType == "Select")
                {
                    ModelState.AddModelError("", "Select User Type!");
                    return View(model);
                }
            }

            if (userdetails.IsCompanyStaff.Value) Roles.AddUsersToRole(new[] { model.UserName }, "User");
            else if (userdetails.IsCompanySupervisor.Value) Roles.AddUsersToRole(new[] { model.UserName }, "Manager");


            //UpdateModel(userdetails);
            CompanyUser companyuser = new CompanyUser();
            companyuser.CompanyId = companyId;
            companyuser.UserId = currentuser.UserId;
            companyuser.CreatedOn = DateTime.Now;
            companyuser.CreatedBy = WebSecurity.CurrentUserId;
            db.CompanyUsers.Add(companyuser);
            db.SaveChanges();
            return RedirectToAction("CompanyUsers", new { compid = companyId });
          }
        }
      }
      catch (MembershipCreateUserException e)
      {
        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
        //return View();
      }
      //return RedirectToAction("CompanyUsers");
      return View();
    }

    //
    // GET: /CompanyUser/Edit/5

    public ActionResult Edit(int? userId)
    {
      var q = db.CompanyUsers.Where(x => x.UserId == userId);
      ViewBag.CurrentUserId = userId;
      var user = q.Any() ? q.First() : new CompanyUser();
      return View(user);
      //return View();
    }

    //
    // POST: /CompanyUser/Edit/5

    [HttpPost]
    public ActionResult Edit(int userId, FormCollection collection, CompanyUser model)
    {
      try
      {
        ViewBag.CurrentUserId = userId;
        string comUserType = Request["hfuType"] + "";
        var test = Request.Form["rdStaff"] + "";
        if (ModelState.IsValid)
        {
          if (String.IsNullOrWhiteSpace(model.UsersProfile.FirstName))
          {
            ModelState.AddModelError("", "Please enter first name");
            return View(model);
          }
          if (String.IsNullOrWhiteSpace(model.UsersProfile.LastName))
          {
            ModelState.AddModelError("", "Please enter last name");
            return View(model);
          }
          if (String.IsNullOrWhiteSpace(model.UsersProfile.Email))
          {
            ModelState.AddModelError("", "Please enter email");
            return View(model);
          }
          if (!String.IsNullOrWhiteSpace(model.UsersProfile.UserName) && WebSecurity.UserExists(model.UsersProfile.UserName) && model.UserId == 0)
          {
            ModelState.AddModelError("", "User name already exist!");
            return View(model);
          }

          var user = db.UserProfiles.Where(x => x.Email == model.UsersProfile.Email && x.UserId != model.UsersProfile.UserId);
          if (user.Any())
          {
            ModelState.AddModelError("", "Email already exist!");
            return View(model);
          }

          var q = db.UserProfiles.Where(x => x.UserId == model.UsersProfile.UserId);
          var newUser = q.Any() ? q.First() : new UserProfile();


          newUser.FirstName = model.UsersProfile.FirstName;
          newUser.LastName = model.UsersProfile.LastName;
          newUser.Email = model.UsersProfile.Email;
          newUser.UpdateOn = DateTime.Now;
          newUser.UpdateBy = WebSecurity.CurrentUserId;
          newUser.IsCompanyStaff = comUserType == "User" ? true : false;
          newUser.IsCompanySupervisor = comUserType == "Manager" ? true : false;
          if (comUserType == "Select")
          {
            ModelState.AddModelError("", "Select User Type!");
            return View(model);
          }
          //UpdateModel(newUser);
          db.SaveChanges();
          ViewBag.Status = "User updated successfully!";
                    return RedirectToAction("CompanyUsers");
        }
      }
      catch (MembershipCreateUserException e)
      {
        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
        //return View();
      }
      //return View(model);
      return View();

    }

    public ActionResult CompanyUser(int? id)
    {
      var list = new List<CompanyUser>();
      int currentUserId = WebSecurity.CurrentUserId;
      if (User.IsInRole("Admin") || User.IsInRole("Consultant"))
      {
        if (id != null)
        {
          ViewBag.CompId = id;
          list = db.CompanyUsers.Where(x => x.CompanyId == id).OrderBy(x => x.UsersProfile.UserName).ToList();
        }
        else
        {
          list = db.CompanyUsers.OrderBy(x => x.UsersProfile.UserName).ToList();
        }
      }
      else
      {
        var user = db.CompanyUsers.FirstOrDefault(x => x.UserId == WebSecurity.CurrentUserId);
        if (user != null)
          list = db.CompanyUsers.Where(x => x.CompanyId == user.CompanyId).OrderBy(x => x.UsersProfile.UserName).ToList();
      }
      return View(list);
    }

    [HttpPost]
    public ActionResult CompanyUser()
    {
      int id = Request["hfCompId"] != "" ? Convert.ToInt32(Request["hfCompId"]) : 0;
      ViewBag.CompId = id;
      var list = new List<CompanyUser>();
      int currentUserId = WebSecurity.CurrentUserId;
      if (User.IsInRole("Admin") || User.IsInRole("Consultant"))
      {
        if (id != 0)
        {
          list = db.CompanyUsers.Where(x => x.CompanyId == id).OrderBy(x => x.UsersProfile.UserName).ToList();
        }
        else
        {
          list = db.CompanyUsers.OrderBy(x => x.UsersProfile.UserName).ToList();
        }
      }
      else
      {
        var user = db.CompanyUsers.FirstOrDefault(x => x.UserId == WebSecurity.CurrentUserId);
        if (user != null)
          list = db.CompanyUsers.Where(x => x.CompanyId == user.CompanyId).OrderBy(x => x.UsersProfile.UserName).ToList();
      }
      return View(list);
    }

    public ActionResult AddCompanyUser()
    {
      return View();
    }

    [HttpPost]
    public ActionResult AddCompanyUser(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        try
        {
          if (WebSecurity.UserExists(model.UserName))
          {
            ModelState.AddModelError("", "User name already exist!");
            return View(model);
          }
          var user = db.UserProfiles.Where(x => x.Email == model.Email);
          if (user.Any())
          {
            ModelState.AddModelError("", "Email already exist!");
            return View(model);
          }
          string confirmationToken =
            WebSecurity.CreateUserAndAccount(model.UserName, model.Password,
                                             new
                                             {
                                               FirstName = model.FirstName,
                                               LastName = model.LastName,
                                               Email = model.Email
                                             },
                                             true);

          bool result = WebSecurity.ConfirmAccount(confirmationToken);

          if (result)
          {
            var profile = db.UserProfiles.FirstOrDefault(x => x.UserName == model.UserName);
            if (profile != null)
            {
              profile.UpdateBy = WebSecurity.CurrentUserId;
              profile.UpdateOn = DateTime.Now.Date;
              //UpdateModel(profile);
              db.SaveChanges();
              var roleName = "Company";
              if (!String.IsNullOrWhiteSpace(roleName))
              {
                System.Web.Security.Roles.AddUsersToRole(new[] { model.UserName }, roleName);
                if (roleName.Equals("Company", StringComparison.InvariantCultureIgnoreCase))
                {
                  var companyId = Request.Form["dlCompany"] + "";
                  if (!string.IsNullOrWhiteSpace(companyId))
                  {
                    CompanyViewModel.AssignCompany(profile.UserId, Convert.ToInt32(companyId));
                  }
                }
              }

              return RedirectToAction("CompanyUser");
            }
          }
        }
        catch (MembershipCreateUserException e)
        {
          ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
        }
      }
      return View(model);
    }

    public ActionResult UpdateCompanyUser(int userId)
    {
      var q = db.UserProfiles.Where(x => x.UserId == userId);
      var user = q.Any() ? q.First() : new UserProfile();
      return View(user);
    }

    [HttpPost]
    public ActionResult UpdateCompanyUser(UserProfile model)
    {
      try
      {
        if (!String.IsNullOrWhiteSpace(model.UserName) && WebSecurity.UserExists(model.UserName) && model.UserId == 0)
        {
          ModelState.AddModelError("", "User name already exist!");
          return View(model);
        }

        var user = db.UserProfiles.Where(x => x.Email == model.Email && x.UserId != model.UserId);
        if (user.Any())
        {
          ModelState.AddModelError("", "Email already exist!");
          return View(model);
        }

        var q = db.UserProfiles.Where(x => x.UserId == model.UserId);
        var newUser = q.Any() ? q.First() : new UserProfile();

        if (ModelState.IsValid)
        {
          if (model.FirstName != null && model.LastName != null)
          {
            newUser.FirstName = model.FirstName;
            newUser.LastName = model.LastName;
            newUser.Email = model.Email;
            newUser.UpdateBy = WebSecurity.CurrentUserId;
            newUser.UpdateOn = DateTime.Now;
            //UpdateModel(newUser);
            db.SaveChanges();

            CompanyViewModel.RemoveFromCompany(model.UserId);
            var companyId = Request.Form["dlCompany"] + "";
            if (companyId != null)
            {
              if (!String.IsNullOrWhiteSpace(companyId))
              {
                CompanyViewModel.AssignCompany(model.UserId, Convert.ToInt32(companyId));
              }
            }

            ViewBag.Status = "User updated successfully!";
            return RedirectToAction("CompanyUser", new { id = companyId });
          }
          else
          {
            ModelState.AddModelError("", "Please enter Name!");
            return View(model);
          }
        }
        else
        {
          return View(model);
        }
      }
      catch (MembershipCreateUserException e)
      {
        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
      }

      return View(model);
    }

    //
    // GET: /CompanyUser/Delete/5

    public ActionResult Delete(int id, string page)
    {
            var usr = db.CompanyUsers.Where(x => x.UserId == id);
            var cmp = usr.First().CompanyId;
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            if (User.IsInRole("Admin") || User.IsInRole("Consultant") || User.IsInRole("Manager"))
            {
                if (usr.Any())
                {
                    var roles = Roles.GetRolesForUser(usr.First().UsersProfile.UserName);

                    foreach (var rl in roles)
                    {
                        Roles.RemoveUserFromRole(usr.First().UsersProfile.UserName, rl);
                    }

                    db.CompanyUsers.Remove(usr.First());
                    db.SaveChanges();
                }
            }
            return RedirectToAction("CompanyUser", new { id = cmp, page = page });
    }

    //
    // POST: /CompanyUser/Delete/5

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

    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://go.microsoft.com/fwlink/?LinkID=177550 for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "User name already exists. Please enter a different user name.";

        case MembershipCreateStatus.DuplicateEmail:
          return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
  }
}
