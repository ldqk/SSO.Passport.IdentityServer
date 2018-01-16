using System.ComponentModel.DataAnnotations;
using Models.Entity;

namespace Models.Dto
{
    public partial class ClientAppInputDto : BaseEntity
    {
        /// <summary>
        /// 客户端子系统名字
        /// </summary>
        [Required]
        public string AppName { get; set; }

        /// <summary>
        /// 客户端子系统唯一标识
        /// </summary>
        [Required]
        public string AppId { get; set; }

        /// <summary>
        /// 客户端子系统密钥
        /// </summary>
        [Required]
        public string AppSecret { get; set; }
    }
}