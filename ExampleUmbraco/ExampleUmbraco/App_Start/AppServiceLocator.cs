using Repository.Models;
using Repository.Repository;
using Service.Interfaces;
using Service.Services;

namespace ExampleUmbraco.App_Start
{
    public static class AppServiceLocator
    {
        public static IMotorService GetMotorService()
        {
            var context = new CustomDbContext();
            var repository = new MotorRepository(context);
            return new MotorService(repository);
        }
    }
}