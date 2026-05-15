using AutoMapper;
using Services;
using Umbraco.Core;

namespace ExampleUmbraco.App_Start
{
    public class DependencyRegistrar : ApplicationEventHandler
    {
        protected override void ApplicationStarted(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            // 1. Khởi tạo AutoMapper
            Mapper.AddProfile<MotorProfile>();
        }
    }
}