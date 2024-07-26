using Rcsa.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Rcsa.Web.Controllers
{
    public class HeatMapController : Controller
    {
        //
        // GET: /HeatMap/

        public ActionResult Index()
        {
            return View();
        }

        public void Generate(int? CompanyId, int? DepartmentId)
        {
            if (CompanyId == null || CompanyId <= 0)
            {
                RedirectToAction("Index", "Orm");
                return;
            }
            HeatMap hm = new HeatMap(HostingEnvironment.MapPath("~/Content/hmap.xlsx"));
            RcsaDb db = new RcsaDb();
            List<HeatMap.HeatMapEntry> cells = new List<HeatMap.HeatMapEntry>();
            var UserDepartments = db.UserDepartments.Where(x => x.UserId == WebSecurity.CurrentUserId).Select(x => x.DepartmentId).ToList();
            string role = System.Web.Security.Roles.GetRolesForUser().FirstOrDefault();

            var metaAccess = db.DepartmentsMaster.Include("CompaniesMaster").Where(x => x.CompanyId == CompanyId);
            if (DepartmentId != null && DepartmentId > 0)
            {
                metaAccess = metaAccess.Where(x => x.DepartmentId == DepartmentId);
            }
            else
            {
                //this means "All Departments" was chosen, so filter them down to the ones I can see if I'm a Manager or User
                if (role == "Manager"|| role== "User")
                {
                    metaAccess = metaAccess.Where(x => UserDepartments.Contains(x.DepartmentId));
                }
            }

            HeatMap.Meta meta = metaAccess.Select(x => new HeatMap.Meta
            {
                Department = x.DepartmentName,
                CompanyName = x.CompaniesMasters.ComapnyName
            }).FirstOrDefault();

            //fix the Department name if none was selected, since it wouldve defaulted from above
            if (DepartmentId == null) meta.Department = "All Departments";

            var riskAccess = db.RiskDetails.Where(x => x.CompanyID == CompanyId);
            if (DepartmentId != null && DepartmentId > 0)
            {
                riskAccess = riskAccess.Where(x => x.DepartmentID == DepartmentId);
            }
            else
            {
                //same deal: this means "All Departments" was selected, so filter for Manager/User if needed.
                if (role == "Manager" || role == "User")
                {
                    riskAccess = riskAccess.Where(x => UserDepartments.Contains(x.DepartmentID.Value));
                }
            }
            cells = riskAccess.Select(x => new HeatMap.HeatMapEntry
            {
                Likely = (
                    x.SubRisk_Likelyhood == "VH" ? 5 :
                    x.SubRisk_Likelyhood == "H" ? 4 :
                    x.SubRisk_Likelyhood == "M" ? 3 :
                    x.SubRisk_Likelyhood == "L" ? 2 : 1
                ),
                Impact = (
                    x.SubRisk_Impact == "VH" ? 5 :
                    x.SubRisk_Impact == "H" ? 4 :
                    x.SubRisk_Impact == "M" ? 3 :
                    x.SubRisk_Impact == "L" ? 2 : 1
                )
            }).ToList();
            hm.Generate(cells, meta);
            db.Dispose();
            hm.DelieverToReponse(System.Web.HttpContext.Current.Response);
        }

    }
}
