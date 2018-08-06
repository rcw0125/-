using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talent.ClinetLog
{
    /// <summary>
    /// 日志辅助类
    /// </summary>
    public class LogHelper
    {
        private static ILog log;
        private static LogHelper logHelper = null;
        ///// <summary>
        ///// 初始化
        ///// </summary>
        ///// <returns></returns>
        //public static ILog GetInstance()
        //{
        //    logHelper = new LogHelper(null);
        //    return log;
        //}
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static ILog GetInstance(string configPath,string pName)
        {
            logHelper = new LogHelper(configPath,pName);
            return log;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configPath"></param>
        private LogHelper(string configPath,string pName)
        {
            if (!string.IsNullOrEmpty(configPath))
            {
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(configPath));
                log = log4net.LogManager.GetLogger(pName);
                //log.Info("OK");
            }
            else
            {
                log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
    }
}
