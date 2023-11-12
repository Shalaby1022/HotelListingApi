using HotelListingApi.DTOs.AuthDTOs;
using HotelListingApi.DTOs.Authentication;
using HotelListingApi.Models;

namespace HotelListingApi.Data.Interfaces
{
    public interface IAuthRepository
    {
        Task<Auth> RegisterAsync(RegisterDto registerDto);
        Task<Auth> GetLoginAsync(LoginDto loginDto);

        Task<string> AddRoleAsync(AddRoleDto roleDto);



    }
}
