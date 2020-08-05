using log4net;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yzbcore.Bussiness
{
    public static class Util
    {
        static DateTime unixStartTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区unix开始时间
        public static int ToInt(this object o, int defaultValue = 0)
        {
            int retValue;
            //o为null 或者转换失败返回默认值
            retValue = o == null || !int.TryParse(o.ToString(), out retValue) ? defaultValue : retValue;
            return retValue;
        }
        public static float ToFloat(this object o, float defaultValue = 0f)
        {
            float retValue;
            //o为null 或者转换失败返回默认值
            retValue = o == null || !float.TryParse(o.ToString(), out retValue) ? defaultValue : retValue;
            return retValue;
        }
        /// <summary>
        /// 将unix时间戳转化为datetime
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime FromUnixStamp(int unixTimeStamp)
        {
            DateTime dt = unixStartTime.AddSeconds(unixTimeStamp);
            return dt;
        }

        /// <summary>
        /// 将datetime转化为unix时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int ToUnixStamp(DateTime dt)
        {
            int timeStamp = (int)(dt - unixStartTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }
    }
    public class Log4NetRepository
    {
        public static ILoggerRepository loggerRepository { get; set; }
    }
    /// <summary>
    /// log4net帮助类
    /// AdoNetAppender仅支持到.net framework4.5，不支持在.net core项目中持久化日志到数据库
    /// </summary>
    public class LogHelper
    {
        // 异常 // 注意：logger name不要写错
        private static readonly ILog logerror = LogManager.GetLogger(Log4NetRepository.loggerRepository.Name, "errLog");
        // 记录
        private static readonly ILog loginfo = LogManager.GetLogger(Log4NetRepository.loggerRepository.Name, "infoLog");

        public static void Error(string throwMsg)
        {
            //string errorMsg = string.Format("【异常描述】：{0} <br>【异常类型】：{1} <br>【异常信息】：{2} <br>【堆栈调用】：{3}",
            //    new object[] {
            //        throwMsg,
            //        ex.GetType().Name,
            //        ex.Message,
            //        ex.StackTrace });
            //errorMsg = errorMsg.Replace("\r\n", "<br>");
            logerror.Error(throwMsg);
        }

        public static void Info(string message)
        {
            loginfo.Info(string.Format("【日志信息】：{0}", message));
        }

    }
}
