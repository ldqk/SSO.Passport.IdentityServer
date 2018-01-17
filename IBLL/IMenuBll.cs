using System.Data.Entity.Infrastructure;
using Models.Dto;

namespace IBLL
{
    public partial interface IMenuBll
    {
        /// <summary>
        /// 通过存储过程获得自己以及自己所有的子元素集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DbRawSqlQuery<MenuOutputDto> GetSelfAndChildrenByParentId(int id);

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetParentIdById(int id);
    }
}