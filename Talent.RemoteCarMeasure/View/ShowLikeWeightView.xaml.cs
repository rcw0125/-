using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.ClientCommonLib;
using Talent.RemoteCarMeasure.Commom;
using System.Collections.ObjectModel;
using Talent.Measure.WPF.Remote;
using Talent.Measure.WPF.Log;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    ///  
    /// </summary>
    public partial class ShowLikeWeightView : Only_WindowBase
    {

        public ObservableCollection<BullInfo> HistotyTare { get; set; }
        private string cMatchid = string.Empty;//当前matchid 
        private string sameType = string.Empty;//当前计量方式
        string postData = string.Empty;//提交数据……
        /// <summary>
        /// 日志记录
        /// </summary>
        //LogsHelpClass logH = new LogsHelpClass();
        public ShowLikeWeightView(List<BullInfo> dataInfos, string cM, string cType)
        {
            InitializeComponent();
            HistotyTare = new ObservableCollection<BullInfo>();
            for (int i = 0; i < dataInfos.Count; i++)
            {
                HistotyTare.Add(dataInfos[i]);
            }
            this.DataContext = this;
            cMatchid = cM;
            sameType = cType;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 行选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataGrid.SelectedItem == null)
            {
                return;
            }
            BullInfo cSelect = this.DataGrid.SelectedItem as BullInfo;
            if (cSelect == null)
            {
                return;
            }
            string selectMatchid = cSelect.matchid;//当前选中的matchid
            //selectMatchid = "1603091747";
            GetImages(selectMatchid, string.Empty, string.Empty);
        }
        /// <summary>
        /// 确认保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SaveInfos();
        }
        /// <summary>
        /// 保存信息
        /// </summary>
        public void SaveInfos()
        {
            string serviceUrl = ConfigurationManager.AppSettings["insertConfigmodel"].ToString();
            postData = GetPostData();
            HttpWebRequest request = WebRequestCommon.GetHttpPostWebRequest(serviceUrl, 10, postData, "");
            request.BeginGetResponse(new AsyncCallback(saveInfoCallback), request);
        }
        /// <summary>
        /// 保存之后回调
        /// </summary>
        private void saveInfoCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                var getServiceModel = InfoExchange.DeConvert(typeof(MeasureServiceModel), strResult) as MeasureServiceModel;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (getServiceModel.success)
                    {
                        #region 日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_OutIn,
                            FunctionName = "坐席_历史相似重量信息_保存信息",
                            Level = LogConstParam.LogLevel_Info,
                            Msg = "相似重量保存信息成功：用户备注：" + this.msgTxt.Text.Trim() + "  物流号集合：" + GetMatchidList(),
                            Origin = LoginUser.Role.Name,
                            OperateUserName = LoginUser.Name,
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        this.Close();
                    }
                    else
                    {
                        #region 日志
                        LogModel log = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_OutIn,
                            FunctionName = "坐席_历史相似重量信息_保存信息",
                            Level = LogConstParam.LogLevel_Warning,
                            Msg = "相似重量保存信息失败,原因：" + getServiceModel.msg,
                            Origin = LoginUser.Role.Name,
                            OperateUserName = LoginUser.Name,
                            Data = getServiceModel
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                        #endregion
                        MessageBox.Show(getServiceModel.msg);
                    }
                }));
            }
            catch
            {
            }
        }
        /// <summary>
        /// 提交保存的数据……
        /// </summary>
        private string GetPostData()
        {
            string rtStr = string.Empty;
            var param = new
                      {
                          matchid = string.Empty,
                          sametype = sameType,
                          fmatchid = cMatchid,
                          memo = this.msgTxt.Text.Trim(),
                          mtype = string.Empty,
                          matchidlist = GetMatchidList()
                      };

            rtStr = "" + JsonConvert.SerializeObject(param) + "";
            return rtStr;
        }
        /// <summary>
        /// 获取matchid集合
        /// </summary>
        /// <returns></returns>
        private string GetMatchidList()
        {
            string rtStr = string.Empty;
            for (int i = 0; i < HistotyTare.Count; i++)
            {
                rtStr = rtStr + "," + HistotyTare[i].matchid;
            }
            if (!string.IsNullOrEmpty(rtStr))
            {
                rtStr = rtStr.Substring(1);
            }
            return rtStr;
        }

        /// <summary>
        /// 从服务获取图片信息
        /// </summary>
        /// <param name="matchid">过磅单号</param>
        /// <param name="equcode">称点code</param>
        /// <param name="equname">称点名称</param>
        private void GetImages(string matchid, string equcode, string equname)
        {
            string serviceUrl = ConfigurationManager.AppSettings["getMeasurePhoto"].ToString();
            var param = new
            {
                matchid = matchid,
                photo = "",
                measuretype = "",
                equcode = "",
                equname = ""
            };
            var jsonStr = "[" + JsonConvert.SerializeObject(param) + "]";
            string url = string.Format(serviceUrl, jsonStr);
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(url, 10);
            request.BeginGetResponse(new AsyncCallback(GetMeasurePhotoCallback), request);
        }

        /// <summary>
        /// 获取图片信息回调方法
        /// </summary>
        /// <param name="asyc"></param>
        private void GetMeasurePhotoCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                List<PictureModel> pictureInfos = InfoExchange.DeConvert(typeof(List<PictureModel>), strResult) as List<PictureModel>;
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Threading.ThreadStart(() =>
                {
                    this.Pictures.ItemsSource = null;
                    DownloadImags(pictureInfos);
                    this.Pictures.ItemsSource = pictureInfos;
                }
                    ));
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_历史相似重量信息_获取图片信息回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务读取图片信息失败！原因：" + ex.Message,
                    Origin = LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="list"></param>
        private void DownloadImags(List<PictureModel> list)
        {
            ComHelpClass cHelp = new ComHelpClass();
            cHelp.DownloadImags(list);
        }

        // 支持标题栏拖动  
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            bool isFirstChangeWindowsState = false;
            base.OnMouseLeftButtonDown(e);

            // 获取鼠标相对标题栏位置  
            Point position = e.GetPosition(gridTitleBar);

            // 如果鼠标位置在标题栏内，允许拖动  
            if (position.X >= 0 && position.X < gridTitleBar.ActualWidth && position.Y >= 0 && position.Y < gridTitleBar.ActualHeight)
            {
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1)
                {
                    this.DragMove();
                }
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                {
                    if (this.WindowState == WindowState.Normal)
                    {
                        isFirstChangeWindowsState = true;
                        this.WindowState = WindowState.Maximized;
                    }
                    if (isFirstChangeWindowsState == false)
                    {
                        if (this.WindowState == WindowState.Maximized)
                        {
                            this.WindowState = WindowState.Normal;
                        }
                    }
                }
            }
        }
    }
}
