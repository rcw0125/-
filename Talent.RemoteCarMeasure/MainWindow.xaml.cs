using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Talent.ClientCommMethod;
using Talent.ClientCommonLib;
using Talent.ClientCommonLib.Controls;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.RemoteCarMeasure.View;
using Talent.RemoteCarMeasure.ViewModel;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.RemoteCarMeasure.Commom.Control;
using System.Configuration;
using System.Net;
using Talent.RemoteCarMeasure.Commom;
using System.Runtime.InteropServices;
using Talent_LT.HelpClass;
using System.Windows.Interop;
using Talent.Measure.WPF;
using Talent.Measure.WPF.Log;

namespace Talent.RemoteCarMeasure
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Only_WindowBase
    {

        /// <summary>
        /// 坐席关注的汽车衡客户端集合
        /// </summary>
        private List<SeatAttentionWeightModel> SeatAttentionInfos;
        CheckRemotePower checkRemoteP = new CheckRemotePower();//判断坐席权限
        WindowsFormHelpClass formHClass = new WindowsFormHelpClass();//窗体帮助类
        /// <summary>
        /// 日志记录
        /// </summary>
        LogsHelpClass logH = new LogsHelpClass();
        /// <summary>
        /// 是否允许关闭窗体
        /// </summary>
        bool isAllowClose = false;
        public MainWindow(List<SeatAttentionWeightModel> seatAttentionInfos)
        {
            this.RegisterMessenger();
            isAllowClose = false;
            RemoteCarMeasureViewModel rvm = this.DataContext as RemoteCarMeasureViewModel;
            InitializeComponent();
            Rect rect = SystemParameters.WorkArea;
            this.MaxWidth = rect.Width + 14;
            this.MaxHeight = rect.Height + 14;
            this.WindowState = WindowState.Maximized;
            this.SeatNameTextBlock.Text = LoginUser.Name + "_" + LoginUser.Role.Name;
            this.SeatAttentionInfos = InfoExchange.Clone<List<SeatAttentionWeightModel>>(seatAttentionInfos);
            this.NewTaskGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 窗体加载后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeatMainForm_Loaded(object sender, RoutedEventArgs e)
        {
            ////程序版本
            //txtVer.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 3);
            //同步时间
            SynTime();
            RemoteCarMeasureViewModel rcmvm = (RemoteCarMeasureViewModel)this.DataContext;
            rcmvm.InitForm(SeatAttentionInfos, this.SeatMainForm);

            //启动日志同步
            //LogSync logSync = new LogSync();
            //logSync.StartSyncLog();
        }

        /// <summary>
        /// 最小化按钮事件
        /// </summary>
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 注册消息管理器
        /// </summary>
        public void RegisterMessenger()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister(this);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<string>(this, "RemoteCarMeasureViewModel", new Action<string>(ShowMsgInfo));
        }

        /// <summary>
        /// 弹出信息框显示信息内容
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMsgInfo(string msg)
        {
            MessageBox.Show(msg);
        }

        private void borderRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Pop_Parent.IsOpen)
                Pop_Parent.IsOpen = false;
            else
                Pop_Parent.IsOpen = true;
        }

        private void btnPop1_MouseEnter(object sender, MouseEventArgs e)
        {
            Pop_Child1.IsOpen = true;
            this.ClosePopup(Pop_Child1);
        }

        private void btnPop2_MouseEnter(object sender, MouseEventArgs e)
        {
            Pop_Child2.IsOpen = true;
            this.ClosePopup(Pop_Child2);
        }

        private void btnPop_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ClosePopup(null);
        }

        private void Pop_Child_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup pop = sender as Popup;
            pop.IsOpen = false;
        }

        private void ClosePopup(Popup curPop)
        {
            UIElementCollection children = gridMenu.Children;
            foreach (UIElement ui in children)
            {
                if (ui is Popup)
                {
                    Popup pop = ui as Popup;
                    if (curPop != null)
                    {
                        if (pop.Name != curPop.Name)
                            pop.IsOpen = false;
                    }
                    else
                    {
                        pop.IsOpen = false;
                    }
                }
            }
        }
        QueryDataView queryDataView;
        /// <summary>
        /// 数据查询
        /// </summary>
        private void btnQueryData_Click(object sender, RoutedEventArgs e)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.sjcx))
            {
                ShowMessage("无数据查询权限");
                return;
            }
            bool isOpen = formHClass.SetForeWindow("智能化远程集中计量管理系统(数据查询)");
            if (!isOpen)
            {
                if (queryDataView == null || queryDataView.Visibility == Visibility.Collapsed)
                {
                    queryDataView = new QueryDataView();
                    queryDataView.Owner = this.SeatMainForm;
                    QueryDataViewModel qdvm = queryDataView.DataContext as QueryDataViewModel;
                    qdvm.InitData(this.weightTabControl.SelectedIndex);
                    queryDataView.Closed += (s1, e1) => { queryDataView = null; FlushMemory(); };
                    queryDataView.ShowDialog();
                }
                else
                {
                    queryDataView.Activate();
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);
        /// <summary>
        /// 释放内存
        /// </summary>
        public void FlushMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                    #region 写日志
                    //LogModel log = new LogModel()
                    //{
                    //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    //    Direction = LogConstParam.Directions_Out,
                    //    FunctionName = "坐席_任务主窗体_内存释放",
                    //    Level = LogConstParam.LogLevel_Info,
                    //    Msg = "释放内存",
                    //    Origin = "汽车衡_" + LoginUser.Role.Name,
                    //    OperateUserName = LoginUser.Name
                    //};
                    //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务主窗体_内存释放",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "释放内存时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        WeightCurveView weightCurveView;
        /// <summary>
        /// 重量日志按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWeightLine_Click(object sender, RoutedEventArgs e)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.zlrz))
            {
                ShowMessage("无重量曲线查看权限");
                return;
            }
            bool isOpen = formHClass.SetForeWindow("智能化远程集中计量管理系统(重量曲线)");
            if (!isOpen)
            {
                if (weightCurveView == null || weightCurveView.Visibility == Visibility.Collapsed)
                {
                    weightCurveView = new WeightCurveView();
                    weightCurveView.Owner = this.SeatMainForm;
                    (weightCurveView.DataContext as WeightCurveViewModel).InitData(this.weightTabControl.SelectedIndex);
                    weightCurveView.Closed += (s1, e1) => { weightCurveView = null; FlushMemory(); };
                    weightCurveView.ShowDialog();
                }
                else
                {
                    weightCurveView.Activate();
                }
            }

        }
        TaskCountView taskCountView;
        /// <summary>
        /// 任务统计
        /// </summary>
        private void btnTaskCount_Click(object sender, RoutedEventArgs e)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.rwtj))
            {
                ShowMessage("无任务统计查看权限");
                return;
            }
            bool isOpen = formHClass.SetForeWindow("智能化远程集中计量管理系统(任务统计)");
            if (!isOpen)
            {
                if (taskCountView == null || taskCountView.Visibility == Visibility.Collapsed)
                {
                    taskCountView = new TaskCountView();
                    taskCountView.Owner = this.SeatMainForm;
                    taskCountView.Closed += (s1, e1) => { taskCountView = null; };
                    taskCountView.ShowDialog();
                }
                else
                {
                    taskCountView.Activate();
                }
            }

        }
        ModifyPasswordView modifyPasswordView;
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.xgmm))
            {
                ShowMessage("无修改密码权限");
                return;
            }
            bool isOpen = formHClass.SetForeWindow("智能化远程集中计量管理系统(修改密码)");
            if (!isOpen)
            {
                if (modifyPasswordView == null || modifyPasswordView.Visibility == Visibility.Collapsed)
                {
                    modifyPasswordView = new ModifyPasswordView();
                    modifyPasswordView.Owner = this.SeatMainForm;
                    modifyPasswordView.Closed += (s1, e1) => { modifyPasswordView = null; };
                    modifyPasswordView.ShowDialog();
                }
                else
                {
                    modifyPasswordView.Activate();
                }

            }

        }
        WeightClientAttentionUpholdView weightClientAttentionUpholdView;
        /// <summary>
        /// 称点关注按钮事件
        /// </summary>
        private void AttentionsButton_Click(object sender, RoutedEventArgs e)
        {
            //if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.gzzd))
            //{
            //    ShowMessage("无关注秤点权限");
            //    return;
            //}
            AttentionTypes at = AttentionTypes.CarMeasure;
            if (weightTabControl.SelectedIndex == 1)
            {
                at = AttentionTypes.TrainMeasure;
            }
            else if (weightTabControl.SelectedIndex == 2)
            {
                at = AttentionTypes.MoltenIron;
            }
            bool isOpen = formHClass.SetForeWindow("智能化远程集中计量管理系统(关注秤点)");
            if (!isOpen)
            {
                if (weightClientAttentionUpholdView == null)
                {
                    weightClientAttentionUpholdView = new WeightClientAttentionUpholdView(at);
                    weightClientAttentionUpholdView.Closed += (s1, e1) => { weightClientAttentionUpholdView = null; };
                    weightClientAttentionUpholdView.ShowDialog();
                }
                else
                {
                    weightClientAttentionUpholdView.Activate();
                    weightClientAttentionUpholdView.ShowDialog();
                }

                if (weightClientAttentionUpholdView!=null&&weightClientAttentionUpholdView.FormState == 1)
                {
                    if (at == AttentionTypes.CarMeasure)
                    {
                        OperateCarMeasure(weightClientAttentionUpholdView);
                    }
                    else if (at == AttentionTypes.MoltenIron)
                    {
                    }
                }
            }

        }
        WeightClientSendNoticeView weightClientSendNoticeView;
        /// <summary>
        /// 往秤体发送消息
        /// </summary>
        private void SendNoticeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!checkRemoteP.CheckIsAllowUse(CheckRemotePower.ButtonMemuEnum.fstz))
            {
                ShowMessage("无发送通知权限");
                return;
            }
            AttentionTypes at = AttentionTypes.CarMeasure;
            if (weightTabControl.SelectedIndex == 1)
            {
                at = AttentionTypes.TrainMeasure;
            }
            else if (weightTabControl.SelectedIndex == 2)
            {
                at = AttentionTypes.MoltenIron;
            }
            bool isOpen = formHClass.SetForeWindow("智能化远程集中计量管理系统(发送通知)");
            if (!isOpen)
            {
                if (weightClientSendNoticeView == null || weightClientSendNoticeView.Visibility == Visibility.Collapsed)
                {
                    weightClientSendNoticeView = new WeightClientSendNoticeView(at);
                    weightClientSendNoticeView.Closed += (s1, e1) => { weightClientSendNoticeView = null; };
                    weightClientSendNoticeView.ShowDialog();
                }
                else
                {
                    weightClientSendNoticeView.Activate();
                }
            }

        }
        /// <summary>
        /// 汽车衡终端关注维护后坐席主页面的处理
        /// </summary>
        /// <param name="wcau"></param>
        private void OperateCarMeasure(WeightClientAttentionUpholdView wcau)
        {
            RemoteCarMeasureViewModel rcmvm = this.DataContext as RemoteCarMeasureViewModel;
            var notCheckedAttionsIds = (from r in wcau.Attentions where r.IsChecked == false select r.equcode).ToList();
            var checkedAttentions = (from r in wcau.Attentions where r.IsChecked == true select r).ToList();
            var removeClients = (from r in rcmvm.CarWeighterClientInfos where notCheckedAttionsIds.Contains(r.ClientId) select r).ToList();
            foreach (var item in removeClients)
            {
                rcmvm.CarWeighterClientInfos.Remove(item);//在已有的称点信息中删除未选中的称点
            }
            foreach (var item in checkedAttentions)//将新添加的称点添加到集合里
            {
                var list = (from r in rcmvm.CarWeighterClientInfos where r.ClientId == item.equcode select r).ToList();
                if (list == null || list.Count == 0)
                {
                    rcmvm.CarWeighterClientInfos.Add(new WeighterClientModel() { ClientId = item.equcode, LeftLightState = LeftLightStates.None, RightLightState = RightLightStates.None, ClientState = WeighterStates.None, ClientName = item.equname, Weight = 0, LeftLine = Visibility.Hidden, RightLine = Visibility.Hidden, ClientConfigName = item.equcode + "_" + item.equname + ".xml" });
                }
            }
            rcmvm.CarWeighterClientInfos = rcmvm.CarWeighterClientInfos.OrderBy(r => Int32.Parse(r.ClientId.Substring(1))).ToList();
            //任务服务器断开然后重新连接
            SocketCls.Emit(ClientSendCmdEnum.logout, "");//断开与任务服务器的连接
            rcmvm.LoginOut();//与任务服务器断开连接后的相关业务处理
            rcmvm.relogin();//重新登录时，会将称点关注改变后的称点ID集合发送给任务服务器。
            ConfigSynchronous(checkedAttentions);
        }

        /// <summary>
        /// 配置文件同步
        /// </summary>
        private void ConfigSynchronous(List<SeatAttentionWeightModel> ClientInfos)
        {
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            foreach (var item in ClientInfos)
            {
                string clientConfigPath = System.IO.Path.Combine(basePath, item.equcode + ".xml");
                if (File.Exists(clientConfigPath))
                {
                    File.Delete(clientConfigPath);//删除掉 直接下载
                    ConfigFileDownLoad(clientConfigPath, item.versionnum, item.equcode, item.equname);
                    //configlist curConfig = XmlHelper.ReadXmlToObj<configlist>(clientConfigPath);
                    //var sysModules = (from r in curConfig.Modules where r.Code == IoConfigParam.Model_Code_SystemConfigs select r).ToList();
                    //if (sysModules.Count > 0)
                    //{
                    //    var sysConfigs = (from r in sysModules.First().SubModules where r.Code == IoConfigParam.Model_Code_SystemConfig select r).ToList();
                    //    if (sysConfigs.Count > 0)
                    //    {
                    //        var versionNum = (from r in sysConfigs.First().Params where r.Name == IoConfigParam.VersionNum select r).ToList().First().Value;
                    //        decimal curVersionNum = decimal.Parse(versionNum);
                    //        decimal serverNum = decimal.Parse(item.versionnum);
                    //        if (curVersionNum < serverNum)
                    //        {
                    //            ConfigFileDownLoad(clientConfigPath, item.versionnum, item.equcode, item.equname);
                    //        }
                    //    }
                    //}
                }
                else
                {
                    ConfigFileDownLoad(clientConfigPath, item.versionnum, item.equcode, item.equname);
                }
            }
        }

        /// <summary>
        /// 下载配置文件
        /// </summary>
        /// <param name="configFileName">配置文件名称(带路径)</param>
        /// <param name="versionNum">版本号</param>
        /// <param name="clientCode">称点编号</param>
        /// <param name="ClientName">称点名称</param>
        private void ConfigFileDownLoad(string configFileName, string versionNum, string clientCode, string ClientName)
        {
            string serviceUrl = ConfigurationManager.AppSettings["getEquParamInfo"].ToString();
            var param = new
            {
                versionnum = -1,
                equcode = clientCode,
                equname = ""//ClientName
            };
            string getUrl = string.Format(serviceUrl, "[" + JsonConvert.SerializeObject(param) + "]");
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);

            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String strResult = sr.ReadToEnd();
            sr.Close();
            List<EquModel> equModels = InfoExchange.DeConvert(typeof(List<EquModel>), strResult) as List<EquModel>;
            if (equModels.Count > 0)
            {
                try
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(configFileName))
                    {
                        sw.WriteLine(equModels.First().paraminfos.Replace("GBK", "UTF-8"));
                        sw.Close();
                    }

                }
                catch (Exception ex)
                {
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_任务主窗体_ConfigFileDownLoad",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "关注完称点，下载称点配置，拿到服务反馈的配置信息生成配置文件时异常:" + ex.Message,
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }

        /// <summary>
        /// 坐席主窗体"是否可用"属性改变触发的事件
        /// </summary>
        private void SeatMainForm_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                RemoteCarMeasureViewModel rcvm = this.DataContext as RemoteCarMeasureViewModel;
                if (rcvm.FormEnableSource == 1)
                {
                    //if (NewTaskGrid.Visibility == Visibility.Visible)
                    //{
                    //    //发送"退回任务"命令给任务服务器
                    //    SocketCls.Emit(SeatSendCmdEnum.backTask, rcvm.SelectedTask.ClientId);
                    //}
                    LoginOutSystem();
                }
                else if (rcvm.FormEnableSource == 2)
                {
                    //if (NewTaskGrid.Visibility == Visibility.Visible)
                    //{
                    //    //发送"退回任务"命令给任务服务器
                    //    SocketCls.Emit(SeatSendCmdEnum.backTask, rcvm.SelectedTask.ClientId);
                    //}
                    CloseSystem();
                }
            }
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        private void CloseSystem()
        {
            SocketCls.Emit(ClientSendCmdEnum.logout, "");//退出
            SocketCls.DisConnectServer();
            SocketCls.s.Dispose();
            #region 写日志
            LogModel log = new LogModel()
            {
                Origin = "汽车衡_" + LoginUser.Role.Name,
                FunctionName = "坐席主窗体_关闭系统",
                Level = LogConstParam.LogLevel_Info,
                Direction = LogConstParam.Directions_In,
                Msg = "发送Socket退出命令【" + ClientSendCmdEnum.logout + "】给任务服务器",
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = ClientSendCmdEnum.logout } },
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 系统注销
        /// </summary>
        private void LoginOutSystem()
        {
            SocketCls.Emit(ClientSendCmdEnum.logout, "");//退出
            SocketCls.DisConnectServer();
            SocketCls.s.Dispose();
            SocketCls.s = null;
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "坐席主窗体_系统注销",
                Level = LogConstParam.LogLevel_Info,
                Msg = "注销系统",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion

            System.Windows.Forms.Application.Restart();
            System.Threading.Thread.Sleep(500);
            Application.Current.Shutdown(0);

            //Login loginWindow = new Login();
            //loginWindow.Show();
            //isAllowClose = true;
            //this.Close();
        }

        /// <summary>
        /// 窗体键盘按下事件
        /// </summary>
        private void SeatMainForm_KeyDown(object sender, KeyEventArgs e)
        {

        }
        /// <summary>
        /// 提示信息展示
        /// </summary>
        /// <param name="inMsg"></param>
        private void ShowMessage(string inMsg)
        {
            MessageBox.Show(inMsg);
        }

        // 支持标题栏拖动  
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            bool isFirstChangeWindowsState = false;
            base.OnMouseLeftButtonDown(e);

            // 获取鼠标相对标题栏位置  
            Point position = e.GetPosition(gridTitleBar);

            // 如果鼠标位置在标题栏内，允许拖动  
            if (position.X >= 0 && position.X < gridTitleBar.ActualWidth && position.Y >= 0 && position.Y < gridTitleBar.ActualHeight)
            {
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1)
                {
                    this.DragMove();
                }
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                {
                    if (this.WindowState == WindowState.Normal)
                    {
                        isFirstChangeWindowsState = true;
                        this.WindowState = WindowState.Maximized;
                    }
                    if (isFirstChangeWindowsState == false)
                    {
                        if (this.WindowState == WindowState.Maximized)
                        {
                            this.WindowState = WindowState.Normal;
                        }
                    }

                }

            }

        }
        /// <summary>
        /// 换肤打开调色板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                RemoteCarMeasureViewModel rcvm = this.DataContext as RemoteCarMeasureViewModel;
                Point cP = e.GetPosition(null);
                ChangeSkinColor changeSkin = new ChangeSkinColor();
                changeSkin.Left = cP.X;
                changeSkin.ToolTip = cP.Y;
                changeSkin.SetValue(rcvm.BColor0);
                changeSkin.ShowDialog();
                string rtClore = changeSkin.rtColor;
                //logH.SaveLog("点击换肤，返回颜色结果为："+rtClore);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席主窗体_换肤打开调色板",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "点击换肤，返回颜色结果为：" + rtClore,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                DoRtColor(rtClore);

            }
        }
        /// <summary>
        /// 处理返回的颜色
        /// </summary>
        /// <param name="rtColor"></param>
        private void DoRtColor(string rtColor)
        {
            if (!string.IsNullOrEmpty(rtColor))
            {
                RemoteCarMeasureViewModel rcvm = this.DataContext as RemoteCarMeasureViewModel;
                rcvm.BColor0 = rtColor;
                rcvm.BColor1 = rtColor;
                string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
                string url = basePath + @"/ClientConfig/UserUseColor.txt";
                FileHelper.WriteText(url, rtColor);
            }

        }
        /// <summary>
        /// 同步数据库服务器时间
        /// </summary>
        private void SynTime()
        {
            DateTime dbTime = GetDbDate();
            ComputerInfoHelpClass.SynchroTime(dbTime);
        }
        /// <summary>
        /// 获取数据库时间
        /// </summary>
        /// <returns></returns>
        private DateTime GetDbDate()
        {
            DateTime rtDtime = DateTime.Now;
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["getOracleDateTime"].ToString();
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(serviceUrl, 10);
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                String strResult = sr.ReadToEnd();
                sr.Close();
                rtDtime = Convert.ToDateTime(strResult);
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_任务主窗体_获取数据库时间",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取数据库时间时异常:" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return rtDtime;
        }

        private void SeatMainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isAllowClose)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 主窗体激活时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeatMainForm_Activated(object sender, EventArgs e)
        {
            RemoteCarMeasureViewModel rcvm = this.DataContext as RemoteCarMeasureViewModel;
            rcvm.CheckTaskHandleViewActive();
        }

    }
}
