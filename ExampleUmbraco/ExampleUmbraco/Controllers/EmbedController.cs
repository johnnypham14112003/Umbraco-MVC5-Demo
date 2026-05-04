using DALClassLibray;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace ExampleUmbraco.Controllers
{
    public class EmbedController : SurfaceController
    {
        private readonly MotorService ms;
        public EmbedController()
        {
            ms = new MotorService();
        }

        [ChildActionOnly]
        public ActionResult RenderFeaturedProducts()
        {
            var data = ms.GetMotors();

            return PartialView("~/Views/Partials/MotorWidget.cshtml", data);
        }
    }
}