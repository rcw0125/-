using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.ServiceModel
{
    public class TaskCodeModel : INotifyPropertyChanged
    {
        //"taskcode":"001435","sourcecode":"54","sourcename":"炼铁2#炉","materialcode":"GZH","materialname":"干渣","targetcode":"22","targetname":"山东莱芜昌盛"
        private string _taskcode;
        /// <summary>
        /// 业务号
        /// </summary>
        public string taskcode
        {
            get { return _taskcode; }
            set { _taskcode = value; }
        }
        //this.SendPropertyChanged("basket");
        private string _sourcecode;
        /// <summary>
        /// 来源编号
        /// </summary>
        public string sourcecode
        {
            get { return _sourcecode; }
            set { _sourcecode = value; }
        }

        private string _sourcename;
        /// <summary>
        /// 来源名称
        /// </summary>
        public string sourcename
        {
            get { return _sourcename; }
            set { _sourcename = value; }
        }

        private string _materialcode;
        /// <summary>
        /// 物料编号
        /// </summary>
        public string materialcode
        {
            get { return _materialcode; }
            set { _materialcode = value; }
        }
        private string _materialname;
        /// <summary>
        /// 物料名称
        /// </summary>
        public string materialname
        {
            get { return _materialname; }
            set { _materialname = value; }
        }
        private string _targetcode;
        /// <summary>
        /// 目的地编码
        /// </summary>
        public string targetcode
        {
            get { return _targetcode; }
            set { _targetcode = value; }
        }

        private string _targetname;
        /// <summary>
        /// 目的地名称
        /// </summary>
        public string targetname
        {
            get { return _targetname; }
            set { _targetname = value; }
        }
        private string _operatype;
        /// <summary>
        /// 业务类型
        /// </summary>
        public string operatype
        {
            get { return _operatype; }
            set { _operatype = value; }
        }
        private string _mqflag;
        /// <summary>
        /// 
        /// </summary>
        public string mqflag
        {
            get { return _mqflag; }
            set { _mqflag = value; }
        }
        private string _accountstype;
        /// <summary>
        /// 
        /// </summary>
        public string accountstype
        {
            get { return _accountstype; }
            set { _accountstype = value; }
        }
        private string _kqflag;
        /// <summary>
        /// 
        /// </summary>
        public string kqflag
        {
            get { return _kqflag; }
            set { _kqflag = value; }
        }
        private string _gflag;
        /// <summary>
        /// 
        /// </summary>
        public string gflag
        {
            get { return _gflag; }
            set { _gflag = value; }
        }
        private string _tarehour;
        /// <summary>
        /// 
        /// </summary>
        public string tarehour
        {
            get { return _tarehour; }
            set { _tarehour = value; }
        }
        private string _printsetgross;
        /// <summary>
        /// 
        /// </summary>
        public string printsetgross
        {
            get { return _printsetgross; }
            set { _printsetgross = value; }
        }
        private string _printsetsuttle;
        /// <summary>
        /// 
        /// </summary>
        public string printsetsuttle
        {
            get { return _printsetsuttle; }
            set { _printsetsuttle = value; }
        }
        private string _printsettare;
        /// <summary>
        /// 
        /// </summary>
        public string printsettare
        {
            get { return _printsettare; }
            set { _printsettare = value; }
        }
        private string _deduction2;
        /// <summary>
        /// 
        /// </summary>
        public string deduction2
        {
            get { return _deduction2; }
            set { _deduction2 = value; }
        }
        private string _deductionunit;
        /// <summary>
        /// 
        /// </summary>
        public string deductionunit
        {
            get { return _deductionunit; }
            set { _deductionunit = value; }
        }
        private string _deductiontype;
        /// <summary>
        /// 
        /// </summary>
        public string deductiontype
        {
            get { return _deductiontype; }
            set { _deductiontype = value; }
        }
        private string _isplan;
        /// <summary>
        /// 
        /// </summary>
        public string isplan
        {
            get { return _isplan; }
            set { _isplan = value; }
        }
        private string _isbasket;
        /// <summary>
        /// 
        /// </summary>
        public string isbasket
        {
            get { return _isbasket; }
            set { _isbasket = value; }
        }
        private string _forcereceive;
        /// <summary>
        /// 
        /// </summary>
        public string forcereceive
        {
            get { return _forcereceive; }
            set { _forcereceive = value; }
        }
        private string _measuretype;
        /// <summary>
        /// 
        /// </summary>
        public string measuretype
        {
            get { return _measuretype; }
            set { _measuretype = value; }
        }
        private string _mflag;
        /// <summary>
        /// 
        /// </summary>
        public string mflag
        {
            get { return _mflag; }
            set { _mflag = value; }
        }

        private string _operaname;
        /// <summary>
        /// 业务类型
        /// </summary>
        public string operaname
        {
            get { return _operaname; }
            set { _operaname = value; }
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
