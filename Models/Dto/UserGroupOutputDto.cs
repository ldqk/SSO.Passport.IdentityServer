using System.Collections.Generic;

namespace Models.Dto
{
    public class UserGroupOutputDto
    {
        public UserGroupOutputDto()
        {
            UserGroupPermission = new HashSet<UserGroupPermissionDto>();
        }

        public int Id { get; set; }

        public string GroupName { get; set; }

        public int? ParentId { get; set; }

        public virtual ICollection<UserGroupPermissionDto> UserGroupPermission { get; set; }
    }
}