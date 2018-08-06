using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talent.CommonMethod
{
    /// <summary>
    /// jeson操作帮助类
    /// </summary>
    public class JesonOperateHelper
    {
        private static string jesonFileMainPath = AppDomain.CurrentDomain.BaseDirectory + "\\ClientConfig";//@"D:\work\远程计量\计量改版程序新\Talent.Measure.Solution\Talent.RemoteCarMeasure\bin\Debug";//Application.ExecutablePath;

        /// <summary>
        /// 读取jeson文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadJesonFile(string fileName)
        {
            string filePath = jesonFileMainPath + "//" + fileName;
            if (File.Exists(filePath))
            {
                StreamReader sr = File.OpenText(filePath);
                StringBuilder jsonArrayText_tmp = new StringBuilder();
                string input = null;
                while ((input = sr.ReadLine()) != null)
                {
                    jsonArrayText_tmp.Append(input);
                }
                sr.Close();
                jsonArrayText_tmp = jsonArrayText_tmp.Replace(@"\\", @"\");
                jsonArrayText_tmp = jsonArrayText_tmp.Replace(@"\\", @"\");
                string jesonStr = jsonArrayText_tmp.Replace(" ", "").ToString();
                jesonStr = jesonStr.Substring(2);
                jesonStr = jesonStr.Substring(0, jesonStr.Length - 2);
                return jesonStr;
            }
            return string.Empty;
        }

        /// <summary>
        /// 写入Jeson字符串到文件中
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="jesonStr">jeson字符串</param>
        /// <returns></returns>
        public static bool WriteJesonFile(string fileName, string jesonStr)
        {
            try
            {
                string filePath = jesonFileMainPath + "//" + fileName;
                if (!Directory.Exists(jesonFileMainPath))
                {
                    Directory.CreateDirectory(jesonFileMainPath);
                }
                //JsonSerializer serializer = new JsonSerializer();
                //serializer.Converters.Add(new JavaScriptDateTimeConverter());
                //serializer.NullValueHandling = NullValueHandling.Ignore; 

                StreamWriter sw = new StreamWriter(filePath);
                JsonWriter writer = new JsonTextWriter(sw);

                //把模型数据序列化并写入Json.net的JsonWriter流中   
                //serializer.Serialize(writer, jesonStr);
                writer.WriteComment(jesonStr);

                writer.Close();
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
