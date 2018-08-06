using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.CarMeasureClient
{
    /// <summary>
    /// 秤体基础信息
    /// </summary>
    public  class ClientInfo
    {
        /// <summary>
        /// 衡器ID
        /// </summary>
       public static string ClientId { set; get; }
       /// <summary>
       /// 衡器Code
       /// </summary>
       public static string ClientCode { set; get; }
       /// <summary>
       /// 衡器名称
       /// </summary>
       public static string Name { set; get; }
       /// <summary>
       /// 衡器IP
       /// </summary>
       public static string Ip { set; get; }
    }

    /// <summary>
    /// 键盘有效范围枚举类
    /// </summary>
    public enum KeyAvailableIn
    {
        无限制,
        计量中
    }
}
