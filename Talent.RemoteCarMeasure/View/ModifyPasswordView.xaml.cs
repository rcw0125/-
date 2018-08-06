using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.ClientCommonLib;
using Talent.ClientCommonLib.Controls;
using Talent.Measure.DomainModel.CommonModel;
using Talent.CommonMethod;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Talent.Measure.DomainModel;
using Talent.Measure.WPF.Remote;
using Talent.Measure.WPF.Log;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// ModifyPasswordView.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyPasswordView : Only_WindowBase
    {
        public ModifyPasswordView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void ModifyPasswordView_Loaded(object sender, RoutedEventArgs e)
        {
            this.userName.Text = LoginUser.Name;
        }

        /// <summary>
        /// 确定按钮事件
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ValidateForm();
            if (string.IsNullOrEmpty(ErrMsgLabel.Content.ToString().Trim()))//错误信息为空
            {
                string serviceUrl = ConfigurationManager.AppSettings["updatePassword"].ToString().Replace('$', '&');
                string getUrl = string.Format(serviceUrl, LoginUser.Code, LoginUser.Password, this.newPasswordBox.Password.Trim());
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                request.BeginGetResponse(new AsyncCallback(GetUpdatePasswordCallback), request);
            }
        }
        private void GetUpdatePasswordCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                MeasureServiceModel mServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_修改密码_保存用户新密码信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "密码修改调用服务完成!",
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = strResult,
                    IsDataValid = LogConstParam.DataValid_Ok
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.ErrMsgLabel.Content = mServiceModel.msg;
                }));
                if (mServiceModel.success)
                {
                    //logH.SaveLog("修改密码成功");
                    #region 日志
                    LogModel sLog = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_OutIn,
                        FunctionName = "坐席_修改密码_保存用户新密码信息",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "修改密码成功!",
                        Origin = LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(sLog));
                    #endregion
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Close();
                    }));
                }
                else
                {
                    //logH.SaveLog("修改密码失败：" + mServiceModel.msg);
                    #region 日志
                    LogModel sLog = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_OutIn,
                        FunctionName = "坐席_修改密码_保存用户新密码信息",
                        Level = LogConstParam.LogLevel_Warning,
                        Msg = "修改密码失败：" + mServiceModel.msg,
                        Origin = LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(sLog));
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_修改密码_保存用户新密码信息",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "用户密码修改调用服务保存新用户信息出现错误:" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }
        /// <summary>
        /// 验证窗体
        /// </summary>
        private void ValidateForm()
        {
            do
            {
                if (string.IsNullOrEmpty(this.passwordBox.Password.Trim()))
                {
                    this.ErrMsgLabel.Content = "请输入【当前密码】!";
                    this.passwordBox.Focus();
                    break;
                }
                if (!this.passwordBox.Password.Trim().Equals(LoginUser.Password))
                {
                    this.ErrMsgLabel.Content = "【当前密码】输入不正确!";
                    this.passwordBox.Focus();
                    break;
                }
                if (string.IsNullOrEmpty(this.newPasswordBox.Password.Trim()))
                {
                    this.ErrMsgLabel.Content = "请输入【新密码】!";
                    this.newPasswordBox.Focus();
                    break;
                }
                if (string.IsNullOrEmpty(this.confirmNewPasswordBox.Password.Trim()))
                {
                    this.ErrMsgLabel.Content = "请输入【确认新密码】!";
                    this.confirmNewPasswordBox.Focus();
                    break;
                }
                if (!this.newPasswordBox.Password.Trim().Equals(this.confirmNewPasswordBox.Password.Trim()))
                {
                    this.ErrMsgLabel.Content = "【新密码】与【确认新密码】不一致!";
                    this.confirmNewPasswordBox.Focus();
                    break;
                }
                this.ErrMsgLabel.Content = string.Empty;
            } while (false);
            if (!string.IsNullOrEmpty(this.ErrMsgLabel.Content.ToString()))
            {
                //logH.SaveLog("修改密码时，系统提示：" + this.ErrMsgLabel.Content.ToString());
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_修改密码_保存前的窗体验证",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "修改密码时，系统提示：" + this.ErrMsgLabel.Content.ToString(),
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 取消按钮
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }

        private void ModifyPassword_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DragMove();
        }
    }
}
