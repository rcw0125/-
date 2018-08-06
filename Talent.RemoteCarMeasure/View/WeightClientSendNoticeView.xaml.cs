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
using Talent.ClientCommonLib;
using Talent.RemoteCarMeasure.Commom;
using Talent.Measure.WPF;
using Talent.Measure.WPF.Remote;
using Talent.Measure.WPF.Log;
using Talent.ClientCommMethod;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// 称点关注维护的交互逻辑
    /// </summary>
    public partial class WeightClientSendNoticeView : Window
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
        /// <summary>
        /// 日志记录
        /// </summary>
        LogsHelpClass logH = new LogsHelpClass();
        public WeightClientSendNoticeView(AttentionTypes attentionTypes)
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
                //this.Title = "汽车衡关注维护";
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
                var oldlist = InfoExchange.DeConvert(typeof(List<SeatAttentionWeightModel>), strResult) as List<SeatAttentionWeightModel>;
                var list = GetNewList(oldlist);
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
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_发送通知窗体_获取关注的汽车衡称点信息回调函数",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取坐席关注的称点信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_"+LoginUser.Role.Name,
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
            //logH.SaveLog("关闭往秤体发送通知");
            #region 日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_In,
                FunctionName = "坐席_发送通知窗体_关闭窗体",
                Level = LogConstParam.LogLevel_Info,
                Msg = "关闭往秤体发送通知",
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name,
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            FormState = 0;
            this.Close();
        }

        /// <summary>
        /// 确定按钮事件
        /// </summary>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            FormState = 1;
            string infos = this.msgTxt.Text.ToString().Trim();
            if (string.IsNullOrEmpty(infos))
            {
                //logH.SaveLog("秤体发送通知，系统提示：消息内容不允许为空");
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_发送通知窗体_确定按钮事件",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "秤体发送通知，系统提示：消息内容不允许为空",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                MessageBox.Show("消息内容不允许为空");
                return;
            }
            if (this.attentionTypes == AttentionTypes.CarMeasure)
            {
                this.Attentions = DataGrid.ItemsSource as List<SeatAttentionWeightModel>;
                SendInfoToClient(infos);
            }
            else if (this.attentionTypes == AttentionTypes.MoltenIron)
            {
                //铁水衡对象集合
            }
            this.Close();
        }

        /// <summary>
        /// 往称点发送信息
        /// </summary>
        private void SendInfoToClient(string infos)
        {
            int succount = 0;
            for (int i = 0; i < Attentions.Count;i++ )
            {
                SeatAttentionWeightModel attion = Attentions[i];
                if(attion.IsChecked)//选中
                {
                    try
                    {
                        int unm = CommonMethod.CommonMethod.GetRandom();
                        var para = new
                        {
                            clientid = attion.equcode,
                            cmd = ParamCmd.UserNotice,
                            msg = infos,
                            msgid = unm
                        };
                        SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
                        //logH.SaveLog("通知内容：" + infos + "  秤体：" + attion.equname);
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            FunctionName = "坐席_发送通知窗体_往称点发送信息",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = attion.seatname + "坐席往秤体发送通知",
                            Origin = "汽车衡_"+LoginUser.Role.Name,
                            Data = para,
                            IsDataValid = LogConstParam.DataValid_Ok,
                            ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                            OperateUserName = LoginUser.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        succount = succount + 1;
                    }
                    catch (Exception ex)
                    {
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            FunctionName = "坐席_发送通知窗体_往称点发送信息",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = attion.seatname + "坐席往秤体发送通知发送错误:"+ex.Message,
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                            Data = "",
                            IsDataValid = LogConstParam.DataValid_Ok,
                            ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                            OperateUserName = LoginUser.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    } 
                }
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
        /// 清空秤体通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearInfosButton_Click(object sender, RoutedEventArgs e)
        {
            string eqNameStr = string.Empty;
            List<string> selectEqucode = GetSelectEqucode(out eqNameStr);     
            if (selectEqucode.Count > 0)
            {
                if (MessageBox.Show("您确定要清空" + eqNameStr + "的通知？", "系统提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //logH.SaveLog("系统提示：您确定要清空" + eqNameStr + "的通知，用户选择确定");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_发送通知窗体_清空秤体通知",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "系统提示：您确定要清空" + eqNameStr + "的通知，用户选择确定",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    SendClearNotic(selectEqucode);
                    this.Close();
                }
                else
                {
                    //logH.SaveLog("系统提示：您确定要清空" + eqNameStr + "的通知，用户选择取消");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_Out,
                        FunctionName = "坐席_发送通知窗体_清空秤体通知",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "系统提示：您确定要清空" + eqNameStr + "的通知，用户选择取消",
                        Origin = "汽车衡_" + LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
            else {
                //logH.SaveLog("清空通知：系统提示：请先选择要清空的衡器");
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_In,
                    FunctionName = "坐席_发送通知窗体_清空秤体通知",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "清空通知：系统提示：请先选择要清空的衡器",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                MessageBox.Show("请先选择要清空的衡器");
            }
        }
        /// <summary>
        /// 获取选中的秤体编码集合
        /// </summary>
        /// <returns></returns>
        private List<string> GetSelectEqucode(out string eqNameStr)
        {
            List<string> rtList = new List<string>();
            eqNameStr = string.Empty;
            int succount = 0;
            for (int i = 0; i < Attentions.Count; i++)
            {
                SeatAttentionWeightModel attion = Attentions[i];
                if (attion.IsChecked)//选中
                {
                    try
                    {
                        rtList.Add(attion.equcode);
                        eqNameStr = eqNameStr+","+attion.equname ;
                        succount = succount + 1;
                    }
                    catch //(Exception ex)
                    {
                     
                    }
                }
            }
            if(!string.IsNullOrEmpty(eqNameStr))
            {
                eqNameStr = eqNameStr.Substring(1);
            }
            return rtList;
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="sEquecode"></param>
        private void SendClearNotic(List<string> sEquecode)
        {
            for (int i = 0; i < Attentions.Count; i++)
            {
                SeatAttentionWeightModel attion = Attentions[i];
                if (attion.IsChecked)//选中
                {
                    try
                    {
                        int unm = CommonMethod.CommonMethod.GetRandom();
                        var para = new
                        {
                            clientid = attion.equcode,
                            cmd = ParamCmd.UserNotice,
                            msg = "ClearNoticeInfos",
                            msgid = unm
                        };
                        SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            FunctionName = "坐席_发送通知窗体_发送清空通知",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = attion.seatname + "坐席往秤体发送清空通知",
                            Origin = "汽车衡_"+LoginUser.Role.Name,
                            Data = para,
                            IsDataValid = LogConstParam.DataValid_Ok,
                            ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                            OperateUserName = LoginUser.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        #region 写日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_Out,
                            FunctionName = "坐席_发送通知窗体_发送清空通知",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = attion.seatname + "坐席往秤体发送清空通知:" + ex.Message,
                            Origin = LoginUser.Role.Name,
                            Data = "",
                            IsDataValid = LogConstParam.DataValid_Ok,
                            ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                            OperateUserName = LoginUser.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                    }
                }
            }
        }
        /// <summary>
        /// 当前选中的行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                try {
                    bool isCheck = (e.AddedItems[0] as SeatAttentionWeightModel).IsChecked;
                    (e.AddedItems[0] as SeatAttentionWeightModel).IsChecked = !isCheck;
                }
                catch //(Exception ex)
                { 
                
                } 
            }
        }
        /// <summary>
        /// 全部默认未选中
        /// </summary>
        /// <param name="inList"></param>
        /// <returns></returns>
        private List<SeatAttentionWeightModel> GetNewList(List<SeatAttentionWeightModel> inList)
        {
            List<SeatAttentionWeightModel> rtList = new List<SeatAttentionWeightModel>();
            rtList = inList;
            for (int i = 0; i < rtList.Count;i++ )
            {
                rtList[i].IsChecked = false;
            }
            return rtList;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Attentions.Count; i++)
            {
                Attentions[i].IsChecked = false;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Attentions.Count; i++)
            {
                Attentions[i].IsChecked = true;
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
