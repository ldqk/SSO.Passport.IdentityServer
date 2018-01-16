
using IDAL;
using Models.Entity;
using System;
namespace DAL
{
	public partial class ClientAppDal :BaseDal<ClientApp>,IClientAppDal{}
	public partial class ControlDal :BaseDal<Control>,IControlDal{}
	public partial class LoginRecordDal :BaseDal<LoginRecord>,ILoginRecordDal{}
	public partial class MenuDal :BaseDal<Menu>,IMenuDal{}
	public partial class PermissionDal :BaseDal<Permission>,IPermissionDal{}
	public partial class RoleDal :BaseDal<Role>,IRoleDal{}
	public partial class UserGroupDal :BaseDal<UserGroup>,IUserGroupDal{}
	public partial class UserGroupPermissionDal :BaseDal<UserGroupPermission>,IUserGroupPermissionDal{}
	public partial class UserInfoDal :BaseDal<UserInfo>,IUserInfoDal{}
	public partial class UserPermissionDal :BaseDal<UserPermission>,IUserPermissionDal{}
}