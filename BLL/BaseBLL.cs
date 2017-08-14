using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DALFactory;
using EFSecondLevelCache;
using IBLL;
using IDAL;

namespace BLL
{
    public class BaseBll<T> : IBaseBll<T> where T : class, new()
    {
        public IBaseDal<T> BaseDal { get; set; } = Factory.CreateInstance<IBaseDal<T>>("DAL." + typeof(T).Name + "Dal");

        /// <summary>
        /// 基本查询方法，获取一个集合
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public IQueryable<T> LoadEntities(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntities(where);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>还未执行的SQL语句</returns>
        public IEnumerable<T> LoadEntitiesFromCache(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return BaseDal.LoadEntitiesFromCache(where, timespan);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从二级缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public IEnumerable<T> LoadEntitiesFromL2Cache(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesFromL2Cache(where);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合(异步)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IQueryable<T>> LoadEntitiesAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesAsync(where);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从缓存读取(异步)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IEnumerable<T>> LoadEntitiesFromCacheAsync(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return BaseDal.LoadEntitiesFromCacheAsync(where, timespan);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从二级缓存读取(异步)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<EFCachedQueryable<T>> LoadEntitiesFromL2CacheAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesFromL2CacheAsync(where);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合（不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public IQueryable<T> LoadEntitiesNoTracking(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesNoTracking(where);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从缓存读取(不跟踪实体)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>实体集合</returns>
        public IEnumerable<T> LoadEntitiesFromCacheNoTracking(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return BaseDal.LoadEntitiesFromCacheNoTracking(where, timespan);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从二级缓存读取(不跟踪实体)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体集合</returns>
        public IEnumerable<T> LoadEntitiesFromL2CacheNoTracking(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesFromL2CacheNoTracking(where);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合（异步，不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体集合</returns>
        public Task<IQueryable<T>> LoadEntitiesNoTrackingAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesNoTrackingAsync(where);
        }

        /// <summary>
        ///  基本查询方法，获取一个集合，优先从缓存读取(异步，不跟踪实体)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>实体集合</returns>
        public Task<IEnumerable<T>> LoadEntitiesFromCacheNoTrackingAsync(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return BaseDal.LoadEntitiesFromCacheNoTrackingAsync(where, timespan);
        }

        /// <summary>
        ///  基本查询方法，获取一个集合，优先从二级缓存读取(异步，不跟踪实体)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体集合</returns>
        public Task<EFCachedQueryable<T>> LoadEntitiesFromL2CacheNoTrackingAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesFromL2CacheNoTrackingAsync(where);
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public T GetFirstEntity(Expression<Func<T, bool>> @where)
        {
            return LoadEntities(where).FirstOrDefault();
        }

        /// <summary>
        /// 获取第一条数据，优先从缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>实体</returns>
        public T GetFirstEntityFromCache(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return BaseDal.GetFirstEntityFromCache(where, timespan);
        }

        /// <summary>
        /// 获取第一条数据，优先从缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public T GetFirstEntityFromL2Cache(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityFromL2Cache(where);
        }

        /// <summary>
        /// 获取第一条数据，优先从缓存读取(异步)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>实体</returns>
        public Task<T> GetFirstEntityFromCacheAsync(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return BaseDal.GetFirstEntityFromCacheAsync(where, timespan);
        }

        /// <summary>
        /// 获取第一条数据，优先从二级缓存读取(异步)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public Task<T> GetFirstEntityFromL2CacheAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityFromL2CacheAsync(where);
        }

        /// <summary>
        /// 获取第一条数据，优先从缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public Task<T> GetFirstEntityAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityAsync(where);
        }

        /// <summary>
        /// 获取第一条数据（不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public T GetFirstEntityNoTracking(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityNoTracking(where);
        }

        /// <summary>
        /// 获取第一条数据，优先从缓存读取（不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>实体</returns>
        public T GetFirstEntityFromCacheNoTracking(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return BaseDal.GetFirstEntityFromCacheNoTracking(where, timespan);
        }

        /// <summary>
        /// 获取第一条数据，优先从二级缓存读取（不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public T GetFirstEntityFromL2CacheNoTracking(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityFromL2CacheNoTracking(where);
        }

        /// <summary>
        /// 获取第一条数据（异步，不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public Task<T> GetFirstEntityNoTrackingAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityNoTrackingAsync(where);
        }

        /// <summary>
        /// 获取第一条数据，优先从缓存读取（异步，不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>实体</returns>
        public Task<T> GetFirstEntityFromCacheNoTrackingAsync(Expression<Func<T, bool>> @where, int timespan = 30)
        {
            return BaseDal.GetFirstEntityFromCacheNoTrackingAsync(where, timespan);
        }

        /// <summary>
        /// 获取第一条数据，优先从缓存读取（异步，不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public Task<T> GetFirstEntityFromL2CacheNoTrackingAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityFromL2CacheNoTrackingAsync(where);
        }

        /// <summary>
        /// 根据ID找实体
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>实体</returns>
        public T GetById(object id)
        {
            return BaseDal.GetById(id);
        }

        /// <summary>
        /// 根据ID找实体(异步)
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>实体</returns>
        public Task<T> GetByIdAsync(object id)
        {
            return BaseDal.GetByIdAsync(id);
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
        /// <returns>还未执行的SQL语句</returns>
        public IQueryable<T> LoadPageEntities<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby,
            bool isAsc = true)
        {
            return BaseDal.LoadPageEntities(pageIndex, pageSize, out totalCount, where, orderby, isAsc);
        }

        /// <summary>
        /// 高效分页查询方法，优先从缓存读取
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="totalCount">数据总数</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>还未执行的SQL语句</returns>
        public IEnumerable<T> LoadPageEntitiesFromCache<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true, int timespan = 30)
        {
            return BaseDal.LoadPageEntitiesFromCache(pageIndex, pageSize, out totalCount, where, orderby, isAsc, timespan);
        }

        /// <summary>
        /// 高效分页查询方法，优先从二级缓存读取
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="totalCount">数据总数</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        /// <exception cref="OverflowException">
        ///         <paramref name="value" /> is less than <see cref="F:System.TimeSpan.MinValue" /> or greater than <see cref="F:System.TimeSpan.MaxValue" />.-or-<paramref name="value" /> is <see cref="F:System.Double.PositiveInfinity" />.-or-<paramref name="value" /> is <see cref="F:System.Double.NegativeInfinity" />. </exception>
        /// <exception cref="ArgumentException">
        ///         <paramref name="value" /> is equal to <see cref="F:System.Double.NaN" />. </exception>
        public IEnumerable<T> LoadPageEntitiesFromL2Cache<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true)
        {
            return BaseDal.LoadPageEntitiesFromL2Cache(pageIndex, pageSize, out totalCount, where, orderby, isAsc);
        }

        /// <summary>
        /// 高效分页查询方法（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="totalCount">数据总数</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        public IQueryable<T> LoadPageEntitiesNoTracking<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true)
        {
            return BaseDal.LoadPageEntitiesNoTracking(pageIndex, pageSize, out totalCount, where, orderby, isAsc);
        }

        /// <summary>
        /// 高效分页查询方法，优先从缓存读取（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="totalCount">数据总数</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <param name="timespan">缓存过期时间</param>
        /// <returns>还未执行的SQL语句</returns>
        public IEnumerable<T> LoadPageEntitiesFromCacheNoTracking<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true, int timespan = 30)
        {
            return BaseDal.LoadPageEntitiesFromCacheNoTracking(pageIndex, pageSize, out totalCount, where, orderby, isAsc, timespan);
        }

        /// <summary>
        /// 高效分页查询方法，优先从缓存读取（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="totalCount">数据总数</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        /// <exception cref="OverflowException">
        ///         <paramref name="value" /> is less than <see cref="F:System.TimeSpan.MinValue" /> or greater than <see cref="F:System.TimeSpan.MaxValue" />.-or-<paramref name="value" /> is <see cref="F:System.Double.PositiveInfinity" />.-or-<paramref name="value" /> is <see cref="F:System.Double.NegativeInfinity" />. </exception>
        /// <exception cref="ArgumentException">
        ///         <paramref name="value" /> is equal to <see cref="F:System.Double.NaN" />. </exception>
        public IEnumerable<T> LoadPageEntitiesFromL2CacheNoTracking<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true)
        {
            return BaseDal.LoadPageEntitiesFromL2CacheNoTracking(pageIndex, pageSize, out totalCount, where, orderby, isAsc);
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>删除成功</returns>
        public bool DeleteById(object id)
        {
            return BaseDal.DeleteById(id);
        }

        /// <summary>
        /// 根据ID删除实体并保存
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>删除成功</returns>
        public bool DeleteByIdSaved(object id)
        {
            BaseDal.DeleteById(id);
            return SaveChanges() > 0;
        }

        /// <summary>
        /// 根据ID删除实体并保存（异步）
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>删除成功</returns>
        public Task<int> DeleteByIdSavedAsync(object id)
        {
            BaseDal.DeleteById(id);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 删除实体并保存
        /// </summary>
        /// <param name="t">需要删除的实体</param>
        /// <returns>删除成功</returns>
        public bool DeleteEntity(T t)
        {
            return BaseDal.DeleteEntity(t);
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>删除成功</returns>
        public int DeleteEntity(Expression<Func<T, bool>> @where)
        {
            return BaseDal.DeleteEntity(where);
        }

        /// <summary>
        /// 根据条件删除实体（异步）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>删除成功</returns>
        public Task<int> DeleteEntityAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.DeleteEntityAsync(where);
        }

        /// <summary>
        /// 删除实体并保存
        /// </summary>
        /// <param name="t">需要删除的实体</param>
        /// <returns>删除成功</returns>
        public bool DeleteEntitySaved(T t)
        {
            BaseDal.DeleteEntity(t);
            return SaveChanges() > 0;
        }

        /// <summary>
        /// 删除实体并保存（异步）
        /// </summary>
        /// <param name="t">需要删除的实体</param>
        /// <returns>删除成功</returns>
        public Task<int> DeleteEntitySavedAsync(T t)
        {
            BaseDal.DeleteEntity(t);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t">更新后的实体</param>
        /// <returns>更新成功</returns>
        public bool UpdateEntity(T t)
        {
            return BaseDal.UpdateEntity(t);
        }

        /// <summary>
        /// 更新实体并保存
        /// </summary>
        /// <param name="t">更新后的实体</param>
        /// <returns>更新成功</returns>
        public bool UpdateEntitySaved(T t)
        {
            BaseDal.UpdateEntity(t);
            return SaveChanges() > 0;
        }

        /// <summary>
        /// 更新实体并保存（异步）
        /// </summary>
        /// <param name="t">更新后的实体</param>
        /// <returns>更新成功</returns>
        public Task<int> UpdateEntitySavedAsync(T t)
        {
            BaseDal.UpdateEntity(t);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 根据条件更新实体
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="t">更新后的实体</param>
        /// <returns>更新成功</returns>
        public int UpdateEntity(Expression<Func<T, bool>> @where, T t)
        {
            return BaseDal.UpdateEntity(where, t);
        }

        /// <summary>
        /// 根据条件更新实体（异步）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="t">更新后的实体</param>
        /// <returns>更新成功</returns>
        public Task<int> UpdateEntityAsync(Expression<Func<T, bool>> @where, T t)
        {
            return BaseDal.UpdateEntityAsync(where, t);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="t">需要添加的实体</param>
        /// <returns>添加成功</returns>
        public T AddEntity(T t)
        {
            return BaseDal.AddEntity(t);
        }

        /// <summary>
        /// 添加实体并保存
        /// </summary>
        /// <param name="t">需要添加的实体</param>
        /// <returns>添加成功</returns>
        public T AddEntitySaved(T t)
        {
            T entity = BaseDal.AddEntity(t);
            bool b = SaveChanges() > 0;
            return b ? entity : null;
        }

        /// <summary>
        /// 添加实体并保存（异步）
        /// </summary>
        /// <param name="t">需要添加的实体</param>
        /// <returns>添加成功</returns>
        public Task<int> AddEntitySavedAsync(T t)
        {
            BaseDal.AddEntity(t);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 统一保存的方法
        /// </summary>
        /// <returns>受影响的行数</returns>
        public int SaveChanges()
        {
            return BaseDal.SaveChanges();
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        /// <returns>受影响的行数</returns>
        public Task<int> SaveChangesAsync()
        {
            return BaseDal.SaveChangesAsync();
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        public void BulkSaveChanges()
        {
            BaseDal.BulkSaveChanges();
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        public void BulkSaveChangesAsync()
        {
            BaseDal.BulkSaveChangesAsync();
        }

        /// <summary>
        /// 判断实体是否在数据库中存在
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>是否存在</returns>
        public bool Any(Expression<Func<T, bool>> @where)
        {
            return BaseDal.Any(where);
        }

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>删除成功</returns>
        public bool DeleteEntities(IEnumerable<T> list)
        {
            return BaseDal.DeleteEntities(list);
        }

        /// <summary>
        /// 删除多个实体并保存
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>删除成功</returns>
        public bool DeleteEntitiesSaved(IEnumerable<T> list)
        {
            BaseDal.DeleteEntities(list);
            return SaveChanges() > 0;
        }

        /// <summary>
        /// 删除多个实体并保存（异步）
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>删除成功</returns>
        public Task<int> DeleteEntitiesSavedAsync(IEnumerable<T> list)
        {
            BaseDal.DeleteEntities(list);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>更新成功</returns>
        public bool UpdateEntities(IEnumerable<T> list)
        {
            return BaseDal.UpdateEntities(list);
        }

        /// <summary>
        /// 更新多个实体并保存
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>更新成功</returns>
        public bool UpdateEntitiesSaved(IEnumerable<T> list)
        {
            BaseDal.UpdateEntities(list);
            return SaveChanges() > 0;
        }

        /// <summary>
        /// 更新多个实体并保存（异步）
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>更新成功</returns>
        public Task<int> UpdateEntitiesSavedAsync(IEnumerable<T> list)
        {
            BaseDal.UpdateEntities(list);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 添加多个实体并保存
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>添加成功</returns>
        public IEnumerable<T> AddEntities(IList<T> list)
        {
            IEnumerable<T> entities = BaseDal.AddEntities(list);
            SaveChanges();
            return entities;
        }

        /// <summary>
        /// 添加多个实体并保存（异步）
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>添加成功</returns>
        public IEnumerable<T> AddEntitiesAsync(IList<T> list)
        {
            IEnumerable<T> entities = BaseDal.AddEntities(list);
            SaveChangesAsync();
            return entities;
        }
    }
}