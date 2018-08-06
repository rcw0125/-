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
using Talent.Ic.Controller;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.CommonMethod;


namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// IcConfig.xaml 的交互逻辑
    /// </summary>
    public partial class IcConfig : UserControl
    {
        IcCardsController icCardMan;
        public string curConfigFileName { get; set; }
        private configlist curConfig;

        private List<string> oldICReadTypeList;
        /// <summary>
        /// "读卡器类型"原始数据集合
        /// </summary>
        public List<string> OldICReadTypeList
        {
            get { return oldICReadTypeList; }
            set { oldICReadTypeList = value; }
        }

        #region 窗体中的下拉框数据源
        private List<ComboxModel> conTypeList;
        /// <summary>
        /// "连接方式"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ConTypeList
        {
            get { return conTypeList; }
            set { conTypeList = value; }
        }

        private List<ComboxModel> comportList;
        /// <summary>
        /// "串口"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ComportList
        {
            get { return comportList; }
            set { comportList = value; }
        }

        private List<ComboxModel> iCReadTypeList;
        /// <summary>
        /// "读卡器类型"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ICReadTypeList
        {
            get { return iCReadTypeList; }
            set { iCReadTypeList = value; }
        }

        private List<ComboxModel> iCWriteTempList;
        /// <summary>
        /// "缓存模式"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ICWriteTempList
        {
            get { return iCWriteTempList; }
            set { iCWriteTempList = value; }
        }

        private List<ComboxModel> isUseList;
        /// <summary>
        /// "是否启用"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> IsUseList
        {
            get { return isUseList; }
            set { isUseList = value; }
        }

        private List<ComboxModel> baudrateList;
        /// <summary>
        ///"波特率"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> BaudrateList
        {
            get { return baudrateList; }
            set { baudrateList = value; }
        }

        #endregion
        /// <summary>
        /// 当前操作的子模块对象
        /// </summary>
        private submodule curSubModule;
        private string path = AppDomain.CurrentDomain.BaseDirectory + "\\ClientConfig";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="curConfig">当前配置对象</param>
        /// <param name="curSubModule">当前模块对象</param>
        /// <param name="configFileName">配置文件名称</param>
        public IcConfig(configlist curConfig, submodule curSubModule, string configFileName)
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

            //读取配置
            ConfigReader cfgReader = new ConfigReader(System.IO.Path.Combine(path, curConfigFileName));
            List<ICCard> icCardList = ConfigReader.ReadIcCard();
            if (icCardList == null || icCardList.Count <= 0)
            {
                MessageBox.Show("IC卡配置信息有误，请检查。");
                return;
            }
            icCardMan = new IcCardsController(icCardList, true);
            icCardMan.OnShowErrorMsg += icCardMan_OnShowErrorMsg;
            icCardMan.OnReadCardNo += icCardMan_OnReadCardNo;
            icCardMan.OnRemoveCard += icCardMan_OnRemoveCard;
        }

        /// <summary>
        /// 设置IC控制窗体中的各控件值
        /// </summary>
        private void SetFormControls()
        {
            if (curSubModule != null)
            {
                IList<UIIcCard> models = new List<UIIcCard>();
                if (curSubModule.GridRow != null)
                {
                    foreach (var row in curSubModule.GridRow.RowList)
                    {
                        UIIcCard model = new UIIcCard();
                        foreach (var item in row.Params)
                        {
                            do
                            {
                                if (IcCardConfigParam.Row_ConType.Equals(item.Name))
                                {
                                    model.ConType = item.Value;
                                    model.ConTypeList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    if (this.ConTypeList==null||this.ConTypeList.Count==0)//设置行中"连接方式"数据源
                                    {
                                        this.ConTypeList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    }
                                    break;
                                }
                                if (IcCardConfigParam.Row_Comport.Equals(item.Name))
                                {
                                    model.ComPort = item.Value;
                                    model.ComportList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    if (this.ComportList == null || this.ComportList.Count == 0)//设置行中"串口"数据源
                                    {
                                        this.ComportList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    }
                                    break;
                                }
                                if (IcCardConfigParam.Row_Ip.Equals(item.Name))
                                {
                                    model.Ip = item.Value;
                                    break;
                                }
                                if (IcCardConfigParam.Row_Port.Equals(item.Name))
                                {
                                    model.Port = item.Value;
                                    break;
                                }
                                if (IcCardConfigParam.Row_Baudrate.Equals(item.Name))
                                {
                                    model.Baudrate = item.Value;
                                    model.BaudrateList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    if (this.BaudrateList == null || this.BaudrateList.Count == 0)//设置行中"串口"数据源
                                    {
                                        this.BaudrateList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    }
                                    break;
                                }
                                if (IcCardConfigParam.Row_Interval.Equals(item.Name))
                                {
                                    model.Interval = item.Value;
                                    break;
                                }
                                if (IcCardConfigParam.Row_ICReadType.Equals(item.Name))
                                {
                                    model.ICReadType = item.Value;
                                    model.ICReadTypeList = (from r in item.List select new ComboxModel() { Code = r.Split('@')[0], Name = r.Split('@')[0], Type = r.Split('@')[0] }).ToList();
                                    if (this.ICReadTypeList == null || this.ICReadTypeList.Count == 0)//设置行中"读卡器类型"数据源
                                    {
                                        OldICReadTypeList = item.List;
                                        this.ICReadTypeList = (from r in item.List select new ComboxModel() { Code = r.Split('@')[0], Name = r.Split('@')[0], Type = r.Split('@')[0] }).ToList();
                                    }
                                    break;
                                }
                                if (IcCardConfigParam.Row_Driver.Equals(item.Name))
                                {
                                    model.Driver = item.Value;
                                    break;
                                }
                                if (IcCardConfigParam.Row_ICWriteTemp.Equals(item.Name))
                                {
                                    model.ICWriteTemp = item.Value.Equals("是") ? true : false;
                                    model.ICWriteTempList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    if (this.ICWriteTempList == null || this.ICWriteTempList.Count == 0)//设置行中"是否缓存"数据源
                                    {
                                        this.ICWriteTempList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    }
                                    break;
                                }

                                if (IcCardConfigParam.Row_IsUse.Equals(item.Name))
                                {
                                    model.IsUse = item.Value.Equals("是") ? true : false;
                                    model.IsUseList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    if (this.IsUseList == null || this.IsUseList.Count == 0)//设置行中"是否启用"数据源
                                    {
                                        this.IsUseList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    }
                                    break;
                                }                               
                            } while (false);
                        }
                        models.Add(model);
                    }
                }
                this.ICConfigDataGrid.ItemsSource = models;
            }
            else
            {
                this.ICConfigDataGrid.ItemsSource = null;
            }
        }

        /// <summary>
        /// 新增按钮点击事件
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //获取配置
            IList<UIIcCard> models = this.ICConfigDataGrid.ItemsSource as IList<UIIcCard>;
            UIIcCard icCard = new UIIcCard()
            {
                ComportList = this.ComportList,
                ConTypeList = this.ConTypeList,
                ICReadTypeList = this.ICReadTypeList,
                ICWriteTempList = this.ICWriteTempList,
                IsUseList = this.IsUseList,
                ICWriteTemp = false,
                IsUse = true,
                BaudrateList = this.BaudrateList
            };
            icCard.ComPort = (icCard.ComportList != null && icCard.ComportList.Count > 0) ? icCard.ComportList.First().Name : string.Empty;
            icCard.ConType = (icCard.ConTypeList != null && icCard.ConTypeList.Count > 0) ? icCard.ConTypeList.First().Name : string.Empty;
            icCard.ICReadType = (icCard.ICReadTypeList != null && icCard.ICReadTypeList.Count > 0) ? icCard.ICReadTypeList.First().Name : string.Empty;
            icCard.Baudrate = (icCard.BaudrateList != null && icCard.BaudrateList.Count > 0) ? icCard.BaudrateList.First().Name : string.Empty;
            icCard.Driver = oldICReadTypeList.First().Split('@')[1];
            models.Add(icCard);
            this.ICConfigDataGrid.ItemsSource = models;
            this.ICConfigDataGrid.Items.Refresh();
        }

        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        private void Detete_Click(object sender, RoutedEventArgs e)
        {
            if (this.ICConfigDataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择一条IC配置信息");
            }
            else
            {
                List<UIIcCard> list = this.ICConfigDataGrid.ItemsSource as List<UIIcCard>;
                UIIcCard removeModel = this.ICConfigDataGrid.SelectedItem as UIIcCard;
                list.Remove(removeModel);
                this.ICConfigDataGrid.ItemsSource = list;
                this.ICConfigDataGrid.Items.Refresh();
            }
        }

        /// <summary>
        /// 修改按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            
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
            //获取配置
            IList<UIIcCard> models = this.ICConfigDataGrid.ItemsSource as IList<UIIcCard>;
            curSubModule.GridRow.RowList.Clear();
            foreach (var item in models)
            {
                Row row = new Row() { Params = new List<Param>() };
                Param conType = new Param()
                {
                    Name = IcCardConfigParam.Row_ConType,
                    Value = item.ConType,
                    List = (from r in item.ConTypeList select r.Name).ToList()
                };
                Param comport = new Param()
                {
                    Name = IcCardConfigParam.Row_Comport,
                    Value = item.ComPort,
                    List = (from r in item.ComportList select r.Name).ToList()
                };
                Param Ip = new Param()
                {
                    Name = IcCardConfigParam.Row_Ip,
                    Value = item.Ip
                };
                Param Port = new Param()
                {
                    Name = IcCardConfigParam.Row_Port,
                    Value = item.Port
                };
                Param baudrate = new Param()
                {
                    Name = IcCardConfigParam.Row_Baudrate,
                    Value = item.Baudrate,
                    List = (from r in item.BaudrateList select r.Name).ToList()
                };
                Param interval = new Param()
                {
                    Name = IcCardConfigParam.Row_Interval,
                    Value = item.Interval
                };
                Param iCReadType = new Param()
                {
                    Name = IcCardConfigParam.Row_ICReadType,
                    Value = item.ICReadType,
                    List = OldICReadTypeList
                };
                Param iCWriteTemp = new Param()
                {
                    Name = IcCardConfigParam.Row_ICWriteTemp,
                    Value = item.ICWriteTemp ? "是" : "否",
                    List = (from r in item.ICWriteTempList select r.Name).ToList()
                };
                Param IsUse = new Param()
                {
                    Name = IcCardConfigParam.Row_IsUse,
                    Value = item.IsUse ? "是" : "否",
                    List = (from r in item.IsUseList select r.Name).ToList()
                };
                Param driver = new Param()
                {
                    Name = IcCardConfigParam.Row_Driver,
                    Value = item.Driver
                };
 
                row.Params.Add(conType);
                row.Params.Add(comport);
                row.Params.Add(Ip);
                row.Params.Add(Port);
                row.Params.Add(baudrate);
                row.Params.Add(interval);
                row.Params.Add(iCReadType);
                row.Params.Add(driver);
                row.Params.Add(iCWriteTemp);
                row.Params.Add(IsUse);
                curSubModule.GridRow.RowList.Add(row);
            }
            if (XmlHelper.WriteXmlFile<configlist>(curConfigFileName, curConfig))
            {
                MessageBox.Show("保存成功");
               // ConfigReader.ReLoadConfig();
            }
        }

        /// <summary>
        /// 测试按钮处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (icCardMan != null)
            {
                icCardMan.Stop();
                icCardMan.Close();
              
                Thread.Sleep(2000);
            }

            
            icCardMan.Start();
        }

        void icCardMan_OnRemoveCard(string pComPortNo)
        {
            MessageBox.Show("卡片已移除。");
        }
        /// <summary>
        /// 停止读写IC卡设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopReadIc_Click(object sender, RoutedEventArgs e)
        {
            if (icCardMan != null)
            {
                icCardMan.Close();
            }
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="msg"></param>
        void icCardMan_OnShowErrorMsg(string msg)
        {
            MessageBox.Show(msg);
        }

        
        void icCardMan_OnReadCardNo(string pComPortNo, string pCardNo)
        {
            this.comTextBox.Dispatcher.Invoke(new Action(() =>
            {
               
                this.comTextBox.Text = pComPortNo;
                this.cardNoTextBox.Text = pCardNo;
            })); 
         
        }

        /// <summary>
        /// 读卡器类型选择改变事件
        /// </summary>
        private void ICReadTypeSelectionChange(object sender, SelectionChangedEventArgs e) 
        {
            if (this.ICConfigDataGrid.SelectedItems.Count==1)
            {
                int rowIndex = this.ICConfigDataGrid.SelectedIndex;
                UIIcCard icCard = this.ICConfigDataGrid.SelectedItem as UIIcCard;
                var st = (from r in oldICReadTypeList where r.Contains(icCard.ICReadType) select r).ToList();
                if (st.Count()>0)
                {
                    //获取当前行                     
                    DataGridRow row = this.ICConfigDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                    if (row != null)
                    {
                        //所选行第三列的单元格数据                         
                        TextBlock tb = this.ICConfigDataGrid.Columns[7].GetCellContent(row) as TextBlock;
                        tb.Text = st.First().Split('@')[1];
                    }
                }
            }
        }
    }
}
