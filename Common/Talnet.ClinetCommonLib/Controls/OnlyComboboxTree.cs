using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Talent.ClientCommonLib
{
    [TemplatePart(Name = "tree", Type = typeof(TreeView))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_TextBlock", Type = typeof(TextBlock))]
    public class OnlyComboboxTree : ComboBox
    {
        static OnlyComboboxTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OnlyComboboxTree), new FrameworkPropertyMetadata(typeof(OnlyComboboxTree)));
        }

        private TreeView _tree;
        private Popup _popup;
        private TextBlock _textBlock;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _tree = GetTemplateChild("tree") as TreeView;
            _popup = GetTemplateChild("PART_Popup") as Popup;
            _textBlock = GetTemplateChild("PART_TextBlock") as TextBlock;
            _tree.MouseDoubleClick += new MouseButtonEventHandler(_tree_MouseDoubleClick);
        }

        void _tree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var node = this._tree.SelectedItem as TreeNode;
            if (node != null)
            {
                this._textBlock.Text = node.Name;
                _popup.IsOpen = false;
            }
        }

        public static readonly DependencyProperty MyItemsSourceProperty =
           DependencyProperty.Register("MyItemsSource", typeof(IEnumerable), typeof(OnlyComboboxTree),
               new FrameworkPropertyMetadata(null,
                   new PropertyChangedCallback(OnMyItemsSourceChanged)));

        public IEnumerable MyItemsSource
        {
            get { return (IEnumerable)GetValue(MyItemsSourceProperty); }
            set { SetValue(MyItemsSourceProperty, value); }
        }

        private static void OnMyItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var control = d as MyComboboxTree;
            //if (control != null&&control._tree!=null)
            //{
            //    control._tree.ItemsSource = e.NewValue as IEnumerable;
            //}
        }
    }
}
