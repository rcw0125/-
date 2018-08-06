using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.ClientCommonLib;
using Talent.CommonMethod;
using Talent.RemoteCarMeasure.ViewModel;

namespace Talent.RemoteCarMeasure
{
    /// <summary>
    /// TestChildForm.xaml 的交互逻辑
    /// </summary>
    public partial class TestChildForm : Only_WindowBase
    {
        public TestChildForm()
        {
            InitializeComponent();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister(this);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<bool>(this, "OpenVideo", new Action<bool>(OpenFirstVideo));
        }

        private void OpenFirstVideo(bool isOpen)
        {
            if (isOpen)
            {
                MessageBox.Show("打开大视频!");
            }
        }

        private void CloseFormBtn_Click(object sender, RoutedEventArgs e)
        {
            //TaskHandleViewModel vm = this.DataContext as TaskHandleViewModel;
            //vm.ShowBusy = true;
            //SaveDoResult("测试");
            try
            {
                //((IDisposable)imageControl).Dispose();
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister(this);
                this.Close();
                //this.Dispose();
            }
            catch (Exception ex)
            {
                string st = ex.Message;
            }
        }

        /// <summary>
        /// 保存任务结果
        /// </summary>
        /// <param name="handleResult">任务处理结果</param>
        private void SaveDoResult(string handleResult)
        {
            string url = "";
            string paramStr = "";
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["saveTaskDoResult"].ToString();
                var param = new
                {
                    opname = "1111",
                    opcode = "1111",
                    taskbegintime = DateTime.Now.ToString("yyyyMMddHHmmss"),//时间改为24 小时制 2016-3-7 09:03:20……
                    taskendtime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    equcode = "1111",
                    equname = "1111",
                    carno = "1111",
                    taskdoresult = handleResult,
                    memo = "1111",
                    equtype = "1111"
                };
                paramStr = "[" + JsonConvert.SerializeObject(param) + "]";
                url = string.Format(serviceUrl, System.Web.HttpUtility.UrlEncode(paramStr, System.Text.Encoding.UTF8));
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(url, 10);
                #region 写日志
                Console.WriteLine("保存任务处理结果!");
                #endregion
                request.BeginGetResponse(new AsyncCallback(saveTaskDoResultCallback), request);
            }
            catch (Exception ex)
            {
                #region 写日志
                Console.WriteLine("保存任务处理结果时异常:" + ex.Message);
                #endregion
                //异常情况下，继续走后续的流程
                saveTaskDoResultCallback(null);
            }
            finally
            {
                //saveTaskDoResultCallback(null);
            }
        }

        /// <summary>
        /// 保存任务结果回调方法(后续处理流程)
        /// </summary>
        /// <param name="ar"></param>
        private void saveTaskDoResultCallback(IAsyncResult ar)
        {
            #region 写日志
            Console.WriteLine("获取到保存任务结果服务的反馈");
            #endregion

            try
            {
                for (int i = 0; i < 3000; i++)
                {
                    Console.WriteLine("当前是第" + i + "个");
                }
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    #region 写日志
                    Console.WriteLine("-----------【准备关闭视频啦】--------------");
                    #endregion
                    CloseVideoControllers();
                }));
                SaveFormLocation();
                FlushMemory();
            }
            catch (Exception ex)
            {
                #region 写日志
                Console.WriteLine("记录关闭时的坐标及释放内存时异常:" + ex.Message);
                #endregion
            }
            finally
            {
                Console.WriteLine("-----------【saveTaskDoResultCallback方法里到finally了】--------------");
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Console.WriteLine("-----------【取消转的圈圈】--------------");
                    TaskHandleViewModel vm = this.DataContext as TaskHandleViewModel;
                    vm.ShowBusy = false;
                    Console.WriteLine("-----------【调用窗体关闭this.Close()】--------------");
                    this.Close();
                }));
            }
            Console.WriteLine("-----------【saveTaskDoResultCallback方法结束】--------------");
        }

        private void FlushMemory()
        {
            #region 写日志
            Console.WriteLine("-----------【释放内存方法执行了】--------------");
            #endregion
        }

        private void SaveFormLocation()
        {
            #region 写日志
            Console.WriteLine("-----------【保存窗体位置方法执行了】--------------");
            #endregion
        }

        private void CloseVideoControllers()
        {
            #region 写日志
            Console.WriteLine("-----------【关闭视频方法执行了】--------------");
            #endregion
        }

        private void Only_WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            TaskHandleViewModel vm = this.DataContext as TaskHandleViewModel;
            vm.ShowBusy = false;
            //获得当前工作进程
            Process proc = Process.GetCurrentProcess();
            long usedMemory = proc.PrivateMemorySize64;
            MessageBox.Show("内存大小:" + Math.Abs(usedMemory / 1024));
        }


    }
}
