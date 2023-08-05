﻿namespace UserService.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public byte[] ProfilePictureUrl { get; set; }
        public string Address { get; set; }
        public UserType Type { get; set; }
        public bool Approved { get; set; }
        public bool Denied { get; set; }
    }
}
