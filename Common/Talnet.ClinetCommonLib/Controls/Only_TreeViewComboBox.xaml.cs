using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Talent.ClientCommonLib
{
    /// <summary>
    /// Only_TreeViewComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class Only_TreeViewComboBox : UserControl
    {
        public Only_TreeViewComboBox()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            tvType.SetBinding(TreeView.ItemsSourceProperty, new System.Windows.Data.Binding() { Source = this, Path = new PropertyPath("ItemsSource2"), Mode = System.Windows.Data.BindingMode.TwoWay });
        }

        #region ItemsSource2属性
        public static readonly DependencyProperty ItemsSource2Property =
            DependencyProperty.Register("ItemsSource2", typeof(IEnumerable), typeof(Only_TreeViewComboBox),
            new PropertyMetadata(null));

        public IEnumerable ItemsSource2
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSource2Property);
            }
            set
            {
                SetValue(ItemsSource2Property, value);
            }
        }
        #endregion

        #region SelectedItem2属性
        public static readonly DependencyProperty SelectedItem2Property =
           DependencyProperty.Register("SelectedItem2", typeof(object), typeof(Only_TreeViewComboBox),
           new PropertyMetadata(null, new PropertyChangedCallback((obj, arg) =>
           {
               (obj as Only_TreeViewComboBox).SelectedChangedMethod();
           }
           )));

        public object SelectedItem2
        {
            get
            {
                return (object)GetValue(SelectedItem2Property);
            }
            set
            {
                SetValue(SelectedItem2Property, value);
            }
        }
        #endregion

        private void SelectedChangedMethod()
        {
            List<object> templist = new List<object>();
            foreach (object obj in cbxType.Items)
            {
                if (!(obj is TreeView))
                {
                    templist.Add(obj);
                }
            }
            foreach (object obj in templist)
            {
                cbxType.Items.Remove(obj);
            }
            if (SelectedItem2 == null)
                return;
            cbxType.Items.Insert(0, SelectedItem2);
            cbxType.SelectedItem = cbxType.Items[0];
        }

        private void tvType_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvType.SelectedItem != null)
                SelectedItem2 = (tvType.SelectedItem as TreeNode).Target;
            else
                SelectedItem2 = null;
        }

        private void cbxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxType.SelectedItem is TreeView)
            {
                cbxType.SelectedItem = null;
                return;
            }
            if (cbxType.SelectedItem != null)
                SelectedItem2 = cbxType.SelectedItem;
        }
    }
}
