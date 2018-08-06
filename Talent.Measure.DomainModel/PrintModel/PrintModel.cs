using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel
{
    /// <summary>
    /// 打印信息模型
    /// </summary>
    public class PrintInfo
    {
        private string _matchid;
        /// <summary>
        /// 过磅单号
        /// </summary>
        public string matchid
        {
            get { return _matchid; }
            set { _matchid = value; }
        }
        private string _opname;
        /// <summary>
        /// 操作人名称
        /// </summary>
        public string opname
        {
            get { return _opname; }
            set
            {
                _opname = value;
            }
        }
        private string _opcode;
        /// <summary>
        /// 操作人编码
        /// </summary>
        public string opcode
        {
            get { return _opcode; }
            set { _opcode = value; }
        }
        private string _clientCode;
        /// <summary>
        /// 称点编码
        /// </summary>
        public string clientcode
        {
            get { return _clientCode; }
            set { _clientCode = value; }
        }
        private string _clientName;
        /// <summary>
        /// 称点名称
        /// </summary>
        public string clientname
        {
            get { return _clientName; }
            set { _clientName = value; }
        }
        private string _carno;
        /// <summary>
        /// 车号
        /// </summary>
        public string carno
        {
            get { return _carno; }
            set { _carno = value; }
        }
        private string _printtype;
        /// <summary>
        /// 打印类型(补打/正常)
        /// </summary>
        public string printtype
        {
            get { return _printtype; }
            set { _printtype = value; }
        }
        private int ticketType;
        /// <summary>
        /// 票据类型(0:净重票据;1:毛重票据)
        /// </summary>
        public int TicketType
        {
            get { return ticketType; }
            set { ticketType = value; }
        }

    }
}
