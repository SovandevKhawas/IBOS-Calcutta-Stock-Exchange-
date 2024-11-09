using Microsoft.AspNetCore.Mvc;

namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    public class GRNController : Controller
    {
        [Route("DisplayGRN")]
        public ActionResult DisplayGRN()
        {
            return View();
        } // DisplayGRN...
    } // class...
}