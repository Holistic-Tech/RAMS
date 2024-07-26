using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rcsa.Web.ReportForms
{
    public partial class CustomizedReport : System.Web.UI.Page
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

                string strReportName = System.Web.HttpContext.Current.Session["ReportName"].ToString();
                Rcsa.Web.Models.RcsaDb db = new Models.RcsaDb();
                var listRiskDetails = db.RiskDetails.Where(x => x.Status == "C").ToList();

                var rptSource = System.Web.HttpContext.Current.Session["rptSource"];

                if (string.IsNullOrEmpty(strReportName))
                {
                    isValid = false;
                }


                if (isValid)
                {
                    ReportDocument rd = new ReportDocument();
                    string strRptPath = Server.MapPath("~/") + "Reports\\" + strReportName;
                    //~/ReportForms/StandardReport.aspx
                    rd.Load(strRptPath);

                    if (rptSource != null && rptSource.GetType().ToString() != "System.String")
                        rd.SetDataSource(rptSource);


                    CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                    int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);
                    CrystalReportViewer1.AllowedExportFormats = exportFormatFlags;

                    //CrystalReportViewer1.Width = 1250;
                    //CrystalReportViewer1.Zoom(100);
                    //CrystalReportViewer1.AutoDataBind = true;
                    //CrystalReportViewer1.BestFitPage = false;
                    //CrystalReportViewer1.PrintMode = CrystalDecisions.Web.PrintMode.Pdf;
                    //CrystalReportViewer1.RefreshReport();
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