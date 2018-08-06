using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// SystemConfig.xaml 的交互逻辑
    /// </summary>
    public partial class SystemConfig : Window
    {
        private configlist curConfig;
        private string path = AppDomain.CurrentDomain.BaseDirectory + "\\ClientConfig";
        public SystemConfig()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载时触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetAllConfigs();
        }

        /// <summary>
        /// 获取所有xml文件列表
        /// </summary>
        private void GetAllConfigs()
        {
            var list = new List<string>();
            if (Directory.Exists(path))//判断要获取的目录文件是否存在。
            {
                var directory = new DirectoryInfo(path);
                FileInfo[] collection = directory.GetFiles("*.xml");//指定类型
                foreach (FileInfo item in collection)
                {
                    string fullname = item.Name.ToString();
                    string filename = fullname.Substring(0, fullname.LastIndexOf("."));//去掉后缀名。
                    TreeViewItem config = new TreeViewItem() { Header = filename };
                    this.configFiles.Items.Add(config);
                }
            }
            else
            {
                MessageBox.Show("文件夹不存在！");
            }
        }

        /// <summary>
        /// 某配置文件选中后触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void configFiles_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.configFiles.SelectedItem != null)
            {
                string fileName = ((TreeViewItem)this.configFiles.SelectedItem).Header + ".xml";
                XmlSerializer configSer = new XmlSerializer(typeof(configlist));
                string configPath = path + fileName;
                StreamReader sr = new StreamReader(File.OpenRead(configPath));
                curConfig = (configlist)configSer.Deserialize(sr);
                this.configNodeNames.Items.Clear();
                foreach (var item in curConfig.Modules)
                {
                    TreeViewItem rootItem = new TreeViewItem() { Header = item.Name };
                    foreach (submodule subItem in item.SubModules)
                    {
                        TreeViewItem childItem = new TreeViewItem() { Header = subItem.Name };
                        rootItem.Items.Add(childItem);
                    }
                    this.configNodeNames.Items.Add(rootItem);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void configNodeNames_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height =new GridLength(30,GridUnitType.Pixel)});
            Label lable1 = new Label() { Name = "lable1", Content="设备检测：" };
            lable1.Width = 120;
            lable1.Height = 25;
            lable1.HorizontalAlignment = HorizontalAlignment.Right;
            lable1.VerticalAlignment = VerticalAlignment.Stretch;
            lable1.HorizontalContentAlignment = HorizontalAlignment.Right;
            MainGrid.Children.Add(lable1);
            lable1.SetValue(Grid.RowProperty, 0);
            lable1.SetValue(Grid.ColumnProperty, 0);

            TextBox tb = new TextBox() { Name = "UsePassCarType", Text = "IO控制板" };
            tb.Width = 250;
            tb.Height = 25;
            tb.TextWrapping = TextWrapping.NoWrap;
            tb.VerticalAlignment = VerticalAlignment.Stretch;
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            MainGrid.Children.Add(tb);
            tb.SetValue(Grid.RowProperty, 0);
            tb.SetValue(Grid.ColumnProperty, 1);


            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Pixel) });

            Label lable2 = new Label() { Name = "lable2", Content = "连接方式：" };
            lable2.Width = 120;
            lable2.Height = 25;
            lable2.HorizontalAlignment = HorizontalAlignment.Right;
            lable2.VerticalAlignment = VerticalAlignment.Stretch;
            lable2.HorizontalContentAlignment = HorizontalAlignment.Right;
            
            lable1.SetValue(Grid.RowProperty, 1);
            lable1.SetValue(Grid.ColumnProperty, 0);
            MainGrid.Children.Add(lable2);

            TextBox tb1 = new TextBox() { Name = "ConType", Text = "网口" };
            tb1.Width = 250;
            tb1.Height = 25;
            tb1.TextWrapping = TextWrapping.NoWrap;
            tb1.VerticalAlignment = VerticalAlignment.Stretch;
            tb1.HorizontalAlignment = HorizontalAlignment.Left;
            
            tb1.SetValue(Grid.RowProperty, 1);
            tb1.SetValue(Grid.ColumnProperty, 1);
            MainGrid.Children.Add(tb1);
        }
    }
}
