using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.ClientCommonLib;

namespace Talent.RemoteCarMeasure.Model
{
    /// <summary>
    /// 计量客户端信息model
    /// </summary>
    public class ClientInfo : Only_ViewModelBase
    {
        private string clientId;
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId
        {
            get { return clientId; }
            set { clientId = value; this.RaisePropertyChanged("ClientId"); }
        }
        private string clientName;
        /// <summary>
        /// 客户端名称
        /// </summary>
        public string ClientName
        {
            get { return clientName; }
            set { clientName = value; this.RaisePropertyChanged("ClientName"); }
        }

        private string redIconSource;
        /// <summary>
        /// 红灯小图标资源地址
        /// </summary>
        public string RedIconSource
        {
            get { return redIconSource; }
            set { redIconSource = value; this.RaisePropertyChanged("RedIconSource"); }
        }

        private string greenIconSource;
        /// <summary>
        /// 绿灯小图标资源地址
        /// </summary>
        public string GreenIconSource
        {
            get { return greenIconSource; }
            set { greenIconSource = value; this.RaisePropertyChanged("GreenIconSource"); }
        }

        private bool isRedLight;
        /// <summary>
        /// 是否红灯亮
        /// </summary>
        public bool IsRedLight
        {
            get { return isRedLight; }
            set 
            { 
                isRedLight = value;
                this.RaisePropertyChanged("IsRedLight");
                if (value)
                {
                    RedIconSource = "/Talent.RemoteCarMeasure;component/Image/SysImage/BtnClose2.png";
                }
                else
                {
                    RedIconSource = "/Talent.RemoteCarMeasure;component/Image/SysImage/BtnClose1.png";
                }
            }
        }

        private bool isGreenLight;
        /// <summary>
        /// 是否是绿灯亮
        /// </summary>
        public bool IsGreenLight
        {
            get { return isGreenLight; }
            set 
            { 
                isGreenLight = value;
                this.RaisePropertyChanged("IsGreenLight");
                if (value)
                {
                    GreenIconSource = "/Talent.RemoteCarMeasure;component/Image/SysImage/+2.png";
                }
                else
                {
                    GreenIconSource = "/Talent.RemoteCarMeasure;component/Image/SysImage/+1.png";
                }
            }
        }


        private string weight;
        /// <summary>
        /// 重量
        /// </summary>
        public string Weight
        {
            get { return weight; }
            set { weight = value; this.RaisePropertyChanged("Weight"); }
        }

        private string state;
        /// <summary>
        /// 状态(1:正在计量,2:无计量任务)
        /// </summary>
        public string State
        {
            get { return state; }
            set 
            { 
                state = value;
                this.RaisePropertyChanged("State");
                if (!string.IsNullOrEmpty(value))
                {
                    do
                    {
                        if (value.Equals("1"))
                        {
                            StateInfo = "正在计量...";
                            break;
                        }
                        if (value.Equals("2"))
                        {
                            StateInfo = "无计量任务...";
                            break;
                        }
                    } while (false);
                }
            }
        }

        private string stateInfo;
        /// <summary>
        /// 状态信息
        /// </summary>
        public string StateInfo
        {
            get { return stateInfo; }
            set { stateInfo = value; this.RaisePropertyChanged("StateInfo"); }
        }

    }
}
