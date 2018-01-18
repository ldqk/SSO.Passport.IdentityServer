using Models.Dto;

namespace SSO.Passport.IdentityServer.Models.Hangfire
{
    public interface IHangfireBackJob
    {
        void LoginRecord(UserInfoDto userInfo, string ip);
    }
}