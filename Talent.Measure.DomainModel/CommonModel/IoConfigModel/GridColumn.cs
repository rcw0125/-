using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class GridColumn
    {
        private List<Column> columns;
        /// <summary>
        /// 列字段对象集合
        /// </summary>
        [System.Xml.Serialization.XmlElement("column")]
        public List<Column> Columns
        {
            get { return columns; }
            set { columns = value; }
        }
    }
}
