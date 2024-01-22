using BEPZA_MEDICAL.Web.Areas.PRM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Web.Areas.PRM.Controllers
{
    public class ExportToExcelController : Controller
    {
        // GET: PRM/ExportToExcel
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ExportToExcel(QueryAnalyzerViewModel model, int? page)
        {
            HttpResponse httpResp = System.Web.HttpContext.Current.Response;
            try
            {
                Response.Clear();
                Response.Buffer = true;              
                Response.AddHeader("Content-Disposition", "attachment; filename=Report.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                Response.Write("<html>");
                Response.Write("<head>");
                Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html;charset=windows-1252\">");
                Response.Write("<!--[if gte mso 9]>");
                Response.Write("<xml>");
                Response.Write("<x:ExcelWorkbook>");
                Response.Write("<x:ExcelWorksheets>");
                Response.Write("<x:ExcelWorksheet>");
                //this line names the worksheet
                Response.Write("<x:Name>Output</x:Name>");
                Response.Write("<x:WorksheetOptions>");
                //these 2 lines are what works the magic
                Response.Write("<x:Panes>");
                Response.Write("</x:Panes>");
                Response.Write("<x:Print>");
                Response.Write("<x:Gridlines />");
                Response.Write("</x:Print>");
                Response.Write("</x:WorksheetOptions>");
                Response.Write("</x:ExcelWorksheet>");
                Response.Write("</x:ExcelWorksheets>");
                Response.Write("</x:ExcelWorkbook>");
                Response.Write("</xml>");
                Response.Write("<![endif]-->");
                Response.Write("</head>");
                Response.Write("<body>");


                string strRptHTMLData = string.Empty;
                strRptHTMLData = model.strHtml;
                if (strRptHTMLData.ToString().Length > 0)
                {
                    Response.Write(strRptHTMLData);
                    Response.Write("</body>");
                    Response.Write("</html>");
                    Response.End();
                }
                else
                {
                    //model.Message = "Data not found to preview report.";
                    Response.Clear();
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            { }
            //return View();
            //return Index();
            return View();
        }


        public JsonResult GetReportData(QueryAnalyzerViewModel model, int? page)
        {
            string strRptHTMLData = string.Empty;
            strRptHTMLData = GetData(model, page);

            return Json(strRptHTMLData);
        }

        private string GetData(QueryAnalyzerViewModel model, int? page)
        {
            string strRptHTMLData = string.Empty;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            try
            {


            }
            catch (Exception ex)
            { }
            return strRptHTMLData;
        }
    }
}