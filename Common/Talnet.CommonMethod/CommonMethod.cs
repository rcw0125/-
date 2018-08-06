using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.CommonMethod
{
    /// <summary>
    /// 公共方法类
    /// </summary>
    public class CommonMethod
    {
        /// <summary>
        /// 数据是否能够转为指定格式的数据
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static bool IsDataTransformSuccess(string dataType, string data)
        {
            bool isSuccess = true;
            do
            {
                if (CommonParam.DataType_Int.ToLower() == dataType.ToLower())
                {
                    try
                    {
                        Int32.Parse(data);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;
                }
                if (CommonParam.DataType_Float.ToLower() == dataType.ToLower())
                {
                    try
                    {
                        float.Parse(data);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                    break;
                }

            }
            while (false);
            return isSuccess;
        }
        /// <summary>
        /// 获取随机数据
        /// </summary>
        /// <returns></returns>
        public static int GetRandom()
        {
            Random ran = new Random();
            int RandKey = ran.Next(100, 999);
            return RandKey;
        }
    }
}
