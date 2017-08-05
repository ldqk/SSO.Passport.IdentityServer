using AutoMapper;
using Models.Dto;
using Models.Entity;

namespace Common
{
    public static class RegisterAutomapper
    {
        public static void Excute()
        {
            Mapper.Initialize(m =>//兼容写法，保证automapper升级后仍可用
            {
                m.CreateMap<UserInfo, UserInfoDto>();
                m.CreateMap<UserInfoDto, UserInfo>();
                m.CreateMap<UserInfo, UserInfoViewModel>();
                m.CreateMap<UserInfoViewModel, UserInfo>();
                m.CreateMap<UserGroup, UserGroupDto>();
                m.CreateMap<UserGroupDto, UserGroup>();
                m.CreateMap<UserPermission, UserPermissionDto>();
                m.CreateMap<UserPermissionDto, UserPermission>();
                m.CreateMap<UserGroupPermission, UserGroupPermissionDto>();
                m.CreateMap<UserGroupPermissionDto, UserGroupPermission>();
                m.CreateMap<Role, RoleDto>();
                m.CreateMap<RoleDto, Role>();
                m.CreateMap<Permission, PermissionDto>();
                m.CreateMap<PermissionDto, Permission>();
                m.CreateMap<Function, FunctionDto>();
                m.CreateMap<FunctionDto, Function>();
                m.CreateMap<FunctionType, FunctionTypeDto>();
                m.CreateMap<FunctionTypeDto, FunctionType>();
            });
        }
    }
}