using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rcsa.Web.ReportForms
{
  public partial class BoardReportAll : System.Web.UI.Page
  {
    /// <summary>
    /// This is generic report viewer
    /// This ReportViewer will load report with the report name Session["ReportName"]
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

      try
      {
        bool isValid = true;

        string strReportName = System.Web.HttpContext.Current.Session["ReportName"].ToString();
        
        var rptSource = System.Web.HttpContext.Current.Session["rptSource"];
        if (string.IsNullOrEmpty(strReportName))
        {
          isValid = false;
        }


        if (isValid)
        {
          ReportDocument rd = new ReportDocument();
         
          string strRptPath = Server.MapPath("~/") + "Reports\\" + strReportName;
          
          rd.Load(strRptPath);         

          if (rptSource != null && rptSource.GetType().ToString() != "System.String")
            rd.SetDataSource(rptSource);


          CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;          
          CrystalReportViewer1.ReportSource = rd;
          CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
          
        }
        else
        {
          Response.Write("<H2>Nothing Found; No Report name found</H2>");
        }
      }
      catch (Exception ex)
      {
        Response.Write(ex.ToString());
      }
    }   
  }
}