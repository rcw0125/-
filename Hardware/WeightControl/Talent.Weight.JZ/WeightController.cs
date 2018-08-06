using log4net;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using Talent.ClinetLog;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.Weight.Interface;
using Talent_LT.HelpClass;

namespace Talent.Weight.JZ
{
    /// <summary>
    /// 品牌：金钟 
    /// 型号：XK3102
    /// 备注: 金钟表头因连续取数无法清零，
    ///       工作模式需要调整为应答模式。
    /// </summary>
    public class WeightController : IWeightController
    {
        /// <summary>
        /// 异常记数，如果大于10则，清除缓存数据关闭串口，重新打开。
        /// </summary>
        int curErrCount = 0;

        Thread _threadSendCmd;
        Thread _threadWeight;


        /// <summary>
        /// 表头取数是否为命令模式。
        /// 连续取数：true，命令取数：false
        /// </summary>
        static readonly bool _IsCommandMode = false;

        /// <summary>
        /// 是否允许发送取数命令
        /// 此变量控制发送取数命令线程是否运行或退出。
        ///  true 线程执行并在指定间隔发送取数命令。false取数命令线程退出。
        /// </summary>
        bool _canSendGetWeightCmd;
        /// <summary>
        /// 控制是否处理取到的表头重量数据
        /// </summary>
        bool _canRunThreadWeight;

        /// <summary>
        /// 串口是否关闭
        /// </summary>
        bool _closeComPort = true;
        /// <summary>
        /// 是否正在接收数据
        /// </summary>
        bool _isDataReceiving = false;

        /// <summary>
        /// 是否触重量处理事件
        /// </summary>
        bool canRaiseWeightDataEvent = false;

        /// <summary>
        /// while条件，循环控制衡器串口数据
        /// </summary>
        bool _isProcessWeightData = false;

        /// <summary>
        /// 数据分割表示
        /// </summary>
        byte[] _dataSplitMark = new Byte[2] { 0X0D, 0X0A };
        /// <summary>
        /// 存储com口读取到的数据
        /// </summary>
        private List<Byte> _comData;
        /// <summary>
        /// 串口类
        /// </summary>
        private SerialPort _spCom;
        /// <summary>
        /// 当前配置信息
        /// </summary>
        WeighingApparatus _curWeightCfg;
        /// <summary>
        /// 重量信号灯
        /// </summary>
        Semaphore weightSem = new Semaphore(1, 1);
        public WeightController(WeighingApparatus pWeighingApparatus)
        {
            _curWeightCfg = pWeighingApparatus;
            _canRunThreadWeight = false;
            _comData = new List<byte>();
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
            try
            {
                if (_closeComPort)
                {
                    _canRunThreadWeight = false;
                    _spCom.Close();
                    if (_comData != null)
                    {
                        _comData.Clear();
                    }
                    WeightDeviceLogger.Debug("读取数据前关闭表头串口成功。");
                    Thread.Sleep(200);
                    return;
                }

                if (_spCom.IsOpen)
                {
                    int readDataLen = _spCom.BytesToRead;
                    byte[] buf = new byte[readDataLen];

                    _isDataReceiving = true;
                    int length = _spCom.Read(buf, 0, readDataLen);
                    _isDataReceiving = false;

                    if (length > 0)
                    {
                        string readstr = Encoding.ASCII.GetString(buf);
                        //FileHelpClass.WriteLog("金钟串口接收信息:" + readstr , "WEIGHT");
                        // weightSem.WaitOne();
                        lock (_comData)
                        {
                            if (_comData.Count <= 54)
                            {
                                _comData.AddRange(buf.Take(length));
                            }
                        }
                        // weightSem.Release();
                    }
                }
            }
            catch (TimeoutException ex)
            {
                WeightDeviceLogger.Error("读取表头串口数据超时1秒。", ex);
            }
            catch (Exception ex)
            {
                WeightDeviceLogger.Error("读取表头串口数据错误。", ex);
            }

        }

        /// <summary>
        /// 提取重量数据
        /// </summary>
        private void ProcessWeight()
        {
            int dataLen = 18;
            int index = 0;
            while (_isProcessWeightData)
            {
                try
                {
                    var tWeight = new Byte[7];//重量
                    var rawData = new Byte[dataLen];//原始数据

                    if (_comData.Count >= dataLen)
                    {
                        #region LeftToRight

                        index = GetSegMarkerIndex() + 2;

                        if (index == dataLen)
                        {
                            #region try
                            string tag = "1";
                            Array.Copy(_comData.ToArray(), rawData, index);

                            byte[] temp = new byte[] { rawData[0], rawData[1] };
                            if(temp[0]==0X4F && temp[1]==0X4C)
                           // if (System.Text.ASCIIEncoding.Default.GetString(temp)!="OV")
                            {
                                tag = "0";//超重
                            }
                           
                            Array.Copy(rawData, 7, tWeight, 0, 7);
                            string weightTemp = System.Text.Encoding.ASCII.GetString(tWeight);

                            if (tag == "1" && int.Parse(weightTemp) > _curWeightCfg.MaxWeight)
                            {
                                tag = "0";
                            }

                            //获取+-符号
                            if (rawData[6] == Convert.ToByte(0X2D))//"-"号
                            {
                                weightTemp = "-" +(tag=="0"?"1":weightTemp.Trim());
                            }

                            //触发事件
                            if (canRaiseWeightDataEvent)
                            {
                                WeightDeviceLogger.Debug(string.Format("触发重量事件。weightTemp={0},temp={1},tag={2}", weightTemp, ByteArrayToHexString(temp), tag));
                                OnGetWeightData(tag,tag=="0"?"-1":weightTemp, "");
                                curErrCount = 0;
                            }
                            if (tag=="0" || int.Parse(weightTemp.Trim()) == 0)
                            {
                                Thread.Sleep(50);
                            }
                            #endregion
                        }
                        else if (index > 0)
                        {
                            var exceptData = new Byte[index];
                            Array.Copy(_comData.ToArray(), exceptData, index);
                            //_log.Warn("异常数据:" + exceptData);
                            WeightDeviceLogger.Error("衡器异常数据。_comData=" + ByteArrayToHexString(_comData.ToArray()));
                            //FileHelpClass.WriteLog("【【【【【【【【【【【【异常数据】】】】】】】】】】：" + exceptData, "WEIGHT");
                           
                            Thread.Sleep(50);
                        }
                        #endregion
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                    
                }
                catch (Exception ex)
                {
                    WeightDeviceLogger.Error("数据线程处理内部异常，_comData=" + ByteArrayToHexString(_comData.ToArray()), ex);
                }
                finally
                {
                    //跳过已处理的数据
                    lock (_comData)
                    {
                        if (index > 0)
                        {
                            try
                            {
                                _comData.RemoveRange(0, index);
                            }
                            catch (Exception ex)
                            {
                                WeightDeviceLogger.Error("移除已处理数据异常，index=" + index.ToString(), ex);
                            }
                            index = 0;

                        }
                    }
                }
            }
        }
        /// <summary>
        /// 翻转字符串
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        private string ReverseString(string pData)
        {
            char[] arr = pData.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="pWeight"></param>
        /// <param name="pRawData"></param>
        private void OnGetWeightData(string pTag, string pWeight, string pRawData)
        {
            try
            {
                if (OnReceivedWeightData != null)
                {
                    OnReceivedWeightData(pTag, pWeight, pRawData);
                }
            }
            catch (Exception ex)
            {
                WeightDeviceLogger.Error("触发OnReceivedWeightData事件异常.", ex);
            }
        }

        /// <summary>
        /// 触发错误消息提示
        /// </summary>
        /// <param name="pMsg"></param>
        private void ShowErrorMsg(ErrorType pErrorType, string pMsg)
        {

            try
            {
                if (OnShowErrorMsg != null)
                {
                    OnShowErrorMsg(pErrorType, pMsg);
                }
            }
            catch (Exception ex)
            {
                WeightDeviceLogger.Error("触发OnShowErrorMsg事件异常", ex);
            }
        }

        /// <summary>
        ///  获取分隔符索引 
        /// </summary>
        /// <returns></returns>
        private int GetSegMarkerIndex()
        {
            int index = -2;
            if (_comData.Count >= 2)
            {
                for (int count = 0; count < _comData.Count - 1; count++)
                {
                    if (_dataSplitMark[0] == _comData[count] && _dataSplitMark[1] == _comData[count + 1])
                    {
                        index = count;
                        break;
                    }
                }
            }
            return index;
        }

        #region IWeightController
        public event ReceivedWeightData OnReceivedWeightData;
        public event Action<ErrorType, string> OnShowErrorMsg;
        /// <summary>
        /// 清零
        /// </summary>
        /// <param name="pDeviceName"></param>
        /// <returns></returns>
        public bool ClearZero()
        {
            bool rtn = false;
            try
            {
                bool isopen = _spCom.IsOpen;
                if (!_spCom.IsOpen)
                {
                    _spCom.Open();
                    WeightDeviceLogger.Debug("表头清零时，先打开串口。");
                }
                byte[] tClear = { 0X5A, 0X0D, 0X0A };//Z
                _spCom.Write(tClear, 0, tClear.Length);

                //if (isopen == false)
                //{
                //    _spCom.Close();
                //}

                WeightDeviceLogger.Debug("表头清零成功。");
                rtn = true;
            }
            catch (Exception ex)
            {
                rtn = false;
                WeightDeviceLogger.Error("表头清零失败.", ex);
                ShowErrorMsg(ErrorType.Error, "表头清零失败.");
            }
            return rtn;
        }


        /// <summary>
        /// 打开串口，并注册数据接收方法
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool rtn = false;
            try
            {
                if (_curWeightCfg != null)
                {
                    WeightDeviceLogger.Debug(string.Format("设置串口参数。ComPort={0},Baudrate={1},DataBits={2}, Parity={3},StopSize={4}",
                        _curWeightCfg.ComPort, _curWeightCfg.Baudrate, _curWeightCfg.ByteSize, _curWeightCfg.Parity, _curWeightCfg.StopSize));
                    #region 打开表头
                    _spCom = new SerialPort(_curWeightCfg.ComPort, int.Parse(_curWeightCfg.Baudrate));
                    //_spCom.Encoding = Encoding.ASCII;
                    _spCom.DataBits = _curWeightCfg.ByteSize;
                    _spCom.ReadBufferSize = 100;
                    _spCom.ReadTimeout = 1000;
                    //奇偶校验
                    if (_curWeightCfg.Parity == ParityType.Even)
                    {
                        _spCom.Parity = Parity.Odd;
                    }
                    else if (_curWeightCfg.Parity == ParityType.Odd)
                    {
                        _spCom.Parity = Parity.Even;
                    }
                    else
                    {
                        _spCom.Parity = Parity.None;
                    }
                    //停止位
                    if (_curWeightCfg.StopSize == StopBitsType.One)
                    {
                        _spCom.StopBits = StopBits.One;
                    }
                    else if (_curWeightCfg.StopSize == StopBitsType.OnePointFive)
                    {
                        _spCom.StopBits = StopBits.OnePointFive;
                    }
                    else if (_curWeightCfg.StopSize == StopBitsType.Two)
                    {
                        _spCom.StopBits = StopBits.Two;
                    }
                    else
                    {
                        _spCom.StopBits = StopBits.None;
                    }
                    if (false == _IsCommandMode)
                    {
                        _spCom.DataReceived += _spCom_DataReceived;
                        _spCom.ErrorReceived += _spCom_ErrorReceived;
                    }
                    try
                    {
                        _spCom.Open();
                        //if (_IsCommandMode)
                        //{
                        //    //启动发送命令的线程
                        //    //StartGetWeightThread();
                        //}
                        WeightDeviceLogger.Debug("表头打开串口。IsOpen=" + _spCom.IsOpen);
                        if (_spCom.IsOpen)
                        {
                            _closeComPort = false;

                            _isProcessWeightData = true;
                            _canSendGetWeightCmd = true;
                           
                            ThreadStart ts = new ThreadStart(ProcessWeight);
                            _threadWeight = new Thread(ts);
                            _threadWeight.IsBackground = true;
                            _canRunThreadWeight = true;
                            _threadWeight.Start();
                            WeightDeviceLogger.Debug("启动表头数据处理线程成功。");
                           
                            rtn = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        WeightDeviceLogger.Error("打开表头失败。", ex);
                        ShowErrorMsg(ErrorType.Error, "表头串口失败.");
                        throw ex;
                    }
                    #endregion
                }
                else
                {
                    WeightDeviceLogger.Error("表头配置信息不存在。");
                    ShowErrorMsg(ErrorType.Error, "表头配置信息不存在.");
                }
            }
            catch (Exception ex)
            {
                WeightDeviceLogger.Error("表头打开异常.", ex);
                throw ex;
            }

            return rtn;
        }
        /// <summary>
        /// 记录串口通讯异常信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _spCom_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            curErrCount++;
            if (curErrCount >= 10)
            {
                Stop();
                Close();
                Thread.Sleep(100);
                WeightDeviceLogger.Error("重新打开串口._spCom.IsOpen=" + _spCom.IsOpen);
                if (Open())
                {
                    Start();
                    WeightDeviceLogger.Debug("已重新打开串口._spCom.IsOpen=" + _spCom.IsOpen);
                }
                curErrCount = 0;
            }
            WeightDeviceLogger.Error("接受到串口通讯错误。e.EventType=" + e.EventType);
            //ShowErrorMsg(ErrorType.Error, "接受到串口通讯异常");
        }
        /// <summary>
        /// 开始数据处理
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            WeightDeviceLogger.Debug("启动表头数据处理。");
            return canRaiseWeightDataEvent = true;
        }

        /// <summary>
        /// 停止数据处理
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            canRaiseWeightDataEvent = false;
            _isProcessWeightData = false;
            int waitTime = 0;
            while (_threadWeight.IsAlive)
            {
                Thread.Sleep(50);
                waitTime += 50;
                if (waitTime >= 300)
                {
                    try
                    {
                        _threadWeight.Abort();
                    }
                    catch
                    {
                        WeightDeviceLogger.Debug("强制结束衡器数据处理线程。");
                    }
                }

            }
            
            WeightDeviceLogger.Debug("停止表头数据处理。");
            return true;
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

                    if (_isDataReceiving)
                    {
                        _closeComPort = true;
                        WeightDeviceLogger.Debug("关闭表头时，串口正在接收数据。");
                    }
                    else
                    {
                        _canSendGetWeightCmd = false;
                        _canRunThreadWeight = false;
                        _spCom.Close();
                     
                        WeightDeviceLogger.Debug("关闭表头成功。");
                      
                    }

                }
                if (_comData != null)
                {
                    _comData.Clear();
                    WeightDeviceLogger.Debug("关闭表头时清除已接收的重量数据。_comData=" + ByteArrayToHexString(_comData.ToArray()));
                }
                Thread.Sleep(200);
            }
            catch (Exception ex)
            {
                rtn = false;
                WeightDeviceLogger.Error("关闭表头失败。", ex);
                ShowErrorMsg(ErrorType.Error, "关闭表头失败。");
            }
            return rtn;
        }
        #endregion

        #region 发送取数命令
        /// <summary>
        /// 开始发送命令取数
        /// </summary>
        private void StartGetWeightThread()
        {
            ThreadStart ts = new ThreadStart(SendGetWeightCmd);
            _threadSendCmd = new Thread(ts);
            _threadSendCmd.IsBackground = true;
            _threadSendCmd.Start();
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        private void SendGetWeightCmd()
        {
            while (_canSendGetWeightCmd)
            {
                byte[] tGetWeight = { 0X52, 0X0D, 0X0A };
                _spCom.Write(tGetWeight, 0, tGetWeight.Length);
                Thread.Sleep(50);
                ReadComData();
                //Thread.Sleep(50);
                Thread.Sleep(200);
                // Thread.Sleep(50);
            }
        }
        #endregion
        #region 16进制数组字符串转换
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }
        #endregion

    }
}
