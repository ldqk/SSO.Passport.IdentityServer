using System;
using System.Collections.Generic;

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
    }
}