using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using IBLL;
using Masuit.Tools;
using Models.Dto;
using Models.Entity;
using Models.Enum;
using Models.ViewModel;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class FunctionController : BaseController
    {
        public IFunctionBll FunctionBll { get; set; }
        public IPermissionBll PermissionBll { get; set; }

        public FunctionController(IFunctionBll functionBll, IPermissionBll permissionBll, IUserInfoBll userInfoBll)
        {
            FunctionBll = functionBll;
            PermissionBll = permissionBll;
            UserInfoBll = userInfoBll;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Get(int id)
        {
            return View(FunctionBll.GetById(id));
        }

        public ActionResult GetAllList()
        {
            IQueryable<Function> entities = FunctionBll.LoadEntitiesNoTracking(c => true);
            IList<FunctionOutputDto> list = Mapper.Map<IList<FunctionOutputDto>>(entities.ToList());
            return ResultData(list, entities.Any());
        }
        public ActionResult GetAllListByType(int id)
        {
            IQueryable<Function> entities = FunctionBll.LoadEntities(c => c.FunctionType == ((id == 1) ? FunctionType.Menu : FunctionType.Operating));
            IList<FunctionOutputDto> list = Mapper.Map<IList<FunctionOutputDto>>(entities.ToList());
            return ResultData(list, entities.Any());
        }

        public ActionResult GetPageData(int page = 1, int size = 10)
        {
            IQueryable<Function> pageData = FunctionBll.LoadPageEntities(page, size, out int totalCount, c => true, c => c.Controller);
            DataTableViewModel model = new DataTableViewModel()
            {
                data = Mapper.Map<IList<FunctionOutputDto>>(pageData),
                recordsFiltered = totalCount,
                recordsTotal = totalCount
            };
            return Content(model.ToJsonString());
        }

        public ActionResult GetPageDataByType(int id, int start = 1, int length = 10)
        {
            var search = Request["search[value]"];
            bool b = search.IsNullOrEmpty();
            var page = start / length + 1;
            IQueryable<Function> pageData = FunctionBll.LoadPageEntities(page, length, out int totalCount, c => c.FunctionType == (id == 1 ? FunctionType.Menu : FunctionType.Operating) && (b || c.Name.Contains(search) || c.Controller.Contains(search) || c.Action.Contains(search)), c => c.Controller);
            DataTableViewModel model = new DataTableViewModel()
            {
                data = Mapper.Map<IList<FunctionOutputDto>>(pageData.ToList()),
                recordsFiltered = totalCount,
                recordsTotal = totalCount
            };
            return Content(model.ToJsonString());
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(FunctionInputDto function)
        {
            if (function.FunctionType <= 0 || function.FunctionType > FunctionType.Operating)
            {
                return ResultData("", false, "请选择一个权限类型");
            }
            if (function.HttpMethod <= 0 || function.HttpMethod > HttpMethod.Proppatch)
            {
                return ResultData("", false, "请选择一个HTTP请求方式");
            }
            function.IsAvailable = true;
            FunctionOutputDto dto = Mapper.Map<FunctionOutputDto>(FunctionBll.AddEntitySaved(Mapper.Map<Function>(function)));
            return ResultData(dto);
        }

        public ActionResult Edit(int id)
        {
            Function fun = FunctionBll.GetById(id);
            return View(fun);
        }

        public ActionResult Update(FunctionInputDto dto)
        {
            if (dto.FunctionType <= 0 || dto.FunctionType > FunctionType.Operating)
            {
                return ResultData("", false, "请选择一个权限类型");
            }
            if (dto.HttpMethod <= 0 || dto.HttpMethod > HttpMethod.Proppatch)
            {
                return ResultData("", false, "请选择一个HTTP请求方式");
            }
            Function function = FunctionBll.GetById(dto.Id);
            function.Controller = dto.Controller;
            function.Action = dto.Action;
            function.CssStyle = dto.CssStyle;
            function.HttpMethod = dto.HttpMethod;
            function.IconUrl = dto.IconUrl;
            function.ParentId = dto.ParentId;
            function.FunctionType = dto.FunctionType;
            bool res = FunctionBll.UpdateEntitySaved(function);
            return res ? ResultData(dto, message: "修改成功！") : ResultData(null, false, "修改失败！");
        }
        public ActionResult ChangeType(int id, int tid)
        {
            Function function = FunctionBll.GetById(id);
            if (function != null)
            {
                function.FunctionType = tid == 1 ? FunctionType.Menu : FunctionType.Operating;
                bool res = FunctionBll.UpdateEntitySaved(function);
                return res ? ResultData(Mapper.Map<FunctionOutputDto>(function), message: $"类型修改成功！") : ResultData(null, false, "类型修改失败！");
            }
            return ResultData(null, false, "数据不存在！");
        }

        public ActionResult Delete(int id)
        {
            bool res = FunctionBll.DeleteEntity(c => c.Id == id) > 0;
            return ResultData(null, res, res ? "删除成功！" : "删除失败！");
        }

        public ActionResult Deletes(string id)
        {
            string[] ids = id.Split(',');
            bool b = FunctionBll.DeleteEntity(r => ids.Contains(r.Id.ToString())) > 0;
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        public ActionResult PermissionNoHasFunction(int id)
        {
            IEnumerable<Function> functions = FunctionBll.LoadEntities(r => true).ToList().Except(PermissionBll.GetById(id).Function.ToList());
            return ResultData(Mapper.Map<IList<FunctionOutputDto>>(functions.ToList()));
        }

        public ActionResult PermissionFunctionList(int id)
        {
            return ResultData(Mapper.Map<IList<FunctionOutputDto>>(PermissionBll.GetById(id).Function.ToList()));
        }

        public ActionResult UpdatePermissionFunction(int id, string fids)
        {
            string[] strs = fids.Split(',');
            IQueryable<Function> functions = FunctionBll.LoadEntities(r => strs.Contains(r.Id.ToString()));
            Permission permission = PermissionBll.GetById(id);
            permission.Function.Clear();
            functions.ToList().ForEach(r => permission.Function.Add(r));
            PermissionBll.UpdateEntity(permission);
            bool b = PermissionBll.SaveChanges() > 0;
            return ResultData(null, b, b ? "权限功能分配成功！" : "权限功能分配失败！");
        }

        public ActionResult Permission(int id)
        {
            Function function = FunctionBll.GetById(id);
            return View(function);
        }

        public ActionResult NoHasPermission(int id)
        {
            IEnumerable<Permission> permissions = PermissionBll.LoadEntities(r => true).ToList().Except(FunctionBll.GetById(id).Permission.ToList());
            return ResultData(Mapper.Map<IList<PermissionOutputDto>>(permissions.ToList()));
        }

        public ActionResult PermissionList(int id)
        {
            return ResultData(Mapper.Map<IList<PermissionOutputDto>>(FunctionBll.GetById(id).Permission.ToList()));
        }

        public ActionResult UpdateFunctionPermission(int id, string pids)
        {
            string[] strs = pids.Split(',');
            IQueryable<Permission> permissions = PermissionBll.LoadEntities(r => strs.Contains(r.Id.ToString()));
            Function function = FunctionBll.GetById(id);
            function.Permission.Clear();
            permissions.ToList().ForEach(r => function.Permission.Add(r));
            FunctionBll.UpdateEntity(function);
            bool b = FunctionBll.SaveChanges() > 0;
            return ResultData(null, b, b ? "权限功能分配成功！" : "权限功能分配失败！");
        }
    }
}