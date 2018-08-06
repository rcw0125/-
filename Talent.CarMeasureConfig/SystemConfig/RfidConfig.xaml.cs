using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Talent.Io.Controller;
using Talent.Measure.DomainModel.CommonModel;
using Talent.Rfid.Controller;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;

namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// RfidConfig.xaml 的交互逻辑
    /// </summary>
    public partial class RfidConfig : UserControl
    {
        private RfidController rfidMain;
        public string curConfigFileName { get; set; }
        private configlist curConfig;
        /// <summary>
        /// 当前操作的子模块对象
        /// </summary>
        private submodule curSubModule;
        private string path = AppDomain.CurrentDomain.BaseDirectory + "\\ClientConfig";
        /// <summary>
        /// 选择的检测设备对应的驱动名称
        /// </summary>
        private string selectedDetectEquDriverName = string.Empty;
        private List<string> equNameList = new List<string>();

        public RfidConfig(configlist curConfig, submodule curSubModule, string configFileName)
        {
            InitializeComponent();
            this.curConfig = curConfig;
            this.curSubModule = curSubModule;
            this.curConfigFileName = configFileName;
        }

        /// <summary>
        /// 窗体加载时触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Window_Loaded;
            SetFormControls();
        }

        /// <summary>
        /// 设置IO控制窗体中的各控件值
        /// </summary>
        private void SetFormControls()
        {
            if (curSubModule != null)
            {
                SetDetectEqu();
                SetLinkType();
                SetComport();
                SetBaudrate();
                var ipList = (from r in curSubModule.Params where r.Name == RfidConfigParam.Ip select r).ToList();
                if (ipList.Count > 0)
                {
                    //this.Ip.MaxLength = !string.IsNullOrEmpty(ipList.First().Size) ? Int32.Parse(ipList.First().Size) : IoConfigParam.TextBox_MaxLenght;
                    this.Ip.Text = ipList.First().Value;
                }
                var portList = (from r in curSubModule.Params where r.Name == RfidConfigParam.Port select r).ToList();
                if (portList.Count > 0)
                {
                    this.Port.MaxLength = !string.IsNullOrEmpty(portList.First().Size) ? Int32.Parse(portList.First().Size) : IoConfigParam.TextBox_MaxLenght;
                    this.Port.Text = portList.First().Value;
                }
                var Intervals = (from r in curSubModule.Params where r.Name == RfidConfigParam.Interval select r).ToList();//IntervalTextBox
                if (Intervals.Count>0)
                {
                    this.IntervalTextBox.MaxLength = !string.IsNullOrEmpty(Intervals.First().Size) ? Int32.Parse(Intervals.First().Size) : IoConfigParam.TextBox_MaxLenght;
                    this.IntervalTextBox.Text = Intervals.First().Value;
                }
                var isUseList = (from r in curSubModule.Params where r.Name == RfidConfigParam.IsUse select r.Value).ToList();
                if (isUseList.Count>0)
                {
                    this.yesCheckBox.IsChecked = isUseList.First().Equals("是") ? true : false;
                    this.noCheckBox.IsChecked = !this.yesCheckBox.IsChecked;
                }

                //功率
                var power = (from r in curSubModule.Params where r.Name == RfidConfigParam.Power select r).ToList();//IntervalTextBox
                if (Intervals.Count > 0)
                {
                    this.txtPower.MaxLength = !string.IsNullOrEmpty(power.First().Size) ? Int32.Parse(power.First().Size) : IoConfigParam.TextBox_MaxLenght;
                    this.txtPower.Text = power.First().Value;
                }
                this.MainGrid.Visibility = Visibility.Visible;
                SetGridView();
            }
            else
            {
                this.Ip.Text = string.Empty;
                this.Port.Text = string.Empty;
                this.EquDll.Text = string.Empty;
                this.UsePassCarType.SelectedIndex = 0;
                this.ConType.SelectedIndex = 0;
                this.Comport.SelectedIndex = 0;
                this.Baudrate.SelectedIndex = 0;
                this.equConfigDataGrid.ItemsSource = null;
            }
        }

        /// <summary>
        /// 设置gridview
        /// </summary>
        private void SetGridView()
        {
            IList<EquConfigModel> models = new List<EquConfigModel>();
            if (curSubModule.GridRow != null)
            {
                foreach (var row in curSubModule.GridRow.RowList)
                {
                    EquConfigModel model = new EquConfigModel();
                    foreach (var item in row.Params)
                    {
                        do
                        {
                            if (RfidConfigParam.Row_AntennaName.Equals(item.Name))
                            {
                                model.EquName = item.Value;
                                break;
                            }
                            if (RfidConfigParam.Row_Port.Equals(item.Name))
                            {
                                model.Port = item.Value;
                                break;
                            }
                            if (RfidConfigParam.Row_IsUse.Equals(item.Name))
                            {
                                model.IsUse = item.Value.Equals("1") ? true : false;
                                break;
                            }
                        } while (false);
                    }
                    models.Add(model);
                }
            }
            this.equConfigDataGrid.ItemsSource = models;
        }

        /// <summary>
        /// 设置检测设备
        /// </summary>
        private void SetDetectEqu()
        {
            var detectEquList = (from r in curSubModule.Params where r.Name == RfidConfigParam.DetectEqu select r).ToList();
            if (detectEquList.Count > 0)
            {
                foreach (var item in detectEquList.First().List)
                {
                    equNameList.Add(item);
                    ComboBoxItem item1 = new ComboBoxItem() { Content = item.Split('@')[0] };
                    this.UsePassCarType.Items.Add(item1);
                }
                for (int i = 0; i < detectEquList.First().List.Count; i++)
                {
                    if (detectEquList.First().List[i].Contains(detectEquList.First().Value))
                    {
                        this.UsePassCarType.SelectedItem = this.UsePassCarType.Items[i];
                        this.selectedDetectEquDriverName = detectEquList.First().List[i].Split('@')[1];
                        break;
                    }
                }
            }
            else
            {
                this.UsePassCarType.Items.Clear();
            }
        }

        /// <summary>
        /// 设置连接方式
        /// </summary>
        private void SetLinkType()
        {
            SetComboxDs(this.ConType, RfidConfigParam.LinkType);
        }

        /// <summary>
        /// 设置串口
        /// </summary>
        private void SetComport()
        {
            SetComboxDs(this.Comport, RfidConfigParam.Comport);
        }

        /// <summary>
        /// 设置波特率
        /// </summary>
        private void SetBaudrate()
        {
            SetComboxDs(this.Baudrate, RfidConfigParam.Baudrate);
        }

        /// <summary>
        /// 设置comboBox数据源及选中的值
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="xmlNodeName"></param>
        private void SetComboxDs(ComboBox cb, string xmlNodeName)
        {
            cb.Items.Clear();
            var list = (from r in curSubModule.Params where r.Name == xmlNodeName select r).ToList();
            if (list.Count > 0)
            {
                foreach (var item in list.First().List)
                {
                    ComboBoxItem item1 = new ComboBoxItem() { Content = item };
                    cb.Items.Add(item1);
                }
                for (int i = 0; i < list.First().List.Count; i++)
                {
                    if (list.First().List[i].Contains(list.First().Value))
                    {
                        cb.SelectedItem = cb.Items[i];
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 检测设备选择改变触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsePassCarType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.UsePassCarType.SelectedItem != null)
            {
                var equName = (this.UsePassCarType.SelectedItem as ComboBoxItem).Content.ToString();
                var drivers = (from r in equNameList where r.Contains(equName) select r.Split('@')[1]).ToList();
                this.EquDll.Text = drivers.First();
            }
            else
            {
                this.EquDll.Text = string.Empty;
            }
        }

        /// <summary>
        /// 打开读写器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (rfidMain != null)
            {
                rfidMain.Stop();
            }         
            try
            {
                rfidMain = new RfidController(System.IO.Path.Combine(path, curConfigFileName));
                rfidMain.OnReceivedData +=ReceivedData;
                if (!rfidMain.Start())
                {
                    MessageBox.Show("打开Rfid读卡器失败!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        /// <summary>
        /// 读取标签数据
        /// </summary>
        /// <param name="pIds"></param>
        private void ReceivedData(List<string> pIds)
        {
            this.cardIdTextBox.Dispatcher.Invoke(new Action(() =>
            {
                if (rfidMain.CardId != null && rfidMain.CardId.Count > 0)
                {
                    this.cardIdTextBox.Text = rfidMain.CardId[0];
                }
                else
                {
                    this.cardIdTextBox.Text = "";
                }

            }));
        }
        /// <summary>
        /// 停止读卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (rfidMain != null)
            {
                rfidMain.Close();
            }
        }

        /// <summary>
        /// 报警
        /// </summary>
        /// <param name="pPort"></param>
        /// <param name="pValue"></param>
        /// <returns></returns>
        void nvr_OnReceiveAlarmSignal(string pPort, string pValue)
        {
            MessageBox.Show(string.Format("端口{0}收到报警信号。", pPort));
        }

        void nvr_ShowErrMsg(string pMsg)
        {
            MessageBox.Show(pMsg);
        }

        /// <summary>
        /// 新增按钮点击事件
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //获取配置
            IList<EquConfigModel> models = this.equConfigDataGrid.ItemsSource as IList<EquConfigModel>;
            EquConfigModel model = new EquConfigModel();
            models.Add(model);
            this.equConfigDataGrid.ItemsSource = models;
            this.equConfigDataGrid.Items.Refresh();
        }

        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        private void Detete_Click(object sender, RoutedEventArgs e)
        {
            if (this.equConfigDataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择一条天线配置信息");
            }
            else
            {
                List<EquConfigModel> list = this.equConfigDataGrid.ItemsSource as List<EquConfigModel>;
                EquConfigModel removeModel = this.equConfigDataGrid.SelectedItem as EquConfigModel;
                list.Remove(removeModel);
                this.equConfigDataGrid.ItemsSource = list;
                this.equConfigDataGrid.Items.Refresh();
            }
        }

        /// <summary>
        /// 是否在用"是"选中事件
        /// </summary>
        private void yesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.yesCheckBox.IsChecked == true)
            {
                if (this.noCheckBox == null)
                {
                    return;
                }
                this.noCheckBox.IsChecked = false;
            }
            else
            {
                this.noCheckBox.IsChecked = true;
            }
        }
        /// <summary>
        /// 是否在用"否"选中事件
        /// </summary>
        private void noCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.noCheckBox.IsChecked == true)
            {
                this.yesCheckBox.IsChecked = false;
            }
            else
            {
                this.yesCheckBox.IsChecked = true;
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (curSubModule == null)
            {
                return;
            }
            //获取检测设备
            var detectEquList = (from r in curSubModule.Params where r.Name == RfidConfigParam.DetectEqu select r).ToList();
            if (detectEquList.Count > 0)
            {
                var detectEqu = this.UsePassCarType.SelectedItem as ComboBoxItem;
                detectEquList.First().Value = detectEqu.Content.ToString();
            }
            //获取连接方式
            var list = (from r in curSubModule.Params where r.Name == RfidConfigParam.LinkType select r).ToList();
            if (list.Count > 0)
            {
                var detectEqu = this.ConType.SelectedItem as ComboBoxItem;
                list.First().Value = detectEqu.Content.ToString();
            }
            //获取串口
            var comportList = (from r in curSubModule.Params where r.Name == RfidConfigParam.Comport select r).ToList();
            if (comportList.Count > 0)
            {
                var port = this.Comport.SelectedItem as ComboBoxItem;
                comportList.First().Value = port.Content.ToString();
            }
            //获取IP
            var ipList = (from r in curSubModule.Params where r.Name == RfidConfigParam.Ip select r).ToList();
            if (ipList.Count > 0)
            {
                ipList.First().Value = this.Ip.Text.Trim();
            }
            //获取端口
            var portList = (from r in curSubModule.Params where r.Name == RfidConfigParam.Port select r).ToList();
            if (portList.Count > 0)
            {
                if (!CommonMethod.CommonMethod.IsDataTransformSuccess(portList.First().Type, this.Port.Text.Trim()))
                {
                    MessageBox.Show(string.Format(CommonParam.Info_Input_Msg_Exption, CommonParam.Port_TextBox_Name));
                    this.Port.Focus();
                    return;
                }
                portList.First().Value = this.Port.Text.Trim();
            }
            //获取波特率
            var baudrateList = (from r in curSubModule.Params where r.Name == RfidConfigParam.Baudrate select r).ToList();
            if (baudrateList.Count > 0)
            {
                var baudrate = this.Baudrate.SelectedItem as ComboBoxItem;
                baudrateList.First().Value = baudrate.Content.ToString();
            }
            //获取寻卡时间
            var IntervalList = (from r in curSubModule.Params where r.Name == RfidConfigParam.Interval select r).ToList();
            if (IntervalList.Count > 0)
            {
                if (!CommonMethod.CommonMethod.IsDataTransformSuccess(IntervalList.First().Type, this.IntervalTextBox.Text.Trim()))
                {
                    MessageBox.Show(string.Format(CommonParam.Info_Input_Msg_Exption, IntervalList.First().Lab));
                    this.IntervalTextBox.Focus();
                    return;
                }
                IntervalList.First().Value = this.IntervalTextBox.Text.Trim();
            }
            //获取是否在用
            var isUseList = (from r in curSubModule.Params where r.Name == RfidConfigParam.IsUse select r).ToList();
            if (isUseList.Count > 0)
            {
                isUseList.First().Value = (bool)this.yesCheckBox.IsChecked ? "是" : "否";
            }
            //获取设备驱动名称
            var dllList = (from r in curSubModule.Params where r.Name == RfidConfigParam.EquDriverName select r).ToList();
            if (dllList.Count > 0)
            {
                dllList.First().Value = this.EquDll.Text.Trim();
            }
            //获取设备配置
            IList<EquConfigModel> models = this.equConfigDataGrid.ItemsSource as IList<EquConfigModel>;
            curSubModule.GridRow.RowList.Clear();
            foreach (var item in models)
            {
                if (!CommonMethod.CommonMethod.IsDataTransformSuccess("int", item.Port))
                {
                    MessageBox.Show("第" + (models.IndexOf(item) + 1) + "个天线配置信息中" + string.Format(CommonParam.Info_Input_Msg_Exption, CommonParam.Port_TextBox_Name));
                    this.equConfigDataGrid.SelectedIndex = models.IndexOf(item);
                    return;
                }

                Row row = new Row() { Params = new List<Param>() };
                Param equName = new Param()
                {
                    Name = RfidConfigParam.Row_AntennaName,
                    Value = item.EquName
                };
                Param Port = new Param()
                {
                    Name = RfidConfigParam.Row_Port,
                    Value = item.Port
                };
                Param IsUse = new Param()
                {
                    Name = RfidConfigParam.Row_IsUse,
                    Value = item.IsUse ? "1" : "0"
                };
                row.Params.Add(equName);
                row.Params.Add(IsUse);
                row.Params.Add(Port);
                curSubModule.GridRow.RowList.Add(row);
            }
            if (XmlHelper.WriteXmlFile<configlist>(curConfigFileName, curConfig))
            {
                MessageBox.Show("保存成功");
               // ConfigReader.ReLoadConfig();
            }
        }
    }
}
