using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.PrintModel
{
    public class Bill
    {
        private string _code;
        [System.Xml.Serialization.XmlAttribute("code")]
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _script;
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("script")]
        public string Script
        {
            get { return _script; }
            set { _script = value; }
        }

        private List<Row> _rowList;
        [System.Xml.Serialization.XmlElement("row")]
        public List<Row> RowList
        {
            get { return _rowList; }
            set { _rowList = value; }
        }
    }
}
