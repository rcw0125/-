using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.Measure.DomainModel.CommonModel;
using Talent.ClientCommonLib;

namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// IoConfigChildForm.xaml 的交互逻辑
    /// </summary>
    public partial class IoConfigChildForm : Window
    {
        private EquConfigModel equConfig;
        /// <summary>
        /// 设备配置
        /// </summary>
        public EquConfigModel EquConfig
        {
            get { return equConfig; }
            set { equConfig = value; }
        }
        public bool isAddForm { get; set; }
        public bool IsFormCancel { get; set; }
        /// <summary>
        /// 下拉树数据源集合
        /// </summary>
        private List<SomeHierarchyViewModel> comboBoxItemSource = new List<SomeHierarchyViewModel>();
        /// <summary>
        /// 下拉树集合
        /// </summary>
        private List<SomeHierarchyViewModel> comboBoxTreeList = new List<SomeHierarchyViewModel>();
        /// <summary>
        /// 窗体构造函数
        /// </summary>
        /// <param name="equList">下拉树捆绑的数据源</param>
        /// <param name="equConfig">设备配置对象</param>
        /// <param name="isAddForm"></param>
        public IoConfigChildForm(List<SomeHierarchyViewModel> equList,List<SomeHierarchyViewModel> equConnections,EquConfigModel equConfig,bool isAddForm)
        {
            InitializeComponent();
            this.comboBoxItemSource = equList;
            this.comboBoxTreeList = equConnections;
            this.EquConfig = equConfig;
            this.isAddForm = isAddForm;
            if (isAddForm)
            {
                EquConfig = new EquConfigModel();
            }
        }

        /// <summary>
        /// 保存按钮事件
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            EquConfig.EquName = (this.comboBoxTree.SelectedItem as SomeHierarchyViewModel).Title;
            EquConfig.IsUse = (bool)this.yesCheckBox.IsChecked;
            EquConfig.Port = this.PortTextBox.Text;
            EquConfig.Code = this.equCodeTextBox.Text;
            this.IsFormCancel = false;
            this.Close();
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsFormCancel = true;
            this.Close();
        }

        /// <summary>
        /// 是否在用"是"选中事件
        /// </summary>
        private void yesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.yesCheckBox.IsChecked == true)
            {
                if (this.noCheckBox==null)
                {
                    return;
                }
                this.noCheckBox.IsChecked = false;
            }
            else
            {
                this.noCheckBox.IsChecked = true;
            }
        }
        /// <summary>
        /// 是否在用"否"选中事件
        /// </summary>
        private void noCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.noCheckBox.IsChecked == true)
            {
                this.yesCheckBox.IsChecked = false;
            }
            else
            {
                this.yesCheckBox.IsChecked = true;
            }
        }

        /// <summary>
        /// 下拉树选择改变事件
        /// </summary>
        private void comboBoxTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems!=null&&e.AddedItems.Count>0)
            {
                SomeHierarchyViewModel model = e.AddedItems[0] as SomeHierarchyViewModel;
                var obj = model.obj as TreeNode;
                ComBoxTreeModel treeModel = obj.Target as ComBoxTreeModel;
                this.equCodeTextBox.Text = treeModel.Code;
                if (EquConfig!=null)
                {
                    EquConfig.Type = treeModel.Type;
                }
            }
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.comboBoxTree.ItemsSource = this.comboBoxItemSource;
            if (isAddForm)
            {
                this.comboBoxTree.SelectedItem = comboBoxItemSource.First().Children.First().Children.First();
                this.PortTextBox.Text = string.Empty;
            }
            else
            {
                this.PortTextBox.Text = equConfig.Port;
                var selects=(from r in comboBoxTreeList where r.Title == equConfig.EquName select r).ToList();
                this.comboBoxTree.SelectedItem = selects.Count > 0 ? selects.First() : null;
                this.yesCheckBox.IsChecked = equConfig.IsUse;
                this.noCheckBox.IsChecked = !equConfig.IsUse;
            }
        }
    }
}
