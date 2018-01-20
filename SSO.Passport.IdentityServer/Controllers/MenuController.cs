using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using Common;
using Models.Dto;
using Models.Entity;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class MenuController : BaseController
    {
        #region 增删查改

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Add(MenuInputDto model)
        {
            if (!ModelState.IsValid) return ResultData(null, false, "数据校验失败！");
            var menu = model.Mapper<Menu>();
            menu = MenuBll.AddEntitySaved(menu);
            return menu != null ? ResultData(null, true, "菜单添加成功！") : ResultData(null, false, "菜单添加失败");
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Update(MenuInputDto model)
        {
            if (!ModelState.IsValid) return ResultData(null, false, "数据校验失败！");
            Menu menu = MenuBll.GetById(model.Id);
            Mapper.Map(model, menu);
            bool b = MenuBll.UpdateEntitySaved(menu);
            return ResultData(null, b, b ? "更新成功！" : "更新失败！");
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            bool b = MenuBll.DeleteByIdSaved(id);
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        /// <summary>
        /// 加载分页数据
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ActionResult PageData(string appid, int page = 1, int size = 10)
        {
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<Menu, bool>>)(c => true) : (c => c.ClientApp.AppId.Equals(appid));
            List<MenuOutputDto> list = MenuBll.LoadPageEntitiesNoTracking<int, MenuOutputDto>(page, size, out int total, where, c => c.Id, false).ToList();
            return PageResult(list, size, total);
        }

        /// <summary>
        /// 获取所有的菜单
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ActionResult GetAll(string appid)
        {
            List<MenuOutputDto> list = MenuBll.LoadEntitiesNoTracking<MenuOutputDto>(m => m.ClientApp.AppId.Equals(appid)).ToList();
            return ResultData(list);
        }

        /// <summary>
        /// 获取菜单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Get(int id)
        {
            MenuOutputDto c = MenuBll.GetById(id).Mapper<MenuOutputDto>();
            return ResultData(c);
        }

        #endregion

        #region 权限配置

        /// <summary>
        /// 指派权限
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <param name="pid">权限id</param>
        /// <returns></returns>
        public ActionResult AssignPermission(int id, int pid)
        {
            Menu menu = MenuBll.GetById(id);
            Permission permission = PermissionBll.GetById(pid);
            if (menu != null && permission != null)
            {
                menu.Permission.Add(permission);
                bool b = MenuBll.UpdateEntitySaved(menu);
                return ResultData(null, b, b ? "权限分配成功！" : "权限分配失败！");
            }
            return ResultData(null, false, "未找到菜单或权限！");
        }
        #endregion
    }
}