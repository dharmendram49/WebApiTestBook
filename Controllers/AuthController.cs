using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using WebApiTestBook.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.OutputCaching;
using WebApiTestBook.Services.Interfaces;
using WebApiTestBook.Services;

namespace WebApiTestBook.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings jwtSettings;
        private readonly IOtpService otpService;

        public AuthController(
             IOptions<JwtSettings> jwtOptions,
             IOtpService otpService
            )
        {
            this.jwtSettings = jwtOptions.Value;
            this.otpService = otpService;
        }



        [HttpPost("geneateToken")]
        public async Task<ActionResult> GenerateToken(TokenRequest request)
        {
            var token  = GenerateAccessToken(request.Id, request.Role);
            return Ok(token);
        }

        // 🔹 Send OTP
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(string mobile)
        {
            var otp = await otpService.GenerateOtp(mobile);


            // In real app → send SMS instead
            return Ok(new { message = "OTP sent", otp });
        }

        // 🔹 Verify OTP
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(string mobile, string otp)
        {
            var isValid = await otpService.VerifyOtp(mobile, otp);

            if (!isValid)
                return BadRequest("Invalid or expired OTP");

            return Ok("OTP verified");
        }

        private string GenerateAccessToken(int userId, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

 

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                jwtSettings.Issuer,
                jwtSettings.Audience,
                claims,
                expires: System.DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    SecurityAlgorithms.HmacSha256
                ));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
