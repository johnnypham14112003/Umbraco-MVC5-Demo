using ExampleUmbraco.App_Start;
using Repository.Models;
using Repository.Repository;
using Service.Interfaces;
using Service.Services;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace ExampleUmbraco.Controllers
{
    public class ShopPageController : RenderMvcController
    {
        private readonly IMotorService _motorService;

        public ShopPageController()
        {
            _motorService = AppServiceLocator.GetMotorService();
        }

        public override ActionResult Index(RenderModel model)
        {
            var motorList = _motorService.GetAll();
            ViewBag.MotorList = motorList;

            return CurrentTemplate(model);
        }
    }
}