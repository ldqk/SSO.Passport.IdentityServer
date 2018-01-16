using System.Collections.Generic;
using Models.Entity;

namespace Models.Dto
{
    public partial class ClientAppOutputDto : BaseEntity
    {

        /// <summary>
        /// 客户端子系统名字
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 客户端子系统唯一标识
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 客户端子系统密钥
        /// </summary>
        public string AppSecret { get; set; }

        public virtual ICollection<UserGroupOutputDto> UserGroup { get; set; }
        public virtual ICollection<ControlOutputDto> Controls { get; set; }
        public virtual ICollection<MenuOutputDto> Menus { get; set; }
        public virtual ICollection<PermissionOutputDto> Permissions { get; set; }
        public virtual ICollection<RoleOutputDto> Roles { get; set; }

        public virtual ICollection<UserInfoOutputDto> UserInfo { get; set; }
    }
}