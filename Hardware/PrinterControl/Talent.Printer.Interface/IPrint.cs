using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.CommonMethod;
using Talent.Measure.DomainModel.PrintModel;

namespace Talent.Printer.Interface
{
    //public enum ErrorType
    //{
    //    Error,
    //    Warning,
    //    Info
    //}
    /// <summary>
    /// 收据打印接口
    /// </summary>
    public interface IPrint
    {
        /// <summary>
        /// 显示错误消息
        /// </summary>
        event Action<ErrorType, string> OnShowErrMsg;
        bool Print(List<Bill> pBillList);
        bool CheckPrinterState(out string pMsg);
    }
}
