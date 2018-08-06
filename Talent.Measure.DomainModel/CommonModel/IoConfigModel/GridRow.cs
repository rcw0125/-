using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class GridRow
    {
        private List<Row> rowList;
        /// <summary>
        /// 行对象集合
        /// </summary>
        [System.Xml.Serialization.XmlElement("Row")]
        public List<Row> RowList
        {
            get { return rowList; }
            set { rowList = value; }
        }
    }
}
