using HotelListingApi.Data;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.Models;

namespace HotelListingApi.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IGenericRepository<Country> _countryRepository;
        private IGenericRepository<Hotel> _hotelRepository;
        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IGenericRepository<Country> CountriesRepository => _countryRepository ?? new GenericRepsitory<Country>(_context);    

        public IGenericRepository<Hotel> HotelsRepsitory => _hotelRepository ?? new GenericRepsitory<Hotel>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
