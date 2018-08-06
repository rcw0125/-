using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Talent.Measure.DomainModel;
using Talent.Video.Controller;

namespace Talent.RemoteCarMeasure
{
    public partial class VideosForm : UserControl
    {
        Talent.Measure.DomainModel.Camera _curCameraConfig;
        Talent.Measure.DomainModel.VideoConfig cfg;
        Talent.Video.Controller.VideoController _curVideoController;
        List<IntPtr> handelList = new List<IntPtr>();
        private PictureBox tempBorderMax = new PictureBox();
        private PictureBox tempBorderMin = new PictureBox();

        public VideosForm()
        {
            InitializeComponent();
        }

        private void ReadVideoConfig()
        {
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            string configPath = System.IO.Path.Combine(basePath, "SystemConfig.xml");
            Talent.Measure.DomainModel.ConfigReader cfgReader1 = new Talent.Measure.DomainModel.ConfigReader(configPath);
            cfg = ConfigReader.ReadVideoConfig();
            cfg.CameraList = (from r in cfg.CameraList select r).OrderBy(c => c.Position).ToList();
        }

        /// <summary>
        /// 打开配置文件中的所有视频
        /// </summary>
        private void OpenVideos()
        {
            List<IntPtr> handelList = new List<IntPtr>();
            for (int i = 0; i < cfg.CameraList.Count; i++)
            {
                if (i == 0)
                {
                    videoBig.MouseUp += videoMin_MouseUp;
                    handelList.Add(videoBig.Handle);
                }
                else
                {
                    PictureBox pb = new PictureBox() { BackColor = System.Drawing.Color.Pink, Width = 100, Height = 120, Margin = new Padding(3, 0, 3, 0) };
                    pb.MouseUp += videoMin_MouseUp;
                    handelList.Add(pb.Handle);
                    this.panel2.Controls.Add(pb);
                }
            }
            if (handelList.Count > 0)
            {
                _curVideoController = new VideoController(new FtpConfig(), cfg, handelList);
                _curVideoController.OnShowErrMsg += _curVideoController_OnShowErrMsg;
                _curVideoController.Open();
                _curVideoController.Start();
            }
            PictureBox pb1 = new PictureBox() { BackColor = System.Drawing.Color.Pink, Width = 100, Height = 120, Margin = new Padding(3, 0, 3, 0) };
            pb1.Image = System.Drawing.Image.FromFile(@"D:\work\远程计量\计量改版程序新\Talent.Measure.Solution\Talent.RemoteCarMeasure\Image\CarImage\1.png");
            PictureBox pb2 = new PictureBox() { BackColor = System.Drawing.Color.Pink, Width = 100, Height = 120, Margin = new Padding(3, 0, 3, 0) };
            pb2.Image = System.Drawing.Image.FromFile(@"D:\work\远程计量\计量改版程序新\Talent.Measure.Solution\Talent.RemoteCarMeasure\Image\CarImage\2.png");
            PictureBox pb3 = new PictureBox() { BackColor = System.Drawing.Color.Pink, Width = 100, Height = 120, Margin = new Padding(3, 0, 3, 0) };
            pb3.Image = System.Drawing.Image.FromFile(@"D:\work\远程计量\计量改版程序新\Talent.Measure.Solution\Talent.RemoteCarMeasure\Image\CarImage\3.png");
            PictureBox pb4 = new PictureBox() { BackColor = System.Drawing.Color.Pink, Width = 100, Height = 120, Margin = new Padding(3, 0, 3, 0) };
            pb4.Image = System.Drawing.Image.FromFile(@"D:\work\远程计量\计量改版程序新\Talent.Measure.Solution\Talent.RemoteCarMeasure\Image\CarImage\4.png");
            PictureBox pb5 = new PictureBox() { BackColor = System.Drawing.Color.Pink, Width = 100, Height = 120, Margin = new Padding(3, 0, 3, 0) };
            pb5.Image = System.Drawing.Image.FromFile(@"D:\work\远程计量\计量改版程序新\Talent.Measure.Solution\Talent.RemoteCarMeasure\Image\CarImage\5.png");
            PictureBox pb6 = new PictureBox() { BackColor = System.Drawing.Color.Pink, Width = 100, Height = 120, Margin = new Padding(3, 0, 3, 0) };
            pb6.Image = System.Drawing.Image.FromFile(@"D:\work\远程计量\计量改版程序新\Talent.Measure.Solution\Talent.RemoteCarMeasure\Image\CarImage\6.png");
            this.panel2.Controls.Add(pb1);
            this.panel2.Controls.Add(pb2);
            this.panel2.Controls.Add(pb3);
            this.panel2.Controls.Add(pb4);
            this.panel2.Controls.Add(pb5);
            this.panel2.Controls.Add(pb6);
        }

        /// <summary>
        /// 交换大小摄像头
        /// </summary>
        /// <param name="tempBorderMin">小摄像头</param>
        /// <param name="tempBorderMax">大摄像头</param>
        private void ExchangeVideo(PictureBox tempBorderMin, PictureBox tempBorderMax)
        {
            //for (int i = 0; i < this.panel2.Controls.Count; i++)
            //{
            //    PictureBox minPb = panel2.Controls[i] as PictureBox;
            //    if (minPb == tempBorderMin)
            //    {
            //        panel2.Controls.RemoveAt(i);
            //        panel2.Controls.Add(tempBorderMax);
            //        panel2.Controls.SetChildIndex(panel2.Controls[panel2.Controls.Count - 1], i);
            //        break;
            //    }
            //}
        }

        private void videoMin_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int minW = 0;
                int minH = 0;
                tempBorderMin = sender as PictureBox;
                if (tempBorderMin.Width > 210)
                    return;
                tempBorderMax = videoBig;
                minH = tempBorderMin.Height;
                minW = tempBorderMin.Width;
                tempBorderMin.Width = tempBorderMax.Width;
                tempBorderMin.Height = tempBorderMax.Height;
                tempBorderMax.Width = minW;
                tempBorderMax.Height = minH;

                ExchangeVideo(tempBorderMin, tempBorderMax);
                videoBig = tempBorderMin;
            }
        }

        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="msg"></param>
        void _curVideoController_OnShowErrMsg(string msg)
        {
            System.Windows.Forms.MessageBox.Show(msg);
        }

        private void VideosForm_Load(object sender, EventArgs e)
        {
            ReadVideoConfig();
            OpenVideos();
        }
    }
}
