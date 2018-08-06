using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Talent.CommonMethod;

namespace Talent.RemoteCarMeasure.Commom
{
     public class AppConfigReader
    {
         /// <summary>
         /// app配置文件路径
         /// </summary>
         internal static string _configUrl = System.AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["SysConfigFileName"].ToString();

         /// <summary>
         /// 读取app配置
         /// </summary>
         /// <param name="pConfigName"></param>
         /// <returns></returns>
         internal static string ReadCfg(string pConfigName)
         {
             string temp = ConfigurationManager.AppSettings[pConfigName].ToString();
             return XpathHelper.GetValue(_configUrl, temp);
         }
     }
}
