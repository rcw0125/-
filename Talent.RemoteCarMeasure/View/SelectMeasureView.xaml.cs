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
using Talent.RemoteCarMeasure.Model;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// 选择衡器的交互逻辑
    /// </summary>
    public partial class SelectMeasureView : Window
    {
        private int formState = 0;
        /// <summary>
        /// 窗体状态(0:关闭窗体;1:确定;2:取消;)
        /// </summary>
        public int FormState
        {
            get { return formState; }
            set { formState = value; }
        }

        private IList<SeatAttentionWeightModel> measures;
        /// <summary>
        /// 衡器集合
        /// </summary>
        public IList<SeatAttentionWeightModel> Measures
        {
            get { return measures; }
            set
            {
                measures = value;
            }
        }

        private SeatAttentionWeightModel selectedMeasure;
        /// <summary>
        /// 选择的称点
        /// </summary>
        public SeatAttentionWeightModel SelectedMeasure
        {
            get { return selectedMeasure; }
            set { selectedMeasure = value; }
        }


        public SelectMeasureView(IList<SeatAttentionWeightModel> measures)
        {
            InitializeComponent();
            this.Measures = measures;
        }
        /// <summary>
        /// 用户录入信息
        /// </summary>
        public string memoInfos = string.Empty;
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataGrid.ItemsSource = this.Measures;
            this.msgTxt.Text = "补打";
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            FormState = 0;
            this.Close();
        }

        /// <summary>
        /// 确定按钮事件
        /// </summary>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择一条衡器信息");
                return;
            }
            memoInfos = this.msgTxt.Text.Trim();
            if(string.IsNullOrEmpty(memoInfos))
            {
                MessageBox.Show("请录入补打内容");
                return;
            } 
            this.SelectedMeasure = this.DataGrid.SelectedItem as SeatAttentionWeightModel;
        
            FormState = 1;
            this.Close();
        }

        /// <summary>
        /// 取消按钮事件
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            FormState = 2;
            this.Close();
        }
        /// <summary>
        /// 第一个选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SetValue(this.r1.Content.ToString());
        }
        /// <summary>
        /// 第二个选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            SetValue(this.r2.Content.ToString());

        }
        /// <summary>
        /// 第三个选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            SetValue(this.r3.Content.ToString());
        }
        /// <summary>
        /// 设置选中的值
        /// </summary>
        /// <param name="inV"></param>
        /// <param name="isCheck"></param>
        private void SetValue(string inV)
        {
            //switch (inV)
            //{
            //    case "打印机缺纸":
            //        this.msgTxt.Text = this.msgTxt.Text + inV;
            //        this.msgTxt.Text = this.msgTxt.Text.Replace(this.r2.Content.ToString(),"");
            //        this.msgTxt.Text = this.msgTxt.Text.Replace(this.r3.Content.ToString(), "");
            //        break;
            //    case "打印机故障":
            //        this.msgTxt.Text = this.msgTxt.Text + inV;
            //        this.msgTxt.Text = this.msgTxt.Text.Replace(this.r1.Content.ToString(), "");
            //        this.msgTxt.Text = this.msgTxt.Text.Replace(this.r3.Content.ToString(), "");
            //        break;
            //    case "票据丢失":
            //        this.msgTxt.Text = this.msgTxt.Text + inV;
            //        this.msgTxt.Text = this.msgTxt.Text.Replace(this.r2.Content.ToString(), "");
            //        this.msgTxt.Text = this.msgTxt.Text.Replace(this.r1.Content.ToString(), "");
            //        break;
            //}

            this.msgTxt.Text = inV;
              
        
        }
    }
}
