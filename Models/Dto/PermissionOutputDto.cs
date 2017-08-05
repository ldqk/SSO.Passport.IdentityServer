using System.Collections.Generic;

namespace Models.Dto
{
    public class PermissionOutputDto
    {
        public PermissionOutputDto()
        {
            Function = new HashSet<FunctionOutputDto>();
        }

        public int Id { get; set; }

        public string PermissionName { get; set; }

        public string Description { get; set; }

        public virtual ICollection<FunctionOutputDto> Function { get; set; }
    }
}