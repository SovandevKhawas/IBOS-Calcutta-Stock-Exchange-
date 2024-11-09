using MetaDataLibrary.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLibrary.Account;

namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    
    public class AccountController : Controller
    {
        private readonly IAccount _Acc;

        public AccountController(IAccount acc)
        {
            _Acc = acc;
        }

        [Route("DiplayAccount")]
        [Authorize(Policy = "GeneralLedgerViewPolicy")]
        public ActionResult DiplayAccount()
        {
            List<AccountModel> accounts = _Acc.GetAccounts();
            return View(accounts);
        }

        [Route("AccountMaster")]
        [Authorize(Policy = "GeneralLedgerAllPolicy")]
        [MainProject.Filters.GetAccLedger]
        public IActionResult AccountMaster(int? Id)
        {
            if (Id != 0)
            {
                AccountModel account = _Acc.GetAccount(Id);
                return View(account);
            }
            return View();
        }

        [Route("AccountMaster")]
        [HttpPost]
        [Authorize(Policy = "GeneralLedgerAllPolicy")]
        [Filters.GetAccLedger]
        public IActionResult AccountMaster(IFormCollection collection, AccountModel account)
        {
            string msg;
            if (!string.IsNullOrEmpty(collection["Back"]))
            {
                return RedirectToAction("DiplayAccount");
            }
            if (collection["AccName"] == "")
            {
                return View(account);
            }
            if (collection["Category"] == "")
            {
                return View(account);
            }
            if (collection["Schedule"] == "")
            {
                return View(account);
            }

            if (collection["Category"] == "BA")
            {
                if (collection["Address1"] == "")
                {
                    ModelState.AddModelError(string.Empty, "The Permanant address field is required.");
                    return View(account);
                }
                if (collection["Address2"] == "")
                {
                    ModelState.AddModelError(string.Empty, "The Present address field is required.");
                    return View(account);
                }
                if (collection["State"] == "")
                {
                    ModelState.AddModelError(string.Empty, "The State is required.");
                    return View(account);
                }
                if (collection["City"] == "")
                {
                    ModelState.AddModelError(string.Empty, "The City is required.");
                    return View(account);
                }
                var pin = collection["Pin"];
                if (collection["Pin"] == "0")
                {
                    ModelState.AddModelError(string.Empty, "The Pin is required.");
                    return View(account);
                }
                if (collection["BranchName"] == "")
                {
                    ModelState.AddModelError(string.Empty, "The Branch Name is required.");
                    return View(account);
                }
                if (collection["BranchCode"] == "")
                {
                    ModelState.AddModelError(string.Empty, "The Branch Code is required.");
                    return View(account);
                }
                if (collection["IFSC_Code"] == "")
                {
                    ModelState.AddModelError(string.Empty, "The IFSC Code is required.");
                    return View(account);
                }
                if (collection["AccountNo"] == "")
                {
                    ModelState.AddModelError(string.Empty, "The Account Number is required.");
                    return View(account);
                }
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(collection["Save"]))
                {
                    msg = _Acc.UpSertAccount(account);

                    if (msg == "Insert" || msg == "Update")
                    {
                        return RedirectToAction("DiplayAccount");
                    }
                }
            }
            return View(account);
        }
    }
}