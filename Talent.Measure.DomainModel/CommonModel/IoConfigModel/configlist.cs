using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class configlist
    {
        private string equCode;
        /// <summary>
        /// 设备编码
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("EQUCODE")]
        public string EquCode
        {
            get { return equCode; }
            set { equCode = value; }
        }

        private string equName;
        /// <summary>
        /// 设备名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("equname")]
        public string EquName
        {
            get { return equName; }
            set { equName = value; }
        }

        private List<module> modules;
        /// <summary>
        /// 字段对象集合
        /// </summary>
        [System.Xml.Serialization.XmlElement("module")]
        public List<module> Modules
        {
            get { return modules; }
            set { modules = value; }
        }
    }
}
