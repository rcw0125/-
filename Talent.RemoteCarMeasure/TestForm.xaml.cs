using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.ClientCommonLib;
using Talent.CommonMethod;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.RemoteCarMeasure.View;

namespace Talent.RemoteCarMeasure
{
    /// <summary>
    /// TestForm.xaml 的交互逻辑
    /// </summary>
    public partial class TestForm : Only_WindowBase
    {
        private List<ClientInfo> models;
        private Timer timer;
        private Random rd = new Random();

        private TestChildForm childForm;
        public TestForm()
        {
            InitializeComponent();
            Rect rect = SystemParameters.WorkArea;
            this.MaxWidth = rect.Width + 14;
            this.MaxHeight = rect.Height + 14;
            this.WindowState = WindowState.Maximized;
        }

        private void LoadImage()
        {
            //List<PictureModel> pmList = new List<PictureModel>();
            //PictureModel pm1 = new PictureModel() { photo = "TMGL/1017010818199_G_1_091647.JPG" };
            //PictureModel pm2 = new PictureModel() { photo = "TMGL/1017010818199_G_2_091648.JPG" };
            //PictureModel pm3 = new PictureModel() { photo = "TMGL/1017010818199_G_3_091647.JPG" };
            //PictureModel pm4 = new PictureModel() { photo = "TMGL/1017010818199_G_4_091647.JPG" };
            //PictureModel pm5 = new PictureModel() { photo = "TMGL/1017010818199_G_5_091646.JPG" };
            //PictureModel pm6 = new PictureModel() { photo = "TMGL/1017010818199_G_SCREEN091646.JPG" };
            //pmList.Add(pm1);
            //pmList.Add(pm2);
            //pmList.Add(pm3);
            //pmList.Add(pm4);
            //pmList.Add(pm5);
            //pmList.Add(pm6);
            //DateTime startTime = DateTime.Now;
            //try
            //{
            //    FtpManager fm = new FtpManager();
            //    string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ClientConfig");
            //    var configPath = System.IO.Path.Combine(basePath, "SystemConfig.xml");

            //    #region 读取FTP配置
            //    #region ftpIp
            //    string ftpIpItem = ConfigurationManager.AppSettings["FtpIp"].ToString();
            //    string ftpIp = XpathHelper.GetValue(configPath, ftpIpItem);
            //    #endregion

            //    #region ftp用户名
            //    string ftpUserNameItem = ConfigurationManager.AppSettings["FtpPicUserName"].ToString();
            //    string ftpUserName = XpathHelper.GetValue(configPath, ftpUserNameItem);
            //    #endregion

            //    #region ftp密码
            //    string ftpPasswordItem = ConfigurationManager.AppSettings["FtpPicPassWord"].ToString();
            //    string ftpPassword = XpathHelper.GetValue(configPath, ftpPasswordItem);
            //    #endregion
            //    #endregion
            //    //fm.FtpUpDown(ftpIp, ftpUserName, ftpPassword);
            //    foreach (var pm in pmList)
            //    {
            //        pm.FtpPhoto = "ftp://admin:123456@180.150.184.100/" + pm.photo;
            //        //byte[] imageData = fm.Download(pm.photo);
            //        //DateTime dt1 = DateTime.Now;
            //        //pm.image = GetImageByBytes(imageData);
            //        //pm.image = GetImageByUrl("ftp://admin:123456@180.150.184.100/" + pm.photo);
            //        //TimeSpan ts1 = DateTime.Now - dt1;
            //        //System.Windows.MessageBox.Show("第" + pmList.IndexOf(pm) + "个花费:" + ts1.Minutes + "分钟" + ts1.Seconds + "秒");
            //    }
            //    this.imageItemControl.ItemsSource = pmList;
            //    TimeSpan ts = DateTime.Now - startTime;
            //    System.Windows.MessageBox.Show("花费:" + ts.Minutes + "分钟" + ts.Seconds + "秒");
            //}
            //catch //(Exception ex)
            //{

            //}
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            models = new List<ClientInfo>();
            //InitForm();
            //timer = new Timer();
            //timer.Interval = 1000;
            //timer.Start();
            //timer.Tick += timer_Tick;

            //byte[] imageBytes = GetImageBytes();
            //BitmapImage bi = GetImageByBytes(imageBytes);
            //this.imageControl.Source = bi;
        }

        private byte[] GetImageBytes()
        {
            FileStream fs = null;
            byte[] buffer = null;
            try
            {
                string imgPath = "D:\\1.JPG";
                if (File.Exists(imgPath)) //图片文件的全路径字符串
                {
                    fs = new FileStream(imgPath, FileMode.Open);
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, int.Parse(fs.Length.ToString()));
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch
            {
                fs.Close();
                fs.Dispose();
            }
            return buffer;
        }

        private BitmapImage GetImageByUrl(string imagePath)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(imagePath);
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

        /// <summary>
        /// 根据图片字节数字获取图片
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private BitmapImage GetImageByBytes(byte[] image)
        {
            BitmapImage bitMapImage = null;
            try
            {
                System.IO.MemoryStream ms = new MemoryStream(image);
                System.Drawing.Bitmap bm = new System.Drawing.Bitmap(ms);
                bitMapImage = BitmapToBitmapImage(bm);
                ms.Close();
                bm.Dispose();
            }
            catch
            {
            }
            return bitMapImage;

            //System.IO.MemoryStream ms = new MemoryStream(image);
            //ms.Seek(0, System.IO.SeekOrigin.Begin);
            //BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //bi.CacheOption = BitmapCacheOption.OnLoad;
            //bi.StreamSource = ms;
            //bi.EndInit();
            //bi.Freeze();
            //return bi;
        }

        /// <summary>
        /// Bitmap转为BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            Bitmap bitmapSource = new Bitmap(bitmap.Width, bitmap.Height);
            int i, j;
            for (i = 0; i < bitmap.Width; i++)
                for (j = 0; j < bitmap.Height; j++)
                {
                    System.Drawing.Color pixelColor = bitmap.GetPixel(i, j);
                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B);
                    bitmapSource.SetPixel(i, j, newColor);
                }
            MemoryStream ms = new MemoryStream();
            bitmapSource.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            bitmapImage.EndInit();
            return bitmapImage;
        }

        ///// <summary>
        ///// 窗体构造函数
        ///// </summary>
        //private void InitForm()
        //{
        //    bool first = true;
        //    for (int i = 0; i < 9; i++)
        //    {
        //        ClientInfo model = new ClientInfo()
        //        {
        //            ClientId = "00" + (i + 1).ToString(),
        //            ClientName = (i + 1) + "号衡器",
        //            Weight = rd.Next(27500, 30000).ToString(),
        //            State = first ? "1" : "2",
        //            IsRedLight = first,
        //            IsGreenLight = !first
        //        };
        //        models.Add(model);
        //        first = !first;
        //    }
        //    this.ItemsControl.ItemsSource = models;
        //    this.ItemsControl.Items.Refresh();
        //}

        ///// <summary>
        ///// 指定的计时器间隔已过去而且计时器处于启用状态时发生
        ///// </summary>
        //void timer_Tick(object sender, EventArgs e)
        //{
        //    List<ClientInfo> Dt = this.ItemsControl.ItemsSource as List<ClientInfo>;
        //    if (Dt!=null&&Dt.Count>0)
        //    {
        //        var ls = (from r in Dt where r.ClientId == "00" + rd.Next(1, 9) select r).ToList();
        //        if (ls!=null&&ls.Count>0)
        //        {
        //            ls.First().IsGreenLight = !ls.First().IsGreenLight;
        //            ls.First().IsRedLight = !ls.First().IsRedLight;
        //            ls.First().State = ls.First().IsRedLight ? "1" : "2";
        //            ls.First().Weight = rd.Next(27500, 30000).ToString();
        //        }
        //    }
        //    this.ItemsControl.Items.Refresh();
        //}

        private void OpenChildFormBtn_Click(object sender, RoutedEventArgs e)
        {
            childForm = new TestChildForm();
            childForm.Closed += childForm_Closed;
            childForm.ShowDialog();
        }

        /// <summary>
        /// 子窗体关闭事件
        /// </summary>
        void childForm_Closed(object sender, EventArgs e)
        {
            FlushMemory();
            childForm = null;
            ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "主窗体中子窗体关闭事件触发了", true, false, "确定", "取消");
            mb.ShowDialog();
        }

        private void sendMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<bool>(true, "OpenVideo");
            if (childForm == null)
            {
                ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "子窗体为null了", true, false, "确定", "取消");
                mb.ShowDialog();
            }
        }

        private void FlushMemory()
        {
            #region 写日志
            Console.WriteLine("-----------【释放内存方法执行了】--------------");
            #endregion
        }

        private void Only_WindowBase_Activated(object sender, EventArgs e)
        {
            #region 写日志
            if (childForm != null)
            {
                childForm.Activate();
            }
            #endregion
        }

        private void loadImagesBtn_Click(object sender, RoutedEventArgs e)
        {
            //LoadImage();

            ConfirmMessageBox mb = new ConfirmMessageBox("系统提示", "这是测试信息框", true, true, "确定", "取消", true, 3);
            mb.ShowDialog();
        }
    }
}
