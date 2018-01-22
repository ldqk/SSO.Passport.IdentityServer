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
    public class AccessController : BaseController
    {
        #region 增删查改

        /// <summary>
        /// 添加功能
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Add(ControlInputDto model)
        {
            if (ModelState.IsValid)
            {
                Control control = model.Mapper<Control>();
                control = ControlBll.AddEntitySaved(control);
                if (control != null)
                {
                    return ResultData(null, true, "功能添加成功！");
                }

                return ResultData(null, false, "功能添加失败");
            }

            return ResultData(null, false, "数据校验失败！");
        }

        /// <summary>
        /// 更新功能
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Update(ControlInputDto model)
        {
            if (ModelState.IsValid)
            {
                Control control = ControlBll.GetById(model.Id);
                Mapper.Map(model, control);
                bool b = ControlBll.UpdateEntitySaved(control);
                return ResultData(null, b, b ? "更新成功！" : "更新失败！");
            }

            return ResultData(null, false, "数据校验失败！");
        }

        /// <summary>
        /// 删除功能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            bool b = ControlBll.DeleteByIdSaved(id);
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
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<Control, bool>>)(c => true) : (c => c.ClientApp.AppId.Equals(appid));
            List<ControlOutputDto> list = ControlBll.LoadPageEntitiesNoTracking<int, ControlOutputDto>(page, size, out int total, where, c => c.Id, false).ToList();
            return PageResult(list, size, total);
        }

        /// <summary>
        /// 获取功能详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Get(int id)
        {
            ControlOutputDto c = ControlBll.GetById(id).Mapper<ControlOutputDto>();
            return ResultData(c);
        }

        /// <summary>
        /// 切换功能控制器的可用状态
        /// </summary>
        /// <param name="id">功能控制器id</param>
        /// <param name="state">可用状态</param>
        /// <returns></returns>
        public ActionResult ToggleState(int id, bool state)
        {
            Control control = ControlBll.GetById(id);
            if (control is null)
            {
                return ResultData(null, false, "功能控制器不存在！");
            }

            control.IsAvailable = !state;
            bool b = ControlBll.UpdateEntitySaved(control);
            return ResultData(null, b, b ? "状态切换成功！" : "状态切换失败！");
        }

        #endregion

        #region 权限配置

        /// <summary>
        /// 获取该控制器的权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyPermissions(int id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<Permission, bool>> where;
            Expression<Func<Permission, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.Controls.All(c => c.Id != id) && u.PermissionName.Contains(kw);
                where2 = u => u.Controls.Any(c => c.Id == id) && u.PermissionName.Contains(kw);
            }
            else
            {
                where = u => u.Controls.All(c => c.Id != id);
                where2 = u => u.Controls.Any(c => c.Id == id);
            }

            List<PermissionOutputDto> not = PermissionBll.LoadPageEntities<int, PermissionOutputDto>(page, size, out int total1, where, u => u.Id, false).ToList(); //不属于该角色
            List<PermissionOutputDto> my = PermissionBll.LoadPageEntities<int, PermissionOutputDto>(page, size, out int total2, where2, u => u.Id, false).ToList(); //属于该角色
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 指派权限
        /// </summary>
        /// <param name="id">功能id</param>
        /// <param name="pid">权限id</param>
        /// <returns></returns>
        public ActionResult AssignPermission(int id, int pid)
        {
            Control control = ControlBll.GetById(id);
            Permission permission = PermissionBll.GetById(pid);
            if (control != null && permission != null)
            {
                control.Permission.Add(permission);
                bool b = ControlBll.UpdateEntitySaved(control);
                return ResultData(null, b, b ? "权限分配成功！" : "权限分配失败！");
            }

            return ResultData(null, false, "未找到功能或权限！");
        }

        /// <summary>
        /// 移除权限
        /// </summary>
        /// <param name="id">功能id</param>
        /// <param name="pid">权限id</param>
        /// <returns></returns>
        public ActionResult RemovePermission(int id, int pid)
        {
            Control control = ControlBll.GetById(id);
            Permission permission = PermissionBll.GetById(pid);
            if (control != null && permission != null)
            {
                control.Permission.Remove(permission);
                bool b = ControlBll.UpdateEntitySaved(control);
                return ResultData(null, b, b ? "权限移除成功！" : "权限移除失败！");
            }

            return ResultData(null, false, "未找到功能或权限！");
        }

        #endregion
    }
}