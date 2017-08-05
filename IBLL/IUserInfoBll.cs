using Models.Entity;

namespace IBLL
{
    public partial interface IUserInfoBll
    {
        UserInfo GetByUsername(string name);
        UserInfo Login(string username, string password);
        UserInfo Register(UserInfo userInfo);
    }
}