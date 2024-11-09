using MetaDataLibrary.Accgroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RepositoryLibrary.Accgroup;

namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    
    public class AccgroupController : Controller
    {
        private readonly IAccgroup _accgroup;

        public AccgroupController(IAccgroup accgroup)
        {
            _accgroup = accgroup;
        }

        [Route("DisplayAccgroup")]
        [Authorize(Policy = "AccountsGroupViewPolicy")]
        public IActionResult DisplayAccgroup()
        {
            var Accgroups = _accgroup.GetAccgroupList();
            return View(Accgroups);
        }

        [Route("AccgroupMaster")]
        [Authorize(Policy = "AccountsGroupAllPolicy")]
        public IActionResult AccgroupMaster(int? Id)
        {
            //IACC = new DALClassAccgroup();
            var accgrp = _accgroup.GetAccgroupDropdown().ToList();
            var listItem = new SelectList(accgrp, "Groupcode", "Groupname");
            ViewBag.AccgroupDropdown = listItem;

            if (Id != 0)
            {
                var grp = _accgroup.GetAccgroupById(Id);
                return View(grp);
            }
            return View();
        }

        [Route("AccgroupMaster")]
        [HttpPost]
        [Authorize(Policy = "AccountsGroupAllPolicy")]
        public IActionResult AccgroupMaster(IFormCollection formCollection, AccgroupModel accgroup)
        {
            string msg;
            //IACC = new DALClassAccgroup();
            if (!string.IsNullOrEmpty(formCollection["Back"]))
            {
                return RedirectToAction("DisplayAccgroup");
            }
            if (ModelState.IsValid)
            {
                msg = _accgroup.SaveAccgroup(accgroup);
                if (msg == "insert" || msg == "update")
                {
                    return RedirectToAction("DisplayAccgroup");
                }
            }
            var grp = _accgroup.GetAccgroupDropdown().ToList();
            var listItem = new SelectList(grp, "Groupcode", "Groupname");
            ViewBag.AccgroupDropdown = listItem;
            return View(accgroup);
        }
    }
}
