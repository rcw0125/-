using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel
{
    public class SeatAttentionWeightModel : INotifyPropertyChanged
    {
        private string _seatid;
        /// <summary>
        /// 坐席编号
        /// </summary>
        public string seatid
        {
            get { return _seatid; }
            set { _seatid = value; this.SendPropertyChanged("seatid"); }
        }
        private string _seatstate;
        /// <summary>
        /// 坐席状态(0:空闲;1:等待任务;2:分配任务;3:处理任务;4:暂停坐席)
        /// </summary>
        public string seatstate
        {
            get { return _seatstate; }
            set { _seatstate = value; this.SendPropertyChanged("seatstate"); }
        }
        private string _seattype;
        /// <summary>
        /// 坐席类型(RC:汽车;RT:火车;RI:铁水)
        /// </summary>
        public string seattype
        {
            get { return _seattype; }
            set { _seattype = value; this.SendPropertyChanged("seattype"); }
        }
        private string _seatright;
        /// <summary>
        /// 坐席权限
        /// </summary>
        public string seatright
        {
            get { return _seatright; }
            set { _seatright = value; this.SendPropertyChanged("seattype"); }
        }

        private string _seatip;
        /// <summary>
        /// 坐席IP
        /// </summary>
        public string seatip
        {
            get { return _seatip; }
            set { _seatip = value; this.SendPropertyChanged("seatip"); }
        }
        private string _seatname;
        /// <summary>
        /// 坐席名称
        /// </summary>
        public string seatname
        {
            get { return _seatname; }
            set { _seatname = value; this.SendPropertyChanged("seatname"); }
        }
        //private string _validflag;
        ///// <summary>
        ///// 有效标记(0:无效;1:有效)
        ///// </summary>
        //public string validflag
        //{
        //    get { return _validflag; }
        //    set 
        //    { 
        //        _validflag = value;
        //        this.SendPropertyChanged("validflag");
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            if (value.Equals("0"))
        //            {
        //                isvalid = false;
        //            }
        //            else
        //            {
        //                isvalid = true;
        //            }
        //        }
        //    }
        //}
        private string _equcode;
        /// <summary>
        /// 称点编码
        /// </summary>
        public string equcode
        {
            get { return _equcode; }
            set { _equcode = value; this.SendPropertyChanged("equcode"); }
        }
        private string _equname;
        /// <summary>
        /// 称点名称
        /// </summary>
        public string equname
        {
            get { return _equname; }
            set { _equname = value; this.SendPropertyChanged("equname"); }
        }
        private string _isinseat;
        /// <summary>
        /// 是否在该坐席内(是/否)
        /// </summary>
        public string isinseat
        {
            get { return _isinseat; }
            set
            {
                _isinseat = value;
                this.SendPropertyChanged("isinseat");
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Equals("是"))
                    {
                        IsChecked = true;
                    }
                    else
                    {
                        IsChecked = false;
                    }
                }
                else
                {
                    IsChecked = false;
                }
            }
        }
        private string _versionnum;
        /// <summary>
        /// 版本号
        /// </summary>
        public string versionnum
        {
            get { return _versionnum; }
            set { _versionnum = value; this.SendPropertyChanged("versionnum"); }
        }
        private bool isChecked;
        /// <summary>
        /// 是否包含
        /// </summary>
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; this.SendPropertyChanged("IsChecked"); }
        }
        //private bool _isvalid;
        ///// <summary>
        ///// 是否有效
        ///// </summary>
        //public bool isvalid
        //{
        //    get { return _isvalid; }
        //    set
        //    {
        //        _isvalid = value;
        //        this.SendPropertyChanged("isvalid");
        //    }
        //}

        public virtual event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(System.String propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
