using AutoMapper;
using Bill_system_API.DTOs;
using Bill_system_API.Models;

namespace Bill_system_API.MappinigProfiles
{
    public class BookProfile:Profile
    {
        public BookProfile()
        {
            CreateMap<CreateBookDTO , Book>().ReverseMap();
            CreateMap<UpdateBookDTO , Book>().ReverseMap();
        }
    }
}
