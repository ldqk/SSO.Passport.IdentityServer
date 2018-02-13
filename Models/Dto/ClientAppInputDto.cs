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
        /// 域名
        /// </summary>
        public string Domain { get; set; }

    }
}