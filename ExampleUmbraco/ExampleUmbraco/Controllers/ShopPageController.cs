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
            // ExampleDbContext sẽ tự động đọc chuỗi "ExampleDb" trong Web.config
            var dbContext = new ExampleDbContext();

            var motorRepository = new MotorRepository(dbContext);

            // (Nếu dùng AutoMapper 3.3.1)
            var mapper = AutoMapper.Mapper.Engine;

            _motorService = new MotorService(motorRepository, mapper);
        }

        public override ActionResult Index(RenderModel model)
        {
                var motorList = _motorService.GetAll();
                ViewBag.MotorList = motorList;

                return CurrentTemplate(model);
        }
    }
}