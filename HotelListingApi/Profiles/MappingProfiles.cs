﻿using AutoMapper;
using HotelListingApi.DTOs.AuthDTOs;
using HotelListingApi.DTOs.Authentication;
using HotelListingApi.DTOs.CountryDTOs;
using HotelListingApi.DTOs.HotelDTOs;
using HotelListingApi.Models;
using HotelListingApi.Models.AuthModels;

namespace HotelListingApi.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Country , CountryDto>().ReverseMap();
            CreateMap<Country , CreateCoutnryDto>().ReverseMap();
            CreateMap<Country , UpdateCountryDto>().ReverseMap();



            CreateMap<Hotel , HotelDto>().ReverseMap();
            CreateMap<Hotel , CreateHotelDto>().ReverseMap();
            CreateMap<Hotel, UpdateHotelDto>().ReverseMap();




            CreateMap<AppUser , RegisterDto>().ReverseMap();
            CreateMap<Auth ,  RegisterDto>().ReverseMap();

            CreateMap<AppUser, LoginDto>().ReverseMap();
            CreateMap<Auth, LoginDto>().ReverseMap();


        }

    }
}
