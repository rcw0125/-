using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Talent.CarMeasureConfig.SystemConfig;
using Talent.Measure.DomainModel.CommonModel;
using Talent.ClientCommonLib;
using Talent.CommonMethod;
using System.Configuration;
using System.Net;
using Talent.Measure.DomainModel;
using Talent.Measure.WPF.Remote;

namespace Talent.CarMeasureConfig
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
       
        /// <summary>
        /// xml文件存放路径
        /// </summary>
        private string path = AppDomain.CurrentDomain.BaseDirectory + "\\ClientConfig";
        /// <summary>
        /// 所有配置叶子节点集合
        /// </summary>
        private List<submodule> subModels;

        private submodule curSubModule;
        /// <summary>
        /// 当前配置文件名称
        /// </summary>
        public string curConfigFileName { get; set; }
        /// <summary>
        /// xml文件反序列化后的对象
        /// </summary>
        private configlist curConfig;
        public MainWindow()
        {
            InitializeComponent();
            this.Background = (Brush)TryFindResource("MyGradientBackground");
        }

        #region 生成设备和设备类型的json文件
        /// <summary>
        /// 生成设备json文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateEquJesonFile_Click(object sender, RoutedEventArgs e)
        {
            IList<EquType> equTypes = new List<EquType>();
            EquType rootType = new EquType() { Id = "01", Code = "EquType", Name = "设备类型" };
            EquType dType = new EquType() { Id = "0101", Code = "Led", Name = "灯", ParentId = "01" };
            //EquType hdType = new EquType() { Id = "010101", Code = "RedLed", Name = "红灯", ParentId = "0101" };
            //EquType ldType = new EquType() { Id = "010102", Code = "GreenLed", Name = "绿灯", ParentId = "0101" };
            EquType hwType = new EquType() { Id = "0102", Code = "InfraredCorrelation", Name = "红外对射", ParentId = "01" };

            //dType.Child.Add(hdType);
            //dType.Child.Add(ldType);
            rootType.Child.Add(dType);
            rootType.Child.Add(hwType);
            equTypes.Add(rootType);
            string equTypeJsonStr = JsonConvert.SerializeObject(equTypes);

            IList<Equ> equs = new List<Equ>();
            Equ zhd = new Equ() { EquTypeId = "0101", Name = "左红绿灯", Id = "1", Code = "LeftLed" };
            Equ yhd = new Equ() { EquTypeId = "0101", Name = "右红绿灯", Id = "2", Code = "RightLed" };
            //Equ zhd = new Equ() { EquTypeId = "010101", Name = "左红灯", Id = "1", Code = "LeftRedLed" };
            //Equ yhd = new Equ() { EquTypeId = "010101", Name = "右红灯", Id = "2", Code = "RightRedLed" };
            //Equ zld = new Equ() { EquTypeId = "010102", Name = "左绿灯", Id = "3", Code = "LeftGreenLed" };
            //Equ yld = new Equ() { EquTypeId = "010102", Name = "右绿灯", Id = "4", Code = "RightGreenLed" };
            Equ zhw = new Equ() { EquTypeId = "0102", Name = "左红外", Id = "3", Code = "LeftInfraredCorrelation" };
            Equ yhw = new Equ() { EquTypeId = "0102", Name = "右红外", Id = "4", Code = "RightInfraredCorrelation" };
            Equ qhw = new Equ() { EquTypeId = "0102", Name = "前红外", Id = "5", Code = "AheadInfraredCorrelation" };
            Equ hhw = new Equ() { EquTypeId = "0102", Name = "后红外", Id = "6", Code = "BehindInfraredCorrelation" };

            equs.Add(zhd);
            equs.Add(yhd);
            //equs.Add(zld);
            //equs.Add(yld);
            equs.Add(zhw);
            equs.Add(yhw);
            equs.Add(qhw);
            equs.Add(hhw);
            string equJsonStr = JsonConvert.SerializeObject(equs);

            JesonOperateHelper.WriteJesonFile("EquTypeList.json", equTypeJsonStr);
            JesonOperateHelper.WriteJesonFile("EquList.json", equJsonStr);
        }
        #endregion

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
                for (int i = 0; i < collection.Length;i++ )
                {
                    FileInfo item=collection[i];
                    string fullname = item.Name.ToString();
                    string filename = fullname.Substring(0, fullname.LastIndexOf("."));//去掉后缀名。
                    TreeViewItem config = new TreeViewItem() { Header = filename };
                    this.configFiles.Items.Add(config);
                    if(i==0)//默认第一个打开
                    {
                        config.IsSelected = true;
                    }
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
                try
                {
                    string fileMainPath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig";
                    curConfigFileName = ((TreeViewItem)this.configFiles.SelectedItem).Header + ".xml";
                    curConfig = XmlHelper.ReadXmlToObj<configlist>(fileMainPath + "\\" + curConfigFileName);
                    this.configNodeNames.Items.Clear();
                    subModels = new List<submodule>();
                    foreach (var item in curConfig.Modules)
                    {
                        TreeViewItem rootItem = new TreeViewItem() { Header = item.Name, Tag = item.Code };
                        rootItem.IsExpanded = true;//默认展开
                        foreach (submodule subItem in item.SubModules)
                        {
                            subModels.Add(subItem);
                            TreeViewItem childItem = new TreeViewItem() { Header = subItem.Name, Tag = subItem.Code };
                            rootItem.Items.Add(childItem);
                            rootItem.IsExpanded = true;                           
                        }
                        this.configNodeNames.Items.Add(rootItem);
                    }
                    tabControl.Items.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
      
            }
        }

        /// <summary>
        /// 配置节点点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void configNodeNames_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.configNodeNames.SelectedItem != null)
            {
                TreeViewItem item = this.configNodeNames.SelectedItem as TreeViewItem;
                string modelCode = item.Tag as string;
                var curSubModels = (from r in subModels where r.Code == modelCode select r).ToList();
                if (curSubModels.Count > 0)
                {
                    curSubModule = curSubModels.First();
                    string fileName = ((TreeViewItem)this.configFiles.SelectedItem).Header + ".xml";
                    do
                    {
                        if (modelCode.Equals(IoConfigParam.Model_Code_IoConfig))
                        {
                            //decimal d = Convert.ToDecimal("dd");//测试异常
                            ChangePage("Talent.CarMeasureConfig.SystemConfig.IoConfig", curSubModels.First().Name, fileName, curSubModels.First().Code);
                            break;
                        }
                        if (modelCode.Equals(IcCardConfigParam.Model_Code_IcConfig))
                        {
                            ChangePage("Talent.CarMeasureConfig.SystemConfig.IcConfig", curSubModels.First().Name, fileName, curSubModels.First().Code);
                            break;
                        }
                        if (modelCode.Equals(RfidConfigParam.Model_Code_RfidConfig))
                        {
                            ChangePage("Talent.CarMeasureConfig.SystemConfig.RfidConfig", curSubModels.First().Name, fileName, curSubModels.First().Code);
                            break;
                        }
                        if (modelCode.Equals(VideoConfigParam.Model_Code_VideoConfig))
                        {
                            ChangePage("Talent.CarMeasureConfig.SystemConfig.VideoConfig", curSubModels.First().Name, fileName, curSubModels.First().Code);
                            break;
                        }
                        if (modelCode.Equals(PrinterConfigParam.Model_Code_PrinterConfig))
                        {
                            ChangePage("Talent.CarMeasureConfig.SystemConfig.PrintConfig", curSubModels.First().Name, fileName, curSubModels.First().Code);
                            break;
                        }
                        if (modelCode.Equals(KeyboardConfigParam.Model_Code_KeyboardConfig))
                        {
                            ChangePage("Talent.CarMeasureConfig.SystemConfig.KeyboardConfig", curSubModels.First().Name, fileName, curSubModels.First().Code);
                            break;
                        }
                        if (modelCode.Equals(MultiRfidConfigParam.Model_Code_RfidConfig))
                        {
                            ChangePage("Talent.CarMeasureConfig.SystemConfig.MultiRfidConfig", curSubModels.First().Name, fileName, curSubModels.First().Code);
                            break;
                        }
                        ChangePage("Talent.CarMeasureConfig.SystemConfig.MeasurementConfig", curSubModule.Name, fileName, curSubModels.First().Code);
                    } while (false);
                }
            }
        }

        /// <summary>
        /// 打开窗体
        /// </summary>
        /// <param name="formUrl">窗体URL</param>
        /// <param name="modelName">窗体名称(就是xml配置的节点名称)</param>
        /// <param name="fileName"></param>
        /// <param name="modelCode"></param>
        private void ChangePage(string formUrl, string modelName, string fileName, string modelCode)
        {
            if (string.IsNullOrEmpty(formUrl))
                return;
            TabItem tabItem = new TabItem();
            tabItem.Header = modelName;
            tabItem.Tag = modelCode;
            foreach (TabItem item in tabControl.Items)
            {
                if (item.Tag.ToString() == modelCode)
                {
                    tabControl.SelectedItem = item;
                    return;
                }
            }
            if (tabControl.Items.Count >= 12)
            {
                MessageBox.Show("打开窗口数量已达到上限！");
                return;
            }
            tabItem.Content = CreateElementFromPrim(formUrl, fileName);
            if (tabControl.Items.Count == 0)
                tabControl.Visibility = Visibility.Visible;

            tabControl.Items.Add(tabItem);
            tabControl.SelectedItem = tabItem;
        }

        /// <summary>
        /// 创建窗体元素
        /// </summary>
        /// <param name="path">窗体路径</param>
        /// <returns></returns>
        public FrameworkElement CreateElementFromPrim(string path, string fileName)
        {
            try
            {
                Type type = Type.GetType(path);
                object obj = System.Activator.CreateInstance(type, curConfig, curSubModule, fileName);
                return obj as FrameworkElement;
            }
            catch (Exception ee)
            {
                if (ee.InnerException != null)
                    return new TextBlock() { Text = ee.InnerException.InnerException.Message };
                else
                    return new TextBlock() { Text = ee.Message };
            }
        }

        /// <summary>
        /// tab关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTabClose_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string header = btn.Tag.ToString();
            foreach (TabItem item in tabControl.Items)
            {
                if (item.Header.ToString() == header)
                {
                    tabControl.Items.Remove(item);
                    if (tabControl.Items.Count == 0)
                        tabControl.Visibility = Visibility.Collapsed;
                    break;
                }
            }
        }

        /// <summary>
        /// 上传配置文件
        /// </summary>
        private void UploadConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.configFiles.SelectedItem != null)
            {
                var sysModule = (from r in curConfig.Modules where r.Code == IoConfigParam.Model_Code_SystemConfigs select r).ToList().First();
                var sysConfig = (from r in sysModule.SubModules where r.Code == IoConfigParam.Model_Code_SystemConfig select r).ToList().First();
                var versionNum = (from r in sysConfig.Params where r.Name == IoConfigParam.VersionNum select r).ToList().First().Value;
                var clientCode = (from r in sysConfig.Params where r.Name == IoConfigParam.ClientCode select r).ToList().First().Value;
                var clientName = (from r in sysConfig.Params where r.Name == IoConfigParam.ClientName select r).ToList().First().Value;
                int versionNo = -1;
                try
                {
                    versionNo = Int32.Parse(versionNum);
                    UpLoadConfigData(versionNo, clientCode, clientName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("上传配置信息异常!原因:" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请选择配置文件!");
            }
        }

        /// <summary>
        /// 上传配置数据
        /// </summary>
        /// <param name="versionNum">版本号</param>
        /// <param name="clientCode">终端编码</param>
        /// <param name="clientName">终端名称</param>
        private void UpLoadConfigData(int versionNum, string clientCode, string clientName)
        {
            StringBuilder configXMlString = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(typeof(configlist));
            using (TextWriter writer = new StringWriter(configXMlString))
            {
                serializer.Serialize(writer, curConfig);
            }
            configXMlString.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "<?xml version=\"1.0\" encoding=\"utf-8\"?>");//由于序列化后的文本为utf-16，需要替换为utf-8
            configXMlString.Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");    //同时替换掉不相关的属性
            configXMlString.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            string serviceUrl = ConfigurationManager.AppSettings["saveEquParamInfo"].ToString();
            var param = new
            {
                paraminfos = configXMlString.ToString(),
                equcode = clientCode,
                equname = clientName,
                versionnum = versionNum
            };
            HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, "[" + JsonConvert.SerializeObject(param) + "]", "utf-8");
            request.BeginGetResponse(new AsyncCallback(UpLoadConfigDataCallback), request);
        }

        /// <summary>
        /// 上传配置数据回调方法
        /// </summary>
        /// <param name="asyc"></param>
        private void UpLoadConfigDataCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult=ComHelpClass.ResponseStr(asyc);                
                var attionCallBack = InfoExchange.DeConvert(typeof(LoginServiceModel), strResult) as LoginServiceModel;
                if (!attionCallBack.success)
                {
                    MessageBox.Show("数据保存失败!");
                    return;
                }
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "配置管理窗体",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "上传配置信息成功!",
                    Origin = "配置管理",
                    OperateUserName = "",
                    Data = attionCallBack,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                MessageBox.Show("上传成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据保存异常!原因:" + ex.Message);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "配置管理窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "上传配置信息失败！原因：" + ex.Message,
                    Origin = "配置管理",
                    OperateUserName = ""
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

       
    }
}
