using BookingApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public AuthController(
            UserManager<IdentityUser> userMgr,
            RoleManager<IdentityRole> roleMgr,
            IConfiguration config)
        {
            userManager = userMgr;
            roleManager = roleMgr;
            configuration = config;
        }

        /// <summary>
        /// Реєстрація нового користувача
        /// POST /api/auth/register
        /// Body: { "email": "...", "password": "...", "role": "User|Admin" }
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict(new { message = "Користувач з таким email вже існує" });

            var user = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            string role = dto.Role == "Admin" ? "Admin" : "User";
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

            await userManager.AddToRoleAsync(user, role);

            return Ok(new { message = $"Користувача {dto.Email} успішно зареєстровано з роллю {role}" });
        }

        /// <summary>
        /// Логін — повертає JWT токен
        /// POST /api/auth/login
        /// Body: { "email": "...", "password": "..." }
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new { message = "Невірний email або пароль" });

            var roles = await userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return Ok(new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email ?? "",
                Roles = roles,
                Expiration = token.ValidTo
            });
        }

        private JwtSecurityToken GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add role claims
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            return new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    double.Parse(jwtSettings["ExpirationHours"] ?? "24")),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}
