using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using Talent.ClientCommMethod;
using Talent.Measure.DomainModel.CommonModel;
using System.Threading;
using Talent_LT.HelpClass;
using Talent_LT.ComFrm;
using Newtonsoft.Json;


namespace Talent.CarMeasureClient
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
            //string FtpAddress = ConfigurationManager.AppSettings["FtpAddress"].ToString();
            ////string FtpPort = ConfigurationManager.AppSettings["FtpPort"].ToString();
            //string FtpUserName = ConfigurationManager.AppSettings["FtpUserName"].ToString();
            //string FtpPassWord = ConfigurationManager.AppSettings["FtpPassWord"].ToString();

            //Updater.SetDownLoadParam(FtpAddress, FtpUserName, FtpPassWord);
            //Updater.CheckUpdateStatus();
            ProcessHelpClass.ClearProcessContainsName("Talent.FIleSync");
            ProcessHelpClass.ClearProcessContainsName("Talent.CarMeasureConfig");
            Application currApp = Application.Current;
            currApp.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.Exception;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "启动页_App.xaml.cs_DispatcherUnhandledException",
                    Msg = "UI线程异常:" + ex.Message + "异常实例InnerException:" + ex.InnerException + "堆栈StackTrace:" + ex.StackTrace,
                    OperateUserName =LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(log));
                #endregion
                e.Handled = true;
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "启动页_App.xaml.cs_DispatcherUnhandledException",
                    Msg = "捕获到UI线程异常处理时异常:" + ex.Message + "异常实例InnerException:" + ex.InnerException + "堆栈StackTrace:" + ex.StackTrace,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        //多线程异常
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            try
            {
                Exception ex = e.ExceptionObject as Exception;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "启动页_App.xaml.cs_CurrentDomain_UnhandledException",
                    Msg = "UI线程异常:" + ex.Message + "异常实例InnerException:" + ex.InnerException + "堆栈StackTrace:" + ex.StackTrace,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(log));
                #endregion
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    FunctionName = "启动页_App.xaml.cs_CurrentDomain_UnhandledException",
                    Msg = "捕获到多线程异常处理时异常:" + ex.Message + "异常实例InnerException:" + ex.InnerException + "堆栈StackTrace:" + ex.StackTrace,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(Newtonsoft.Json.JsonConvert.SerializeObject(log));
                #endregion
            }
        }
    }
}
