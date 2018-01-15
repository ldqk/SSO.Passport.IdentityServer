using System.Collections.Generic;

namespace Common
{
    public class CommonHelper
    {
        #region 性能历史数据

        public static List<object[]> HistoryCpuLoad { get; set; } = new List<object[]>();
        public static List<object[]> HistoryMemoryUsage { get; set; } = new List<object[]>();
        public static List<object[]> HistoryCpuTemp { get; set; } = new List<object[]>();
        public static List<object[]> HistoryIORead { get; set; } = new List<object[]>();
        public static List<object[]> HistoryIOWrite { get; set; } = new List<object[]>();
        public static List<object[]> HistoryNetSend { get; set; } = new List<object[]>();
        public static List<object[]> HistoryNetReceive { get; set; } = new List<object[]>();

        #endregion
    }
}