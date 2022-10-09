using AaCTraveling.API.Dtos;
using AaCTraveling.API.Models;
using AaCTraveling.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AaCTraveling.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITouristRouteRepository _touristRouteRepository;

        public AuthenticateController(IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITouristRouteRepository touristRouteRepository)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _touristRouteRepository = touristRouteRepository;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // check email and password
            var loginResult = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);

            if (!loginResult.Succeeded)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(loginDto.Email);
            var roleNames = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName,user.Email)
            };

            for (int i = 0; i < roleNames.Count; i++)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleNames[i]));
            }

            var secretKey = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            var signingKey = new SymmetricSecurityKey(secretKey);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signingCredentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenString);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };

            await _touristRouteRepository.CreateShoppingCartAsync(shoppingCart);
            await _touristRouteRepository.SaveAsync();

            return Ok();
        }
    }
}
