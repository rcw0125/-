using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Talent.ClientCommonLib.Controls;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 计量客户端模型
    /// added by wangchao on 20151023
    /// </summary>
    public class WeighterClientModel : INotifyPropertyChanged
    {
        private string clientId;
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId
        {
            get { return clientId; }
            set
            {
                clientId = value;
                this.SendPropertyChanged("ClientId");
            }
        }

        private LeftLightStates leftlightState;
        /// <summary>
        /// 左红绿灯状态
        /// </summary>
        public LeftLightStates LeftLightState
        {
            get { return leftlightState; }
            set
            {
                leftlightState = value;
                this.SendPropertyChanged("LeftLightState");
            }
        }
        private RightLightStates rightlightState;
        /// <summary>
        /// 右红绿灯状态
        /// </summary>
        public RightLightStates RightLightState
        {
            get { return rightlightState; }
            set
            {
                rightlightState = value;
                this.SendPropertyChanged("RightLightState");
            }
        }

        private Visibility leftLine;
        /// <summary>
        /// 左红外被挡时显示的"×"
        /// </summary>
        public Visibility LeftLine
        {
            get { return leftLine; }
            set
            {
                leftLine = value;
                this.SendPropertyChanged("LeftLine");
            }
        }

        private Visibility rightLine;
        /// <summary>
        /// 右红外被挡时显示的"×"
        /// </summary>
        public Visibility RightLine
        {
            get { return rightLine; }
            set
            {
                rightLine = value;
                this.SendPropertyChanged("RightLine");
            }
        }

        private WeighterStates clientState;
        /// <summary>
        /// 衡器状态
        /// </summary>
        public WeighterStates ClientState
        {
            get { return clientState; }
            set
            {
                clientState = value;
                this.SendPropertyChanged("ClientState");
            }
        }

        private string clientName;
        /// <summary>
        /// 衡器名称
        /// </summary>
        public string ClientName
        {
            get { return clientName; }
            set
            {
                clientName = value;
                this.SendPropertyChanged("ClientName");
            }
        }

        private decimal weight;
        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                this.SendPropertyChanged("Weight");
            }
        }

        private string equTag;
        /// <summary>
        /// 设备标示(W:称;L:左灯;R:右灯;LR:左右灯;RLL:左红外;RLR:右红外 P:打印机 CT:清除任务 RFID:RFID)
        /// </summary>
        public string EquTag
        {
            get { return equTag; }
            set
            {
                equTag = value;
                this.SendPropertyChanged("EquTag");
            }
        }
        private string clientConfigName;
        /// <summary>
        /// 客户端配置文件名称
        /// </summary>
        public string ClientConfigName
        {
            get { return clientConfigName; }
            set
            {
                clientConfigName = value;
                this.SendPropertyChanged("ClientConfigName");
            }
        }

        private string printState;
        /// <summary>
        /// 打印机状态
        /// </summary>
        public string PrintState
        {
            get { return printState; }
            set
            {
                printState = value;
            }
        }
        private string weightMsg;
        /// <summary>
        /// 称点异常信息
        /// </summary>
        public string WeightMsg
        {
            get { return weightMsg; }
            set
            {
                weightMsg = value;
                this.SendPropertyChanged("WeightMsg");
            }
        }
        private bool isStopTask;
        /// <summary>
        /// 是否结束任务
        /// </summary>
        public bool IsStopTask
        {
            get { return isStopTask; }
            set
            {
                isStopTask = value;
            }
        }

        private string rfidStrs;
        /// <summary>
        /// RFID数据
        /// </summary>
        public string RfidStrs
        {
            get { return rfidStrs; }
            set
            {
                rfidStrs = value;
            }
        }
        
        public virtual event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(System.String propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
