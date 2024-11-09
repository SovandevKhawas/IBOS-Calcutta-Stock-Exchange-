using MetaDataLibrary.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using RepositoryLibrary.Login;

namespace MainProject.Areas.LogIn.Controllers
{
    [Area("Login")]

    public class UserPrivilegeController : Controller
    {
        private readonly ILogin ilogin;

        public UserPrivilegeController(ILogin ilogin)
        {
            this.ilogin = ilogin;
        } // constructor...

        [Route("UserPrivilegeModule")]
        [MainProject.Filters.UserPrivilege]
        [Authorize(Policy = "UserPrivilegeAllPolicy")]
        public IActionResult UserPrivilegeModule()
        {
            List<UserModuleMapping> mappings = new List<UserModuleMapping>();
            return View(mappings);
        } // UserPrivilegeModule...

        [Route("UserPrivilegeModule")]
        [HttpPost]
        [MainProject.Filters.UserPrivilege]
        [Authorize(Policy = "UserPrivilegeAllPolicy")]
        public IActionResult UserPrivilegeModule(IFormCollection collection)
        {
            List<UserModuleMapping> mappings = new List<UserModuleMapping>();
            List<UserModuleMapping> mappings1 = new List<UserModuleMapping>();
            List<UserModuleMapping> mappings2 = new List<UserModuleMapping>();
            List<UserModuleMapping> mappings3 = new List<UserModuleMapping>();
            List<UserModuleMapping> mappings4 = new List<UserModuleMapping>();

            if (!string.IsNullOrEmpty(collection["Display"]))
            {
                ViewBag.UserName = collection["Users"].ToString();
                mappings = ilogin.GetUserModuleMappings().Where(m => m.UserName == collection["Users"].ToString()).ToList();
            }
            else
            {
                ViewBag.UserName = collection["UserName"].ToString();
                
                JavaScriptSerializer js = new JavaScriptSerializer();
                mappings1 = js.Deserialize<List<UserModuleMapping>>(collection["JsonText1"].ToString());
                mappings2 = js.Deserialize<List<UserModuleMapping>>(collection["JsonText2"].ToString());
                mappings3 = js.Deserialize<List<UserModuleMapping>>(collection["JsonText3"].ToString());
                mappings4 = js.Deserialize<List<UserModuleMapping>>(collection["JsonText4"].ToString());
                mappings.AddRange(mappings1);
                mappings.AddRange(mappings2);
                mappings.AddRange(mappings3);
                mappings.AddRange(mappings4);

                string message = string.Empty;
                message = ilogin.SaveUserModuleMappings(mappings);
                if(message == "Success")
                {
                    ModelState.AddModelError(string.Empty, "User Privilege updated successfully!!");
                    return RedirectToAction("UserPrivilegeModule");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, message);
                    mappings = ilogin.GetUserModuleMappings().Where(m => m.UserName == ViewBag.UserName).ToList();
                    return View(mappings);
                }
            } // if save button is clicked...

            return View(mappings);
        } // UserPrivilegeModule...
    } // class...
}
