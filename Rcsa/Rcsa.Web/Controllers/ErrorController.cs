using System;
using System.Web.Mvc;
using Rcsa.Web.Models;

namespace Rcsa.Web.Controllers
{
  public class ErrorController : Controller
  {
    public ActionResult Index(int statusCode, Exception exception)
    {
      var model = new ErrorModel { HttpStatusCode = statusCode, Exception = exception };
      Response.StatusCode = statusCode;

      return View(model);
    }
  }
}
