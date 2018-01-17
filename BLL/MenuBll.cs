using System.Data.Entity.Infrastructure;
using System.Linq;
using Masuit.Tools.Net;
using Models.Application;
using Models.Dto;

namespace BLL
{
    public partial class MenuBll
    {
        /// <summary>
        /// 通过存储过程获得自己以及自己所有的子元素集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DbRawSqlQuery<MenuOutputDto> GetSelfAndChildrenByParentId(int id)
        {
            return WebExtension.GetDbContext<DataContext>().Database.SqlQuery<MenuOutputDto>("exec sp_getChildrenMenuByParentId " + id);
        }

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetParentIdById(int id)
        {
            DbRawSqlQuery<int> raw = WebExtension.GetDbContext<DataContext>().Database.SqlQuery<int>("exec sp_getParentMenuIdByChildId " + id);
            if (raw.Any())
            {
                return raw.FirstOrDefault();
            }
            return 0;
        }

    }
}