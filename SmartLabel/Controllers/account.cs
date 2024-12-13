using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartLabel.labelData;
using SmartLabel.models;

namespace SmartLabel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class account : ControllerBase
    {
        public account(UserManager<Users> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        private readonly UserManager<Users> _userManager;
        private readonly IConfiguration _configuration;


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] registerUser register, [FromQuery] bool isAdmin = false)
        {
            var user = new Users { UserName = register.Username, Email = register.Email };
            var result = await _userManager.CreateAsync(user, register.Password);

            if (result.Succeeded)
            {
                var role = isAdmin ? "Admin" : "User";
                await _userManager.AddToRoleAsync(user, role);

                return Ok("User created successfully.");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login([FromBody] loginUser login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, login.Password)))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            var token = await GenerateJwtToken(user);

            return Ok(new { token });
           
        }

        private async Task<string> GenerateJwtToken(Users user)
        {
            // Set up claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // Get roles and check if the user is an admin
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add the isAdmin claim (true if the user is in the Admin role)
            var isAdmin = roles.Contains("Admin");
            claims.Add(new Claim("isAdmin", isAdmin.ToString())); // Add isAdmin claim

      
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:secretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["jwt:issuer"],
                audience: _configuration["jwt:audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(20),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
