using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel
{
    public class MeasureServiceModel
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
        /// 0自动计量，1手动，2远程 //修改  0代表自动计量  1 代表进行提示 2 代表进行选择 3 代表终止 lt 2016-2-2 14:26:14……
        /// </summary>
        private int _mfunc;
        public int mfunc
        {
            set { _mfunc = value; }
            get { return _mfunc; }
        }
        private BullData _data;
        public BullData data
        {
            set { _data = value; }
            get { return _data; }
        }
        private List<BullInfo> _rows;
        /// <summary>
        /// 业务信息集合
        /// </summary>
        public List<BullInfo> rows
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

        private List<RenderUI> _mores;
        /// <summary>
        /// 页面展示字段
        /// </summary>
        public List<RenderUI> mores
        {
            set { _mores = value; }
            get { return _mores; }
        }

        private object _more;
        /// <summary>
        /// 20171123增加，用于单独计皮等无业务信息情况下,汽车衡保存成功后，接收machid
        /// </summary>
        public object more
        {
            get { return _more; }
            set { _more = value; }
        }

    }

    /// <summary>
    /// 服务反馈的对象模型
    /// </summary>
    /// <typeparam name="T">对象</typeparam>
    public class JavaServiceModel<T>
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
        /// 0自动计量，1手动，2远程 //修改  0代表自动计量  1 代表进行提示 2 代表进行选择 3 代表终止 lt 2016-2-2 14:26:14……
        /// </summary>
        private int _mfunc;
        public int mfunc
        {
            set { _mfunc = value; }
            get { return _mfunc; }
        }
        private T _data;
        public T data
        {
            set { _data = value; }
            get { return _data; }
        }
        private List<BullInfo> _rows;
        /// <summary>
        /// 业务信息集合
        /// </summary>
        public List<BullInfo> rows
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

        private List<RenderUI> _mores;
        /// <summary>
        /// 页面展示字段
        /// </summary>
        public List<RenderUI> mores
        {
            set { _mores = value; }
            get { return _mores; }
        }
    }

    /// <summary>
    /// 卡有效性信息对象
    /// </summary>
    public class BullData
    {
        private string _icId;
        public string icId
        {
            set { _icId = value; }
            get { return _icId; }
        }
        private string _rfidId;
        public string rfidId
        {
            set { _rfidId = value; }
            get { return _rfidId; }
        }
        private string _carNo;
        public string carNo
        {
            set { _carNo = value; }
            get { return _carNo; }
        }
        private int _flag;
        public int flag
        {
            set { _flag = value; }
            get { return _flag; }
        }
        private string _msg;
        public string msg
        {
            set { _msg = value; }
            get { return _msg; }
        }
        private int _validflag;
        /// <summary>
        /// 是否有效
        /// </summary>
        public int validflag
        {
            set { _validflag = value; }
            get { return _validflag; }
        }
        private string _cardId;
        public string cardId
        {
            set { _cardId = value; }
            get { return _cardId; }
        }
        private string _recordType;
        public string recordType
        {
            set { _recordType = value; }
            get { return _recordType; }
        }

    }

    /// <summary>
    /// 业务信息
    /// </summary>
    public class BullInfo : INotifyPropertyChanged
    {
        public BullInfo()
        {
            tare = null;
            gross = null;

        }

        private string _rfid;
        /// <summary>
        /// rfid(硬件读取的值)
        /// </summary>
        public string rfid
        {
            get { return _rfid; }
            set
            {
                _rfid = value;
                this.SendPropertyChanged("rfid");
            }
        }


        private string _basket;
        /// <summary>
        /// 料篮号/罐号
        /// </summary>
        public string basket
        {
            set { _basket = value; this.SendPropertyChanged("basket"); }
            get { return _basket; }
        }
        private string _icid;
        /// <summary>
        /// IC卡
        /// </summary>
        public string icid
        {
            set { _icid = value; this.SendPropertyChanged("icid"); }
            get { return _icid; }
        }
        private decimal? _tareserial;
        /// <summary>
        /// 皮重组内序号
        /// </summary>
        public decimal? tareserial
        {
            set { _tareserial = value; this.SendPropertyChanged("tareserial"); }
            get { return _tareserial; }
        }
        private string _ship;
        /// <summary>
        /// 船名名称
        /// </summary>
        public string ship
        {
            set { _ship = value; this.SendPropertyChanged("ship"); }
            get { return _ship; }
        }
        private string _targetname;
        /// <summary>
        ///收货地名称
        /// </summary>
        public string targetname
        {
            set { _targetname = value; this.SendPropertyChanged("targetname"); }
            get { return _targetname; }
        }
        private decimal? _tareb;
        /// <summary>
        ///供方皮重
        /// </summary>
        public decimal? tareb
        {
            set { _tareb = value; this.SendPropertyChanged("tareb"); }
            get { return _tareb; }
        }
        private string _taretimeb;
        /// <summary>
        ///皮重时间
        /// </summary>
        public string taretimeb
        {
            set { _taretimeb = value; this.SendPropertyChanged("taretimeb"); }
            get { return _taretimeb; }
        }
        private string _tareweighidb;
        /// <summary>
        ///皮重衡器id
        /// </summary>
        public string tareweighidb
        {
            set { _tareweighidb = value; this.SendPropertyChanged("tareweighidb"); }
            get { return _tareweighidb; }
        }

        private string _tareweighb;
        /// <summary>
        ///皮重衡器
        /// </summary>
        public string tareweighb
        {
            set { _tareweighb = value; this.SendPropertyChanged("tareweighb"); }
            get { return _tareweighb; }
        }

        private string _tarelogidb;
        /// <summary>
        ///皮重日志id
        /// </summary>
        public string tarelogidb
        {
            set { _tarelogidb = value; this.SendPropertyChanged("tarelogidb"); }
            get { return _tarelogidb; }
        }
        private string _batchcode;
        /// <summary>
        ///批号
        /// </summary>
        public string batchcode
        {
            set { _batchcode = value; this.SendPropertyChanged("batchcode"); }
            get { return _batchcode; }
        }

        private string _tareoperanameb;
        /// <summary>
        ///皮重计量员
        /// </summary>
        public string tareoperanameb
        {
            set { _tareoperanameb = value; this.SendPropertyChanged("tareoperanameb"); }
            get { return _tareoperanameb; }
        }
        private string _tareoperacodeb;
        /// <summary>
        ///皮重计量员编码
        /// </summary>
        public string tareoperacodeb
        {
            set { _tareoperacodeb = value; this.SendPropertyChanged("tareoperacodeb"); }
            get { return _tareoperacodeb; }
        }
        private string _grossoperacode;
        /// <summary>
        ///毛重人编码
        /// </summary>
        public string grossoperacode
        {
            set { _grossoperacode = value; this.SendPropertyChanged("grossoperacode"); }
            get { return _grossoperacode; }
        }
        private string _materialspec;
        /// <summary>
        ///物料规格
        /// </summary>
        public string materialspec
        {
            set { _materialspec = value; this.SendPropertyChanged("materialspec"); }
            get { return _materialspec; }
        }
        private string _tareweighgroup;
        /// <summary>
        ///皮重衡器组
        /// </summary>
        public string tareweighgroup
        {
            set { _tareweighgroup = value; this.SendPropertyChanged("tareweighgroup"); }
            get { return _tareweighgroup; }
        }
        private string _grossweighgroup;
        /// <summary>
        ///发货毛重衡器组
        /// </summary>
        public string grossweighgroup
        {
            set { _grossweighgroup = value; this.SendPropertyChanged("grossweighgroup"); }
            get { return _grossweighgroup; }
        }
        private string _shipcode;
        /// <summary>
        ///船名编码
        /// </summary>
        public string shipcode
        {
            set { _shipcode = value; this.SendPropertyChanged("shipcode"); }
            get { return _shipcode; }
        }
        private decimal? _grossb;
        /// <summary>
        ///供方毛重/发货重量
        /// </summary>
        public decimal? grossb
        {
            set { _grossb = value; this.SendPropertyChanged("grossb"); }
            get { return _grossb; }
        }
        private string _grosstimeb;
        /// <summary>
        ///发货毛重时间
        /// </summary>
        public string grosstimeb
        {
            set { _grosstimeb = value; this.SendPropertyChanged("grosstimeb"); }
            get { return _grosstimeb; }
        }
        private string _grossoperacodeb;
        /// <summary>
        ///发货毛重计量员编码
        /// </summary>
        public string grossoperacodeb
        {
            set { _grossoperacodeb = value; this.SendPropertyChanged("grossoperacodeb"); }
            get { return _grossoperacodeb; }
        }
        private string _grossoperanameb;
        /// <summary>
        ///发货毛重计量员
        /// </summary>
        public string grossoperanameb
        {
            set { _grossoperanameb = value; this.SendPropertyChanged("grossoperanameb"); }
            get { return _grossoperanameb; }
        }
        private string _grossweighidb;
        /// <summary>
        ///发货毛重衡器id
        /// </summary>
        public string grossweighidb
        {
            set { _grossweighidb = value; this.SendPropertyChanged("grossweighidb"); }
            get { return _grossweighidb; }
        }
        private string _grossweighb;
        /// <summary>
        ///发货毛重衡器
        /// </summary>
        public string grossweighb
        {
            set { _grossweighb = value; this.SendPropertyChanged("grossweighb"); }
            get { return _grossweighb; }
        }
        private string _grosslogidb;
        /// <summary>
        ///发货毛重日志id
        /// </summary>
        public string grosslogidb
        {
            set { _grosslogidb = value; this.SendPropertyChanged("grosslogidb"); }
            get { return _grosslogidb; }
        }
        private decimal? _gross;
        /// <summary>
        ///毛重
        /// </summary>
        public decimal? gross
        {
            set
            {
                _gross = value; this.SendPropertyChanged("gross");
                var Tdeduction = deduction == null ? 0 : deduction;
                if (tare == null || tare == 0)
                {
                    suttle = null;
                }
                else
                {
                    //suttle = value - tare - Tdeduction;
                    suttle = value - tare;
                }

            }
            get { return _gross; }
        }
        private string _tarelogid;
        /// <summary>
        ///皮重LOGID
        /// </summary>
        public string tarelogid
        {
            set { _tarelogid = value; this.SendPropertyChanged("tarelogid"); }
            get { return _tarelogid; }
        }
        private string _materialspeccode;
        /// <summary>
        ///物料规格编码
        /// </summary>
        public string materialspeccode
        {
            set { _materialspeccode = value; this.SendPropertyChanged("materialspeccode"); }
            get { return _materialspeccode; }
        }

        private string _tareweighid;
        /// <summary>
        ///皮重衡器ID
        /// </summary>
        public string tareweighid
        {
            set { _tareweighid = value; this.SendPropertyChanged("tareweighid"); }
            get { return _tareweighid; }
        }
        private string _sourceplace;
        /// <summary>
        ///供货地点/发站
        /// </summary>
        public string sourceplace
        {
            set { _sourceplace = value; this.SendPropertyChanged("sourceplace"); }
            get { return _sourceplace; }
        }
        private decimal? _deduction2;
        /// <summary>
        ///扣重值2
        /// </summary>
        public decimal? deduction2
        {
            set { _deduction2 = value; this.SendPropertyChanged("deduction2"); }
            get { return _deduction2; }
        }
        private decimal? _deduction;
        /// <summary>
        ///扣重值2
        /// </summary>
        public decimal? deduction
        {
            set
            {
                _deduction = value; this.SendPropertyChanged("deduction");

                var Tdeduction = value == null ? 0 : value;
                if (gross == null || tare == null || gross <= 0 || tare <= 0)
                {
                    suttle = null;
                }
                else
                {
                    //suttle = gross - tare - Tdeduction;
                    //suttle = gross - tare;
                }

            }
            get { return _deduction; }
        }
        private string _carno;
        /// <summary>
        ///车号
        /// </summary>
        public string carno
        {
            set { _carno = value; this.SendPropertyChanged("carno"); }
            get { return _carno; }
        }
        private string _grossweighid;
        /// <summary>
        ///	计毛衡器ID
        /// </summary>
        public string grossweighid
        {
            set { _grossweighid = value; this.SendPropertyChanged("grossweighid"); }
            get { return _grossweighid; }
        }
        private string _grossgroupno;
        /// <summary>
        ///	毛重组号
        /// </summary>
        public string grossgroupno
        {
            set { _grossgroupno = value; this.SendPropertyChanged("grossgroupno"); }
            get { return _grossgroupno; }
        }
        private int? _snumber;
        /// <summary>
        ///	当前车计划件/支量
        /// </summary>
        public int? snumber
        {
            set { _snumber = value; this.SendPropertyChanged("snumber"); }
            get { return _snumber; }
        }
        private string _planid;
        /// <summary>
        ///	计划号
        /// </summary>
        public string planid
        {
            set { _planid = value; this.SendPropertyChanged("planid"); }
            get { return _planid; }
        }
        private string _targetplace;
        /// <summary>
        ///	收货地点（港口/站名）
        /// </summary>
        public string targetplace
        {
            set { _targetplace = value; this.SendPropertyChanged("targetplace"); }
            get { return _targetplace; }
        }
        private string _matchid;
        /// <summary>
        ///	过磅单号
        /// </summary>
        public string matchid
        {
            set { _matchid = value; this.SendPropertyChanged("matchid"); }
            get { return _matchid; }
        }
        private string _taretime;
        /// <summary>
        ///	皮重时间
        /// </summary>
        public string taretime
        {
            set { _taretime = value; this.SendPropertyChanged("taretime"); }
            get { return _taretime; }
        }
        private int? _grossserial;
        /// <summary>
        ///	毛重组内序号
        /// </summary>
        public int? grossserial
        {
            set { _grossserial = value; this.SendPropertyChanged("grossserial"); }
            get { return _grossserial; }
        }
        private string _materialname;
        /// <summary>
        ///	物料名称
        /// </summary>
        public string materialname
        {
            set { _materialname = value; this.SendPropertyChanged("materialname"); }
            get { return _materialname; }
        }

        private int? _flag;
        /// <summary>
        ///	标示
        /// </summary>
        public int? flag
        {
            set { _flag = value; this.SendPropertyChanged("flag"); }
            get { return _flag; }
        }
        private decimal? _suttleb;
        /// <summary>
        ///	供方净重
        /// </summary>
        public decimal? suttleb
        {
            set { _suttleb = value; this.SendPropertyChanged("suttleb"); }
            get { return _suttleb; }
        }
        private string _sourcecode;
        /// <summary>
        ///	来源编码
        /// </summary>
        public string sourcecode
        {
            set { _sourcecode = value; this.SendPropertyChanged("sourcecode"); }
            get { return _sourcecode; }
        }
        private decimal? _tare;
        /// <summary>
        ///	皮重
        /// </summary>
        public decimal? tare
        {
            set
            {
                _tare = value; this.SendPropertyChanged("tare");
                var Tdeduction = deduction == null ? 0 : deduction;
                if (gross == null || gross == 0)
                {
                    suttle = null;
                }
                else
                {
                    //suttle = gross - value - Tdeduction;
                    suttle = gross - value; //净重=毛-皮 fengyb(2017-03-23)
                }

            }
            get { return _tare; }
        }
        private decimal _tarespeed;
        /// <summary>
        ///	皮重速度
        /// </summary>
        public decimal tarespeed
        {
            set { _tarespeed = value; this.SendPropertyChanged("tarespeed"); }
            get { return _tarespeed; }
        }
        private decimal _taregroupno;
        /// <summary>
        ///	皮重组号
        /// </summary>
        public decimal taregroupno
        {
            set { _taregroupno = value; this.SendPropertyChanged("taregroupno"); }
            get { return _taregroupno; }
        }
        private string _grosslogid;
        /// <summary>
        ///	毛重LOGID,通过毛重logid,可以从照片表中读取照片
        /// </summary>
        public string grosslogid
        {
            set { _grosslogid = value; this.SendPropertyChanged("grosslogid"); }
            get { return _grosslogid; }
        }
        private string _taskcode;
        /// <summary>
        ///	业务号
        /// </summary>
        public string taskcode
        {
            set { _taskcode = value; this.SendPropertyChanged("taskcode"); }
            get { return _taskcode; }
        }
        private string _tareoperaname;
        /// <summary>
        ///	皮重人
        /// </summary>
        public string tareoperaname
        {
            set { _tareoperaname = value; this.SendPropertyChanged("tareoperaname"); }
            get { return _tareoperaname; }
        }
        private int? _mflag;
        /// <summary>
        ///	计量流程0不限制,1先毛后皮,2先皮后毛, 3皮毛毛皮, 4毛毛皮,5毛毛
        /// </summary>
        public int? mflag
        {
            set { _mflag = value; this.SendPropertyChanged("mflag"); }
            get { return _mflag; }
        }
        private string _sourcename;
        /// <summary>
        ///	来源名称
        /// </summary>
        public string sourcename
        {
            set { _sourcename = value; this.SendPropertyChanged("sourcename"); }
            get { return _sourcename; }
        }
        private decimal? _suttleapp;
        /// <summary>
        ///	计划重量
        /// </summary>
        public decimal? suttleapp
        {
            set { _suttleapp = value; this.SendPropertyChanged("suttleapp"); }
            get { return _suttleapp; }
        }

        private decimal? _planweight;
        /// <summary>
        ///	计划量 lt 2016-1-28 13:16:37 新增 
        /// </summary>
        public decimal? planweight
        {
            set { _planweight = value; this.SendPropertyChanged("suttleapp"); }
            get { return _planweight; }
        }
        private string _tareweigh;
        /// <summary>
        ///	皮重衡器名称
        /// </summary>
        public string tareweigh
        {
            set { _tareweigh = value; this.SendPropertyChanged("tareweigh"); }
            get { return _tareweigh; }
        }
        private string _grossweigh;
        /// <summary>
        ///	毛重衡器名称
        /// </summary>
        public string grossweigh
        {
            set { _grossweigh = value; this.SendPropertyChanged("grossweigh"); }
            get { return _grossweigh; }
        }

        private string _matchidb;
        /// <summary>
        ///	发货单号
        /// </summary>
        public string matchidb
        {
            set { _matchidb = value; this.SendPropertyChanged("matchidb"); }
            get { return _matchidb; }
        }
        private int? _materialcount;
        /// <summary>
        ///	出库件数
        /// </summary>
        public int? materialcount
        {
            set { _materialcount = value; this.SendPropertyChanged("materialcount"); }
            get { return _materialcount; }
        }
        private string _tareoperacode;
        /// <summary>
        ///	皮重人编码
        /// </summary>
        public string tareoperacode
        {
            set { _tareoperacode = value; this.SendPropertyChanged("tareoperacode"); }
            get { return _tareoperacode; }
        }
        private string _deductioncode;
        /// <summary>
        ///	扣重单位编码
        /// </summary>
        public string deductioncode
        {
            set { _deductioncode = value; this.SendPropertyChanged("deductioncode"); }
            get { return _deductioncode; }
        }
        private string _deductionname;
        /// <summary>
        ///	扣重单位
        /// </summary>
        public string deductionname
        {
            set { _deductionname = value; this.SendPropertyChanged("deductionname"); }
            get { return _deductionname; }
        }
        private string _deductionoperacode;
        /// <summary>
        ///	扣重人编码
        /// </summary>
        public string deductionoperacode
        {
            set { _deductionoperacode = value; this.SendPropertyChanged("deductionoperacode"); }
            get { return _deductionoperacode; }
        }
        private string _deductionoperaname;
        /// <summary>
        ///	扣重人
        /// </summary>
        public string deductionoperaname
        {
            set { _deductionoperaname = value; this.SendPropertyChanged("deductionoperaname"); }
            get { return _deductionoperaname; }
        }
        private string _deductiontime;
        /// <summary>
        ///	扣重时间
        /// </summary>
        public string deductiontime
        {
            set { _deductiontime = value; this.SendPropertyChanged("deductiontime"); }
            get { return _deductiontime; }
        }
        private string _grossoperaname;
        /// <summary>
        ///	计毛计量员
        /// </summary>
        public string grossoperaname
        {
            set { _grossoperaname = value; this.SendPropertyChanged("grossoperaname"); }
            get { return _grossoperaname; }
        }
        private string _materialcode;
        /// <summary>
        ///	物料编码
        /// </summary>
        public string materialcode
        {
            set { _materialcode = value; this.SendPropertyChanged("materialcode"); }
            get { return _materialcode; }
        }
        private string _targetcode;
        /// <summary>
        ///收货单位编码
        /// </summary>
        public string targetcode
        {
            set { _targetcode = value; this.SendPropertyChanged("targetcode"); }
            get { return _targetcode; }
        }
        private string _grosstime;
        /// <summary>
        ///毛重时间
        /// </summary>
        public string grosstime
        {
            set { _grosstime = value; this.SendPropertyChanged("grosstime"); }
            get { return _grosstime; }
        }
        private string _rfidid;
        /// <summary>
        ///RFID卡号(制卡的时候获取的)
        /// </summary>
        public string rfidid
        {
            set { _rfidid = value; this.SendPropertyChanged("rfidid"); }
            get { return _rfidid; }
        }
        private string _rfidtype;
        /// <summary>
        /// rfid卡的类型
        /// </summary>
        public string rfidtype
        {
            set { _rfidtype = value; this.SendPropertyChanged("rfidtype"); }
            get { return _rfidtype; }
        }
        private string _operatype;
        /// <summary>
        /// 业务类型编码
        /// </summary>
        public string operatype
        {
            set { _operatype = value; this.SendPropertyChanged("operatype"); }
            get { return _operatype; }
        }
        private string _measurestate;
        /// <summary>
        /// 计量类型(G计毛T计皮)
        /// </summary>
        public string measurestate
        {
            set { _measurestate = value; this.SendPropertyChanged("measurestate"); }
            get { return _measurestate; }
        }

        private decimal? _suttle;
        /// <summary>
        /// 净重
        /// </summary>
        public decimal? suttle
        {
            get
            {
                return _suttle;
            }
            set
            {
                _suttle = value;
                this.SendPropertyChanged("suttle");
            }
        }
        private string _suttletime;
        /// <summary>
        ///净重时间
        /// </summary>
        public string suttletime
        {
            get { return _suttletime; }
            set
            {
                _suttletime = value;
                this.SendPropertyChanged("suttletime");
            }
        }
        private string _suttleoperacode;
        /// <summary>
        ///净重计量员编码
        /// </summary>
        public string suttleoperacode
        {
            get { return _suttleoperacode; }
            set
            {
                _suttleoperacode = value;
                this.SendPropertyChanged("suttleoperacode");
            }
        }
        private string _suttleoperaname;
        /// <summary>
        ///净重计量员
        /// </summary>
        public string suttleoperaname
        {
            get { return _suttleoperaname; }
            set
            {
                _suttleoperaname = value;
                this.SendPropertyChanged("suttleoperaname");
            }
        }
        private string _suttleweighid;
        /// <summary>
        ///净重衡器编码
        /// </summary>
        public string suttleweighid
        {
            get { return _suttleweighid; }
            set
            {
                _suttleweighid = value;
                this.SendPropertyChanged("suttleweighid");
            }
        }
        private string _suttleweigh;
        /// <summary>
        ///净重衡器
        /// </summary>
        public string suttleweigh
        {
            get { return _suttleweigh; }
            set
            {
                _suttleweigh = value;
                this.SendPropertyChanged("suttleweigh");
            }
        }
        private int? _bflag;
        /// <summary>
        ///退货标记  0-不是退货 1-退货车辆
        /// </summary>
        public int? bflag
        {
            get { return _bflag; }
            set
            {
                _bflag = value;
                this.SendPropertyChanged("bflag");
            }
        }
        private int? _dflag;
        /// <summary>
        ///一车多货标记  0-一车一货 1-一车多货
        /// </summary>
        public int? dflag
        {
            get { return _dflag; }
            set
            {
                _dflag = value;
                this.SendPropertyChanged("dflag");
            }
        }

        //private string _weightno;
        ///// <summary>
        ///// 衡器编号
        ///// </summary>
        //public string weightno
        //{
        //    get { return _weightno; }
        //    set
        //    {
        //        _weightno = value;
        //        this.SendPropertyChanged("weightno");
        //    }
        //}
        private string _usermemo;
        /// <summary>
        /// 用户备注
        /// </summary>
        public string usermemo
        {
            get { return _usermemo; }
            set
            {
                _usermemo = value;
                this.SendPropertyChanged("usermemo");
            }
        }
        private string _sysmemo;
        /// <summary>
        /// 系统备注
        /// </summary>
        public string sysmemo
        {
            get { return _sysmemo; }
            set
            {
                _sysmemo = value;
                this.SendPropertyChanged("sysmemo");
            }
        }
        private string _clientid;
        /// <summary>
        /// 终端id
        /// </summary>
        public string clientid
        {
            get { return _clientid; }
            set
            {
                _clientid = value;
                this.SendPropertyChanged("clientid");
            }
        }
        private string _recordtype;
        /// <summary>
        /// 记录类型
        /// </summary>
        public string recordtype
        {
            get { return _recordtype; }
            set
            {
                _recordtype = value;
                this.SendPropertyChanged("recordtype");
            }
        }

        private int? _sflag;
        /// <summary>
        /// 是否出入库0不限制,1出库,2入库,3出入库  默认0
        /// </summary>
        public int? sflag
        {
            get { return _sflag; }
            set
            {
                _sflag = value;
                this.SendPropertyChanged("sflag");
            }
        }
        private int? _gflag;
        /// <summary>
        /// 是否进出门
        /// </summary>
        public int? gflag
        {
            get { return _gflag; }
            set
            {
                _gflag = value;
                this.SendPropertyChanged("gflag");
            }
        }

        private string _caller;
        /// <summary>
        /// 0终端1远程
        /// </summary>
        public string caller
        {
            get { return _caller; }
            set
            {
                _caller = value;
                this.SendPropertyChanged("caller");
            }
        }
        public virtual event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(System.String propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private int? _ruleflag;
        /// <summary>
        /// 验证标记 1表示验证获取时方法，2表示验证获取保存时方法 2016-2-25 11:22:37……
        /// </summary>
        public int? ruleflag
        {
            get { return _ruleflag; }
            set
            {
                _ruleflag = value;
            }
        }

        private double _dvalue;
        /// <summary>
        /// 皮重与历史皮重差值 2016-3-21 14:20:27……
        /// </summary>
        public double dvalue
        {
            get { return _dvalue; }
            set
            {
                _dvalue = value;
            }
        }

        private string _operaname;
        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string operaname
        {
            get { return _operaname; }
            set
            {
                _operaname = value;
            }
        }
        private string _mtypes;
        /// <summary>
        /// 服务需要
        /// </summary>
        public string mtypes
        {
            get { return _mtypes; }
            set
            {
                _mtypes = value;
            }
        }
    }
    public class flagMsg
    {
        private string _Msg;
        /// <summary>
        ///错误信息
        /// </summary>
        public string Msg
        {
            set { _Msg = value; }
            get { return _Msg; }
        }
        //新增 success  flag  list count  属性 2016-2-2 14:57:42…… lt
        private Boolean _success;
        /// <summary>
        ///接口调用成功true，如果失败返回false
        /// </summary>
        public Boolean success
        {
            set { _success = value; }
            get { return _success; }
        }
        private int _flag;
        /// <summary>
        ///  0代表允许计量  1 代表进行提示 2 代表进行选择 3 代表终止
        /// </summary>
        public int flag
        {
            set { _flag = value; }
            get { return _flag; }
        }

        private List<BullInfo> _list;
        /// <summary>
        ///返回list 
        /// </summary>
        public List<BullInfo> list
        {
            set { _list = value; }
            get { return _list; }
        }

        private int _count;
        /// <summary>
        ///返回list  条数
        /// </summary>
        public int count
        {
            set { _count = value; }
            get { return _count; }
        }
    }

    public class roleCls
    {
        private int _mfunc;
        /// <summary>
        /// 
        /// </summary>
        public int mfunc
        {
            set { _mfunc = value; }
            get { return _mfunc; }

        }
        private string _otherParams;
        public string otherParams
        {
            set { _otherParams = value; }
            get { return _otherParams; }
        }
    }
    public class hardwarectrlCls
    {
        private string _name;
        public string name
        {
            set { _name = value; }
            get { return _name; }
        }
        private string _check;
        public string check
        {
            set { _check = value; }
            get { return _check; }
        }
        private roleCls _roles;
        public roleCls roles
        {
            set { _roles = value; }
            get { return _roles; }
        }
    }

    public class RenderUI
    {
        private string _operatype;
        public string operatype
        {
            set { _operatype = value; }
            get { return _operatype; }
        }
        private int _orderno;
        public int orderno
        {
            set { _orderno = value; }
            get { return _orderno; }
        }
        private string _displayname;
        public string displayname
        {
            set { _displayname = value; }
            get { return _displayname; }
        }
        private string _fieldname;
        public string fieldname
        {
            set { _fieldname = value; }
            get { return _fieldname; }
        }
        private int _isdisplay;
        public int isdisplay
        {
            set { _isdisplay = value; }
            get { return _isdisplay; }
        }
        private int _labeltype;
        public int labeltype
        {
            set { _labeltype = value; }
            get { return _labeltype; }
        }
        private string _points;
        public string points
        {
            set { _points = value; }
            get { return _points; }
        }
        private int _aboutweight;
        public int aboutweight
        {
            set { _aboutweight = value; }
            get { return _aboutweight; }
        }
        private int _quicksuggest;
        public int quicksuggest
        {
            set { _quicksuggest = value; }
            get { return _quicksuggest; }
        }
        private int _writeable;
        /// <summary>
        /// 是否只读 0 只读 1可以编辑
        /// </summary>
        public int writeable
        {
            set { _writeable = value; }
            get { return _writeable; }
        }
    }


}
