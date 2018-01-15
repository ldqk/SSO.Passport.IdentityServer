
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/15/2018 23:16:50
-- Generated from EDMX file: E:\Private\SSO.Passport.IdentityServer\ModelCodeGenerate\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [IdentityServer];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_dbo_UserPermission_dbo_Permission_PermissionId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserPermission] DROP CONSTRAINT [FK_dbo_UserPermission_dbo_Permission_PermissionId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_UserGroupPermission_dbo_Role_RoleId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserGroupPermission] DROP CONSTRAINT [FK_dbo_UserGroupPermission_dbo_Role_RoleId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_UserGroupPermission_dbo_UserGroup_UserGroupId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserGroupPermission] DROP CONSTRAINT [FK_dbo_UserGroupPermission_dbo_UserGroup_UserGroupId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_UserPermission_dbo_UserInfo_UserInfoId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserPermission] DROP CONSTRAINT [FK_dbo_UserPermission_dbo_UserInfo_UserInfoId];
GO
IF OBJECT_ID(N'[dbo].[FK_RolePermission_Permission]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RolePermission] DROP CONSTRAINT [FK_RolePermission_Permission];
GO
IF OBJECT_ID(N'[dbo].[FK_RolePermission_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RolePermission] DROP CONSTRAINT [FK_RolePermission_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_UserInfoRole_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserInfoRole] DROP CONSTRAINT [FK_UserInfoRole_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_UserInfoRole_UserInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserInfoRole] DROP CONSTRAINT [FK_UserInfoRole_UserInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_UserInfoUserGroup_UserGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserInfoUserGroup] DROP CONSTRAINT [FK_UserInfoUserGroup_UserGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_UserInfoUserGroup_UserInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserInfoUserGroup] DROP CONSTRAINT [FK_UserInfoUserGroup_UserInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientAppUserInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserInfo] DROP CONSTRAINT [FK_ClientAppUserInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_ClientAppUserGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserGroup] DROP CONSTRAINT [FK_ClientAppUserGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_MenuMenu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Menu] DROP CONSTRAINT [FK_MenuMenu];
GO
IF OBJECT_ID(N'[dbo].[FK_PermissionMenu_Permission]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PermissionMenu] DROP CONSTRAINT [FK_PermissionMenu_Permission];
GO
IF OBJECT_ID(N'[dbo].[FK_PermissionMenu_Menu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PermissionMenu] DROP CONSTRAINT [FK_PermissionMenu_Menu];
GO
IF OBJECT_ID(N'[dbo].[FK_PermissionControls_Permission]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PermissionControls] DROP CONSTRAINT [FK_PermissionControls_Permission];
GO
IF OBJECT_ID(N'[dbo].[FK_PermissionControls_Controls]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PermissionControls] DROP CONSTRAINT [FK_PermissionControls_Controls];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Permission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Permission];
GO
IF OBJECT_ID(N'[dbo].[Role]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Role];
GO
IF OBJECT_ID(N'[dbo].[UserGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserGroup];
GO
IF OBJECT_ID(N'[dbo].[UserGroupPermission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserGroupPermission];
GO
IF OBJECT_ID(N'[dbo].[UserInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserInfo];
GO
IF OBJECT_ID(N'[dbo].[UserPermission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserPermission];
GO
IF OBJECT_ID(N'[dbo].[Controls]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Controls];
GO
IF OBJECT_ID(N'[dbo].[ClientApp]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClientApp];
GO
IF OBJECT_ID(N'[dbo].[Menu]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Menu];
GO
IF OBJECT_ID(N'[dbo].[RolePermission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RolePermission];
GO
IF OBJECT_ID(N'[dbo].[UserInfoRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserInfoRole];
GO
IF OBJECT_ID(N'[dbo].[UserInfoUserGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserInfoUserGroup];
GO
IF OBJECT_ID(N'[dbo].[PermissionMenu]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PermissionMenu];
GO
IF OBJECT_ID(N'[dbo].[PermissionControls]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PermissionControls];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Permission'
CREATE TABLE [dbo].[Permission] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PermissionName] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'Role'
CREATE TABLE [dbo].[Role] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleName] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'UserGroup'
CREATE TABLE [dbo].[UserGroup] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [GroupName] nvarchar(max)  NOT NULL,
    [ParentId] int  NULL,
    [ClientAppId] int  NOT NULL
);
GO

-- Creating table 'UserGroupPermission'
CREATE TABLE [dbo].[UserGroupPermission] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HasPermission] bit  NOT NULL,
    [UserGroupId] int  NOT NULL,
    [RoleId] int  NOT NULL
);
GO

-- Creating table 'UserInfo'
CREATE TABLE [dbo].[UserInfo] (
    [Id] uniqueidentifier  NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [SaltKey] nvarchar(max)  NOT NULL,
    [PhoneNumber] nvarchar(11)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [LastLoginTime] datetime  NULL,
    [ClientAppId] int  NOT NULL
);
GO

-- Creating table 'UserPermission'
CREATE TABLE [dbo].[UserPermission] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HasPermission] bit  NOT NULL,
    [UserInfoId] uniqueidentifier  NULL,
    [PermissionId] int  NULL
);
GO

-- Creating table 'Controls'
CREATE TABLE [dbo].[Controls] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Controller] nvarchar(max)  NOT NULL,
    [Action] nvarchar(max)  NOT NULL,
    [HttpMethod] nvarchar(max)  NOT NULL,
    [IsAvailable] nvarchar(max)  NOT NULL,
    [FunctionType] int  NOT NULL
);
GO

-- Creating table 'ClientApp'
CREATE TABLE [dbo].[ClientApp] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AppName] nvarchar(max)  NOT NULL,
    [AppId] nvarchar(max)  NOT NULL,
    [AppSecret] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Menu'
CREATE TABLE [dbo].[Menu] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IconUrl] nvarchar(max)  NULL,
    [CssStyle] nvarchar(max)  NULL,
    [IsAvailable] nvarchar(max)  NOT NULL,
    [ParentId] int  NOT NULL
);
GO

-- Creating table 'RolePermission'
CREATE TABLE [dbo].[RolePermission] (
    [Permission_Id] int  NOT NULL,
    [Role_Id] int  NOT NULL
);
GO

-- Creating table 'UserInfoRole'
CREATE TABLE [dbo].[UserInfoRole] (
    [Role_Id] int  NOT NULL,
    [UserInfo_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UserInfoUserGroup'
CREATE TABLE [dbo].[UserInfoUserGroup] (
    [UserGroup_Id] int  NOT NULL,
    [UserInfo_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'PermissionMenu'
CREATE TABLE [dbo].[PermissionMenu] (
    [Permission_Id] int  NOT NULL,
    [Menu_Id] int  NOT NULL
);
GO

-- Creating table 'PermissionControls'
CREATE TABLE [dbo].[PermissionControls] (
    [Permission_Id] int  NOT NULL,
    [Controls_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Permission'
ALTER TABLE [dbo].[Permission]
ADD CONSTRAINT [PK_Permission]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Role'
ALTER TABLE [dbo].[Role]
ADD CONSTRAINT [PK_Role]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserGroup'
ALTER TABLE [dbo].[UserGroup]
ADD CONSTRAINT [PK_UserGroup]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserGroupPermission'
ALTER TABLE [dbo].[UserGroupPermission]
ADD CONSTRAINT [PK_UserGroupPermission]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserInfo'
ALTER TABLE [dbo].[UserInfo]
ADD CONSTRAINT [PK_UserInfo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserPermission'
ALTER TABLE [dbo].[UserPermission]
ADD CONSTRAINT [PK_UserPermission]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Controls'
ALTER TABLE [dbo].[Controls]
ADD CONSTRAINT [PK_Controls]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ClientApp'
ALTER TABLE [dbo].[ClientApp]
ADD CONSTRAINT [PK_ClientApp]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Menu'
ALTER TABLE [dbo].[Menu]
ADD CONSTRAINT [PK_Menu]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Permission_Id], [Role_Id] in table 'RolePermission'
ALTER TABLE [dbo].[RolePermission]
ADD CONSTRAINT [PK_RolePermission]
    PRIMARY KEY CLUSTERED ([Permission_Id], [Role_Id] ASC);
GO

-- Creating primary key on [Role_Id], [UserInfo_Id] in table 'UserInfoRole'
ALTER TABLE [dbo].[UserInfoRole]
ADD CONSTRAINT [PK_UserInfoRole]
    PRIMARY KEY CLUSTERED ([Role_Id], [UserInfo_Id] ASC);
GO

-- Creating primary key on [UserGroup_Id], [UserInfo_Id] in table 'UserInfoUserGroup'
ALTER TABLE [dbo].[UserInfoUserGroup]
ADD CONSTRAINT [PK_UserInfoUserGroup]
    PRIMARY KEY CLUSTERED ([UserGroup_Id], [UserInfo_Id] ASC);
GO

-- Creating primary key on [Permission_Id], [Menu_Id] in table 'PermissionMenu'
ALTER TABLE [dbo].[PermissionMenu]
ADD CONSTRAINT [PK_PermissionMenu]
    PRIMARY KEY CLUSTERED ([Permission_Id], [Menu_Id] ASC);
GO

-- Creating primary key on [Permission_Id], [Controls_Id] in table 'PermissionControls'
ALTER TABLE [dbo].[PermissionControls]
ADD CONSTRAINT [PK_PermissionControls]
    PRIMARY KEY CLUSTERED ([Permission_Id], [Controls_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [PermissionId] in table 'UserPermission'
ALTER TABLE [dbo].[UserPermission]
ADD CONSTRAINT [FK_dbo_UserPermission_dbo_Permission_PermissionId]
    FOREIGN KEY ([PermissionId])
    REFERENCES [dbo].[Permission]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_UserPermission_dbo_Permission_PermissionId'
CREATE INDEX [IX_FK_dbo_UserPermission_dbo_Permission_PermissionId]
ON [dbo].[UserPermission]
    ([PermissionId]);
GO

-- Creating foreign key on [RoleId] in table 'UserGroupPermission'
ALTER TABLE [dbo].[UserGroupPermission]
ADD CONSTRAINT [FK_dbo_UserGroupPermission_dbo_Role_RoleId]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Role]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_UserGroupPermission_dbo_Role_RoleId'
CREATE INDEX [IX_FK_dbo_UserGroupPermission_dbo_Role_RoleId]
ON [dbo].[UserGroupPermission]
    ([RoleId]);
GO

-- Creating foreign key on [UserGroupId] in table 'UserGroupPermission'
ALTER TABLE [dbo].[UserGroupPermission]
ADD CONSTRAINT [FK_dbo_UserGroupPermission_dbo_UserGroup_UserGroupId]
    FOREIGN KEY ([UserGroupId])
    REFERENCES [dbo].[UserGroup]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_UserGroupPermission_dbo_UserGroup_UserGroupId'
CREATE INDEX [IX_FK_dbo_UserGroupPermission_dbo_UserGroup_UserGroupId]
ON [dbo].[UserGroupPermission]
    ([UserGroupId]);
GO

-- Creating foreign key on [UserInfoId] in table 'UserPermission'
ALTER TABLE [dbo].[UserPermission]
ADD CONSTRAINT [FK_dbo_UserPermission_dbo_UserInfo_UserInfoId]
    FOREIGN KEY ([UserInfoId])
    REFERENCES [dbo].[UserInfo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_UserPermission_dbo_UserInfo_UserInfoId'
CREATE INDEX [IX_FK_dbo_UserPermission_dbo_UserInfo_UserInfoId]
ON [dbo].[UserPermission]
    ([UserInfoId]);
GO

-- Creating foreign key on [Permission_Id] in table 'RolePermission'
ALTER TABLE [dbo].[RolePermission]
ADD CONSTRAINT [FK_RolePermission_Permission]
    FOREIGN KEY ([Permission_Id])
    REFERENCES [dbo].[Permission]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Role_Id] in table 'RolePermission'
ALTER TABLE [dbo].[RolePermission]
ADD CONSTRAINT [FK_RolePermission_Role]
    FOREIGN KEY ([Role_Id])
    REFERENCES [dbo].[Role]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RolePermission_Role'
CREATE INDEX [IX_FK_RolePermission_Role]
ON [dbo].[RolePermission]
    ([Role_Id]);
GO

-- Creating foreign key on [Role_Id] in table 'UserInfoRole'
ALTER TABLE [dbo].[UserInfoRole]
ADD CONSTRAINT [FK_UserInfoRole_Role]
    FOREIGN KEY ([Role_Id])
    REFERENCES [dbo].[Role]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [UserInfo_Id] in table 'UserInfoRole'
ALTER TABLE [dbo].[UserInfoRole]
ADD CONSTRAINT [FK_UserInfoRole_UserInfo]
    FOREIGN KEY ([UserInfo_Id])
    REFERENCES [dbo].[UserInfo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserInfoRole_UserInfo'
CREATE INDEX [IX_FK_UserInfoRole_UserInfo]
ON [dbo].[UserInfoRole]
    ([UserInfo_Id]);
GO

-- Creating foreign key on [UserGroup_Id] in table 'UserInfoUserGroup'
ALTER TABLE [dbo].[UserInfoUserGroup]
ADD CONSTRAINT [FK_UserInfoUserGroup_UserGroup]
    FOREIGN KEY ([UserGroup_Id])
    REFERENCES [dbo].[UserGroup]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [UserInfo_Id] in table 'UserInfoUserGroup'
ALTER TABLE [dbo].[UserInfoUserGroup]
ADD CONSTRAINT [FK_UserInfoUserGroup_UserInfo]
    FOREIGN KEY ([UserInfo_Id])
    REFERENCES [dbo].[UserInfo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserInfoUserGroup_UserInfo'
CREATE INDEX [IX_FK_UserInfoUserGroup_UserInfo]
ON [dbo].[UserInfoUserGroup]
    ([UserInfo_Id]);
GO

-- Creating foreign key on [ClientAppId] in table 'UserInfo'
ALTER TABLE [dbo].[UserInfo]
ADD CONSTRAINT [FK_ClientAppUserInfo]
    FOREIGN KEY ([ClientAppId])
    REFERENCES [dbo].[ClientApp]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientAppUserInfo'
CREATE INDEX [IX_FK_ClientAppUserInfo]
ON [dbo].[UserInfo]
    ([ClientAppId]);
GO

-- Creating foreign key on [ClientAppId] in table 'UserGroup'
ALTER TABLE [dbo].[UserGroup]
ADD CONSTRAINT [FK_ClientAppUserGroup]
    FOREIGN KEY ([ClientAppId])
    REFERENCES [dbo].[ClientApp]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ClientAppUserGroup'
CREATE INDEX [IX_FK_ClientAppUserGroup]
ON [dbo].[UserGroup]
    ([ClientAppId]);
GO

-- Creating foreign key on [ParentId] in table 'Menu'
ALTER TABLE [dbo].[Menu]
ADD CONSTRAINT [FK_MenuMenu]
    FOREIGN KEY ([ParentId])
    REFERENCES [dbo].[Menu]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MenuMenu'
CREATE INDEX [IX_FK_MenuMenu]
ON [dbo].[Menu]
    ([ParentId]);
GO

-- Creating foreign key on [Permission_Id] in table 'PermissionMenu'
ALTER TABLE [dbo].[PermissionMenu]
ADD CONSTRAINT [FK_PermissionMenu_Permission]
    FOREIGN KEY ([Permission_Id])
    REFERENCES [dbo].[Permission]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Menu_Id] in table 'PermissionMenu'
ALTER TABLE [dbo].[PermissionMenu]
ADD CONSTRAINT [FK_PermissionMenu_Menu]
    FOREIGN KEY ([Menu_Id])
    REFERENCES [dbo].[Menu]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PermissionMenu_Menu'
CREATE INDEX [IX_FK_PermissionMenu_Menu]
ON [dbo].[PermissionMenu]
    ([Menu_Id]);
GO

-- Creating foreign key on [Permission_Id] in table 'PermissionControls'
ALTER TABLE [dbo].[PermissionControls]
ADD CONSTRAINT [FK_PermissionControls_Permission]
    FOREIGN KEY ([Permission_Id])
    REFERENCES [dbo].[Permission]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Controls_Id] in table 'PermissionControls'
ALTER TABLE [dbo].[PermissionControls]
ADD CONSTRAINT [FK_PermissionControls_Controls]
    FOREIGN KEY ([Controls_Id])
    REFERENCES [dbo].[Controls]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PermissionControls_Controls'
CREATE INDEX [IX_FK_PermissionControls_Controls]
ON [dbo].[PermissionControls]
    ([Controls_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------