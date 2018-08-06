using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using System.Xml.Serialization;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.ClientCommonLib;
using Talent.CommonMethod;
using Talent.Io.Controller;

namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// SystemConfig.xaml 的交互逻辑
    /// </summary>
    public partial class IoConfig : UserControl
    {
        public string curConfigFileName { get; set; }
        private configlist curConfig;
        /// <summary>
        /// 当前操作的子模块对象
        /// </summary>
        private submodule curSubModule;
        private List<ComboxModel> portTypeList;
        /// <summary>
        /// "端口类型"可供选择的信息集合
        /// </summary>
        public List<ComboxModel> PortTypeList
        {
            get { return portTypeList; }
            set { portTypeList = value; }
        }
        /// <summary>
        /// 下拉树集合
        /// </summary>
        private List<SomeHierarchyViewModel> comboBoxTreeList=new List<SomeHierarchyViewModel> ();
        /// <summary>
        /// 下拉树数据源集合
        /// </summary>
        private List<SomeHierarchyViewModel> comboBoxItemSource = new List<SomeHierarchyViewModel>();
        /// <summary>
        /// 选择的设备对应的驱动名称
        /// </summary>
        private string selectedDetectEquDriverName = string.Empty;
        private List<string> equNameList = new List<string>();
        public IoConfig(configlist curConfig, submodule curSubModule, string configFileName)
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
            GetEquTypeAndEquTree();
            SetFormControls();
        }
        /// <summary>
        /// 读取设备集合（Type数值：设备类型:1;设备:2）
        /// </summary>
        private void GetEquTypeAndEquTree()
        {
            string equJsonStr = JesonOperateHelper.ReadJesonFile("EquList.json");
            string equTypeJsonStr = JesonOperateHelper.ReadJesonFile("EquTypeList.json");
            List<Equ> equList = JsonConvert.DeserializeObject(equJsonStr, typeof(List<Equ>)) as List<Equ>;
            List<EquType> equTypeList = JsonConvert.DeserializeObject(equTypeJsonStr, typeof(List<EquType>)) as List<EquType>;
            var rootTypes = (from r in equTypeList where string.IsNullOrEmpty(r.ParentId) select r).ToList();
            if (rootTypes.Count > 0)
            {
                EquType rootType = rootTypes.First();//获取根节点
                ComBoxTreeModel rootNode = new ComBoxTreeModel() { Id = rootType.Id, Code = rootType.Code, Type = IoConfigParam.Type_EquType, Name = rootType.Name};
                #region 加载设备信息
                var equs = equList.Where(p => p.EquTypeId == rootType.Id).ToList();
                ComBoxTreeModel etemp;
                foreach (var equ in equs)
                {
                    etemp = new ComBoxTreeModel()
                        {
                            Id = equ.Id,
                            Code = equ.Code,
                            Name = equ.Name,
                            Type = IoConfigParam.Type_Equ,
                            Parent = rootNode
                        };
                    rootNode.Child.Add(etemp);
                }
                #endregion
                InitTreeObjects(rootNode, rootType.Child, equList);
                var list = from r in rootNode.Child select new TreeNode().Parse(r, () => r.Name, () => r.Child);
                List<TreeNode> final = new List<TreeNode>();
                TreeNode rootNode1 = new TreeNode(rootNode.Name, list)
                {
                    Target = new ComBoxTreeModel() { Child = rootNode.Child, Id = rootNode.Id, Code = rootNode.Code, Name = rootNode.Name, Type=rootNode.Type, Parent=rootNode.Parent }
                };
                final.Add(rootNode1);
                comboBoxItemSource = CovertToTree.CovertObjToTree(final, ref comboBoxTreeList);
            }
        }

        /// <summary>
        /// 构造设备类型和设备树
        /// </summary>
        /// <param name="treeNode">树节点</param>
        /// <param name="list">设备类型集合</param>
        /// <param name="equList">设备集合</param>
        private ComBoxTreeModel InitTreeObjects(ComBoxTreeModel treeNode, List<EquType> list, List<Equ> equList)
        {
            ComBoxTreeModel temp;
            ComBoxTreeModel ptemp;
            foreach (EquType item in list)
            {
                temp = new ComBoxTreeModel()
                {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    Type = IoConfigParam.Type_EquType
                };
                #region 加载设备信息
                var equs = equList.Where(p => p.EquTypeId == item.Id).ToList();
                foreach (var p in equs)
                {
                    if (p != null && !string.IsNullOrEmpty(p.Name))
                    {
                        ptemp = new ComBoxTreeModel()
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Name = p.Name,
                            Type = IoConfigParam.Type_Equ, Parent=temp
                        };
                        temp.Child.Add(ptemp);
                    }
                }
                #endregion
                if (item.Child != null && item.Child.Count > 0)
                {
                    treeNode.Child.Add(InitTreeObjects(temp, item.Child, equList));
                }
                else
                {
                    temp.Parent = treeNode;
                    treeNode.Child.Add(temp);
                }
            }
            return treeNode;
        }

        /// <summary>
        /// 设置IO控制窗体中的各控件值
        /// </summary>
        private void SetFormControls()
        {
            if (curSubModule != null)
            {
                SetDetectEqu();//
                SetLinkType();
                SetComport();
                SetBaudrate();
                var ipList = (from r in curSubModule.Params where r.Name == IoConfigParam.Ip select r).ToList();
                if (ipList.Count>0)
                {
                    this.Ip.Text = ipList.First().Value;
                    this.Ip.MaxLength = !string.IsNullOrEmpty(ipList.First().Size) ? Int32.Parse(ipList.First().Size) : IoConfigParam.TextBox_MaxLenght;
                }
                var portList = (from r in curSubModule.Params where r.Name == IoConfigParam.Port select r).ToList();
                if (portList.Count>0)
                {
                    this.Port.Text = portList.First().Value;
                    this.Port.MaxLength = !string.IsNullOrEmpty(portList.First().Size) ? Int32.Parse(portList.First().Size) : IoConfigParam.TextBox_MaxLenght;
                }
                var equLoginNameList = (from r in curSubModule.Params where r.Name == IoConfigParam.EquLoginName select r).ToList();
                if (equLoginNameList.Count>0)
                {
                    this.EquLoginName.Text = equLoginNameList.First().Value;
                    this.EquLoginName.MaxLength = !string.IsNullOrEmpty(equLoginNameList.First().Size) ? Int32.Parse(equLoginNameList.First().Size) : IoConfigParam.TextBox_MaxLenght;
                }
                var equLoginPwdList = (from r in curSubModule.Params where r.Name == IoConfigParam.EquLoginPwd select r).ToList();
                if (equLoginPwdList.Count>0)
                {
                    this.EquLoginPwd.Password = equLoginPwdList.First().Value;
                    this.EquLoginPwd.MaxLength = !string.IsNullOrEmpty(equLoginPwdList.First().Size) ? Int32.Parse(equLoginPwdList.First().Size) : IoConfigParam.TextBox_MaxLenght;
                }
                var portNumList = (from r in curSubModule.Params where r.Name == IoConfigParam.PortNum select r).ToList();
                if (portNumList.Count>0)
                {
                    this.PortNumTextBox.Text = portNumList.First().Value;
                    this.PortNumTextBox.MaxLength = !string.IsNullOrEmpty(portNumList.First().Size) ? Int32.Parse(portNumList.First().Size) : IoConfigParam.TextBox_MaxLenght;
                }               
                //this.PortNumTextBox.Text = portNumList.Count > 0 ? portNumList.First() : string.Empty;
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
                this.EquLoginName.Text = string.Empty;
                this.EquLoginPwd.Password = string.Empty;
                this.PortNumTextBox.Text = string.Empty;
                this.equConfigDataGrid.ItemsSource = null;
            }
        }

        /// <summary>
        /// 设置gridview
        /// </summary>
        private void SetGridView()
        {
            //this.comboBoxTree.ItemsSource = comboBoxItemSource;
            IList<EquConfigModel> models = new List<EquConfigModel>();
            if (curSubModule.GridRow!=null)
            {
                foreach (var row in curSubModule.GridRow.RowList)
                {
                    EquConfigModel model = new EquConfigModel();
                    foreach (var item in row.Params)
                    {
                        do
                        {
                            if (IoConfigParam.Row_EquName.Equals(item.Name))
                            {
                                model.ComboBoxTreeList = new List<SomeHierarchyViewModel>(this.comboBoxTreeList);
                                model.ComboBoxItemSource = new List<SomeHierarchyViewModel>(this.comboBoxItemSource);                              
                                var list = (from r in model.ComboBoxTreeList where r.Title == item.Value select r).ToList();
                                if (list.Count>0)
                                {
                                    list.First().IsSelected = true;
                                    model.SelectedEqu = list.First();
                                }                                
                                model.EquName = item.Value;
                                break;
                            }
                            if (IoConfigParam.Row_Code.Equals(item.Name))
                            {
                                model.Code = item.Value;
                                break;
                            }
                            if (IoConfigParam.Row_IsUse.Equals(item.Name))
                            {
                                model.IsUse = item.Value.Equals(YesNo.Yes) ? true : false;
                                break;
                            }
                            if (IoConfigParam.Row_Port.Equals(item.Name))
                            {
                                model.Port = item.Value;
                                break;
                            }
                            if (IoConfigParam.Row_AlwaysLight.Equals(item.Name))
                            {
                                model.AlwaysLight = item.Value;
                                break;
                            }
                            if (IoConfigParam.Row_PortType.Equals(item.Name))
                            {
                                model.PortType = item.Value;
                                model.PortTypeList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                if (this.PortTypeList == null || this.PortTypeList.Count == 0)//设置行中"端口类型"数据源
                                {
                                    this.PortTypeList = (from r in item.List select new ComboxModel() { Code = r, Name = r, Type = r }).ToList();
                                }
                                break;
                            }
                            if (IoConfigParam.Row_Type.Equals(item.Name))
                            {
                                model.Type = item.Value.Equals(IoConfigParam.Type_EquType) ? "设备类型" : "设备";
                                break;
                            }
                            if (IoConfigParam.Row_EquTypeCode.Equals(item.Name))
                            {
                                model.EquTypeCode = item.Value;
                                break;
                            }
                        } while (false);
                    }
                    models.Add(model);
                }
            }
            this.equConfigDataGrid.ItemsSource = models;
            this.equConfigDataGrid.Items.Refresh();
            SetSelectedComboxTree();
        }


        /// <summary>
        /// 设置检测设备
        /// </summary>
        private void SetDetectEqu()
        {
            var detectEquList = (from r in curSubModule.Params where r.Name == IoConfigParam.DetectEqu select r).ToList();
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
            SetComboxDs(this.ConType, IoConfigParam.LinkType);
        }

        /// <summary>
        /// 设置串口
        /// </summary>
        private void SetComport()
        {
            SetComboxDs(this.Comport, IoConfigParam.Comport);
        }
        /// <summary>
        /// 波特率
        /// </summary>
        private void SetBaudrate()
        {
            SetComboxDs(this.Baudrate, IoConfigParam.Baudrate);
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

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime t1 = DateTime.Now;
            Thread.Sleep(1000);
 
            string data = TestData.Text.Trim();
            string[] tempList = data.Split(new char[]{'|'});
            if (tempList.Length != 3) return;
            string userName = tempList[0];
            string password = tempList[1];
            string command = tempList[2];          
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ClientConfig");
            string configPath = System.IO.Path.Combine(basePath, curConfigFileName); ;
            IoController ioc = new IoController(configPath);
            ioc.OnShowErrMsg += ioc_OnShowErrMsg;
            ioc.OnReceiveAlarmSignal += ioc_OnReceiveAlarmSignal;            
            bool result = ioc.TestExecCommand(command);
            
        }
        /// <summary>
        /// 报警处理
        /// </summary>
        /// <param name="pDeviceCode"></param>
        /// <param name="pValue"></param>
        void ioc_OnReceiveAlarmSignal(string pDeviceCode, string pValue)
        {
            string msg = "";
            if (pValue == "0")
            {
                msg = string.Format("端口：{0}，状态：{1}", pDeviceCode, "未遮挡");
            }
            else
            {
                msg = string.Format("端口：{0}，状态：{1}", pDeviceCode, "遮挡");
            }

            txtIsZheDang.Dispatcher.Invoke(new Action(() =>
            {
                txtIsZheDang.Text = msg;
            }));
        }
        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="msg"></param>
        void ioc_OnShowErrMsg(string msg)
        {
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 报警
        /// </summary>
        /// <param name="pPort"></param>
        /// <param name="pValue"></param>
        /// <returns></returns>
        void nvr_OnReceiveAlarmSignal(string pPort, string pValue)
        {
            MessageBox.Show(string.Format("端口{0}收到报警信号。",pPort));
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
            #region 弹出子窗体方式
            //IoConfigChildForm childFrom = new IoConfigChildForm(comboBoxItemSource,comboBoxTreeList,null, true);
            //childFrom.ShowDialog();
            //if (!childFrom.IsFormCancel)
            //{
            //    List<EquConfigModel> list = this.equConfigDataGrid.ItemsSource as List<EquConfigModel>;
            //    list.Add(childFrom.EquConfig);
            //    this.equConfigDataGrid.ItemsSource = list;
            //    this.equConfigDataGrid.SelectedItem = this.equConfigDataGrid.Items[this.equConfigDataGrid.Items.Count-1];
            //    this.equConfigDataGrid.Items.Refresh();
            //}
            #endregion
            //获取配置
            IList<EquConfigModel> models = this.equConfigDataGrid.ItemsSource as IList<EquConfigModel>;
            EquConfigModel model = new EquConfigModel()
            {
                ComboBoxItemSource = new List<SomeHierarchyViewModel>(this.comboBoxItemSource),
                ComboBoxTreeList = new List<SomeHierarchyViewModel>(this.comboBoxTreeList),
                IsUse = true, PortTypeList=this.PortTypeList
            };
            model.PortType = (model.PortTypeList != null && model.PortTypeList.Count > 0) ? model.PortTypeList.First().Name : string.Empty;
            model.SelectedEqu = model.ComboBoxItemSource.First();
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
                MessageBox.Show("请选择一条设备配置信息");
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
            //获取检测设备
            var detectEquList = (from r in curSubModule.Params where r.Name == IoConfigParam.DetectEqu select r).ToList();
            if (detectEquList.Count > 0)
            {
                var detectEqu = this.UsePassCarType.SelectedItem as ComboBoxItem; // detectEquList
                detectEquList.First().Value = detectEqu.Content.ToString();
            }
            //获取连接方式
            var list = (from r in curSubModule.Params where r.Name == IoConfigParam.LinkType select r).ToList();
            if (list.Count > 0)
            {
                var detectEqu = this.ConType.SelectedItem as ComboBoxItem; // detectEquList
                list.First().Value = detectEqu.Content.ToString();
            }
            //获取串口
            var comportList = (from r in curSubModule.Params where r.Name == IoConfigParam.Comport select r).ToList();
            if (comportList.Count > 0)
            {
                var port = this.Comport.SelectedItem as ComboBoxItem; // detectEquList
                comportList.First().Value = port.Content.ToString();
            }
            //获取IP
            var ipList = (from r in curSubModule.Params where r.Name == IoConfigParam.Ip select r).ToList();
            if (ipList.Count > 0)
            {
                ipList.First().Value = this.Ip.Text.Trim();
            }
            //获取端口
            var portList = (from r in curSubModule.Params where r.Name == IoConfigParam.Port select r).ToList();
            if (portList.Count > 0)
            {
               
                if (!CommonMethod.CommonMethod.IsDataTransformSuccess(portList.First().Type, this.Port.Text.Trim()))
                {
                    MessageBox.Show(string.Format(CommonParam.Info_Input_Msg_Exption, portList.First().Lab));
                    this.Port.Focus();
                    return;
                }
                portList.First().Value = this.Port.Text.Trim();
            }
            //获取波特率
            var baudrateList = (from r in curSubModule.Params where r.Name == IoConfigParam.Baudrate select r).ToList();
            if (baudrateList.Count > 0)
            {
                var baudrate = this.Baudrate.SelectedItem as ComboBoxItem;
                baudrateList.First().Value = baudrate.Content.ToString();
            }
            //获取设备登录名
            var equLoginNameList = (from r in curSubModule.Params where r.Name == IoConfigParam.EquLoginName select r).ToList();
            if (equLoginNameList.Count > 0)
            {
                equLoginNameList.First().Value = this.EquLoginName.Text.Trim();
            }
            //获取设备登录密码
            var equLoginPwdList = (from r in curSubModule.Params where r.Name == IoConfigParam.EquLoginPwd select r).ToList();
            if (equLoginPwdList.Count > 0)
            {
                equLoginPwdList.First().Value = this.EquLoginPwd.Password.Trim();
            }
            //获取端口数量
            var portNumList = (from r in curSubModule.Params where r.Name == IoConfigParam.PortNum select r).ToList();
            if (portNumList.Count > 0)
            {
                if (!CommonMethod.CommonMethod.IsDataTransformSuccess(portNumList.First().Type, this.PortNumTextBox.Text.Trim()))
                {
                    MessageBox.Show(string.Format(CommonParam.Info_Input_Msg_Exption, portNumList.First().Lab));
                    this.PortNumTextBox.Focus();
                    return;
                }
                portNumList.First().Value = this.PortNumTextBox.Text.Trim();
            }
            //获取设备驱动名称
            var dllList = (from r in curSubModule.Params where r.Name == IoConfigParam.EquDriverName select r).ToList();
            if (dllList.Count > 0)
            {
                dllList.First().Value = this.EquDll.Text.Trim();
            }
            //获取设备配置
            IList<EquConfigModel> models = this.equConfigDataGrid.ItemsSource as IList<EquConfigModel>;
            foreach (var item in models)
            {
                var codes = (from r in models where r.Code == item.Code select r).ToList();
                if (codes!=null&&codes.Count>1)
                {
                    int firstEqu = models.IndexOf(codes.First());
                    int secondEqu = models.IndexOf(codes[1]);
                    MessageBox.Show("第" + (firstEqu + 1) + "个设备【" + codes.First().EquName + "】与第" + (secondEqu + 1) + "个设备【" + codes[1].EquName + "】重复");
                    return;
                }
            }
            curSubModule.GridRow.RowList.Clear();
            foreach (var item in models)
            {
                if (!CommonMethod.CommonMethod.IsDataTransformSuccess("int", item.Port))
                {
                    MessageBox.Show("第" + (models.IndexOf(item) + 1) + "个设备配置信息中" + string.Format(CommonParam.Info_Input_Msg_Exption, CommonParam.Port_TextBox_Name));
                    this.equConfigDataGrid.SelectedIndex = models.IndexOf(item);
                    return;
                }
                Row row = new Row() { Params = new List<Param>() };
                Param equName = new Param()
                    {
                        Name = IoConfigParam.Row_EquName,
                        Value = item.EquName
                    };
                Param code = new Param()
                {
                    Name = IoConfigParam.Row_Code,
                    Value = item.Code
                };
                Param IsUse = new Param()
                {
                    Name = IoConfigParam.Row_IsUse,
                    Value = item.IsUse ? YesNo.Yes : YesNo.No
                };
                Param portType = new Param()
                {
                    Name = IoConfigParam.Row_PortType,
                    Value = item.PortType,
                    List = (from r in item.PortTypeList select r.Name).ToList()
                };
                Param Port = new Param()
                {
                    Name = IoConfigParam.Row_Port,
                    Value = item.Port
                };
                Param alwaysLight = new Param()
                {
                    Name = IoConfigParam.Row_AlwaysLight,
                    Value = item.AlwaysLight
                };
                Param Type = new Param()
                {
                    Name = IoConfigParam.Row_Type,
                    Value = item.Type.Equals("设备类型") ? IoConfigParam.Type_EquType : IoConfigParam.Type_Equ
                };
                Param equTypeCode = new Param()
                {
                    Name = IoConfigParam.Row_EquTypeCode,
                    Value = item.EquTypeCode
                };
                row.Params.Add(equName);
                row.Params.Add(code);
                row.Params.Add(equTypeCode);
                row.Params.Add(IsUse);
                row.Params.Add(Port);
                row.Params.Add(Type);
                row.Params.Add(portType);
                row.Params.Add(alwaysLight);
                curSubModule.GridRow.RowList.Add(row);
            }
            if (XmlHelper.WriteXmlFile<configlist>(curConfigFileName, curConfig))
            {
                MessageBox.Show("保存成功");
                //ConfigReader.ReLoadConfig();
            }
        }

        /// <summary>
        /// 下拉框选择触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxTreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;
            if (this.equConfigDataGrid.SelectedItems.Count == 1)
            {
                int rowIndex = this.equConfigDataGrid.SelectedIndex;
                DataGridRow row = this.equConfigDataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
                if (row != null)
                {
                    SomeHierarchyViewModel smodel = ((EquConfigModel)this.equConfigDataGrid.SelectedItem).SelectedEqu;
                    if (smodel!=null)
                    {
                        ComBoxTreeModel cbtModel = (smodel.obj as TreeNode).Target as ComBoxTreeModel;
                        //所选行第4列的单元格数据(赋值)                       
                        TextBlock tb = this.equConfigDataGrid.Columns[5].GetCellContent(row) as TextBlock;
                        if (tb != null)
                        {
                            tb.Text = cbtModel.Code;
                        }
                        //值在界面不需要显示，故而可只给属性赋值，如果要界面显示，则此处后面注释掉的代码打开即可
                        (row.Item as EquConfigModel).Type = cbtModel.Type.Equals(IoConfigParam.Type_EquType) ? "设备类型" : "设备";
                        if (cbtModel.Type.Equals(IoConfigParam.Type_EquType))
                        {
                            (row.Item as EquConfigModel).EquTypeCode = cbtModel.Code;
                        }
                        else
                        {
                            (row.Item as EquConfigModel).EquTypeCode = GetEquTypeCode(smodel);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 递归查询设备所属的设备类型编码
        /// </summary>
        /// <param name="smodel"></param>
        /// <returns></returns>
        private string GetEquTypeCode(SomeHierarchyViewModel smodel)
        {
            if (smodel != null)
            {
                if (smodel.Parent != null)
                {
                    SomeHierarchyViewModel shv = smodel.Parent;
                    ComBoxTreeModel cbtModel = (shv.obj as TreeNode).Target as ComBoxTreeModel;
                    if (cbtModel.Type.Equals(IoConfigParam.Type_EquType))
                    {
                        return cbtModel.Code;
                    }
                    else
                    {
                        return GetEquTypeCode(shv);
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private void SetSelectedComboxTree()
        {
        }
    }
}
