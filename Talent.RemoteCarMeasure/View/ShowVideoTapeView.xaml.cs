using System;
using System.Collections.Generic;
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
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.Video.Controller;
using Talent.CommonMethod;
using Talent.Measure.DomainModel.ServiceModel;
using System.Configuration;
using System.Net;
using Talent.Measure.WPF.Remote;
using Talent_LT.HelpClass;
using Newtonsoft.Json;

namespace Talent.RemoteCarMeasure.View
{
    /// <summary>
    /// 查看录像的交互逻辑
    /// </summary>
    public partial class ShowVideoTapeView : Window
    {
        VideoPlayBackController maoVideoPlayBackController;
        VideoPlayBackController piVideoPlayBackController;
        private WeightRecord weightModel;
        private VideoConfig piVideoConfig;
        private VideoConfig maoVideoConfig;
        private AudioConfig piNvrConfig;
        private AudioConfig maoNvrConfig;
        private int piPlaySpeedCount = 0;
        private int maoPlaySpeedCount = 0;
        /// <summary>
        /// 查看类型(0:净重;1:皮重;2:毛重)
        /// </summary>
        private int showType = 0;

        /// <summary>
        /// 是否皮重视频暂停
        /// </summary>
        bool isPiPause = false;
        /// <summary>
        /// 是否毛重视频暂停
        /// </summary>
        bool isMaoPause = false;

        /// <summary>
        /// 查看录像
        /// </summary>
        /// <param name="wm">称量记录</param>
        /// <param name="showType">查看类型(0:净重;1:皮重;2:毛重)</param>
        public ShowVideoTapeView(WeightRecord wm, int showType)
        {
            InitializeComponent();
            if (showType == 0)
            {
                FormNameTextBlock.Text = "净重记录_查看录像";
            }
            else if (showType == 1)
            {
                FormNameTextBlock.Text = "皮重记录_查看录像";
            }
            else if (showType == 2)
            {
                FormNameTextBlock.Text = "毛重记录_查看录像";
            }
            weightModel = wm;
            LoadDefaultTime();
            this.showType = showType;
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region 毛重视频关闭及释放
                if (maoVideoPlayBackController != null)
                {
                    maoVideoPlayBackController.PlayStop();
                }
                maoVideoPlayBackController = null;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "数据查询_查看录像_窗体关闭(btnClose_Click)",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "关闭毛重视频录像成功"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                maoFormHis.Dispose();
                maoFormHis = null;
                #region 日志
                LogModel log1 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "数据查询_查看录像_窗体关闭(btnClose_Click)",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "释放毛重视频录像成功"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                #endregion
                #endregion

                #region 皮重视频关闭及释放
                if (piVideoPlayBackController != null)
                {
                    piVideoPlayBackController.PlayStop();
                }
                piVideoPlayBackController = null;
                #region 日志
                LogModel log2 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "数据查询_查看录像_窗体关闭(btnClose_Click)",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "关闭皮重视频录像成功"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                #endregion
                piFormHis.Dispose();
                piFormHis = null;
                #region 日志
                LogModel log4 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "数据查询_查看录像_窗体关闭(btnClose_Click)",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "释放皮重视频录像成功"
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log4));
                #endregion
                #endregion

                this.Close();
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log2 = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "数据查询_查看录像_窗体关闭(btnClose_Click)",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "关闭窗体时异常:" + ex.Message
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log2));
                #endregion
            }
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ReadNVRConfig();
            if (this.showType == 1)//皮重
            {
                ReadPiVideoConfig();
                this.piVideoList.ItemsSource = this.piVideoConfig.CameraList;
            }
            else if (this.showType == 2)//毛重
            {
                ReadMaoVideoConfig();
                this.maoVideoList.ItemsSource = this.maoVideoConfig.CameraList;
            }
            else//净重
            {
                ReadPiVideoConfig();
                ReadMaoVideoConfig();
                this.maoVideoList.ItemsSource = this.maoVideoConfig.CameraList;
                this.piVideoList.ItemsSource = this.piVideoConfig.CameraList;
            }
        }
        #region 读取视频配置
        /// <summary>
        /// 读取毛重所在衡器的视频配置
        /// </summary>
        private void ReadMaoVideoConfig()
        {
            var clientName = this.weightModel.grossweigh;
            //根据计毛衡器获取衡器的视频配置,暂时先写死
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            string configPath = System.IO.Path.Combine(basePath, this.weightModel.grossweighid + ".xml");
            Talent.Measure.DomainModel.ConfigReader cfgReader1 = new Talent.Measure.DomainModel.ConfigReader(configPath);
            this.maoVideoConfig = ConfigReader.ReadVideoConfig();
            this.maoNvrConfig = ConfigReader.ReadAudioConfig();
            this.maoVideoConfig.CameraList = this.maoVideoConfig.CameraList.OrderBy(c => c.Position).ToList();
        }
        /// <summary>
        /// 读取NVR配置信息
        /// </summary>
        //private void ReadNVRConfig()
        //{
        //    string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
        //    string configPath = System.IO.Path.Combine(basePath, this.weightModel.grossweighid + ".xml");
        //    Talent.Measure.DomainModel.ConfigReader cfgReader1 = new Talent.Measure.DomainModel.ConfigReader(configPath);
        //    this.nvrConfig = ConfigReader.ReadAudioConfig();
        //}

        /// <summary>
        /// 读取皮重所在衡器的视频配置
        /// </summary>
        private void ReadPiVideoConfig()
        {
            var clientName = this.weightModel.tareweigh;
            //根据计皮衡器获取衡器的视频配置,暂时先写死
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            string configPath = System.IO.Path.Combine(basePath, this.weightModel.tareweighid + ".xml");
            Talent.Measure.DomainModel.ConfigReader cfgReader1 = new Talent.Measure.DomainModel.ConfigReader(configPath);
            this.piVideoConfig = ConfigReader.ReadVideoConfig();
            this.piNvrConfig = ConfigReader.ReadAudioConfig();
            this.piVideoConfig.CameraList = this.piVideoConfig.CameraList.OrderBy(c => c.Position).ToList();

        }
        #endregion

        #region 皮重视频回放相关

        private void piVideoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ReadNVRConfig();
            OpenPiVideo();
        }

        /// <summary>
        /// 皮重视频播放
        /// </summary>
        private void OpenPiVideo()
        {
            if (this.showType == 0 || this.showType == 1)
            {
                if (weightModel != null && !string.IsNullOrEmpty(weightModel.taretime))
                {
                    if (piVideoPlayBackController != null)
                    {
                        piVideoPlayBackController.PlayStop();
                    }
                    DateTime startTime = dptPZPlayStartTime.Value.Value; // DateTime.Parse(weightModel.JiMaoDate);
                    DateTime endTime = dptPZPlayEndTime.Value.Value; //startTime.AddMinutes(5);// 
                    if (endTime <= startTime) return;
                    var getCamera = piVideoList.SelectedItem as Camera;
                    if (getCamera != null)
                    {
                        if (this.piVideoConfig != null)
                        {
                            VideoConfig gCfg = InfoExchange.Clone<VideoConfig>(this.piVideoConfig);
                            gCfg.CameraList.Clear();
                            getCamera.Position = "1";

                            getCamera.Ip = piNvrConfig.Ip;
                            getCamera.Port = piNvrConfig.Port;
                            getCamera.UserName = piNvrConfig.UserName;
                            getCamera.PassWord = piNvrConfig.PassWord;

                            gCfg.CameraList.Add(getCamera);
                            piVideoPlayBackController = new VideoPlayBackController(gCfg, piVideoHis.Handle);
                            piVideoPlayBackController.PlayByTime(startTime, endTime);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 皮重视频暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piButtonPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (piVideoPlayBackController != null)
            {
                isPiPause = !isPiPause;
                if (isPiPause)
                {
                    piVideoPlayBackController.PlayPause();
                    this.piButtonPlayPause.Content = "播放";
                }
                else
                {
                    piVideoPlayBackController.PlayStart();
                    this.piButtonPlayPause.Content = "暂停";
                }
            }
        }

        /// <summary>
        /// 皮重视频快进
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piButtonPlayFast_Click(object sender, RoutedEventArgs e)
        {
            if (piVideoPlayBackController != null)
            {
                if (piVideoPlayBackController.PlayFast())
                {
                    piPlaySpeedCount++;
                    this.piButtonPlayFast.Content = "快进(" + piPlaySpeedCount.ToString() + ")";
                }
            }
        }
        /// <summary>
        /// 皮重视频快退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piButtonPlaySlow_Click(object sender, RoutedEventArgs e)
        {
            if (piVideoPlayBackController != null)
            {
                if (piVideoPlayBackController.PlaySlow())
                {
                    piPlaySpeedCount--;
                    this.piButtonPlayFast.Content = "快进(" + piPlaySpeedCount.ToString() + ")";
                }
            }
        }

        /// <summary>
        /// 皮重视频停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piButtonPlayStop_Click(object sender, RoutedEventArgs e)
        {
            if (piVideoPlayBackController != null)
            {
                if (piVideoPlayBackController.PlayStop())
                {
                    piPlaySpeedCount = 0;
                    this.piButtonPlayFast.Content = "快进";
                }
            }
        }
        #endregion

        #region 毛重视频回放相关
        private void maoVideoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ReadNVRConfig();
            OpenMaoVideo();
        }

        /// <summary>
        /// 毛重视频播放
        /// </summary>
        private void OpenMaoVideo()
        {
            if (this.showType == 0 || this.showType == 2)
            {
                if (weightModel != null && !string.IsNullOrEmpty(weightModel.grosstime))
                {
                    if (maoVideoPlayBackController != null)
                    {
                        maoVideoPlayBackController.PlayStop();
                    }
                    DateTime startTime = dptMZPlayStartTime.Value.Value; // DateTime.Parse(weightModel.JiMaoDate);
                    DateTime endTime = dptMZPlayEndTime.Value.Value; //startTime.AddMinutes(5);//计毛结束时间不晓得服务那边是否有，暂时先给计毛时间加5分钟作为计毛结束时间
                    if (endTime <= startTime) return;
                    var getCamera = maoVideoList.SelectedItem as Camera;
                    if (getCamera != null)
                    {
                        //Talent.ClinetLog.SysLog.Log("选择了视频:" + getCamera.VideoName);
                        if (this.maoVideoConfig != null)
                        {
                            VideoConfig gCfg = InfoExchange.Clone<VideoConfig>(this.maoVideoConfig);
                            gCfg.CameraList.Clear();
                            //Talent.ClinetLog.SysLog.Log("清空摄像头列表");
                            getCamera.Position = "1";

                            getCamera.Ip = maoNvrConfig.Ip;
                            getCamera.Port = maoNvrConfig.Port;
                            getCamera.UserName = maoNvrConfig.UserName;
                            getCamera.PassWord = maoNvrConfig.PassWord;
                            //Talent.ClinetLog.SysLog.Log(string.Format("摄像头信息赋值,Ip={0};Port={1}", getCamera.Ip, getCamera.Port));
                            gCfg.CameraList.Add(getCamera);
                            //Talent.ClinetLog.SysLog.Log("准备构造摄像头Controller");
                            maoVideoPlayBackController = new VideoPlayBackController(gCfg, maoVideoHis.Handle);
                            //Talent.ClinetLog.SysLog.Log("构造摄像头Controller完毕,准备设置播放录像的时间段");
                            maoVideoPlayBackController.PlayByTime(startTime, endTime);
                            //Talent.ClinetLog.SysLog.Log("设置播放录像的时间段完毕");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 毛重视频暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maoButtonPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (maoVideoPlayBackController != null)
            {
                isMaoPause = !isMaoPause;
                if (isMaoPause)
                {
                    maoVideoPlayBackController.PlayPause();
                    this.maoButtonPlayPause.Content = "播放";
                }
                else
                {
                    maoVideoPlayBackController.PlayStart();//此功能视频底层暂时未实现
                    this.maoButtonPlayPause.Content = "暂停";
                }
            }
        }

        /// <summary>
        /// 毛重视频快进
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maoButtonPlayFast_Click(object sender, RoutedEventArgs e)
        {
            if (maoVideoPlayBackController != null)
            {
                if (maoVideoPlayBackController.PlayFast())
                {
                    maoPlaySpeedCount++;
                    this.maoButtonPlayFast.Content = "快进(" + maoPlaySpeedCount.ToString() + ")";
                }
            }
        }
        /// <summary>
        /// 毛重视频快退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maoButtonPlaySlow_Click(object sender, RoutedEventArgs e)
        {
            if (maoVideoPlayBackController != null)
            {
                if (maoVideoPlayBackController.PlaySlow())
                {
                    maoPlaySpeedCount--;
                    this.maoButtonPlayFast.Content = "快进(" + maoPlaySpeedCount.ToString() + ")";
                }
            }
        }

        /// <summary>
        /// 毛重视频停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maoButtonPlayStop_Click(object sender, RoutedEventArgs e)
        {
            if (maoVideoPlayBackController != null)
            {
                if (maoVideoPlayBackController.PlayStop())
                {
                    maoPlaySpeedCount = 0;
                    this.maoButtonPlayFast.Content = "快进";
                }
            }
        }
        /// <summary>
        /// 窗体关闭的时候
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            if (maoVideoPlayBackController != null)
            {
                maoVideoPlayBackController.PlayStop();
            }
            if (piVideoPlayBackController != null)
            {
                piVideoPlayBackController.PlayStop();
            }
            base.OnClosed(e);
        }
        #endregion
        /// <summary>
        /// 加载默认开始结束时间
        /// </summary>
        private void LoadDefaultTime()
        {
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["querytimeBymatchid"].ToString().Replace('$', '&');
                string getUrl = string.Format(serviceUrl, weightModel.matchid);
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                string strResult = ComHelpClass.ResponseSynStr(request);
                var serviceModel = InfoExchange.DeConvert(typeof(WeightRealServiceModel), strResult) as WeightRealServiceModel;
                DateTime tareStartTime = DateTime.Now.AddSeconds(-30);
                DateTime tareEndTime = DateTime.Now.AddSeconds(30);
                DateTime grossStartTime = DateTime.Now.AddSeconds(-30);
                DateTime grossEndTime = DateTime.Now.AddSeconds(30);
                GetDefaultTime(serviceModel, ref tareStartTime, ref tareEndTime, ref grossStartTime, ref grossEndTime);
                dptPZPlayStartTime.Value = tareStartTime;
                dptPZPlayEndTime.Value = tareEndTime;
                dptMZPlayStartTime.Value = grossStartTime;
                dptMZPlayEndTime.Value = grossEndTime;
            }
            catch //(Exception ex)
            {

            }

        }
        /// <summary>
        /// 获取默认时间
        /// </summary>
        /// <param name="serviceModel"></param>
        /// <param name="tareStartTime"></param>
        /// <param name="tareEndTime"></param>
        /// <param name="grossStartTime"></param>
        /// <param name="grossEndTime"></param>
        private void GetDefaultTime(WeightRealServiceModel serviceModel, ref DateTime tareStartTime, ref DateTime tareEndTime, ref DateTime grossStartTime, ref DateTime grossEndTime)
        {

            try
            {
                decimal tareStartTime_d = 10160101010101;
                decimal tareEndTime_d = 10160101010101;
                decimal grossStartTime_d = 10160101010101;
                decimal grossEndTime_d = 10160101010101;
                List<WeightRealData> listRows = serviceModel.rows;
                string tareTime = weightModel.taretime;
                string grossTime = weightModel.grosstime;
                for (int i = 0; i < listRows.Count; i++)
                {
                    string bTime = listRows[i].begintime;
                    string eTime = listRows[i].endtime;
                    decimal bTime_d = Convert.ToDecimal((Convert.ToDateTime(bTime)).ToString("yyyyMMddHHmmss"));
                    if (!string.IsNullOrEmpty(tareTime))
                    {
                        if (bTime_d < Convert.ToDecimal((Convert.ToDateTime(tareTime)).ToString("yyyyMMddHHmmss")))
                        {
                            if (tareStartTime_d < bTime_d)
                            {
                                tareStartTime_d = bTime_d;
                                tareEndTime_d = Convert.ToDecimal((Convert.ToDateTime(eTime)).ToString("yyyyMMddHHmmss"));
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(grossTime))
                    {
                        if (bTime_d < Convert.ToDecimal((Convert.ToDateTime(grossTime)).ToString("yyyyMMddHHmmss")))
                        {
                            if (grossStartTime_d < bTime_d)
                            {
                                grossStartTime_d = bTime_d;
                                grossEndTime_d = Convert.ToDecimal((Convert.ToDateTime(eTime)).ToString("yyyyMMddHHmmss"));
                            }
                        }
                    }
                }
                if (tareStartTime_d > 10160101010101)
                {
                    tareStartTime = (ConvertDateTimeHelpClass.ConvertDecimalDateTime(tareStartTime_d)).AddSeconds(-5);
                    tareEndTime = (ConvertDateTimeHelpClass.ConvertDecimalDateTime(tareEndTime_d)).AddSeconds(5);
                }
                if (grossStartTime_d > 10160101010101)
                {
                    grossStartTime = (ConvertDateTimeHelpClass.ConvertDecimalDateTime(grossStartTime_d)).AddSeconds(-5);
                    grossEndTime = (ConvertDateTimeHelpClass.ConvertDecimalDateTime(grossEndTime_d)).AddSeconds(5);
                }
            }
            catch //(Exception ex)
            {

            }


        }
    }
}
