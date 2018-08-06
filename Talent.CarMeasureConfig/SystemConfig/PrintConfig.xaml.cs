using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.Printer.Controller;

namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// 打印配置 的交互逻辑
    /// </summary>
    public partial class PrintConfig : UserControl
    {
        public string curConfigFileName { get; set; }
        private configlist curConfig;
        /// <summary>
        /// 当前操作的子模块对象
        /// </summary>
        private submodule curSubModule;
        private string path = AppDomain.CurrentDomain.BaseDirectory + "\\ClientConfig";

        #region 窗体中的下拉框数据源

        private List<ComboxModel> comportList;
        /// <summary>
        /// "串口"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> ComportList
        {
            get { return comportList; }
            set { comportList = value; }
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

        private List<ComboxModel> isUseList;
        /// <summary>
        /// "是否启用"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> IsUseList
        {
            get { return isUseList; }
            set { isUseList = value; }
        }

        private List<ComboxModel> notchList;
        /// <summary>
        /// "黑标"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> NotchList
        {
            get { return notchList; }
            set { notchList = value; }
        }


        private List<ComboxModel> brandList;
        /// <summary>
        /// 品牌
        /// </summary>
        public List<ComboxModel> BrandList
        {
            get { return brandList; }
            set { brandList = value; }
        }

        private List<string> oldBrandList;
        /// <summary>
        /// 原有品牌信息
        /// </summary>
        public List<string> OldBrandList
        {
            get { return oldBrandList; }
            set { oldBrandList = value; }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="curConfig">当前配置对象</param>
        /// <param name="curSubModule">当前模块对象</param>
        /// <param name="configFileName">配置文件名称</param>
        public PrintConfig(configlist curConfig, submodule curSubModule, string configFileName)
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
        /// 设置IC控制窗体中的各控件值
        /// </summary>
        private void SetFormControls()
        {
            if (curSubModule != null)
            {
                IList<UIPrintConfigModel> models = new List<UIPrintConfigModel>();
                if (curSubModule.GridRow != null)
                {
                    foreach (var row in curSubModule.GridRow.RowList)
                    {
                        UIPrintConfigModel model = new UIPrintConfigModel();
                        foreach (var item in row.Params)
                        {
                            do
                            {
                                if (PrinterConfigParam.Row_PrinterName.Equals(item.Name))
                                {
                                    model.PrinterName = item.Value;
                                    break;
                                }
                                if (PrinterConfigParam.Row_Comport.Equals(item.Name))
                                {
                                    model.ComPort = item.Value;
                                    model.ComportList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    if (this.ComportList == null || this.ComportList.Count == 0)//设置行中"串口"数据源
                                    {
                                        this.ComportList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    }
                                    break;
                                }
                                if (PrinterConfigParam.Row_Baudrate.Equals(item.Name))
                                {
                                    model.Baudrate = item.Value;
                                    model.BaudrateList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    if (this.BaudrateList == null || this.BaudrateList.Count == 0)//设置行中"串口"数据源
                                    {
                                        this.BaudrateList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    }
                                    break;
                                }
                                if (PrinterConfigParam.Row_PageMaxCount.Equals(item.Name))
                                {
                                    model.PageMaxCount = string.IsNullOrEmpty(item.Value) ? 0 : int.Parse(item.Value);
                                    break;
                                }
                                if (PrinterConfigParam.Row_Driver.Equals(item.Name))
                                {
                                    model.Driver = item.Value;
                                    break;
                                }
                                if (PrinterConfigParam.Row_Notch.Equals(item.Name))
                                {
                                    model.Notch = item.Value.Equals("是") ? true : false;
                                    model.NotchList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    if (this.NotchList == null || this.NotchList.Count == 0)//设置行中"启用黑标"数据源
                                    {
                                        this.NotchList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                    }
                                    break;
                                }
                                if (PrinterConfigParam.Row_Band.Equals(item.Name))
                                {
                                    model.Brand = item.Value;
                                    model.BrandList = (from r in item.List select new ComboxModel() { Code = r, Name = r.Split('@')[0], Type = r.Split('@')[0] }).ToList();
                                    if (this.BrandList == null || this.BrandList.Count == 0)
                                    {
                                        OldBrandList = item.List;
                                        this.BrandList = (from r in item.List select new ComboxModel() { Code = r, Name = r.Split('@')[0], Type = r.Split('@')[0] }).ToList();
                                    }
                                    break;
                                }
                                if (PrinterConfigParam.Row_IsUse.Equals(item.Name))
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
                this.PrintConfigDataGrid.ItemsSource = models;
            }
            else
            {
                this.PrintConfigDataGrid.ItemsSource = null;
            }
        }

        /// <summary>
        /// 新增按钮点击事件
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //获取配置
            IList<UIPrintConfigModel> models = this.PrintConfigDataGrid.ItemsSource as IList<UIPrintConfigModel>;
            UIPrintConfigModel printConfig = new UIPrintConfigModel()
            {
                ComportList = this.ComportList,
                IsUseList = this.IsUseList,
                IsUse = true,
                BaudrateList = this.BaudrateList,
                NotchList = this.NotchList,
                Notch = true,
                BrandList = this.BrandList,
            };
            printConfig.Driver = BrandList.First().Code.Split('@')[1];
            printConfig.ComPort = (printConfig.ComportList != null && printConfig.ComportList.Count > 0) ? printConfig.ComportList.First().Name : string.Empty;
            printConfig.Baudrate = (printConfig.BaudrateList != null && printConfig.BaudrateList.Count > 0) ? printConfig.BaudrateList.First().Name : string.Empty;
            models.Add(printConfig);
            this.PrintConfigDataGrid.ItemsSource = models;
            this.PrintConfigDataGrid.Items.Refresh();
        }
        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        private void Detete_Click(object sender, RoutedEventArgs e)
        {
            if (this.PrintConfigDataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择一条打印配置信息");
            }
            else
            {
                List<UIPrintConfigModel> list = this.PrintConfigDataGrid.ItemsSource as List<UIPrintConfigModel>;
                UIPrintConfigModel removeModel = this.PrintConfigDataGrid.SelectedItem as UIPrintConfigModel;
                list.Remove(removeModel);
                this.PrintConfigDataGrid.ItemsSource = list;
                this.PrintConfigDataGrid.Items.Refresh();
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
            //获取配置
            IList<UIPrintConfigModel> models = this.PrintConfigDataGrid.ItemsSource as IList<UIPrintConfigModel>;
            curSubModule.GridRow.RowList.Clear();
            foreach (var item in models)
            {
                Row row = new Row() { Params = new List<Param>() };
                Param printName = new Param()
                {
                    Name = PrinterConfigParam.Row_PrinterName,
                    Value = item.PrinterName
                };
                Param comport = new Param()
                {
                    Name = PrinterConfigParam.Row_Comport,
                    Value = item.ComPort,
                    List = (from r in item.ComportList select r.Name).ToList()
                };
                Param baudrate = new Param()
                {
                    Name = PrinterConfigParam.Row_Baudrate,
                    Value = item.Baudrate,
                    List = (from r in item.BaudrateList select r.Name).ToList()
                };
                Param PageMaxCount = new Param()
                {
                    Name = PrinterConfigParam.Row_PageMaxCount,
                    Value = item.PageMaxCount.ToString()
                };
                Param IsUse = new Param()
                {
                    Name = PrinterConfigParam.Row_IsUse,
                    Value = item.IsUse ? "是" : "否",
                    List = (from r in item.IsUseList select r.Name).ToList()
                };
                Param Notch = new Param()
                {
                    Name = PrinterConfigParam.Row_Notch,
                    Value = item.Notch ? "是" : "否",
                    List = (from r in item.IsUseList select r.Name).ToList()
                };
                Param Brand = new Param()
                {
                    Name = PrinterConfigParam.Row_Band,
                    Value = item.Brand,
                    List = (from r in item.BrandList select r.Code).ToList()
                };
                Param driver = new Param()
                {
                    Name = IcCardConfigParam.Row_Driver,
                    Value = item.Driver
                };
                row.Params.Add(printName);
                row.Params.Add(comport);
                row.Params.Add(baudrate);
                row.Params.Add(PageMaxCount);
                row.Params.Add(IsUse);
                row.Params.Add(Notch);
                row.Params.Add(Brand);
                row.Params.Add(driver);
                curSubModule.GridRow.RowList.Add(row);
            }
            if (XmlHelper.WriteXmlFile<configlist>(curConfigFileName, curConfig))
            {
                MessageBox.Show("保存成功");
                //  ConfigReader.ReLoadConfig();
            }
        }

        /// <summary>
        /// 打印测试页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ClientConfig");

            string configPath = System.IO.Path.Combine(basePath, curConfigFileName); ;
           
         
            #region local
            string data = GetPrintData();
            if (data.Length > 0)
            {
                List<string> tDataList = new List<string>();
                tDataList.Add(data);
                Thread ts = new Thread(new ThreadStart(delegate {
                    PrinterController printer = new PrinterController(configPath);
                    printer.OnShowErrMsg += printer_OnShowErrMsg;
                    printer.Print(tDataList); 
                }));
                ts.IsBackground = true;
                ts.Start();
               // printer.Print(tDataList);

            }
            #endregion
        }

        void printer_OnShowErrMsg(ErrorType arg1, string arg2)
        {
            if (arg1 == ErrorType.Error)
            {
                MessageBox.Show(arg2);
            }
        }

        private static string RevertBase64(string data)
        {
            byte[] outputb = Convert.FromBase64String(data);
            string orgStr = System.Text.Encoding.UTF8.GetString(outputb);
            return orgStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetPrintData()
        {
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ClientConfig\\TestPrint.xml");
            TextReader sr = new StreamReader(basePath, Encoding.UTF8);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// 品牌选择改变事件
        /// </summary>
        private void BrandSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (this.PrintConfigDataGrid.SelectedItems.Count == 1)
            {
                int rowIndex = this.PrintConfigDataGrid.SelectedIndex;
                UIPrintConfigModel printer = this.PrintConfigDataGrid.SelectedItem as UIPrintConfigModel;
                var st = (from r in OldBrandList where r.Contains(printer.Brand) select r).ToList();
                if (st.Count() > 0)
                {
                    //获取当前行                     
                    DataGridRow row = this.PrintConfigDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                    if (row != null)
                    {
                        TextBlock tb = this.PrintConfigDataGrid.Columns[5].GetCellContent(row) as TextBlock;
                        tb.Text = st.First().Split('@')[1];
                    }
                }
            }
        }
    }
}
