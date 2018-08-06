using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talent.Rfid.Interface
{
    /// <summary>
    /// 收到数据
    /// </summary>
    public delegate void ReceivedData(List<string> pCardIdList);
   
    /// <summary>
    /// RFID功能接口
    /// </summary>
    public interface IRfidController
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        event Action<string> OnShowErrorMsg;
        /// <summary>
        /// 数据处理事件
        /// </summary>
        event ReceivedData onReceivedData;
        /// <summary>
        /// 打卡初始化RFID设备
        /// </summary>
        /// <returns></returns>
         bool Open();
        /// <summary>
        /// 启动数据扫描功能
        /// </summary>
        /// <param name="tIp"></param>
        /// <param name="tPort"></param>
        /// <returns></returns>
        bool Start();
        /// <summary>
        /// 关闭扫描功能
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// 注销RFID设备
        /// </summary>
        /// <returns></returns>
        bool Close();

        /// <summary>
        /// 读取卡设备源编号即卡号
        /// </summary>
        /// <returns></returns>
        string ReadRFIDCardOrgNo();
    }
}