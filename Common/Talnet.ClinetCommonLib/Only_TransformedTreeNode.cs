using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talent.ClientCommonLib
{

    /// <summary>
    /// 重构的树节点
    /// 
    /// </summary>
    public class TransformedTreeNode : TreeNode
    {
        bool _IsSel = false;
        bool _IsExpanded = false;
        int _level;
        string _ID;
        int? _SequenceNumber;

        /// <summary>
        /// 重写父节点
        /// </summary>
        TransformedTreeNode _parent;
        public new TransformedTreeNode Parent
        {
            get { return this._parent; }
            set
            {
                this._parent = value;
                this.RaisePropertyChanged("Parent");
            }
        }

        /// <summary>
        /// 重写子节点
        /// </summary>
        ObservableCollection<TransformedTreeNode> _childs = new ObservableCollection<TransformedTreeNode>();
        public new ObservableCollection<TransformedTreeNode> Childs
        {
            get { return this._childs; }
            set
            {
                this._childs = value;
                this.RaisePropertyChanged("Childs");
            }

        }

        /// <summary>
        /// 这个属性的名称要与属性值的名称一模一样
        /// </summary>
        public bool IsSelected
        {
            get { return this._IsSel; }
            set
            {
                if (value != _IsSel)
                {

                    this._IsSel = value;
                    this.RaisePropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// 这个属性的名称要与属性值的名称一模一样
        /// </summary>
        public bool IsExpanded
        {
            get { return this._IsExpanded; }
            set
            {
                if (value != _IsExpanded)
                {

                    this._IsExpanded = value;
                    this.RaisePropertyChanged("IsExpanded");
                }
            }
        }

        /// <summary>
        /// 节点层级
        /// </summary>
        public int Level
        {
            get { return this._level; }
            set
            {
                if (value != _level)
                {

                    this._level = value;
                    this.RaisePropertyChanged("Level");
                }
            }
        }

        /// <summary>
        /// 节点ID
        /// </summary>
        public string ID
        {
            get { return this._ID; }
            set
            {
                if (value != _ID)
                {

                    this._ID = value;
                    this.RaisePropertyChanged("ID");
                }
            }
        }

        /// <summary>
        /// 顺序号
        /// </summary>
        public int? SequenceNumber
        {
            get { return this._SequenceNumber; }
            set
            {
                if (value != _SequenceNumber)
                {

                    this._SequenceNumber = value;
                    this.RaisePropertyChanged("SequenceNumber");
                }
            }
        }

        public TransformedTreeNode ParseList<T>(T target, string idnameProperty, string nameProperty, string childsProperty)
        {
            TransformedTreeNode node = new TransformedTreeNode();
            node.Target = target;
            if (target != null)
            {

                //判定子对象属性是否存在
                if (target.GetType().GetProperty(idnameProperty) != null)
                {
                    //获取子对象
                    object objValue = target.GetType().GetProperty(idnameProperty).GetValue(target, null);
                    node.ID = Convert.ToString(objValue);
                }

                //判定子对象属性是否存在
                if (target.GetType().GetProperty(nameProperty) != null)
                {
                    //获取子对象
                    object objValue = target.GetType().GetProperty(nameProperty).GetValue(target, null);
                    node.Name = Convert.ToString(objValue);
                }

                //判定子对象属性是否存在
                if (target.GetType().GetProperty(childsProperty) != null)
                {
                    //获取子对象
                    object objValue = target.GetType().GetProperty(childsProperty).GetValue(target, null);
                    if (objValue != null)
                    {
                        IEnumerable<T> childs = objValue as IEnumerable<T>;
                        if (childs != null)
                        {
                            foreach (var item in childs)
                            {
                                TransformedTreeNode childNode = new TransformedTreeNode();
                                childNode = ParseList(item, idnameProperty, nameProperty, childsProperty);
                                childNode.Parent = node;
                                node.Childs.Add(childNode);
                            }
                        }
                    }
                }
            }

            return node;

        }
    }
}

