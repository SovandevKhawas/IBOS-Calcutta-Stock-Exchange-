using MetaDataLibrary.GSTOUT_RETURN;
using MetaDataLibrary.GSTReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLibrary.GSTOUT_RETURN;
using RepositoryLibrary.GSTReports;
using RepositoryLibrary.PatientBilling;

namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    
    public class GSTOutReturnController : Controller
    {
        private readonly IGSTReturn _igstret;      
        public GSTOutReturnController(IGSTReturn igstret)
        {
            _igstret = igstret;
            
        } // constructor,,,

        [Route("GetGstOutputReturn")]
        [Authorize(Policy = "GSTOutRetBnDateRangesAllPolicy")]
        public IActionResult GetGstOutputReturn()
        {
            return View(new List<GSTOUTReturn>());
        }

        [HttpPost,Route("GetGstOutputReturn")]
        [Authorize(Policy = "GSTOutRetBnDateRangesAllPolicy")]
        public IActionResult GetGstOutputReturn(IFormCollection collection, string Type)
        {
            string FromDate = collection["DateFrom"].ToString();
            string ToDate = collection["DateTo"].ToString();
            ViewData["DateFrom"] = FromDate;
            ViewData["DateTo"] = ToDate;
            List<GSTOUTReturn> gstreport = new List<GSTOUTReturn>();
            GSTOUTReturn returnFooterSum = new GSTOUTReturn();
            string fin_year = HttpContext.Session.GetString("FinYear")!;

            if (Type != "DischargeIPD")
            {
                gstreport = _igstret.GetGstOutReturn(FromDate, ToDate, Type);
                returnFooterSum = _igstret.GetGstOutReturnFooterSum(FromDate, ToDate, Type);
            }
            else
            {
                gstreport = _igstret.GetGstOutReturn(FromDate, ToDate);
                returnFooterSum = _igstret.GetFooterSumForDischargedPatients(FromDate, ToDate);
            }

            ViewBag.TotalTaxableAmount = returnFooterSum.TaxableAmount;
            ViewBag.TotalMRPAmount = returnFooterSum.MRPAmount;
            ViewBag.TotalPayableAmount = returnFooterSum.PayableAmount;
            ViewBag.TotalCGSTAmount = returnFooterSum.CGSTAmount;
            ViewBag.TotalSGSTAmount = returnFooterSum.SGSTAmount;            
            ViewBag.TotalGst = returnFooterSum.TotalGSTAmount;

            return View(gstreport);
        }//GetGstOutputReturn...
    }
}
