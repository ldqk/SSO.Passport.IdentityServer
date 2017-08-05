using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.Caching;
using EntityFramework.Extensions;
using IDAL;
using Masuit.Tools.Net;
using Models.Application;

namespace DAL
{
    public class BaseDal<T> : IBaseDal<T> where T : class, new()
    {
        private PermissionContext db = WebExtension.GetDbContext<PermissionContext>();

        /// <summary>
        /// 基本查询方法
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<T> LoadEntities(Expression<Func<T, bool>> @where)
        {
            return db.Set<T>().Where(@where);
        }

        /// <summary>
        /// 基本查询方法
        /// </summary>
        /// <param name="where"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public IEnumerable<T> LoadEntitiesFromCache(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return db.Set<T>().Where(@where).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(timespan)));
        }

        /// <summary>
        /// 基本查询方法
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<IQueryable<T>> LoadEntitiesAsync(Expression<Func<T, bool>> @where)
        {
            return Task.Run(() => db.Set<T>().Where(@where));
        }

        /// <summary>
        /// 基本查询方法
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> LoadEntitiesFromCacheAsync(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return Task.Run(() => db.Set<T>().Where(@where).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(timespan))));
        }

        /// <summary>
        /// 基本查询方法（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<T> LoadEntitiesNoTracking(Expression<Func<T, bool>> @where)
        {
            return db.Set<T>().AsNoTracking().Where(@where);
        }

        /// <summary>
        /// 基本查询方法（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IEnumerable<T> LoadEntitiesFromCacheNoTracking(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return db.Set<T>().AsNoTracking().Where(@where).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(timespan)));
        }

        /// <summary>
        /// 基本查询方法（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<IQueryable<T>> LoadEntitiesNoTrackingAsync(Expression<Func<T, bool>> @where)
        {
            return Task.Run(() => db.Set<T>().AsNoTracking().Where(@where));
        }

        /// <summary>
        /// 基本查询方法（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> LoadEntitiesFromCacheNoTrackingAsync(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return Task.Run(() => db.Set<T>().AsNoTracking().Where(@where).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(timespan))));
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T GetFirstEntity(Expression<Func<T, bool>> @where)
        {
            return db.Set<T>().FirstOrDefault(where);
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T GetFirstEntityFromCache(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return LoadEntitiesFromCache(where, timespan).FirstOrDefault();
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<T> GetFirstEntityAsync(Expression<Func<T, bool>> @where)
        {
            return db.Set<T>().FirstOrDefaultAsync(where);
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<T> GetFirstEntityFromCacheAsync(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return Task.Run(() => LoadEntitiesFromCache(where, timespan).FirstOrDefault());
        }

        /// <summary>
        /// 获取第一条数据（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T GetFirstEntityNoTracking(Expression<Func<T, bool>> @where)
        {
            return db.Set<T>().AsNoTracking().FirstOrDefault(where);
        }

        /// <summary>
        /// 获取第一条数据（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T GetFirstEntityFromCacheNoTracking(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return LoadEntitiesFromCacheNoTracking(where, timespan).FirstOrDefault();
        }

        /// <summary>
        /// 获取第一条数据（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<T> GetFirstEntityNoTrackingAsync(Expression<Func<T, bool>> @where)
        {
            return db.Set<T>().AsNoTracking().FirstOrDefaultAsync(where);
        }

        /// <summary>
        /// 获取第一条数据（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<T> GetFirstEntityFromCacheNoTrackingAsync(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return Task.Run(() => LoadEntitiesFromCacheNoTracking(where, timespan).FirstOrDefault());
        }

        /// <summary>
        /// 根据ID找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById(object id)
        {
            return db.Set<T>().Find(id);
        }

        /// <summary>
        /// 根据ID找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T> GetByIdAsync(object id)
        {
            return db.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// 高效分页查询方法
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="totalCount">数据总数</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns></returns>
        public IQueryable<T> LoadPageEntities<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc)
        {
            var temp = db.Set<T>().Where(where);
            totalCount = temp.Count();
            if (pageIndex * pageSize > totalCount)
            {
                pageIndex = (int)Math.Ceiling(totalCount / (pageSize * 1.0));
            }
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            return isAsc ? temp.OrderBy(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize) : temp.OrderByDescending(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }

        /// <summary>
        /// 高效分页查询方法
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="totalCount">数据总数</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns></returns>
        public IEnumerable<T> LoadPageEntitiesFromCache<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc, int timespan = 30)
        {
            var temp = db.Set<T>().Where(where);
            totalCount = temp.Count();
            if (pageIndex * pageSize > totalCount)
            {
                pageIndex = (int)Math.Ceiling(totalCount / (pageSize * 1.0));
            }
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            return isAsc ? temp.OrderBy(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(timespan))) : temp.OrderByDescending(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(timespan)));
        }

        /// <summary>
        /// 高效分页查询方法（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public IQueryable<T> LoadPageEntitiesNoTracking<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true)
        {
            var temp = db.Set<T>().AsNoTracking().Where(where);
            totalCount = temp.Count();
            if (pageIndex * pageSize > totalCount)
            {
                pageIndex = (int)Math.Ceiling(totalCount / (pageSize * 1.0));
            }
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            return isAsc ? temp.OrderBy(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize) : temp.OrderByDescending(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }

        /// <summary>
        /// 高效分页查询方法（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public IEnumerable<T> LoadPageEntitiesFromCacheNoTracking<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true, int timespan = 30)
        {
            var temp = db.Set<T>().AsNoTracking().Where(where);
            totalCount = temp.Count();
            if (pageIndex * pageSize > totalCount)
            {
                pageIndex = (int)Math.Ceiling(totalCount / (pageSize * 1.0));
            }
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            return isAsc ? temp.OrderBy(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(timespan))) : temp.OrderByDescending(orderby).Skip(pageSize * (pageIndex - 1)).Take(pageSize).FromCache(CachePolicy.WithDurationExpiration(TimeSpan.FromSeconds(timespan)));
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById(object id)
        {
            T t = GetById(id);
            db.Entry(t).State = EntityState.Deleted;
            return true;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool DeleteEntity(T t)
        {
            db.Entry(t).State = EntityState.Deleted;
            return true;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int DeleteEntity(Expression<Func<T, bool>> @where)
        {
            return db.Set<T>().Where(@where).Delete();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Task<int> DeleteEntityAsync(Expression<Func<T, bool>> @where)
        {
            return db.Set<T>().Where(@where).DeleteAsync();
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool UpdateEntity(T t)
        {
            db.Entry(t).State = EntityState.Modified;
            return true;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int UpdateEntity(Expression<Func<T, bool>> @where, T t)
        {
            return db.Set<T>().Where(@where).Update(ts => t);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Task<int> UpdateEntityAsync(Expression<Func<T, bool>> @where, T t)
        {
            return db.Set<T>().Where(@where).UpdateAsync(ts => t);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T AddEntity(T t)
        {
            db.Entry(t).State = EntityState.Added;
            return t;
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return db.SaveChanges();
        }
        /// <summary>
        /// 统一保存数据
        /// </summary>
        /// <returns></returns>
        public void BulkSaveChanges()
        {
            db.BulkSaveChanges();
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveChangesAsync()
        {
            return db.SaveChangesAsync();
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        /// <returns></returns>
        public void BulkSaveChangesAsync()
        {
            db.BulkSaveChangesAsync();
        }

        /// <summary>
        /// 判断实体是否在数据库中存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> @where)
        {
            return db.Set<T>().Any(where);
        }

        /// <summary>
        /// 删除多个实体并保存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool DeleteEntities(IEnumerable<T> list)
        {
            db.BulkDelete(list);
            return true;
        }

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpdateEntities(IEnumerable<T> list)
        {
            db.BulkUpdate(list);
            return true;
        }

        /// <summary>
        /// 添加多个实体并保存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<T> AddEntities(IEnumerable<T> list)
        {
            db.BulkInsert(list);
            return list;
        }
    }
}