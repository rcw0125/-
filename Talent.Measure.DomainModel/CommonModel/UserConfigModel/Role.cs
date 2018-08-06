using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class Role
    {
        private string name;
        /// <summary>
        /// 名称(设备:称点、坐席...)
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string code;
        /// <summary>
        /// 编码(设备:称点、坐席...)
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("code")]
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        private string formUrl;
        /// <summary>
        /// 值(窗体URL,用于根据不同的角色跳转至不同的窗体)
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("formUrl")]
        public string FormUrl
        {
            get { return formUrl; }
            set { this.formUrl = value; }
        }
    }
}
