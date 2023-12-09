using HotelListingApi.Models;
using HotelListingApi.Models.AuthModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>()
                .HasData(
                new Country
                {
                    Id = 1,
                    Name = "Egypt",
                    ShortName = "EGY"
                },
                new Country
                {
                    Id = 2,
                    Name = "Jamaica",
                    ShortName = "Jm"
                },
                new Country
                {
                    Id = 3,
                    Name = "Cayman Island",
                    ShortName = "Cy"
                }
                );

                 modelBuilder.Entity<Hotel>()
                .HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Holiday In",
                    Address = "Cairo",
                    CountryId = 1,
                    Rating = 4.5
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Bahamas",
                    Address = "Bs",
                    CountryId = 2,
                    Rating = 4.8
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Grand Ballidum",
                    Address = "GB",
                    CountryId = 3,
                    Rating = 4.2
                }
                );
        }

        }
    }
