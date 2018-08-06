using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talent.Cheat.Interface
{
    /// <summary>
    /// 收到数据
    /// <param name="pRawData">原始数据</param>
    /// </summary>
    public delegate void ReceivedCheatData(string pRawData);
    public delegate void   OnShowError(string errorStr);
    /// <summary>
    /// 接口
    /// </summary>
    public interface ICheatController
    {
        event OnShowError OnShowErrorMsg;
        event ReceivedCheatData OnReceivedCheatData;
        /// <summary>
        /// 打开设备
        /// </summary>
        /// <returns></returns>
        bool Open();
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        bool Close();
    }
}
