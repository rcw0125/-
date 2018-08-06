using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.Measure.DomainModel.ServiceModel;
using Talent.ClientCommonLib;

namespace Talent.RemoteCarMeasure.Model
{
    public class WeightRecordModel : Only_ViewModelBase
    {
        private string _recordtime;
        /// <summary>
        /// 记录时间(带毫秒)
        /// </summary>
        public string recordtime
        {
            set 
            { 
                _recordtime = value; 
                this.RaisePropertyChanged("recordtime");
                if (!string.IsNullOrEmpty(value))
                {
                    //WeightTime = DateTime.Parse(value).ToString("yyyy-MM-dd HH:mm:ss");
                    WeightTime = DateTime.Parse(value).ToString("HH:mm:ss");
                }
            }
            get { return _recordtime;}
        }
        private string weightTime;
        /// <summary>
        /// 记录时间(不带毫秒)
        /// </summary>
        public string WeightTime
        {
            get { return weightTime; }
            set { weightTime = value; }
        }

        /// <summary>
        /// 重量记录
        /// </summary>
        private decimal _recorddata;
        public decimal recorddata
        {
            set { _recorddata = value; this.RaisePropertyChanged("recorddata"); }
            get { return _recorddata; }
        }

        private int _weightCount;
        /// <summary>
        /// 重量出现次数
        /// </summary>
        public int weightCount
        {
            get { return _weightCount; }
            set { _weightCount = value; this.RaisePropertyChanged("weightCount"); }
        }

    }
}
