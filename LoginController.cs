using MainProject.Filters;
using MetaDataLibrary.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLibrary.Common;
using RepositoryLibrary.Login;
using System.Security.Claims;

namespace MainProject.Areas.LogIn.Controllers
{
    [Area("Login")]
    [Authorize]
    public class LoginController : Controller
    {
        private readonly ILogin _ilogin;
        private readonly ICommon _icommon;

        public LoginController(ILogin ilogin, ICommon icommon)
        {
            _ilogin = ilogin;
            _icommon = icommon;
        } // construction...

        [Route("Start")]
        [ActionName("Start")]
        [GetFinYears]
        [AllowAnonymous]
        public IActionResult StartGet()
        {
            MetaDataLibrary.Login.User user = new MetaDataLibrary.Login.User();
            user.UserName = string.Empty;
            user.Password = string.Empty;
            user.FinYear = _icommon.GetDefaultFinancialYear();

            return View(user);
        } // Default Financial Year...

        [Route("Start")]
        [ActionName("Start")]
        [GetFinYears]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> StartPost()
        {
            var user = new MetaDataLibrary.Login.User();
            await TryUpdateModelAsync(user);
            if (ModelState.IsValid)
            {
                user = _ilogin.GetLogin(user.UserName, user.Password, user.FinYear!);
                if (user.UserName == "0")
                {
                    ModelState.AddModelError(string.Empty, "Invalid credentials. Login denied.");
                    return View();
                }
                else
                {
                    HttpContext.Session.SetString("FinYear", user.FinYear!);
                    HttpContext.Session.SetString("FinYearFrom", user.FinYearFrom!);
                    HttpContext.Session.SetString("FinYearTo", user.FinYearTo!);

                    List<UserModuleMapping> mappings = _ilogin.GetUserModuleMappings().Where(m => m.UserName == user.UserName && 
                            m.PermissionType != "None").ToList();

                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.UserName),
                        new Claim(ClaimTypes.Email,"company email"),
                    };
                    if(user.IsAdmin == "Yes")
                    {
                        claims.Add(new Claim("IsAdmin", "Yes"));
                    }
                    foreach(var mapping in mappings)
                    {
                        if (mapping.PermissionType == "All")
                        {
                            claims.Add(new Claim(mapping.ModuleName!, "View Only"));
                            claims.Add(new Claim(mapping.ModuleName!, mapping.PermissionType!));
                        }
                        else
                        {
                            claims.Add(new Claim(mapping.ModuleName!, mapping.PermissionType!));
                        }
                    } // end of foreach loop...

                    ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = false
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity), properties);
                    return RedirectToAction("Dashboard");
                }
            } // if valid...
            return View();
        } // Start...

        [Route("CreateNewUser")]
        [AllowAnonymous]
        public IActionResult CreateNewUser()
        {
            return View();
        } // CreateNewUser...

        [Route("CreateNewUser")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateNewUser(IFormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["Back"]))
            {
                return RedirectToAction("Start");
            } // for back button...

            UserValidations user = new UserValidations();
            await TryUpdateModelAsync(user);
            if(ModelState.IsValid)
            {
                if(_ilogin.CheckUserExists(user.UserName))
                {
                    string message = string.Empty;
                    message = _ilogin.CreateNewUser(user);
                    if (message == "Success")
                    {
                        ModelState.AddModelError(string.Empty, "User Created Successfully!!");
                        return View();
                    }
                    ModelState.AddModelError(string.Empty, message);
                    return View();
                } // if success...

                ModelState.AddModelError(string.Empty, "Username " + user.UserName + " already exists!!");
                return View();
            } // if valid...

            return View();
        } // CreateNewUser...


        [Route("dashboard")]
        public ActionResult Dashboard()
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
            ViewBag.Name = claimsUser.Claims.FirstOrDefault()!.Value.ToString();
            return View();
        } // Dashboard...

        [Route("ChangePassword")]
        [ActionName("ChangePassword")]
        public IActionResult ChangePasswordGet()
        {
            ChangePassword cp = new ChangePassword();
            cp.UserName = HttpContext.User!.Claims.FirstOrDefault()!.Value;
            return View(cp);
        } // ChangePassword...

        [Route("ChangePassword")]
        [ActionName("ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePasswordPost()
        {
            string UserName = HttpContext.User!.Claims.FirstOrDefault()!.Value;
            ChangePassword cp = new ChangePassword();
            await TryUpdateModelAsync(cp);
            if(ModelState.IsValid)
            {
                string password = _ilogin.GetUsers().Where(m => m.UserName == UserName).FirstOrDefault()!.Password;
                if(password != cp.CurrentPassword)
                {
                    ModelState.AddModelError(string.Empty, "Invalid current password!!");
                    return View(cp);
                }

                string msg = _ilogin.UpdatePassword(cp);
                if(msg == "Success")
                {
                    return RedirectToAction("Dashboard");
                }
                ModelState.AddModelError(string.Empty, msg);
                return View(cp);
            } // if valid...

            cp.UserName = UserName;
            return View(cp);
        } // ChangePassword...

        [Route("_AccessDenied")]
        public ActionResult _AccessDenied()
        {
            return View();
        } // _AccessDenied...

        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Start");
        } // Logout...
    }
}
