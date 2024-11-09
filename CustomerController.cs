using MetaDataLibrary.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLibrary.Customer;

namespace MainProject.Areas.Pharmacy.Controllers;

[Area("Pharmacy")]
public class CustomerController : Controller
{
    private readonly ICustomer _customer;

    public CustomerController(ICustomer customer)
    {
        _customer = customer;
    }

    [Route("DisplayCustomers")]
    [Authorize(Policy = "CustomerViewPolicy")]
    public IActionResult DisplayCustomers() => View(_customer.GetAllCustomer());

    [Route("CustomerMaster")]
    [Authorize(Policy = "CustomerAllPolicy")]
    public IActionResult CustomerMaster(int? Id)
    {
        Id ??= 0;
        var model = (Id != 0) ? _customer.GetCustomerById((int)Id) : new CustomerModel();
        return View(model);
    }

    [HttpPost, Route("CustomerMaster")]
    [Authorize(Policy = "CustomerAllPolicy")]
    public IActionResult CustomerMaster(CustomerModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        else
        {
            var message = _customer.SaveCustomer(model);
            if (message == "Success")
            {
                return RedirectToAction("DisplayCustomers");
            }
            else
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }
        }
    }
} // class...
