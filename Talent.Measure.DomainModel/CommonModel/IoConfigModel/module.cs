using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class module
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

        private List<submodule> subModules;
        /// <summary>
        /// 子模块对象集合
        /// </summary>
        [System.Xml.Serialization.XmlElement("submodule")]
        public List<submodule> SubModules
        {
            get { return subModules; }
            set { subModules = value; }
        }
    }
}
