namespace UserService.DTOs
{
    public class UserUpdateDTO
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
    }
}
