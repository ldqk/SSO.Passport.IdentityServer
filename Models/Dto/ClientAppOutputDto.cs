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
        /// 域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 客户端子系统唯一标识
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 客户端子系统密钥
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// 是否是预置
        /// </summary>
        public bool Preset { get; set; }
    }
}