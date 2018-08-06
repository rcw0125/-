using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace Talent.CommonMethod
{
    public class CommonTranslationHelper
    {
        private const string sKey = "SysADMIN";

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="orgStr"></param>
        /// <returns></returns>
        public static string MD5(string orgStr)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(orgStr));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转换成MD5
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToMD5(string pToEncrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }
        ///MD5解密
        public static string MD5Decrypt(string pToDecrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
        /// <summary>
        /// 整型转换
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt(object obj)
        {
            if (obj == null) { return 0; }
            int r = 0;
            try
            {
                r = Convert.ToInt32(obj);
            }
            catch
            {
                r = 0;
            }
            return r;
        }



        public static string ToEmptyDouble(object obj, int m)
        {
            if (obj == null) { return ""; }
            string r = string.Empty;
            try
            {
                double @double = Convert.ToDouble(obj);
                r = (@double * m).ToString();

            }
            catch
            {
                r = "";
            }
            return r;
        }



        /// <summary>
        /// Double 转换 (SQL float = double)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ToDouble(object obj)
        {
            if (obj == null) { return 0; }
            double r = 0;
            try
            {
                r = Convert.ToDouble(obj);
            }
            catch
            {
                r = 0;
            }
            return r;
        }
        /// <summary>
        /// Decimal 转换 (SQL float = decimal)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ToDecimal(object obj)
        {
            if (obj == null) { return 0; }
            decimal r = 0;
            try
            {
                r = Convert.ToDecimal(obj);
            }
            catch
            {
                r = 0;
            }
            return r;
        }
        /// <summary>
        /// DateTime 转换 输入类型(System.String,System.DateTime,System.Int64,long)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(object obj)
        {
            DateTime time = DateTime.Now;
            string typename = obj.GetType().FullName;
            try
            {
                switch (typename)
                {
                    case "System.String":
                        time = DateTime.Parse((string)obj);
                        break;
                    case "System.DateTime":
                        time = (DateTime)obj;
                        break;
                    case "System.Int64":
                        time = new DateTime((long)(obj));
                        break;
                    default:
                        time = DateTime.Now;
                        break;
                }
            }
            catch
            {
                time = DateTime.Now;
            }
            if (time == DateTime.MaxValue) { time = DateTime.Now; }
            return time;
        }
        /// <summary>
        /// HTML输入编码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string EnHTML(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }
            string temp = html;
            //请勿修改编码顺序
            //temp = temp.Replace("$G$S$ROOT$", "");
            temp = temp.Replace("&", "&amp;");//表单编码
            temp = temp.Replace("<", "&lt;");
            temp = temp.Replace(">", "&gt;");
            //SQL注入编码 ',---
            temp = temp.Replace("'", "&#39;");
            temp = temp.Replace("\"", "&quot;");
            return temp;
        }
        /// <summary>
        /// HTML输出解码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string DeHTML(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }
            string temp = html;
            //请勿修改解码顺序
            temp = temp.Replace("&quot;", "\"");
            temp = temp.Replace("&#39;", "'");
            temp = temp.Replace("&gt;", ">");
            temp = temp.Replace("&lt;", "<");
            temp = temp.Replace("&amp;", "&");
            return temp;
        }
        /// <summary>
        /// 构造日记查询参数表
        /// </summary>
        public static string BuildDateWhere(string key, string startdate, string enddate)
        {
            return BuildDateWhere(key, ToDateTime(startdate), ToDateTime(enddate));
        }
        /// <summary>
        /// 构造日记查询参数表
        /// </summary>
        /// <returns></returns>
        public static string BuildDateWhere(string key, DateTime startDate, DateTime endDate)
        {
            if (startDate == DateTime.MinValue && endDate == DateTime.MinValue)
            {
                return "";
            }
            else if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                DateTime temp = DateTime.MinValue;
                if (startDate > endDate)
                {
                    temp = startDate;
                    startDate = endDate;
                    endDate = temp;
                }
                return string.Format(" ( {0} between '{1:yyyy-MM-dd HH:mm:ss}' and '{2:yyyy-MM-dd HH:mm:ss}' )", key, startDate, endDate);
            }
            else if (startDate != DateTime.MinValue)
            {
                return string.Format(" ( {0} >= '{1:yyyy-MM-dd HH:mm:ss}' )", key, startDate);
            }
            else
            {
                return string.Format(" ( {0} <= '{1:yyyy-MM-dd HH:mm:ss}' )", key, endDate);
            }
        }
        public static string ParsebyBT8170(object number, int length)
        {

            double d;
            if (double.TryParse(number.ToString(), out d))
            {
                return ParsebyBT8170(d, length);
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 通过国标转换
        /// </summary>
        /// <param name="number">数值</param>
        /// <param name="length">精确度</param>
        /// <returns></returns>
        public static string ParsebyBT8170(double number, int length)
        {

            string n = number.ToString();



            if (string.IsNullOrEmpty(n))
            {
                return "0";
            }




            if (n.Substring(n.LastIndexOf(".") + 1).Length > length + 1)
            {
                return number.ToString("F" + length);
            }
            string Format = "F" + (length + 1);
            string f = number.ToString(Format);
            if (length == 0)
            {
                int z1 = int.Parse(f.Substring(f.Length - 1, 1));
                int d1 = int.Parse(f.Substring(f.Length - 3, 1));


                if (z1 == 5 && d1 % 2 == 0)
                {
                    f = f.Substring(0, f.Length - 2);
                }
                else
                {
                    f = number.ToString("F" + length);
                }

                return f;


            }


            int z = int.Parse(f.Substring(f.Length - 1));
            int d = int.Parse(f.Substring(f.Length - 2, 1));

            if (z == 5 && d % 2 == 0)
            {
                f = f.Substring(0, f.Length - 1);
            }
            else
            {
                f = number.ToString("F" + length);
            }

            return f;

        }


        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="level">标准</param>
        /// <param name="number"></param>
        /// <returns>0 等于 -1小于 1大于</returns>
        public static int CompareNUM(double level, double number)
        {
            return CompareNUM((decimal)level, (decimal)number);


        }


        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="level">标准</param>
        /// <param name="number"></param>
        /// <returns>0 等于 -1小于 1大于</returns>
        public static int CompareNUM(decimal level, decimal number, string id = "")
        {
            string levelstr = level.ToString();
            string numstr = "";
            int p = -1;

            if (id == "1344")
            {
                numstr = number.ToString("F2");
            }
            else if (id == "1347")
            {
                numstr = CommonTranslationHelper.ParsebyBT8170(number, 1).ToString();
            }
            else
            {

                if ((p = levelstr.LastIndexOf(".")) == -1)
                {
                    numstr = number.ToString("F0");
                }
                else
                {
                    numstr = number.ToString("F" + levelstr.Substring(levelstr.LastIndexOf(".") + 1).Length);
                }
            }
            decimal newnum = decimal.Parse(numstr);

            int result = 0;

            if (newnum > level)
            {
                result = 1;
            }
            else if (newnum < level)
            {
                result = -1;
            }
            return result;


        }

        /// <summary>
        /// 允差为0的比较
        /// </summary>
        /// <param name="level">标准</param>
        /// <param name="number"></param>
        /// <returns>0 等于 -1小于 1大于</returns>
        public static int CompareNUM1342(decimal level, decimal number)
        {
            string levelstr = level.ToString();
            string numstr = "";
            numstr = number.ToString();
            decimal newnum = decimal.Parse(numstr);
            int result = 0;

            if (newnum > level)
            {
                result = 1;
            }
            else if (newnum < level)
            {
                result = -1;
            }
            return result;
        }


        public static string GetJsonByDataset(DataSet ds)
        {
            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
            {
                //如果查询到的数据为空则返回标记ok:false
                return "{\"ok\":false}";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"ok\":true,");
            foreach (DataTable dt in ds.Tables)
            {
                sb.Append(string.Format("\"{0}\":[", dt.TableName));

                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("{");
                    for (int i = 0; i < dr.Table.Columns.Count; i++)
                    {
                        sb.AppendFormat("\"{0}\":\"{1}\",", dr.Table.Columns[i].ColumnName.Replace("\"", "\\\"").Replace("\'", "\\\'"), ObjToStr(dr[i]).Replace("\"", "\\\"").Replace("\'", "\\\'")).Replace(Convert.ToString((char)13), "\\r\\n").Replace(Convert.ToString((char)10), "\\r\\n");
                    }
                    sb.Remove(sb.ToString().LastIndexOf(','), 1);
                    sb.Append("},");
                }

                sb.Remove(sb.ToString().LastIndexOf(','), 1);
                sb.Append("],");
            }
            sb.Remove(sb.ToString().LastIndexOf(','), 1);
            sb.Append("}");
            return sb.ToString();
        }

        public static string ObjToStr(object ob)
        {
            if (ob == null)
            {
                return string.Empty;
            }
            else
                return ob.ToString();
        }
        public static object JsonToObject(string jsonString, object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return serializer.ReadObject(mStream);
            //Newtonsoft.Json
        }
        #region 补足位数
        /// <summary>
        /// 指定字符串的固定长度，如果字符串小于固定长度，
        /// 则在字符串的前面补足零，可设置的固定长度最大为9位
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="limitedLength">字符串的固定长度</param>
        public static string RepairZero(string text, int limitedLength)
        {
            //补足0的字符串
            string temp = "";

            //补足0
            for (int i = 0; i < limitedLength - text.Length; i++)
            {
                temp += "0";
            }

            //连接text
            temp += text;

            //返回补足0的字符串
            return temp;
        }
        #endregion
        #region 各进制数间转换
        /// <summary>
        /// 实现各进制数间的转换。ConvertBase("15",10,16)表示将十进制数15转换为16进制的数。
        /// </summary>
        /// <param name="value">要转换的值,即原值</param>
        /// <param name="from">原值的进制,只能是2,8,10,16四个值。</param>
        /// <param name="to">要转换到的目标进制，只能是2,8,10,16四个值。</param>
        public static string ConvertBase(string value, int from, int to)
        {
            try
            {
                int intValue = Convert.ToInt32(value, from);  //先转成10进制
                string result = Convert.ToString(intValue, to);  //再转成目标进制
                if (to == 2)
                {
                    int resultLength = result.Length;  //获取二进制的长度
                    switch (resultLength)
                    {
                        case 7:
                            result = "0" + result;
                            break;
                        case 6:
                            result = "00" + result;
                            break;
                        case 5:
                            result = "000" + result;
                            break;
                        case 4:
                            result = "0000" + result;
                            break;
                        case 3:
                            result = "00000" + result;
                            break;
                    }
                }
                return result;
            }
            catch //(Exception ex)
            {

                return "0";
            }
        }
        #endregion



        /// <summary>
        /// 使用指定字符集将string转换成byte[]
        /// </summary>
        /// <param name="text">要转换的字符串</param>
        /// <param name="encoding">字符编码</param>
        public static byte[] StringToBytes(string text, Encoding encoding)
        {
            return encoding.GetBytes(text);
        }


        #region 使用指定字符集将byte[]转换成string

        /// <summary>
        /// 使用指定字符集将byte[]转换成string
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <param name="encoding">字符编码</param>
        public static string BytesToString(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }
        #endregion
        #region 判断对象是否为空
        /// <summary>
        /// 判断对象是否为空，为空返回true
        /// </summary>
        /// <typeparam name="T">要验证的对象的类型</typeparam>
        /// <param name="data">要验证的对象</param>        
        public static bool IsNullOrEmpty<T>(T data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //如果为DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不为空
            return false;
        }

        /// <summary>
        /// 判断对象是否为空，为空返回true
        /// </summary>
        /// <param name="data">要验证的对象</param>
        public static bool IsNullOrEmpty(object data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //如果为DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不为空
            return false;
        }
        #endregion
        #region 将byte[]转换成int
        /// <summary>
        /// 将byte[]转换成int
        /// </summary>
        /// <param name="data">需要转换成整数的byte数组</param>
        public static int BytesToInt32(byte[] data)
        {
            //如果传入的字节数组长度小于4,则返回0
            if (data.Length < 4)
            {
                return 0;
            }

            //定义要返回的整数
            int num = 0;

            //如果传入的字节数组长度大于4,需要进行处理
            if (data.Length >= 4)
            {
                //创建一个临时缓冲区
                byte[] tempBuffer = new byte[4];

                //将传入的字节数组的前4个字节复制到临时缓冲区
                Buffer.BlockCopy(data, 0, tempBuffer, 0, 4);

                //将临时缓冲区的值转换成整数，并赋给num
                num = BitConverter.ToInt32(tempBuffer, 0);
            }

            //返回整数
            return num;
        }
        #endregion
        #region 将数据转换为指定类型
        /// <summary>
        /// 将数据转换为指定类型
        /// </summary>
        /// <param name="data">转换的数据</param>
        /// <param name="targetType">转换的目标类型</param>
        public static object ConvertTo(object data, Type targetType)
        {
            //如果数据为空，则返回
            if (IsNullOrEmpty(data))
            {
                return null;
            }

            try
            {
                //如果数据实现了IConvertible接口，则转换类型
                if (data is IConvertible)
                {
                    return Convert.ChangeType(data, targetType);
                }
                else
                {
                    return data;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将数据转换为指定类型
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="data">转换的数据</param>
        public static T ConvertTo<T>(object data)
        {
            //如果数据为空，则返回
            if (IsNullOrEmpty(data))
            {
                return default(T);
            }

            try
            {
                //如果数据是T类型，则直接转换
                if (data is T)
                {
                    return (T)data;
                }

                //如果数据实现了IConvertible接口，则转换类型
                if (data is IConvertible)
                {
                    return (T)Convert.ChangeType(data, typeof(T));
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                return default(T);
            }
        }
        #endregion

    }

}
