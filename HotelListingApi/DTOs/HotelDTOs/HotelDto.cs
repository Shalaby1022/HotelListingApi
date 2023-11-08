using HotelListingApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListingApi.DTOs.HotelDTOs
{
    public class HotelDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name should be between 2 and 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Address should be between 10 and 100 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage ="Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public double Rating { get; set; }

        [Required]
        public int CountryId { get; set; }
    }
}
