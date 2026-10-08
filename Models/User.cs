using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace webasp.Models
{
    [Index(nameof(Username), IsUnique = true)] // проверка на уникальность юза
    public class User // класс для пользователей при взаимодействии с сервером
    {
        public int Id { get; set; }

        [Required] 
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
    }


}
