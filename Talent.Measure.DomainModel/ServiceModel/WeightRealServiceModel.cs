using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.ServiceModel
{
    /// <summary>
    /// 实时重量数据服务器返回的模型
    /// </summary>
    public class WeightRealServiceModel
    {
        private bool _success;
        public bool success
        {
            set { _success = value; }
            get { return _success; }
        }
        private string _msg;
        public string msg
        {
            set { _msg = value; }
            get { return _msg; }
        }
        private string _mtype;
        /// <summary>
        /// 模块类型
        /// </summary>
        public string mtype
        {
            get { return _mtype; }
            set { _mtype = value; }
        }
        /// <summary>
        /// 0自动计量，1手动，2远程
        /// </summary>
        private int _mfunc;
        public int mfunc
        {
            set { _mfunc = value; }
            get { return _mfunc; }
        }
        private object _data;
        public object data
        {
            set { _data = value; }
            get { return _data; }
        }
        private List<WeightRealData> _rows;
        /// <summary>
        /// 业务信息集合
        /// </summary>
        public List<WeightRealData> rows
        {
            set { _rows = value; }
            get { return _rows; }
        }
        private List<flagMsg> _flags;
        /// <summary>
        /// 业务调用过程错误信息
        /// </summary>
        public List<flagMsg> flags
        {
            set { _flags = value; }
            get { return _flags; }
        }
        private int _total;
        public int total
        {
            set { _total = value; }
            get { return _total; }
        }
        private List<hardwarectrlCls> _hardwarectrl;
        /// <summary>
        /// 硬件设置
        /// </summary>
        public List<hardwarectrlCls> hardwarectrl
        {
            set { _hardwarectrl = value; }
            get { return _hardwarectrl; }
        }
    }
}
