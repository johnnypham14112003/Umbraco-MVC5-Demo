using AutoMapper;
using Repository.Models;
using Service.ViewModels;

namespace Services
{
    public class MotorProfile : Profile
    {
        protected override void Configure()
        {
            // Cú pháp: CreateMap<Source, Destination>();
            CreateMap<Motor, MotorViewModel>();

            // Nếu muốn map ngược lại từ DTO sang Entity (để Insert/Update)
            CreateMap<MotorViewModel, Motor>();
        }
    }
}