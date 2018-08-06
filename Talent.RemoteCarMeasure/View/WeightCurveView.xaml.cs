using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Talent.ClientCommonLib;
using Talent.RemoteCarMeasure.Report;
using Talent.RemoteCarMeasure.ViewModel;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// WeightCurveView.xaml 的交互逻辑
    /// </summary>
    public partial class WeightCurveView : Only_WindowBase
    {
        private LineAndMarker<ElementMarkerPointsGraph> chart;
        private IPointDataSource ds;
        public WeightCurveView()
        {
            InitializeComponent();
            //Rect rect = SystemParameters.WorkArea;
            //this.MaxWidth = rect.Width;
            //this.MaxHeight = rect.Height;
            //this.WindowState = WindowState.Maximized;
            //this.gridOuter.Width = rect.Width;
            //this.gridOuter.Height = rect.Height;
            plotter.Legend.LegendLeft = 10;
            plotter.Legend.LegendRight = Double.NaN;
            WeightCurveViewModel vm = this.DataContext as WeightCurveViewModel;
            vm.plotter = plotter;
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
        /// 报表控制可用性更改触发的事件
        /// </summary>
        private void reportControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                WeightCurveViewModel vm = this.DataContext as WeightCurveViewModel;
                if (vm.WeightCurveDataList.Count > 0)
                {
                    #region 曲线1（不用）
                    ////ReportTest tReportTest = new ReportTest();
                    ////tReportTest.SetCrossTableDataSource(vm.WeightCurveDataList);
                    ////this.rptViewParts.ReportSource = tReportTest;
                    //var xMin = (from r in vm.WeightCurveDataList select DateTime.Parse(r.WeightTime)).Min();
                    //var xMax = (from r in vm.WeightCurveDataList select DateTime.Parse(r.WeightTime)).Max();
                    //var yMin = (from r in vm.WeightCurveDataList select r.recorddata).Min();
                    //var yMax = (from r in vm.WeightCurveDataList select r.recorddata).Max();
                    //var xList = (from r in vm.WeightCurveDataList select DateTime.Parse(r.WeightTime).ToOADate()).ToArray();
                    //var yList = (from r in vm.WeightCurveDataList select (double)r.recorddata).ToArray();
                    //chartContrl1.Chart_DrawLine(0, xList, yList, Brushes.Black, 3f);
                    //chartContrl1.Chart_setXY(xMin.ToOADate(), (double)yMin, xMax.ToOADate(), (double)yMax);
                    //foreach (var item in vm.WeightCurveDataList)
                    //{
                    //    chartContrl1.Chart_AddLinePoint(0, DateTime.Parse(item.WeightTime).ToOADate(), (double)item.recorddata);
                    //}
                    #endregion

                    ShowCurve(vm);
                    vm.IsReportShow = false;
                }
                else
                {
                    //ReportNoData tReportNoData = new ReportNoData();
                    //this.rptViewParts.ReportSource = tReportNoData;
                }
            }
            else
            {
                //ReportNoData tReportNoData = new ReportNoData();
                //this.rptViewParts.ReportSource = tReportNoData;                  
            }
        }

        /// <summary>
        /// 显示曲线图表
        /// </summary>
        /// <param name="vm"></param>
        private void ShowCurve(WeightCurveViewModel vm)
        {
            DateTime[] dates = (from r in vm.WeightCurveDataList select DateTime.Parse(r.recordtime)).ToArray();
            decimal[] weightList = (from r in vm.WeightCurveDataList select r.recorddata).ToArray();

            var xDataSource = new EnumerableDataSource<DateTime>(dates);
            xDataSource.SetXMapping(x => dateAxis.ConvertToDouble(x));

            var yDataSource = new EnumerableDataSource<decimal>(weightList);
            yDataSource.SetYMapping(y => Convert.ToDouble(y));
            yDataSource.AddMapping(ShapeElementPointMarker.ToolTipTextProperty, Y => String.Format("{0}", Y));

            ds = new CompositeDataSource(xDataSource, yDataSource);
            plotter.Children.RemoveAll(typeof(LineGraph));
            plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
            plotter.Children.RemoveAll(typeof(ElementMarkerPointsGraph));
            plotter.Children.RemoveAll(typeof(Microsoft.Research.DynamicDataDisplay.Charts.Navigation.CursorCoordinateGraph));
            chart = plotter.AddLineGraph(ds,
                   new Pen(Brushes.LimeGreen, 2),
                    new CircleElementPointMarker
                    {
                        Size = 5,
                        Brush = Brushes.Red,
                        Fill = Brushes.Orange,
                        Pen = new Pen(Brushes.Blue, 5)
                    },
                    new PenDescription("重量曲线"));
            plotter.Children.Add(new CursorCoordinateGraph());
            plotter.FitToView();
        }

        private void markerButton_Click(object sender, RoutedEventArgs e)
        {
            if (chart.MarkerGraph.DataSource != null)
            {
                chart.MarkerGraph.DataSource = null;
                this.markerButton.Content = "显示标签";
            }
            else
            {
                chart.MarkerGraph.DataSource = ds;
                this.markerButton.Content = "隐藏标签";
            }
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
            catch{}
        }
    }
}
