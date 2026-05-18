using Service.ViewModels;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace ExampleUmbraco.Controllers
{
    public class AuthSurfaceController : SurfaceController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleLogin(AuthViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            // Use helper of Umbraco
            if (Members.Login(model.Email, model.Password))
            {
                return RedirectToCurrentUmbracoPage();
            }

            // Case login failed
            ModelState.AddModelError("authError", "Username or password is incorrect!");
            return CurrentUmbracoPage();
        }

        [HttpGet]
        public ActionResult HandleLogout()
        {
            Members.Logout();
            return Redirect("/");
        }
    }
}