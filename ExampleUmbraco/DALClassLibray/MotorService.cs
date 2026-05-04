using System.Collections.Generic;

namespace DALClassLibray
{
    //public interface IMotorService.
    public class MotorService
    {
        public static IEnumerable<Motor> list = new List<Motor>
        {
            new Motor
                {
                    Name = "Kawasaki",
                    Price = 150,
                    Description = "Kawasaki is beautiful"
                },
                new Motor
                {
                    Name = "Honda",
                    Price = 155,
                    Description = "Honda is tough"
                }
        };

        public IEnumerable<Motor> GetMotors()
        {
            return list;
        }
    }
}