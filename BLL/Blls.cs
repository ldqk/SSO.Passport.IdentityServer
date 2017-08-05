
using IBLL;
using Models.Entity;
namespace BLL
{
	
	public partial class FunctionBll :BaseBll<Function>,IFunctionBll{}   
	
	public partial class PermissionBll :BaseBll<Permission>,IPermissionBll{}   
	
	public partial class RoleBll :BaseBll<Role>,IRoleBll{}   
	
	public partial class UserGroupBll :BaseBll<UserGroup>,IUserGroupBll{}   
	
	public partial class UserGroupPermissionBll :BaseBll<UserGroupPermission>,IUserGroupPermissionBll{}   
	
	public partial class UserInfoBll :BaseBll<UserInfo>,IUserInfoBll{}   
	
	public partial class UserPermissionBll :BaseBll<UserPermission>,IUserPermissionBll{}   
}