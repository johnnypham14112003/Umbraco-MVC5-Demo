using DALClassLibray;
using System.Web.Mvc;

namespace ExampleUmbraco.Controllers
{
    public class AdminController : Controller
    {
        private readonly MotorService ms;
        public AdminController()
        {
            ms = new MotorService();
        }

        // GET: Admin
        public ActionResult Index()
        {
            var data = ms.GetMotors();

            return View(data);
        }
    }
}