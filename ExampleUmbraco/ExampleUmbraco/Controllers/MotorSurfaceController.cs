using ExampleUmbraco.App_Start;
using Service.Interfaces;
using Service.ViewModels;
using System;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace ExampleUmbraco.Controllers
{
    public class MotorSurfaceController : SurfaceController
    {
        private readonly IMotorService _motorService;

        public MotorSurfaceController()
        {
            _motorService = AppServiceLocator.GetMotorService();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMotor(MotorViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                _motorService.Add(model);
            }

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMotor(MotorViewModel model)
        {
            if (ModelState.IsValid)
            {
                _motorService.Update(model);
            }

            return RedirectToCurrentUmbracoPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMotor(Guid id)
        {
            _motorService.Delete(id);
            return RedirectToCurrentUmbracoPage();
        }
    }
}