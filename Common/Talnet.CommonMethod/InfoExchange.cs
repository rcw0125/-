using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Talent.CommonMethod
{
    public class InfoExchange
    {
        public static string ConvertToJson(object obj)
        {
            string str;
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    DateParseHandling = DateParseHandling.DateTime,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                };
                str = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return str;
        }

        /// <summary>
        /// 将对象转换为json字符串(忽略引用)
        /// </summary>
        /// <param name="obj">需要转换的对象</param>
        /// <returns>json字符串</returns>
        public static string ConvertToJsonIgnoreRef(object obj)
        {
            string jsonStr = string.Empty;
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    DateParseHandling = DateParseHandling.DateTime,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    
                };
                jsonStr = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return jsonStr;
        }

        /// <summary>
        /// 将对象转换为json字符串(忽略引用),不含内部生成的$id
        /// </summary>
        /// <param name="obj">需要转换的对象</param>
        /// <returns>json字符串</returns>
        public static string ConvertToJsonIgnoreRef1(object obj)
        {
            string jsonStr = string.Empty;
            try
            {
                jsonStr = JsonConvert.SerializeObject(obj);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return jsonStr;
        }

        public static object DeConvert(Type type, string json)
        {
            object obj2;
            try
            {
                obj2 = JsonConvert.DeserializeObject(json, type);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return obj2;
        }
        /// <summary>   
        /// 对象转换为Json字符串   
        /// </summary>   
        /// <param name="jsonObject">对象</param>   
        /// <returns>Json字符串</returns>   
        public static string ToJson(object jsonObject)
        {
            try
            {
                StringBuilder jsonString = new StringBuilder();
                jsonString.Append("{");
                PropertyInfo[] propertyInfo = jsonObject.GetType().GetProperties();
                for (int i = 0; i < propertyInfo.Length; i++)
                {
                    object objectValue = propertyInfo[i].GetGetMethod().Invoke(jsonObject, null);
                    if (objectValue == null)
                    {
                        continue;
                    }
                    StringBuilder value = new StringBuilder();
                    if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                    {
                        value.Append("\"" + objectValue.ToString() + "\"");
                    }
                    else if (objectValue is string)
                    {
                        value.Append("\"" + objectValue.ToString() + "\"");
                    }
                    else if (objectValue is IEnumerable)
                    {
                        value.Append(ToJson((IEnumerable)objectValue));
                    }
                    else
                    {
                        value.Append("\"" + objectValue.ToString() + "\"");
                    }
                    jsonString.Append("\"" + propertyInfo[i].Name + "\":" + value + ","); ;
                }
                return jsonString.ToString().TrimEnd(',') + "}";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 对象克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(T obj)
        {
            string jsonStr = ConvertToJsonIgnoreRef(obj);
            return (T)JsonConvert.DeserializeObject(jsonStr,typeof(T));
        }
    }
}
