using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Talent.ClientCommonLib
{
    public class ExtendedTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            var childItem = ExtendedTreeViewItem.CreateItemWithBinding();

            childItem.OnHierarchyMouseUp += OnChildItemMouseLeftButtonUp;

            return childItem;
        }

        private void OnChildItemMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (this.OnHierarchyMouseUp != null)
            {
                this.OnHierarchyMouseUp(this, e);
            }
        }

        public event MouseEventHandler OnHierarchyMouseUp;
    }

    public class ExtendedTreeViewItem : TreeViewItem
    {
        public ExtendedTreeViewItem()
        {
            this.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.OnHierarchyMouseUp != null)
            {
                this.OnHierarchyMouseUp(this, e);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var childItem = CreateItemWithBinding();

            childItem.MouseLeftButtonUp += OnMouseLeftButtonUp;

            return childItem;
        }

        public static ExtendedTreeViewItem CreateItemWithBinding()
        {
            var tvi = new ExtendedTreeViewItem();

            var expandedBinding = new Binding("IsExpanded");
            expandedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(ExtendedTreeViewItem.IsExpandedProperty, expandedBinding);

            var selectedBinding = new Binding("IsSelected");
            selectedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(ExtendedTreeViewItem.IsSelectedProperty, selectedBinding);

            return tvi;
        }

        public event MouseEventHandler OnHierarchyMouseUp;
    }
}
