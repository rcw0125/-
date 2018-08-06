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

namespace Talent.CarMeasureClient.MyUC
{
    /// <summary>
    /// Countdown.xaml 的交互逻辑
    /// </summary>
    public partial class Countdown : UserControl
    {
        private Grid grid = null;
        private Window wm;
        public Countdown(Grid _grid, Window _wm)
        {
            InitializeComponent();
            grid = _grid;
            wm = _wm;
            left = wm.Left;
            top = wm.Top;
        }

        /// <summary>
        /// 创建动画执行对象
        /// </summary>
        private Storyboard sb = null;
        /// <summary>
        /// 给文本框赋值
        /// </summary>
        private string txtValue = string.Empty;
        public string TxtValue
        {
            get { return txtValue; }
            set { txtValue = value; this.tb.Text = txtValue; }
        }
        //记录初始位置
        private double left = 0;
        private double top = 0;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //创建动画对象实例
            sb = new Storyboard();
            //ScaleX缩放动画
            DoubleAnimation daX = new DoubleAnimation();
            daX.Duration = TimeSpan.FromSeconds(0.6);
            daX.From = 20;
            daX.To = 1;
            Storyboard.SetTarget(daX, this);
            Storyboard.SetTargetProperty(daX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            //ScaleY缩放动画
            DoubleAnimation daY = new DoubleAnimation();
            daY.Duration = TimeSpan.FromSeconds(0.6);
            daY.From = 20;
            daY.To = 1;
            Storyboard.SetTarget(daY, this);
            Storyboard.SetTargetProperty(daY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
            //Opacity变换动画
            DoubleAnimation daO = new DoubleAnimation();
            daO.Duration = TimeSpan.FromSeconds(0.6);
            daO.From = 0;
            daO.To = 1;
            Storyboard.SetTarget(daO, this);
            Storyboard.SetTargetProperty(daO, new PropertyPath("(Opacity)"));
            sb.Children.Add(daX);
            sb.Children.Add(daY);
            sb.Children.Add(daO);
            //抖动效果
            DoubleAnimation daLeft1 = new DoubleAnimation();
            daLeft1.BeginTime = TimeSpan.FromSeconds(0.6);
            daLeft1.Duration = TimeSpan.FromSeconds(0.2);
            daLeft1.From = wm.Left;
            daLeft1.To = wm.Left - 10;
            daLeft1.EasingFunction = new BounceEase() { Bounces = 10, EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(daLeft1, wm);
            Storyboard.SetTargetProperty(daLeft1, new PropertyPath("(Left)"));
            DoubleAnimation daLeft2 = new DoubleAnimation();
            daLeft2.BeginTime = TimeSpan.FromSeconds(0.7);
            daLeft2.Duration = TimeSpan.FromSeconds(0.2);
            daLeft2.From = wm.Left;
            daLeft2.To = wm.Left + 10;
            daLeft2.EasingFunction = new BounceEase() { Bounces = 10, EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(daLeft2, wm);
            Storyboard.SetTargetProperty(daLeft2, new PropertyPath("(Left)"));


            DoubleAnimation daTop1 = new DoubleAnimation();
            daTop1.BeginTime = TimeSpan.FromSeconds(0.6);
            daTop1.Duration = TimeSpan.FromSeconds(0.2);
            daTop1.From = wm.Top;
            daTop1.To = wm.Top + 10; ;
            daTop1.EasingFunction = new BounceEase() { Bounces = 10, EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(daTop1, wm);
            Storyboard.SetTargetProperty(daTop1, new PropertyPath("(Top)"));

            DoubleAnimation daTop2 = new DoubleAnimation();
            daTop2.BeginTime = TimeSpan.FromSeconds(0.7);
            daTop2.Duration = TimeSpan.FromSeconds(0.2);
            daTop2.From = wm.Top;
            daTop2.To = wm.Top - 10;
            daTop2.EasingFunction = new BounceEase() { Bounces = 10, EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(daTop2, wm);
            Storyboard.SetTargetProperty(daTop2, new PropertyPath("(Top)"));
            sb.Children.Add(daLeft1);
            sb.Children.Add(daLeft2);
            sb.Children.Add(daTop1);
            sb.Children.Add(daTop2);
            sb.Completed += new EventHandler(sb_Completed);
            sb.Begin(this, true);
        }

        private void sb_Completed(object sender, EventArgs e)
        {
            //解除绑定 
            sb.Remove(this);
            sb.Children.Clear();
            grid.Children.Clear();
            //窗体恢复初始位置
            wm.Left = left;
            wm.Top = top;
        }
    }
}
