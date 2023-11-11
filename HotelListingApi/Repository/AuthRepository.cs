using AutoMapper;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.DTOs.Authentication;
using HotelListingApi.Helpers.AuthJwt;
using HotelListingApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using HotelListingApi.DTOs.AuthDTOs;

namespace HotelListingApi.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Jwt _jwt;
        private readonly IMapper _mapper;

       

        public AuthRepository(UserManager<AppUser> userManager , IMapper mapper , IOptions<Jwt> jwt )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _jwt = jwt.Value ?? throw new ArgumentNullException(nameof(jwt));
      
        }
        public async Task<Auth> RegisterAsync(RegisterDto registerDto)
        {
            if(await _userManager.FindByEmailAsync(registerDto.EmailAddress) is not null) 
            {
                return new Auth { Message = "Email already registered before and has been used" };
            }

            if(await _userManager.FindByNameAsync(registerDto.Username) is not null)
            {
                return new Auth { Message = "UserName already used in System! try another" };
            }

            var newUserRegistration = _mapper.Map<AppUser>(registerDto);

            var result =await _userManager.CreateAsync(newUserRegistration , registerDto.Password);
           
            if(!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }

                return new Auth {Message= errors};

            }

            await _userManager.AddToRoleAsync(newUserRegistration, "User");

            var jwtSecurityToken = await CreateJwtToken(newUserRegistration);

            return new Auth
            {
                Email = newUserRegistration.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = newUserRegistration.UserName
            };


        }

        public async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));


            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.NameId, user.UserName ?? "defaultUserName"),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "defaultEmail"),
            new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "defaultRole"),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id?.ToString() ?? "defaultUserId"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("LoggedOn", DateTime.Now.ToString()),
            }
            .Union(userClaims)
            .Union(roleClaims);
            


            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.Durationindays),
                signingCredentials: signingCredentials);


            return jwtSecurityToken;

        }

        public async Task<Auth> GetLoginAsync(LoginDto loginDto)
        {
            var authMdel = new Auth();

            var userEmailExistence = await _userManager.FindByEmailAsync(loginDto.Email);

            if (userEmailExistence is null || !await _userManager.CheckPasswordAsync(userEmailExistence, loginDto.Password))
            {
                authMdel.Message = "Email or pass may be wrong BRO";
                return authMdel;
            }

            var JwtSecurityToken = await CreateJwtToken(userEmailExistence);
            var rolesList = await _userManager.GetRolesAsync(userEmailExistence);

            authMdel.Email = userEmailExistence.Email;
            authMdel.ExpiresOn = JwtSecurityToken.ValidTo;
            authMdel.IsAuthenticated = true;
            authMdel.Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);
            authMdel.Roles = rolesList.ToList();
            authMdel.UserName = userEmailExistence.UserName;

            return authMdel;

        }
    }
}
