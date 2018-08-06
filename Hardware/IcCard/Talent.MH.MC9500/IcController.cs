using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talent.Ic;
using Talent.Measure.DomainModel;

namespace Talent.MH.MC9500
{
    public class IcController : IIcController
    {
        private static readonly Dictionary<int, string> _dicErr;

        private FixRFIDReaderWrapper fw;

        private bool _isDebug = false;

        private uint cardNo = 0u;

        private int _monitorCardNumNoReadCount = 0;

        private bool _canReadCard = false;

        private Timer _readCardNoTimer;

        private Timer _monitorCardNoTimer;

        private ICCard _icCardCfg;

        public event ReadCardNo OnReadCardNo;

        public event RemoveCard OnRemoveCard;

        public event Action<string> OnShowErrMsg;

        public IcController(ICCard pICCard, bool pCanReadCard)
        {
            this._canReadCard = pCanReadCard;
            this._icCardCfg = pICCard;
            TimerCallback callback = new TimerCallback(this.Alarm);
            this._readCardNoTimer = new Timer(callback, null, -1, int.Parse(this._icCardCfg.Interval));
        }

        private void Alarm(object paramObject)
        {
            string text = this.fw.Excute();
            if (!string.IsNullOrEmpty(text))
            {
                if (this.OnReadCardNo != null && this._canReadCard)
                {
                    this.fw.Beep(1, 100);
                    if (this._isDebug)
                    {
                        this.OnReadCardNo.Invoke(this._icCardCfg.ComPort, "B35CADE2");
                    }
                    else
                    {
                        this.OnReadCardNo.Invoke(this._icCardCfg.ComPort, text);
                    }
                }
                this._readCardNoTimer.Change(-1, -1);
                TimerCallback timerCallback = new TimerCallback(this.MonitorCard);
                this._monitorCardNoTimer = new Timer(new TimerCallback(this.MonitorCard), null, -1, 500);
                this._monitorCardNoTimer.Change(100, 500);
            }
        }

        private void MonitorCard(object paramObject)
        {
            string value = this.fw.Excute();
            if (string.IsNullOrEmpty(value))
            {
                this._monitorCardNumNoReadCount++;
            }
            else
            {
                this._monitorCardNumNoReadCount = 0;
            }
            if (this._monitorCardNumNoReadCount >= 3)
            {
                if (this.OnRemoveCard != null)
                {
                    this._monitorCardNoTimer.Change(-1, -1);
                    this._monitorCardNumNoReadCount = 0;
                    this.OnRemoveCard.Invoke(this._icCardCfg.ComPort);
                }
            }
        }

        private void ShowMsg(string pMsg)
        {
            if (this.OnShowErrMsg != null)
            {
                this.OnShowErrMsg(pMsg);
            }
        }

        public bool Open()
        {
            bool result = false;
            try
            {
                this.fw = new FixRFIDReaderWrapper((int)(short.Parse(this._icCardCfg.ComPort) + 1), int.Parse(this._icCardCfg.Baudrate));
                bool flag = this.fw.OpenDevice();
                if (flag)
                {
                    this.fw.Beep(1, 100);
                    result = true;
                }
                else
                {
                    this.ShowMsg("MC9500 打开失败");
                }
            }
            catch (Exception ex)
            {
                this.ShowMsg("MC9500 卡读卡器初始化异常:" + ex.Message);
            }
            return result;
        }

        public bool Start()
        {
            bool result = true;
            try
            {
                if (this.fw == null)
                {
                    result = this.Open();
                }
                if (this.fw != null)
                {
                    if (!this._readCardNoTimer.Change(0, int.Parse(this._icCardCfg.Interval)))
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                    this.ShowMsg(string.Format("MC9500 IC卡打开失败", new object[0]));
                }
            }
            catch (Exception var_1_77)
            {
                result = false;
            }
            return result;
        }

        public string ReadCardOrgNo()
        {
            string result = string.Empty;
            try
            {
                if (this.fw == null)
                {
                    this.Open();
                }
                if (this.fw != null)
                {
                    string text = this.fw.Excute();
                    if (!string.IsNullOrEmpty(text))
                    {
                        result = text;
                    }
                }
                else
                {
                    this.ShowMsg(string.Format("MC9500 IC卡打开失败，初始化为空", new object[0]));
                }
            }
            catch (Exception ex)
            {
                this.ShowMsg("MC9500 IC卡执行ReadCardOrgNo功能异常：" + ex.Message);
            }
            return result;
        }

        public bool Stop()
        {
            bool result = true;
            try
            {
                if (this.fw != null)
                {
                    if (this._monitorCardNoTimer != null)
                    {
                        this._monitorCardNoTimer.Change(-1, -1);
                    }
                    if (this._readCardNoTimer != null)
                    {
                        this._readCardNoTimer.Change(-1, -1);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                this.ShowMsg("MC9500 停止IC卡读写功能异常:" + ex.Message);
            }
            return result;
        }

        public bool Close()
        {
            bool result = false;
            if (this.fw != null)
            {
                if (this._monitorCardNoTimer != null)
                {
                    this._monitorCardNoTimer.Change(-1, -1);
                }
                if (this._readCardNoTimer != null)
                {
                    this._readCardNoTimer.Change(-1, -1);
                }
                result = this.fw.CloseDevice();
            }
            return result;
        }
    }
}
