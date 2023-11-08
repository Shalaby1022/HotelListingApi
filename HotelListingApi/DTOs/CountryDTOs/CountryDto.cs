using HotelListingApi.DTOs.HotelDTOs;
using HotelListingApi.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.CountryDTOs
{
    public class CountryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name should be between 2 and 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "ShortName is required.")]
        [StringLength(maximumLength: 5, ErrorMessage = "ShortName should be 5 characters at Max.")]
        public string ShortName { get; set; } = string.Empty;

        public IEnumerable<HotelDto> Hotels { get; set; } = new List<HotelDto>();
    }
}
