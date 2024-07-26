using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;

namespace Rcsa.Web.ReportForms
{
  public partial class StandardReport : System.Web.UI.Page
    {

        /// <summary>
        /// This is generic report viewer
        /// This ReportViewer will load report with the report name Session["ReportName"]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            
        }

        

        private void Page_Init(object sender, EventArgs e)
        {

          ConfigureCrystalReports();

        }

        private void ConfigureCrystalReports()
        {
          try
          {
            bool isValid = true;
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            string strReportName = System.Web.HttpContext.Current.Session["ReportName"].ToString();
            if (System.Web.HttpContext.Current.Session["rptSource"] != null)
            {
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


                CrystalReportViewer1.ReportSource = rd;
                CrystalReportViewer1.RefreshReport();

              }
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
