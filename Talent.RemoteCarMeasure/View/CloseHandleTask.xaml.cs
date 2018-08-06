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
using Talent.ClientCommMethod;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.Measure.WPF.Remote;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// CloseHandleTask.xaml 的交互逻辑
    /// </summary>
    public partial class CloseHandleTask : Window
    {
        private int formState = 0;
        /// <summary>
        /// 窗体状态(0:关闭窗体;1:回退任务;2:转发他人;)
        /// </summary>
        public int FormState
        {
            get { return formState; }
            set { formState = value; }
        }
        private string clientId;

        public CloseHandleTask(string weightClientId)
        {
            InitializeComponent();
            this.clientId = weightClientId;
        }

        private List<SeatAttentionWeightModel> seats;
        /// <summary>
        ///  坐席集合
        /// </summary>
        public List<SeatAttentionWeightModel> Seats
        {
            get { return seats; }
            set { seats = value; }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            FormState = 0;
            SocketCls.listenEvent -= SocketCls_listenEvent;
            this.Hide();
        }

        /// <summary>
        /// 回退任务按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopWeightButton_Click(object sender, RoutedEventArgs e)
        {
            FormState = 1;
            SocketCls.listenEvent -= SocketCls_listenEvent;//回退任务时将事件移除，防止窗体隐藏 接收别的信息……lt 2016-2-18 08:50:53……
            this.Hide();
        }

        /// <summary>
        /// 转发他人按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void toOterSeatButton_Click(object sender, RoutedEventArgs e)
        //{
        //    FormState = 2;
        //    this.Close();
        //}

        private void toOterSeatButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Pop_Other.IsOpen)
                Pop_Other.IsOpen = false;
            else
                Pop_Other.IsOpen = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetSeatList();
            //SocketCls.listenEvent += SocketCls_listenEvent;
            this.itemsControlMenu.ItemsSource = Seats;
        }

        /// <summary>
        /// 获取称点被关注的所有坐席信息
        /// </summary>
        private void GetSeatList()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getEqucodeSeat"].ToString();
            var param = new
            {
                clientid = this.clientId
            };
            string getUrl = string.Format(serviceUrl, "[" + JsonConvert.SerializeObject(param) + "]");
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(GetSeatListCallback), request);
        }

        /// <summary>
        /// 获取坐席信息回调函数
        /// </summary>
        /// <param name="asyc"></param>
        private void GetSeatListCallback(IAsyncResult asyc)
        {
            try
            {
                List<SeatAttentionWeightModel> equModels;
                string strResult = ComHelpClass.ResponseStr(asyc);
                equModels = InfoExchange.DeConvert(typeof(List<SeatAttentionWeightModel>), strResult) as List<SeatAttentionWeightModel>;
                Seats = (from r in equModels
                         where r.seatid != LoginUser.Role.Code
                         select r).OrderBy(c => c.seatid).ToList();
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => { this.itemsControlMenu.ItemsSource = Seats; }));
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席任务处理_关闭窗体_获取坐席信息回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取坐席信息失败！原因：" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        #region Socket回调事件
        public void SocketCls_listenEvent(object sender, CallBackEventArgs e)
        {
            //Console.WriteLine("**************得到命令:" + e.EventName + "__" + e.Message);
            switch (e.EventName)
            {
                case "reply":
                    TransmissionResult(e.Message);
                    break;
            }
        }

        /// <summary>
        /// 转发结果（ok/fail）
        /// </summary>
        /// <param name="p"></param>
        private void TransmissionResult(string msg)
        {
            if (msg.Equals("ok"))
            {
                SocketCls.listenEvent -= SocketCls_listenEvent;
                CloseTaskWindow.Dispatcher.Invoke(new Action(() =>
                {
                    CloseTaskWindow.FormState = 2;
                    CloseTaskWindow.Close();
                }));
            }
            else
            {
                System.Windows.MessageBox.Show("转发坐席失败,请确保此坐席已登录并处于【空闲】");
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席任务处理_关闭窗体_转发坐席后的反馈结果",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "转发坐席失败,请确保此坐席已登录并处于【空闲】",
                    Data = msg,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        #endregion

        /// <summary>
        /// 选择坐席后转发他人业务实现
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            var list = (from r in Seats where r.seatname == b.Content.ToString().Trim() select r).ToList();
            if (list.Count > 0)
            {
                var data = new
                {
                    clientid = this.clientId,
                    newseatid = list.First().seatid
                };
                string paramJson = Talent.CommonMethod.InfoExchange.ToJson(data);
                try
                {
                    SocketCls.Emit(SeatlistenCmdEnum.redirectTask, paramJson);
                    this.Focus();
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        Origin = LoginUser.Role.Name,
                        FunctionName = "坐席任务处理窗体_关闭窗体_任务转发_选择坐席后转发他人业务实现",
                        Level = LogConstParam.LogLevel_Info,
                        Direction = LogConstParam.Directions_Out,
                        Msg = "发送转发任务命令【" + SeatlistenCmdEnum.redirectTask + "】给任务服务器",
                        Data = data,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "clientid", ParamValue = this.clientId }, new DataParam() { ParamName = "newseatid", ParamValue = list.First().equcode } },
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                    //转发之后自动关闭，去掉等待……
                    CloseTaskWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        CloseTaskWindow.FormState = 2;
                        CloseTaskWindow.Close();
                    }));
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("任务转发失败,请确认与任务服务器连接是否正常!");
                    #region 写日志
                    LogModel log = new LogModel()
                    {
                        Origin = LoginUser.Role.Name,
                        FunctionName = "坐席任务处理窗体_关闭窗体_任务转发_选择坐席后转发他人业务实现",
                        Level = LogConstParam.LogLevel_Error,
                        Direction = LogConstParam.Directions_Out,
                        Msg = "发送转发任务命令【" + SeatlistenCmdEnum.redirectTask + "】给任务服务器失败.",
                        Data = data,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "clientid", ParamValue = this.clientId }, new DataParam() { ParamName = "newseatid", ParamValue = list.First().equcode } },
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                    #endregion
                }
            }
        }
    }
}
