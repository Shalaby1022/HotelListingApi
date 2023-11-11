using AutoMapper;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.DTOs.AuthDTOs;
using HotelListingApi.DTOs.Authentication;
using HotelListingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace HotelListingApi.Controllers
{
    [Route("api/auth")]
    [ApiController]

    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepository , IMapper mapper)
        {
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Auth))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> RegiterNewUser (RegisterDto registerDto)
        {
            if (registerDto == null) return BadRequest(ModelState);

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var result = await _authRepository.RegisterAsync(registerDto);

            var mappedNewUser = _mapper.Map<Auth>(result);

            if (!mappedNewUser.IsAuthenticated)
            {
                return BadRequest(mappedNewUser.Message);
            }

            return Ok("Successfully Added");
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Auth))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetLogin(LoginDto loginDto)
        {
            if (loginDto == null) return BadRequest(ModelState);


            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var result = await _authRepository.GetLoginAsync(loginDto);

            var mappedResult = _mapper.Map<Auth>(result);

            if (!mappedResult.IsAuthenticated)
            {
                return BadRequest(mappedResult.Message);
            }

            return Ok("Successfully Loged");
        }





    }
    }

