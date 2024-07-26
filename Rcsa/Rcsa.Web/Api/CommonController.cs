using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Rcsa.Web.Models;

namespace Rcsa.Web.Api
{
    public class CommonController : ApiController
    {
        // GET api/comman
        public List<int> GetDepartmentsByUserId(int userId, int companyId)
        {
            RcsaDb db = new RcsaDb();
            var list =from x in db.UserDepartments
                          where x.UserId == userId && x.CompanyId == companyId
                          select x.DepartmentId;
            return list.ToList();
        }

        // GET api/comman/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/comman
        public void Post([FromBody]string value)
        {
        }

        // PUT api/comman/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/comman/5
        public void Delete(int id)
        {
        }
    }
}
