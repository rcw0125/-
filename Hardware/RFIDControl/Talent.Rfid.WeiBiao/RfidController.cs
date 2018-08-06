using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talent.Rfid.Interface;
using System.Net;
using Talent.Measure.DomainModel;
using Talent.ClinetLog;
using System.Net.Sockets;
using System.Threading;

namespace Talent.Rfid.WeiBiao
{
    public class RfidController : IRfidController
    {
        private static readonly Byte[] openCmd = new Byte[] { 0XFA, 0X00, 0X0A, 0X76, 0XF5 };
        private static readonly Byte[] closeCmd = new Byte[] { 0XFA, 0X00, 0X05, 0X7B, 0XF5 };
        Thread t;
        Thread tsListen;
        Socket _listenSocket;
        bool _canProcessData;
        List<Byte> _rawData;
        byte[] MsgBuffer;
        public RfidCfg _curRfidCfg;
        Socket _serverSocket;
        public RfidController(RfidCfg pRfidCfg)
        {
            _curRfidCfg = pRfidCfg;
            MsgBuffer = new Byte[100];
            _rawData = new List<byte>();
            //打开监听线程
            tsListen = new Thread(new ThreadStart(delegate
            {
                _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(_curRfidCfg.Ip);
                IPEndPoint ipe = new IPEndPoint(ip, int.Parse(_curRfidCfg.Port));
                _listenSocket.Bind(ipe);
                _listenSocket.Listen(0);
                while (true)
                {
                    _serverSocket = _listenSocket.Accept();
                    ReceiveData();
                }

            }));
            tsListen.IsBackground = true;
            tsListen.Start();

            //RFIDReaderLogger.Debug(string.Format("实例化RFID读写器。IP={0}，端口={1}", pRfidCfg.Ip, pRfidCfg.Port), pRfidCfg.Ip);
        }

        public void ReceiveData()
        {
            _serverSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (_serverSocket.Connected)
                {
                    int REnd = _serverSocket.EndReceive(ar);

                    if (REnd > 0)
                    {
                        byte[] data = new byte[REnd];
                        Array.Copy(MsgBuffer, data, REnd);
                        lock (_rawData)
                        {
                            _rawData.AddRange(data);
                        }
                        //在此次可以对data进行按需处理
                        _serverSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);

                    }
                    else
                    {
                        this._serverSocket.Shutdown(SocketShutdown.Both);
                        this._serverSocket.Close();
                    }
                }

            }
            catch (SocketException ex)
            {
                if (_serverSocket.Connected)
                {
                    this._serverSocket.Shutdown(SocketShutdown.Both);
                    this._serverSocket.Close();
                }
            }
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

        private void ProcessRowData()
        {
            int dataLen = 45;
            int index = 0;
            while (_canProcessData)
            {
                try
                {
                    if (_rawData.Count >= dataLen)
                    {
                        #region LeftToRight

                        index = _rawData.IndexOf(0XF5) + 1;

                        byte[] tagData = new byte[25];

                        if (index == dataLen)
                        {
                            tagData = _rawData.GetRange(3, 25).ToArray();
                            if (onReceivedData != null)
                            {
                                string id = System.Text.ASCIIEncoding.UTF7.GetString(tagData);
                                onReceivedData(new List<string> { id });
                            }
                        }
                        else if (index > 0)
                        {
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
                    //WeightDeviceLogger.Error("数据线程处理内部异常，_comData=" + ByteArrayToHexString(_comData.ToArray()), ex);
                }
                finally
                {
                    //跳过已处理的数据
                    lock (_rawData)
                    {
                        if (index > 0)
                        {
                            _rawData.RemoveRange(0, index);
                            index = 0;
                        }
                    }
                }
            }
        }

        #region IRfidController
        /// <summary>
        /// 错误消息
        /// </summary>
        public event Action<string> OnShowErrorMsg;
        /// <summary>
        /// 数据处理事件
        /// </summary>
        public event ReceivedData onReceivedData;
        /// <summary>
        /// 打卡初始化RFID设备
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool rtn = false;
            try
            {
                _serverSocket.Send(openCmd);
                rtn = true;
            }
            catch
            {

            }
            return rtn;
        }
        /// <summary>
        /// 启动数据扫描功能
        /// </summary>
        /// <param name="tIp"></param>
        /// <param name="tPort"></param>
        /// <returns></returns>
        public bool Start()
        {
            if (!_canProcessData)
            {
                ThreadStart ts = new ThreadStart(ProcessRowData);
                t = new Thread(ts);
                t.IsBackground = true;
                t.Start();
            }
            _canProcessData = true;

            return true;
        }
        /// <summary>
        /// 关闭扫描功能
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            _canProcessData = false;
            return true;
        }

        /// <summary>
        /// 注销RFID设备
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            bool rtn = false;
            try
            {
                //停止处理标签数据
                if (t != null && t.IsAlive)
                {
                    _canProcessData = false;
                    t.Abort();
                }
                //发送关闭设备命令
                _serverSocket.Send(closeCmd);
                Thread.Sleep(100);
                //关闭通信
                this._serverSocket.Shutdown(SocketShutdown.Both);
                this._serverSocket.Close();
                //关闭监听
                if (tsListen != null && tsListen.IsAlive)
                {
                    this._listenSocket.Shutdown(SocketShutdown.Both);
                    this._listenSocket.Close();
                    tsListen.Abort();
                }

                rtn = true;
            }
            catch
            {

            }
            return rtn;
        }

        /// <summary>
        /// 读取卡设备源编号即卡号
        /// </summary>
        /// <returns></returns>
        public string ReadRFIDCardOrgNo()
        {
            return "";
        }


        public byte GetCRCValue(byte[] pData)
        {
            byte temp = 0;
            byte[] tData = pData;
            foreach (byte item in tData)
            {
                temp += item;
            }
            return (byte)(0X7F & temp);
        }
        #endregion
    }
}
