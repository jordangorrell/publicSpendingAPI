using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using spendingAPI.Data;
using spendingAPI.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using spendingAPI.Services;

namespace spendingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SpendingContext _context;
        private readonly IJWTService _jwtService;

        public AuthController(SpendingContext context, UserManager<IdentityUser> userManager, IJWTService jwtService)
        {
            _userManager = userManager;
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDetails registerDetails)
        {
            var user = new IdentityUser
            {
                UserName = registerDetails.Email,
                Email = registerDetails.Email
            };

            if (!PasswordAndConfirmPasswordMatch(registerDetails.Password, registerDetails.ConfirmPassword))
            {
                var errors = new List<string>
                {
                    "Password and confirm password do not match"
                };
                return BadRequest(new { errors });
            }

            var result = await _userManager.CreateAsync(user, registerDetails.Password);
                
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                var errors = new List<string>();
                foreach (IdentityError error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                var response = new { errors };
                return BadRequest(response); // Change this 
            }
        }

        [HttpPost("token")]
        public async Task<IActionResult> Login(LoginCredentials credentials)
        {
            if (await IsValidUsernameAndPassword(credentials.Email, credentials.Password))
            {
                var user = await _userManager.FindByNameAsync(credentials.Email);

                return new ObjectResult(_jwtService.GenerateToken(user));
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("pulse")]
        public IActionResult Pulse()
        {
            return Ok();
        }

        private async Task<bool> IsValidUsernameAndPassword(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username);
            return await _userManager.CheckPasswordAsync(user, password);
        }

        private bool PasswordAndConfirmPasswordMatch(string password, string confirmPassword)
        {
            return password == confirmPassword;
        }
    }
}