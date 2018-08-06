using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 日志模型
    /// added by wangc on 20151111
    /// </summary>
    public class LogModel
    {
        private string createTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }

        private string functionName;
        /// <summary>
        /// 功能点名称
        /// </summary>
        public string FunctionName
        {
            get { return functionName; }
            set { functionName = value; }
        }

        private string msg;
        /// <summary>
        /// 信息内容
        /// </summary>
        public string Msg
        {
            get { return msg; }
            set { msg = value; }
        }

        private string origin;
        /// <summary>
        /// 来源(坐席/称点等)
        /// </summary>
        public string Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        private string level;
        /// <summary>
        /// 级别(“正常”、”错误”、”警告”等)
        /// </summary>
        public string Level
        {
            get { return level; }
            set { level = value; }
        }

        private string operateUserName;
        /// <summary>
        /// 操作人
        /// </summary>
        public string OperateUserName
        {
            get { return operateUserName; }
            set { operateUserName = value; }
        }

        private string direction;
        /// <summary>
        /// 方向(In/Out/Out-in/In-out)
        /// </summary>
        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        private object data;
        /// <summary>
        /// 数据
        /// </summary>
        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        private List<DataParam> paramList;
        /// <summary>
        /// 数据参数
        /// </summary>
        public List<DataParam> ParamList
        {
            get { return paramList; }
            set { paramList = value; }
        }
        private string isDataValid;
        /// <summary>
        /// 数据有效性(有效/无效)
        /// </summary>
        public string IsDataValid
        {
            get { return isDataValid; }
            set { isDataValid = value; }
        }
    }

    public class DataParam
    {
        private string paramName;
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName
        {
            get { return paramName; }
            set { paramName = value; }
        }
        private string paramValue;
        /// <summary>
        /// 参数值
        /// </summary>
        public string ParamValue
        {
            get { return paramValue; }
            set { paramValue = value; }
        }
    }

    public class LogConstParam
    {
        /// <summary>
        /// 画横线(用以分割业务块)
        /// </summary>
        public readonly static string Draw_Line = "-----------------------------------------------------------------------------";
        /// <summary>
        /// 方向="In"(正常的业务操作时)
        /// </summary>
        public readonly static string Directions_In = "In";
        /// <summary>
        /// 方向="Out"(调用外部接口)
        /// </summary>
        public readonly static string Directions_Out = "Out";
        /// <summary>
        /// 方向="Out-In"(调用外部接口后,外部接口返回数据时)
        /// </summary>
        public readonly static string Directions_OutIn = "Out-In";
        /// <summary>
        /// 方向="In-Out"(外部接口调用后，给外部接口返回数据时)
        /// </summary>
        public readonly static string Directions_InOut = "In-Out";
        /// <summary>
        ///  数据有效性="有效"
        /// </summary>
        public readonly static string DataValid_Ok = "有效";
        /// <summary>
        ///  数据有效性="无效"
        /// </summary>
        public readonly static string DataValid_No = "无效";
        /// <summary>
        ///  日志等级="正常"
        /// </summary>
        public readonly static string LogLevel_Info = "正常";
        /// <summary>
        ///  日志等级="错误"
        /// </summary>
        public readonly static string LogLevel_Error = "错误";
        /// <summary>
        ///  日志等级="警告"
        /// </summary>
        public readonly static string LogLevel_Warning = "警告";
    }
}
