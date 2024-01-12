using ReadingClub.Infrastructure.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ReadingClub.Services.Interfaces;

namespace ReadingClub.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<ActionResult> Create(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Parameter is missing");
            }

            var countSummary = await _userService.CheckIfUserExists(createUserDto);

            // 0 - nothing, 1 - username exists, 2 - email exists, 3 - both exists
            if (countSummary == 1)
            {
                return Json(new { Status = false, Message = "This user name already exists." });
            }
            else if (countSummary == 2)
            {
                return Json(new { Status = false, Message = "This email already exists." });
            }
            else if (countSummary == 3)
            {
                return Json(new { Status = false, Message = "This user name and emai already exists." });
            }

            var userDto = await _userService.Create(createUserDto);

            return Json(new { Status = true, Data = userDto });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.IsValid);
            }

            var user = await _userService.Get(userDto);
            if (user != null)
            {
                var token = GenerateToken(user);
                return Json(new { Status = true, Data = token });
            }

            return Json(new { Status = false, Message = "User do not exists or password is incorrect." });
        }

        [AllowAnonymous]
        [HttpPost("isTokenValid")]
        public async Task<IActionResult> IsTokenValid([FromBody] TokenDto tokenDto) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var token = tokenHandler.ReadJwtToken(tokenDto.Token);
            var userEmail = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userDto = await _userService.Get(userEmail ??= ""); // if euserEmail is null assign ""

            if (userDto == null)
            {
                return Json(new { Status = false, Message = "Token validation failed, user not found." });
            }

            try
            {
                tokenHandler.ValidateToken(tokenDto.Token, validationParameters, out SecurityToken validatedToken);
                return Json(new { Status = true, Data = tokenDto.Token });
            }
            catch (SecurityTokenExpiredException)
            {
                // Token is expired, extract claims for refreshing
                var expiredToken = tokenHandler.ReadJwtToken(tokenDto.Token);
                var userName = expiredToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var email = expiredToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                // Generate a new token if expiration not bigger than a day
                if (userName != null && email != null && (DateTime.UtcNow - expiredToken.ValidTo).TotalDays <= 1) 
                {
                    var newToken = GenerateToken(new UserDto()
                    {
                        UserName = userName,
                        Email = email
                    });

                    return Json(new { Status = true, Data = newToken });
                }
                else
                {
                    return Json(new { Status = false, Message = "Token validation failed." });
                }
            }
            catch
            {
                return Json(new { Status = false, Message = "Token validation failed." });
            }
        }

        [Authorize]
        [HttpPost("getFullDetailsOfLoggedUser")]
        public async Task<ActionResult> Get()
        {
            string? userEmail = null;

            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userEmail = identity?.FindFirst(ClaimTypes.Email)?.Value;
            }

            if (userEmail == null)
            {
                return Json(new { Status = false, Message = "An error occurred during processing, user not found." });
            }

            var userDto = await _userService.Get(userEmail);

            if (userDto == null)
            {
                return Json(new { Status = false, Message = "An error occurred during processing, user not found." });
            }

            return Json(new { Status = true, Data = userDto });
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult> Update(UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.IsValid);
            }

            var userToUpdate = await _userService.Get(updateUserDto.OldEmail);

            if (userToUpdate == null)
            {
                return Json(new { Status = false, Message = "An error occurred during processing, user not found." });
            }

            var userDto = await _userService.Update(updateUserDto);

            return Json(new { NewStatus = true, Data = userDto });
        }

        [Authorize]
        [HttpDelete("deleteLoggedUser")]
        public async Task<ActionResult> Delete()
        {
            string? userEmail = null;

            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userEmail = identity?.FindFirst(ClaimTypes.Email)?.Value;
            }

            if (string.IsNullOrEmpty(userEmail))
            {
                return Json(new { Status = false, Message = "An error occurred during processing, user not found." });
            }

            try
            {
                await _userService.Delete(userEmail);
                return Json(new { Status = true });
            }
            catch (Exception)
            {
                return Json(new { Status = false, Message = "An error occurred during processing, cannot remove user." });
            }
        }

        [Authorize]
        [HttpPost("getLoggedUser")]
        public ActionResult GetLoggedUser()
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                string? email = identity?.FindFirst(ClaimTypes.Email)?.Value;
                string? userName = identity?.FindFirst(ClaimTypes.Name)?.Value;

                if ( string.IsNullOrEmpty(email) == false && string.IsNullOrEmpty(userName) == false)
                {
                    return Json(new
                    {
                        Status = true,
                        Data = new
                        {
                            email,
                            userName
                        }
                    });
                }
            }
            return Json(new { Status = false, Message = "An error occurred during processing, user not found." });
        }

        private string GenerateToken(UserDto userDto)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userDto.UserName),
                new Claim(ClaimTypes.Email, userDto.Email)
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
