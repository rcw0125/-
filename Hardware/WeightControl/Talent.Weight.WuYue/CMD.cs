using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talent.Weight.WuYue
{
    public enum JlType
    {
        DTHL,//动态计量
        JTJL//静态计量
    }
    /// <summary>
    /// 动态计量命令
    /// </summary>
    public class DTJL_CMD
    {
        //发送
        public const string SEND_DTJL = "DTJL";//打开动态计量
        //发送
        public const string SEND_CJTJL = "CJTJL";//退出
        //接收
        /// <summary>
        /// 打开成功
        /// </summary>
        public const string RECEIVE_DKCG = "DKCG";//打开成功
        /// <summary>
        /// 左来车
        /// </summary>
        public const string RECEIVE_ZL = "ZL";//左来车
        /// <summary>
        /// 右来车
        /// </summary>
        public const string RECEIVE_YL = "YL";//右来车
        /// <summary>
        /// 打开车号识别功放失败
        /// </summary>
        public const string RECEIVE_CK = "CK";//打开车号识别功放失败
        /// <summary>
        /// 收尾
        /// </summary>
        public const string RECEIVE_SW = "SW";//收尾
        /// <summary>
        /// 退出
        /// </summary>
        public const string RECEIVE_TC = "TC";//退出
        /// <summary>
        /// 零点
        /// </summary>
        public const string RECEIVE_LD = "LD";//零点

        public const string RECEIVE_CC = "CC";//每节信息
    }
    /// <summary>
    /// 静态计量命令
    /// </summary>
    public class JTJL_CMD
    {
        /// <summary>
        /// 打开动态计量
        /// </summary>
        public const string SEND_JTJL = "JTJL1";//打开动态计量
        /// <summary>
        /// 清零
        /// </summary>
        public const string SEND_QL = "QL";//清零
        //接收
        /// <summary>
        /// 打开成功
        /// </summary>
        public const string RECEIVE_DKCG = "DKCG";//打开成功
        /// <summary>
        /// 重量前缀
        /// </summary>
        public const string RECEIVE_WL = "WL";//重量前缀
        /// <summary>
        /// 车号，车型前缀
        /// </summary>
        public const string RECEIVE_WC = "WC";//车号，车型前缀

        /// <summary>
        /// 静态计量退出
        /// </summary>
        public const string RECEIVE_WT = "WT";//静态计量退出
        //
    }
}
