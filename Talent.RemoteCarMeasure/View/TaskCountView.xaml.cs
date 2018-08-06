using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.ClientCommonLib;
using Talent.Measure.WPF;
using Talent.RemoteCarMeasure.Commom;
using Talent.RemoteCarMeasure.ViewModel;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// TaskCountView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskCountView 
    {
        public TaskCountView()
        {
            InitializeComponent();
            //Rect rect = SystemParameters.WorkArea;
            //this.MaxWidth = rect.Width;
            //this.MaxHeight = rect.Height;
            //this.WindowState = WindowState.Maximized;
            //this.gridOuter.Width = rect.Width;
            //this.gridOuter.Height = rect.Height;

            this.InitData();
        }

        private void InitData()
        {
            //List<ChartClass> list = new List<ChartClass>();
            //list.Add(new ChartClass() { Name = "1", Value = 124 });
            //list.Add(new ChartClass() { Name = "2", Value = 283 });
            //list.Add(new ChartClass() { Name = "3", Value = 168 });
            //list.Add(new ChartClass() { Name = "4", Value = 108 });
            //list.Add(new ChartClass() { Name = "5", Value = 69 });

            ////((LineSeries)mcChart.Series[0]).ItemsSource = list;
            //((LineSeries)mcChart.Series[0]).ItemsSource = list;
            //((LineSeries)mcChart1.Series[0]).ItemsSource = list;
            //((ColumnSeries)mcChart2.Series[0]).ItemsSource = list;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            FlushMemory();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.tabStat.SelectedIndex == 0)
            {
                ExportExcel.ExportDataGridSaveAs(true, userStat);
            }
            else if (this.tabStat.SelectedIndex == 1)
            {
                ExportExcel.ExportDataGridSaveAs(true, sumStat);
            }
            else if (this.tabStat.SelectedIndex == 2)
            {
                ExportExcel.ExportDataGridSaveAs(true, weightStat);
            }
        }

        private void TaskCount_Loaded(object sender, RoutedEventArgs e)
        {
            TaskStatViewModel tsvm = this.DataContext as TaskStatViewModel;
            tsvm.PjsdChart = this.pjsdChart;
            tsvm.CsChart = this.csChart;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);
        public void FlushMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                }
            }
            catch {}
        }

    }
}
