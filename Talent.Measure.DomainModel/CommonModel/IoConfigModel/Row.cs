using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class Row
    {
        private List<Param> paramList;
        /// <summary>
        /// 字段对象集合
        /// </summary>
        [System.Xml.Serialization.XmlElement("param")]
        public List<Param> Params
        {
            get { return paramList; }
            set { paramList = value; }
        }
    }
}
