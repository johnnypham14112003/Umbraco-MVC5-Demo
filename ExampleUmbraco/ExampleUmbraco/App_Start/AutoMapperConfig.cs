using AutoMapper;
using Services;

namespace ExampleUmbraco.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            // AutoMapper 3.3.1: dùng Mapper.Initialize
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<MotorProfile>();
            });
        }
    }
}