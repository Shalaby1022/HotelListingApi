using HotelListingApi.Models;

namespace HotelListingApi.Data.Interfaces
{
public interface IUnitOfWork : IDisposable
{
        IGenericRepository<Country> CountriesRepository { get; }
        IGenericRepository<Hotel> HotelsRepsitory { get; }
        Task SaveAsync();
}
}
