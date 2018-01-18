using System;
using IBLL;
using Masuit.Tools.Models;
using Masuit.Tools.Net;
using Models.Dto;
using Models.Entity;

namespace SSO.Passport.IdentityServer.Models.Hangfire
{
    public class HangfireBackJob : IHangfireBackJob
    {
        public IUserInfoBll UserInfoBll { get; set; }
        public HangfireBackJob(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }
        public void LoginRecord(UserInfoDto userInfo, string ip)
        {
            PhysicsAddress address = ip.GetPhysicsAddressInfo();
            LoginRecord record = new LoginRecord() { IP = ip, LoginTime = DateTime.Now, PhysicAddress = address.AddressResult.FormattedAddress, Province = address.AddressResult.AddressComponent.Province };
            UserInfo u = UserInfoBll.GetByUsername(userInfo.Username);
            u.LoginRecords.Add(record);
            UserInfoBll.UpdateEntitySaved(u);
        }

    }
}