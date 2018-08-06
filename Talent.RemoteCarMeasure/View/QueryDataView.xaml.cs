using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.ClientCommonLib;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.ServiceModel;
using Talent.Measure.WPF;
using Talent.Measure.WPF.Remote;
using Talent.RemoteCarMeasure.Commom;
using Talent.RemoteCarMeasure.ViewModel;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// QueryDataView.xaml 的交互逻辑
    /// </summary>
    public partial class QueryDataView : Only_WindowBase
    {
        public QueryDataView()
        {
            InitializeComponent();
            //Rect rect = SystemParameters.WorkArea;
            //this.MaxWidth = rect.Width;
            //this.MaxHeight = rect.Height;
            //this.WindowState = WindowState.Maximized;
            //this.gridOuter.Width = rect.Width;
            //this.gridOuter.Height = rect.Height;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            FlushMemory();
        }
 

        #region 净重
        /// <summary>
        /// 毛重图片左翻滚按钮事件
        /// </summary>
        private void maoBtnImgLeft_Click(object sender, RoutedEventArgs e)
        {
            maoScrollViewer.ScrollToHorizontalOffset(maoScrollViewer.HorizontalOffset - 140);
        }

        /// <summary>
        /// 毛重图片右翻滚按钮事件
        /// </summary>
        private void maoBtnImgRight_Click(object sender, RoutedEventArgs e)
        {
            maoScrollViewer.ScrollToHorizontalOffset(maoScrollViewer.HorizontalOffset + 140);
        }

        /// <summary>
        /// 皮重图片左翻滚按钮事件
        /// </summary>
        private void piBtnImgLeft_Click(object sender, RoutedEventArgs e)
        {
            piScrollViewer.ScrollToHorizontalOffset(piScrollViewer.HorizontalOffset - 140);
        }

        /// <summary>
        /// 皮重图片右翻滚按钮事件
        /// </summary>
        private void piBtnImgRight_Click(object sender, RoutedEventArgs e)
        {
            piScrollViewer.ScrollToHorizontalOffset(piScrollViewer.HorizontalOffset + 140);
        }
        #endregion

        #region 毛重记录
        /// <summary>
        /// 毛重记录里毛重图片左翻滚按钮事件
        /// </summary>
        private void mzMaoBtnImgLeft_Click(object sender, RoutedEventArgs e)
        {
            mzMaoScrollViewer.ScrollToHorizontalOffset(mzMaoScrollViewer.HorizontalOffset - 140);
        }

        /// <summary>
        /// 毛重记录里毛重图片右翻滚按钮事件
        /// </summary>
        private void mzMaoBtnImgRight_Click(object sender, RoutedEventArgs e)
        {
            mzMaoScrollViewer.ScrollToHorizontalOffset(mzMaoScrollViewer.HorizontalOffset + 140);
        }
        #endregion

        #region 皮重记录
        /// <summary>
        /// 皮重记录里皮重图片左翻滚按钮事件
        /// </summary>
        private void pzPiBtnImgLeft_Click(object sender, RoutedEventArgs e)
        {
            pzPiScrollViewer.ScrollToHorizontalOffset(pzPiScrollViewer.HorizontalOffset - 140);
        }

        /// <summary>
        /// 皮重记录里皮重图片右翻滚按钮事件
        /// </summary>
        private void pzPiBtnImgRight_Click(object sender, RoutedEventArgs e)
        {
            pzPiScrollViewer.ScrollToHorizontalOffset(pzPiScrollViewer.HorizontalOffset + 140);
        }
        #endregion

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.TabControl.SelectedIndex == 0)
            {
                if (this.jzDataGrid.Items.Count > 0)
                {
                    ExportExcel.ExportDataGridSaveAs(true, this.jzDataGrid);
                }
            }
            else if (this.TabControl.SelectedIndex == 1)
            {
                if (this.mzDataGrid.Items.Count > 0)
                {
                    ExportExcel.ExportDataGridSaveAs(true, this.mzDataGrid);
                }
            }
            else if (this.TabControl.SelectedIndex == 2)
            {
                if (this.pzDataGrid.Items.Count > 0)
                {
                    ExportExcel.ExportDataGridSaveAs(true, this.pzDataGrid);
                }
            }

        }

        /// <summary>
        /// 图片鼠标左键事件
        /// </summary>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image bi = sender as Image;
        }

        private Image bi;
        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (bi == null)
            {
                bi = sender as Image;
                ShowEnlargeImage(bi);
            }
        }

        /// <summary>
        /// 显示放大的图片
        /// </summary>
        /// <param name="bi"></param>
        private void ShowEnlargeImage(Image bi)
        {
            this.EnlargeImage.Source = bi.Source;
            this.EnlargeImage.Visibility = Visibility.Visible;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            this.EnlargeImage.Source = null;
            bi = null;
            this.EnlargeImage.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 双击皮重记录，显示历史皮重
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pzDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point aP = e.GetPosition(pzDataGrid);
            IInputElement obj = pzDataGrid.InputHitTest(aP);
            DependencyObject target = obj as DependencyObject;
            while (target != null)
            {
                if (target is DataGridRow)
                {
                    WeightRecord sp = pzDataGrid.SelectedItem as WeightRecord;
                    ShowHistoryTare(sp);
                    break;
                }
                target = VisualTreeHelper.GetParent(target);
            }

        }
        /// <summary>
        /// 显示历史皮重
        /// </summary>
        /// <param name="sp"></param>
        private void ShowHistoryTare(WeightRecord sp)
        {
            string serviceUrl = ConfigurationManager.AppSettings["getCarHistoryTare"].ToString().Replace('$', '&');
            string getUrl = string.Format(serviceUrl, sp.carno, Convert.ToDecimal(sp.tare)*1000);
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(GetCarHistoryTare1Callback), request);
        }
        /// <summary>
        /// 异步调用返回值
        /// </summary>
        /// <param name="asyc"></param>
        private void GetCarHistoryTare1Callback(IAsyncResult asyc)
        {
            try
            {
                MeasureServiceModel mServiceModel;
                string strResult = ComHelpClass.ResponseStr(asyc);
                mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ShowHistoryTareView view = new ShowHistoryTareView(mServiceModel.rows);
                    view.ShowDialog();
                }));
            }
            catch //(Exception ex)
            {
            }
        }
        /// <summary>
        /// 窗体拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
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



    public class MyClass
    {
        public string ID { get; set; }

        public string CarNum { get; set; }

        public string Bis { get; set; }

        public string ProductName { get; set; }

        public string Provide { get; set; }

        public string Delivery { get; set; }

        public string Mao { get; set; }

        public string Pi { get; set; }

        public string Kou { get; set; }

        public string Jing { get; set; }

    }


}
