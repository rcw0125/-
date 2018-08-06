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
using System.Windows.Threading;

namespace Talent.ClientCommonLib
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Only.BT.CommonLib"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Only.BT.CommonLib;assembly=Only.BT.CommonLib"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:TLH_Busy/>
    ///
    /// </summary>
    [TemplateVisualState(Name = "Busy", GroupName = "BusyGroup")]
    [TemplateVisualState(Name = "UnBusy", GroupName = "BusyGroup")]
    public class AutoHideMsgTip : Control
    {
        DispatcherTimer _timer;
        int _secondCount = 0;
        int _totalSecond = 5;
        public AutoHideMsgTip()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(TLH_Busy), new FrameworkPropertyMetadata(typeof(TLH_Busy)));
            this.DefaultStyleKey = typeof(AutoHideMsgTip);
            this.Visibility = Visibility.Collapsed;

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += _timer_Tick;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public readonly static DependencyProperty ISBusyProperty = DependencyProperty.Register("ISBusy1",
            typeof(bool),
            typeof(AutoHideMsgTip),
            new PropertyMetadata(default(bool), new PropertyChangedCallback((sender, arg) =>
            {
                (sender as AutoHideMsgTip).SetVisibily();
            })));

        public bool ISBusy1
        {
            get
            {
                return (bool)GetValue(ISBusyProperty);
            }
            set
            {
                SetValue(ISBusyProperty, value);
            }
        }
        private void ShowMsg()
        {
            _secondCount = 0;  
            this.Visibility = System.Windows.Visibility.Visible;
            _timer.Start();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _secondCount++;
            if (_secondCount >= _totalSecond)
            {
                _timer.Stop();
                this.Visibility = Visibility.Hidden;
            }
        }
        private void SetVisibily()
        {
            if (ISBusy1)
            {
                this.Visibility = System.Windows.Visibility.Visible;
                ShowMsg();
            }
            else
                this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public readonly static DependencyProperty Text1Property = DependencyProperty.Register("Text1", typeof(string), typeof(AutoHideMsgTip),
            new PropertyMetadata(default(string)));

        public string Text1
        {
            get
            {
                return (string)GetValue(Text1Property);
            }
            set
            {
                SetValue(Text1Property, value);
               
            }
        }
    }
}
