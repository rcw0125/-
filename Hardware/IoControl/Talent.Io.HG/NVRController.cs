using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talent.Measure.DomainModel;

namespace Talent.Io.HG
{
    public class NVRController : IIoController
    {
        /// <summary>
        /// 串口是否已打开
        /// </summary>
        public bool IsDeviceOpen
        {
            get
            {
                return _spCom.IsOpen;
            }
        }
        /// <summary>
        /// 是否启动线程向串口发送指令
        /// </summary>
        bool _canSendCmd;
        /// <summary>
        /// 当前串口配置
        /// </summary>
        IOconfig _curIOconfig;
        /// <summary>
        /// 命令发送线程
        /// </summary>
        Thread _threadSendCmd;
        /// <summary>
        /// 串口类
        /// </summary>
        private SerialPort _spCom;


        public NVRController(IOconfig pIOconfig)
        {

            _curIOconfig = pIOconfig;
            _spCom = new SerialPort();

        }
        public NVRController(string pConfigFile)
        {
            _canSendCmd = false;
            ConfigReader cfgReader = new ConfigReader(pConfigFile);
            _curIOconfig = ConfigReader.ReadIoConfig();
            _spCom = new SerialPort();
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (!_spCom.IsOpen)
            {
                _spCom.PortName = _curIOconfig.Comport;
                _spCom.BaudRate = int.Parse(_curIOconfig.Baudrate);
                _spCom.ReadTimeout = 500;
                _spCom.Open();
            }
            return _spCom.IsOpen;
        }

        #region start
        public bool Start()
        {
            _canSendCmd = true;
            _threadSendCmd = new Thread(new ThreadStart(SendCmd));
            _threadSendCmd.IsBackground = true;
            _threadSendCmd.Start();
            return true;
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        private void SendCmd()
        {
            while (_canSendCmd)
            {
                if (_spCom.IsOpen)
                {
                    SendCmdAA6("01");
                }
                Thread.Sleep(200);
            }
        }
        #endregion

        /// <summary>
        /// 停止处理数据
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            _canSendCmd = false;
            return true;
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns></returns>
        public bool Closed()
        {
            _canSendCmd = false;
            if (_threadSendCmd != null)
            {
                while (_threadSendCmd.IsAlive)
                {
                    _threadSendCmd.Abort();
                    //RFIDReaderLogger.Debug("启动寻卡时先强制结束寻卡线程。", _curRfidCfg.Ip);
                }
            }
            if(_spCom.IsOpen)
            {
                _spCom.Close();
            }
            return true;
        }

        #region IIoController
        /// <summary>
        /// 弹出错误消息
        /// </summary>
        public event Action<string> OnShowErrMsg;
        /// <summary>
        /// 报警事件
        /// </summary>
        public event ReceiveAlarmSignal OnReceiveAlarmSignal;
        // bool Init();
        public bool ExecCommand(List<IoCommand> pCommandList)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pJsonCommand">json格式的命令</param>
        /// <returns></returns>
        public bool ExecCommand(string pJsonCommand)
        {
            return false;
        }
        public bool ExecTestCommand(string pCmd)
        {
            return false;
        }
        #endregion


        #region private

        /// <summary>
        /// 发送串口命令
        /// </summary>
        /// <param name="pCmd"></param>
        /// <param name="pOverTime">超时时间，以毫秒为单位</param>
        private string SendCmd(string pCmd)
        {
            string cmd = ProcessCommand(pCmd);
            _spCom.DiscardInBuffer();
            _spCom.Write(cmd);
            return _spCom.ReadLine();
        }

        /// <summary>
        /// 模块地址
        /// </summary>
        /// <param name="pAddress"></param>
        /// <returns></returns>
        private void SendCmdAA6(string pAddress)
        {
            try
            {
                string rtn = SendCmd(pAddress + "6");
                if (rtn != "")
                {
                    if (rtn.StartsWith("!"))
                    {
                        //检测CRC
                        if (CRCIsVallid(rtn))
                        {
                            //处理返回数据
                            string data = rtn.Substring(1, 2);
                            byte status = Convert.ToByte(data, 16);
                            foreach (Device de in _curIOconfig.InDeviceList)
                            {
                                int stat = GetbitValue(status, int.Parse(de.Port));
                                if (OnReceiveAlarmSignal != null)
                                {
                                    OnReceiveAlarmSignal(de.Code, stat.ToString());
                                }
                                //if (stat == 1)
                                //{
                                   
                                //    //开关打开
                                //}
                                //else
                                //{
                                //    //开关关闭
                                //}
                            }

                        }
                        else
                        {
                            //CRC校验失败
                        }
                    }
                    else if (rtn.StartsWith("?"))
                    {
                        //命令异常
                    }
                }
            }
            catch (TimeoutException ex)
            {
                // throw ex;
                //超时
            }
            catch (Exception ex)
            {
                // throw ex;
                //其他异常
            }

        }

        /// <summary>
        /// 获取数据中某一位的值
        /// </summary>
        /// <param name="input">传入的数据类型,可换成其它数据类型,比如Int</param>
        /// <param name="index">要获取的第几位的序号,从0开始</param>
        /// <returns>返回值为-1表示获取值失败</returns>
        private int GetbitValue(byte input, int index)
        {
            if (index > sizeof(byte))
            {
                return -1;
            }
            //左移到最高位
            int value = input << (sizeof(byte) - 1 - index);
            //右移到最低位
            value = value >> (sizeof(byte) - 1);
            return value;
        }
        /// <summary>
        /// 给命令添加CRC和终止符
        /// </summary>
        /// <param name="pCmd"></param>
        /// <returns></returns>
        private string ProcessCommand(string pCmd)
        {
            string tcmd = pCmd;
            tcmd += GetCRCValue(pCmd);
            return tcmd + byteToHexString(0X0D);
        }

        #region CRC_CHECK
        /// <summary>
        /// 获取CRC
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        private byte GetCRCValue(byte[] pData)
        {
            byte temp = 0;
            byte[] tData = pData;
            foreach (byte item in tData)
            {
                temp += item;
            }
            return (byte)(0XFF & temp);
        }
        /// <summary>
        /// 获取CRC
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        private string GetCRCValue(string pData)
        {
            byte[] tData = System.Text.ASCIIEncoding.UTF7.GetBytes(pData); ;
            byte temp = GetCRCValue(tData);
            return byteToHexString(temp);
        }
        /// <summary>
        /// 字节转换为16进制字符串，
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        private string byteToHexString(byte pData)
        {
            return Convert.ToString(pData, 16).PadLeft(2, '0').ToUpper();
        }

        /// <summary>
        /// 检测返回值的CRC是否正确
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        private bool CRCIsVallid(string pResponse)
        {
            bool rtn = false;
            string pData = pResponse.Substring(0, pResponse.Length - 3);
            string crcvalue = pResponse.Substring(pResponse.Length - 3, 2);
            if (crcvalue == GetCRCValue(pData))
            {
                rtn = true;
            }
            return rtn;
        }
        #endregion

        #region HEX_STRING
        /// <summary>
        /// 十六进制字符串转换为字节
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        #endregion

        #endregion
    }
}
