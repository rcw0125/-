using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.ClinetLog
{
    /// <summary>
    /// IC卡读写器日志记录器
    /// </summary>
    public class IcCardReaderLogger
    {
        private static string _loggerName = "IC_CARD_READER_LOGGER";
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
        public static void Debug(string pMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Info(pMsg);
        }
        /// <summary>
        /// 写正常级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Error(pMsg);
        }
        /// <summary>
        /// 写正常级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg,Exception ex)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Error(pMsg,ex);
        }
    }

    /// <summary>
    /// 衡器日志记录器
    /// </summary>
    public class WeightDeviceLogger
    {
        private static string _loggerName = "WEIGHT_DEVICE_LOGGER";
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
        public static void Debug(string pMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Info(pMsg);
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Error(pMsg);
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg, Exception ex)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Error(pMsg, ex);
        }
    }

    /// <summary>
    /// RFID日志记录器
    /// </summary>
    public class RFIDReaderLogger
    {
        private static string _loggerName = "RFID_READER_LOGGER";
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
        public static void Debug(string pMsg,string pDeviceId)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.DebugFormat("[{0}] {1}", pDeviceId, pMsg); 
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg, string pDeviceId)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.ErrorFormat("[{0}] {1}", pDeviceId, pMsg);
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg,string pDeviceId, Exception ex)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.ErrorFormat("[{0}] {1}", pDeviceId, pMsg);
        }
    }

    /// <summary>
    /// 打印机日志记录器
    /// </summary>
    public class PrinterDevLogger
    {
        private static string _loggerName = "PRINTER_LOG";
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
        public static void Debug(string pMsg, string pDeviceId)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.DebugFormat("[{0}] {1}", pDeviceId, pMsg);
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg, string pDeviceId)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.ErrorFormat("[{0}] {1}", pDeviceId, pMsg);
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg, string pDeviceId, Exception ex)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.ErrorFormat("[{0}] {1}", pDeviceId, pMsg);
        }
    }

    /// <summary>
    /// IC卡读写器日志记录器
    /// </summary>
    public class VideoPlayBackLogger
    {
        private static string _loggerName = "VIDEO_PLAYBACK_LOGGER";
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
        public static void Debug(string pMsg, string pDeviceId)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.DebugFormat("[{0}] {1}", pDeviceId, pMsg);
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg, string pDeviceId)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.ErrorFormat("[{0}] {1}", pDeviceId, pMsg);
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg, string pDeviceId, Exception ex)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.ErrorFormat("[{0}] {1}", pDeviceId, pMsg);
        }
    }

    /// <summary>
    /// 视频回放日志
    /// </summary>
    public class VideoPlayLogger
    {
        private static string _loggerName = "VIDEO_PLAY_LOGGER";
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
        public static void Debug(string pMsg, string pDeviceId)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.DebugFormat("[{0}] {1}", pDeviceId, pMsg);
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg, string pDeviceId)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.ErrorFormat("[{0}] {1}", pDeviceId, pMsg);
        }
        /// <summary>
        /// 写错误级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg, string pDeviceId, Exception ex)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Error(string.Format("[{0}] {1}", pDeviceId, pMsg), ex);
        }
    }
    /// <summary>
    /// 安国防作弊
    /// </summary>
    public class AGCheatLogger
    {
        private static string _loggerName = "AG_CHEAT_LOGGER";
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
        public static void Debug(string pMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Info(pMsg);
        }
        /// <summary>
        /// 写正常级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Error(pMsg);
        }
        /// <summary>
        /// 写正常级别的日志信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string pMsg, Exception ex)
        {
            if (log == null)
            {
                log = LogHelper.GetInstance(_logpath, _loggerName);
            }
            log.Error(pMsg, ex);
        }
    }
}
