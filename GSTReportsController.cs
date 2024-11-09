using MetaDataLibrary.BillingDetails;
using MetaDataLibrary.GSTReports;
using MetaDataLibrary.SB;
using MetaDataLibrary.SR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLibrary.GSTReports;
using RepositoryLibrary.PatientBilling;

namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    
    public class GSTReportsController : Controller
    {
        private readonly IGSTReports _igst;
        private readonly IPharmacyBilling ibilling;

        public GSTReportsController(IGSTReports igst, IPharmacyBilling ibilling)
        {
            _igst = igst;
            this.ibilling = ibilling;
        } // constructor...

        [Route("MonthlyGSTIN")]
        [Authorize(Policy = "GSTINMonthWiseAllPolicy")]
        public IActionResult MonthlyGSTIN()
        {
            List<GSTINOutReport> gstreport = new();
            return View(gstreport); // MonthlyGSTIN...
        }

        [Route("MonthlyGSTIN")]
        [HttpPost]
        [Authorize(Policy = "GSTINMonthWiseAllPolicy")]
        public IActionResult MonthlyGSTIN(string month)
        {
            List<GSTINOutReport> gstreport = new();
            if (month == null)
            {
                return View(gstreport);
            }
            string fin_year = HttpContext.Session.GetString("FinYear")!;
            gstreport = _igst.GetGstIn(month, fin_year);
            return View(gstreport); // MonthlyGSTIN...
        }

        [Route("MonthlyGSTOUT")]
        [Authorize(Policy = "GSTOutMonthWiseAllPolicy")]
        public IActionResult MonthlyGSTOUT()
        {
            List<GSTINOutReport> gstreport = new();
            return View(gstreport); // MonthlyGSTOUT...
        }

        [Route("MonthlyGSTOUT")]
        [HttpPost]
        [Authorize(Policy = "GSTOutMonthWiseAllPolicy")]
        public IActionResult MonthlyGSTOUT(string month)
        {
            List<GSTINOutReport> gstreport = new();
            if (month == null)
            {
                return View(gstreport);
            }
            string fin_year = HttpContext.Session.GetString("FinYear")!;
            gstreport = _igst.GetGstOut(month, fin_year);
            return View(gstreport); // MonthlyGSTOUT...
        }

        [Route("BillingDetails")]
        [HttpGet]
        [Authorize(Policy = "PharmacyBillingDetailsAllPolicy")]
        public IActionResult PatientBillingDetails()
        {
            BillingDetailsClass models = new();
            List<BillingDetailsModel> billingDetails = new();
            models.BillingModel=billingDetails;
            return View(models);
        } // PatientBillingDetails...

        [Route("BillingDetails")]
        [HttpPost]
        [Authorize(Policy = "PharmacyBillingDetailsAllPolicy")]
        public IActionResult PatientBillingDetails(string RegNo,string FromDate,string ToDate)
        {
 
            BillingDetailsClass model = _igst.GetPatientBillingDetails(RegNo, FromDate, ToDate);
            return View(model);
        } // PatientBillingDetails...

        [Route("GetBillDetailsByVoucherNo")]
        [HttpGet]
        public IActionResult GetBillDetailsByVoucherNo(string VoucherNo)
        {
            ViewBag.VoucherNo = VoucherNo;
            return View();
        } // PatientBillingDetails...

        [Route("AdvanceGSTOUT")]
        [Authorize(Policy = "GSTOutSalesBnDateRangesAllPolicy")]
        public IActionResult AdvanceGSTOUT() => View(new List<GSTINOutReport>());


        [HttpPost,Route("AdvanceGSTOUT")]
        [Authorize(Policy = "GSTOutSalesBnDateRangesAllPolicy")]
        public IActionResult AdvanceGSTOUT(string FromDate,string ToDate, string Type)
        {
            List<GSTINOutReport> gstreport = new List<GSTINOutReport>();
            GSTINOutReport inoutFooterSum = new GSTINOutReport();
            string fin_year = HttpContext.Session.GetString("FinYear")!;
            
            if (Type != "DischargeIPD")
            {
                gstreport = _igst.GetAdvGstOut(FromDate, ToDate, Type);
                inoutFooterSum = _igst.GetGstAdvReportFooterSum(FromDate, ToDate, Type);
            }
            else
            {
                gstreport = _igst.GetAdvGstOut(FromDate, ToDate);
                inoutFooterSum = _igst.GetFooterSumForDischargedPatients(FromDate, ToDate);
             }

            ViewBag.TotalTaxableAmount = inoutFooterSum.TaxableAmount;
            ViewBag.TotalMRPAmount = inoutFooterSum.MRPAmount;
            ViewBag.TotalPayableAmount = inoutFooterSum.PayableAmount;
            ViewBag.TotalCGSTAmount = inoutFooterSum.CGSTAmount;
            ViewBag.TotalSGSTAmount = inoutFooterSum.SGSTAmount;
            //ViewBag.TotalGst = (inoutFooterSum.CGSTAmount + inoutFooterSum.SGSTAmount);
            ViewBag.TotalGst = inoutFooterSum.TotalGSTAmount;

            return View(gstreport);
        } // AdvanceGSTOUT...

        [Route("GetSBDtlsByBillNo")]
        public JsonResult GetSBDtlsByBillNo(string doc_no)
        {
            List<SBDtl> dtls = _igst.GetSbDtls(doc_no);
            return Json(dtls);
        } // GetSBDtlsByBillNo...

        [Route("GetSRDtlsByReturnNo")]
        public JsonResult GetSRDtlsByReturnNo(string doc_no)
        {
            List<SRDtl> dtls = _igst.GetSrDtls(doc_no);
            return Json(dtls);
        } // GetSRDtlsByReturnNo...
    } // class...
}
