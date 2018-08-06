using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
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
using System.Xml.Serialization;
using Talent.Measure.DomainModel.PrintModel;

namespace Talent.CarMeasureClient
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string logConfigPath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\通用票据净重模版.xml"; 
             XmlSerializer configSer = new XmlSerializer(typeof(Bill));
             StreamReader sr = new StreamReader(File.OpenRead(logConfigPath));
             Bill bill =(Bill)configSer.Deserialize(sr);

            Dictionary<string,string> data=new Dictionary<string,string>();
            data.Add("Title", "计量单");
            data.Add("operaname", "物资调拨");
            data.Add("matchid", "80111111");
            data.Add("carno", "陕A12345");
            data.Add("planid", "1111111");

            data.Add("taskcode", "222222");
            data.Add("materialName", "304不锈钢");
            data.Add("ship", "");
            data.Add("sourcename", "虎豹钢厂");
            data.Add("targetname", "853库");

            data.Add("grossweigh", "1号衡器");
            data.Add("grosstime", "051010170800");
            data.Add("gross", "100吨");

            data.Add("tareweigh", "2号衡器");
            data.Add("taretime", "051010170800");
            data.Add("tare", "10吨");

            data.Add("deduction", "0.1吨");

            data.Add("suttleweigh", "3号衡器");
            data.Add("suttletime", "051010170800");
            data.Add("suttle", "7吨");

            data.Add("usermemo", "兆工");
            data.Add("printtime", "051010170800");
            data.Add("printweigh", "7吨");
            data.Add("printset", "净重");
            data.Add("printcount", "1");

             PrintDialog dialog = new PrintDialog();
             var pageMediaSize = LocalPrintServer.GetDefaultPrintQueue()

                              .GetPrintCapabilities()

                              .PageMediaSizeCapability

                              .FirstOrDefault(x => x.PageMediaSizeName == PageMediaSizeName.ISOA3);


             //if (pageMediaSize != null)
             //{
             //    pageSize = new Size((double)pageMediaSize.Width, (double)pageMediaSize.Height);
             //}
             DataPaginator dp = new DataPaginator(bill, new Typeface("SimSun"), 12, 96 * 0.35, new Size((double)pageMediaSize.Width, (double)pageMediaSize.Height), data);

             if (dialog.ShowDialog() == true)
             {
                 dialog.PrintDocument(dp, "Test Page");
             }
             string ss = "";
        }
    }
}
