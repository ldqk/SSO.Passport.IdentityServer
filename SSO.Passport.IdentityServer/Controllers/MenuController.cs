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
        /// 添加或修改菜单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ActionResult Save(MenuInputDto model, string appid)
        {
            model.ParentId = model.ParentId == 0 ? null : model.ParentId;
            if (string.IsNullOrEmpty(model.IconUrl) || !model.IconUrl.Contains("/"))
            {
                model.IconUrl = null;
            }
            Menu m = MenuBll.GetById(model.Id);
            if (m == null)
            {
                if (!string.IsNullOrEmpty(appid) && ClientAppBll.Any(a => a.AppId.Equals(appid)))
                {
                    ClientApp app = ClientAppBll.GetFirstEntity(a => a.AppId.Equals(appid));
                    app.Menus.Add(model.Mapper<Menu>());
                    bool saved = ClientAppBll.UpdateEntitySaved(app);
                    return ResultData(null, saved, saved ? "添加成功" : "添加失败");
                }
                var menu = MenuBll.AddEntitySaved(model.Mapper<Menu>());
                return menu != null ? ResultData(menu, true, "添加成功") : ResultData(null, false, "添加失败");
            }
            Mapper.Map(model, m);
            bool b = MenuBll.UpdateEntitySaved(m);
            return ResultData(null, b, b ? "修改成功" : "修改失败");
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
        /// 获取所有的菜单
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult GetAll(string appid, string kw)
        {
            var @where = string.IsNullOrEmpty(kw) ? (Expression<Func<Menu, bool>>)(m => m.ClientApp.AppId.Equals(appid)) : (m => m.ClientApp.AppId.Equals(appid) && (m.Name.Contains(kw) || (m.Url != null && m.Url.Contains(kw)) || (m.Route != null && m.Route.Contains(kw)) || (m.RouteName != null && m.RouteName.Contains(kw))));
            List<MenuOutputDto> list = MenuBll.LoadEntitiesNoTracking<int, MenuOutputDto>(where, m => m.Sort).ToList();
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