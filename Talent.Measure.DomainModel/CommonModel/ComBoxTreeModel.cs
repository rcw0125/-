using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 下拉树模型
    /// </summary>
    public class ComBoxTreeModel : ComboxModel
    {
        public ComBoxTreeModel()
        {
            Child = new List<ComBoxTreeModel>();
        }

        private ComBoxTreeModel parent;
        /// <summary>
        /// 父级节点
        /// </summary>
        public ComBoxTreeModel Parent
        {
            get { return parent; }
            set { parent = value; }
        }


        private List<ComBoxTreeModel> child;
        /// <summary>
        /// 子节点
        /// </summary>
        public List<ComBoxTreeModel> Child
        {
            get { return child; }
            set { child = value; }
        }
    }
}
