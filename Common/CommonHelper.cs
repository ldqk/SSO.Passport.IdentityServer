using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Common
{
    public static class CommonHelper
    {
        /// <summary>
        /// 类型映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Mapper<T>(this object source) where T : class => AutoMapper.Mapper.Map<T>(source);

        #region 性能历史数据

        public static List<object[]> HistoryCpuLoad { get; set; } = new List<object[]>();
        public static List<object[]> HistoryMemoryUsage { get; set; } = new List<object[]>();
        public static List<object[]> HistoryCpuTemp { get; set; } = new List<object[]>();
        public static List<object[]> HistoryIORead { get; set; } = new List<object[]>();
        public static List<object[]> HistoryIOWrite { get; set; } = new List<object[]>();
        public static List<object[]> HistoryNetSend { get; set; } = new List<object[]>();
        public static List<object[]> HistoryNetReceive { get; set; } = new List<object[]>();
        public static DateTime StartupTime { get; set; } = DateTime.Now;

        #endregion
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, string strname)
        {
            var dateExpr = Expression.Parameter(typeof(T));
            var parameterReplacer = new ParameterReplacer(dateExpr);
            var leftwhere = parameterReplacer.Replace(left.Body);
            var rightwhere = parameterReplacer.Replace(right.Body);
            var body = Expression.And(leftwhere, rightwhere);
            return Expression.Lambda<Func<T, bool>>(body, dateExpr);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, string strname)
        {
            var dateExpr = Expression.Parameter(typeof(T));
            var leftwhere = left.Body;
            var rightwhere = right.Body;
            var body = Expression.Or(leftwhere, rightwhere);
            return Expression.Lambda<Func<T, bool>>(body, dateExpr);
        }
    }
}