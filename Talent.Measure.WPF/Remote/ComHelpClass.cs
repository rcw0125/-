using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;
using Talent.CommonMethod;
using Talent.Measure.DomainModel.CommonModel;
using Talent_LT.HelpClass;

namespace Talent.Measure.WPF.Remote
{
    public class ComHelpClass
    {
        #region 下载图片
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
            catch //(Exception ex)
            {
                #region 日志
                //LogModel log = new LogModel()
                //{
                //    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //    Direction = LogConstParam.Directions_Out,
                //    FunctionName = "坐席_公共帮助方法",
                //    Level = LogConstParam.LogLevel_Error,
                //    Msg = "根据图片字节数字获取图片失败！原因：" + ex.Message,
                //    Origin = "汽车衡_" + LoginUser.Role.Name,
                //    OperateUserName = LoginUser.Name
                //};
                //Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            return bitMapImage;

            #region 原来的写法
            //System.IO.MemoryStream ms = new MemoryStream(image);
            //ms.Seek(0, System.IO.SeekOrigin.Begin);
            //BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //bi.CacheOption = BitmapCacheOption.OnLoad;
            //bi.StreamSource = ms;
            //bi.EndInit();
            //bi.Freeze();
            //return bi;
            #endregion
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
                    Color pixelColor = bitmap.GetPixel(i, j);
                    Color newColor = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B);
                    bitmapSource.SetPixel(i, j, newColor);
                }
            MemoryStream ms = new MemoryStream();
            bitmapSource.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
            bitmapImage.EndInit();
            ms.Close();
            ms.Dispose();
            return bitmapImage;
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="list"></param>
        public void DownloadImags(List<PictureModel> list)
        {
            if (list != null && list.Count > 0)
            {
                try
                {
                    FtpManager fm = new FtpManager();
                    string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
                    string clientConfigPath = System.IO.Path.Combine(basePath, list.First().equcode + ".xml");//正式用

                    #region 读取FTP配置
                    #region ftpIp
                    string ftpIpItem = ConfigurationManager.AppSettings["FtpIp"].ToString();
                    string ftpIp = XpathHelper.GetValue(clientConfigPath, ftpIpItem);
                    #endregion

                    #region ftp用户名
                    string ftpUserNameItem = ConfigurationManager.AppSettings["FtpPicUserName"].ToString();
                    string ftpUserName = XpathHelper.GetValue(clientConfigPath, ftpUserNameItem);
                    #endregion

                    #region ftp密码
                    string ftpPasswordItem = ConfigurationManager.AppSettings["FtpPicPassWord"].ToString();
                    string ftpPassword = XpathHelper.GetValue(clientConfigPath, ftpPasswordItem);
                    #endregion
                    #endregion
                    //fm.FtpUpDown(ftpIp, ftpUserName, ftpPassword);
                    foreach (var item in list)
                    {
                        //byte[] imageData = fm.Download(item.photo);
                        //item.image = GetImageByBytes(imageData);
                        //item.image = GetImageByUrl(ftpIp, ftpUserName, ftpPassword, item.photo);
                        item.FtpPhoto = "ftp://" + ftpUserName + ":" + ftpPassword + "@" + ftpIp + "/" + item.photo;
                    }
                }
                catch //(Exception ex)
                {
                    //this.ShowBusy = false;
                    //this.ShowMessage("信息提示", "下载图片失败!原因:" + ex.Message, true, false);
                }
            }
        }

        /// <summary>
        /// 通过图片的ftp路径访问获取图片
        /// </summary>
        /// <param name="ftpIp"></param>
        /// <param name="ftpUserName"></param>
        /// <param name="ftpPw"></param>
        /// <param name="imgPath"></param>
        /// <returns></returns>
        private BitmapImage GetImageByUrl(string ftpIp, string ftpUserName, string ftpPw, string imgPath)
        {
            string path = "ftp://" + ftpUserName + ":" + ftpPw + "@" + ftpIp + "/" + imgPath;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(path);
            bi.EndInit();
            bi.Freeze();
            return bi;
        }
        #endregion
        /// <summary>
        /// 异步获取HTTP返回信息
        /// </summary>
        /// <param name="asyc"></param>
        /// <returns></returns>
        public static string ResponseStr(IAsyncResult asyc)
        {
            //return WebHttpHelpClass.ResponseStr(asyc);
            string rtStr = string.Empty;
            HttpWebRequest request = (HttpWebRequest)asyc.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyc);
            try
            {
                Encoding myEncode = Encoding.GetEncoding("UTF-8");
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), myEncode))
                {
                    rtStr = sr.ReadToEnd();
                    sr.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                response.Close();
            }
            return rtStr;
        }
        /// <summary>
        /// 同步获取HTTP返回信息
        /// </summary>
        /// <param name="asyc"></param>
        /// <returns></returns>
        public static string ResponseSynStr(HttpWebRequest req)
        {
            //return WebHttpHelpClass.ResponseSynStr(req);
            string rtStr = string.Empty;
            HttpWebRequest request = req;
            WebResponse response = request.GetResponse();
            try
            {
                Encoding myEncode = Encoding.GetEncoding("UTF-8");
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), myEncode))
                {
                    rtStr = sr.ReadToEnd();
                    sr.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                response.Close();
            }
            return rtStr;
        }
        /// <summary>
        /// 判断是不是显示一行信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CheckIsOneRow(string fileName)
        {
            bool rtB = false;
            switch (fileName.ToUpper())
            {
                case "TARGETNAME":
                    rtB = true;
                    break;
                case "SOURCENAME":
                    rtB = true;
                    break;
                case "USERMEMO":
                    rtB = true;
                    break;
            }
            return rtB;
        }

        /// <summary>
        /// 判断是不是要字体加粗
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CheckIsFontWeightBold(string fileName)
        {
            bool rtB = false;
            switch (fileName.ToUpper())
            {
                case "TARGETNAME":
                    rtB = true;
                    break;
                case "SOURCENAME":
                    rtB = true;
                    break;
                case "MATERIALNAME":
                    rtB = true;
                    break;
            }
            return rtB;
        }
    }
}
