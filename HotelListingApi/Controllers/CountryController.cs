using AutoMapper;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.DTOs.CountryDTOs;
using HotelListingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingApi.Controllers
{
    [Route("api/country")]
    [ApiController]

    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;


        public CountryController(IUnitOfWork unitOfWork , ILogger<CountryController> logger , IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Country>>> GetAllCountries()
        {
            try
            {
                var countries = await _unitOfWork.CountriesRepository.GetAllAsync();

                var mappedCountries = _mapper.Map<IEnumerable<CountryDto>>(countries);

                return Ok(mappedCountries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong on {nameof(GetAllCountries)}");
                return StatusCode(500, "Internal server error Try Again Later");
            }
        }

        [HttpGet("{countryId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCountryById(int countryId)
        {
            try
            {
                if (countryId == null) return NotFound();

                var country = await _unitOfWork.CountriesRepository.GetByIdAsync(c => c.Id == countryId, new List<string> { "Hotels" });

                if (country == null) return NotFound();

                var mappedCountry = _mapper.Map<CountryDto>(country);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(mappedCountry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing GetCountryById.");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
