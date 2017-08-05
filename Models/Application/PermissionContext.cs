using System.Data.Entity;
using Models.Entity;
using Models.Migrations;

namespace Models.Application
{
    public class PermissionContext : DbContext
    {
        public PermissionContext() : base("name=PermissionContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PermissionContext, Configuration>());
        }

        public virtual DbSet<Function> Function { get; set; }
        public virtual DbSet<FunctionType> FunctionType { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserGroup> UserGroup { get; set; }
        public virtual DbSet<UserGroupPermission> UserGroupPermission { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<UserPermission> UserPermission { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FunctionType>().HasMany(e => e.Function).WithRequired(e => e.FunctionType).WillCascadeOnDelete(false);

            modelBuilder.Entity<Permission>().HasMany(e => e.Function).WithRequired(e => e.Permission).WillCascadeOnDelete(false);

            modelBuilder.Entity<Permission>().HasMany(e => e.Role).WithMany(e => e.Permission).Map(m => m.ToTable("RolePermission"));

            modelBuilder.Entity<Role>().HasMany(e => e.UserGroupPermission).WithRequired(e => e.Role).WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>().HasMany(e => e.UserInfo).WithMany(e => e.Role).Map(m => m.ToTable("UserInfoRole"));

            modelBuilder.Entity<UserGroup>().HasMany(e => e.UserGroupPermission).WithRequired(e => e.UserGroup).WillCascadeOnDelete(false);

            modelBuilder.Entity<UserGroup>().HasMany(e => e.UserInfo).WithMany(e => e.UserGroup).Map(m => m.ToTable("UserInfoUserGroup"));
        }
    }
}