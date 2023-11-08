namespace HotelListingApi.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;

        public IEnumerable<Hotel> Hotels { get; set; } = new List<Hotel>();

    }
}
