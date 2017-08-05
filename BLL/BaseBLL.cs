using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DALFactory;
using IBLL;
using IDAL;

namespace BLL
{
    public class BaseBll<T> : IBaseBll<T> where T : class, new()
    {
        public IBaseDal<T> BaseDal { get; set; } = Factory.CreateInstance<IBaseDal<T>>("DAL." + typeof(T).Name + "Dal");

        public IQueryable<T> LoadEntities(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntities(where);
        }

        /// <summary>
        /// 基本查询方法
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<IQueryable<T>> LoadEntitiesAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesAsync(where);
        }

        /// <summary>
        /// 基本查询方法（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<T> LoadEntitiesNoTracking(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesNoTracking(where);
        }

        /// <summary>
        /// 基本查询方法（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<IQueryable<T>> LoadEntitiesNoTrackingAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.LoadEntitiesNoTrackingAsync(where);
        }

        /// <summary>
        /// 根据ID找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById(object id)
        {
            return BaseDal.GetById(id);
        }

        /// <summary>
        /// 根据ID找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T> GetByIdAsync(object id)
        {
            return BaseDal.GetByIdAsync(id);
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T GetFirstEntity(Expression<Func<T, bool>> @where)
        {
            return LoadEntities(where).FirstOrDefault();
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<T> GetFirstEntityAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityAsync(where);
        }

        /// <summary>
        /// 获取第一条数据（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T GetFirstEntityNoTracking(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityNoTracking(where);
        }

        /// <summary>
        /// 获取第一条数据（不跟踪实体）
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<T> GetFirstEntityNoTrackingAsync(Expression<Func<T, bool>> @where)
        {
            return BaseDal.GetFirstEntityNoTrackingAsync(where);
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
        public IQueryable<T> LoadPageEntities<TS>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby,
            bool isAsc)
        {
            return BaseDal.LoadPageEntities(pageIndex, pageSize, out totalCount, where, orderby, isAsc);
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
            return BaseDal.LoadPageEntitiesNoTracking(pageIndex, pageSize, out totalCount, where, orderby, isAsc);
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById(object id)
        {
            return BaseDal.DeleteById(id);
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteByIdSaved(object id)
        {
            BaseDal.DeleteById(id);
            return SaveChanges() > 0;
        }
        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> DeleteByIdSavedAsync(object id)
        {
            BaseDal.DeleteById(id);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool DeleteEntity(T t)
        {
            return BaseDal.DeleteEntity(t);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool DeleteEntitySaved(T t)
        {
            BaseDal.DeleteEntity(t);
            return SaveChanges() > 0;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Task<int> DeleteEntitySavedAsync(T t)
        {
            BaseDal.DeleteEntity(t);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool UpdateEntity(T t)
        {
            return BaseDal.UpdateEntity(t);
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool UpdateEntitySaved(T t)
        {
            BaseDal.UpdateEntity(t);
            return SaveChanges() > 0;
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Task<int> UpdateEntitySavedAsync(T t)
        {
            BaseDal.UpdateEntity(t);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T AddEntity(T t)
        {
            return BaseDal.AddEntity(t);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T AddEntitySaved(T t)
        {
            T entity = BaseDal.AddEntity(t);
            bool b = SaveChanges() > 0;
            return b ? entity : null;
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Task<int> AddEntitySavedAsync(T t)
        {
            BaseDal.AddEntity(t);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 统一保存的方法
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return BaseDal.SaveChanges();
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveChangesAsync()
        {
            return BaseDal.SaveChangesAsync();
        }

        /// <summary>
        /// 判断实体是否在数据库中存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<T, bool>> @where)
        {
            return BaseDal.Any(where);
        }

        /// <summary>
        /// 删除多个实体并保存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool DeleteEntities(IEnumerable<T> list)
        {
            return BaseDal.DeleteEntities(list);
        }

        /// <summary>
        /// 删除多个实体并保存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool DeleteEntitiesSaved(IEnumerable<T> list)
        {
            BaseDal.DeleteEntities(list);
            return SaveChanges() > 0;
        }

        /// <summary>
        /// 删除多个实体并保存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Task<int> DeleteEntitiesSavedAsync(IEnumerable<T> list)
        {
            BaseDal.DeleteEntities(list);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpdateEntities(IEnumerable<T> list)
        {
            return BaseDal.UpdateEntities(list);
        }

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpdateEntitiesSaved(IEnumerable<T> list)
        {
            BaseDal.UpdateEntities(list);
            return SaveChanges() > 0;
        }

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Task<int> UpdateEntitiesSavedAsync(IEnumerable<T> list)
        {
            BaseDal.UpdateEntities(list);
            return SaveChangesAsync();
        }

        /// <summary>
        /// 添加多个实体并保存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<T> AddEntities(IEnumerable<T> list)
        {
            IEnumerable<T> entities = BaseDal.AddEntities(list);
            SaveChanges();
            return entities;
        }

        /// <summary>
        /// 添加多个实体并保存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<T> AddEntitiesAsync(IEnumerable<T> list)
        {
            IEnumerable<T> entities = BaseDal.AddEntities(list);
            SaveChangesAsync();
            return entities;
        }
    }
}