using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.Models.AuthModels
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }

    }
}
