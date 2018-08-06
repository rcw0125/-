//using Ezhu.AutoUpdater;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Talent.RemoteCarMeasure.Commom;
using Talent_LT.ComFrm;

namespace Talent.RemoteCarMeasure
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            //全局异常捕捉
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; //多线程异常
        }

        public void Application_Startup(object sender, StartupEventArgs e)
        {
            //string FtpAddress = AppConfigReader.ReadCfg("FtpUpdateDir");
            ////string FtpPort = ConfigurationManager.AppSettings["FtpPort"].ToString();
            //string FtpUserName = AppConfigReader.ReadCfg("FtpUserName");
            //string FtpPassWord = AppConfigReader.ReadCfg("FtpPassWord");

            //try
            //{
            //    Updater.SetDownLoadParam(FtpAddress, FtpUserName, FtpPassWord);
            //    Updater.CheckUpdateStatus();
            //}
            //catch
            //{
            //}  
            Application currApp = Application.Current;
            currApp.StartupUri = new Uri("Login.xaml", UriKind.RelativeOrAbsolute);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //ComShowExecption_Frm.ShowException((Exception)e.Exception,false);
            try
            {
                Exception ex = (Exception)e.Exception;
                #region 日志
                Talent.Measure.DomainModel.CommonModel.LogModel log = new Talent.Measure.DomainModel.CommonModel.LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "坐席_App.xaml.cs_DispatcherUnhandledException",
                    Msg = "UI线程异常:" + ex.Message + "异常实例InnerException:" + ex.InnerException + "堆栈StackTrace:" + ex.StackTrace,
                    Origin = Talent.Measure.DomainModel.CommonModel.LoginUser.Role.Name,
                    OperateUserName = Talent.Measure.DomainModel.CommonModel.LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(log));
                #endregion
                e.Handled = true;
            }
            catch(Exception ex) 
            {
                #region 日志
                Talent.Measure.DomainModel.CommonModel.LogModel log = new Talent.Measure.DomainModel.CommonModel.LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "坐席_App.xaml.cs_DispatcherUnhandledException",
                    Msg = "捕获到UI线程异常处理时异常:" + ex.Message + "异常实例InnerException:" + ex.InnerException + "堆栈StackTrace:" + ex.StackTrace,
                    Origin = Talent.Measure.DomainModel.CommonModel.LoginUser.Role.Name,
                    OperateUserName = Talent.Measure.DomainModel.CommonModel.LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        //多线程异常
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //ComShowExecption_Frm.ShowException((Exception)e.ExceptionObject,false);
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                #region 日志
                Talent.Measure.DomainModel.CommonModel.LogModel log = new Talent.Measure.DomainModel.CommonModel.LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "坐席_App.xaml.cs_CurrentDomain_UnhandledException",
                    Msg = "UI线程异常:" + ex.Message + "异常实例InnerException:" + ex.InnerException + "堆栈StackTrace:" + ex.StackTrace,
                    Origin = Talent.Measure.DomainModel.CommonModel.LoginUser.Role.Name,
                    OperateUserName = Talent.Measure.DomainModel.CommonModel.LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(log));
                #endregion

            }
            catch(Exception ex)
            {
                #region 日志
                Talent.Measure.DomainModel.CommonModel.LogModel log = new Talent.Measure.DomainModel.CommonModel.LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "坐席_App.xaml.cs_CurrentDomain_UnhandledException",
                    Msg = "捕获到多线程异常处理时异常:" + ex.Message + "异常实例InnerException:" + ex.InnerException + "堆栈StackTrace:" + ex.StackTrace,
                    Origin = Talent.Measure.DomainModel.CommonModel.LoginUser.Role.Name,
                    OperateUserName = Talent.Measure.DomainModel.CommonModel.LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(log));
                #endregion
            }
        }
    }
}
