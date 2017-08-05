using System.Collections.Generic;

namespace Models.Dto
{
    public class PermissionDto
    {
        public PermissionDto()
        {
            Function = new HashSet<FunctionDto>();
        }
        
        public string PermissionName { get; set; }

        public string Description { get; set; }

        public virtual ICollection<FunctionDto> Function { get; set; }
    }
}