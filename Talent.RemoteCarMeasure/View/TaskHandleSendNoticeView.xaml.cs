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
using Talent.Measure.WPF.Log;
using Talent.ClientCommMethod;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// 称点关注维护的交互逻辑
    /// </summary>
    public partial class TaskHandleSendNoticeView : Window
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
        /// 秤点名称
        /// </summary>
        string clientName = string.Empty;
        /// <summary>
        /// 秤点编码
        /// </summary>
        string clientCode = string.Empty;
        /// <summary>
        /// 日志记录
        /// </summary>
        //LogsHelpClass logH = new LogsHelpClass();
        public TaskHandleSendNoticeView(string cName,string cCode)
        {
            clientName = cName;
            clientCode = cCode;
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.clientNameTxt.Text = clientName;
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
            string infos = this.msgTxt.Text.ToString().Trim();
            if (string.IsNullOrEmpty(infos))
            {
                //logH.SaveLog("随车通知，系统提示：消息内容不允许为空");
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_向称点发送通知信息_确定按钮事件",
                    Level = LogConstParam.LogLevel_Warning,
                    Msg = "随车通知，系统提示:消息内容不允许为空",
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                MessageBox.Show("消息内容不允许为空");
                return;
            }
            SendInfoToClient(infos);
            this.Close();
        }

        /// <summary>
        /// 往称点发送信息
        /// </summary>
        private void SendInfoToClient(string infos)
        {
            try
            {
                //logH.SaveLog("随车通知:" + infos);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_向称点发送通知信息_发送信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "随车通知:" + infos,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                int unm = CommonMethod.CommonMethod.GetRandom();
                var para = new
                {
                    clientid = clientCode,
                    cmd = ParamCmd.UserNotice,
                    msg = infos+"~$",
                    msgid = unm
                };
                SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
            }
            catch (Exception ex)
            {
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_向称点发送通知信息_发送信息",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "往秤体发送通知错误:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    IsDataValid = LogConstParam.DataValid_Ok,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }       
        } 

        /// <summary>
        /// 取消按钮事件
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //logH.SaveLog("用户退出随车通知");
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_向称点发送通知信息_取消按钮事件",
                Level = LogConstParam.LogLevel_Info,
                Msg = "用户退出随车通知",
                Origin = LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            FormState = 2;
            this.Close();
        }
    }
}
