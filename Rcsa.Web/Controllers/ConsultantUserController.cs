using Rcsa.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Rcsa.Web.Controllers
{
  [Authorize]
  public class ConsultantUserController : Controller
  {
    //
    // GET: /ConsultantUser/
    RcsaDb db = new RcsaDb();
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult ConsultantUsers(int? id, string page)
    {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;

            int currentUserId = WebSecurity.CurrentUserId;
      IList<UserProfile> list = null;
      var listofUsers = new List<UserProfile>();
      if (User.IsInRole("Admin"))
      {
        var lst = db.UserProfiles.OrderBy(x => x.FirstName).ToList();
        foreach (var item in lst)
        {
          var roles = (SimpleRoleProvider)Roles.Provider;
          if (roles.GetRolesForUser(item.UserName).SingleOrDefault() == "Consultant")
          {
            listofUsers.Add(item);
          }
        }
        list = listofUsers;
      }
      else
      {
        list = db.UserProfiles.Where(x => x.UpdateBy == currentUserId).ToList();
      }

      return View(list);
    }

    [HttpPost]
    public ActionResult ConsultantUsers(string keyword, string command)
    {
      var userList = new List<UserProfile>();

      if (keyword != "" && keyword != null)
      {
        userList = db.UserProfiles.Where(x => x.UserName.Contains(keyword) || x.FirstName.Contains(keyword) || x.LastName.Contains(keyword) || x.Email.Contains(keyword)).ToList();
      }
      if (command == "Reset")
      {
        userList = db.UserProfiles.OrderBy(x => x.FirstName).ToList();
      }

      ViewBag.Mode = "Search";
      return View(userList);
    }

    //
    // GET: /ConsultantUser/Details/5

    public ActionResult Details(int? userId, string page)
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

    //
    // GET: /ConsultantUser/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: /ConsultantUser/Create
    [HttpPost]
    public ActionResult Create(RegisterModel model)
    {
      try
      {
        if (ModelState.IsValid)
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

          string confirmationToken =
            WebSecurity.CreateUserAndAccount(model.UserName, model.Password,
                                             new
                                             {
                                               FirstName = model.FirstName,
                                               LastName = model.LastName,
                                               Email = model.Email,
                                             },
                                             true);


          bool result = WebSecurity.ConfirmAccount(confirmationToken);
          if (result == true)
          {
            var currentuser = db.UserProfiles.Where(x => x.UserName == model.UserName).Select(i => new { i.UserId }).FirstOrDefault();
            Roles.AddUsersToRole(new[] { model.UserName }, "Consultant");
            var userUpdate = db.UserProfiles.Where(x => x.UserName == model.UserName).SingleOrDefault();
            userUpdate.UpdateBy = WebSecurity.CurrentUserId;
            userUpdate.UpdateOn = DateTime.Now;
            //UpdateModel(userUpdate);
            db.SaveChanges();
            return RedirectToAction("ConsultantUsers");
          }
        }
      }
      catch (MembershipCreateUserException e)
      {
        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
      }
      return View();
    }

    //
    // GET: /ConsultantUser/Edit/5
    public ActionResult Update(int? userId)
    {
      var q = db.UserProfiles.Where(x => x.UserId == userId);
      var user = q.Any() ? q.First() : new UserProfile();
      return View(user);
    }

    [HttpPost]
    public ActionResult Update(int userId, FormCollection collection, UserProfile model)
    {
      try
      {
                string page = collection["page"];
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


        var users = db.UserProfiles.Where(x => x.UserId == model.UserId).SingleOrDefault(); ;
        if ((model.FirstName != null) && (model.LastName != null))
        {
          if (ModelState.IsValid)
          {
            users.FirstName = model.FirstName;
            users.LastName = model.LastName;
            users.Email = model.Email;
            users.UpdateOn = DateTime.Now;
            users.UpdateBy = WebSecurity.CurrentUserId;
            db.SaveChanges();
            ViewBag.Status = "User updated successfully!";
            return RedirectToAction("ConsultantUsers", "ConsultantUser", new { page = page });
          }
        }
        else
        {

          ModelState.AddModelError("", "Please enter Name !");
          return View(model);
        }
      }
      catch (MembershipCreateUserException e)
      {
        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
      }

      return View(model);
    }

        public ActionResult Delete(int id, string page)
        {
            if (page == null)
            {
                page = null;
            }
            ViewBag.CurrentlyOnPage = page;
            var usr = db.UserProfiles.Where(x => x.UserId == id);

            if (User.IsInRole("Admin") || User.IsInRole("Consultant") || User.IsInRole("Supervisor") || User.IsInRole("Managers"))
            {
                if (usr.Any())
                {
                    var roles = Roles.GetRolesForUser(usr.First().UserName);

                    foreach (var rl in roles)
                    {
                        Roles.RemoveUserFromRole(usr.First().UserName, rl);
                    }

                    db.UserProfiles.Remove(usr.First());
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ConsultantUsers", new { page = page });
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
