using AutoMapper;
using HotelListingApi.DTOs.CountryDTOs;
using HotelListingApi.DTOs.HotelDTOs;
using HotelListingApi.Models;

namespace HotelListingApi.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Country , CountryDto>().ReverseMap();
            CreateMap<Country , CreateCoutnryDto>().ReverseMap();


            CreateMap<Hotel , HotelDto>().ReverseMap();
            CreateMap<Hotel , CreateHotelDto>().ReverseMap();
        }
    }
}
