using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Talent.ClientCommonLib;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.View;
using Talent.CommonMethod;

namespace Talent.RemoteCarMeasure.ViewModel
{
    public class ModifyPasswordViewModel : Only_ViewModelBase
    {
        #region 属性
        private string userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                this.RaisePropertyChanged("UserName");
            }
        }

        private string password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                this.RaisePropertyChanged("Password");
            }
        }

        private string newPassword;
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword
        {
            get { return newPassword; }
            set
            {
                newPassword = value;
                this.RaisePropertyChanged("NewPassword");
            }
        }

        private string confirmNewPassword;
        /// <summary>
        /// 确认新密码
        /// </summary>
        public string ConfirmNewPassword
        {
            get { return confirmNewPassword; }
            set
            {
                confirmNewPassword = value;
                this.RaisePropertyChanged("ConfirmNewPassword");
            }
        }

        private string errMsg;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg
        {
            get { return errMsg; }
            set
            {
                errMsg = value;
                this.RaisePropertyChanged("ErrMsg");
            }
        }


        #endregion

        #region 命令
        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand { get; private set; }
        /// <summary>
        /// 取消命令
        /// </summary>
        public ICommand CancelCommand { get; private set; }
        #endregion
        private string userConfig = ConfigurationManager.AppSettings["UserConfigFileName"].ToString();
        #region 构造
        public ModifyPasswordViewModel()
        {
            if (this.IsInDesignMode)
                return;
            SaveCommand = new ActionCommand(SaveMethod);
            CancelCommand = new ActionCommand(CancelMethod);
            //string fileMainPath = AppDomain.CurrentDomain.BaseDirectory + "ClientConfig";
            //user = XmlHelper.ReadXmlToObj<User>(fileMainPath + "\\" + userConfig);
            //this.UserName = user.Name;
            this.UserName = LoginUser.LoginName;//修改为取当前登录人……2016-2-24 10:06:12……
        }

        #endregion

        #region 方法

        /// <summary>
        /// 保存密码
        /// </summary>
        private void SaveMethod()
        {
            ValidateForm();
            if (string.IsNullOrEmpty(ErrMsg))
            {

            }
        }

        /// <summary>
        /// 窗体验证
        /// </summary>
        private void ValidateForm()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="obj"></param>
        private void CancelMethod(object obj)
        {
            
        }

        #endregion
    }
}
