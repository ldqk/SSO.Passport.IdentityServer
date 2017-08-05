using System;
using System.Collections.Generic;

namespace Models.Dto
{
    public class UserInfoDto
    {
        public UserInfoDto()
        {
            UserPermission = new HashSet<UserPermissionDto>();
            Role = new HashSet<RoleDto>();
            UserGroup = new HashSet<UserGroupDto>();
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public virtual ICollection<UserPermissionDto> UserPermission { get; set; }

        public virtual ICollection<RoleDto> Role { get; set; }

        public virtual ICollection<UserGroupDto> UserGroup { get; set; }
    }
}