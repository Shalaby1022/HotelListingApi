using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Models.AuthModels
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        private bool IsExpired => DateTime.UtcNow > ExpiresOn;

        public DateTime CreatedOn { get; set; }
        public DateTime RevokedOn { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired;



    }
}
