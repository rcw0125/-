using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class WeightModel:TaskModel
    {
        private string jiMaoDate;
        /// <summary>
        /// 计毛时间
        /// </summary>
        public string JiMaoDate
        {
            get { return jiMaoDate; }
            set
            {
                jiMaoDate = value;
                this.SendPropertyChanged("JiMaoDate");
            }
        }

        private string jiPiDate;
        /// <summary>
        /// 计皮时间
        /// </summary>
        public string JiPiDate
        {
            get { return jiPiDate; }
            set
            {
                jiPiDate = value;
                this.SendPropertyChanged("JiPiDate");
            }
        }

        private string chuPiaoDate;
        /// <summary>
        /// 出票时间
        /// </summary>
        public string ChuPiaoDate
        {
            get { return chuPiaoDate; }
            set
            {
                chuPiaoDate = value;
                this.SendPropertyChanged("ChuPiaoDate");
            }
        }

        private string jiMaoWeighter;
        /// <summary>
        /// 计毛衡器
        /// </summary>
        public string JiMaoWeighter
        {
            get { return jiMaoWeighter; }
            set
            {
                jiMaoWeighter = value;
                this.SendPropertyChanged("JiMaoWeighter");
            }
        }

        private string jiPiWeighter;
        /// <summary>
        /// 计皮衡器
        /// </summary>
        public string JiPiWeighter
        {
            get { return jiPiWeighter; }
            set
            {
                jiPiWeighter = value;
                this.SendPropertyChanged("JiPiWeighter");
            }
        }

        private string chuPiaoWeighter;
        /// <summary>
        /// 出票衡器
        /// </summary>
        public string ChuPiaoWeighter
        {
            get { return chuPiaoWeighter; }
            set
            {
                chuPiaoWeighter = value;
                this.SendPropertyChanged("ChuPiaoWeighter");
            }
        }

        private string jiMaoOperater;
        /// <summary>
        /// 计毛计量员
        /// </summary>
        public string JiMaoOperater
        {
            get { return jiMaoOperater; }
            set
            {
                jiMaoOperater = value;
                this.SendPropertyChanged("JiMaoOperater");
            }
        }
        private string jiPiOperater;
        /// <summary>
        /// 计皮计量员
        /// </summary>
        public string JiPiOperater
        {
            get { return jiPiOperater; }
            set
            {
                jiPiOperater = value;
                this.SendPropertyChanged("JiPiOperater");
            }
        }

        //public virtual event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void SendPropertyChanged(System.String propertyName)
        //{
        //    var handler = this.PropertyChanged;
        //    if (handler != null)
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
