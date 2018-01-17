using Models.Entity;

namespace Models.Dto
{
    public partial class UserGroupRoleOutputDto : BaseEntity
    {
        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool HasRole { get; set; }

        public int UserGroupId { get; set; }

        public int RoleId { get; set; }

    }
}