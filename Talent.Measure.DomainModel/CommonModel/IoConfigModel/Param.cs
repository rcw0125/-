using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class Param
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

        private string lab;
        /// <summary>
        /// 名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("lab")]
        public string Lab
        {
            get { return lab; }
            set { lab = value; }
        }

        private string type;
        /// <summary>
        /// 数据类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("type")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string size;
        /// <summary>
        /// 数据长度
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("size")]
        public string Size
        {
            get { return size; }
            set { size = value; }
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

        private List<string> list;
        /// <summary>
        /// 字段对象集合
        /// </summary>
        [System.Xml.Serialization.XmlElement("list")]
        public List<string> List
        {
            get { return list; }
            set { list = value; }
        }
    }
}
