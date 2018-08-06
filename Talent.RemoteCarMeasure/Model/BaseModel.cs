using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Talent.ClientCommonLib;

namespace Talent.RemoteCarMeasure.Model
{
    /// <summary>
    /// 业务类型模型
    /// added by wangc on 20151020
    /// </summary>
    public class BaseModel : Only_ViewModelBase
    {
        private string id;
        /// <summary>
        /// Id
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                this.RaisePropertyChanged("Id");
            }
        }
        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.RaisePropertyChanged("Name");
            }
        }
        private string code;
        /// <summary>
        /// 编码
        /// </summary>
        public string Code
        {
            get { return code; }
            set
            {
                code = value;
                this.RaisePropertyChanged("Code");
            }
        }
    }

    /// <summary>
    /// 服务返回的基础信息
    /// </summary>
    public class ServiceBasicInfo
    {
        private string _item;

        public string item
        {
            get { return _item; }
            set { _item = value; }
        }
    }
}
