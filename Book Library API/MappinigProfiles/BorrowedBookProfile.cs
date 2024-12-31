using AutoMapper;
using Bill_system_API.DTOs;
using Bill_system_API.Models;

namespace Bill_system_API.MappinigProfiles
{
    public class BorrowedBookProfile:Profile
    {
        public BorrowedBookProfile()
        {
            CreateMap<BorrowedBookDTO , BorrowedBook>().ReverseMap();
        }
    }
}
