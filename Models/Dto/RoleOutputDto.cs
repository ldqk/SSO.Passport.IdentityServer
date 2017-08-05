using System.Collections.Generic;

namespace Models.Dto
{
    public class RoleOutputDto
    {
        public RoleOutputDto()
        {
            Permission = new HashSet<PermissionOutputDto>();
        }

        public int Id { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        public virtual ICollection<PermissionOutputDto> Permission { get; set; }
    }
}