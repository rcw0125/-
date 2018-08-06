using System;
using System.Collections.Generic;
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
using Talent.Measure.DomainModel;
using Talent.Video.Controller;
using Model = Talent.Measure.DomainModel;

namespace Talent.CarMeasureConfig.SystemConfig
{
    /// <summary>
    /// 展示摄像头图像信息的交互逻辑
    /// </summary>
    public partial class ShowVideoForm : Window
    {
        VideoController _curVideoController;
        string _curConfigFilename;
        Model.Camera _curCameraConfig;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="videoDriverStr"></param>
        /// <param name="pConfigFileName"></param>
        public ShowVideoForm(Camera camera,string pConfigFileName)
        {
            InitializeComponent();
            _curConfigFilename = pConfigFileName;
            _curCameraConfig = camera;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ClientConfig");
            string configPath = System.IO.Path.Combine(basePath, _curConfigFilename);
            //默认为第一位
            _curCameraConfig.Position = "1";

            // 加载配置
            ConfigReader cfgReader = new ConfigReader(configPath);
            Model.VideoConfig cfg=ConfigReader.ReadVideoConfig();
            Model.FtpConfig phCfg = ConfigReader.ReadPhotoConfig();
            cfg.CameraList.Clear();
            cfg.CameraList.Add(_curCameraConfig);

            //打开摄像头
            List<IntPtr> handelList=new List<IntPtr>();
            handelList.Add(VideoPictureBox.Handle);

            _curVideoController = new VideoController(phCfg,cfg, handelList);
            _curVideoController.OnShowErrMsg += _curVideoController_OnShowErrMsg;
            _curVideoController.Open();
            _curVideoController.Start();
        }

        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="msg"></param>
        void _curVideoController_OnShowErrMsg(string msg)
        {
            MessageBox.Show(msg);
        }

        /// <summary>
        /// 关闭窗体时停止预览，退出。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            _curVideoController.Stop();
            _curVideoController.Close();
            this.VideoPictureBox.Image = null;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _curVideoController.CapturePicture("123","G");
        }
    }
}
