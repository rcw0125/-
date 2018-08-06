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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    ///     <MyNamespace:TLH_ChildWindow/>
    ///
    /// </summary>
    public class Only_ChildWindow : ContentControl
    {
        public Only_ChildWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(TLH_ChildWindow), new FrameworkPropertyMetadata(typeof(TLH_ChildWindow)));
            this.DefaultStyleKey = typeof(Only_ChildWindow);
        }

        #region 成员变量
        Storyboard Storyboard_Open, Storyboard_Close;
        Button CloseButton, bntOK, bntCancel;
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Storyboard_Open = this.GetTemplateChild("Storyboard_Open") as Storyboard;
            Storyboard_Close = this.GetTemplateChild("Storyboard_Close") as Storyboard;

            bntOK = GetTemplateChild("bntOK") as Button;
            CloseButton = GetTemplateChild("CloseButton") as Button;
            bntCancel = GetTemplateChild("bntCancel") as Button;
            SetChildWindowState();
            initEvent();
        }

        #region 加载事件
        public void initEvent()
        {
            if (Storyboard_Close != null)
                Storyboard_Close.Completed += Storyboard_Close_Completed;

            if (bntOK != null)
                bntOK.Click += new RoutedEventHandler(bntOK_Click);

            if (CloseButton != null)
                CloseButton.Click += new RoutedEventHandler(CloseButton_Click);

            if (bntCancel != null)
                bntCancel.Click += new RoutedEventHandler(bntCancel_Click);
        }

        void bntCancel_Click(object sender, RoutedEventArgs e)
        {
            Shown = false;
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Shown = false;
        }

        void bntOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        void Storyboard_Close_Completed(object sender, EventArgs e)
        {
            this.Focus();
        }
        #endregion

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Only_ChildWindow),
            new PropertyMetadata(default(string)));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty WindowHeightProperty = DependencyProperty.Register("WindowHeight", typeof(double), typeof(Only_ChildWindow),
            new PropertyMetadata(double.NaN));

        public double WindowHeight
        {
            get
            {
                return (double)GetValue(WindowHeightProperty);
            }
            set
            {
                SetValue(WindowHeightProperty, value);
            }
        }

        public static readonly DependencyProperty WindowWidthProperty = DependencyProperty.Register("WindowWidth", typeof(double), typeof(Only_ChildWindow),
            new PropertyMetadata(double.NaN));

        public double WindowWidth
        {
            get
            {
                return (double)GetValue(WindowWidthProperty);
            }
            set
            {
                SetValue(WindowWidthProperty, value);
            }
        }

        public static readonly DependencyProperty ShowOkButtonProperty = DependencyProperty.Register("ShowOkButton", typeof(Visibility), typeof(Only_ChildWindow),
            new PropertyMetadata(Visibility.Visible));
        public Visibility ShowOkButton
        {
            get
            {
                return (Visibility)GetValue(ShowOkButtonProperty);
            }
            set
            {
                SetValue(ShowOkButtonProperty, value);
            }
        }

        public static readonly DependencyProperty ShowCancelButtonProperty = DependencyProperty.Register("ShowCancelButton", typeof(Visibility), typeof(Only_ChildWindow),
            new PropertyMetadata(Visibility.Visible));
        public Visibility ShowCancelButton
        {
            get
            {
                return (Visibility)GetValue(ShowCancelButtonProperty);
            }
            set
            {
                SetValue(ShowCancelButtonProperty, value);
            }
        }

        public bool DialogResult
        { get; set; }

        public static readonly DependencyProperty ShownProperty = DependencyProperty.Register("Shown", typeof(bool), typeof(Only_ChildWindow),
            new PropertyMetadata(false, new PropertyChangedCallback((sender, arg) =>
            {
                (sender as Only_ChildWindow).SetChildWindowState();
            })));

        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(Only_ChildWindow),
            new PropertyMetadata(default(ICommand)));
        public ICommand CloseCommand
        {
            get
            {
                return (ICommand)GetValue(CloseCommandProperty);
            }
            set
            {
                SetValue(CloseCommandProperty, value);
            }
        }

        public static readonly DependencyProperty OKCommandProperty = DependencyProperty.Register("OKCommand", typeof(ICommand), typeof(Only_ChildWindow),
            new PropertyMetadata(default(ICommand)));

        public ICommand OKCommand
        {
            get
            {
                return (ICommand)GetValue(OKCommandProperty);
            }
            set
            {
                SetValue(OKCommandProperty, value);
            }
        }

        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(Only_ChildWindow),
            new PropertyMetadata(default(ICommand)));
        public ICommand CancelCommand
        {
            get
            {
                return (ICommand)GetValue(CancelCommandProperty);
            }
            set
            {
                SetValue(CancelCommandProperty, value);
            }
        }

        public static readonly DependencyProperty WindowParameterProperty = DependencyProperty.Register("WindowParameter", typeof(object),
            typeof(Only_ChildWindow), new PropertyMetadata(default(object)));
        public object WindowParameter
        {
            get
            {
                return GetValue(WindowParameterProperty);
            }
            set
            {
                SetValue(WindowParameterProperty, value);
            }
        }

        public bool Shown
        {
            get
            {
                return (bool)GetValue(ShownProperty);
            }
            set
            {
                SetValue(ShownProperty, value);
            }
        }

        private void SetChildWindowState()
        {
            if (Shown)
                Show();
            else
                Close();

        }

        private void Show()
        {
            if (Storyboard_Open != null)
                Storyboard_Open.Begin();
        }

        private void Close()
        {
            this.Storyboard_Close.Begin();
            this.DialogResult = false;
        }
    }
}
