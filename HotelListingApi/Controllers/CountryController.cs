using AutoMapper;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.DTOs.CountryDTOs;
using HotelListingApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

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

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewCountry([FromBody] CreateCoutnryDto createCoutnryDto)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateNewCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                if (createCoutnryDto == null) return BadRequest(ModelState);
                var countryMap = _mapper.Map<Country>(createCoutnryDto);

               await _unitOfWork.CountriesRepository.CreateAsync(countryMap);
                await _unitOfWork.SaveAsync();

                return NoContent();

            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, $"An error occurred while creating and adding new country {nameof(CreateNewCountry)}.");
                return StatusCode(500, "Internal server error");
            }


        }

     
        [HttpPut("{countryId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateExistedCountry( int countryId , [FromBody] UpdateCountryDto updateCountryDto)
        {
            if (updateCountryDto == null) return BadRequest(ModelState);

            if (countryId != updateCountryDto.Id) return BadRequest(ModelState);

            if (!ModelState.IsValid || countryId <1 )
            {
                _logger.LogError($"Invalid Put attempt in {nameof(UpdateExistedCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                var countryRetreving = _unitOfWork.CountriesRepository.GetAllAsync(q => q.Id == countryId);

                    if (countryRetreving == null)
                    {
                        _logger.LogError($"An error occurred while retreving the original data Coz id isn't matching" +
                                                                            $" existing ones {nameof(UpdateExistedCountry)}.");
                        return BadRequest("id deosn't match");
                    }
                
                var countryMap = _mapper.Map<Country>(updateCountryDto);
                 _unitOfWork?.CountriesRepository.UpdateAsync(countryMap);
                await _unitOfWork.SaveAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating and adding new country {nameof(UpdateExistedCountry)}.");
                return StatusCode(500, "Internal server error");
            }

        }


    }
}
