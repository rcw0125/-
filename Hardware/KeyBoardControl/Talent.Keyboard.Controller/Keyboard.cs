using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using Talent.Keyboard.Interface;
using Talent.Measure.DomainModel;


namespace Talent.Keyboard.Controller
{
    /// <summary>
    /// 键盘硬件控制类
    /// </summary>
    public class KeyBoard : IKeyboard
    {
        /// <summary>
        /// 阿拉伯数字集合
        /// </summary>
        static char[] _numberList = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        /// <summary>
        /// 是否处理键盘事件
        /// </summary>
        bool canRaiseDataEvent = false;
        /// <summary>
        /// 串口类
        /// </summary>
        private SerialPort _spCom;
        /// <summary>
        /// 键盘配置对象
        /// </summary>
        public KeyboardConfig _curKeyboardConfig;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pKeyboardConfig"></param>
        public KeyBoard(KeyboardConfig pKeyboardConfig)
        {
            _curKeyboardConfig = pKeyboardConfig;
        }

        /// <summary>
        /// 触发错误消息提示
        /// </summary>
        /// <param name="pMsg"></param>
        private void ShowErrorMsg(string pMsg)
        {
            if (OnShowErrorMsg != null)
            {
                OnShowErrorMsg(pMsg);
            }
        }
        /// <summary>
        /// 获取键盘数据的方法
        /// </summary>
        /// <param name="pType">数据类型</param>
        /// <param name="pData">数据内容</param>
        /// <param name="pCommand">命令</param>
        private void OnGetKeyData(KeyDataType pType, string pData, KeyCommand pCommand)
        {
            if (OnReceivedKeyData != null)
            {
                OnReceivedKeyData(pType, pData, pCommand);
            }
        }

        /// <summary>
        /// 键盘事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _spCom_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (canRaiseDataEvent)
            {
                int data = Convert.ToInt32(_spCom.ReadByte());
                if (data != -1)
                {
                    if (data == Convert.ToInt32(_curKeyboardConfig.KeyOk.KeyValue))
                    {
                        OnGetKeyData(KeyDataType.COMMAND, _curKeyboardConfig.KeyOk.AvailableIn, KeyCommand.OK);
                    }
                    else if (data == Convert.ToInt32(_curKeyboardConfig.KeyHelp.KeyValue))
                    {
                        OnGetKeyData(KeyDataType.COMMAND, _curKeyboardConfig.KeyHelp.AvailableIn, KeyCommand.HELP);
                    }
                    else if (data == Convert.ToInt32(_curKeyboardConfig.KeyDelete.KeyValue))
                    {
                        OnGetKeyData(KeyDataType.COMMAND, _curKeyboardConfig.KeyDelete.AvailableIn, KeyCommand.DELETE);
                    }
                    else if (data == Convert.ToInt32(_curKeyboardConfig.KeyClear.KeyValue))
                    {
                        OnGetKeyData(KeyDataType.COMMAND, _curKeyboardConfig.KeyClear.AvailableIn, KeyCommand.CLEAR);
                    }
                    else if (data == Convert.ToInt32(_curKeyboardConfig.KeyCancel.KeyValue))
                    {
                        OnGetKeyData(KeyDataType.COMMAND, _curKeyboardConfig.KeyCancel.AvailableIn, KeyCommand.CANCEL);
                    }
                    else if (_numberList.Contains(Convert.ToChar(data)))
                    {
                        OnGetKeyData(KeyDataType.DATA, Convert.ToChar(data).ToString(), KeyCommand.NONE);
                    }
                }
            }
        }

       
        #region IKeyboard
        /// <summary>
        /// 接收键盘数据的事件
        /// </summary>
        public event ReceivedKeyData OnReceivedKeyData;
        /// <summary>
        /// 异常信息事件
        /// </summary>
        public event Action<string> OnShowErrorMsg;

        /// <summary>
        /// 打开键盘
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool rtn = false;
            _spCom = new SerialPort(_curKeyboardConfig.ComPort, _curKeyboardConfig.Baudrate);
            try
            {
                _spCom.DataReceived += _spCom_DataReceived;
                _spCom.Open();
                rtn = true;
            }
            catch
            {
                ShowErrorMsg(string.Format("键盘打开失败,串口号:{0}", _curKeyboardConfig.ComPort));
            }

            return rtn;
        }

        /// <summary>
        /// 开始处理键盘事件
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            canRaiseDataEvent = true;
            return true;
        }
        /// <summary>
        /// 停止处理键盘事件
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            canRaiseDataEvent = false;
            return true;
        }
        /// <summary>
        ///  关闭键盘
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            bool rtn = true;
            try
            {
                if (_spCom.IsOpen)
                {
                    _spCom.Close();
                }
            }
            catch
            {
                rtn = false;
                ShowErrorMsg(string.Format("键盘关闭失败,串口号:{0}", _curKeyboardConfig.ComPort));
            }
            return rtn;
        }
        #endregion
    }
}
