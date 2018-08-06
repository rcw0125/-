using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 列属性
    /// </summary>
    public class Column
    {
        private string name;
        /// <summary>
        /// 标识
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string value;
        /// <summary>
        /// 值
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("value")]
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
