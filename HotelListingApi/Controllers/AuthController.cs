using AutoMapper;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.DTOs.AuthDTOs;
using HotelListingApi.DTOs.Authentication;
using HotelListingApi.Models.AuthModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Security.Cryptography;

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

            if(!string.IsNullOrEmpty(mappedResult.RefreshToken))
            {
                SetRefreshTokenInCookies(mappedResult.RefreshToken, mappedResult.RefreshTokenExpiration);
            }

            return Ok("Successfully Loged");
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleDto roleDto)
        {
            if (roleDto == null) return BadRequest(ModelState);


            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var result = await _authRepository.AddRoleAsync(roleDto);

            if (!string.IsNullOrEmpty(result)) return BadRequest(result);

            return Ok(roleDto);
        }

        private void SetRefreshTokenInCookies(string refreshToken , DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };

            Response.Cookies.Append("refresh Token",refreshToken, cookieOptions);

        }

        [HttpGet]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authRepository.RefreshTOkenAsync(refreshToken);

            if(!result.IsAuthenticated) return BadRequest(result);

            SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpiration);


            return Ok(result);

        }




    }
    }

