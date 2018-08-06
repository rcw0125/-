using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talent.ClientCommonLib;

namespace Talent.Measure.DomainModel.CommonModel
{
    public class SomeHierarchyViewModel : ITreeViewItemModel
    {
        public SomeHierarchyViewModel(string title, List<SomeHierarchyViewModel> children, Object obj, SomeHierarchyViewModel parent = null)
        {
            this.Title = title;
            this.Parent = parent;
            this.Children = children;
            this.obj = obj;

            if (this.Children != null)
            {
                this.Children.ForEach(ch => ch.Parent = this);
            }
        }
        public object obj { get; set; }
        public string Title { get; set; }

        public SomeHierarchyViewModel Parent { get; set; }

        public List<SomeHierarchyViewModel> Children { get; set; }


        #region ITreeViewItemModel
        public string SelectedValuePath
        {
            get { return Title; }
        }

        public string DisplayValuePath
        {
            get { return Title; }
        }

        private bool isExpanded;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public IEnumerable<ITreeViewItemModel> GetHierarchy()
        {
            return GetAscendingHierarchy().Reverse().Cast<ITreeViewItemModel>();
        }

        public IEnumerable<ITreeViewItemModel> GetChildren()
        {
            if (this.Children != null)
            {
                return this.Children.Cast<ITreeViewItemModel>();
            }

            return null;
        }

        #endregion

        private IEnumerable<SomeHierarchyViewModel> GetAscendingHierarchy()
        {
            var vm = this;

            yield return vm;
            while (vm.Parent != null)
            {
                yield return vm.Parent;
                vm = vm.Parent;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

        public class CovertToTree
        {
          
            /// <summary>
            /// 转换树结构为树节点
            /// </summary>
            /// <param name="pTreeNodeSour"></param>
            /// <returns></returns>
            public static List<SomeHierarchyViewModel> CovertObjToTree(IEnumerable<TreeNode> pTreeNodeSour, ref List<SomeHierarchyViewModel> treeList)
            {
                List<SomeHierarchyViewModel> rtn = new List<SomeHierarchyViewModel>();
                foreach (var item in pTreeNodeSour)
                {
                    if (item.Childs.Count > 0)
                    {
                        SomeHierarchyViewModel temp = new SomeHierarchyViewModel("", new List<SomeHierarchyViewModel>(), item);
                        temp.Title = item.Name;
                        treeList.Add(temp);
                        List<SomeHierarchyViewModel> listTemp = CovertObjToTree(item.Childs, ref treeList);
                        listTemp.ForEach(n => n.Parent = temp);
                        temp.Children = listTemp;

                        rtn.Add(temp);
                    }
                    else
                    {
                        SomeHierarchyViewModel temp = new SomeHierarchyViewModel("", new List<SomeHierarchyViewModel>(), item);
                        temp.Title = item.Name;
                        temp.obj = item;
                        temp.Children = null;
                        rtn.Add(temp);
                        treeList.Add(temp);
                    }
                }
                return rtn;
            }
        }
}
