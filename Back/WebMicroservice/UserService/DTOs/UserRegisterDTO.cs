using System.ComponentModel.DataAnnotations;
using UserService.Models;

namespace UserService.DTOs
{
    public class UserRegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public UserType type { get; set; }
    }
}
