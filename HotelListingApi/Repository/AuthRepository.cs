using AutoMapper;
using HotelListingApi.Data.Interfaces;
using HotelListingApi.DTOs.Authentication;
using HotelListingApi.Helpers.AuthJwt;
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
using HotelListingApi.Models.AuthModels;
using System.Security.Cryptography;

namespace HotelListingApi.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Jwt _jwt;
        private readonly IMapper _mapper;

       

        public AuthRepository(UserManager<AppUser> userManager , RoleManager<IdentityRole> roleManager, IMapper mapper , IOptions<Jwt> jwt )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
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
                //ExpiresOn = jwtSecurityToken.ValidTo,
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
            //authMdel.ExpiresOn = JwtSecurityToken.ValidTo;
            authMdel.IsAuthenticated = true;
            authMdel.Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);
            authMdel.Roles = rolesList.ToList();
            authMdel.UserName = userEmailExistence.UserName;




            if(userEmailExistence.RefreshTokens.Any(token => token.IsActive))
            {
                var activerefreshToken = userEmailExistence.RefreshTokens.FirstOrDefault(token=>token.IsActive);
                authMdel.RefreshToken = activerefreshToken.Token;
                authMdel.RefreshTokenExpiration = activerefreshToken.ExpiresOn;

            }
            else
            {
                var refreshTOken = GenerateRfreshToekn();
                authMdel.RefreshToken = refreshTOken.Token;
                authMdel.RefreshTokenExpiration = refreshTOken.ExpiresOn;
                userEmailExistence.RefreshTokens.Add(refreshTOken);
                await _userManager.UpdateAsync(userEmailExistence);
            }




            return authMdel;

        }

        public async Task<string> AddRoleAsync(AddRoleDto roleDto)
        {
            var user = await _userManager.FindByIdAsync(roleDto.UserId);
            if(user is null)
            {
                return "Invald User ID";
            }
            
            if(!await _roleManager.RoleExistsAsync(roleDto.RoleName))
            {
                return "Invalid RoleName";
            }

            if(await _userManager.IsInRoleAsync(user , roleDto.RoleName))
            {
                return "User already assigned to this Role! You can't add it again";
            }

            var result = await _userManager.AddToRoleAsync(user , roleDto.RoleName);
            if(!result.Succeeded)
            {
                return "Something Went Wrong";
            }

            return result.ToString();
        }


        private RefreshToken GenerateRfreshToekn()
        {
            var randomNumber = new byte[32];

            using var Generator = new RNGCryptoServiceProvider();
            Generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(1),
                CreatedOn = DateTime.UtcNow
            };
        }

        public async Task<Auth> RefreshTOkenAsync(string refreshToken)
        {
            var authModel = new Auth();

            var user = _userManager.Users.SingleOrDefault(users => users.RefreshTokens.Any(t=>t.Token == refreshToken));    
            if(user == null)
            {

                authModel.Message = "Invalid TOken";
                return authModel;
            }

            var refreshedToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken);


            if (!refreshedToken.IsActive)
            {
                authModel.Message = "InActive Token";
                return authModel;
            }

            refreshedToken.RevokedOn = DateTime.UtcNow;

            var newRefreshTOken = GenerateRfreshToekn();
            user.RefreshTokens.Add(newRefreshTOken);
            await _userManager.UpdateAsync(user);


            var jwtToken = await CreateJwtToken(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshTOken.Token;
            authModel.RefreshTokenExpiration = newRefreshTOken.ExpiresOn;
            

            return authModel;

        }
    }
}
