using MetaDataLibrary.GST_B2C;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLibrary.GST_B2C;


namespace MainProject.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    public class GST_B2CController : Controller
    {
        private readonly IGSTB2C _igstb2c;
        public GST_B2CController(IGSTB2C igstb2c)
        {
            _igstb2c = igstb2c;
        }

        [Route("B2CDisplaySaleReturn")]
        [Authorize(Policy = "GSTOutB2CAllPolicy")]
        public IActionResult B2CDisplaySaleReturn()
        {
            List<ReturnSegregation> lstR = new List<ReturnSegregation>();
            List<SaleSegregation> lstS = new List<SaleSegregation>();
            List<AdjustedReturn> lstD = new List<AdjustedReturn>();
            Total t = new Total();

            Sale_Return sale_Return = new Sale_Return();
            sale_Return.returns = lstR;
            sale_Return.sales = lstS;
            sale_Return.adjreturn = lstD;
            sale_Return.total = t;

            return View(sale_Return);
        }//B2CDisplaySale...

        [HttpPost, Route("B2CDisplaySaleReturn")]
        [Authorize(Policy = "GSTOutB2CAllPolicy")]
        public IActionResult B2CDisplaySaleReturn(IFormCollection collection)
        {
            string FromDate = collection["DateFrom"].ToString();
            string ToDate = collection["DateTo"].ToString();
            ViewData["DateFrom"] = FromDate;
            ViewData["DateTo"] = ToDate;

            Sale_Return sale_Return = new Sale_Return();
            sale_Return = _igstb2c.Populate(FromDate, ToDate);

            return View(sale_Return);
        }//B2CDisplaySale..
    }
}
