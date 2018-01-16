using Models.Entity;

namespace Models.Dto
{
    public partial class UserGroupPermissionOutputDto : BaseEntity
    {
        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool HasPermission { get; set; }

        public int UserGroupId { get; set; }

        public int RoleId { get; set; }
        
    }
}