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

namespace Talent.ClientCommonLib.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Talent.ClientCommonLib"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Talent.ClientCommonLib;assembly=Talent.ClientCommonLib"
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
    ///     <MyNamespace:WeighterPanel/>
    ///
    /// </summary>
    public class WeighterPanel : Control
    {
        static WeighterPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WeighterPanel), new FrameworkPropertyMetadata(typeof(WeighterPanel)));
        }

        public Brush LeftLight
        {
            get { return (Brush)GetValue(LeftLightProperty); }
            set { SetValue(LeftLightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RedLight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftLightProperty =
            DependencyProperty.Register("LeftLight", typeof(Brush), typeof(WeighterPanel), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush RightLight
        {
            get { return (Brush)GetValue(RightLightProperty); }
            set { SetValue(RightLightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GreenLight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightLightProperty =
            DependencyProperty.Register("RightLight", typeof(Brush), typeof(WeighterPanel), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public string WeighterName
        {
            get { return (string)GetValue(WeighterNameProperty); }
            set { SetValue(WeighterNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeighterName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeighterNameProperty =
            DependencyProperty.Register("WeighterName", typeof(string), typeof(WeighterPanel), new PropertyMetadata("衡器名称"));

        public decimal Weight
        {
            get { return (decimal)GetValue(WeightProperty); }
            set { SetValue(WeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Weight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeightProperty =
            DependencyProperty.Register("Weight", typeof(decimal), typeof(WeighterPanel), new PropertyMetadata(0.0m));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(WeighterPanel), new PropertyMetadata(null));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(WeighterPanel), new PropertyMetadata(string.Empty));

        public Brush MessageForeground
        {
            get { return (Brush)GetValue(MessageForegroundProperty); }
            set { SetValue(MessageForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageForegroundProperty =
            DependencyProperty.Register("MessageForeground", typeof(Brush), typeof(WeighterPanel), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public LeftLightStates LeftLightState
        {
            get { return (LeftLightStates)GetValue(LeftLightStateProperty); }
            set { SetValue(LeftLightStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LightState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftLightStateProperty =
            DependencyProperty.Register("LeftLightState", typeof(LeftLightStates), typeof(WeighterPanel), new PropertyMetadata(default(LeftLightStates), OnLeftLightStateChanged));

        public RightLightStates RightLightState
        {
            get { return (RightLightStates)GetValue(RightLightStateProperty); }
            set { SetValue(RightLightStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LightState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightLightStateProperty =
            DependencyProperty.Register("RightLightState", typeof(RightLightStates), typeof(WeighterPanel), new PropertyMetadata(default(RightLightStates), OnRightLightStateChanged));

        public WeighterStates WeighterState
        {
            get { return (WeighterStates)GetValue(WeighterStateProperty); }
            set { SetValue(WeighterStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeighterState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeighterStateProperty =
            DependencyProperty.Register("WeighterState", typeof(WeighterStates), typeof(WeighterPanel), new PropertyMetadata(default(WeighterStates), OnWeighterStateChanged));

        /// <summary>
        /// 表头信息(暂时用于提示称点超重)
        /// </summary>
        public string WeightMsg
        {
            get { return (string)GetValue(WeightMsgProperty); }
            set { SetValue(WeightMsgProperty, value); }
        }

        public static readonly DependencyProperty WeightMsgProperty =
            DependencyProperty.Register("WeightMsg", typeof(string), typeof(WeighterPanel), new PropertyMetadata(""));
        
        public Visibility LeftLine
        {
            get { return (Visibility)GetValue(LeftLineProperty); }
            set { SetValue(LeftLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftLineProperty =
            DependencyProperty.Register("LeftLine", typeof(Visibility), typeof(WeighterPanel), new PropertyMetadata(Visibility.Hidden));

        public Visibility RightLine
        {
            get { return (Visibility)GetValue(RightLineProperty); }
            set { SetValue(RightLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightLineProperty =
            DependencyProperty.Register("RightLine", typeof(Visibility), typeof(WeighterPanel), new PropertyMetadata(Visibility.Hidden));

        private static void OnLeftLightStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WeighterPanel wp = d as WeighterPanel;
           string getLeftState=Convert.ToInt32(Enum.Parse(typeof(LeftLightStates),wp.LeftLightState.ToString())).ToString();
          
           wp.LeftLight =new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + Enum.Parse(typeof(LightColour), getLeftState).ToString()));
         

          // var g = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2786E4"));
            //if (wp.LeftLightState == LeftLightStates.Red)
            //{
            //    wp.RedLight = Visibility.Visible;
            //    wp.GreenLight = Visibility.Hidden;
            //}
            //else if (wp.LightState == LightStates.Green)
            //{
            //    wp.RedLight = Visibility.Hidden;
            //    wp.GreenLight = Visibility.Visible;
            //}
            //else
            //{
            //    wp.RedLight = Visibility.Hidden;
            //    wp.GreenLight = Visibility.Hidden;
            //}
        }

        private static void OnRightLightStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WeighterPanel wp = d as WeighterPanel;
           
            string getRightState = Convert.ToInt32(Enum.Parse(typeof(RightLightStates), wp.RightLightState.ToString())).ToString();
          
            wp.RightLight = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + Enum.Parse(typeof(LightColour), getRightState).ToString()));

            // var g = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2786E4"));
            //if (wp.LeftLightState == LeftLightStates.Red)
            //{
            //    wp.RedLight = Visibility.Visible;
            //    wp.GreenLight = Visibility.Hidden;
            //}
            //else if (wp.LightState == LightStates.Green)
            //{
            //    wp.RedLight = Visibility.Hidden;
            //    wp.GreenLight = Visibility.Visible;
            //}
            //else
            //{
            //    wp.RedLight = Visibility.Hidden;
            //    wp.GreenLight = Visibility.Hidden;
            //}
        }

        private static void OnWeighterStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WeighterPanel wp = d as WeighterPanel;
            if (wp.WeighterState == WeighterStates.Working)
            {
                wp.Icon = new BitmapImage(new Uri("pack://application:,,,/Talent.ClientCommonLib;Component/Images/waiting.png"));
                wp.Message = "正在计量...";
                wp.MessageForeground = new SolidColorBrush(Color.FromRgb(157, 239, 157));
            }
            else if (wp.WeighterState == WeighterStates.Free)
            {
                wp.Icon = new BitmapImage(new Uri("pack://application:,,,/Talent.ClientCommonLib;Component/Images/stop.png"));
                wp.Message = "无计量任务";
                wp.MessageForeground = new SolidColorBrush(Color.FromRgb(238, 144, 132));
            }
            else if (wp.WeighterState == WeighterStates.NoReaderCard)
            {
                wp.Icon = new BitmapImage(new Uri("pack://application:,,,/Talent.ClientCommonLib;Component/Images/waiting.png"));
                wp.Message = "车上秤未插卡";
                wp.MessageForeground = new SolidColorBrush(Color.FromRgb(238, 144, 132));
            }
            else if (wp.WeighterState == WeighterStates.Wait)
            {
                wp.Icon = new BitmapImage(new Uri("pack://application:,,,/Talent.ClientCommonLib;Component/Images/waiting.png"));
                wp.Message = "等待计量...";
                wp.MessageForeground = new SolidColorBrush(Color.FromRgb(238, 144, 132));
            }
            else if (wp.WeighterState == WeighterStates.EndTask)
            {
                wp.Icon = new BitmapImage(new Uri("pack://application:,,,/Talent.ClientCommonLib;Component/Images/stop.png"));
                wp.Message = "计量完成...";
                wp.MessageForeground = new SolidColorBrush(Color.FromRgb(238, 144, 132));
            }
            else if (wp.WeighterState == WeighterStates.StopTask)
            {
                wp.Icon = new BitmapImage(new Uri("pack://application:,,,/Talent.ClientCommonLib;Component/Images/stop.png"));
                wp.Message = "终止计量...";
                wp.MessageForeground = new SolidColorBrush(Color.FromRgb(238, 144, 132));
            }
            else
            {
                wp.Icon = null;
                wp.Message = "衡器端断开连接";
                wp.MessageForeground = new SolidColorBrush(Color.FromRgb(157, 239, 157));
            }
        }
    }

    /// <summary>
    /// 左红绿灯状态
    /// </summary>
    public enum LeftLightStates
    {
        /// <summary>
        /// 红灯
        /// </summary>
        Red = 0,
        /// <summary>
        /// 绿灯
        /// </summary>
        Green = 1,
        /// <summary>
        ///
        /// </summary>
        None = 2
    }

    /// <summary>
    /// 右红绿灯状态
    /// </summary>
    public enum RightLightStates
    {
        /// <summary>
        /// 红灯
        /// </summary>
        Red = 0,
        /// <summary>
        /// 绿灯
        /// </summary>
        Green = 1,
        /// <summary>
        ///
        /// </summary>
        None = 2
    }
    /// <summary>
    /// 红绿灯颜色
    /// </summary>
    public enum LightColour
    {
        /// <summary>
        /// 红灯
        /// </summary>
        FFEE786C = 0,
        /// <summary>
        /// 绿灯
        /// </summary>
        FF9DEF9D = 1,
        /// <summary>
        /// 灭
        /// </summary>
        FFDDDDDD = 2
    }
    /// <summary>
    /// 衡器状态
    /// </summary>
    public enum WeighterStates
    {
        /// <summary>
        /// 正在计量
        /// </summary>
        Working = 0,
        /// <summary>
        /// 无计量任务
        /// </summary>
        Free = 1,
        /// <summary>
        /// 无状态
        /// </summary>
        None = 2,
        /// <summary>
        /// 等待计量
        /// </summary>
        Wait=3,
        /// <summary>
        /// 计量完成
        /// </summary>
        EndTask = 4,
        /// <summary>
        /// 终止计量
        /// </summary>
        StopTask=5,
        /// <summary>
        /// 车上称未插卡
        /// </summary>
        NoReaderCard = 6,
    }
}
