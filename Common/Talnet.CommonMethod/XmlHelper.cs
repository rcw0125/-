using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Talent.CommonMethod
{
    /// <summary>
    /// xml帮助类
    /// </summary>
    public class XmlHelper
    {
        private static string fileMainPath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig";//@"D:\work\远程计量\计量改版程序新\Talent.Measure.Solution\Talent.RemoteCarMeasure\bin\Debug";

        /// <summary>
        /// 将对象写入到xml文件中
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool WriteXmlFile<T>(string fileName,T obj)
        {
            try
            {
                string filePath = fileMainPath + "\\" + fileName;
                if (!Directory.Exists(fileMainPath))
                {
                    Directory.CreateDirectory(fileMainPath);
                }
                XmlSerializer configSer = new XmlSerializer(typeof(T));
                StreamWriter sw = new StreamWriter(filePath);
                XmlWriter writer = new XmlTextWriter(sw);
                configSer.Serialize(writer, obj);
                writer.Close();
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将xml文件序列化为对象
        /// </summary>
        /// <param name="fileName">带绝对路径的文件名称</param>
        /// <returns></returns>
        public static T ReadXmlToObj<T>(string fileName)
        {
            try
            {
                XmlSerializer configSer = new XmlSerializer(typeof(T));
                if (!File.Exists(fileName))
                {
                    return default(T);
                }
                using (StreamReader sr = new StreamReader(File.OpenRead(fileName)))
                {
                    return (T)configSer.Deserialize(sr);
                }
            }
            catch
            {
                throw new Exception("文件:【" + fileName + "】读取异常!");
            }
        }
    }
}
