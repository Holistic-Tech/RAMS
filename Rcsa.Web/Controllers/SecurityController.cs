using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Rcsa.Web.Models;
using WebMatrix.WebData;
using System.Collections.Generic;

using System.Web;
using Rcsa.Web.ViewModel;
//using System.Web.Security;
//using Rcsa.Web.Models;
//using WebMatrix.WebData;

namespace Rcsa.Web.Controllers
{
  [Authorize]
  public class SecurityController : Controller
  {
    RcsaDb db = new RcsaDb();
    public ActionResult Users()
    {
      var users = db.UserProfiles.OrderBy(x => x.FirstName);
      return View(users.ToList());
    }

    public ActionResult ChangePassword(int userId)
    {
      ViewBag.UserId = userId;
      return View();
    }

    [HttpPost]
    public ActionResult ChangePassword(LocalResetPasswordModel model)
    {
      int userId;
      int.TryParse(Request.Form["hdUserId"], out userId);
      if (ModelState.IsValid)
      {
        bool changePasswordSucceeded;
        try
        {
          var userName = db.UserProfiles.FirstOrDefault(x => x.UserId == userId).UserName;
          var token = WebSecurity.GeneratePasswordResetToken(userName);
          var result = WebSecurity.ResetPassword(token, model.NewPassword);
          changePasswordSucceeded = true;
        }
        catch (Exception ex)
        {
          changePasswordSucceeded = false;
        }

        if (changePasswordSucceeded)
        {
          ViewBag.Message = "Password changed successfully";
        }
        else
        {
          ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        }
      }

      ViewBag.UserId = userId;
      return View();
    }

    [HttpPost]
    public ActionResult Users(string keyword, string command)
    {
      var userList = new List<UserProfile>();
      if (keyword != "" && keyword != null)
      {
        userList = db.UserProfiles.Where(x => x.UserName.Contains(keyword) ||
                           x.FirstName.Contains(keyword) || x.LastName.Contains(keyword) || x.Email.Contains(keyword)).ToList();
      }
      if (command == "Reset")
      {
        userList = db.UserProfiles.OrderBy(x => x.FirstName).ToList();
      }
      ViewBag.Mode = "Search";
      return View(userList);
    }

    public ActionResult UsersEdit(int userId)
    {
      var q = db.UserProfiles.Where(x => x.UserId == userId);
      var user = q.Any() ? q.First() : new UserProfile();
      return View(user);
    }

    [HttpPost]
    public ActionResult UsersEdit(UserProfile model, string isActive)
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

        model.IsActive = newUser.IsActive = !String.IsNullOrWhiteSpace(isActive);
        newUser.FirstName = model.FirstName;
        newUser.LastName = model.LastName;
        newUser.Email = model.Email;
        newUser.UpdateOn = DateTime.Now;
        newUser.UpdateBy = WebSecurity.CurrentUserId;

        ////newUser.UpdateBy =
        //if (newUser.UserId == 0)
        //{
        //  db.UserProfiles.Add(newUser);
        //}

        db.SaveChanges();
        foreach (string roleName in System.Web.Security.Roles.GetRolesForUser(model.UserName))
        {
          System.Web.Security.Roles.RemoveUserFromRole(model.UserName, roleName);
        }

        //Remove from company if any assigned
        CompanyViewModel.RemoveFromCompany(model.UserId);

        if (!String.IsNullOrWhiteSpace(Request.Form["Role"] + ""))
        {
          var roleName = Request.Form["Role"] + "";
          System.Web.Security.Roles.AddUsersToRole(new[] { model.UserName }, roleName);
          //Assing company if in company role
          if (roleName.Equals("Manager", StringComparison.InvariantCultureIgnoreCase) || roleName.Equals("User", StringComparison.InvariantCultureIgnoreCase))
          {
            var companyId = Request.Form["dlCompany"] + "";
            if (!String.IsNullOrWhiteSpace(companyId))
            {
              CompanyViewModel.AssignCompany(model.UserId, Convert.ToInt32(companyId));
            }
          }
        }


        ViewBag.Status = "User updated successfully!";
      }
      catch (MembershipCreateUserException e)
      {
        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
      }

      return View(model);
    }

    public ActionResult UsersAdd()
    {
      return View();
    }
    [HttpPost]
    public ActionResult UsersAdd(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        // Attempt to register the user
        try
        {
          //Check if user already exist 
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

          //Check if email id already exist

          string confirmationToken =
            WebSecurity.CreateUserAndAccount(model.UserName, model.Password,
                                             new
                                             {
                                               FirstName = model.FirstName,
                                               LastName = model.LastName,
                                               Email = model.Email,
                                               IsActive=true
                                             },
                                             true);


          bool result = WebSecurity.ConfirmAccount(confirmationToken);


          if (result)
          {
            var profile = db.UserProfiles.FirstOrDefault(x => x.UserName == model.UserName);
            if (profile != null)
            {
              var roleName = Request.Form["dlRole"] + "";
              if (!String.IsNullOrWhiteSpace(roleName))
              {
                System.Web.Security.Roles.AddUsersToRole(new[] { model.UserName }, roleName);
                //Assign company if in company role
                if (roleName.Equals("Manager", StringComparison.InvariantCultureIgnoreCase) || roleName.Equals("User", StringComparison.InvariantCultureIgnoreCase))
                {
                    var companyId = Request.Form["dlCompany"] + "";
                    if (!String.IsNullOrWhiteSpace(companyId))
                    {
                        CompanyViewModel.AssignCompany(profile.UserId, Convert.ToInt32(companyId));
                    }
                }
              }

              return RedirectToAction("Users");
            }
          }
        }
        catch (MembershipCreateUserException e)
        {
          ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    public ActionResult UsersRole()
    {
      //var role = Roles.Provider;

      //Roles.GetUsersInRole("admin");
      //return View();

      var role = System.Web.Security.Roles.GetRolesForUser().Single();
      var users = db.UserProfiles.OrderBy(x => x.FirstName);
      return View(users.ToList());
    }

    [HttpPost]
    public ActionResult UsersRole(UserProfile model)
    {
      try
      {
        string userName = "", roleName = "";
        int userId = 0, newEdit = 0;
        if (Request["hfNewEdit"] != null && Request["hfNewEdit"].ToString().Length > 0)
          newEdit = Convert.ToInt32(Request["hfNewEdit"]);
        if (Request["ddlUser"] != null && Request["ddlUser"].ToString().Length > 0)
          userId = Convert.ToInt32(Request["ddlUser"]);
        if (Request["ddlRole"] != null && Request["ddlRole"].ToString().Length > 0)
          roleName = Request["ddlRole"].ToString();
        if (userId > 0)
        {
          var user = db.UserProfiles.FirstOrDefault(x => x.UserId == userId);
          string[] role = System.Web.Security.Roles.GetRolesForUser(user.UserName);
          if (newEdit == 0)
          {
            if (role.Length > 0)
            {
              ViewBag.Status = "Role Already Exists!";
              //ModelState.AddModelError("", "Role Already Define");
              return RedirectToAction("UsersRole");

            }
            System.Web.Security.Roles.AddUsersToRole(new[] { user.UserName }, roleName);
          }
          else if (newEdit == 1)
          {
            if (role.Length > 0)
            {
              System.Web.Security.Roles.RemoveUserFromRole(user.UserName, role[0]);
            }
            System.Web.Security.Roles.AddUsersToRole(new[] { user.UserName }, roleName);
          }
        }
        //return View();
        return RedirectToAction("UsersRole");
      }
      catch (Exception ex)
      {
        return RedirectToAction("UsersRole");
      }
    }

    public ActionResult UserDetail(int userId)
    {
      var q = db.UserProfiles.Where(x => x.UserId == userId);
      var user = q.Any() ? q.First() : new UserProfile();
      return View(user);
    }

    public ActionResult Roles()
    {
      return View();
    }

    public ActionResult RolesModule()
    {
      return View();
    }

    public ActionResult UserDepartment(int? id)
    {
      if (id == null)
        id = 0;
      ViewBag.companyId = id;
      return View();
    }

    public ActionResult bindUserDepartment(int? id)
    {
      ViewBag.companyId = id;
      return View("UserDepartment");
    }
    #region OLD Code
    //[HttpPost]
    //public ActionResult UserDepartment(FormCollection collection, string[] optionDepartment)
    //{
    //  int userId = 0, companyId = 0;
    //  if (optionDepartment != null && optionDepartment.Length > 0)
    //  {
    //    //Delete(UserId);

    //    if (Request["ddlUser"] != null && Request["ddlUser"].ToString().Length > 0)
    //      userId = Convert.ToInt32(Request["ddlUser"]);
    //    if (Request["ddlCompany"] != null && Request["ddlCompany"].ToString().Length > 0)
    //      companyId = Convert.ToInt32(Request["ddlCompany"]);

    //      var department = db.UserDepartments.Where(x => x.UserId == 1 && x.CompanyId==companyId).ToList();
    //      if (department.Any())
    //      {
    //        foreach (var p in department)
    //        {
    //          db.UserDepartments.Remove(p);
    //          db.SaveChanges();
    //        }
    //      }
    //    for (int i = 0; i < optionDepartment.Length; i++)
    //    {
    //      var userDepartment = new UserDepartment ();
    //      userDepartment.CompanyId = companyId;
    //      userDepartment.DepartmentId = Convert.ToInt32(optionDepartment[i]);
    //      userDepartment.UserId = 1;
    //      db.UserDepartments.Add(userDepartment);
    //      db.SaveChanges();
    //    }
    //  }
    //  var q = db.UserDepartments;
    //  return View();
    //}
    #endregion
    public ActionResult UpdateUserDepartment(string CompanyID, string UserID, string[] departments)// 
    {
      int userId = 0, companyId = 0;
      if (departments != null && departments.Length > 0)
      {
        userId = Convert.ToInt32(UserID);
        companyId = Convert.ToInt32(CompanyID);
        var department = db.UserDepartments.Where(x => x.UserId == userId && x.CompanyId == companyId).ToList();
        if (department.Any())
        {
          foreach (var p in department)
          {
            db.UserDepartments.Remove(p);
            db.SaveChanges();
          }
        }
        for (int i = 0; i < departments.Length; i++)
        {
          var userDepartment = new UserDepartment();
          userDepartment.CompanyId = companyId;
          userDepartment.DepartmentId = Convert.ToInt32(departments[i]);
          userDepartment.UserId = userId;
          db.UserDepartments.Add(userDepartment);
          db.SaveChanges();
        }
      }

      return View();
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
