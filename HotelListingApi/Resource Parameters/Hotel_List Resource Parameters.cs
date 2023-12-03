namespace HotelListingApi.HotelListResourceParameters
{
    public class Hotel_List_Resource_Parameters
    {
        const int MaxPageSize = 20;
        public string? name { get; set; }
        public string? SearchQuery { get; set; }

        public int PageNumber = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
