using MetaDataLibrary.CashBank;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLibrary.CashBank;

namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]

    public class CashBankController : Controller
    {
        ICashBank _icb;
        decimal TotalDr = 0, TotalCr = 0, TotalAmount;
        string DrCrAmount = "";

        public CashBankController(ICashBank icb)
        {
            _icb = icb;
        } // constructor...

        [Route("DiplayCollection")]
        [Authorize(Policy = "CashBankBookAllPolicy")]
        public ActionResult DiplayCollection()
        {

            //_icb = new DALClass();
            DateTime fromdate = DateTime.Now;
            string FromDate = string.Format(fromdate.ToString("dd/MM/yyyy"), "{0:d}", new System.Globalization.CultureInfo("en-GB"));
            DateTime todate = DateTime.Now;
            string ToDate = string.Format(todate.ToString("dd/MM/yyyy"), "{0:d}", new System.Globalization.CultureInfo("en-GB"));

            List<CollectionModel> collections = new List<CollectionModel>();
            collections = _icb.GetCollections(FromDate, ToDate);

            foreach (var item in collections)
            {
                if (item.DR_CR == "Debit")
                {
                    TotalDr = TotalDr + item.Debit;
                }
                else
                {
                    TotalCr = TotalCr + item.Credit;
                }
            }
            if (TotalDr > TotalCr)
            {
                TotalAmount = TotalDr - TotalCr;
                DrCrAmount = TotalAmount.ToString() + " (DR)";
            }
            if (TotalCr > TotalDr)
            {
                TotalAmount = TotalCr - TotalDr;
                DrCrAmount = TotalAmount.ToString() + " (CR)";
            }
            ViewBag.Totalbalance = DrCrAmount;

            return View(collections);
        }
        [Route("DiplayCollection")]
        [HttpPost]
        [Authorize(Policy = "CashBankBookAllPolicy")]
        public ActionResult DiplayCollection(string FromDate, string ToDate)
        {
            //_icb = new DALClass();
            List<CollectionModel> collections = new List<CollectionModel>();
            collections = _icb.GetCollections(FromDate, ToDate);
            foreach (var item in collections)
            {
                if (item.DR_CR == "Debit")
                {
                    TotalDr = TotalDr + item.Debit;
                }
                else
                {
                    TotalCr = TotalCr + item.Credit;
                }
            }
            if (TotalDr > TotalCr)
            {
                TotalAmount = TotalDr - TotalCr;
                DrCrAmount = TotalAmount.ToString() + " (DR)";
            }
            else
            {
                TotalAmount = TotalCr - TotalDr;
                DrCrAmount = TotalAmount.ToString() + " (CR)";
            }
            ViewBag.Totalbalance = DrCrAmount;
            return View(collections);
        }

    }
}