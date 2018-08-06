
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talent.ClinetLog;
using Talent.Measure.DomainModel;


namespace Talent.Ic.MH
{
    /// <summary>
    /// 明华IC卡读卡器
    /// </summary>
    public class IcController : IIcController
    {
        static readonly Dictionary<int, string> _dicErr;
        static IcController()
        {
            _dicErr = new Dictionary<int, string>();
            _dicErr.Add(0X01, "无卡");
            _dicErr.Add(0X02, "CRC校验错");
            _dicErr.Add(0X03, "值溢出");
            _dicErr.Add(0X04, "未验证密码");
            _dicErr.Add(0X05, "奇偶校验错");
            _dicErr.Add(0X06, "通讯出错");
            //_dicErr.Add(0X07,"无卡");
            _dicErr.Add(0X08, "错误的序列号");
            _dicErr.Add(10, "验证密码失败");
            _dicErr.Add(11, "接收的数据位错误");
            _dicErr.Add(12, "接收的数据字节错误");
            _dicErr.Add(14, "Transfer错误");
            _dicErr.Add(15, "写失败");
            _dicErr.Add(16, "加值失败");

            _dicErr.Add(17, "减值失败");
            _dicErr.Add(18, "读失败");
            _dicErr.Add(-0x10, "PC与读写器通讯错误");
            _dicErr.Add(-0x11, "通讯超时");
            _dicErr.Add(-0X20, "打开串口失败");

            _dicErr.Add(-0x24, "串口已被占用");
            _dicErr.Add(-0x30, "地址格式错误");
            _dicErr.Add(-0X31, "该块数据不是值格式");
            _dicErr.Add(-0x32, "PC长度错误");
            _dicErr.Add(-0x40, "值操作失败");
            _dicErr.Add(-0X50, "卡中的值不够减");
        }
        bool _isDebug = false;

        /// <summary>
        /// 是否正在寻卡
        /// </summary>
        bool _searchCard = false;
        /// <summary>
        /// 卡号
        /// </summary>
        uint cardNo = 0;
        /// <summary>
        /// 监控时未读取到卡片次数
        /// </summary>
        int _monitorCardNumNoReadCount = 0;
        /// <summary>
        /// 是否允许读卡
        /// </summary>
        bool _canReadCard = false;
        /// <summary>
        /// 通讯设备标识符
        /// </summary>
        int _icDev = -1;
        /// <summary>
        /// 读卡计时器
        /// </summary>
        Thread threadICCardReader;
        /// <summary>
        /// IC卡配置
        /// </summary>
        ICCard _icCardCfg;
        /// <summary>
        /// 读取卡号
        /// </summary>
        public event ReadCardNo OnReadCardNo;

        /// <summary>
        /// 移卡
        /// </summary>
        public event RemoveCard OnRemoveCard;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pICCard">IC卡配置</param>
        /// <param name="pCanReadCard">是否允许读卡</param>
        public IcController(ICCard pICCard, bool pCanReadCard)
        {
            // _log = LogHelper.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\Log4Net.config", "IC_LOG");
            _canReadCard = pCanReadCard;
            _icCardCfg = pICCard;
        }


        /// <summary>
        /// 读卡回调函数
        /// </summary>
        /// <param name="paramObject"></param>
        private void Alarm()
        {
            int temp = 1000;//默认寻卡时间
            int.TryParse(_icCardCfg.Interval, out temp);
            IcCardReaderLogger.Debug("读取IC读卡器寻卡时间！Interval=" + temp);
            while (_searchCard)
            {
                short result = Rf32Controller.rf_card(_icDev, 0, out cardNo);
                IcCardReaderLogger.Debug("IC读卡器寻卡！result=" + result.ToString());
                if (result == 0)
                {
                    Rf32Controller.rf_beep(_icDev, 20);
                    IcCardReaderLogger.Debug("读卡成功");
                    if (OnReadCardNo != null && _canReadCard)
                    {
                        if (_isDebug)
                        {
                            OnReadCardNo(_icCardCfg.ComPort, "B35CADE2");
                        }
                        else
                        {
                            IcCardReaderLogger.Debug(string.Format("触发读卡事件。ComPort={0},cardNo={1}", _icCardCfg.ComPort, cardNo.ToString()));
                            OnReadCardNo(_icCardCfg.ComPort, cardNo.ToString());
                        }
                    }

                    ////启动监控定时器
                    //TimerCallback callBack = new TimerCallback(MonitorCard);
                    //_monitorCardNoTimer = new Timer(MonitorCard, null, Timeout.Infinite, 500);
                    //_monitorCardNoTimer.Change(100, 500);

                }
                Thread.Sleep(temp);
            }
        }

        /// <summary>
        /// 读卡回调函数
        /// </summary>
        /// <param name="paramObject"></param>
        private void MonitorCard(object paramObject)
        {
            uint cardNo;
            short result = Rf32Controller.rf_card(_icDev, 0, out cardNo);

            //Debug.WriteLine("jianshi:" + result);
            if (result != 0)
            {
                _monitorCardNumNoReadCount++;
            }
            else
            {
                // Rf32Controller.rf_reset(_icDev, 3);
                _monitorCardNumNoReadCount = 0;
            }
            if (_monitorCardNumNoReadCount >= 3)
            {
                //触发移卡事件
                if (OnRemoveCard != null)
                {
                    //_monitorCardNoTimer.Change(-1, -1);
                    _monitorCardNumNoReadCount = 0;
                    OnRemoveCard(_icCardCfg.ComPort);
                }
            }
        }
        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="pMsg"></param>
        private void ShowMsg(string pMsg)
        {
            if (OnShowErrMsg != null)
            {
                OnShowErrMsg(pMsg);
            }
        }

        #region IIcController
        public event Action<string> OnShowErrMsg;
        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool rtn = false;
            try
            {
                if (_icDev > 0)
                {
                    this.Stop();
                    this.Close();
                    Thread.Sleep(1000);
                }
                _icDev = Rf32Controller.rf_init(short.Parse(_icCardCfg.ComPort), int.Parse(_icCardCfg.Baudrate.Substring(0, _icCardCfg.Baudrate.Length - 3)));

                if (_icDev > 0)
                {
                    IcCardReaderLogger.Debug("打开IC读卡器成功！");
                    Rf32Controller.rf_beep(_icDev, 20);
                    IcCardReaderLogger.Debug("IC读卡器播放声音！");
                    rtn = true;
                }
                else
                {
                    IcCardReaderLogger.Error("打开IC读卡器失败！_icDev=" + _icDev.ToString());
                    string msg = GetErrMsgByErrCode(_icDev);
                    ShowMsg(msg);
                }
            }
            catch (Exception ex)
            {
                IcCardReaderLogger.Error("打开IC读卡器异常！", ex);
                ShowMsg("IC读卡器初始化异常.");
            }
            return rtn;
        }
        /// <summary>
        /// 开始
        /// </summary>
        public bool Start()
        {
            bool rtn = true;
            try
            {
                if (_icDev <= 0)
                {
                    IcCardReaderLogger.Debug("在启动时打开IC读卡器！");
                    rtn = Open();
                }

                if (_icDev > 0)
                {
                    if (threadICCardReader != null)
                    {
                        while (threadICCardReader.IsAlive)
                        {
                            threadICCardReader.Abort();
                            IcCardReaderLogger.Debug("强制结束IC读卡器寻卡线程。");
                        }
                       
                    }
                    threadICCardReader = new Thread(new ThreadStart(Alarm));
                    threadICCardReader.IsBackground = true;
                    _searchCard = true;
                    threadICCardReader.Start();
                    IcCardReaderLogger.Debug("启动IC读卡器寻卡线程。");
                }
                else
                {
                    rtn = false;
                    IcCardReaderLogger.Debug("设备故障，无法启动寻卡线程。_icDev=" + _icDev.ToString());
                    ShowMsg(string.Format("IC卡打开失败，返回值={0}", _icDev.ToString()));
                }
            }
            catch (Exception ex)
            {
                rtn = false;
                IcCardReaderLogger.Error("启动IC读卡器寻卡异常！", ex);
                throw ex;
            }
            return rtn;
        }
        /// <summary>
        /// 停止读取IC卡
        /// </summary>
        public bool Stop()
        {
            IcCardReaderLogger.Debug("停止IC读卡器寻卡！_searchCard=" + _searchCard.ToString());
            _searchCard = false;
            return true;
        }

        /// <summary>
        /// 注销设备
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            bool rtn = false;
            IcCardReaderLogger.Debug("关闭IC卡读卡器！_icDev=" + _icDev.ToString());
            if (_icDev > 0)
            {

                if (Rf32Controller.rf_exit(_icDev) == 0) ;
                {
                    IcCardReaderLogger.Debug("关闭IC卡读卡器成功！");
                    _icDev = -1;
                }
                rtn = true;
                //_log.Info(string.Format("关闭退出IC卡定时器，退出成功。", _icDev));
            }
            return rtn;
        }
        #endregion

        /// <summary>
        /// 读取卡号
        /// </summary>
        /// <returns></returns>
        public string ReadCardOrgNo()
        {
            string result = string.Empty;
            try
            {
                if (this._icDev <= 0)
                {
                    this.Open();
                }
                if (this._icDev > 0)
                {
                    short num = Rf32Controller.rf_card(this._icDev, 0, out this.cardNo);
                    if (num == 0)
                    {
                        result = this.cardNo.ToString();
                        Rf32Controller.rf_beep(_icDev, 20);
                    }
                    else
                    {
                        IcCardReaderLogger.Error(string.Format("ReadCardOrgNo读卡失败，原因：{0}", num));
                    }
                }
                else
                {
                    IcCardReaderLogger.Error("ReadCardOrgNo打开IC读卡器失败！_icDev=" + _icDev.ToString());
                }
            }
            catch (Exception ex)
            {
                IcCardReaderLogger.Error("ReadCardOrgNo执行异常！_icDev=" + _icDev.ToString(),ex);
            }
            return result;
        }
        /// <summary>
        /// 获取错误编码
        /// </summary>
        /// <param name="pErrCode"></param>
        /// <returns></returns>
        private string GetErrMsgByErrCode(int pErrCode)
        {
            if (_dicErr.ContainsKey(pErrCode))
            {
                return _dicErr[pErrCode];
            }
            return string.Format("未知错误，错误码:{0}", pErrCode);
        }
    }
}
