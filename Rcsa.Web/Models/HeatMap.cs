using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Rcsa.Web.Models
{
    public class HeatMap
    {
        public class HeatMapEntry
        {
            public int Likely { get; set; }
            public int Impact { get; set; }
        }

        public class Meta
        {
            public string CompanyName { get; set; }
            public string Department { get; set; }
        }

        public XLWorkbook OriginalXLBook { get; set; }
        public XLWorkbook XLBook { get; set; }
        private Meta HMMeta { get; set; }

        private char ImpactColumn { get; set; } = 'H';
        private char LikelyColumn { get; set; } = 'I';

        public HeatMap(string path)
        {
            this.OriginalXLBook = ExcelHandler.LoadExcelFromFS(path);
        }
        
        public void Generate(List<HeatMapEntry> heatMapEntry, HeatMap.Meta meta)
        {
            this.XLBook = this.OriginalXLBook;
            this.HMMeta = meta;
            var row = 1;
            foreach(HeatMapEntry hmEntry in heatMapEntry)
            {
                this.XLBook.Worksheet(2).Cell(String.Format("{0}{1}", ImpactColumn, row)).Value = hmEntry.Impact;
                this.XLBook.Worksheet(2).Cell(String.Format("{0}{1}", LikelyColumn, row)).Value = hmEntry.Likely;
                row++;
            }

            var raw = this.XLBook.Worksheet(1).Cell("B1").Value.ToString();
            StringBuilder builder = new StringBuilder(raw);
            builder.Replace("{{Company Name}}", meta.CompanyName);
            builder.Replace("{{Department}}", meta.Department);
            this.XLBook.Worksheet(1).Cell("B1").Value = builder.ToString(); //Heat Map: {{Company Name}} - {{Department}}
            //this.XLBook.Worksheet(1).Cell("I1").Value = builder.ToString(); //Heat Map: {{Company Name}} - {{Department}}
        }


        public void DelieverToReponse(HttpResponse response)
        {
            // Prepare the response
            HttpResponse httpResponse = response;
            httpResponse.Clear();
            httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            httpResponse.AddHeader("content-disposition", "attachment;filename=\""+ HMMeta.CompanyName + " - " + HMMeta.Department+".xlsx\"");

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                this.XLBook.SaveAs(memoryStream);
                memoryStream.WriteTo(httpResponse.OutputStream);
                memoryStream.Close();
            }

            httpResponse.End();
        }
    }

    public class ExcelHandler
    {

        public static XLWorkbook LoadExcelFromFS(string path)
        {
            XLWorkbook Workbook = new XLWorkbook(path);
            return Workbook;
        }
    }
}