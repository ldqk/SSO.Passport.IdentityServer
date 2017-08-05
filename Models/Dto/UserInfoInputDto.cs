using System;

namespace Models.Dto
{
    public class UserInfoInputDto
    {

        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string SaltKey { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime? LastLoginTime { get; set; }
    }
}