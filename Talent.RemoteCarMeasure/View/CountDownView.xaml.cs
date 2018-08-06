using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// Interaction logic for CountDownView.xaml
    /// </summary>
    public partial class CountDownView : Window
    {
        public CountDownView()
        {
            InitializeComponent();
            this.Loaded += CountDownView_Loaded;
        }
        private System.Windows.Forms.Timer confirmTimer;
        private int i = 3;
        void CountDownView_Loaded(object sender, RoutedEventArgs e)
        {
            confirmTimer = new System.Windows.Forms.Timer();
            confirmTimer.Tick += confirmTimer_Tick;
            confirmTimer.Interval = 1000;
            confirmTimer.Start();
        }

        void confirmTimer_Tick(object sender, EventArgs e)
        {
            this.tbCountDown.Text = i + "";
            i--;
            if (i == 0)
            {
                confirmTimer.Stop();
                this.Close();
            }
        }
    }
}
