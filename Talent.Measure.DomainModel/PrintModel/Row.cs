using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.PrintModel
{
    public class Row
    {
        private string _type;
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _length;
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("length")]
        public string Length
        {
            get { return _length; }
            set { _length = value; }
        }

        private string _align;
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("align")]
        public string Align
        {
            get { return _align; }
            set { _align = value; }
        }


        private string _width;
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("width")]
        public string Width
        {
            get { return _width; }
            set { this._width = value; }
        }

        private string _height;
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("height")]
        public string Height
        {
            get { return _height; }
            set { this._height = value; }
        }
        private string _line;
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("line")]
        public string Line
        {
            get { return _line; }
            set { this._line = value; }
        }

        private string value;
        /// <summary>
        /// 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("value")]
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        private List<Param> _paramList;
        [System.Xml.Serialization.XmlElement("param")]
        public List<Param> Params
        {
            get { return _paramList; }
            set { _paramList = value; }
        }
    }
}
