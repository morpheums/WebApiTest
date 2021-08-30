using AutoMapper;
using WebApiTest.Data.Entities;
using WebApiTest.Logic.Models.User;

namespace WebApiTest.Logic.Configuration
{
    public class AutoMappperProfile : Profile
	{
		public AutoMappperProfile()
		{
			CreateMap<User, UserDto>().ReverseMap();
			CreateMap<User, InsertUserDto>().ReverseMap();
			CreateMap<Address, UserAddressDto>().ReverseMap();
		}
	}
}
