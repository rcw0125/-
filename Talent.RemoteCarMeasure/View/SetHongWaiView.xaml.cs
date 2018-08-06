using System;
using System.Collections.Generic;
using System.Configuration;
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
using Talent.ClientCommonLib;
using Talent.ClientCommonLib.Controls;
using Talent.Measure.DomainModel.CommonModel;
using Talent.CommonMethod;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Talent.Measure.DomainModel;
using Talent.Measure.WPF.Remote;
using Talent.Measure.WPF.Log;
using System.Collections.ObjectModel;
using Talent.RemoteCarMeasure.Model;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// 红外设置的交互逻辑
    /// </summary>
    public partial class SetHongWaiView : Only_WindowBase
    {
        private ObservableCollection<BaseModel> comboxSources = null;
        public SetHongWaiView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void SetHongWaiView_Loaded(object sender, RoutedEventArgs e)
        {
            InitHongWaiComboxSource();
            SetSelectedHongWaiCombox();
        }

        /// <summary>
        /// 设置选中的红外下拉框项
        /// </summary>
        private void SetSelectedHongWaiCombox()
        {
            //string lineMark = Properties.Settings.Default.IsLineMeasure;
            //if (!string.IsNullOrEmpty(lineMark))
            //{
            //    var coms = comboxSources.Where(r => r.Id == lineMark).ToList();
            //    if (coms != null && coms.Count > 0)
            //    {
            //        this.hongWaiComboBox.SelectedItem = coms.First();
            //    }
            //}
        }

        /// <summary>
        /// 构造红外下拉框数据源
        /// </summary>
        private void InitHongWaiComboxSource()
        {
            List<BaseModel> bms = new List<BaseModel>();
            BaseModel bm1 = new BaseModel() { Id = "1", Name = "否" };
            BaseModel bm2 = new BaseModel() { Id = "2", Name = "是" };
            bms.Add(bm1);
            bms.Add(bm2);
            comboxSources = new ObservableCollection<BaseModel>(bms);
            this.hongWaiComboBox.ItemsSource = comboxSources;
        }

        /// <summary>
        /// 确定按钮事件
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.hongWaiComboBox.SelectedItem != null)
            {
                
            }
        }

        /// <summary>
        /// 取消按钮
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void ModifyPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
    }
}
