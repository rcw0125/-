using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.Measure.WPF.Remote;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// 称点关注维护的交互逻辑
    /// </summary>
    public partial class WeightClientAttentionUpholdView : Window
    {
        private int formState = 0;
        /// <summary>
        /// 窗体状态(0:关闭窗体;1:确定;2:取消;)
        /// </summary>
        public int FormState
        {
            get { return formState; }
            set { formState = value; }
        }
        /// <summary>
        /// 关注类型(汽车衡、铁水衡.....)
        /// </summary>
        private AttentionTypes attentionTypes;

        private IList<SeatAttentionWeightModel> attentions;
        /// <summary>
        /// 关注的称点集合
        /// </summary>
        public IList<SeatAttentionWeightModel> Attentions
        {
            get { return attentions; }
            set
            {
                attentions = value;
            }
        }

        public WeightClientAttentionUpholdView(AttentionTypes attentionTypes)
        {
            this.attentionTypes = attentionTypes;
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.attentionTypes == AttentionTypes.CarMeasure)
            {
                this.Title = "汽车衡关注维护";
                this.groupBox.Header = "汽车衡列表";
                InitCarMeasureDataGrid();
                BindingCarMeasureClients();
            }
            else if (this.attentionTypes == AttentionTypes.MoltenIron)
            {

            }

        }

        /// <summary>
        /// 构造汽车衡DataGrid
        /// </summary>
        private void InitCarMeasureDataGrid()
        {
            //空白列
            DataGridTextColumn spaceColumn = new DataGridTextColumn();
            spaceColumn.IsReadOnly = true;
            spaceColumn.Width = 20;

            DataGridTextColumn idColumn = new DataGridTextColumn();
            idColumn.Header = "称点Id";
            idColumn.IsReadOnly = true;
            idColumn.Width = 120;
            //idColumn.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
            //idColumn.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            idColumn.Binding = new Binding("equcode") { Mode = BindingMode.TwoWay };

            DataGridTextColumn clientNameColumn = new DataGridTextColumn();
            clientNameColumn.Header = "称点名称";
            clientNameColumn.IsReadOnly = true;
            clientNameColumn.MinWidth = 120;
            //clientNameColumn.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
            //clientNameColumn.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            clientNameColumn.Binding = new Binding("equname") { Mode = BindingMode.TwoWay };

            DataGridCheckBoxColumn isAttentionColumn = new DataGridCheckBoxColumn();
            isAttentionColumn.Header = "是否关注";
            isAttentionColumn.IsReadOnly = false;
            isAttentionColumn.Width = 100;
            isAttentionColumn.SetValue(Grid.IsEnabledProperty, true);
            isAttentionColumn.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
            isAttentionColumn.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            isAttentionColumn.Binding = new Binding("IsChecked") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            this.DataGrid.Columns.Add(spaceColumn);
            this.DataGrid.Columns.Add(isAttentionColumn);
            this.DataGrid.Columns.Add(clientNameColumn);
            this.DataGrid.Columns.Add(idColumn);
        }

        /// <summary>
        /// 从服务器获取坐席和称点关注关系信息
        /// </summary>
        private void BindingCarMeasureClients()
        {
            #region 作废代码
            //服务暂无，暂时构造临时数据
            //IList<WeightAttentionModel> attentions = new List<WeightAttentionModel>();
            //WeightAttentionModel m1 = new WeightAttentionModel() { ClientId = "a001", ClientName = "1号衡器", IsChecked = true };
            //WeightAttentionModel m2 = new WeightAttentionModel() { ClientId = "a002", ClientName = "2号衡器", IsChecked = true };
            //WeightAttentionModel m3 = new WeightAttentionModel() { ClientId = "a003", ClientName = "3号衡器", IsChecked = true };
            //WeightAttentionModel m4 = new WeightAttentionModel() { ClientId = "a004", ClientName = "4号衡器", IsChecked = true };
            //WeightAttentionModel m5 = new WeightAttentionModel() { ClientId = "a005", ClientName = "5号衡器", IsChecked = true };
            //WeightAttentionModel m6 = new WeightAttentionModel() { ClientId = "a006", ClientName = "6号衡器", IsChecked = true };
            //WeightAttentionModel m7 = new WeightAttentionModel() { ClientId = "a007", ClientName = "7号衡器", IsChecked = true };
            //WeightAttentionModel m8 = new WeightAttentionModel() { ClientId = "a008", ClientName = "8号衡器", IsChecked = true };
            //WeightAttentionModel m9 = new WeightAttentionModel() { ClientId = "a009", ClientName = "9号衡器", IsChecked = false };
            //WeightAttentionModel m10 = new WeightAttentionModel() { ClientId = "a0010", ClientName = "10号衡器", IsChecked = false };
            //attentions.Add(m1);
            //attentions.Add(m2);
            //attentions.Add(m3);
            //attentions.Add(m4);
            //attentions.Add(m5);
            //attentions.Add(m6);
            //attentions.Add(m7);
            //attentions.Add(m8);
            //attentions.Add(m9);
            //attentions.Add(m10);
            //DataGrid.ItemsSource = attentions;
            #endregion
            GetWeighterClientInfos();
        }

        /// <summary>
        /// 获取关注的汽车衡称点信息
        /// </summary>
        private void GetWeighterClientInfos()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getSeatClient"].ToString();
            string seatType = string.Empty;
            if (this.attentionTypes == AttentionTypes.CarMeasure)
            {
                seatType = "RC";
            }
            else if (this.attentionTypes == AttentionTypes.TrainMeasure)
            {
                seatType = "RT";
            }
            else if (this.attentionTypes == AttentionTypes.MoltenIron)
            {
                seatType = "RI";
            }
            var param = new
            {
                seatname = LoginUser.Role.Name,
                seatid = LoginUser.Role.Code,
                seattype = seatType
            };
            string getUrl = string.Format(serviceUrl, "[" + JsonConvert.SerializeObject(param) + "]");
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(GetWeighterClientInfoCallback), request);
        }

        /// <summary>
        /// 获取关注的汽车衡称点信息回调函数
        /// </summary>
        /// <param name="asyc"></param>
        private void GetWeighterClientInfoCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc); 
                var list = InfoExchange.DeConvert(typeof(List<SeatAttentionWeightModel>), strResult) as List<SeatAttentionWeightModel>;
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (this.attentionTypes == AttentionTypes.CarMeasure)
                    {
                        Attentions = (from r in list where r.seattype == "RC" select r).OrderBy(c => c.equcode).ToList();
                    }
                    else if (this.attentionTypes == AttentionTypes.TrainMeasure)
                    {
                        Attentions = (from r in list where r.seattype == "RT" select r).OrderBy(c => c.equcode).ToList();
                    }
                    else if (this.attentionTypes == AttentionTypes.MoltenIron)
                    {
                        Attentions = (from r in list where r.seattype == "RI" select r).OrderBy(c => c.equcode).ToList();
                    }
                    DataGrid.ItemsSource = attentions;
                }));
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_关注称点维护_获取关注的汽车衡称点信息回调方法",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "获取坐席关注的称点信息成功!",
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = Attentions,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    ParamList = new List<DataParam>() 
                    { 
                        new DataParam() { ParamName = "seatname", ParamValue = LoginUser.Role.Name },
                        new DataParam(){ParamName = "seatid", ParamValue = LoginUser.Role.Code}
                    }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_关注称点维护_获取关注的汽车衡称点信息回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取坐席关注的称点信息失败！原因：" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    ParamList = new List<DataParam>() 
                    { 
                        new DataParam(){ ParamName = "seatname", ParamValue = LoginUser.Role.Name },
                        new DataParam(){ ParamName = "seatid", ParamValue = LoginUser.Role.Code}
                    }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }


        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            FormState = 0;
            this.Close();
        }

        /// <summary>
        /// 确定按钮事件
        /// </summary>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            FormState = 1;
            if (this.attentionTypes == AttentionTypes.CarMeasure)
            {
                this.Attentions = DataGrid.ItemsSource as List<SeatAttentionWeightModel>;
                SaveAttentionClient();
            }
            else if (this.attentionTypes == AttentionTypes.MoltenIron)
            {
                //铁水衡对象集合
            }
        }

        /// <summary>
        /// 保存坐席关注的称点信息
        /// </summary>
        private void SaveAttentionClient()
        {
            string serviceUrl = ConfigurationManager.AppSettings["saveSeatClient"].ToString();
            var param = (from r in this.Attentions.Where(c => c.IsChecked).ToList()
                         select new
                         {
                             seatname = r.seatname,
                             seatid = r.seatid,
                             clientid = r.equcode,
                             seattype = r.seattype,
                             seatright = r.seatright
                         }).ToList();
            string getUrl = string.Format(serviceUrl, JsonConvert.SerializeObject(param));
            HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, JsonConvert.SerializeObject(param), "");
            request.BeginGetResponse(new AsyncCallback(SaveAttentionClientCallback), request);
        }

        /// <summary>
        /// 保存坐席关注的称点信息回调方法
        /// </summary>
        /// <param name="asyc"></param>
        private void SaveAttentionClientCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc); 
                var attionCallBack = InfoExchange.DeConvert(typeof(LoginServiceModel), strResult) as LoginServiceModel;
                if (!attionCallBack.success)
                {
                    MessageBox.Show("数据保存失败!");
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => { this.Close(); }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据保存失败!原因:" + ex.Message);
            }
        }

        /// <summary>
        /// 取消按钮事件
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            FormState = 2;
            this.Close();
        }
        /// <summary>
        /// 选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                try
                {
                    bool isCheck = (e.AddedItems[0] as SeatAttentionWeightModel).IsChecked;
                    (e.AddedItems[0] as SeatAttentionWeightModel).IsChecked = !isCheck;
                }
                catch //(Exception ex)
                {

                }
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Attentions.Count;i++ )
            {
                Attentions[i].IsChecked = true;
            }
        }

        /// <summary>
        /// 全不选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Attentions.Count; i++)
            {
                Attentions[i].IsChecked = false;
            }
        }
        /// <summary>
        /// 窗体拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
    }
}
