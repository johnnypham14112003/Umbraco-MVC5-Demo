using Repository.Interfaces;
using Repository.Models;
using Repository.Repository;
using Service.Interfaces;
using Service.Services;
using System.Web;

namespace ExampleUmbraco.App_Start
{
    public static class AppServiceLocator
    {
        /* PROPER DI + PER-REQUEST SCOPING */
        /* CREATE INSTANCE FOR EACH HTTP REQUEST AND REUSE ITS INSTANCE UNTIL FINISH, DISPOSE AFTER FINISH */
        private static T GetOrCreate<T>(string key, System.Func<T> factory)
        {
            if (HttpContext.Current.Items[key] == null)
                HttpContext.Current.Items[key] = factory();

            return (T)HttpContext.Current.Items[key];
        }

        // DbContext: 1 instance for whole request
        public static CustomDbContext GetDbContext()
        {
            return GetOrCreate("di_DbContext", () => new CustomDbContext());
        }

        // Repository: reuse DbContext from current request
        public static IMotorRepository GetMotorRepository()
        {
            return GetOrCreate("di_MotorRepository",
                () => new MotorRepository(GetDbContext()));
        }

        // Service: reuse Repository from current request
        public static IMotorService GetMotorService()
        {
            return GetOrCreate("di_MotorService",
                () => new MotorService(GetMotorRepository()));
        }


        /* TRANSIENT */
        /* CREATE INSTANCE EVERY REQUEST */
        //public static IMotorService GetMotorService()
        //{
        //    var context = new CustomDbContext();
        //    var repository = new MotorRepository(context);
        //    return new MotorService(repository);
        //}
    }
}