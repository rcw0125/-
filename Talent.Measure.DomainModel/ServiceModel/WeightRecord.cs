using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.ServiceModel
{
    /// <summary>
    /// 服务端返回的计量记录对象
    /// </summary>
    public class WeightRecord : INotifyPropertyChanged
    {
        private string _lastprintequname;
        /// <summary>
        /// 票据衡器
        /// </summary>
        public string lastprintequname
        {
            get { return _lastprintequname; }
            set
            {
                _lastprintequname = value;
                this.SendPropertyChanged("lastprintequname");
            }
        }
        private string _lastprintdate;
        /// <summary>
        /// 打印票据时间
        /// </summary>
        public string lastprintdate
        {
            get { return _lastprintdate; }
            set
            {
                _lastprintdate = value;
                this.SendPropertyChanged("lastprintdate");
            }
        }
        private string _matchid;
        /// <summary>
        /// 验配ID clientID+yymmdd+00000  该验配ID是第一个业务点生成
        /// </summary>
        public string matchid
        {
            get { return _matchid; }
            set
            {
                _matchid = value;
                this.SendPropertyChanged("matchid");
            }
        }
        private string _carno;
        /// <summary>
        /// 车号
        /// </summary>
        public string carno
        {
            get { return _carno; }
            set
            {
                _carno = value;
                this.SendPropertyChanged("carno");
            }
        }
        private string _cartype;
        /// <summary>
        /// 车类别(C汽车,T火车)
        /// </summary>
        public string cartype
        {
            get { return _cartype; }
            set
            {
                _cartype = value;
                this.SendPropertyChanged("cartype");
            }
        }
        private string _rfidid;
        /// <summary>
        /// RFID卡号 ,卡的唯一标示号
        /// </summary>
        public string rfidid
        {
            get { return _rfidid; }
            set
            {
                _rfidid = value;
                this.SendPropertyChanged("rfidid");
            }
        }
        private string _icid;
        /// <summary>
        /// IC卡ID号 ,卡的唯一标示号
        /// </summary>
        public string icid
        {
            get { return _icid; }
            set
            {
                _icid = value;
                this.SendPropertyChanged("icid");
            }
        }
        private string _operatype;
        /// <summary>
        /// 业务类型
        /// </summary>
        public string operatype
        {
            get { return _operatype; }
            set
            {
                _operatype = value;
                this.SendPropertyChanged("operatype");
            }
        }
        private string _operaname;
        /// <summary>
        /// 业务名称
        /// </summary>
        public string operaname
        {
            get { return _operaname; }
            set
            {
                _operaname = value;
                this.SendPropertyChanged("operaname");
            }
        }

        private string _planid;
        /// <summary>
        /// 计划ID
        /// </summary>
        public string planid
        {
            get { return _planid; }
            set
            {
                _planid = value;
                this.SendPropertyChanged("planid");
            }
        }
        private string _taskcode;
        /// <summary>
        /// 调拨业务号
        /// </summary>
        public string taskcode
        {
            get { return _taskcode; }
            set
            {
                _taskcode = value;
                this.SendPropertyChanged("taskcode");
            }
        }
        private string _materialcode;
        /// <summary>
        /// 物料编码
        /// </summary>
        public string materialcode
        {
            get { return _materialcode; }
            set
            {
                _materialcode = value;
                this.SendPropertyChanged("materialcode");
            }
        }
        private string _materialname;
        /// <summary>
        /// 物料名称
        /// </summary>
        public string materialname
        {
            get { return _materialname; }
            set
            {
                _materialname = value;
                this.SendPropertyChanged("materialname");
            }
        }
        private string _materialspeccode;
        /// <summary>
        /// 物料规格编码
        /// </summary>
        public string materialspeccode
        {
            get { return _materialspeccode; }
            set
            {
                _materialspeccode = value;
                this.SendPropertyChanged("materialspeccode");
            }
        }
        private string _materialspec;
        /// <summary>
        /// 物料规格
        /// </summary>
        public string materialspec
        {
            get { return _materialspec; }
            set
            {
                _materialspec = value;
                this.SendPropertyChanged("materialspec");
            }
        }
        private string _shipcode;
        /// <summary>
        /// 船名编码
        /// </summary>
        public string shipcode
        {
            get { return _shipcode; }
            set
            {
                _shipcode = value;
                this.SendPropertyChanged("shipcode");
            }
        }
        private string _ship;
        /// <summary>
        /// 船名
        /// </summary>
        public string ship
        {
            get { return _ship; }
            set
            {
                _ship = value;
                this.SendPropertyChanged("ship");
            }
        }
        private string _sourcecode;
        /// <summary>
        /// 发货单位编码
        /// </summary>
        public string sourcecode
        {
            get { return _sourcecode; }
            set
            {
                _sourcecode = value;
                this.SendPropertyChanged("sourcecode");
            }
        }
        private string _sourcename;
        /// <summary>
        /// 发货单位
        /// </summary>
        public string sourcename
        {
            get { return _sourcename; }
            set
            {
                _sourcename = value;
                this.SendPropertyChanged("sourcename");
            }
        }
        private string _sourcetime;
        /// <summary>
        /// 发货时间
        /// </summary>
        public string sourcetime
        {
            get { return _sourcetime; }
            set
            {
                _sourcetime = value;
                this.SendPropertyChanged("sourcetime");
            }
        }
        private string _sourceplace;
        /// <summary>
        /// 供货地点/发站
        /// </summary>
        public string sourceplace
        {
            get { return _sourceplace; }
            set
            {
                _sourceplace = value;
                this.SendPropertyChanged("sourceplace");
            }
        }
        private string _targetcode;
        /// <summary>
        /// 收货单位编码
        /// </summary>
        public string targetcode
        {
            get { return _targetcode; }
            set
            {
                _targetcode = value;
                this.SendPropertyChanged("targetcode");
            }
        }
        private string _targetname;
        /// <summary>
        /// 收货单位
        /// </summary>
        public string targetname
        {
            get { return _targetname; }
            set
            {
                _targetname = value;
                this.SendPropertyChanged("targetname");
            }
        }
        private string _targettime;
        /// <summary>
        /// 收货时间
        /// </summary>
        public string targettime
        {
            get { return _targettime; }
            set
            {
                _targettime = value;
                this.SendPropertyChanged("targettime");
            }
        }
        private string _targetplace;
        /// <summary>
        /// 收货地点名称（集港的是港口，火车是到站名）
        /// </summary>
        public string targetplace
        {
            get { return _targetplace; }
            set
            {
                _targetplace = value;
                this.SendPropertyChanged("targetplace");
            }
        }
        private string _basket;
        /// <summary>
        /// 料篮号、包号
        /// </summary>
        public string basket
        {
            get { return _basket; }
            set
            {
                _basket = value;
                this.SendPropertyChanged("basket");
            }
        }
        private string _planweight;
        /// <summary>
        /// 计划发货重量
        /// </summary>
        public string planweight
        {
            get { return _planweight; }
            set
            {
                _planweight = value;
                this.SendPropertyChanged("planweight");
            }
        }
        private string _planmaterialcount;
        /// <summary>
        /// 计划发货总支数
        /// </summary>
        public string planmaterialcount
        {
            get { return _planmaterialcount; }
            set
            {
                _planmaterialcount = value;
                this.SendPropertyChanged("planmaterialcount");
            }
        }
        private string _materialcount;
        /// <summary>
        /// 实际发货总支件数
        /// </summary>
        public string materialcount
        {
            get { return _materialcount; }
            set
            {
                _materialcount = value;
                this.SendPropertyChanged("materialcount");
            }
        }
        private string _plancarcount;
        /// <summary>
        /// 计划发货总车数
        /// </summary>
        public string plancarcount
        {
            get { return _plancarcount; }
            set
            {
                _plancarcount = value;
                this.SendPropertyChanged("plancarcount");
            }
        }
        private string _gross;
        /// <summary>
        /// 毛重
        /// </summary>
        public string gross
        {
            get { return _gross; }
            set
            {
                _gross = value;
                this.SendPropertyChanged("gross");
            }
        }
        private string _grosstime;
        /// <summary>
        /// 毛重时间
        /// </summary>
        public string grosstime
        {
            get { return _grosstime; }
            set
            {
                _grosstime = value;
                this.SendPropertyChanged("grosstime");
            }
        }
        private string _grossweighid;
        /// <summary>
        /// 计毛衡器code(服务起名"....id",其实，值是code)
        /// </summary>
        public string grossweighid
        {
            get { return _grossweighid; }
            set
            {
                _grossweighid = value;
                this.SendPropertyChanged("grossweighid");
            }
        }
        private string _grossweigh;
        /// <summary>
        /// 毛重衡器名称
        /// </summary>
        public string grossweigh
        {
            get { return _grossweigh; }
            set
            {
                _grossweigh = value;
                this.SendPropertyChanged("grossweigh");
            }
        }
        private string _grossoperacode;
        /// <summary>
        /// 计毛计量员编码
        /// </summary>
        public string grossoperacode
        {
            get { return _grossoperacode; }
            set
            {
                _grossoperacode = value;
                this.SendPropertyChanged("grossoperacode");
            }
        }
        private string _grossoperaname;
        /// <summary>
        /// 计毛计量员
        /// </summary>
        public string grossoperaname
        {
            get { return _grossoperaname; }
            set
            {
                _grossoperaname = value;
                this.SendPropertyChanged("grossoperaname");
            }
        }
        private string _grossgroupno;
        /// <summary>
        /// 毛重组号
        /// </summary>
        public string grossgroupno
        {
            get { return _grossgroupno; }
            set
            {
                _grossgroupno = value;
                this.SendPropertyChanged("grossgroupno");
            }
        }
        private string _grossserial;
        /// <summary>
        /// 毛重组内序号
        /// </summary>
        public string grossserial
        {
            get { return _grossserial; }
            set
            {
                _grossserial = value;
                this.SendPropertyChanged("grossserial");
            }
        }
        private string _grossspeed;
        /// <summary>
        /// 毛重速度
        /// </summary>
        public string grossspeed
        {
            get { return _grossspeed; }
            set
            {
                _grossspeed = value;
                this.SendPropertyChanged("grossspeed");
            }
        }
        private string _grosslogid;
        /// <summary>
        /// 毛重LOGID,通过毛重logid,可以从照片表中读取照片
        /// </summary>
        public string grosslogid
        {
            get { return _grosslogid; }
            set
            {
                _grosslogid = value;
                this.SendPropertyChanged("grosslogid");
            }
        }
        private string _tare;
        /// <summary>
        /// 皮重
        /// </summary>
        public string tare
        {
            get { return _tare; }
            set
            {
                _tare = value;
                this.SendPropertyChanged("tare");
            }
        }
        private string _taretime;
        /// <summary>
        /// 皮重时间
        /// </summary>
        public string taretime
        {
            get { return _taretime; }
            set
            {
                _taretime = value;
                this.SendPropertyChanged("taretime");
            }
        }
        private string _tareweighid;
        /// <summary>
        /// 皮重衡器code(服务起名"....id",其实，值是code)
        /// </summary>
        public string tareweighid
        {
            get { return _tareweighid; }
            set
            {
                _tareweighid = value;
                this.SendPropertyChanged("tareweighid");
            }
        }
        private string _tareweigh;
        /// <summary>
        /// 皮重衡器名称
        /// </summary>
        public string tareweigh
        {
            get { return _tareweigh; }
            set
            {
                _tareweigh = value;
                this.SendPropertyChanged("tareweigh");
            }
        }
        private string _tareoperacode;
        /// <summary>
        /// 皮重计量员编码
        /// </summary>
        public string tareoperacode
        {
            get { return _tareoperacode; }
            set
            {
                _tareoperacode = value;
                this.SendPropertyChanged("tareoperacode");
            }
        }
        private string _tareoperaname;
        /// <summary>
        /// 皮重计量员
        /// </summary>
        public string tareoperaname
        {
            get { return _tareoperaname; }
            set
            {
                _tareoperaname = value;
                this.SendPropertyChanged("tareoperaname");
            }
        }
        private string _taregroupno;
        /// <summary>
        /// 皮重组号
        /// </summary>
        public string taregroupno
        {
            get { return _taregroupno; }
            set
            {
                _taregroupno = value;
                this.SendPropertyChanged("taregroupno");
            }
        }
        private string _tareserial;
        /// <summary>
        /// 皮重组内序号
        /// </summary>
        public string tareserial
        {
            get { return _tareserial; }
            set
            {
                _tareserial = value;
                this.SendPropertyChanged("tareserial");
            }
        }
        private string _tarelogid;
        /// <summary>
        /// 皮重LOGID,通过皮重logid,可以从照片表中读取照片
        /// </summary>
        public string tarelogid
        {
            get { return _tarelogid; }
            set
            {
                _tarelogid = value;
                this.SendPropertyChanged("tarelogid");
            }
        }
        private string _tarespeed;
        /// <summary>
        /// 计毛速度
        /// </summary>
        public string tarespeed
        {
            get { return _tarespeed; }
            set
            {
                _tarespeed = value;
                this.SendPropertyChanged("tarespeed");
            }
        }
        private string _deduction;
        /// <summary>
        /// 扣重
        /// </summary>
        public string deduction
        {
            get { return _deduction; }
            set
            {
                _deduction = value;
                this.SendPropertyChanged("deduction");
            }
        }
        private string _deductiontime;
        /// <summary>
        /// 扣重时间
        /// </summary>
        public string deductiontime
        {
            get { return _deductiontime; }
            set
            {
                _deductiontime = value;
                this.SendPropertyChanged("deductiontime");
            }
        }
        private string _deductioncode;
        /// <summary>
        /// 扣重单位编码
        /// </summary>
        public string deductioncode
        {
            get { return _deductioncode; }
            set
            {
                _deductioncode = value;
                this.SendPropertyChanged("deductioncode");
            }
        }
        private string _deductionname;
        /// <summary>
        /// 扣重单位
        /// </summary>
        public string deductionname
        {
            get { return _deductionname; }
            set
            {
                _deductionname = value;
                this.SendPropertyChanged("deductionname");
            }
        }
        private string _deductionoperacode;
        /// <summary>
        /// 扣重人编码
        /// </summary>
        public string deductionoperacode
        {
            get { return _deductionoperacode; }
            set
            {
                _deductionoperacode = value;
                this.SendPropertyChanged("deductionoperacode");
            }
        }
        private string _deductionoperaname;
        /// <summary>
        /// 扣重人
        /// </summary>
        public string deductionoperaname
        {
            get { return _deductionoperaname; }
            set
            {
                _deductionoperaname = value;
                this.SendPropertyChanged("deductionoperaname");
            }
        }
        private string _suttle;
        /// <summary>
        /// 净重
        /// </summary>
        public string suttle
        {
            get { return _suttle; }
            set
            {
                _suttle = value;
                this.SendPropertyChanged("suttle");
            }
        }
        private string _suttletime;
        /// <summary>
        /// 净重时间
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
        private string _suttleweighid;
        /// <summary>
        /// 净重衡器Code(服务起名"....id",其实，值是code)
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
        /// 净重衡器名称
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
        private string _suttleoperacode;
        /// <summary>
        /// 净重计量员编码
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
        /// 净重计量员
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
        private string _matchidb;
        /// <summary>
        /// 发货ID
        /// </summary>
        public string matchidb
        {
            get { return _matchidb; }
            set
            {
                _matchidb = value;
                this.SendPropertyChanged("matchidb");
            }
        }
        private string _suttleb;
        /// <summary>
        /// 发货前净重
        /// </summary>
        public string suttleb
        {
            get { return _suttleb; }
            set
            {
                _suttleb = value;
                this.SendPropertyChanged("suttleb");
            }
        }
        private string _grossb;
        /// <summary>
        /// 发货毛重
        /// </summary>
        public string grossb
        {
            get { return _grossb; }
            set
            {
                _grossb = value;
                this.SendPropertyChanged("grossb");
            }
        }
        private string _grosstimeb;
        /// <summary>
        /// 发货毛重时间
        /// </summary>
        public string grosstimeb
        {
            get { return _grosstimeb; }
            set
            {
                _grosstimeb = value;
                this.SendPropertyChanged("grosstimeb");
            }
        }
        private string _grossweighidb;
        /// <summary>
        /// 发货计毛衡器ID
        /// </summary>
        public string grossweighidb
        {
            get { return _grossweighidb; }
            set
            {
                _grossweighidb = value;
                this.SendPropertyChanged("grossweighidb");
            }
        }
        private string _grossweighb;
        /// <summary>
        /// 发货毛重衡器名称
        /// </summary>
        public string grossweighb
        {
            get { return _grossweighb; }
            set
            {
                _grossweighb = value;
                this.SendPropertyChanged("grossweighb");
            }
        }
        private string _grossoperacodeb;
        /// <summary>
        /// 发货计毛计量员编码
        /// </summary>
        public string grossoperacodeb
        {
            get { return _grossoperacodeb; }
            set
            {
                _grossoperacodeb = value;
                this.SendPropertyChanged("grossoperacodeb");
            }
        }
        private string _grossoperanameb;
        /// <summary>
        /// 发货计毛计量员
        /// </summary>
        public string grossoperanameb
        {
            get { return _grossoperanameb; }
            set
            {
                _grossoperanameb = value;
                this.SendPropertyChanged("grossoperanameb");
            }
        }
        private string _grossgroupnob;
        /// <summary>
        /// 发货毛重组号
        /// </summary>
        public string grossgroupnob
        {
            get { return _grossgroupnob; }
            set
            {
                _grossgroupnob = value;
                this.SendPropertyChanged("grossgroupnob");
            }
        }
        private string _grossserialb;
        /// <summary>
        /// 发货毛重组内序号
        /// </summary>
        public string grossserialb
        {
            get { return _grossserialb; }
            set
            {
                _grossserialb = value;
                this.SendPropertyChanged("grossserialb");
            }
        }
        private string _grossspeedb;
        /// <summary>
        /// 发货毛重速度
        /// </summary>
        public string grossspeedb
        {
            get { return _grossspeedb; }
            set
            {
                _grossspeedb = value;
                this.SendPropertyChanged("grossspeedb");
            }
        }
        private string _grosslogidb;
        /// <summary>
        /// 发货毛重LOGID,通过毛重logid,可以从照片表中读取照片
        /// </summary>
        public string grosslogidb
        {
            get { return _grosslogidb; }
            set
            {
                _grosslogidb = value;
                this.SendPropertyChanged("grosslogidb");
            }
        }
        private string _tareb;
        /// <summary>
        /// 发货皮重
        /// </summary>
        public string tareb
        {
            get { return _tareb; }
            set
            {
                _tareb = value;
                this.SendPropertyChanged("tareb");
            }
        }
        private string _taretimeb;
        /// <summary>
        /// 发货皮重时间
        /// </summary>
        public string taretimeb
        {
            get { return _taretimeb; }
            set
            {
                _taretimeb = value;
                this.SendPropertyChanged("taretimeb");
            }
        }
        private string _tareweighidb;
        /// <summary>
        /// 发货皮重衡器ID
        /// </summary>
        public string tareweighidb
        {
            get { return _tareweighidb; }
            set
            {
                _tareweighidb = value;
                this.SendPropertyChanged("tareweighidb");
            }
        }
        private string _tareweighb;
        /// <summary>
        /// 发货皮重衡器名称
        /// </summary>
        public string tareweighb
        {
            get { return _tareweighb; }
            set
            {
                _tareweighb = value;
                this.SendPropertyChanged("tareweighb");
            }
        }
        private string _tareoperacodeb;
        /// <summary>
        /// 发货皮重计量员编码
        /// </summary>
        public string tareoperacodeb
        {
            get { return _tareoperacodeb; }
            set
            {
                _tareoperacodeb = value;
                this.SendPropertyChanged("tareoperacodeb");
            }
        }
        private string _tareoperanameb;
        /// <summary>
        /// 发货皮重计量员
        /// </summary>
        public string tareoperanameb
        {
            get { return _tareoperanameb; }
            set
            {
                _tareoperanameb = value;
                this.SendPropertyChanged("tareoperanameb");
            }
        }
        private string _taregroupnob;
        /// <summary>
        /// 发货皮重组号
        /// </summary>
        public string taregroupnob
        {
            get { return _taregroupnob; }
            set
            {
                _taregroupnob = value;
                this.SendPropertyChanged("taregroupnob");
            }
        }
        private string _tareserialb;
        /// <summary>
        /// 发货皮重组内序号
        /// </summary>
        public string tareserialb
        {
            get { return _tareserialb; }
            set
            {
                _tareserialb = value;
                this.SendPropertyChanged("tareserialb");
            }
        }
        private string _tarelogidb;
        /// <summary>
        /// 皮重LOGID,通过皮重logid,可以从照片表中读取照片
        /// </summary>
        public string tarelogidb
        {
            get { return _tarelogidb; }
            set
            {
                _tarelogidb = value;
                this.SendPropertyChanged("tarelogidb");
            }
        }
        private string _tarespeedb;
        /// <summary>
        /// 计毛速度
        /// </summary>
        public string tarespeedb
        {
            get { return _tarespeedb; }
            set
            {
                _tarespeedb = value;
                this.SendPropertyChanged("tarespeedb");
            }
        }
        private string _batchcode;
        /// <summary>
        /// 批号
        /// </summary>
        public string batchcode
        {
            get { return _batchcode; }
            set
            {
                _batchcode = value;
                this.SendPropertyChanged("batchcode");
            }
        }
        private string _bflag;
        /// <summary>
        /// 退货标记 0 不退货，1部分退货，2全部退货
        /// </summary>
        public string bflag
        {
            get { return _bflag; }
            set
            {
                _bflag = value;
                this.SendPropertyChanged("bflag");
            }
        }
        private string _printgrossnum;
        /// <summary>
        /// 打印票据毛重次数
        /// </summary>
        public string printgrossnum
        {
            get { return _printgrossnum; }
            set
            {
                _printgrossnum = value;
                this.SendPropertyChanged("printgrossnum");
            }
        }
        private string _printsuttlenum;
        /// <summary>
        /// 打印票据净重次数
        /// </summary>
        public string printsuttlenum
        {
            get { return _printsuttlenum; }
            set
            {
                _printsuttlenum = value;
                this.SendPropertyChanged("printsuttlenum");
            }
        }
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
        private string _validflag;
        /// <summary>
        /// 是否作废 0 作废 1正常
        /// </summary>
        public string validflag
        {
            get { return _validflag; }
            set
            {
                _validflag = value;
                this.SendPropertyChanged("validflag");
            }
        }
        private string _validflagtime;
        /// <summary>
        /// 作废时间
        /// </summary>
        public string validflagtime
        {
            get { return _validflagtime; }
            set
            {
                _validflagtime = value;
                this.SendPropertyChanged("validflagtime");
            }
        }
        private string _validflagoperaname;
        /// <summary>
        /// 作废人
        /// </summary>
        public string validflagoperaname
        {
            get { return _validflagoperaname; }
            set
            {
                _validflagoperaname = value;
                this.SendPropertyChanged("validflagoperaname");
            }
        }
        private string _recordtype;
        /// <summary>
        /// 记录类型 0 远程手动  1远程自动  2 现场自助 3现场自动  4 异常维护
        /// </summary>
        public string recordtype
        {
            get { return _recordtype; }
            set
            {
                _recordtype = value;
                this.SendPropertyChanged("recordtype");
                do
                {
                    if (value == "0")
                    {
                        recordtypename = "远程手动";
                        break;
                    }
                    if (value == "1")
                    {
                        recordtypename = "远程自动";
                        break;
                    }
                    if (value == "2")
                    {
                        recordtypename = "现场自助";
                        break;
                    }
                    if (value == "3")
                    {
                        recordtypename = "现场自动";
                        break;
                    }
                    if (value == "4")
                    {
                        recordtypename = "异常维护";
                        break;
                    }
                } while (false);
            }
        }
        private string _recordtypename;
        /// <summary>
        /// 记录类型名称
        /// </summary>
        public string recordtypename
        {
            get { return _recordtypename; }
            set
            {
                _recordtypename = value;
                this.SendPropertyChanged("recordtypename");
            }
        }
        private string _createdate;
        /// <summary>
        /// 记录的添加时间
        /// </summary>
        public string createdate
        {
            get { return _createdate; }
            set
            {
                _createdate = value;
                this.SendPropertyChanged("createdate");
            }
        }
        private string _uptime;
        /// <summary>
        /// 
        /// </summary>
        public string uptime
        {
            get { return _uptime; }
            set
            {
                _uptime = value;
                this.SendPropertyChanged("uptime");
            }
        }
        private string _uptimestamp;
        /// <summary>
        /// 
        /// </summary>
        public string uptimestamp
        {
            get { return _uptimestamp; }
            set
            {
                _uptimestamp = value;
                this.SendPropertyChanged("uptimestamp");
            }
        }
        private string _decutiontypebak;
        /// <summary>
        /// 扣重单位：1吨 2 百分比 之前使用，先备份
        /// </summary>
        public string decutiontypebak
        {
            get { return _decutiontypebak; }
            set
            {
                _decutiontypebak = value;
                this.SendPropertyChanged("decutiontypebak");
            }
        }
        private string _motorcadename;
        /// <summary>
        /// 物流公司
        /// </summary>
        public string motorcadename
        {
            get { return _motorcadename; }
            set
            {
                _motorcadename = value;
                this.SendPropertyChanged("motorcadename");
            }
        }
        private string _motorcadecode;
        /// <summary>
        /// 物流公司编码
        /// </summary>
        public string motorcadecode
        {
            get { return _motorcadecode; }
            set
            {
                _motorcadecode = value;
                this.SendPropertyChanged("motorcadecode");
            }
        }
        private string _deductiontype;
        /// <summary>
        /// 扣重类型 0不扣、1固定值和3录入值
        /// </summary>
        public string deductiontype
        {
            get { return _deductiontype; }
            set
            {
                _deductiontype = value;
                this.SendPropertyChanged("deductiontype");
            }
        }
        private string _deduction2;
        /// <summary>
        /// 扣重值 扣重值从配置参数或者业务点录入 0<value<1百分比控制，value>=1 按千克计算
        /// </summary>
        public string deduction2
        {
            get { return _deduction2; }
            set
            {
                _deduction2 = value;
                this.SendPropertyChanged("deduction2");
            }
        }
        private string _tname;
        /// <summary>
        /// 车队
        /// </summary>
        public string tname
        {
            get { return _tname; }
            set
            {
                _tname = value;
                this.SendPropertyChanged("tname");
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
