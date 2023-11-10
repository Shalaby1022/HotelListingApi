using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LName { get; set; }
    }
}
