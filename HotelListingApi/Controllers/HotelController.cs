using AutoMapper;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.DTOs.CountryDTOs;
using HotelListingApi.DTOs.HotelDTOs;
using HotelListingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelListingApi.Controllers
{
    [Route("api/hotel")]
    [ApiController]

    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;


        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Hotel>))]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetAllHotels()
        {
            try
            {
                var hotels = await _unitOfWork.HotelsRepsitory.GetAllAsync();

                var mappedHotels = _mapper.Map<IEnumerable<HotelDto>>(hotels);

                return Ok(mappedHotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong on {nameof(GetAllHotels)}");
                return StatusCode(500, "Internal server error Try Again Later");
            }
        }


        [HttpGet("{HotelId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetHotelById(int HotelId)
        {
            try
            {
                if (HotelId == null) return NotFound();

                var hotel = await _unitOfWork.HotelsRepsitory.GetByIdAsync(c => c.Id == HotelId, new List<string> { "Country" });

                if (hotel == null) return NotFound();

                var mappedHotel = _mapper.Map<HotelDto>(hotel);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(mappedHotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing GetHotelById.");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
