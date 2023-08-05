using UserService.Models;

namespace UserService.DTOs
{
    public class UserDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public UserType type { get; set; }
        public bool Approved { get; set; }
        public bool Denied { get; set; }
    }
}
