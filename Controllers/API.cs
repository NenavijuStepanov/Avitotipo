using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webasp.Data;
using webasp.Models;

namespace webasp.Controllers
{
    [ApiController]
    [Route("api")] 
    public class AuthController : ControllerBase
    {
        private readonly DB _db;
        private readonly IPasswordHasher<User> _hasher;

        public AuthController(DB db, IPasswordHasher<User> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return Unauthorized(); 

            
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(); 

            return Ok(); 
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Register([FromBody] LoginDto dto)
        {

            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Пользователь уже существует");

            var newUser = new User
            {
                Username = dto.Username
            };


            newUser.PasswordHash = _hasher.HashPassword(newUser, dto.Password);

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return Ok(); 
        }
    }
}