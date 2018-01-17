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

                m.CreateMap<Control, ControlOutputDto>();
                m.CreateMap<ControlInputDto, Control>();

                m.CreateMap<Menu, MenuOutputDto>();
                m.CreateMap<MenuInputDto, Menu>();

                m.CreateMap<Permission, PermissionOutputDto>();
                m.CreateMap<PermissionInputDto, Permission>();

                m.CreateMap<Role, RoleOutputDto>();
                m.CreateMap<RoleInputDto, Role>();

                m.CreateMap<UserInfo, UserInfoOutputDto>();
                m.CreateMap<UserInfo, UserInfoViewModel>();
                m.CreateMap<UserInfoInputDto, UserInfo>();

                m.CreateMap<UserGroup, UserGroupOutputDto>();
                m.CreateMap<UserGroupInputDto, UserGroup>();

                m.CreateMap<LoginRecord, LoginRecordDto>();

                m.CreateMap<UserPermission, UserPermissionOutputDto>();

                m.CreateMap<UserGroupRole, UserGroupRoleOutputDto>();
            });
        }
    }
}