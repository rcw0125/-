using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public class EquType
    {
        public EquType()
        {
            Child = new List<EquType>();
        }
        private string id;
        /// <summary>
        /// id
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
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

        private string code;
        /// <summary>
        /// 编码
        /// </summary>
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private string parentId;
        /// <summary>
        /// 父设备类型ID
        /// </summary>
        public string ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }

        private List<EquType> child;
        /// <summary>
        /// 子节点
        /// </summary>
        public List<EquType> Child
        {
            get { return child; }
            set { child = value; }
        }
    }
}
