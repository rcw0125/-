using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Talent.ClientCommonLib
{
   public class TreeNode : BaseNotifyObject
    {  
       #region fields && propertys
        private string name;     //节点名称
        private ObservableCollection<TreeNode> childs = new ObservableCollection<TreeNode>(); //子节点
        private TreeNode parent; //父节点
        public object Target { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [Display(Name = "名称")]
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.RaisePropertyChanged(() => this.Name);
                }
            }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public TreeNode Parent
        {
            get { return this.parent; }
            set
            {
                if (this.parent != value)
                {
                    this.parent = value;
                    this.RaisePropertyChanged(() => this.Parent);
                }
            }
        }

        /// <summary>
        /// 该节点下的子节点
        /// </summary>
        public ObservableCollection<TreeNode> Childs
        {
            get { return this.childs; }
            set
            {
                this.childs = value;
                this.RaisePropertyChanged(() => this.Childs);
            }
        }
        #endregion

        #region constructs
        public TreeNode()
        {

        }

        public TreeNode(string name)
        {
            this.name = name;
        }

        public TreeNode(string name, IEnumerable<TreeNode> childs)
            : this(name)
        {
            this.Childs = new ObservableCollection<TreeNode>(childs);
        }
        #endregion
     
    }
   public static class TreeNodeExtend
   {
       /// <typeparam name="T">指定对象类型</typeparam>
       /// <param name="source">TreeNode对象</param>
       /// <param name="target">指定对象</param>
       /// <param name="nameProperty">显示的对象属性</param>
       /// <param name="childsProperty">指定对象的子对象</param>
       /// <returns>TreeNode</returns>
       public static TreeNode Parse<T>(this TreeNode source, T target, Expression<Func<string>> nameProperty, Expression<Func<IEnumerable<T>>> childsProperty)
       {
           PropertyInfo propName = PropertySupport.ExtractProperty(nameProperty);
           PropertyInfo propChilds = PropertySupport.ExtractProperty(childsProperty);
           try
           {
               if (target == null) throw new NullReferenceException("解析的目标不能为空！");
               TreeNode node = new TreeNode();
               string nodeName = propName.GetValue(target, null).ToString();
               node.Name = nodeName;
               object obj = propChilds.GetValue(target, null);
               if (obj != null && obj is IEnumerable<T>)
               {
                   foreach (var item in obj as IEnumerable<T>)
                   {
                       var cNode = node.Parse(item, nameProperty, childsProperty);
                       cNode.Target = item;

                       cNode.Parent = node;
                       node.Childs.Add(cNode);
                   }
               }
               node.Target = target;
               return node;
           }
           catch
           {
               throw;
           }
       }
   
   }
    
    
}
