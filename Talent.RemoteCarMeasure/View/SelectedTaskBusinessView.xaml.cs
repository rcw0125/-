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
using Talent.Measure.DomainModel;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// SelectedTaskBusinessView.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedTaskBusinessView : Window
    {

        private IList<BullInfo> bulls;
        /// <summary>
        /// 业务信息集合
        /// </summary>
        public IList<BullInfo> Bulls
        {
            get { return bulls; }
            set
            {
                bulls = value;
            }
        }

        private BullInfo selectedBull;
        /// <summary>
        /// 选择的业务信息
        /// </summary>
        public BullInfo SelectedBull
        {
            get { return selectedBull; }
            set { selectedBull = value; }
        }

        public SelectedTaskBusinessView(IList<BullInfo> Bulls)
        {
            InitializeComponent();
            this.Bulls = Bulls;
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataGrid.ItemsSource = this.Bulls;
        }

        /// <summary>
        /// 确定按钮事件
        /// </summary>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择一条业务信息");
                return;
            }
            this.SelectedBull = this.DataGrid.SelectedItem as BullInfo;
            this.Close();
        }
    }
}
