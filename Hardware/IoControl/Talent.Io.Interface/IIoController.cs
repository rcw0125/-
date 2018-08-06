using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talent.Io
{

    /// <summary>
    /// 报警信号接收代理
    /// </summary>
    /// <param name="pPort">设备编号</param>
    /// <param name="pValue">控制状态 0or1</param>
    /// <returns></returns>
    public delegate void ReceiveAlarmSignal(string pDeviceCode, string pValue);
    public interface IIoController
    {
        /// <summary>
        /// 弹出错误消息
        /// </summary>
        event Action<string> OnShowErrMsg;
        /// <summary>
        /// 报警事件
        /// </summary>
        event ReceiveAlarmSignal OnReceiveAlarmSignal;
       // bool Init();
        bool ExecCommand(List<IoCommand> pCommandList);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pJsonCommand">json格式的命令</param>
        /// <returns></returns>
        bool ExecCommand(string pJsonCommand);
        bool ExecTestCommand(string pCmd);
    }

    public class IoCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pType">设备2,设备类型1 ，参见 IoConfigParam.Type_EquType</param>
        /// <param name="pCode">设备或,设备类型编码</param>
        /// <param name="pValue"> 值 0（关闭）或1（打开）</param>
        public IoCommand(string pType,string pCode,string pValue)
        {
            _type = pType;
            _code = pCode;
            _value = pValue;
        }

        private string _type;

        /// <summary>
        /// 设备0或者设备类型1
        /// </summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _code;
        
        /// <summary>
        /// 设备或者设备类型编码
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }


        private string _value;

        /// <summary>
        /// 设备或者设备类型值 0（关闭）或1（打开）
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
