using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Keyboard.Interface
{
    /// <summary>
    /// 收到数据的委托方法
    /// <param name="pType">数据类型</param>
    /// <param name="pData">原始数据</param>
    /// <param name="pCommand">命令</param>
    /// </summary>
    public delegate void ReceivedKeyData(KeyDataType pType, string pData, KeyCommand pCommand);
    /// <summary>
    /// 键盘接口类
    /// </summary>
    public interface IKeyboard
    {
        /// <summary>
        /// 异常信息事件
        /// </summary>
        event Action<string> OnShowErrorMsg;
        /// <summary>
        /// 接收到键盘数据的事件
        /// </summary>
        event ReceivedKeyData OnReceivedKeyData;
        /// <summary>
        /// 键盘事件
        /// </summary>
        //event ReceivedKeyData OnReceivedKeyData;
        /// <summary>
        /// 打开键盘
        /// </summary>
        /// <returns>成功与否</returns>
        bool Open();
        /// <summary>
        /// 开始接收键盘数据
        /// </summary>
        /// <returns>成功与否</returns>
        bool Start();
        /// <summary>
        /// 停止接收键盘数据
        /// </summary>
        /// <returns>成功与否</returns>
        bool Stop();
        /// <summary>
        /// 关闭键盘硬件
        /// </summary>
        /// <returns>成功与否</returns>
        bool Close();
    }

    /// <summary>
    /// 键盘数据类型
    /// </summary>
    public enum KeyDataType
    {
        /// <summary>
        /// 命令
        /// </summary>
        COMMAND = 0,
        /// <summary>
        /// 数据
        /// </summary>
        DATA
    }
    /// <summary>
    /// 键盘命令
    /// </summary>
    public enum KeyCommand
    {
        /// <summary>
        /// 空命令
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 确定
        /// </summary>
        OK,
        /// <summary>
        /// 求助
        /// </summary>
        HELP,
        /// <summary>
        /// 删除
        /// </summary>
        DELETE,
        /// <summary>
        /// 清空
        /// </summary>
        CLEAR,
        /// <summary>
        /// 取消
        /// </summary>
        CANCEL,
        /// <summary>
        /// 提示
        /// </summary>
        KEYPROMPT
    }
}
