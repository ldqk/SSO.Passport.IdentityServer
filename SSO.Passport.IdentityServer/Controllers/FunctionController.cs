using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using IBLL;
using Masuit.Tools;
using Models.Dto;
using Models.Entity;
using Models.Enum;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class FunctionController : BaseController
    {
        public IFunctionBll FunctionBll { get; set; }
        public IPermissionBll PermissionBll { get; set; }

        public FunctionController(IFunctionBll functionBll, IPermissionBll permissionBll)
        {
            FunctionBll = functionBll;
            PermissionBll = permissionBll;
        }


        public ActionResult Get(int id)
        {
            FunctionOutputDto dto = Mapper.Map<FunctionOutputDto>(FunctionBll.GetById(id));
            return ResultData(dto);
        }

        public ActionResult GetAllList()
        {
            IQueryable<Function> entities = FunctionBll.LoadEntitiesNoTracking(c => true);
            IList<FunctionOutputDto> list = Mapper.Map<IList<FunctionOutputDto>>(entities.ToList());
            return ResultData(list, entities.Any());
        }
        public ActionResult GetAllListByType(int id)
        {
            IQueryable<Function> entities = FunctionBll.LoadEntitiesNoTracking(c => c.FunctionType == ((id == 1) ? FunctionType.Menu : FunctionType.Operating));
            IList<FunctionOutputDto> list = Mapper.Map<IList<FunctionOutputDto>>(entities.ToList());
            return ResultData(list, entities.Any());
        }

        public ActionResult GetPageData(int page = 1, int size = 10)
        {
            IQueryable<Function> pageData = FunctionBll.LoadPageEntitiesNoTracking(page, size, out int totalCount, c => true, c => c.Controller);
            PageDataViewModel model = new PageDataViewModel()
            {
                Data = Mapper.Map<IList<FunctionOutputDto>>(pageData),
                PageIndex = page,
                PageSize = size,
                TotalPage = Math.Ceiling(totalCount.To<double>() / size.To<double>()).ToInt32(),
                TotalCount = totalCount
            };
            return ResultData(model, pageData.Any());
        }

        public ActionResult GetPageDataByType(int id, int page = 1, int size = 10)
        {
            IQueryable<Function> pageData = FunctionBll.LoadPageEntitiesNoTracking(page, size, out int totalCount, c => c.FunctionType == (id == 1 ? FunctionType.Menu : FunctionType.Operating), c => c.Controller);
            PageDataViewModel model = new PageDataViewModel()
            {
                Data = Mapper.Map<IList<FunctionOutputDto>>(pageData),
                PageIndex = page,
                PageSize = size,
                TotalPage = Math.Ceiling(totalCount.To<double>() / size.To<double>()).ToInt32(),
                TotalCount = totalCount
            };
            return ResultData(model, pageData.Any());
        }

        public ActionResult Add(FunctionInputDto function)
        {
            FunctionOutputDto dto = Mapper.Map<FunctionOutputDto>(FunctionBll.AddEntitySaved(Mapper.Map<Function>(function)));
            return ResultData(dto);
        }

        public ActionResult Update(FunctionInputDto dto)
        {
            Function function = FunctionBll.GetById(dto.Id);
            function.Controller = dto.Controller;
            function.Action = dto.Action;
            function.CssStyle = dto.CssStyle;
            function.HttpMethod = dto.HttpMethod;
            function.IconUrl = dto.IconUrl;
            function.IsAvailable = dto.IsAvailable;
            function.ParentId = dto.ParentId;
            Permission permission = PermissionBll.GetById(dto.PermissionId);
            if (permission != null)
            {
                function.PermissionId = dto.PermissionId;
            }
            bool res = FunctionBll.UpdateEntity(function);
            return res ? ResultData(dto, message: "修改成功！") : ResultData(null, false, "修改失败！");
        }


        public ActionResult ChangePermission(int id, int pid)
        {
            Function function = FunctionBll.GetById(id);
            Permission permission = PermissionBll.GetById(pid);
            if (permission != null && function != null)
            {
                function.PermissionId = permission.Id;
                bool res = FunctionBll.UpdateEntity(function);
                return res ? ResultData(Mapper.Map<FunctionOutputDto>(function), message: $"权限成功修改为：{permission.PermissionName}！") : ResultData(null, false, "权限修改失败！");
            }
            return ResultData(null, false, "数据不存在！");
        }
        public ActionResult ChangeType(int id, int tid)
        {
            Function function = FunctionBll.GetById(id);
            if (function != null)
            {
                function.FunctionType = tid == 1 ? FunctionType.Menu : FunctionType.Operating;
                bool res = FunctionBll.UpdateEntity(function);
                return res ? ResultData(Mapper.Map<FunctionOutputDto>(function), message: $"类型修改成功！") : ResultData(null, false, "类型修改失败！");
            }
            return ResultData(null, false, "数据不存在！");
        }

        public ActionResult Delete(int id)
        {
            bool res = FunctionBll.DeleteById(id);
            return ResultData(null, res, res ? "删除成功！" : "删除失败！");
        }

        public ActionResult Deletes(string id)
        {
            string[] ids = id.Split(',');
            IQueryable<Function> functions = FunctionBll.LoadEntities(r => ids.Contains(r.Id.ToString()));
            bool b = FunctionBll.DeleteEntitiesSaved(functions);
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }
    }
}