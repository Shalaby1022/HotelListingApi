using AutoMapper;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.DTOs.CountryDTOs;
using HotelListingApi.DTOs.HotelDTOs;
using HotelListingApi.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewHotel([FromBody] CreateHotelDto createHotelDto)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateHotelDto)}");
                return BadRequest(ModelState);
            }

            try
            {
                if (createHotelDto == null) return BadRequest(ModelState);
                var hotelMap = _mapper.Map<Hotel>(createHotelDto);

                await _unitOfWork.HotelsRepsitory.CreateAsync(hotelMap);
                await _unitOfWork.SaveAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating and adding new Hotel {nameof(createHotelDto)}.");
                return StatusCode(500, "Internal server error");
            }


        }


        [HttpPut("{hotelID:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateExisitingHotel(int hotelID, [FromBody] UpdateHotelDto updateHotelDto)
        {
            if (updateHotelDto == null) return BadRequest(ModelState);

            if (hotelID != updateHotelDto.Id) return BadRequest(ModelState);

            if (!ModelState.IsValid || hotelID < 1)
            {
                _logger.LogError($"Invalid Put attempt in {nameof(UpdateExisitingHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotelRetrieving = await _unitOfWork.HotelsRepsitory.GetAllAsync(q => q.Id == hotelID);

                if ( hotelRetrieving.FirstOrDefault() == null)
                {
                    _logger.LogError($"An error occurred while retreving the original data Coz id isn't matching" +
                                                                        $" existing ones {nameof(UpdateExisitingHotel)}.");
                    return BadRequest("id deosn't match");
                }

                var hotelMap = _mapper.Map<Hotel>(updateHotelDto);
                _unitOfWork?.HotelsRepsitory.UpdateAsync(hotelMap);
                await _unitOfWork.SaveAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating existing Hotel {nameof(UpdateExisitingHotel)}.");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpDelete("{hotelId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotel(int hotelId)
        {
            if (hotelId == null) { return NotFound(); }

            if (!ModelState.IsValid || hotelId < 0)
            {
                _logger.LogError($"Invalid Delete attempt in {nameof(DeleteHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                await _unitOfWork.HotelsRepsitory.GetByIdAsync(q => q.Id == hotelId);

                await _unitOfWork.HotelsRepsitory.DeleteAsync(hotelId);
                await  _unitOfWork?.SaveAsync();
                return NoContent();

            }


            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while Deleting existing Hotel {nameof(DeleteHotel)}.");
                return StatusCode(500, "Internal server error! Please Try again");

            }

        }
    }
}
