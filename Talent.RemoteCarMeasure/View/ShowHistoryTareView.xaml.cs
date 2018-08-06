using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.ClientCommonLib;
using Talent.RemoteCarMeasure.Commom;
using System.Collections.ObjectModel;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// 显示历史皮重
    /// </summary>
    public partial class ShowHistoryTareView : Window
    {

        public ObservableCollection<BullInfo> HistotyTare { get; set; }
        public ShowHistoryTareView(List<BullInfo> dataInfos)
        {
            InitializeComponent();
            HistotyTare = new ObservableCollection<BullInfo>();
            for (int i = 0; i < dataInfos.Count;i++ )
            {
                HistotyTare.Add(dataInfos[i]);
            }
            this.DataContext = this;
            
        }
        /// <summary>
        /// 关闭
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // 支持标题栏拖动  
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            bool isFirstChangeWindowsState = false;
            base.OnMouseLeftButtonDown(e);
            // 获取鼠标相对标题栏位置  
            Point position = e.GetPosition(gridTitleBar);
            // 如果鼠标位置在标题栏内，允许拖动  
            if (position.X >= 0 && position.X < gridTitleBar.ActualWidth && position.Y >= 0 && position.Y < gridTitleBar.ActualHeight)
            {
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1)
                {
                    this.DragMove();
                }
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                {
                    if (this.WindowState == WindowState.Normal)
                    {
                        isFirstChangeWindowsState = true;
                        this.WindowState = WindowState.Maximized;
                    }
                    if (isFirstChangeWindowsState == false)
                    {
                        if (this.WindowState == WindowState.Maximized)
                        {
                            this.WindowState = WindowState.Normal;
                        }
                    }

                }

            }

        }
       
    }
}
