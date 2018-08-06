using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Talent.ClientCommonLib.Controls
{
    [StyleTypedProperty(Property = "WatermarkStyle", StyleTargetType = typeof(TextBlock))]
    public class WatermarkTextBox : TextBox
    {
        public WatermarkTextBox()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public Style WatermarkStyle
        {
            get { return (Style)GetValue(WatermarkStyleProperty); }
            set { SetValue(WatermarkStyleProperty, value); }
        }

        public static Style GetWatermarkStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(WatermarkStyleProperty);
        }

        public static void SetWatermarkStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(WatermarkStyleProperty, value);
        }

        public static readonly DependencyProperty WatermarkStyleProperty =
        DependencyProperty.RegisterAttached("WatermarkStyle", typeof(Style), typeof(WatermarkTextBox));

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        public static readonly DependencyProperty WatermarkProperty =
        DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkTextBox),
        new FrameworkPropertyMetadata(OnWatermarkChanged));

        private static void OnWatermarkChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pwdBox = sender as PasswordBox;

            if (pwdBox == null)
            {
                return;
            }

            pwdBox.PasswordChanged -= OnPasswordChanged;
            pwdBox.PasswordChanged += OnPasswordChanged;
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pwdBox = sender as PasswordBox;
            TextBlock watermarkTextBlock = pwdBox.Template.FindName("WatermarkTextBlock", pwdBox) as TextBlock;

            if (watermarkTextBlock != null)
            {
                watermarkTextBlock.Visibility = pwdBox.SecurePassword.Length == 0
         ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }

    public class NullOrEmptyStringToVisibilityConverter : IValueConverter
    {
        public NullOrEmptyStringToVisibilityConverter()
        {
            NullOrEmpty = Visibility.Collapsed;
            NotNullOrEmpty = Visibility.Visible;
        }

        public Visibility NullOrEmpty { get; set; }
        public Visibility NotNullOrEmpty { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value == null ? string.Empty : value.ToString();
            return string.IsNullOrEmpty(strValue) ? NullOrEmpty : NotNullOrEmpty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
