using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 设备编码静态字符串
    /// </summary>
    public class DeviceConst
    {
        #region 设备类型硬编码
        /// <summary>
        /// 红灯
        /// </summary>
        public readonly static string RedLight = "RedLight";
        /// <summary>
        /// 绿灯
        /// </summary>
        public readonly static string GreenLight = "GreenLight";

        /// <summary>
        /// 红外对射
        /// </summary>
        public readonly static string Infrared_Correlation = "InfraredCorrelation";
        /// <summary>
        /// IO开关设备
        /// </summary>
        public readonly static string IoSwitche = "IoSwitche";
        #endregion

        #region 设备编码
        /// <summary>
        /// 左红外对射
        /// </summary>
        public readonly static string Left_Infrared_Correlation = "LeftInfraredCorrelation";
        /// <summary>
        /// 右红外对射
        /// </summary>
        public readonly static string Right_Infrared_Correlation = "RightInfraredCorrelation";
        /// <summary>
        /// 前红外对射编码="AheadInfraredCorrelation"
        /// </summary>
        public readonly static string Ahead_Infrared_Correlation = "AheadInfraredCorrelation";
        /// <summary>
        /// 后红外对射编码="BehindInfraredCorrelation"
        /// </summary>
        public readonly static string Behind_Infrared_Correlation = "BehindInfraredCorrelation";
        /// <summary>
        /// 继电器="RelayIoSwitche"
        /// </summary>
        public readonly static string RelayIoSwitche = "RelayIoSwitche";
        #endregion
     
    }
    #region 系统配置常量

    #region 系统计量方式
    public enum eMeasureType
    {
        现场自助 = 0,
        远程计量 = 1
    }
    #endregion

    #region 计量启动方式
    public enum eStartup
    {
        重量 = 0,
        重量IC卡 = 1,
        重量RFID卡 = 2,
        重量IC卡RFID卡 = 3
    }
    #endregion
    #region 计量业务状态
    public enum eBullTag
    { 
        /// <summary>
        /// 初始化
        /// </summary>
        init = 100,

        /// <summary>
        /// 空闲
        /// </summary>
        free= 0,
        /// <summary>
        /// 车上称
        /// </summary>
        weight = 1,
        /// <summary>
        /// 称量规则
        /// </summary>
        specification = 2,
        /// <summary>
        /// 计量
        /// </summary>
        metering = 3,
        /// <summary>
        /// 结束
        /// </summary>
        end=4,

        /// <summary>
        /// 错误
        /// </summary>
        error=-1,
        /// <summary>
        /// 终止计量
        /// </summary>
        stop=5,
    }
    #endregion

    #endregion
}
