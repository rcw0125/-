
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talent.Rfid.Interface;
using Talent.Measure.DomainModel;
using System.Threading;
using Talent.Measure.DomainModel.CommonModel;
using Talent_LT.HelpClass;
using ReaderB;
using Talent.ClinetLog;
using System.Runtime.ExceptionServices;

namespace Talent.Rfid.Ljyzn102
{
    /// <summary>
    /// 陆加壹 ljyzn  102  串口 包括网口  
    /// </summary>
    public class RfidController : IRfidController
    {

        /// <summary>
        /// 设备是否已经打开
        /// </summary>
        bool _isDeviceOpen = false;

        /// <summary>
        /// 是否正在寻卡
        /// </summary>
        bool _searchCard = false;

        Thread tsRfidReader;
        /// <summary>
        /// 设备支持的波特率
        /// </summary>
        static readonly List<int> baudrateList = new List<int> { 9600, 19200, 38400, 56000, 57600, 115200 };

        /// <summary>
        /// 读写器的地址，默认设置为广播地址
        /// </summary>
        byte _readerAddr = 255;
        /// <summary>
        /// 读写器连接端口对应的句柄
        /// </summary>
        int _readerHandel;
        /// <summary>
        /// com口
        /// </summary>
        int comPort;
        /// <summary>
        /// rfid读写器配置
        /// </summary>
        public RfidCfg _curRfidCfg;
        public RfidController(RfidCfg pRfidCfg)
        {
            _curRfidCfg = pRfidCfg;
            RFIDReaderLogger.Debug(string.Format("实例化RFID读写器。IP={0}，端口={1}", pRfidCfg.Ip, pRfidCfg.Port), pRfidCfg.Ip);
        }

        /// <summary>
        /// 读取标签数据
        /// </summary>
        private void ReadData()
        {
            while (_searchCard)
            {
                List<string> cardIds = ReadCardId();
               
                if (cardIds.Count > 0 && onReceivedData != null)
                {
                    RFIDReaderLogger.Debug("读取到卡片数据,并触发onReceivedData事件。cardIds=" + string.Join(",", cardIds.ToArray()), _curRfidCfg.Ip);
                    onReceivedData(cardIds);
                }
                Thread.Sleep(_curRfidCfg.Interval);
            }
        }

        /// <summary>
        ///  打开读写器
        /// </summary>
        /// <param name="pRfidCfg"></param>
        private bool OpenReader(RfidCfg pRfidCfg)
        {
            bool rtn = false;
            try
            {
                if (pRfidCfg.ConType == DeviceConType.Net)
                {
                    int temp = ReaderB.StaticClassReaderB.OpenNetPort(int.Parse(pRfidCfg.Port), pRfidCfg.Ip, ref _readerAddr, ref _readerHandel);
                    if (temp != 0)
                    {
                        RFIDReaderLogger.Error(string.Format("打开RFID读写器失败。IP={0}，端口={1}", pRfidCfg.Ip, pRfidCfg.Port), pRfidCfg.Ip);
                    }
                    else
                    {
                        rtn = true;
                    }
                }
                else //if (pRfidCfg.ConType == "串口")
                {
                    byte indexBaud = GetBaudrateIndex(pRfidCfg.Baudrate);
                    comPort = int.Parse(pRfidCfg.ComPort.ToUpper().Replace("COM", ""));
                    if (indexBaud != byte.MaxValue)
                    {

                        int temp = StaticClassReaderB.OpenComPort(comPort, ref _readerAddr, indexBaud, ref _readerHandel);
                        if (temp !=0)
                        {
                            RFIDReaderLogger.Error(string.Format("打开RFID读写器失败。ComPort={0}", pRfidCfg.ComPort), pRfidCfg.ComPort);
                        }
                        else
                        {
                            rtn = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RFIDReaderLogger.Error("连接RFID读写器异常。", pRfidCfg.Ip, ex);
            }

            return rtn;
        }

        /// <summary>
        /// 获取某某波特率对应的索引
        /// </summary>
        /// <param name="pBaud"></param>
        /// <returns></returns>
        private byte GetBaudrateIndex(string pBaud)
        {
            byte rtn;
            pBaud = pBaud.IndexOf("bps") >= 0 ? pBaud.Replace("bps", "").Trim() : pBaud.Trim();
            int temp = int.Parse(pBaud);
            int index = baudrateList.FindIndex(m => m == temp);
            if (index == -1)
            {
                rtn = byte.MaxValue;
            }
            else
            {
                rtn = byte.Parse(index.ToString());
            }
            return rtn;
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
        // public ly
        #region 接口实现--IRfidController
        public event ReceivedData onReceivedData;
        public event Action<string> OnShowErrorMsg;

        /// <summary>
        /// 连接初始化设备
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool rtn = false;
            //FileHelpClass.WriteLog("Ljyzn102准备打开RFID设备:" + _curRfidCfg.IsUse, "RFID");
            if (_curRfidCfg != null && _curRfidCfg.IsUse)
            {
                //FileHelpClass.WriteLog(string.Format("Ljyzn102读写器连接方式：{0}", _curRfidCfg.ConType), "RFID");
                if (!OpenReader(_curRfidCfg))
                {
                    //FileHelpClass.WriteLog("Ljyzn102启动RFID设备失败。", "RFID");
                    ShowErrorMsg("打开RFID设备失败。");
                }
                else
                {
                    _isDeviceOpen = true;
                    rtn = true;
                    RFIDReaderLogger.Debug("打开RFID设备成功。", _curRfidCfg.Ip);
                }
            }
            else
            {
                RFIDReaderLogger.Error("打开RFID设备失败，配置信息不存在或读写器未启用。", _curRfidCfg.Ip);
                rtn = false;
            }
            return rtn;
        }
        /// <summary>
        /// 启动定时读取功能
        /// </summary>
        /// <param name="tIp"></param>
        /// <param name="tPort"></param>
        /// <returns></returns>
        public bool Start()
        {
            bool rtn = false;
            if (!_isDeviceOpen)
            {
                Open();
            }
            if (_isDeviceOpen)
            {
                try
                {
                    if (_isDeviceOpen)
                    {
                        if (tsRfidReader != null)
                        {
                            while (tsRfidReader.IsAlive)
                            {
                                tsRfidReader.Abort();
                                RFIDReaderLogger.Debug("启动寻卡时先强制结束寻卡线程。", _curRfidCfg.Ip);
                            }
                        }
                        //
                        tsRfidReader = new Thread(new ThreadStart(ReadData));
                        tsRfidReader.IsBackground = true;
                        _searchCard = true;
                        tsRfidReader.Start();
                        RFIDReaderLogger.Debug("启动寻卡线程。", _curRfidCfg.Ip);
                        rtn = true;
                    }
                    else
                    {
                        RFIDReaderLogger.Debug("启动寻卡时设备未打开。", _curRfidCfg.Ip);
                    }
                }
                catch (Exception ex)
                {
                    RFIDReaderLogger.Error("启动寻卡时异常。", _curRfidCfg.Ip,ex);
                }
            }
            return rtn;

        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            RFIDReaderLogger.Debug("停止寻卡。_searchCard=" + _searchCard.ToString(), _curRfidCfg.Ip);
            _searchCard = false;
            return true;
        }

        /// <summary>
        /// 关闭注销设备
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            bool rtn = false;
            if (_isDeviceOpen)
            {
                Stop();
                try
                {
                    if (_curRfidCfg.ConType == DeviceConType.Net)
                    {
                        ReaderB.StaticClassReaderB.CloseNetPort(_readerHandel);
                    }
                    else//串口
                    {
                        ReaderB.StaticClassReaderB.CloseSpecComPort(comPort);
                        ReaderB.StaticClassReaderB.CloseComPort();
                       
                    }
                    rtn = true;
                    RFIDReaderLogger.Debug("关闭读卡器。", _curRfidCfg.Ip);
                }
                catch (Exception ex)
                {
                    RFIDReaderLogger.Error("关闭设备异常。", _curRfidCfg.Ip, ex);
                }
               
            }
            else
            {
                RFIDReaderLogger.Error("关闭设备时，设备未打开。", _curRfidCfg.Ip);
            }
            return rtn;
        }

        /// <summary>
        /// 读取一次卡号
        /// </summary>
        /// <returns></returns>
        public string ReadRFIDCardOrgNo()
        {
            string rtStr = string.Empty;
            List<string> rtList = ReadCardId();
            string cardInfos = string.Empty;
            for (int i = 0; i < rtList.Count; i++)
            {
                string cRfid = rtList[i];
                if (!cardInfos.Contains(cRfid))
                {
                    cardInfos = cRfid + "," + cardInfos;
                }
            }
            rtStr = string.IsNullOrEmpty(cardInfos) ? string.Empty : cardInfos.Substring(0, cardInfos.Length - 1);
            return rtStr;
        }
        #endregion

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="tPortList"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public List<string> ReadCardId()
        {
            List<string> rtn = new List<string>();
            try
            {
                int i;
                int CardNum = 0;
                int Totallen = 0;
                int EPClen, m;
                byte[] EPC = new byte[5000];
                int CardIndex;
                string temps;
                string sEPC;
                byte AdrTID = 0;
                byte LenTID = 0;
                byte TIDFlag = 0;
                int fCmdRet = ReaderB.StaticClassReaderB.Inventory_G2(ref _readerAddr, AdrTID, LenTID, TIDFlag, EPC, ref Totallen, ref CardNum, _readerHandel);
                //FileHelpClass.WriteLog("读取厂家接口程序返回：" + fCmdRet + "CardNum=" + CardNum, "RFID");
                if ((fCmdRet == 1) | (fCmdRet == 2) | (fCmdRet == 3) | (fCmdRet == 4) | (fCmdRet == 0xFB))//代表已查找结束，
                {
                    byte[] daw = new byte[Totallen];
                    Array.Copy(EPC, daw, Totallen);
                    temps = ByteArrayToHexString(daw);

                    m = 0;
                    for (CardIndex = 0; CardIndex < CardNum; CardIndex++)
                    {
                        EPClen = daw[m];
                        sEPC = temps.Substring(m * 2 + 2, EPClen * 2);
                        m = m + EPClen + 1;
                        if (sEPC.Length == EPClen * 2)
                        {
                            if (!rtn.Contains(sEPC))
                            {
                                rtn.Add(sEPC);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RFIDReaderLogger.Error("寻卡时，读取卡片ID异常。", _curRfidCfg.Ip, ex);
            }

            return rtn;
        }

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

