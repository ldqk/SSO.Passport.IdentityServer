
using IBLL;
using Models.Entity;
namespace BLL
{
	
	public partial class ClientAppBll :BaseBll<ClientApp>,IClientAppBll{}   
	
	public partial class ControlBll :BaseBll<Control>,IControlBll{}   
	
	public partial class LoginRecordBll :BaseBll<LoginRecord>,ILoginRecordBll{}   
	
	public partial class MenuBll :BaseBll<Menu>,IMenuBll{}   
	
	public partial class PermissionBll :BaseBll<Permission>,IPermissionBll{}   
	
	public partial class RoleBll :BaseBll<Role>,IRoleBll{}   
	
	public partial class UserGroupBll :BaseBll<UserGroup>,IUserGroupBll{}   
	
	public partial class UserGroupRoleBll :BaseBll<UserGroupRole>,IUserGroupRoleBll{}   
	
	public partial class UserInfoBll :BaseBll<UserInfo>,IUserInfoBll{}   
	
	public partial class UserPermissionBll :BaseBll<UserPermission>,IUserPermissionBll{}   
}