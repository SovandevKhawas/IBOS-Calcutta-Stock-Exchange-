using MetaDataLibrary.HSN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLibrary.HSN;


namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    
    public class HSNController : Controller
    {
        private readonly IHSN _ihsn;

        public HSNController(IHSN ihsn)
        {
            _ihsn = ihsn;
        }

        [Route("DisplayHSN")]
        [Authorize(Policy = "HSNViewPolicy")]
        public ActionResult DisplayHSN()
        {
            List<HSNMaster> hsns = _ihsn.GetHSNs();
            return View(hsns);
        } // DisplayHSN...

        [Route("AddEditHSN")]
        [MainProject.Filters.GetHSN]
        [Authorize(Policy = "HSNAllPolicy")]
        public IActionResult AddEditHSN(int id)
        {
            var master = new HSNMaster();
            if (id == 0)
            {
                master.IdNo = id;
            }
            else
            {
                master = _ihsn.GetHSNs().FirstOrDefault(m => m.IdNo == id);
            }

            return View(master);
        } // AddEditHSN...

        [Route("AddEditHSN")]
        [MainProject.Filters.GetHSN]
        [Authorize(Policy = "HSNAllPolicy")]
        [HttpPost]
        public IActionResult AddEditHSN(IFormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["Back"]))
            {
                return RedirectToAction("DisplayHSN");
            } // for back...

            var master = new HSNMaster();
            TryUpdateModelAsync(master);
            if (ModelState.IsValid)
            {
                //_ihsn = new DALClass();
                string msg = _ihsn.SaveHSNMaster(master);
                if (msg == "Success")
                {
                    return RedirectToAction("DisplayHSN");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, msg);
                    return View(master);
                }
            } // if valid...

            return View(master);
        } // AddEditHSN...
    } // class...
}