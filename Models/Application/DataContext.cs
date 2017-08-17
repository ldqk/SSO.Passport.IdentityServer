using System;
using System.Data.Entity;
using System.Linq;
using EFSecondLevelCache;
using Models.Entity;
using Models.Migrations;
using static System.Data.Entity.Core.Objects.ObjectContext;

namespace Models.Application
{
    public class DataContext : DbContext
    {
        public DataContext() : base("name=DataContext")
        {
            Database.CreateIfNotExists();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
#if DEBUG
            Database.Log = Console.WriteLine;
#endif
        }

        public virtual DbSet<Function> Function { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserGroup> UserGroup { get; set; }
        public virtual DbSet<UserGroupPermission> UserGroupPermission { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<UserPermission> UserPermission { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasMany(e => e.Function).WithMany(e => e.Permission).Map(m => m.ToTable("PermissionFunction"));
            modelBuilder.Entity<Permission>().HasMany(e => e.Role).WithMany(e => e.Permission).Map(m => m.ToTable("RolePermission"));
            modelBuilder.Entity<Role>().HasMany(e => e.UserGroupPermission).WithRequired(e => e.Role).WillCascadeOnDelete(true);
            modelBuilder.Entity<Role>().HasMany(e => e.UserInfo).WithMany(e => e.Role).Map(m => m.ToTable("UserInfoRole"));
            modelBuilder.Entity<UserGroup>().HasMany(e => e.UserGroupPermission).WithRequired(e => e.UserGroup).WillCascadeOnDelete(true);
            modelBuilder.Entity<UserGroup>().HasMany(e => e.UserInfo).WithMany(e => e.UserGroup).Map(m => m.ToTable("UserInfoUserGroup"));
            modelBuilder.Entity<UserInfo>().HasMany(e => e.UserPermission).WithRequired(e => e.UserInfo).WillCascadeOnDelete(true);
            modelBuilder.Entity<Permission>().HasMany(e => e.UserPermission).WithRequired(e => e.Permission).WillCascadeOnDelete(true);
        }

        //重写 SaveChanges
        public int SaveChanges(bool invalidateCacheDependencies = true)
        {
            return SaveAllChanges(invalidateCacheDependencies);
        }

        public int SaveAllChanges(bool invalidateCacheDependencies = true)
        {
            var changedEntityNames = GetChangedEntityNames();
            var result = base.SaveChanges();
            if (invalidateCacheDependencies)
            {
                new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            }
            return result;
        }

        //修改、删除、添加数据时缓存失效
        private string[] GetChangedEntityNames()
        {
            return ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted).Select(x => GetObjectType(x.Entity.GetType()).FullName).Distinct().ToArray();
        }
    }
}