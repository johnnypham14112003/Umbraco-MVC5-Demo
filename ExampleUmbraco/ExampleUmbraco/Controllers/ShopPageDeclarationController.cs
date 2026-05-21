using ExampleUmbraco.App_Start;
using Service.Interfaces;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace ExampleUmbraco.Controllers
{
    // Name must match with the Alias of Document Type
    public class ShopPageDeclarationController : RenderMvcController
    {
        private readonly IMotorService _motorService;

        public ShopPageDeclarationController()
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