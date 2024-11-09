using FluentValidation;
using FluentValidation.Results;
using MainProject.Filters;
using MetaDataLibrary.FINCB;
using MetaDataLibrary.FINCBOutsideCustomer;
using MetaDataLibrary.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using RepositoryLibrary.Common;
using RepositoryLibrary.FINCBOutsideCustomer;
using RepositoryLibrary.Order;
using System.Collections.Generic;

namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]

    public class FinCBOutsideController : Controller
    {
        private readonly IFINCBOutsideCustomer iFinCB;
        private readonly ICommon icommon;
        private readonly IValidator<FinCBHeadOutside> validator;

        public FinCBOutsideController(IFINCBOutsideCustomer iFinCB, ICommon icommon, IValidator<FinCBHeadOutside> validator)
        {
            this.iFinCB = iFinCB;
            this.icommon = icommon;
            this.validator = validator;
        } // constructor...

        [Route("DisplayFinCBOutside")]
        [Authorize(Policy = "ReceiptFromOutsideCustomersViewPolicy")]
        public IActionResult DisplayFinCBOutside(int pg = 1, int pageSize = 5, string SearchText = "")
        {
            ViewBag.SearchText = SearchText;
            IQueryable<FinCBHeadOutside> heads;
            if (string.IsNullOrEmpty(SearchText))
            {
                heads = iFinCB.GetFinCBHeadOutsides().AsQueryable();
            }
            else
            {
                heads = iFinCB.GetFinCBHeadOutsides()
                    .Where(m => m.DocNo!.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    m.DocDate!.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    m.CustomerName!.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    ).AsQueryable();
            }
            return View(icommon.GetGenericPaginationModel<FinCBHeadOutside>(heads, heads.Count(), pg, pageSize));
        } // DisplayFinCBOutside...

        [Route("DisplayFinCBOutside")]
        [HttpPost]
        [Authorize(Policy = "ReceiptFromOutsideCustomersViewPolicy")]
        public ActionResult DisplayFinCBOutside(IFormCollection collection, int pg = 1, int pageSize = 5, string SearchText = "")
        {
            IQueryable<FinCBHeadOutside> heads;
            if (string.IsNullOrEmpty(collection["SearchText"]))
            {
                heads = iFinCB.GetFinCBHeadOutsides().AsQueryable();
            }
            else
            {
                ViewBag.SearchText = collection["SearchText"].ToString();
                heads = iFinCB.GetFinCBHeadOutsides()
                    .Where(m => m.DocNo!.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    m.DocDate!.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    m.CustomerName!.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    ).AsQueryable();
            }
            return View(icommon.GetGenericPaginationModel<FinCBHeadOutside>(heads, heads.Count(), pg, pageSize));
        } // DisplayFinCBOutside...

        [GetOutsideCustomersReceiptActionFilter]
        [Route("FINCBOutsideAddEdit")]
        [Authorize(Policy = "ReceiptFromOutsideCustomersAllPolicy")]
        public IActionResult FINCBOutsideAddEdit(int id)
        {
            var result = id == 0 ? FinCBHeadOutside.SetData() : iFinCB.PopulateDetails(id);
            return View(result);
        } // FINCBOutsideAddEdit...

        [GetOutsideCustomersReceiptActionFilter]
        [Route("FINCBOutsideAddEdit")]
        [Authorize(Policy = "ReceiptFromOutsideCustomersAllPolicy")]
        [HttpPost]
        public async Task<IActionResult> FINCBOutsideAddEdit(IFormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["Back"]))
            {
                return RedirectToAction("DisplayFinCBOutside");
            } // back button...

            List<FinCBAdjustOutsideFetch> results = new List<FinCBAdjustOutsideFetch>();
            List<FinCBAdjustOutside> adjusts = new List<FinCBAdjustOutside>();
            FinCBDtlOutside detail = null!;
            FinCBHeadOutside head = null!;
            int docId = 0;
            string docNo = string.Empty;
            string docDate = string.Empty;
            int ? cbCode = null!;
            int? customerCode = null!;
            decimal Amount = 0;
            int? acCode = null!;
            int recIdDtl = 0;
            string narration = string.Empty;

            docId = Convert.ToInt32(collection["DocId"]);
            docNo = string.IsNullOrEmpty(collection["DocNo"]) ? string.Empty : collection["DocNo"].ToString();
            docDate = string.IsNullOrEmpty(collection["DocDate"]) ? string.Empty : collection["DocDate"].ToString();
            cbCode = string.IsNullOrEmpty(collection["CbCode"]) ? 0 : Convert.ToInt32(collection["CbCode"]);
            customerCode = string.IsNullOrEmpty(collection["CustomerCode"]) ? 0 : Convert.ToInt32(collection["CustomerCode"]);
            Amount = !string.IsNullOrEmpty(collection["Credit"]) ? Convert.ToDecimal(collection["Credit"]) : 0.00m;
            acCode = string.IsNullOrEmpty(collection["ddlAcCode"]) ? 0 : Convert.ToInt32(collection["ddlAcCode"]);
            recIdDtl = string.IsNullOrEmpty(collection["RecIdDtl"]) ? 0 : Convert.ToInt32(collection["RecIdDtl"]);
            narration = string.IsNullOrEmpty(collection["Narration"]) ? string.Empty : collection["Narration"].ToString();

            if (collection["PopulateJson"].ToString() != "-1")
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                results = js.Deserialize<List<FinCBAdjustOutsideFetch>>(collection["PopulateJson"].ToString());
            }

            foreach(var data in results)
            {
                FinCBAdjustOutside adjust = FinCBAdjustOutside.SetData(data.RecId, data.VchId, data.VchNo, data.VchDate, data.VchType, data.VchModuleName,
                            data.NetAmount, data.AmountAdjTillDate, data.AmountPending, data.MaxAmountAdjustable, data.Amount, data.CustomerCode);
                adjusts.Add(adjust);
            } // end of foreach loop...
            detail = FinCBDtlOutside.SetData(recIdDtl, (int)acCode, string.Empty, customerCode, string.Empty, (int)cbCode, "Outstanding Bills/Advances", 
                        "Credit", Amount, Amount, narration, "CASH BANK ENTRY", string.Empty);
            head = FinCBHeadOutside.SetData(docId, docNo, docDate, (int)cbCode, string.Empty, (int)customerCode, string.Empty, narration,
                    HttpContext.User.Claims.FirstOrDefault()!.Value.ToString(), detail, adjusts);

            ValidationResult valResult = await validator.ValidateAsync(head);
            if (valResult.IsValid)
            {
                Task<string> tskMsg = iFinCB.SaveFinCBHeadOutside(head, HttpContext.Session.GetString("FinYear")!);
                if(tskMsg.Result == "Success")
                {
                    return RedirectToAction("DisplayFinCBOutside");
                }
                ModelState.AddModelError(string.Empty, tskMsg.Result);
                return View(head);
            } // if valid...

            valResult.Errors.ForEach(m => ModelState.AddModelError(m.PropertyName, m.ErrorMessage));
            return View(head);
        } // FINCBOutsideAddEdit...

        [Route("GetCBAdjustReceiptsOutsides")]
        public JsonResult GetReceipts(int customer_code)
        {
            List<FinCBAdjustOutside> adjusts = iFinCB.GetFinCBAdjustOutsides(customer_code);
            return Json(adjusts);
        } // GetReceipts...

        //SDK 
        [Route("FinCBPrint")]
        public ActionResult PaymentReceiptReport(int id)
        {
            FINCBCustomerBillings records = iFinCB.FinCBPrint(id);
            return View(records);
        }
    } // class...
}
