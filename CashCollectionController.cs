using MetaDataLibrary.R_CashCollection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLibrary.R_CashCollection;

namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]

    public class CashCollectionController : Controller
    {
        private readonly ICashCollection _icc;
        decimal TotalRcv = 0, TotalPay = 0, TotalAmount;
        string RcvPayAmount = "";

        public CashCollectionController(ICashCollection icc)
        {
            _icc = icc;
        }

        [Route("DisplayCashCollection")]
        [Authorize(Policy = "DailyCashInflowOutflowAllPolicy")]
        public ActionResult DisplayCashCollection()
        {
            DateTime fromdate = DateTime.Now;
            string FromDate = string.Format(fromdate.ToString("dd/MM/yyyy"), "{0:d}", new System.Globalization.CultureInfo("en-GB"));
            DateTime todate = DateTime.Now;
            string ToDate = string.Format(todate.ToString("dd/MM/yyyy"), "{0:d}", new System.Globalization.CultureInfo("en-GB"));

            var rms = new List<ReceivableModel>();
            var pms = new List<PayableModel>();
            var ccm = new CashCollectionModel
            {
                Rcv = rms,
                Pay = pms,
                CategoryDebitCreditModels = null
            };

            foreach (var item in ccm.Rcv)
            {
                TotalRcv += item.Debit;
            }

            foreach (var item in ccm.Pay)
            {
                TotalPay += item.Credit;
            }

            if (TotalRcv > TotalPay)
            {
                TotalAmount = TotalRcv - TotalPay;
                RcvPayAmount = TotalAmount.ToString() + "(Debit)";
            }
            else
            {
                TotalAmount = TotalPay - TotalRcv;
                RcvPayAmount = TotalAmount.ToString() + "(Credit)";
            }

            ViewBag.Totalbalance = RcvPayAmount;
            return View(ccm);
        }

        [HttpPost]
        [Route("DisplayCashCollection")]
        [Authorize(Policy = "DailyCashInflowOutflowAllPolicy")]
        public ActionResult DisplayCashCollection(string FromDate, string ToDate)
        {
            var ccm = new CashCollectionModel
            {
                Rcv = _icc.GetCashCollectionReceivable(FromDate, ToDate),
                Pay = _icc.GetCashCollectionPayable(FromDate, ToDate),
                CategoryDebitCreditModels = _icc.GetCategoryWiseCashBank(FromDate, ToDate)
            };

            foreach (var item in ccm.Rcv)
            {
                TotalRcv += item.Debit;
            }

            foreach (var item in ccm.Pay)
            {
                TotalPay += item.Credit;
            }

            if (TotalRcv > TotalPay)
            {
                TotalAmount = TotalRcv - TotalPay;
                RcvPayAmount = TotalAmount.ToString() + "(Debit)";
            }
            else
            {
                TotalAmount = TotalPay - TotalRcv;
                RcvPayAmount = TotalAmount.ToString() + "(Credit)";
            }

            ViewBag.Totalbalance = RcvPayAmount;
            ViewBag.fromdate = FromDate;
            ViewBag.todate = ToDate;
            return View(ccm);
        }

        [Route("DisplayAdjustedInflowOutflow")]
        [Authorize(Policy = "AdjustedCashInflowOutflowAllPolicy")]
        public ActionResult DisplayAdjustedInflowOutflow()
        {
            DateTime fromdate = DateTime.Now;
            string FromDate = string.Format(fromdate.ToString("dd/MM/yyyy"), "{0:d}", new System.Globalization.CultureInfo("en-GB"));
            DateTime todate = DateTime.Now;
            string ToDate = string.Format(todate.ToString("dd/MM/yyyy"), "{0:d}", new System.Globalization.CultureInfo("en-GB"));

            var rms = new List<ReceivableModel>();
            var pms = new List<PayableModel>();
            var ccm = new CashCollectionModel
            {
                Rcv = rms,
                Pay = pms,
                CategoryDebitCreditModels = null
            };

            foreach (var item in ccm.Rcv)
            {
                TotalRcv += item.Debit;
            }

            foreach (var item in ccm.Pay)
            {
                TotalPay += item.Credit;
            }

            if (TotalRcv > TotalPay)
            {
                TotalAmount = TotalRcv - TotalPay;
                RcvPayAmount = TotalAmount.ToString() + "(Debit)";
            }
            else
            {
                TotalAmount = TotalPay - TotalRcv;
                RcvPayAmount = TotalAmount.ToString() + "(Credit)";
            }

            ViewBag.Totalbalance = RcvPayAmount;
            return View(ccm);
        } // DisplayAdjustedInflowOutflow...

        [Route("DisplayAdjustedInflowOutflow")]
        [HttpPost]
        [Authorize(Policy = "AdjustedCashInflowOutflowAllPolicy")]
        public ActionResult DisplayAdjustedInflowOutflow(string FromDate, string ToDate)
        {
            var ccm = new CashCollectionModel
            {
                Rcv = _icc.GetDailyAdjustedReceivable(FromDate, ToDate),
                Pay = _icc.GetDailyAdjustedPayable(FromDate, ToDate),
                CategoryDebitCreditModels = null // _icc.GetCategoryWiseCashBank(FromDate, ToDate)
            };

            foreach (var item in ccm.Rcv)
            {
                TotalRcv += item.Debit;
            }

            foreach (var item in ccm.Pay)
            {
                TotalPay += item.Credit;
            }

            if (TotalRcv > TotalPay)
            {
                TotalAmount = TotalRcv - TotalPay;
                RcvPayAmount = TotalAmount.ToString() + "(Debit)";
            }
            else
            {
                TotalAmount = TotalPay - TotalRcv;
                RcvPayAmount = TotalAmount.ToString() + "(Credit)";
            }

            ViewBag.Totalbalance = RcvPayAmount;
            ViewBag.fromdate = FromDate;
            ViewBag.todate = ToDate;
            return View(ccm);
        } // DisplayAdjustedInflowOutflow...
    } // class...
}