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
    /// 键盘控制类
    /// </summary>
    public class KeyBoardController 
    {
        /// <summary>
        /// 键盘
        /// </summary>
        List<KeyBoard> _curKeyBoardList;
        /// <summary>
        /// 键盘配置
        /// </summary>
        List<KeyboardConfig> _curKeyBoardConfigList;

        /// <summary>
        /// 数据处理
        /// </summary>
        public event ReceivedKeyData OnReceivedKeyData;
        /// <summary>
        /// 错误提示
        /// </summary>
        public event Action<string> OnShowErrorMsg;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pConfigFile">配置文件名称</param>
        public KeyBoardController(string pConfigFile)
        {

            ConfigReader cfgReader = new ConfigReader(pConfigFile);
            _curKeyBoardConfigList = ConfigReader.ReadKeyboardConfig();

            if (_curKeyBoardConfigList != null && _curKeyBoardConfigList.Count > 0)
            {
                _curKeyBoardList = new List<KeyBoard>();
                KeyBoard temp;
                foreach (KeyboardConfig item in _curKeyBoardConfigList)
                {
                    temp = new KeyBoard(item);
                    temp.OnShowErrorMsg += temp_OnShowErrorMsg;
                    temp.OnReceivedKeyData += temp_OnReceivedKeyData;
                    _curKeyBoardList.Add(temp);
                }

            }

        }

        /// <summary>
        /// 开始接收键盘数据
        /// </summary>
        /// <param name="pType">数据类型</param>
        /// <param name="pData">数据</param>
        /// <param name="pCommand">键盘命令</param>
        void temp_OnReceivedKeyData(KeyDataType pType, string pData, KeyCommand pCommand)
        {
            if (OnReceivedKeyData != null)
            {
                OnReceivedKeyData(pType, pData, pCommand);
            }
        }

        /// <summary>
        /// 异常信息处理的方法
        /// </summary>
        /// <param name="obj"></param>
        void temp_OnShowErrorMsg(string obj)
        {
            if (OnShowErrorMsg != null)
            {
                OnShowErrorMsg(obj);
            }
        }

        #region IKeyboard
        /// <summary>
        /// 打开键盘
        /// </summary>
        public bool Open()
        {
            bool rtn = true;
            try
            {
                _curKeyBoardList.ForEach(m => m.Open());
            }
            catch (Exception ex)
            {
                rtn = false;
                throw ex;
            }
            return rtn;
        }
        /// <summary>
        /// 开始处理键盘事件
        /// </summary>
        public bool Start()
        {
            bool rtn = true;
            try
            {
                _curKeyBoardList.ForEach(m => m.Start());
            }
            catch (Exception ex)
            {
                rtn = false;
                throw ex;
            }
            return rtn;
            
        }
        /// <summary>
        /// 停止处理键盘事件
        /// </summary>
        public bool Stop()
        {
            bool rtn = true;
            try
            {
                _curKeyBoardList.ForEach(m => m.Stop());
            }
            catch (Exception ex)
            {
                rtn = false;
                throw ex;
            }
            return rtn;
           
        }
        /// <summary>
        /// 关闭键盘
        /// </summary>
        public bool Close()
        {
            bool rtn = true;
            try
            {
                _curKeyBoardList.ForEach(m => m.Close());
            }
            catch (Exception ex)
            {
                rtn = false;
                throw ex;
            }
            return rtn;
        }

        #endregion

        /// <summary>
        /// 返回按键对应的命令
        /// </summary>
        /// <param name="pKeyData">按键码</param>
        /// <returns></returns>
        public KeyCommand GetKeyCommand(int pKeyData)
        {
            KeyCommand rtn = KeyCommand.NONE;
            if (pKeyData == int.Parse(_curKeyBoardConfigList[0].KeyOk.KeyValue))
            {
                rtn = KeyCommand.OK;
            }
            else if (pKeyData == int.Parse(_curKeyBoardConfigList[0].KeyCancel.KeyValue))
            {
                rtn = KeyCommand.CANCEL;
            }
            else if (pKeyData == int.Parse(_curKeyBoardConfigList[0].KeyDelete.KeyValue))
            {
                rtn = KeyCommand.DELETE;
            }
            else if (pKeyData == int.Parse(_curKeyBoardConfigList[0].KeyClear.KeyValue))
            {
                rtn = KeyCommand.CLEAR;
            }
            else if (pKeyData == int.Parse(_curKeyBoardConfigList[0].KeyHelp.KeyValue))
            {
                rtn = KeyCommand.HELP;
            }
            //else if (pKeyData == int.Parse(_curKeyBoardConfigList[0].KeyPrompt.KeyValue))
            //{
            //    rtn = KeyCommand.KEYPROMPT;
            //}
            return rtn;
        }
        
    }
}
