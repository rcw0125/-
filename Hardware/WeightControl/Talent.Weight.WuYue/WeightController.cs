using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Talent.ClinetLog;
using Talent.CommonMethod;
using Talent.Measure.DomainModel.TrainMeasure.ConfigModel;

namespace Talent.Weight.WuYue
{
    /// <summary>
    ///     来车方向
    /// </summary>
    //public enum TrainDirectionEnum
    //{
    //    ZL,//左来
    //    YL//右来
    //}
    public class WeightController
    {
        /// <summary>
        ///     当前五岳配置
        /// </summary>
        private WuYueConfig _curWuYueConfig;


        private bool _isProcessData;

        /// <summary>
        ///     接收数据的缓存
        /// </summary>
        private Queue<string> _receiveDataQueue;

        private Thread _threadProcessData;


        private Thread _threadSocket;
        private EndPoint _localIpep;
        private EndPoint _remoteIpep;

        /// <summary>
        ///  用于UDP发送的网络服务类
        /// </summary>
        private Socket _udpcSend;

        public WeightController()
        {
            WeightDeviceLogger.Debug("----------------------------------五岳通讯开始-----------------------------------");
            var temp = TrainMeasureCfgReader.CfgReader.ReadWuYueConfig();
            WeightDeviceLogger.Debug("读取到配置信息");
            Init(temp);
        }


        /// <summary>
        ///     五岳衡器配置相关
        /// </summary>
        /// <param name="pWuYueConfig"></param>
        public WeightController(WuYueConfig pWuYueConfig)
        {
            WeightDeviceLogger.Debug("----------------------------------五岳通讯开始-----------------------------------");
            Init(pWuYueConfig);
        }

        /// <summary>
        ///     测试时，显示调试信息。
        /// </summary>
        public event Action<string> OnShowLogMsg;

        private void Init(WuYueConfig pWuYueConfig)
        {
            WeightDeviceLogger.Debug(string.Format("配置明细，发送IP地址：{0}，端口：{1}，接收端口：{2},零点最大偏差：{3}", pWuYueConfig.SendIp,
                pWuYueConfig.SendPort, pWuYueConfig.ReceivePort, pWuYueConfig.DynamicOffset));

            _receiveDataQueue = new Queue<string>(20);
            _curWuYueConfig = pWuYueConfig;

            _localIpep = new IPEndPoint(IPAddress.Parse(_curWuYueConfig.SendIp), _curWuYueConfig.SendPort);
            // 本机IP，指定的端口号
            _remoteIpep = new IPEndPoint(IPAddress.Any, _curWuYueConfig.ReceivePort);
            _udpcSend = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udpcSend.Bind(_remoteIpep);
            WeightDeviceLogger.Debug("初始化五岳通讯服务成功！");
            ShowErrMsg("初始化五岳通讯服务成功！");
        }

        /// <summary>
        ///     释放
        /// </summary>
        public void Close()
        {
            //SendQuit();
            _isProcessData = false;

            if (_threadProcessData != null && _threadProcessData.IsAlive)
            {
                _threadProcessData.Abort();
                _receiveDataQueue.Clear();
            }
            if (_threadSocket != null && _threadSocket.IsAlive)
            {
                _threadSocket.Abort();
            }
            _udpcSend.Close();
            WeightDeviceLogger.Debug("退出五岳衡器。");
        }

        private void ShowErrMsg(string pMsg)
        {
            if (OnShowLogMsg != null)
            {
                OnShowLogMsg(pMsg);
            }
        }


        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="obj"></param>
        private void SendCmd(string obj)
        {
            var message = obj;
            var sendbytes = Encoding.ASCII.GetBytes(message);
            _udpcSend.SendTo(sendbytes, _localIpep);
        }

        #region 静态计量事件

        /// <summary>
        ///     string pTag,string pWeight,string pRawData
        /// </summary>
        public event Action<string, string, string> OnJTJLReceiveWL;

        /// <summary>
        ///     静态计量接收到车型，车号
        /// </summary>
        public event Action<string, string> OnJTJLReceiveWC;

        /// <summary>
        ///     静态计量接收到错误
        /// </summary>
        public event Action<ErrorType, string> OnJTJLReceiveErr;

        #endregion

        #region 动态计量事件

        /// <summary>
        ///     动态来车方向
        /// </summary>
        public event Action<string> OnDTJLTrainDirection;

        /// <summary>
        /// 动态计量心跳，保证五岳动态计量界面退出时，通讯不中断。
        /// </summary>
        public event Action<string> OnDTJLSocketXT;

        /// <summary>
        ///     动态计量收到零点值
        /// </summary>
        public event Action<string> OnDTJLReceiveLD;

        /// <summary>
        ///     动态计量接收到错误
        /// </summary>
        public event Action<ErrorType, string> OnDTJLReceiveErr;

        /// <summary>
        ///     收到收尾消息
        /// </summary>
        public event Action OnDTJLReceiveSW;

        /// <summary>
        ///     收到每一节信息
        ///     序号，重量，速度，车号
        /// </summary>
        public event Action<string, string, string, string> OnDTJLReceiveCC;

        #endregion

        #region 动态计量

        /// <summary>
        ///     启动发送和接收线程
        /// </summary>
        /// <returns></returns>
        public void StartDTJL()
        {

            WeightDeviceLogger.Debug("启动动态计量！");
            ShowErrMsg("启动动态计量！");
            _isProcessData = false;
            Thread.Sleep(100);
            //if (_threadProcessData != null)
            //{
            //    while (_threadProcessData.IsAlive)
            //    {
            //        Thread.Sleep(50);
            //    }
            //}
            //if (_threadSocket != null)
            //{
            //    while (_threadSocket.IsAlive)
            //    {
            //        Thread.Sleep(50);
            //    }
            //}
            //_receiveDataQueue.Clear();

            if (_threadProcessData != null && _threadProcessData.IsAlive)
            {
                _threadProcessData.Abort();
                lock (_receiveDataQueue)
                {
                    _receiveDataQueue.Clear();
                }
            }
            if (_threadSocket != null && _threadSocket.IsAlive)
            {
                _threadSocket.Abort();
            }
            _isProcessData = true;

            //启动数据处理线程
            ThreadStart ts1 = ProcessDTJL;
            _threadProcessData = new Thread(ts1) { IsBackground = true };
            _threadProcessData.Start();

            //启动动态计量
            ThreadStart ts = ReceiveDtjlSocketData;
            _threadSocket = new Thread(ts) { IsBackground = true };
            _threadSocket.Start();
        }

        /// <summary>
        ///     处理接收到的动态计量数据
        /// </summary>
        private void ReceiveDtjlSocketData()
        {
            SendQuit();
            Thread.Sleep(200);
            SendCmd(DTJL_CMD.SEND_DTJL);
            WeightDeviceLogger.Debug("发送指令：" + DTJL_CMD.SEND_DTJL);
            ShowErrMsg("发送指令：" + DTJL_CMD.SEND_DTJL);
            //接收打开返回

            while (_isProcessData)
            {
                try
                {
                    var data = new byte[1024];
                    var recv = _udpcSend.ReceiveFrom(data, ref _remoteIpep);
                    var message = Encoding.ASCII.GetString(data, 0, recv);
                    ShowErrMsg("收到数据：" + message);
                    WeightDeviceLogger.Debug("收到数据：" + message);
                    lock (_receiveDataQueue)
                    {
                        _receiveDataQueue.Enqueue(message);
                    }
                }
                catch (Exception ex)
                {
                    ShowErrMsg("接收数据异常，原因：" + ex.Message);
                    WeightDeviceLogger.Error("接收数据异常，原因：" + ex.Message);
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        ///     处理接收到的动态计量数据
        /// </summary>
        private void ProcessDTJL()
        {
            //零点预设为10000
            double tempLd = 10140;
            double.TryParse(_curWuYueConfig.LD.ToString(),out tempLd);
            //int 
            Stopwatch htInterval=new Stopwatch();
         
            while (_isProcessData)
            {
                
                if (_receiveDataQueue.Count > 0)
                {
                    var temp = "";
                    lock (_receiveDataQueue)
                    {
                        temp = _receiveDataQueue.Dequeue();
                    }
                    var msg = temp.Split(',');
                    switch (msg[0])
                    {
                        case DTJL_CMD.RECEIVE_DKCG:
                            ShowErrMsg("动态计量打开成功。");
                            break;
                        case DTJL_CMD.RECEIVE_LD:
                            if (msg.Length == 2)
                            {
                                if (msg[1] != "65")
                                {
                                    if (OnDTJLReceiveLD != null)
                                    {
                                        double curLd = double.Parse(msg[1]) * 1000;
                                        if (Math.Abs(curLd - tempLd) > _curWuYueConfig.DynamicOffset)
                                        {
                                            if (OnDTJLReceiveErr != null)
                                                OnDTJLReceiveErr(ErrorType.Error, "零点异常");
                                        }
                                        else
                                        {
                                            OnDTJLReceiveLD(curLd.ToString());
                                            //tempLd = curLd;
                                        }

                                    }
                                }
                                else
                                {
                                    if (OnDTJLReceiveErr != null)
                                    {
                                        OnDTJLReceiveErr(ErrorType.Error, "设备异常");
                                    }
                                }
                            }
                            break;
                        case DTJL_CMD.RECEIVE_ZL:
                            if (OnDTJLTrainDirection != null)
                            {
                                OnDTJLTrainDirection("ZL");
                            }
                            break;
                        case DTJL_CMD.RECEIVE_YL:
                            if (OnDTJLTrainDirection != null)
                            {
                                OnDTJLTrainDirection("YL");
                            }
                            break;
                        case DTJL_CMD.RECEIVE_CK:
                            if (OnDTJLReceiveErr != null)
                            {
                                OnDTJLReceiveErr(ErrorType.Error, "动态计量时,打开车号识别系统失败。");
                            }
                            break;
                        case DTJL_CMD.RECEIVE_SW:
                            if (OnDTJLReceiveSW != null)
                            {
                                OnDTJLReceiveSW();
                            }
                            break;
                        case DTJL_CMD.RECEIVE_CC:
                            if (msg.Length == 5)
                            {
                                if (OnDTJLReceiveCC != null)
                                {
                                    OnDTJLReceiveCC(msg[1], msg[2], msg[3], msg[4]);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    htInterval.Reset();
                }
                else
                {
                    Thread.Sleep(50);
                    if (!htInterval.IsRunning)
                    {
                        htInterval.Start();
                    }
                    if (htInterval.ElapsedMilliseconds >= 500)
                    {
                        ShowErrMsg("XT");
                        if (OnDTJLSocketXT != null)
                            OnDTJLSocketXT("XT");
                        htInterval.Restart();
                    }
                }
            }
            htInterval.Stop();
        }

        #endregion

        #region 静态计量

        /// <summary>
        ///     发送清零指令
        /// </summary>
        public void SendQL()
        {
            SendCmd(JTJL_CMD.SEND_QL);
            WeightDeviceLogger.Debug("发送清零指令：" + JTJL_CMD.SEND_QL);
        }

        /// <summary>
        ///     发送清零指令
        /// </summary>
        public void SendQuit()
        {
            SendCmd(DTJL_CMD.SEND_CJTJL);
            WeightDeviceLogger.Debug("发送退出指令：" + DTJL_CMD.SEND_CJTJL);
        }

        /// <summary>
        ///     启动发送和接收线程
        /// </summary>
        /// <returns></returns>
        public void StartJTJL()
        {

            WeightDeviceLogger.Debug("启动静态计量！");
            //WeightDeviceLogger.Debug("启动静态计量！");
            _isProcessData = false;
            Thread.Sleep(100);

            if (_threadProcessData != null && _threadProcessData.IsAlive)
            {
                _threadProcessData.Abort();
                lock (_receiveDataQueue)
                {
                    _receiveDataQueue.Clear();
                }
            }
            if (_threadSocket != null && _threadSocket.IsAlive)
            {
                _threadSocket.Abort();
            }


            _isProcessData = true;
            //启动数据处理线程
            ThreadStart ts1 = ProcessJTJL;
            _threadProcessData = new Thread(ts1) { IsBackground = true };
            _threadProcessData.Start();

            //启动动态计量
            ThreadStart ts = ReceiveJtjlSocketData;
            _threadSocket = new Thread(ts) { IsBackground = true };
            _threadSocket.Start();
        }

        /// <summary>
        ///     处理接收到的动态计量数据
        /// </summary>
        private void ReceiveJtjlSocketData()
        {

            SendQuit();
            Thread.Sleep(200);
            SendCmd(JTJL_CMD.SEND_JTJL);
            WeightDeviceLogger.Debug("发送指令：" + JTJL_CMD.SEND_JTJL);
            //接收打开返回
            ShowErrMsg("发送指令：" + JTJL_CMD.SEND_JTJL);

            while (_isProcessData)
            {
                try
                {
                    var data = new byte[1024];
                    var recv = _udpcSend.ReceiveFrom(data, ref _remoteIpep);
                    var message = Encoding.ASCII.GetString(data, 0, recv);
                    ShowErrMsg("收到数据：" + message);
                    WeightDeviceLogger.Debug("收到数据：" + message);
                    lock (_receiveDataQueue)
                    {
                        _receiveDataQueue.Enqueue(message);
                    }
                }
                catch (Exception ex)
                {
                    ShowErrMsg("接收数据异常，原因：" + ex.Message);
                    WeightDeviceLogger.Error("接收数据异常，原因：" + ex.Message);
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        ///     处理接收到的动态计量数据
        /// </summary>
        private void ProcessJTJL()
        {
            while (_isProcessData)
            {
                if (_receiveDataQueue.Count > 0)
                {
                    var temp = "";
                    lock (_receiveDataQueue)
                    {
                        temp = _receiveDataQueue.Dequeue();
                    }
                    var msg = temp.Split(',');
                    switch (msg[0])
                    {
                        case JTJL_CMD.RECEIVE_DKCG:
                            ShowErrMsg("静态计量打开成功。");
                            break;
                        case JTJL_CMD.RECEIVE_WC:
                            if (OnJTJLReceiveWC != null && msg.Length == 3)
                            {
                                //车号,车型
                                OnJTJLReceiveWC(msg[2], msg[1]);
                            }
                            break;
                        case JTJL_CMD.RECEIVE_WL:
                            if (OnJTJLReceiveWL != null && msg.Length == 2)
                            {
                                if (msg[1] != "000.000")
                                {
                                    //tag,weight,rawweight
                                    bool isngv = msg[1].StartsWith("-");
                                    int tempWeight = (int)(double.Parse(msg[1]) * 1000);
                                    string tempweightStr = tempWeight.ToString();
                                    //if (isngv)
                                    //    tempweightStr = "-" + tempweightStr;
                                    if (OnJTJLReceiveWL != null)
                                    {
                                        OnJTJLReceiveWL("1", tempweightStr, "");
                                    }

                                }
                                else
                                {
                                    //静态计量，五岳设备异常
                                    if (OnJTJLReceiveErr != null)
                                    {
                                        OnJTJLReceiveErr(ErrorType.Error, "静态计量，五岳设备异常。");
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        #endregion
    }
}