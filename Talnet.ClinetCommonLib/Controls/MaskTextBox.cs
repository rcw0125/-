using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Talent.ClientCommonLib.Controls
{
    public class MaskTextBox : TextBox
    {
        #region MaskText
        public static readonly DependencyProperty MaskTextProperty =
                   DependencyProperty.Register("MaskText", typeof(string), typeof(MaskTextBox));

        public string MaskText
        {
            get { return (string)GetValue(MaskTextProperty); }
            set { SetValue(MaskTextProperty, value); }
        }
        #endregion

        public MaskTextBox()
        {
            Loaded += (sender, args) =>
            {
                if (string.IsNullOrEmpty(base.Text))
                {
                    base.Text = MaskText;
                    base.Foreground = Brushes.Gray;
                }
            };

            base.GotFocus += (sender, args) =>
            {
                base.Foreground = Brushes.Black;
                if (base.Text == MaskText)
                    base.Text = string.Empty;
            };
            base.LostFocus += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(base.Text))
                    return;

                base.Text = MaskText;
                base.Foreground = Brushes.Gray;
            };
        }

        public new string Text
        {
            get
            {
                if (base.Text == MaskText)
                    return string.Empty;
                else
                    return base.Text;
            }
            set { base.Text = value; }
        }
    }
}
