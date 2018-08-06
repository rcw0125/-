using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.ClinetLog
{
    /// <summary>
    /// 系统日志
    /// added by wangc on 20151111
    /// </summary>
    public class SysLog
    {
        private static string _logpath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\Log4Net.config";
        public static ILog log;

        public static void SetConfigPath(string configPath)
        {
            if (!string.IsNullOrEmpty(configPath))
            {
                _logpath = configPath;
            }
        }

        /// <summary>
        /// 写正常级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string jsonMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, "SYS_LOG");
            }
            log.Info(jsonMsg);
        }
    }

    /// <summary>
    /// 实时数据日志
    /// </summary>
    public class IwdLog
    {
        private static string _logpath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\Log4Net.config";
        public static ILog log;
        /// <summary>
        /// 设置配置文件路径，默认为运行目录“\\ClientConfig\\Log4Net.config”文件
        /// </summary>
        /// <param name="configPath"></param>
        public static void SetConfigPath(string configPath)
        {
            if (!string.IsNullOrEmpty(configPath))
            {
                _logpath = configPath;
            }
        }

        /// <summary>
        /// 写正常级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string jsonMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, "IWD_LOG");
            }
            log.Info(jsonMsg);
        }
    }

    /// <summary>
    /// 称重量日志
    /// </summary>
    public class WeightLog
    {
        private static string _logpath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\Log4Net.config";
        public static ILog log;

        /// <summary>
        /// 设置配置文件路径，默认为运行目录“\\ClientConfig\\Log4Net.config”文件
        /// </summary>
        /// <param name="configPath"></param>
        public static void SetConfigPath(string configPath)
        {
            if (!string.IsNullOrEmpty(configPath))
            {
                _logpath = configPath;
            }
        }

        /// <summary>
        /// 写正常级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string jsonMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, "WEIGHT_LOG");
            }
            log.Info(jsonMsg);
        }
    }

    /// <summary>
    /// 时间日志
    /// </summary>
    public class TimeLog
    {
        private static string _logpath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\Log4Net.config";
        public static ILog log;

        public static void SetConfigPath(string configPath)
        {
            if (!string.IsNullOrEmpty(configPath))
            {
                _logpath = configPath;
            }
        }

        /// <summary>
        /// 写正常级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string jsonMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, "TIME_LOG");
            }
            log.Info(jsonMsg);
        }
    }
}
