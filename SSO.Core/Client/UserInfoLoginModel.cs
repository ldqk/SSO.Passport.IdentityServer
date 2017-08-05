using System;

namespace SSO.Core.Client
{
    public class UserInfoLoginModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginTime { get; set; }
    }
}