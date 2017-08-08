using System;
using System.Collections.Generic;

namespace Models.Dto
{
    public class UserInfoOutputDto
    {
        public UserInfoOutputDto()
        {
            UserPermission = new HashSet<UserPermissionOutputDto>();
            Role = new HashSet<RoleOutputDto>();
            UserGroup = new HashSet<UserGroupOutputDto>();
        }

        public Guid Id { get; set; }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public virtual ICollection<UserPermissionOutputDto> UserPermission { get; set; }

        public virtual ICollection<RoleOutputDto> Role { get; set; }

        public virtual ICollection<UserGroupOutputDto> UserGroup { get; set; }
    }
}