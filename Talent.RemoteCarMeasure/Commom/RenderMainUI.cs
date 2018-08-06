using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Data;
using Talent.Measure.DomainModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Talent.RemoteCarMeasure.Model;
using System.Configuration;
using System.Net;
using Talent.CommonMethod;
using Talent.Measure.DomainModel.CommonModel;
using Newtonsoft.Json;
using System.IO;
using Talent.Measure.DomainModel.ServiceModel;
using System.ComponentModel;
using System.Web;
using Talent.Measure.WPF.Remote;


namespace Talent.RemoteCarMeasure.Commom
{
    public class RenderMainUI
    {
        #region 属性
        private decimal _gridHeight;
        /// <summary>
        /// grid高度
        /// </summary>
        public decimal gridHeight
        {
            set { _gridHeight = value; }
            get { return _gridHeight; }
        }
        private Grid _gridReader;
        /// <summary>
        /// grid实例
        /// </summary>
        public Grid gridReader
        {
            set { _gridReader = value; }
            get { return _gridReader; }
        }
        private Grid _gridSupplier;
        /// <summary>
        /// grid实例
        /// </summary>
        public Grid gridSupplier
        {
            set { _gridSupplier = value; }
            get { return _gridSupplier; }
        }
        private Grid _gridMeasureWeight;
        /// <summary>
        /// grid实例
        /// </summary>
        public Grid gridMeasureWeight
        {
            set { _gridMeasureWeight = value; }
            get { return _gridMeasureWeight; }
        }
        private Grid _gridMeasure;
        /// <summary>
        /// grid实例
        /// </summary>
        public Grid gridMeasure
        {
            set { _gridMeasure = value; }
            get { return _gridMeasure; }
        }
        private List<RenderUI> _readerInfoList;
        /// <summary>
        /// 页面实例化字段
        /// </summary>
        public List<RenderUI> ReaderInfoList
        {
            set { _readerInfoList = value; }
            get { return _readerInfoList; }
        }
        private BullInfo _bindObject;
        /// <summary>
        /// 绑定数据源
        /// </summary>
        public BullInfo BindObject
        {
            set { _bindObject = value; }
            get { return _bindObject; }
        }

        private Popup _dropDownPop;
        /// <summary>
        /// 包含下拉框的pop
        /// </summary>
        public Popup dropDownPop
        {
            get { return _dropDownPop; }
            set { _dropDownPop = value; }
        }

        private Popup _dataViewPop;
        /// <summary>
        /// 包含下拉表格的pop
        /// </summary>
        public Popup dataViewPop
        {
            get { return _dataViewPop; }
            set { _dataViewPop = value; }
        }
        private List<ServiceBasicInfo> _basicInfos;
        /// <summary>
        /// 查询服务获取的基础信息
        /// </summary>
        public List<ServiceBasicInfo> basicInfos
        {
            get { return _basicInfos; }
            set { _basicInfos = value; }
        }
        private TextBox _curQueryUI;
        /// <summary>
        /// 当前查询信息的UI控件
        /// </summary>
        public TextBox curQueryUI
        {
            get { return _curQueryUI; }
            set { _curQueryUI = value; }
        }

        #endregion
        public RenderMainUI(decimal gridHeight, Grid gridReader, Grid gridSupplier, Grid gridMeasure, Grid gridMeasureWeight, BullInfo bindObject, List<RenderUI> readerInfoList, Popup dropDownPop, Popup dataViewPop)
        {
            this.gridHeight = gridHeight;
            this.gridReader = gridReader;
            this.BindObject = bindObject;
            this.ReaderInfoList = readerInfoList;
            this.gridSupplier = gridSupplier;
            this.gridMeasure = gridMeasure;
            this.gridMeasureWeight = gridMeasureWeight;
            this.dropDownPop = dropDownPop;
            this.dataViewPop = dataViewPop;
            this.dropDownPop.MouseLeave += Pop_MouseLeave;
            this.dataViewPop.MouseLeave += Pop_MouseLeave;
        }

        /// <summary>
        /// 鼠标离开Pop触发的事件
        /// </summary>
        void Pop_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup pop = sender as Popup;
            pop.IsOpen = false;
        }

        /// <summary>
        /// 获取textBlock控件
        /// </summary>
        /// <returns></returns>
        private TextBlock getLableTB()
        {
            Color color1 = (Color)ColorConverter.ConvertFromString("#FF7C7C7C");
            TextBlock tb1 = new TextBlock();
            tb1.Foreground = new SolidColorBrush(color1);
            tb1.FontSize = 16;
            tb1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            tb1.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tb1.FontWeight = System.Windows.FontWeights.Bold;
            return tb1;
        }
        /// <summary>
        /// 获取TextBox控件
        /// </summary>
        /// <param name="isFirstRow">返回的控件是否为grid中的第一行</param>
        /// <returns></returns>
        private TextBox getInputTB(bool isFirstRow)
        {
            TextBox tb2 = new TextBox();
            Color color = (Color)ColorConverter.ConvertFromString("#FF333333");
            tb2.Foreground = new SolidColorBrush(color);
            tb2.FontSize = 16;
            if (isFirstRow)
            {
                tb2.Margin = new System.Windows.Thickness(5, 10, 10, 5);
            }
            else
            {
                tb2.Margin = new System.Windows.Thickness(5, 5, 10, 5);
            }
            return tb2;
        }
        public bool SetRenderMainUI()
        {
            #region 计量业务信息控制
            if (gridReader != null && gridReader.Children != null)
            {
                gridReader.Children.Clear();
            }
            for (int i = gridReader.RowDefinitions.Count - 1; i > -1; i--)
            {
                gridReader.RowDefinitions.RemoveAt(i);
            }
            for (int i = gridReader.ColumnDefinitions.Count - 1; i > -1; i--)
            {
                gridReader.ColumnDefinitions.RemoveAt(i);
            }
            //增加判断 供方毛皮净 以及扣重 不再在动态列表显示……lt 2016-2-3 08:58:51……
            List<RenderUI> getBullInfo = ReaderInfoList.Where(p => p.aboutweight == 0 && p.isdisplay == 1
                && p.fieldname != "tareb" && p.fieldname != "grossb" && p.fieldname != "suttleb" && p.fieldname != "deduction").OrderBy(o => o.orderno).ToList<RenderUI>();
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new System.Windows.GridLength(70);
            ColumnDefinition cd2 = new ColumnDefinition();
            ColumnDefinition cd3 = new ColumnDefinition();
            cd3.Width = new System.Windows.GridLength(70);
            ColumnDefinition cd4 = new ColumnDefinition();
            gridReader.ColumnDefinitions.Add(cd1);
            gridReader.ColumnDefinitions.Add(cd2);
            gridReader.ColumnDefinitions.Add(cd3);
            gridReader.ColumnDefinitions.Add(cd4);
            int columnNum = 0;
            int rowNum = 0;//控件添加在哪一行 lt 2016-2-3 09:36:46……
            Color color = (Color)ColorConverter.ConvertFromString("#636363");  
            for (int i = 0; i <= getBullInfo.Count - 1; i++)
            {
                RowDefinition rd = new RowDefinition();
                if (getBullInfo.Count > 12)
                {
                    rd.Height = new System.Windows.GridLength(45);
                }
                else
                {
                    rd.Height = new System.Windows.GridLength(50);
                }
                gridReader.RowDefinitions.Add(rd);

                TextBlock tb1 = getLableTB();
                tb1.Text = getBullInfo[i].displayname;
                tb1.SetValue(Grid.RowProperty, rowNum);
                tb1.SetValue(Grid.ColumnProperty, columnNum);//0
                tb1.Foreground = new SolidColorBrush(color);
                gridReader.Children.Add(tb1);
                columnNum = columnNum + 1;
                bool isBold = ComHelpClass.CheckIsFontWeightBold(getBullInfo[i].fieldname);
                TextBox tb2 = (i == 0 ? getInputTB(true) : getInputTB(false));
                if (getBullInfo[i].quicksuggest == 1)
                {
                    tb2.KeyDown += tb2_KeyDown;
                }
                tb2.TextWrapping = System.Windows.TextWrapping.Wrap;
                tb2.Name = getBullInfo[i].fieldname;
                tb2.VerticalContentAlignment = VerticalAlignment.Center;
                tb2.IsReadOnly = getBullInfo[i].writeable==0?true:false;//0只读  1 可编辑
                tb2.SetBinding(TextBox.TextProperty, new Binding(getBullInfo[i].fieldname) { Source = BindObject, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                tb2.SetValue(Grid.RowProperty, rowNum);
                tb2.SetValue(Grid.ColumnProperty, columnNum);//1
                if(isBold)
                {
                    tb2.FontWeight = FontWeights.Bold;
                }
                gridReader.Children.Add(tb2);
                columnNum = columnNum + 1;
                bool checkIsOneRow =ComHelpClass.CheckIsOneRow(getBullInfo[i].fieldname);//文本框 是不是一行显示出来……lt 2016-2-3 09:17:49……
                if (checkIsOneRow)
                {
                    tb2.SetValue(Grid.ColumnSpanProperty, 3);
                    columnNum = 0;
                    rowNum = rowNum + 1;
                    continue;
                }
                //todo: lt 2016-1-27 19:10:44…… 两个一组 奇数时 不存在第二个  错误：大于索引退出 …… 
                if (i + 1 >= getBullInfo.Count)
                {
                    break;
                }
                checkIsOneRow = ComHelpClass.CheckIsOneRow(getBullInfo[i + 1].fieldname);//如果第一个不是 第二个是 则直接换行…… 2016-2-3 09:57:12
                if (checkIsOneRow)
                {
                    columnNum = 0;
                    rowNum = rowNum + 1;
                    continue;
                }
                isBold = ComHelpClass.CheckIsFontWeightBold(getBullInfo[i + 1].fieldname);
                TextBlock tb3 = getLableTB();
                tb3.Text = getBullInfo[i + 1].displayname;
                tb3.SetValue(Grid.RowProperty, rowNum);
                tb3.SetValue(Grid.ColumnProperty, columnNum);//2
                tb3.Foreground = new SolidColorBrush(color);                
                gridReader.Children.Add(tb3);
                columnNum = columnNum + 1;

                TextBox tb4 = (i == 0 ? getInputTB(true) : getInputTB(false));
                if (getBullInfo[i + 1].quicksuggest == 1)
                {
                    tb4.KeyDown += tb2_KeyDown;
                }
                tb4.TextWrapping = System.Windows.TextWrapping.Wrap;
                tb4.Name = getBullInfo[i + 1].fieldname;
                tb4.VerticalContentAlignment = VerticalAlignment.Center;
                tb4.IsReadOnly = getBullInfo[i + 1].writeable == 0 ? true : false;//0只读  1 可编辑
                tb4.SetBinding(TextBox.TextProperty, new Binding(getBullInfo[i + 1].fieldname) { Source = BindObject, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                //tb4.Text = "测试测试测试测试测试测试";
                tb4.SetValue(Grid.RowProperty, rowNum);
                tb4.SetValue(Grid.ColumnProperty, columnNum);//3
                if (isBold)
                {
                    tb4.FontWeight = FontWeights.Bold;
                }
                gridReader.Children.Add(tb4);
                columnNum = 0;
                rowNum = rowNum + 1;
                i++;

            }
            #endregion
            int isDisplay = 0;
            #region 供方信息控制
            //增加异常抛出  解决若不存在 供方信息时  则报 未将对象引用到实例的错误…… lt 2016-1-28 10:41:54 
            try
            {
                int tareBDis = ReaderInfoList.Where(p => p.fieldname == "tareb").FirstOrDefault().isdisplay;
                int grossbDis = ReaderInfoList.Where(p => p.fieldname == "grossb").FirstOrDefault().isdisplay;
                int suttlebDis = ReaderInfoList.Where(p => p.fieldname == "suttleb").FirstOrDefault().isdisplay;
                isDisplay = tareBDis * grossbDis * suttlebDis;
                //int isDisplay = ReaderInfoList.Where(p => p.fieldname == "tareb").FirstOrDefault().isdisplay * ReaderInfoList.Where(p => p.fieldname == "grossb").FirstOrDefault().isdisplay * ReaderInfoList.Where(p => p.fieldname == "suttleb").FirstOrDefault().isdisplay;
                if (isDisplay == 0)
                {
                    gridSupplier.Visibility = Visibility.Collapsed;
                }
                else
                {
                    gridSupplier.Visibility = Visibility.Visible;
                }
            }
            catch //(Exception ex)
            {
            }


            #endregion
            #region 称量信息控制
            //解决扣重未配置时的错误…… lt 2016-1-28 10:43:26
            try
            {
                isDisplay = 0;
                isDisplay = ReaderInfoList.Where(p => p.fieldname == "deduction").FirstOrDefault().isdisplay;//扣重是否显示
            }
            catch //(Exception ex)
            {

            }
            //isDisplay = ReaderInfoList.Where(p => p.fieldname == "deduction").FirstOrDefault().isdisplay;//扣重是否显示
            var getMinusDeduction = gridMeasureWeight.FindName("minusdeduction") as Border;
            var getTxtDeduction = gridMeasureWeight.FindName("txtdeduction") as TextBox;
            var getlblDeduction = gridMeasure.FindName("lbldeduction") as Label;
            if (isDisplay == 0)
            {
                if (getMinusDeduction != null)
                {
                    getMinusDeduction.Visibility = Visibility.Hidden;
                }
                if (getTxtDeduction != null)
                {
                    getTxtDeduction.Visibility = Visibility.Hidden;
                }
                if (getlblDeduction != null)
                {
                    getlblDeduction.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                if (getMinusDeduction != null)
                {
                    getMinusDeduction.Visibility = Visibility.Visible;
                }
                if (getTxtDeduction != null)
                {
                    getTxtDeduction.Visibility = Visibility.Visible;
                }
                if (getlblDeduction != null)
                {
                    getlblDeduction.Visibility = Visibility.Visible;
                }
            }
            #endregion
            return true;
        }
        
        /// <summary>
        /// 键盘按下事件
        /// </summary>
        void tb2_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsDown)
            {
                if (e.Key == Key.Enter)
                {
                    curQueryUI = sender as TextBox;
                    if (curQueryUI.Name == "taskcode")
                    {
                        GetBusinessAbortInfos();
                    }
                    else
                    {
                        //GetBasicInfoFromService();
                    }
                }
            }
        }

        /// <summary>
        /// 查询业务号对应的信息
        /// </summary>
        private void GetBusinessAbortInfos()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getBusinessNoAbortInfo"].ToString();
            string getUrl = string.Format(serviceUrl, curQueryUI.Text.Trim());
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(getBusinessAbortInfosCallback), request);
            #region 日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席任务处理窗体",
                Level = LogConstParam.LogLevel_Info,
                Msg = "开始从服务器查询业务号相关基础数据!",
                Origin = LoginUser.Role.Name,
                OperateUserName = LoginUser.Name,
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "URL", ParamValue = getUrl } }
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        /// <summary>
        /// 通过业务号查询相关基础信息的回调方法
        /// </summary>
        /// <param name="asyc"></param>
        public void getBusinessAbortInfosCallback(IAsyncResult asyc)
        {
            //以下代码未开发完
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                var taskCodeInfos = InfoExchange.DeConvert(typeof(List<TaskCodeModel>), strResult) as List<TaskCodeModel>;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席任务处理窗体",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务器查询基础数据成功!",
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = taskCodeInfos,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                if (taskCodeInfos != null && taskCodeInfos.Count > 0)
                {
                    gridReader.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        dataViewPop.PlacementTarget = curQueryUI;
                        dataViewPop.Placement = PlacementMode.Bottom;
                        DataGrid dg = dataViewPop.FindName("PopDataGrid") as DataGrid;
                        dg.ItemsSource = taskCodeInfos;
                        dataViewPop.IsOpen = true;
                    }));
                    //dataViewPop.PlacementTarget = curQueryUI;
                    //dataViewPop.Placement = PlacementMode.Bottom;
                    //dataViewPop.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席任务处理窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务查询基础信息失败！原因：" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 从服务查询信息
        /// </summary>
        private void GetBasicInfoFromService()
        {
            if (basicInfos != null)
            {
                basicInfos.Clear();
            }

            #region 测试时使用的代码
            //basicInfos = new List<ServiceBasicInfo>();
            //basicInfos.Add(new ServiceBasicInfo() { item = "测试信息 1" });
            //basicInfos.Add(new ServiceBasicInfo() { item = "测试信息 2" });
            //basicInfos.Add(new ServiceBasicInfo() { item = "测试信息 3" });
            //basicInfos.Add(new ServiceBasicInfo() { item = "测试信息 4" });
            //basicInfos.Add(new ServiceBasicInfo() { item = "测试信息 5" });
            //basicInfos.Add(new ServiceBasicInfo() { item = "测试信息 6" });
            //dropDownPop.PlacementTarget = curQueryUI;
            //dropDownPop.Width = 165;
            //dropDownPop.Height = 200;
            //dropDownPop.Placement = PlacementMode.Bottom;
            //ItemsControl dg = dropDownPop.FindName("dgTest") as ItemsControl;
            //dg.ItemsSource = basicInfos;
            //dropDownPop.IsOpen = true;
            #endregion

            string serviceUrl = ConfigurationManager.AppSettings["getServiceBasiceInfo"].ToString().Replace('$', '&');
            string getUrl = string.Format(serviceUrl, curQueryUI.Name, HttpUtility.UrlEncode(curQueryUI.Text.Trim()));
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(getBasicInfoCallback), request);
            #region 日志
            //LogModel log = new LogModel()
            //{
            //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //    Direction = LogConstParam.Directions_Out,
            //    FunctionName = "坐席任务处理窗体",
            //    Level = LogConstParam.LogLevel_Info,
            //    Msg = "开始从服务器查询基础数据!",
            //    Origin = LoginUser.Role.Name,
            //    OperateUserName = LoginUser.Name,
            //    ParamList = new List<DataParam>() { new DataParam() { ParamName = "URL", ParamValue = getUrl } }
            //};
            //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        /// <summary>
        /// 通过服务获取业务信息的回调方法
        /// </summary>
        /// <param name="asyc"></param>
        public void getBasicInfoCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc); 
                basicInfos = InfoExchange.DeConvert(typeof(List<ServiceBasicInfo>), strResult) as List<ServiceBasicInfo>;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席任务处理窗体",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "从服务器查询基础数据成功!",
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = basicInfos,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                if (basicInfos != null && basicInfos.Count > 0)
                {
                    gridReader.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        dropDownPop.PlacementTarget = curQueryUI;
                        dropDownPop.Placement = PlacementMode.Bottom;
                        ItemsControl dg = dropDownPop.FindName("dgTest") as ItemsControl;
                        dg.ItemsSource = basicInfos;
                        dropDownPop.IsOpen = true;
                    }));
                }
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席任务处理窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务查询基础信息失败！原因：" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
    }
}
