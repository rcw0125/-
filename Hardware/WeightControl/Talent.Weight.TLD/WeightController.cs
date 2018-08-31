//using log4net;

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

namespace Talent.Weight.TLD
{
    /// <summary>
    ///     日期：2016-01-20
    ///     品牌：托利多
    ///     型号：IND245（已测试） 、T080
    /// </summary>
    public class WeightController : IWeightController
    {
        /// <summary>
        ///     存储com口读取到的数据
        /// </summary>
        private readonly List<byte> _comData;

        /// <summary>
        ///     当前配置信息
        /// </summary>
        private readonly WeighingApparatus _curWeightCfg;

        /// <summary>
        ///     是否触重量处理事件
        /// </summary>
        private bool _canRaiseWeightDataEvent;

        //private bool _canRunThreadWeight;

        private bool _closeComPort = true;

        /// <summary>
        ///     是否正在接收数据
        /// </summary>
        private bool _isDataReceiving;

        /// <summary>
        ///     串口类
        /// </summary>
        private SerialPort _spCom;

        private Thread _threadWeight;

        /// <summary>
        ///     重量信号灯
        /// </summary>
        //private readonly Semaphore _weightSem = new Semaphore(1, 1);
        public WeightController(WeighingApparatus pWeighingApparatus)
        {
            //_log = pLog;
            //_log.Info("----------------------------衡器.托利多-------------------------------");
            _curWeightCfg = pWeighingApparatus;
            // _canRunThreadWeight = false;
            _comData = new List<byte>();
        }

        /// <summary>
        ///     获取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _spCom_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!_spCom.IsOpen)
                {
                    WeightDeviceLogger.Error("接收数据时错误，原因：串口未打开");
                    return;
                }
                if (_closeComPort)
                {
                    _spCom.Close();
                    Thread.Sleep(200);
                    return;
                }

                var readDataLen = _spCom.BytesToRead;
                var buf = new byte[readDataLen];
                _isDataReceiving = true;
                var length = _spCom.Read(buf, 0, readDataLen);
                _isDataReceiving = false;

                if (length <= 0) return;
                lock (_comData)
                {
                    _comData.AddRange(buf.Take(length));
                }
            }
            catch (Exception ex)
            {
                WeightDeviceLogger.Error("串口接收数据异常，原因：", ex);
            }
        }

        /// <summary>
        ///     提取重量数据
        /// </summary>
        private void ProcessWeight()
        {
            const int dataLen = 17;
            var index = 0;
            while (true)
            {
                try
                {
                    var tWeight = new byte[6]; //重量
                    var rawData = new byte[dataLen]; //原始数据

                    if (_comData.Count >= dataLen)
                    {
                        #region LeftToRight

                        index = _comData.IndexOf(Convert.ToByte(0X0D)) + 1;

                        if (index == dataLen)
                        {
                            Array.Copy(_comData.ToArray(), rawData, index);
                            Array.Copy(rawData, 4, tWeight, 0, 6);
                            var weightTemp = Encoding.ASCII.GetString(tWeight);
                            //获取+-符号
                            if (BinaryHelper.IsOne(rawData[2], 1))
                            {
                                weightTemp = "-" + weightTemp.Trim();
                            }
                            //触发事件

                            if (!_canRaiseWeightDataEvent) return;
                            WeightDeviceLogger.Debug(string.Format("触发重量事件。weightTemp={0},tag={1}", weightTemp, 0));
                            OnGetWeightData(weightTemp, ByteArrayToHexString(rawData));
                            if (int.Parse(weightTemp.Trim()) == 0)
                            {
                                Thread.Sleep(100);
                            }

                        }
                        else if (index > 0)
                        {
                            var exceptData = new byte[index];
                            Array.Copy(_comData.ToArray(), exceptData, index);
                            WeightDeviceLogger.Error("衡器异常数据。_comData=" + ByteArrayToHexString(_comData.ToArray()));
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
                    if (index > 0)
                    {
                        lock (_comData)
                        {
                            _comData.RemoveRange(0, index);
                            index = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     翻转字符串
        /// </summary>
        /// <param name="pData"></param>
        /// <returns></returns>
        private string ReverseString(string pData)
        {
            var arr = pData.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        ///     事件处理
        /// </summary>
        /// <param name="pWeight"></param>
        /// <param name="pRawData"></param>
        private void OnGetWeightData(string pWeight, string pRawData)
        {
            try
            {
                if (OnReceivedWeightData != null)
                {
                    OnReceivedWeightData("1", pWeight, pRawData);
                }
            }
            catch (Exception ex)
            {
                WeightDeviceLogger.Error("触发OnReceivedWeightData事件异常.", ex);
            }
        }

        /// <summary>
        ///     触发错误消息提示
        /// </summary>
        /// <param name="pErrorType"></param>
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
                //FileHelpClass.WriteLog("OnShowErrorMsg事件错误:" + ex.Message + "堆栈：" + ex.Message, "WEIGHT");
            }
        }

        #region IWeightController

        public event ReceivedWeightData OnReceivedWeightData;
        public event Action<ErrorType, string> OnShowErrorMsg;

        /// <summary>
        ///     清零
        /// </summary>
        /// <returns></returns>
        public bool ClearZero()
        {
            var rtn = false;
            try
            {
                var isopen = _spCom.IsOpen;
                if (!_spCom.IsOpen)
                {
                    _spCom.Open();
                }
                _spCom.Write("Z");
                //if (isopen == false)
                //{
                //    _spCom.Close();
                //}
                WeightDeviceLogger.Debug("表头清零成功.");
                rtn = true;
            }
            catch (Exception ex)
            {
                rtn = false;
                WeightDeviceLogger.Error("表头清零失败。", ex);
                ShowErrorMsg(ErrorType.Warning, "表头清零失败.");
            }
            return rtn;
        }  


        /// <summary>
        ///     打开串口，并注册数据接收方法
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            var rtn = false;
            if (_curWeightCfg != null)
            {
                WeightDeviceLogger.Debug(
                    string.Format("设置串口参数。ComPort={0},Baudrate={1},DataBits={2}, Parity={3},StopSize={4}",
                        _curWeightCfg.ComPort, _curWeightCfg.Baudrate, _curWeightCfg.ByteSize, _curWeightCfg.Parity,
                        _curWeightCfg.StopSize));

                _spCom = new SerialPort(_curWeightCfg.ComPort, int.Parse(_curWeightCfg.Baudrate))
                {
                    DataBits = _curWeightCfg.ByteSize,
                    ReadBufferSize = 100
                };
               
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

                _spCom.DataReceived += _spCom_DataReceived;
                try
                {
                    WeightDeviceLogger.Debug("表头打开串口。IsOpen=" + _spCom.IsOpen);
                    _spCom.Open();
                 
                    ThreadStart ts = ProcessWeight;
                    _threadWeight = new Thread(ts)
                    {
                        IsBackground = true
                    };
                    _threadWeight.Start();
                    WeightDeviceLogger.Debug("启动表头数据处理线程成功。");
                    _closeComPort = false;
                    rtn = true;
                }
                catch (Exception ex)
                {
                    WeightDeviceLogger.Error("打开表头失败。", ex);
                    ShowErrorMsg(ErrorType.Error, "打开串口失败.");
                }
            }
            else
            {
                WeightDeviceLogger.Error("表头配置信息不存在。");
                ShowErrorMsg(ErrorType.Error, "配置信息不存在.");
            }
            return rtn;
        }

        /// <summary>
        ///     开始数据处理
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            WeightDeviceLogger.Debug("启动表头数据处理。");
            return _canRaiseWeightDataEvent = true;
        }

        /// <summary>
        ///     停止数据处理
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            _canRaiseWeightDataEvent = false;
            WeightDeviceLogger.Debug("停止表头数据处理。");
            return true;
        }

        /// <summary>
        ///     关闭表头
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            var rtn = true;
            try
            {
                if (_spCom.IsOpen)
                {
                    //_log.Info("关闭串口。");
                    if (_isDataReceiving)
                    {
                        _closeComPort = true;
                        WeightDeviceLogger.Debug("关闭表头时，串口正在接收数据。");
                    }
                    else
                    {
                        //_canRunThreadWeight = false;
                        _spCom.Close();
                        if (_comData != null)
                        {
                            lock (_comData)
                            {
                                _comData.Clear();
                            }
                            WeightDeviceLogger.Debug("关闭表头时清除已接收的重量数据。_comData=" +
                                                     ByteArrayToHexString(_comData.ToArray()));
                        }
                        Thread.Sleep(200);
                    }
                }
            }
            catch (Exception ex)
            {
                rtn = false;
                WeightDeviceLogger.Error("关闭表头失败。", ex);
                ShowErrorMsg(ErrorType.Warning, "关闭表头失败。");
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