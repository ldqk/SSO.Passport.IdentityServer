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
                m.CreateMap<ClientApp, ClientAppInputDto>();
                m.CreateMap<ClientAppInputDto, ClientApp>();

                m.CreateMap<Control, ControlOutputDto>().ForMember(c => c.HttpMethod, e => e.MapFrom(c => c.HttpMethod.ToString()));
                m.CreateMap<Control, ControlInputDto>().ForMember(c => c.HttpMethod, e => e.MapFrom(c => c.HttpMethod.ToString()));
                m.CreateMap<ControlInputDto, Control>();

                m.CreateMap<Menu, MenuOutputDto>();
                m.CreateMap<Menu, MenuInputDto>();
                m.CreateMap<MenuInputDto, Menu>().ForMember(e => e.Url, e => e.MapFrom(x => x.Url.Equals("null") ? null : x.Url)).ForMember(e => e.Route, e => e.MapFrom(x => x.Route.Equals("null") ? null : x.Route)).ForMember(e => e.RouteName, e => e.MapFrom(x => x.RouteName.Equals("null") ? null : x.RouteName));

                m.CreateMap<Permission, PermissionOutputDto>();
                m.CreateMap<Permission, PermissionInputDto>();
                m.CreateMap<PermissionInputDto, Permission>();

                m.CreateMap<Role, RoleOutputDto>();
                m.CreateMap<Role, RoleInputDto>();
                m.CreateMap<RoleInputDto, Role>();

                m.CreateMap<UserInfo, UserInfoDto>();
                m.CreateMap<UserInfo, UserInfoViewModel>();

                m.CreateMap<UserGroup, UserGroupOutputDto>();
                m.CreateMap<UserGroup, UserGroupInputDto>();
                m.CreateMap<UserGroupInputDto, UserGroup>();

                m.CreateMap<LoginRecord, LoginRecordDto>();

                m.CreateMap<UserPermission, UserPermissionOutputDto>();

                m.CreateMap<UserGroupRole, UserGroupRoleOutputDto>();
            });
        }
    }
}