using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talent.Ic
{

    /// <summary>
    /// 读取IC卡卡号事件
    /// </summary>
    /// <param name="pPort">串口号或者ip地址</param>
    /// <param name="pValue">卡号</param>
    /// <returns></returns>
    public delegate void ReadCardNo(string pComPortNo, string pCardNo);
    /// <summary>
    /// 移卡操作
    /// </summary>
    /// <param name="pComPortNo"></param>
    /// <param name="pCardNo"></param>
    public delegate void RemoveCard(string pComPortNo);
    public interface IIcController
    {
        event Action<string> OnShowErrMsg;
        /// <summary>
        /// 读取卡号
        /// </summary>
        event ReadCardNo OnReadCardNo;
        /// <summary>
        /// 移卡
        /// </summary>
        event RemoveCard OnRemoveCard;
        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns></returns>
        bool Open();
        /// <summary>
        /// 启动读取功能
        /// </summary>
        /// <returns></returns>
        bool Start();
        /// <summary>
        /// 停止扫描功能
        /// </summary>
        /// <returns></returns>
        bool Stop();
        /// <summary>
        /// 关闭注销设备
        /// </summary>
        /// <returns></returns>
        bool Close();

        string ReadCardOrgNo();
    }
}
