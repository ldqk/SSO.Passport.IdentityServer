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
            Mapper.Initialize(m =>//兼容写法，保证automapper升级后仍可用
            {
                m.CreateMap<UserInfo, UserInfoOutputDto>();
                m.CreateMap<UserInfoOutputDto, UserInfo>();
                m.CreateMap<UserInfo, UserInfoViewModel>();
                m.CreateMap<UserInfoViewModel, UserInfo>();
                m.CreateMap<UserInfo, UserInfoInputDto>();
                m.CreateMap<UserInfoInputDto, UserInfo>();
                m.CreateMap<UserInfoInputDto, UserInfoViewModel>();
                m.CreateMap<UserInfoViewModel, UserInfoInputDto>();
                m.CreateMap<UserInfoOutputDto, UserInfoViewModel>();
                m.CreateMap<UserInfoViewModel, UserInfoOutputDto>();
                m.CreateMap<UserInfoInputDto, UserInfoOutputDto>();
                m.CreateMap<UserInfoOutputDto, UserInfoInputDto>();


                m.CreateMap<UserGroup, UserGroupOutputDto>();
                m.CreateMap<UserGroupOutputDto, UserGroup>();
                m.CreateMap<UserGroup, UserGroupInputDto>();
                m.CreateMap<UserGroupInputDto, UserGroup>();
                m.CreateMap<UserGroupInputDto, UserGroupOutputDto>();
                m.CreateMap<UserGroupOutputDto, UserGroupInputDto>();

                m.CreateMap<UserPermission, UserPermissionOutputDto>();
                m.CreateMap<UserPermissionOutputDto, UserPermission>();
                m.CreateMap<UserPermissionOutputDto, UserPermissionInputDto>();
                m.CreateMap<UserPermissionInputDto, UserPermissionOutputDto>();
                m.CreateMap<UserPermissionInputDto, UserPermission>();
                m.CreateMap<UserPermission, UserPermissionInputDto>();

                m.CreateMap<UserGroupPermission, UserGroupPermissionDto>();
                m.CreateMap<UserGroupPermissionDto, UserGroupPermission>();

                m.CreateMap<Role, RoleOutputDto>();
                m.CreateMap<RoleOutputDto, Role>();
                m.CreateMap<Role, RoleInputDto>();
                m.CreateMap<RoleInputDto, Role>();
                m.CreateMap<RoleInputDto, RoleOutputDto>();
                m.CreateMap<RoleOutputDto, RoleInputDto>();

                m.CreateMap<Permission, PermissionOutputDto>();
                m.CreateMap<PermissionOutputDto, Permission>();
                m.CreateMap<Permission, PermissionInputDto>();
                m.CreateMap<PermissionInputDto, Permission>();
                m.CreateMap<PermissionInputDto, PermissionOutputDto>();
                m.CreateMap<PermissionOutputDto, PermissionInputDto>();
                
            });
        }
    }
}