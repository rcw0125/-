using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class submodule
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

        private string type;
        /// <summary>
        /// 类型
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("type")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string code;
        /// <summary>
        /// 编码
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("code")]
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

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

        private GridColumn gridColumn;
        /// <summary>
        /// 列字段对象集合
        /// </summary>
        [System.Xml.Serialization.XmlElement("GridColumn")]
        public GridColumn GridColumn
        {
            get { return gridColumn; }
            set { gridColumn = value; }
        }

        private GridRow gridRow;
        /// <summary>
        /// 行对象集合
        /// </summary>
        [System.Xml.Serialization.XmlElement("GridRow")]
        public GridRow GridRow
        {
            get { return gridRow; }
            set { gridRow = value; }
        }
    }
}
