using System;
using Rcsa.Web.Models;
using System.Linq;
using System.Collections.Generic;
using WebMatrix.WebData;

namespace Rcsa.Web.ViewModel
{
  public class CompanyViewModel
  {
    public static List<CompanyMaster> GetAllCompany()
    {
      using (var db = new RcsaDb())
      {
        var q = from x in db.CompaniesMaster
                orderby x.ComapnyName
                select x;

        return q.ToList();
      }
    }

    public static CompanyMaster GetBydUserId(int userId)
    {
      using (var db=new RcsaDb())
      {
        var q = db.CompanyUsers.FirstOrDefault(x => x.UserId == userId);
        return q == null ? new CompanyMaster() : q.CompanyMasters;
      }
    }

    public static void RemoveFromCompany(int userId)
    {
      using (var db = new RcsaDb())
      {
        foreach (var cUser in db.CompanyUsers.Where(x => x.UserId == userId).ToList())
        {
          db.CompanyUsers.Remove(cUser);
        }
        db.SaveChanges();
      }
    }

    public static void AssignCompany(int userId, int companyId)
    {
      using (var db=new RcsaDb())
      {
        var q = db.CompanyUsers.FirstOrDefault(x => x.UserId == userId);
        var userCompany = q ?? new CompanyUser();
        userCompany.UserId = userId;
        userCompany.CompanyId = companyId;        
        if (userCompany.CompanyUserId != 0)
        {
          userCompany.UpdatedOn = DateTime.Now;
          userCompany.UpdatedBy = WebSecurity.CurrentUserId;
        }
        {
          userCompany.CreatedOn = DateTime.Now;
          userCompany.CreatedBy = WebSecurity.CurrentUserId;
        }
        if (userCompany.CompanyUserId == 0)
          db.CompanyUsers.Add(userCompany);

        db.SaveChanges();
      }
    }
  }
}