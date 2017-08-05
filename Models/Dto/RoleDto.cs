using System.Collections.Generic;

namespace Models.Dto
{
    public class RoleDto
    {
        public RoleDto()
        {
            Permission = new HashSet<PermissionDto>();
        }
        
        public string RoleName { get; set; }

        public string Description { get; set; }
        
        public virtual ICollection<PermissionDto> Permission { get; set; }
    }
}