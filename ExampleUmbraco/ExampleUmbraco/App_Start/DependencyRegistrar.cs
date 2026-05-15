using LightInject;
using Umbraco.Core;
using Umbraco.Web;

namespace ExampleUmbraco.App_Start
{
    public class DependencyRegistrar : ApplicationEventHandler
    {
        protected override void ApplicationStarted(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
            // 1. Khởi tạo AutoMapper
            AutoMapperConfig.Configure();

            // 2. Lấy LightInject container của Umbraco
            var container = (IServiceContainer)DependencyResolver.Current
                                .GetService(typeof(IServiceContainer));

            if (container == null) return;

            // 3. Đăng ký DbContext - PerRequest để tránh conflict
            container.Register<AppDbContext>(new PerRequestLifeTime());

            // 4. Đăng ký Repository
            container.Register<IProductRepository, ProductRepository>(
                new PerRequestLifeTime());

            // 5. Đăng ký Service
            container.Register<IProductService, ProductService>(
                new PerRequestLifeTime());
        }
    }
}