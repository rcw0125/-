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

namespace Talent.RemoteCarMeasure.Commom.Control
{
    /// <summary>
    /// ChangeSkinColor.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeSkinColor : Window
    {
        /// <summary>
        /// 返回颜色值
        /// </summary>
        public string rtColor = string.Empty;
        public ChangeSkinColor()
        {
            InitializeComponent();
            this.Background = new SolidColorBrush(Color.FromArgb((byte)a.Value, (byte)r.Value, (byte)g.Value, (byte)b.Value));

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        /// <summary>
        /// 更改颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            rtColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb((int)r.Value, (int)g.Value, (int)b.Value));
            this.Close();
        }
        /// <summary>
        /// 设置初始化值
        /// </summary>
        /// <param name="inStrColor"></param>
        public void SetValue(string inStrColor)
        {
            if (inStrColor.Length > "#528DAD".Length)//处理 #FF528DAD 带透明度的数据2016-3-8 10:32:34……
            {
                inStrColor ="#"+ inStrColor.Substring(3);
            }
            r.Value = System.Int32.Parse(inStrColor.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            g.Value = System.Int32.Parse(inStrColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            b.Value = System.Int32.Parse(inStrColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private void a_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        private void r_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeBackColor();
        }
        private void g_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeBackColor();
        }
        private void b_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeBackColor();
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 更改背景颜色
        /// </summary>
        private void ChangeBackColor()
        {
            try
            {
                this.Background = new SolidColorBrush(Color.FromArgb((byte)a.Value, (byte)r.Value, (byte)g.Value, (byte)b.Value));
            }
            catch //(Exception ex)
            {

            }

        }

    }
}
