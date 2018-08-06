using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    #region IO
    public class IoConfigParam
    {
        /// <summary>
        /// 默认输入框最大长度=14
        /// </summary>
        public static Int32 TextBox_MaxLenght = 14;
        /// <summary>
        /// 检测设备="UsePassCarType"
        /// </summary>
        public static string DetectEqu = "UsePassCarType";
        /// <summary>
        /// 连接方式="ConType"
        /// </summary>
        public static string LinkType = "ConType";
        /// <summary>
        /// 设备驱动="EquDll"
        /// </summary>
        public static string EquDriverName = "EquDll";
        /// <summary>
        /// 串口="Comport"
        /// </summary>
        public static string Comport = "Comport";
        /// <summary>
        /// Ip="Ip"
        /// </summary>
        public static string Ip = "Ip";
        /// <summary>
        /// 端口="Port"
        /// </summary>
        public static string Port = "Port";
        /// <summary>
        /// 波特率="Baudrate"
        /// </summary>
        public static string Baudrate = "Baudrate";
        /// <summary>
        /// 设备登录用户名="EquLoginName"
        /// </summary>
        public static string EquLoginName = "EquLoginName";
        /// <summary>
        /// 设备登录密码="EquLoginPwd"
        /// </summary>
        public static string EquLoginPwd = "EquLoginPwd";
        /// <summary>
        /// 端口数量="PortNum"
        /// </summary>
        public static string PortNum = "PortNum";

        /// <summary>
        /// 列名_设备名称="EquName"
        /// </summary>
        public static string Col_EquName = "EquName";
        /// <summary>
        /// 列名_串口="Port"
        /// </summary>
        public static string Col_Port = "Port";
        /// <summary>
        /// 列名_是否启用="IsUse"
        /// </summary>
        public static string Col_IsUse = "IsUse";
        /// <summary>
        /// 列名_端口类型="PortType"
        /// </summary>
        public static string Col_PortType = "PortType";
        /// <summary>
        /// 列名_设备类型编码="EquTypeCode"
        /// </summary>
        public static string Col_EquTypeCode = "EquTypeCode";
        /// <summary>
        /// 列名_测试数据="TestData"
        /// </summary>
        public static string Col_TestData = "TestData";
        /// <summary>
        /// 行_设备名称="EquName"
        /// </summary>
        public static string Row_EquName = "EquName";
        /// <summary>
        /// 行_端口="Port"
        /// </summary>
        public static string Row_Port = "Port";
        /// <summary>
        /// 行_是否在用="IsUse"
        /// </summary>
        public static string Row_IsUse = "IsUse";
        /// <summary>
        /// 行_编码="Code"
        /// </summary>
        public static string Row_Code = "Code";
        /// <summary>
        /// 行_类型="Type"(1:设备类型;2:设备)
        /// </summary>
        public static string Row_Type = "Type";
        /// <summary>
        /// 行_端口类型="PortType"
        /// </summary>
        public static string Row_PortType = "PortType";
        /// <summary>
        /// 输入端口
        /// </summary>
        public static string Row_PortType_In = "输入";
        /// <summary>
        /// 输出端口
        /// </summary>
        public static string Row_PortType_Out = "输出";
        /// <summary>
        /// 行_设备类型编码="EquTypeCode"
        /// </summary>
        public static string Row_EquTypeCode = "EquTypeCode";
        /// <summary>
        /// 类型值="1"(设备类型)
        /// </summary>
        public static string Type_EquType = "1";
        /// <summary>
        /// 类型值="2"(设备)
        /// </summary>
        public static string Type_Equ = "2";
        /// <summary>
        /// 行_常亮设置="AlwaysLight"
        /// </summary>
        public static string Row_AlwaysLight = "AlwaysLight";
        /// <summary>
        /// 版本号="VersionNum"
        /// </summary>
        public static string VersionNum = "VersionNum";
        /// <summary>
        /// 终端编码="ClientCode"
        /// </summary>
        public static string ClientCode = "ClientCode";
        /// <summary>
        /// 终端名称="ClientName"
        /// </summary>
        public static string ClientName = "ClientName";

        #region 模块编码
        /// <summary>
        /// 业务设置模块编码值="BusinessConfigs"
        /// </summary>
        public static string Model_Code_BusinessConfigs = "BusinessConfigs";
        /// <summary>
        /// 计量设置模块编码值="BusinessConfigs"
        /// </summary>
        public static string Model_Code_MeasurementConfig = "MeasurementConfig";
        /// <summary>
        /// 系统设置模块编码值="SystemConfigs"
        /// </summary>
        public static string Model_Code_SystemConfigs = "SystemConfigs";
        /// <summary>
        /// 系统设置模块编码值="SystemConfig"
        /// </summary>
        public static string Model_Code_SystemConfig = "SystemConfig";
        /// <summary>
        /// 外设设置模块编码值="ExtrnEquConfigs"
        /// </summary>
        public static string Model_Code_ExtrnEquConfigs = "ExtrnEquConfigs";
        /// <summary>
        /// IO设置模块编码值="IoConfig"
        /// </summary>
        public static string Model_Code_IoConfig = "IoConfig";
        /// <summary>
        /// 声音设置模块编码值="VoiceConfig"
        /// </summary>
        public static string Model_Code_VoiceConfig = "VoiceConfig";

        #endregion
    }
    #endregion

    #region IcCard
    public class IcCardConfigParam
    {
        //<column name="ConType" value="连接方式" />
        //<column name="Comport" value="串口" />
        //<column name="Ip" value="IP" />
        //<column name="Port" value="端口" />
        //<column name="Baudrate" value="波特率" />
        //<column name="interval" value="寻卡时间" />
        //<column name="ICReadType" value="读卡器类型" />
        //<column name="ICWriteTemp" value="缓存模式" />
        //<column name="IsUse" value="是否启用" />
        /// <summary>
        /// IO设置模块编码值="IcConfig"
        /// </summary>
        public static string Model_Code_IcConfig = "IcConfig";


        public readonly static string Col_ConType = "ConType";
        public readonly static string Col_Comport = "Comport";
        public readonly static string Col_Ip = "Ip";
        public readonly static string Col_Port = "Port";
        public readonly static string Col_Baudrate = "Baudrate";
        public readonly static string Col_Interval = "Interval";
        public readonly static string Col_ICReadType = "ICReadType";
        public readonly static string Col_ICWriteTemp = "ICWriteTemp";
        public readonly static string Col_IsUse = "IsUse";

        public readonly static string Row_ConType = "ConType";
        public readonly static string Row_Comport = "Comport";
        public readonly static string Row_Ip = "Ip";
        public readonly static string Row_Port = "Port";
        public readonly static string Row_Baudrate = "Baudrate";
        public readonly static string Row_Interval = "Interval";
        public readonly static string Row_ICReadType = "ICReadType";
        public readonly static string Row_Driver = "Driver";
        public readonly static string Row_ICWriteTemp = "ICWriteTemp";
        public readonly static string Row_IsUse = "IsUse";
    }
    #endregion

    #region RFID
    public class RfidConfigParam
    {
        public readonly static string Model_Code_RfidConfig = "RfidConfig";

        /// <summary>
        /// 检测设备="UsePassCarType"
        /// </summary>
        public readonly static string DetectEqu = "UsePassCarType";
        /// <summary>
        /// 连接方式="ConType"
        /// </summary>
        public readonly static string LinkType = "ConType";
        /// <summary>
        /// 设备驱动="EquDll"
        /// </summary>
        public readonly static string EquDriverName = "EquDll";
        public readonly static string Comport = "Comport";
        public readonly static string Baudrate = "Baudrate";
        public readonly static string Ip = "Ip";
        public readonly static string Port = "Port";
        public readonly static string Interval = "Interval";
        public readonly static string IsUse = "IsUse";
        /// <summary>
        /// 功率
        /// </summary>
        public readonly static string Power = "Power";

        public readonly static string Col_AntennaName = "AntennaName";
        public readonly static string Col_Port = "Port";
        public readonly static string Col_IsUse = "IsUse";

        public readonly static string Row_AntennaName = "AntennaName";
        public readonly static string Row_Port = "Port";
        public readonly static string Row_IsUse = "IsUse";

    }
    #endregion
    #region MULTIRFID
    public class MultiRfidConfigParam
    {
        public readonly static string Model_Code_RfidConfig = "MultiRfidConfig";

        /// <summary>
        /// 检测设备="UsePassCarType"
        /// </summary>
        public readonly static string DetectEqu = "UsePassCarType";
        /// <summary>
        /// 连接方式="ConType"
        /// </summary>
        public readonly static string LinkType = "ConType";
        /// <summary>
        /// 设备驱动="EquDll"
        /// </summary>
        public readonly static string EquDriverName = "EquDll";
        public readonly static string Comport = "Comport";
        public readonly static string Baudrate = "Baudrate";
        public readonly static string Ip = "Ip";
        public readonly static string Port = "Port";
        public readonly static string Interval = "Interval";
        public readonly static string IsUse = "IsUse";
        /// <summary>
        /// 功率
        /// </summary>
        public readonly static string Power = "Power";

        public readonly static string Col_AntennaName = "AntennaName";
        public readonly static string Col_Port = "Port";
        public readonly static string Col_IsUse = "IsUse";

        public readonly static string Row_AntennaName = "AntennaName";
        public readonly static string Row_Port = "Port";
        public readonly static string Row_IsUse = "IsUse";

    }
    #endregion

    #region 衡器
    public class WeighterConfigParam
    {
        /// <summary>
        /// 模块编码
        /// </summary>
        public readonly static string Model_Code_WeighterConfig = "WeighterConfig";

        /// <summary>
        /// 串口
        /// </summary>
        public readonly static string Comport = "Comport";

        /// <summary>
        /// 波特率
        /// </summary>
        public readonly static string Baudrate = "Baudrate";

        /// <summary>
        /// 停止位
        /// </summary>
        public readonly static string Stopsize = "Stopsize";

        /// <summary>
        /// 奇偶校验
        /// </summary>
        public readonly static string Parity = "Parity";

        /// <summary>
        /// 数据位
        /// </summary>
        public readonly static string ByteSize = "ByteSize";

        /// <summary>
        /// 设备
        /// </summary>
        public readonly static string DeviceName = "DeviceName";

        /// <summary>
        /// 驱动
        /// </summary>
        public readonly static string DirverName = "DirverName";
        /// <summary>
        /// 最大量程
        /// </summary>
        public readonly static string MaxWeight = "MaxWeight";

        ///// <summary>
        ///// 标记类型
        ///// </summary>
        //public readonly static string DataMarkType = "DataMarkType";

        ///// <summary>
        ///// char标记
        ///// </summary>
        //public readonly static string DataMarkChar = "DataMarkChar";
        ///// <summary>
        ///// 字符标记
        ///// </summary>
        //public readonly static string DataMark = "DataMark";
        ///// <summary>
        ///// 取数方向
        ///// </summary>
        //public readonly static string Direction = "Direction";
        ///// <summary>
        ///// 反向取数
        ///// </summary>
        //public readonly static string DataOrder = "DataOrder";
        ///// <summary>
        ///// 数据长度
        ///// </summary>
        //public readonly static string CharLength = "CharLength";
        ///// <summary>
        ///// 取数位置
        ///// </summary>
        //public readonly static string DataPostion = "DataPostion";
        ///// <summary>
        ///// 取数长度
        ///// </summary>
        //public readonly static string DataLength = "DataLength";
        ///// <summary>
        ///// 倍数
        ///// </summary>
        //public readonly static string Multiple = "Multiple";
        ///// <summary>
        ///// 录像重量
        ///// </summary>
        //public readonly static string VideoAlarm = "VideoAlarm";
        ///// <summary>
        ///// 清零命令
        ///// </summary>
        //public readonly static string ClearCmd = "ClearCmd";
        ///// <summary>
        ///// 取数命令
        ///// </summary>
        //public readonly static string WeightCmd = "WeightCmd";
        ///// <summary>
        ///// 发送间隔
        ///// </summary>
        //public readonly static string InterVal = "InterVal";
    }

    #region 取数方向
    public class Direction
    {
        /// <summary>
        /// 向左
        /// </summary>
        public readonly static string RightToLeft = "向左";
        /// <summary>
        /// 向右
        /// </summary>
        public readonly static string LeftToRight = "向右";
    }
    #endregion
    #region 标记类型
    public class DataMarkType
    {
        /// <summary>
        /// Char字符
        /// </summary>
        public readonly static string DataMarkChar = "Char字符";
        /// <summary>
        /// 字符串
        /// </summary>
        public readonly static string DataMark = "字符串";
    }
    #endregion
    #region 奇偶校验
    public class ParityType
    {
        /// <summary>
        /// 偶校验
        /// </summary>
        public readonly static string Odd = "偶校验";
        /// <summary>
        /// 奇校验
        /// </summary>
        public readonly static string Even = "奇校验";

        public readonly static string None = "无";
    }
    #endregion
    #region 停止位
    public class StopBitsType
    {
        /// <summary>
        /// 偶校验
        /// </summary>
        public readonly static string One = "1";
        /// <summary>
        /// 奇校验
        /// </summary>
        public readonly static string OnePointFive = "1.5";

        public readonly static string Two = "2";
    }
    #endregion
    //#region 器横设备
    //public class WeightDeviceType
    //{
    //    /// <summary>
    //    /// 托利多
    //    /// </summary>
    //    public readonly static string TLD = "托利多";
    //    /// <summary>
    //    /// 天成
    //    /// </summary>
    //    public readonly static string TC = "天成";
    //    /// <summary>
    //    /// 恒科
    //    /// </summary>
    //    public readonly static string HK = "恒科";
    //}
    //#endregion
    #region 使用公共模块是、否

    #endregion
    #endregion

    #region 防作弊器
    public class CheatConfigParam
    {

        /// <summary>
        /// 模块编码
        /// </summary>
        public readonly static string Model_Code_CheatConfig = "CheatConfig";

        /// <summary>
        /// 串口
        /// </summary>
        public readonly static string Comport = "Comport";

        /// <summary>
        /// 波特率
        /// </summary>
        public readonly static string Baudrate = "Baudrate";

        /// <summary>
        /// 停止位
        /// </summary>
        public readonly static string Stopsize = "Stopsize";

        /// <summary>
        /// 奇偶校验
        /// </summary>
        public readonly static string Parity = "Parity";

        /// <summary>
        /// 数据位
        /// </summary>
        public readonly static string ByteSize = "ByteSize";

        /// <summary>
        /// 是否启用
        /// </summary>
        public readonly static string IsUse = "IsUse";

        ///// <summary>
        ///// 设备
        ///// </summary>
        //public readonly static string DeviceName = "DeviceName";

        ///// <summary>
        ///// 驱动
        ///// </summary>
        //public readonly static string DirverName = "DirverName";
 
    } 
    #endregion

    #region 视频设备
    public class VideoConfigParam
    {
        /// <summary>
        /// 视频模块编码
        /// </summary>
        public readonly static string Model_Code_VideoConfig = "VideoConfig";

        /// <summary>
        /// 视频厂家
        /// </summary>
        public readonly static string VideoType = "VideoType";
        /// <summary>
        /// 对讲设备
        /// </summary>
        public readonly static string DialogNum = "DialogNum";
        /// <summary>
        /// IO设备
        /// </summary>
        public readonly static string IONum = "IONum";
        /// <summary>
        /// 视频驱动
        /// </summary>
        public readonly static string VideoDriver = "VideoDriver";

        /// <summary>
        /// 行_是否在用="IsUse"
        /// </summary>
        public static string Row_IsUse = "IsUse";
        /// <summary>
        /// 视频名称
        /// </summary>
        public static string Row_VideoName = "VideoName";
        /// <summary>
        /// 视频位置
        /// </summary>
        public static string Row_Position = "Position";
        /// <summary>
        /// 云台控制
        /// </summary>
        public static string Row_Control = "Control";

        /// <summary>
        /// IP
        /// </summary>
        public static string Row_Ip = "Ip";
        /// <summary>
        /// Port
        /// </summary>
        public static string Row_Port = "Port";
        /// <summary>
        /// 用户名
        /// </summary>
        public static string Row_UserName = "UserName";
        /// <summary>
        /// 密码
        /// </summary>
        public static string Row_PassWord = "PassWord";
        /// <summary>
        /// 通道
        /// </summary>
        public static string Row_Channel = "Channel";
        /// <summary>
        /// 远程对讲
        /// </summary>
        public static string Row_Dialog = "Dialog";
        /// <summary>
        /// 是否拍照
        /// </summary>
        public static string Row_Photograph = "Photograph";

        /// <summary>
        /// 录像回放channel
        /// </summary>
        public static string Row_DvrChannel = "DvrChannel";
    }
    public class VideoType
    {
        /// <summary>
        /// 海康
        /// </summary>
        public readonly static string HK = "海康";
        /// <summary>
        /// 大华
        /// </summary>
        public readonly static string DH = "大华";
    }
    #endregion

    #region 键盘
    /// <summary>
    /// 键盘配置
    /// </summary>
    public class KeyboardConfigParam
    {
        /// <summary>
        /// PhotoConfig
        /// </summary>
        public readonly static string Model_Code_KeyboardConfig = "KeyboardConfig";
        /// <summary>
        /// 串口
        /// </summary>
        public readonly static string HostComport = "HostComport";

        /// <summary>
        /// 波特率
        /// </summary>
        public readonly static string HostBaudrate = "HostBaudrate";

        /// <summary>
        /// 是否启用
        /// </summary>
        public static string HostIsUse = "HostIsUsed";
        /// <summary>
        /// 串口
        /// </summary>
        public readonly static string AuxiliaryComport = "AuxiliaryComport";

        /// <summary>
        /// 波特率
        /// </summary>
        public readonly static string AuxiliaryBaudrate = "AuxiliaryBaudrate";

        /// <summary>
        /// 是否启用
        /// </summary>
        public static string AuxiliaryIsUse = "AuxiliaryIsUsed";

        /// <summary>
        /// 是否标准键盘
        /// </summary>
        public static string IsStandardBoard = "IsStandardBoard";

        /// <summary>
        /// 确认键
        /// </summary>
        public static string KeyOk = "KeyOk";
        /// <summary>
        /// 帮助键
        /// </summary>
        public static string KeyHelp = "KeyHelp";
        /// <summary>
        /// 删除键
        /// </summary>
        public static string KeyDelete = "KeyDelete";
        /// <summary>
        /// 取消键
        /// </summary>
        public static string KeyCancel = "KeyCancel";
        /// <summary>
        /// 清除键
        /// </summary>
        public static string KeyClear = "KeyClear";

        public readonly static string Row_KeyName = "KeyName";
        public readonly static string Row_KeyValue = "KeyValue";
        public readonly static string Row_KeyCode = "KeyCode";
        public readonly static string Row_AvailableIn = "AvailableIn";
    }

    /// <summary>
    /// 按键作用范围
    /// </summary>
    public class KeyUseRange
    {
        /// <summary>
        /// 无限制
        /// </summary>
        public readonly static string NoRange = "无限制";
        /// <summary>
        /// 计量中
        /// </summary>
        public readonly static string Metering = "计量中";
    }
    #endregion

    #region 对讲
    /// <summary>
    /// 照片配置
    /// </summary>
    public class AudioConfigParam
    {
        /// <summary>
        /// AudioConfig
        /// </summary>
        public readonly static string Model_Code_AudioConfig = "AudioConfig";
        /// <summary>
        /// IP
        /// </summary>
        public static string Ip = "Ip";
        /// <summary>
        /// Port
        /// </summary>
        public static string Port = "Port";
        /// <summary>
        /// UserName
        /// </summary>
        public static string UserName = "UserName";
        /// <summary>
        /// PassWord
        /// </summary>
        public static string PassWord = "PassWord";

        /// <summary>
        /// 是否启用
        /// </summary>
        public static string IsUse = "IsUse";
    }
    #endregion

    #region 打印机
    /// <summary>
    /// 打印机配置编码
    /// </summary>
    public class PrinterConfigParam
    {
        /// <summary>
        /// 打印机配置模块编码值
        /// </summary>
        public static string Model_Code_PrinterConfig = "PrinterConfig";

        public readonly static string Row_PrinterName = "PrinterName";
        public readonly static string Row_Comport = "Comport";
        public readonly static string Row_Baudrate = "Baudrate";
        public readonly static string Row_PageMaxCount = "PageMaxCount";
        public readonly static string Row_Notch = "Notch";
        public readonly static string Row_IsUse = "IsUse";
        public readonly static string Row_Band = "Brand";
        public readonly static string Row_Driver = "Driver";
    }
    #endregion

    #region 系统设置
    #region 照片设置
    /// <summary>
    /// 照片配置相关编码
    /// </summary>
    public class FtpConfigParam
    {
        /// <summary>
        /// FtpConfig
        /// </summary>
        public readonly static string Model_Code_FtpConfig = "FtpConfig";
        /// <summary>
        /// 日志存储路径
        /// </summary>
        public readonly static string LogSavePath = "LogSavePath";
        /// <summary>
        /// 照片存储路径
        /// </summary>
        public readonly static string PictureSavePath = "PictureSavePath";
        /// <summary>
        /// FtpIp
        /// </summary>
        public readonly static string FtpIp = "FtpIp";
        /// <summary>
        /// FtpPort
        /// </summary>
        public readonly static string FtpPort = "FtpPort";
        /// <summary>
        /// FtpUserName
        /// </summary>
        public readonly static string FtpUserName = "FtpUserName";
        /// <summary>
        /// FtpPassWord
        /// </summary>
        public readonly static string FtpPassWord = "FtpPassWord";
        /// <summary>
        /// IsPhoto
        /// </summary>
        public readonly static string IsPhoto = "IsPhoto";
    }
    #endregion
    #endregion

    #region 公共硬编码
    public class CommonParam
    {
        /// <summary>
        /// 数据类型值="Int"
        /// </summary>
        public static string DataType_Int = "Int";
        /// <summary>
        /// 数据类型值="String"
        /// </summary>
        public static string DataType_String = "String";
        /// <summary>
        /// 数据类型值="Float"
        /// </summary>
        public static string DataType_Float = "Float";

        /// <summary>
        /// 输入信息异常="【{0}】输入异常!"
        /// </summary>
        public static string Info_Input_Msg_Exption = "【{0}】输入异常!";
        /// <summary>
        /// TextBox名称="端口数量"
        /// </summary>
        //public static string PortNum_TextBox_Name = "端口数量";
        /// <summary>
        /// TextBox名称="端口"
        /// </summary>
        public static string Port_TextBox_Name = "端口";
    }
    #endregion
    #region 设备连接方式
    public class DeviceConType
    {
        /// <summary>
        /// 网口
        /// </summary>
        public readonly static string Net = "网口";
        /// <summary>
        /// 串口
        /// </summary>
        public readonly static string Com = "串口";
    }
    #endregion
    #region 是、否
    public class YesNo
    {
        /// <summary>
        /// 是
        /// </summary>
        public readonly static string Yes = "是";
        /// <summary>
        /// 否
        /// </summary>
        public readonly static string No = "否";
    }
    #endregion
}
