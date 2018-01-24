using AutoMapper;
using Models.Dto;
using Models.Entity;
using Models.ViewModel;

namespace Common
{
    public static class RegisterAutomapper
    {
        public static void Excute()
        {
            Mapper.Initialize(m =>
            {
                m.CreateMap<ClientApp, ClientAppOutputDto>();
                m.CreateMap<ClientAppInputDto, ClientApp>();

                m.CreateMap<Control, ControlOutputDto>().ForMember(c => c.HttpMethod, e => e.MapFrom(c => c.HttpMethod.ToString()));
                m.CreateMap<ControlInputDto, Control>();

                m.CreateMap<Menu, MenuOutputDto>();
                m.CreateMap<MenuInputDto, Menu>().ForMember(e => e.Url, e => e.MapFrom(x => x.Url.Equals("null") ? null : x.Url)).ForMember(e => e.Route, e => e.MapFrom(x => x.Route.Equals("null") ? null : x.Route)).ForMember(e => e.RouteName, e => e.MapFrom(x => x.RouteName.Equals("null") ? null : x.RouteName));

                m.CreateMap<Permission, PermissionOutputDto>();
                m.CreateMap<PermissionInputDto, Permission>();

                m.CreateMap<Role, RoleOutputDto>();
                m.CreateMap<RoleInputDto, Role>();

                m.CreateMap<UserInfo, UserInfoDto>();
                m.CreateMap<UserInfo, UserInfoViewModel>();

                m.CreateMap<UserGroup, UserGroupOutputDto>();
                m.CreateMap<UserGroupInputDto, UserGroup>();

                m.CreateMap<LoginRecord, LoginRecordDto>();

                m.CreateMap<UserPermission, UserPermissionOutputDto>();

                m.CreateMap<UserGroupRole, UserGroupRoleOutputDto>();
            });
        }
    }
}