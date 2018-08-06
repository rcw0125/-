using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 任务模型
    /// added by wangc on 20151022
    /// </summary>
    public class TaskModel : INotifyPropertyChanged 
    {
        private string clientid;
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId
        {
            get { return clientid; }
            set
            {
                clientid = value;
                this.SendPropertyChanged("ClientId");
            }
        }

        private string icId;
        /// <summary>
        /// Ic卡号
        /// </summary>
        public string IcId
        {
            get { return icId; }
            set
            {
                icId = value;
                this.SendPropertyChanged("IcId");
            }
        }
        

        private string clientCode;
        /// <summary>
        /// 客户端编号
        /// </summary>
        public string ClientCode
        {
            get { return clientCode; }
            set
            {
                clientCode = value;
                this.SendPropertyChanged("ClientCode");
            }
        }


        private string clientName;
        /// <summary>
        /// 客户端名称
        /// </summary>
        public string ClientName
        {
            get { return clientName; }
            set { clientName = value; this.SendPropertyChanged("ClientName"); }
        }
        private DateTime createTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return createTime; }
            set
            {
                createTime = value;
                this.SendPropertyChanged("CreateTime");
            }
        }
        private string carNumber;
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNumber
        {
            get { return carNumber; }
            set
            {
                carNumber = value;
                this.SendPropertyChanged("CarNumber");
            }
        }
        private string measureType;
        /// <summary>
        /// 计量方式(现场自助、远程计量)
        /// </summary>
        public string MeasureType
        {
            get { return measureType; }
            set
            {
                measureType = value;
                this.SendPropertyChanged("MeasureType");
            }
        }
        private string errorMsg;
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMsg
        {
            get { return errorMsg; }
            set
            {
                errorMsg = value;
                this.SendPropertyChanged("ErrorMsg");
            }
        }

        private string bussinessTypeId;
        /// <summary>
        /// 业务ID
        /// </summary>
        public string BussinessTypeId
        {
            get { return bussinessTypeId; }
            set
            {
                bussinessTypeId = value;
                this.SendPropertyChanged("BussinessTypeId");
            }
        }

        private string bussinessTypeName;
        /// <summary>
        /// 业务名称
        /// </summary>
        public string BussinessTypeName
        {
            get { return bussinessTypeName; }
            set
            {
                bussinessTypeName = value;
                this.SendPropertyChanged("BussinessTypeName");
            }
        }
        private bool isHelpCmd;
        /// <summary>
        /// 是否为求助CMD
        /// </summary>
        public bool IsHelpCmd
        {
            get { return isHelpCmd; }
            set
            {
                isHelpCmd = value;
                this.SendPropertyChanged("IsHelpCmd");
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
        private int  _msgid;
        /// <summary>
        /// 重量(随机数,用)
        /// </summary>
        public int msgid
        {
            get { return _msgid; }
            set
            {
                _msgid = value;
                this.SendPropertyChanged("msgid");
            }
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
        private BullInfo bullInfo;
        /// <summary>
        /// 业务信息(ServiceResult中的业务信息转换后的业务对象)
        /// </summary>
        public BullInfo BullInfo
        {
            get { return bullInfo; }
            set
            {
                bullInfo = value;
                this.SendPropertyChanged("BullInfo");
            }
        }

        //private string serviceResult;
        ///// <summary>
        ///// 服务结果(业务信息服务反馈的json字符串)
        ///// </summary>
        //public string ServiceResult
        //{
        //    get { return serviceResult; }
        //    set
        //    {
        //        serviceResult = value;
        //        this.SendPropertyChanged("ServiceResult");
        //    }
        //}

        private MeasureServiceModel serviceModel;
        /// <summary>
        /// 服务下发的称量信息对应的模型
        /// </summary>
        public MeasureServiceModel ServiceModel
        {
            get { return serviceModel; }
            set
            {
                serviceModel = value;
                this.SendPropertyChanged("ServiceModel");
            }
        }


        private List<BullInfo> bullInfos;
        /// <summary>
        /// 业务信息集合
        /// </summary>
        public List<BullInfo> BullInfos
        {
            set { bullInfos = value; this.SendPropertyChanged("BullInfos");}
            get { return bullInfos; }
        }

        private bool isBusinessInfoQuery;
        /// <summary>
        /// 是否查询业务信息
        /// </summary>
        public bool IsBusinessInfoQuery
        {
            get { return isBusinessInfoQuery; }
            set
            {
                isBusinessInfoQuery = value;
                this.SendPropertyChanged("IsBusinessInfoQuery");
            }
        }
      

        #region 作废属性
        //private string matchId;
        ///// <summary>
        ///// 物流业务号(由物流系统提供)
        ///// </summary>
        //public string MatchId
        //{
        //    get { return matchId; }
        //    set
        //    {
        //        matchId = value;
        //        this.SendPropertyChanged("MatchId");
        //    }
        //}
        //private string icCardNo;
        ///// <summary>
        ///// Ic卡号
        ///// </summary>
        //public string IcCardNo
        //{
        //    get { return icCardNo; }
        //    set { icCardNo = value; this.SendPropertyChanged("IcCardNo"); }
        //}

        //private string rfidCardNo;
        ///// <summary>
        ///// rfid卡号
        ///// </summary>
        //public string RfidCardNo
        //{
        //    get { return rfidCardNo; }
        //    set { rfidCardNo = value; this.SendPropertyChanged("RfidCardNo"); }
        //}

        //private string taskName;
        ///// <summary>
        ///// 任务名称
        ///// </summary>
        //public string TaskName
        //{
        //    get { return taskName; }
        //    set
        //    {
        //        taskName = value;
        //        this.SendPropertyChanged("TaskName");
        //    }
        //}

        //private string description;
        ///// <summary>
        ///// 描述
        ///// </summary>
        //public string Description
        //{
        //    get { return description; }
        //    set
        //    {
        //        description = value;
        //        this.SendPropertyChanged("Description");
        //    }
        //}

        //private int taskType;
        ///// <summary>
        ///// 任务类型(0计毛1计皮)
        ///// </summary>
        //public int TaskType
        //{
        //    get { return taskType; }
        //    set
        //    {
        //        taskType = value;
        //        this.SendPropertyChanged("TaskType");
        //    }
        //}

        //private string functionNode;
        ///// <summary>
        ///// 功能节点(计毛、计皮...)
        ///// </summary>
        //public string FunctionNode
        //{
        //    get { return functionNode; }
        //    set
        //    {
        //        functionNode = value;
        //        this.SendPropertyChanged("FunctionNode");
        //    }
        //}

        //private string planId;
        ///// <summary>
        ///// 计划ID
        ///// </summary>
        //public string PlanId
        //{
        //    get { return planId; }
        //    set
        //    {
        //        planId = value;
        //        this.SendPropertyChanged("PlanId");
        //    }
        //}

        //private string planName;
        ///// <summary>
        ///// 计划名称
        ///// </summary>
        //public string PlanName
        //{
        //    get { return planName; }
        //    set
        //    {
        //        planName = value;
        //        this.SendPropertyChanged("PlanName");
        //    }
        //}

        //private string goodsId;
        ///// <summary>
        ///// 货品ID
        ///// </summary>
        //public string GoodsId
        //{
        //    get { return goodsId; }
        //    set
        //    {
        //        goodsId = value;
        //        this.SendPropertyChanged("GoodsId");
        //    }
        //}

        //private string goodsName;
        ///// <summary>
        ///// 货品名称
        ///// </summary>
        //public string GoodsName
        //{
        //    get { return goodsName; }
        //    set
        //    {
        //        goodsName = value;
        //        this.SendPropertyChanged("GoodsName");
        //    }
        //}

        //private string goodsSpecId;
        ///// <summary>
        ///// 商品规格Id
        ///// </summary>
        //public string GoodsSpecId
        //{
        //    get { return goodsSpecId; }
        //    set
        //    {
        //        goodsSpecId = value;
        //        this.SendPropertyChanged("GoodsSpecId");
        //    }
        //}

        //private string goodsSpecName;
        ///// <summary>
        ///// 商品规格名称
        ///// </summary>
        //public string GoodsSpecName
        //{
        //    get { return goodsSpecName; }
        //    set
        //    {
        //        goodsSpecName = value;
        //        this.SendPropertyChanged("GoodsSpecName");
        //    }
        //}

        //private string supplyAddressId;
        ///// <summary>
        ///// 供货地ID
        ///// </summary>
        //public string SupplyAddressId
        //{
        //    get { return supplyAddressId; }
        //    set
        //    {
        //        supplyAddressId = value;
        //        this.SendPropertyChanged("SupplyAddressId");
        //    }
        //}

        //private string supplyAddressName;
        ///// <summary>
        ///// 供货地名称
        ///// </summary>
        //public string SupplyAddressName
        //{
        //    get { return supplyAddressName; }
        //    set
        //    {
        //        supplyAddressName = value;
        //        this.SendPropertyChanged("SupplyAddressName");
        //    }
        //}

        //private string collectAddressId;
        ///// <summary>
        ///// 收货地ID
        ///// </summary>
        //public string CollectAddressId
        //{
        //    get { return collectAddressId; }
        //    set
        //    {
        //        collectAddressId = value;
        //        this.SendPropertyChanged("CollectAddressId");
        //    }
        //}

        //private string collectAddressName;
        ///// <summary>
        ///// 收货地名称
        ///// </summary>
        //public string CollectAddressName
        //{
        //    get { return collectAddressName; }
        //    set
        //    {
        //        collectAddressName = value;
        //        this.SendPropertyChanged("CollectAddressName");
        //    }
        //}

        //private int? packageQty;
        ///// <summary>
        ///// 件数
        ///// </summary>
        //public int? PackageQty
        //{
        //    get { return packageQty; }
        //    set
        //    {
        //        packageQty = value;
        //        this.SendPropertyChanged("PackageQty");
        //    }
        //}

        //private decimal? deliveryGrossWeight;
        ///// <summary>
        ///// 发货单中的毛重
        ///// </summary>
        //public decimal? DeliveryGrossWeight
        //{
        //    get { return deliveryGrossWeight; }
        //    set { deliveryGrossWeight = value; this.SendPropertyChanged("DeliveryGrossWeight"); }
        //}

        //private decimal? grossWeight;
        ///// <summary>
        ///// 毛重
        ///// </summary>
        //public decimal? GrossWeight
        //{
        //    get { return grossWeight; }
        //    set
        //    {
        //        grossWeight = value;
        //        this.SendPropertyChanged("GrossWeight");
        //        Suttle = value - TareWeight - DeductWeight;
        //    }
        //}

        //private decimal? deliveryTareWeight;
        ///// <summary>
        ///// 发货单中的皮重
        ///// </summary>
        //public decimal? DeliveryTareWeight
        //{
        //    get { return deliveryTareWeight; }
        //    set { deliveryTareWeight = value; this.SendPropertyChanged("DeliveryTareWeight"); }
        //}

        //private decimal? tareWeight;
        ///// <summary>
        ///// 皮重
        ///// </summary>
        //public decimal? TareWeight
        //{
        //    get { return tareWeight; }
        //    set
        //    {
        //        tareWeight = value;
        //        this.SendPropertyChanged("TareWeight");
        //        Suttle = GrossWeight - value - DeductWeight;
        //    }
        //}

        //private decimal? deductWeight;
        ///// <summary>
        ///// 扣重
        ///// </summary>
        //public decimal? DeductWeight
        //{
        //    get { return deductWeight; }
        //    set
        //    {
        //        deductWeight = value;
        //        this.SendPropertyChanged("DeductWeight");
        //        Suttle = GrossWeight - TareWeight - value;
        //    }
        //}

        //private decimal? deliverySuttle;
        ///// <summary>
        ///// 发货单中的净重
        ///// </summary>
        //public decimal? DeliverySuttle
        //{
        //    get { return deliverySuttle; }
        //    set { deliverySuttle = value; this.SendPropertyChanged("DeliverySuttle"); }
        //}

        //private decimal? suttle;
        ///// <summary>
        ///// 净重
        ///// </summary>
        //public decimal? Suttle
        //{
        //    get { return suttle; }
        //    set
        //    {
        //        suttle = value;
        //        this.SendPropertyChanged("Suttle");
        //    }
        //}

        //private int deliveryQuantity;
        ///// <summary>
        ///// 发货数量
        ///// </summary>
        //public int DeliveryQuantity
        //{
        //    get { return deliveryQuantity; }
        //    set { deliveryQuantity = value; this.SendPropertyChanged("DeliveryQuantity");}
        //}

        //private decimal deliveryQty;
        ///// <summary>
        ///// 发货量
        ///// </summary>
        //public decimal DeliveryQty
        //{
        //    get { return deliveryQty; }
        //    set { deliveryQty = value; this.SendPropertyChanged("DeliveryQuantity");}
        //}

        //private string _taskcode;
        ///// <summary>
        ///// 业务号
        ///// </summary>
        //public string TaskCode
        //{
        //    get { return _taskcode; }
        //    set { _taskcode = value;
        //    this.SendPropertyChanged("TaskCode");
        //    }
        //}
        #endregion

        public virtual event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(System.String propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
