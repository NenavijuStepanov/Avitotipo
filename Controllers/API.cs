using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webasp.Data;
using webasp.Models;

namespace webasp.Controllers
{
    [ApiController] // сообщает ASP.NET Core, что этот класс - REST API. Включает автоматическую валидацию моделей и автоматически считывает тела запросов из JSON.
    [Route("api")]  // задает префикс URL для всех методов класса. Запросы будут приходить на http://localhost:XXXX/api/
    public class AuthController : ControllerBase  // класс апишки для аутентификации
    {
        private readonly DB _db;
        private readonly IPasswordHasher<User> _hasher;  // приватные поля, чтоб в них записывать объекты из других файлов

        public AuthController(DB db, IPasswordHasher<User> hasher) // в функции переменные мы получаем их из других файлов и присваиваем нашим приватным полям
        {
            _db = db; // объект для работы с бд
            _hasher = hasher; // объект для хэширования
        }

        [HttpPost("login")]  // POST /api/login
        public async Task<IActionResult> Login([FromBody] LoginDto dto)   // получение из класса LoginDTO.cs значений
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);    // запрос к бд на поиск первого попавшегося с таким юзером
            if (user == null)
                return Unauthorized(); 

            
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);    // проверка на совпадение паролей
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(); 

            return Ok(); 
        }

        [HttpPost("registration")] // POST /api/registration 
        public async Task<IActionResult> Register([FromBody] LoginDto dto) // получение из класса LoginDTO.cs значений
        {

            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))  // проверка на существование кого-либо с таким юзером
                return BadRequest("Пользователь уже существует");

            var newUser = new User   // объявление объекта класса User.cs
            {
                Username = dto.Username
            };


            newUser.PasswordHash = _hasher.HashPassword(newUser, dto.Password); //хэширование

            _db.Users.Add(newUser);  // в объект добавляет данные
            await _db.SaveChangesAsync(); // посылает запрос и коммитит

            return Ok(); 
        }
    }
}

// апишка сыровата на самом деле, она работает, но возможны уязвимости и неправильные сценарии