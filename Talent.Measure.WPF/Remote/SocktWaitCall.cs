using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Talent.ClientCommMethod;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.Measure.WPF
{
    public class SocktWaitCall
    {
        AutoResetEvent loadCompleted = new AutoResetEvent(false);
        /// <summary>
        /// 重复发送次数
        /// </summary>
        int replyCount = 0;
        /// <summary>
        /// 重发时间间隔
        /// </summary>
        int reSendInterval = 1000;

        private int _setreplyCount;
        public int setreplyCount
        {
            set { _setreplyCount = value; }
            get { return _setreplyCount; }
        }
        public SocktWaitCall()
        {
            SocketCls.listenEvent += SocketCls_listenEvent;
        }
        int getRandomNum;
        bool hasStarted;
        #region Socket回调事件
        void SocketCls_listenEvent(object sender, CallBackEventArgs e)
        {
            switch (e.EventName)
            {
                case "reply":
                    GetReplyData(e.Message);
                    break;
            }
        }
        #endregion


        private void GetReplyData(string data)
        {
            if (data == getRandomNum.ToString())
            {
                hasStarted = false;
                SocketCls.listenEvent -= SocketCls_listenEvent;
               // loadCompleted.Set();
            }
        }
        public int endtask(string paramJson, int RandomNum)
        {
            hasStarted = true;
            getRandomNum = RandomNum;

            int time = 0;
            
            while (hasStarted)
            {
                if (setreplyCount <= replyCount)
                {
                    if (time >= reSendInterval)
                    {
                        int state = SocketCls.Emit(SeatlistenCmdEnum.endtask, paramJson);
                        time = 0;
                        setreplyCount++;
                    }
                    Thread.Sleep(50);
                    time += 50;
                    Application.DoEvents();
                   // loadCompleted.WaitOne(5000);
                }
                else
                {
                    SocketCls.listenEvent -= SocketCls_listenEvent;
                    return -1;
                }
            }
            return 0;

        }
        public int cmd2client(string paramJson, int RandomNum)
        {
            hasStarted = true;
            getRandomNum = RandomNum;
            int time = 0;
            while (hasStarted)
            {
                if (setreplyCount <= replyCount)
                {
                    if (time >= reSendInterval)
                    {
                        int state = SocketCls.Emit(SeatSendCmdEnum.cmd2client, paramJson);
                        time = 0;
                        setreplyCount++;
                    }
                    Thread.Sleep(50);
                    time += 50;
                    Application.DoEvents();
                    // loadCompleted.WaitOne(5000);
                }
                else
                {
                    SocketCls.listenEvent -= SocketCls_listenEvent;
                    return -1;
                }
            }
            return 0;

        }


    }
}
