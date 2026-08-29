using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace webasp.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
    }


}
