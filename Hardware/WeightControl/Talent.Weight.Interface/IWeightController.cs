using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.CommonMethod;

namespace Talent.Weight.Interface
{
    /// <summary>
    /// 收到数据
    /// <param name="pWeight">重量</param>
    /// <param name="pRawData">原始数据</param>
    /// </summary>
    public delegate void ReceivedWeightData(string pTag,string pWeight,string pRawData);
    public interface IWeightController
    {
        event Action<ErrorType,string> OnShowErrorMsg;
        event ReceivedWeightData OnReceivedWeightData;

        bool ClearZero();
        /// <summary>
        /// 打开设备
        /// </summary>
        /// <returns></returns>
        bool Open();
        /// <summary>
        /// 开始重量读取
        /// </summary>
        /// <returns></returns>
        bool Start();
        /// <summary>
        /// 停止重量读取
        /// </summary>
        /// <returns></returns>
        bool Stop();
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        bool Close();
    }
}
