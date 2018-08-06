using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Talent.Measure.DomainModel;
using Talent.ClinetLog;
using Talent.CommonMethod;
using System.Net;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Talent.FIleSync
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint RegisterWindowMessage(string lpString);
        private uint msgexit;
        string msgtxt = "QUIT_FILESYNC";
        /// <summary>
        /// 日志
        /// </summary>
        ILog log;

        /// <summary>
        /// 是否正在同步文件
        /// </summary>
        bool _fileUploadRunning = false;

        /// <summary>
        /// 配置文件路径
        /// </summary>
        string _sysConfigFile = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["SysConfigFileName"].ToString();
        /// <summary>
        /// 秤点编号
        /// </summary>
        string _curClientCode = ConfigurationManager.AppSettings["ClientIdInfo"].ToString();

        /// <summary>
        /// 保存服务的uri
        /// </summary>
        string _saveMeasurePhoto = ConfigurationManager.AppSettings["saveMeasurePhoto"].ToString();
        //XpathHelper


        FtpManager _curFtpServices;
        /// <summary>
        /// 照片同步配置
        /// </summary>
        FtpConfig _curPhotoConfig;
        /// <summary>
        /// 当前要上传的照片文件
        /// </summary>
        FileInfo[] _curUploadJpgFiles;
        /// <summary>
        /// 当前要上传的日志文件
        /// </summary>
        FileInfo[] _curUploadLogFiles;
        /// <summary>
        /// 文件同步计时器
        /// </summary>
        DispatcherTimer curUploadTimer;
        WindowState ws;
        WindowState wsl;
        NotifyIcon notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            log = LogHelper.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\Log4NetFileSync.config", "FILE_SYNC");
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                msgexit = RegisterWindowMessage(msgtxt);
                if (msgexit == 0)
                {
                    //MessageBox.Show(Marshal.GetLastWin32Error().ToString());
                }
            }));
           
         

        }
        private  IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == msgexit)
            {
                Quit(null,null);
                //System.Windows.MessageBox.Show("ok");
               // return true;
            }
            return IntPtr.Zero;
           // return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            //获取称点编码
            _curClientCode = XpathHelper.GetValue(_sysConfigFile, _curClientCode);

            this.notifyIcon = new NotifyIcon();
            //this.notifyIcon.BalloonTipText = "文件同步程序"; //设置程序启动时显示的文本
            //this.notifyIcon.Text = "文件同步程序";//最小化到托盘时，鼠标点击时显示的文本
            this.notifyIcon.BalloonTipText = ""; //设置程序启动时显示的文本
            this.notifyIcon.Text = "";//最小化到托盘时，鼠标点击时显示的文本
            this.notifyIcon.Icon = Properties.Resources.FileSync;
            this.notifyIcon.Visible = true;


            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(Quit);
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            // notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            //this.notifyIcon.ShowBalloonTip(1000);
           
            log.Info("注册托盘图标完成");
            //读取照片配置
            ConfigReader rf = new ConfigReader(_sysConfigFile);
            _curPhotoConfig = ConfigReader.ReadPhotoConfig();
            log.Info("读取配置文件完成");
            _curFtpServices = new FtpManager();
            _curFtpServices.FtpUpDown(_curPhotoConfig.FtpIp, _curPhotoConfig.FtpUserName, _curPhotoConfig.FtpPassWord);
            log.Info("初始化文件服务器完成");



            //定时器相关
            curUploadTimer = new DispatcherTimer();
            curUploadTimer.Interval = new TimeSpan(0, 1, 0); //一分钟检测一次
            curUploadTimer.Tick += curUploadJpgTimer_Tick;
            curUploadTimer.Start();
            ShowUpLoadInfo("开始文件同步");
            WindowState = System.Windows.WindowState.Minimized;
            wsl = WindowState;
          
           // this.Visibility = System.Windows.Visibility.Hidden;
            this.Hide();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource sorce = PresentationSource.FromVisual(this) as HwndSource;
            sorce.AddHook(new HwndSourceHook(WindowProc));
        }
        /// <summary>
        /// 退出
        /// </summary>
        public void Quit(object sender, EventArgs e)
        {
            this.Close(); 
            System.Windows.Forms.Application.Exit();
        }
        /// <summary>
        /// 定时处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void curUploadJpgTimer_Tick(object sender, EventArgs e)
        {
            if (!_fileUploadRunning)
            {
                _curUploadLogFiles = GetLogFile(_curPhotoConfig.LogSavePath);
                _curUploadJpgFiles = GetJpgFile(_curPhotoConfig.PictureSavePath);
            }
            UpLoadFile();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        private void UpLoadFile()
        {
            _fileUploadRunning = true;
            //上传日志文件
            if (_curUploadLogFiles != null && _curUploadLogFiles.Length > 0)
            {
                log.Info("开始日志同步");
                foreach (FileInfo file in _curUploadLogFiles)
                {
                    try
                    {
                        string date = file.CreationTime.ToString("yyyy-MM-dd");
                        string path = string.Format("ClientLog/{0}/{1}/{2}", _curClientCode, file.CreationTime.Year, date);

                        if (file.DirectoryName != _curPhotoConfig.LogSavePath)
                        {
                            path = System.IO.Path.Combine(path, file.Directory.Name);
                        }
                        //设置当前目录
                        _curFtpServices.FtpUpDown(_curPhotoConfig.FtpIp, _curPhotoConfig.FtpUserName, _curPhotoConfig.FtpPassWord);
                        //创建目录
                        // if(Directory. _curPhotoConfig.LogSavePath!=file.Directory.Parent)
                        _curFtpServices.CheckDirectoryAndMakeMyWilson3(path);
                        //设置目录
                        _curFtpServices.FtpUpDown(_curPhotoConfig.FtpIp + "/" + path + "/", _curPhotoConfig.FtpUserName, _curPhotoConfig.FtpPassWord);
                        //上传文件

                        _curFtpServices.Upload(file.FullName);
                        log.Info("上传日志：" + file.FullName);
                        file.Delete();
                        ShowUpLoadInfo(string.Format("日志{0}上传成功。", file.FullName));
                    }
                    catch (Exception ex)
                    {
                        log.Error("上传日志文件失败：" + file.FullName + "。" + ex.Message);
                    }
                }
            }
            //上传照片文件
            if (_curUploadJpgFiles != null && _curUploadJpgFiles.Length > 0)
            {
                //ftp上传文件
                log.Info("开始照片同步");
                foreach (FileInfo file in _curUploadJpgFiles)
                {
                    try
                    {
                        string date = file.CreationTime.ToString("yyyy-MM-dd");
                        string path = string.Format("Photo/{0}/{1}/{2}", _curClientCode, file.CreationTime.Year, date);
                        //设置当前目录
                        _curFtpServices.FtpUpDown(_curPhotoConfig.FtpIp, _curPhotoConfig.FtpUserName, _curPhotoConfig.FtpPassWord);
                        //创建目录
                        _curFtpServices.CheckDirectoryAndMakeMyWilson3(path);
                        //设置目录
                        _curFtpServices.FtpUpDown(_curPhotoConfig.FtpIp + "/" + path + "/", _curPhotoConfig.FtpUserName, _curPhotoConfig.FtpPassWord);
                        log.Info("上传照片路径：" + _curPhotoConfig.FtpIp + "/" + path + "/");
                        //上传文件
                        _curFtpServices.Upload(file.FullName);
                        log.Info("上传照片：" + file.FullName);
                        if (SaveImgInfo(file.Name, path, _curClientCode))
                        {
                            file.Delete();
                            log.Info("删除照片：" + file.FullName);
                        }
                        //log.Info("调用服务写ftp文件路径完成。");
                        ShowUpLoadInfo(string.Format("照片{0}上传成功。", file.FullName));
                    }
                    catch (Exception ex)
                    {
                        log.Error("上传日志文件失败：" + file.FullName + "。" + ex.Message);
                    }
                }
                log.Info("文件同步结束");
                _curUploadJpgFiles = null;
            }
            _fileUploadRunning = false;
        }

        /// <summary>
        /// 调用服务保存图片信息
        /// <param name="pFile"> 文件名</param>
        /// </summary>
        private bool SaveImgInfo(string pFile, string pPath, string pClientCode)
        {
            try
            {
                string[] data = pFile.Split(new Char[] { '_' });
                if (data.Length < 2) return false;
                var temp = new
                {
                    matchid = data[0],
                    photo = pPath + "/" + pFile,
                    measuretype = data[1],
                    equcode = pClientCode,
                    equname = "",
                };

                string jsonStr = "[" + InfoExchange.ConvertToJsonIgnoreRef1(temp) + "]";

                string serviceUrl = string.Format(_saveMeasurePhoto, jsonStr);
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(serviceUrl, 10);
                WebResponse tResponse = request.GetResponse();

                Encoding myEncode = Encoding.GetEncoding("UTF-8");

                string strResult;
                using (StreamReader sr = new StreamReader(tResponse.GetResponseStream(), myEncode))
                {
                    strResult = sr.ReadToEnd();
                }

                LoginServiceModel rtn = InfoExchange.DeConvert(typeof(LoginServiceModel), strResult) as LoginServiceModel;
                if (rtn != null && rtn.success == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("写文件信息到服务失败，错误信息：" + ex.Message);
                return false;
            }


        }
        /// <summary>
        /// 双击任务栏图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            WindowState = wsl;
        }

        //private void Window_StateChanged(object sender, EventArgs e)
        //{
        //    ws = WindowState;
        //    if (ws == WindowState.Minimized)
        //    {
        //        this.Hide();
        //    }
        //}
        int n = 0;
        int max = 1000;
        private void ShowUpLoadInfo(string pMsg)
        {
            TextBlock t = new TextBlock() { Text = DateTime.Now.ToLongTimeString() + "  " + pMsg };
            this.listBox.Items.Insert(0, t);
            if (this.listBox.Items.Count == max)
            {
                this.listBox.Items.RemoveAt(999);
            }
            n++;
        }

        protected override void OnClosed(EventArgs e)
        {
            curUploadTimer.Stop();
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            base.OnClosed(e);
        }

       
        #region 目录操作
        /// <summary>
        /// 获取某个目录下的Jpg文件，并按照时间排序。
        /// </summary>
        /// <param name="pDir"></param>
        /// <returns></returns>
        private FileInfo[] GetJpgFile(string pDir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(pDir);
            //lt 2016-1-29 13:48:05…… 增加异常捕获 解决不存在盘符系统 退出的BUG
            try
            {
                if (Directory.Exists(pDir) == false)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.Info("GetJpgFile获取" + pDir + "异常：" + ex.Message);
            }
            FileInfo[] rtnList = dirInfo.GetFiles("*.jpg", SearchOption.AllDirectories);
            log.Info("获取图片文件完成。");
            Array.Sort(rtnList, new DateSorter());
            log.Info("图片文件按照日期排序。");

            return rtnList;
        }
        /// <summary>
        /// 获取日志文件
        /// </summary>
        /// <param name="pDir"></param>
        /// <returns></returns>
        private FileInfo[] GetLogFile(string pDir)
        {
            List<FileInfo> rtn = new List<FileInfo>();
            //lt 2016-1-29 13:48:05…… 增加异常捕获 解决不存在盘符系统 退出的BUG
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(pDir);
                if (Directory.Exists(pDir) == false) return null;
                FileInfo[] rtnList = dirInfo.GetFiles("*.log", SearchOption.AllDirectories);
                //获取今天之前的文件
                if (rtnList != null && rtnList.Length > 0)
                {
                    for (int count = 0; count < rtnList.Length; count++)
                    {
                        if (rtnList[count].CreationTime.Date < DateTime.Now.Date)
                        {
                            rtn.Add(rtnList[count]);
                        }
                    }
                }
                log.Info("获取日志文件完成。");
                //Array.Sort(rtnList, new DateSorter());
                log.Info("日志文件按照日期排序。");
            }
            catch (Exception ex)
            {
                log.Info("GetLogFile获取" + pDir + "异常：" + ex.Message);
            }

            return rtn.ToArray();
        }
        #endregion

    }
}
