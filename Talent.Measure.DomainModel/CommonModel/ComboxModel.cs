using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class ComboxModel
    {
        private string id;
        /// <summary>
        /// id
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private string code;
        /// <summary>
        /// 编码
        /// </summary>
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string type;
        /// <summary>
        /// 类型
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
