using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talent.Cheat.Interface;
using Talent.ClinetLog;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;



namespace Talent.Cheat.AnGuoAQD108
{
    public class CheatController : ICheatController
    {

        Thread curDataReceive;
        List<byte> curDataBuf;
        /// <summary>
        /// 串口类
        /// </summary>
        private SerialPort _spCom;
        /// <summary>
        /// 当前配置信息
        /// </summary>
        CheatApparatus _curCheatCfg;
        public CheatController(CheatApparatus pCheatApparatus)
        {
            AGCheatLogger.Debug("----------------安国-初始化---------------------");
            _curCheatCfg = pCheatApparatus;
            curDataBuf = new List<byte>(200);

        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _spCom_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ReadComData();
        }

        private void ReadComData()
        {
            while (_spCom.IsOpen)
            {
                try
                {
                    int readDataLen = _spCom.BytesToRead;
                    byte[] buf = new byte[readDataLen];
                    int length = _spCom.Read(buf, 0, readDataLen);
                    if (length > 0)
                    {
                        curDataBuf.AddRange(buf);
                        //string readstr = Encoding.UTF8.GetString(buf);
                        AGCheatLogger.Debug("接收到数据：" + ByteArrayToHexString(buf));
                        if (buf.Length > 4)
                        {
                            byte[] temp = buf.Skip(buf.Length - 4).Take(2).ToArray();
                            if (temp[0] == 0x20 && temp[1] == 0x20)
                            {
                               // string readstr = Encoding.GetEncoding("GB2312").GetString(curDataBuf.ToArray());
                                string readstr = Encoding.Default.GetString(curDataBuf.ToArray());
                                AGCheatLogger.Debug("触发事件，readstr=" + readstr);
                                OnGetCheatData(readstr);
                                curDataBuf.Clear();
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    AGCheatLogger.Error("接收防作弊数据异常。", ex);
                }
            }

        }

        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="pRawData"></param>
        private void OnGetCheatData(string pRawData)
        {
            try
            {
                if (OnReceivedCheatData != null)
                {
                    OnReceivedCheatData(pRawData);
                }
            }
            catch (Exception ex)
            {
                AGCheatLogger.Error("OnGetCheatData异常。", ex);
                // FileHelpClass.WriteLog("防作弊OnReceivedCheatData事件错误：" + ex.Message + "堆栈：" + ex.StackTrace, "Cheat");
            }
        }

        /// <summary>
        /// 触发错误消息提示
        /// </summary>
        /// <param name="pMsg"></param>
        private void ShowErrorMsg(string pMsg)
        {

            try
            {
                if (OnShowErrorMsg != null)
                {
                    OnShowErrorMsg(pMsg);
                }
            }
            catch (Exception ex)
            {
                AGCheatLogger.Error("防作弊OnShowErrorMsg事件错误.", ex);
                //FileHelpClass.WriteLog("防作弊OnShowErrorMsg事件错误：" + ex.Message + "堆栈：" + ex.StackTrace, "Cheat");
            }
        }

        #region ICheatController
        public event ReceivedCheatData OnReceivedCheatData;
        public event OnShowError OnShowErrorMsg;
        /// <summary>
        /// 打开串口，并注册数据接收方法
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool rtn = false;
            try
            {
                if (_curCheatCfg != null)
                {
                    #region 打开表头
                    _spCom = new SerialPort(_curCheatCfg.ComPort, int.Parse(_curCheatCfg.Baudrate));
                   // _spCom.Encoding = Encoding.GetEncoding("GB2312");
                    _spCom.DataBits = _curCheatCfg.ByteSize;
                    // _spCom.ReadTimeout = 1000;
                    //_spCom.
                    // _spCom.ReadBufferSize = 1000;
                    //奇偶校验
                    if (_curCheatCfg.Parity == ParityType.Even)
                    {
                        _spCom.Parity = Parity.Odd;
                    }
                    else if (_curCheatCfg.Parity == ParityType.Odd)
                    {
                        _spCom.Parity = Parity.Even;
                    }
                    else
                    {
                        _spCom.Parity = Parity.None;
                    }
                    //停止位
                    if (_curCheatCfg.StopSize == StopBitsType.One)
                    {
                        _spCom.StopBits = StopBits.One;
                    }
                    else if (_curCheatCfg.StopSize == StopBitsType.OnePointFive)
                    {
                        _spCom.StopBits = StopBits.OnePointFive;
                    }
                    else if (_curCheatCfg.StopSize == StopBitsType.Two)
                    {
                        _spCom.StopBits = StopBits.Two;
                    }
                    else
                    {
                        _spCom.StopBits = StopBits.None;
                    }

                    try
                    {
                        //_spCom.DataReceived += _spCom_DataReceived;
                        _spCom.Open();
                        AGCheatLogger.Debug("打开串口:" + _spCom.IsOpen);
                        if (_spCom.IsOpen)
                        {
                            ThreadStart ts = new ThreadStart(ReadComData);
                            curDataReceive = new Thread(ts);
                            curDataReceive.IsBackground = true;
                            curDataReceive.Start();
                            AGCheatLogger.Debug("启动数据处理线程。");
                            rtn = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMsg("防作弊串口失败：" + ex.Message);
                    }
                    #endregion
                }
                else
                {
                    ShowErrorMsg("防作弊配置信息不存在.");
                }
            }
            catch (Exception ex)
            {
                AGCheatLogger.Error("防作弊打开串口错误.", ex);
            }

            return rtn;
        }

        /// <summary>
        /// 关闭表头
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
                    Thread.Sleep(200);
                    AGCheatLogger.Debug("防作弊打开串口关闭成功.");
                }
            }
            catch (Exception ex)
            {
                rtn = false;
                AGCheatLogger.Error("关闭防作弊设备失败", ex);
            }
            return rtn;
        }
        #endregion

        #region 16进制数组字符串转换

        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            var buffer = new byte[s.Length / 2];
            for (var i = 0; i < s.Length; i += 2)
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        public static string ByteArrayToHexString(byte[] data)
        {
            var sb = new StringBuilder(data.Length * 3);
            foreach (var b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();
        }

        #endregion
    }
}
