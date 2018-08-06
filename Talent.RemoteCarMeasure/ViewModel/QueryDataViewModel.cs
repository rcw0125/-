using Microsoft.Expression.Interactivity.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Talent.ClientCommMethod;
using Talent.ClientCommonLib;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.Measure.DomainModel.ServiceModel;
using Talent.RemoteCarMeasure.Commom;
using Talent.RemoteCarMeasure.Model;
using Talent.RemoteCarMeasure.View;
using Talent.Measure.WPF;
using Talent.Measure.WPF.Remote;
using Talent.Measure.WPF.Log;

namespace Talent.RemoteCarMeasure.ViewModel
{
    /// <summary>
    /// 数据查询viewModel
    /// </summary>
    public class QueryDataViewModel : Only_ViewModelBase
    {
        #region 属性
        /// <summary>
        /// 弹出的信息子窗体是否已经打开
        /// </summary>
        bool isShowChildFormMsg = false;

        /// <summary>
        /// 等待打印结果计时器
        /// </summary>
        private Calculagraph waitPrintReusltTimer;

        private string _tab1Title;

        /// <summary>
        /// 个人统计tab标签名称
        /// </summary>
        public string Tab1Title
        {
            get { return _tab1Title; }
            set { _tab1Title = value; }
        }

        private ObservableCollection<SeatAttentionWeightModel> carMeasures;
        /// <summary>
        /// 衡器集合
        /// </summary>
        public ObservableCollection<SeatAttentionWeightModel> CarMeasures
        {
            get { return carMeasures; }
            set
            {
                carMeasures = value;
                this.RaisePropertyChanged("CarMeasures");
            }
        }
        private bool isAllClient = false;
        /// <summary>
        /// 是否所有衡器
        /// </summary>
        public bool IsAllClient
        {
            get { return isAllClient; }
            set
            {
                isAllClient = value;
                this.RaisePropertyChanged("IsAllClient");
            }
        }
        private SeatAttentionWeightModel selectedCarMeasure;
        /// <summary>
        /// 选择的衡器
        /// </summary>
        public SeatAttentionWeightModel SelectedCarMeasure
        {
            get { return selectedCarMeasure; }
            set
            {
                selectedCarMeasure = value;
                this.RaisePropertyChanged("SelectedCarMeasure");
            }
        }


        private DateTime startDateTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDateTime
        {
            get { return startDateTime; }
            set
            {
                startDateTime = value;
                this.RaisePropertyChanged("StartDateTime");
            }
        }

        private DateTime endDateTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime
        {
            get { return endDateTime; }
            set
            {
                endDateTime = value;
                this.RaisePropertyChanged("EndDateTime");
            }
        }

        private string businessId;
        /// <summary>
        /// 业务id
        /// </summary>
        public string BusinessId
        {
            get { return businessId; }
            set
            {
                businessId = value;
                this.RaisePropertyChanged("BusinessId");
            }
        }

        private string carNum;
        /// <summary>
        /// 车号
        /// </summary>
        public string CarNum
        {
            get { return carNum; }
            set
            {
                carNum = value;
                this.RaisePropertyChanged("CarNum");
            }
        }

        private List<WeightRecord> jingRecords;
        /// <summary>
        /// 净重集合
        /// </summary>
        public List<WeightRecord> JingRecords
        {
            get { return jingRecords; }
            set
            {
                jingRecords = value;
                this.RaisePropertyChanged("JingRecords");
            }
        }

        private WeightRecord selectedJingRecord;
        /// <summary>
        /// 选择的净重记录
        /// </summary>
        public WeightRecord SelectedJingRecord
        {
            get { return selectedJingRecord; }
            set
            {
                selectedJingRecord = value;
                this.RaisePropertyChanged("SelectedJingRecord");
                if (value == null)
                {
                    if (SelectedTabControlIndex == 0)
                    {
                        this.JingScrollViewerVisibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private List<WeightRecord> maoRecords;
        /// <summary>
        /// 毛重集合
        /// </summary>
        public List<WeightRecord> MaoRecords
        {
            get { return maoRecords; }
            set
            {
                maoRecords = value;
                this.RaisePropertyChanged("MaoRecords");
            }
        }

        private WeightRecord selectedMaoRecord;
        /// <summary>
        /// 选择的毛重记录
        /// </summary>
        public WeightRecord SelectedMaoRecord
        {
            get { return selectedMaoRecord; }
            set
            {
                selectedMaoRecord = value;
                this.RaisePropertyChanged("SelectedMaoRecord");
                if (value == null)
                {
                    if (SelectedTabControlIndex == 1)
                    {
                        this.MZMaoScrollViewerVisibility = Visibility.Collapsed;
                    }
                }
            }
        }


        private List<WeightRecord> piRecords;
        /// <summary>
        /// 皮重集合
        /// </summary>
        public List<WeightRecord> PiRecords
        {
            get { return piRecords; }
            set
            {
                piRecords = value;
                this.RaisePropertyChanged("PiRecords");
            }
        }

        private WeightRecord selectedPiRecord;
        /// <summary>
        /// 选择的皮重记录
        /// </summary>
        public WeightRecord SelectedPiRecord
        {
            get { return selectedPiRecord; }
            set
            {
                selectedPiRecord = value;
                this.RaisePropertyChanged("SelectedPiRecord");
                if (value == null)
                {
                    if (SelectedTabControlIndex == 2)
                    {
                        this.PZPiScrollViewerVisibility = Visibility.Collapsed;
                    }
                }
            }
        }

        #region 各Tab中滚动条控件可见性
        private Visibility mZMaoScrollViewerVisibility;
        /// <summary>
        /// 毛重记录中毛重滚动条是否显示
        /// </summary>
        public Visibility MZMaoScrollViewerVisibility
        {
            get { return mZMaoScrollViewerVisibility; }
            set
            {
                mZMaoScrollViewerVisibility = value;
                this.RaisePropertyChanged("MZMaoScrollViewerVisibility");
            }
        }

        private Visibility pZPiScrollViewerVisibility;
        /// <summary>
        /// 皮重记录中皮重滚动条是否显示
        /// </summary>
        public Visibility PZPiScrollViewerVisibility
        {
            get { return pZPiScrollViewerVisibility; }
            set
            {
                pZPiScrollViewerVisibility = value;
                this.RaisePropertyChanged("PZPiScrollViewerVisibility");
            }
        }

        private Visibility jingScrollViewerVisibility;
        /// <summary>
        /// 净重记录中滚动条是否显示
        /// </summary>
        public Visibility JingScrollViewerVisibility
        {
            get { return jingScrollViewerVisibility; }
            set
            {
                jingScrollViewerVisibility = value;
                this.RaisePropertyChanged("JingScrollViewerVisibility");
            }
        }

        #endregion

        #region 各Tab中图片数据集
        private List<PictureModel> maoPictures;
        /// <summary>
        /// 毛重图片集合
        /// </summary>
        public List<PictureModel> MaoPictures
        {
            get { return maoPictures; }
            set
            {
                maoPictures = value;
                this.RaisePropertyChanged("MaoPictures");
            }
        }

        private List<PictureModel> piPictures;
        /// <summary>
        /// 皮重图片集合
        /// </summary>
        public List<PictureModel> PiPictures
        {
            get { return piPictures; }
            set
            {
                piPictures = value;
                this.RaisePropertyChanged("PiPictures");
            }
        }


        #endregion

        private int selectedTabControlIndex;
        /// <summary>
        /// 选择的tabcontrol的index值
        /// </summary>
        public int SelectedTabControlIndex
        {
            get { return selectedTabControlIndex; }
            set
            {
                selectedTabControlIndex = value;
                this.RaisePropertyChanged("SelectedTabControlIndex");
                if (this.SelectedCarMeasure != null || this.IsAllClient)
                {
                    if (value == 0)
                    {
                        //查询净
                        GetJingRecords();
                    }
                    else if (value == 1)
                    {
                        //查询毛
                        GetMaoRecords();
                    }
                    else if (value == 2)
                    {
                        //查询皮
                        GetPiRecords();
                    }
                }
            }
        }

        private int weightSelectedIndex = -1;
        /// <summary>
        /// 坐席主窗体中,选择的衡器在tabControl里的Index值
        /// </summary>
        public int WeightSelectedIndex
        {
            get { return weightSelectedIndex; }
            set
            {
                weightSelectedIndex = value;
                if (value == 0)
                {
                    equType = EquTypeEnum.Type_Car_Seat;
                }//else 如果坐席主窗体选择的是其他衡器(非汽车衡).......
            }
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        private string equType = string.Empty;
        /// <summary>
        /// 选择的称点(远程补票使用)
        /// </summary>
        private SeatAttentionWeightModel selectedMeasure;
        /// <summary>
        /// 打印机状态
        /// </summary>
        string printError = string.Empty;
        #endregion

        #region 命令
        public ICommand SelectMeasureCommand { get; private set; }
        /// <summary>
        /// 查询命令
        /// </summary>
        public ICommand QueryCommand { get; private set; }
        /// <summary>
        /// 远程补票
        /// </summary>
        public ICommand SupplementCommand { get; private set; }
        /// <summary>
        /// 查看图片
        /// </summary>
        public ICommand ShowPictureCommand { get; private set; }
        /// <summary>
        /// 查看录像
        /// </summary>
        public ICommand ShowVideoCommand { get; private set; }
        #endregion
        /// <summary>
        /// 日志记录
        /// </summary>
        LogsHelpClass logH = new LogsHelpClass();
        public QueryDataViewModel()
        {
            if (this.IsInDesignMode)
                return;
            SelectMeasureCommand = new ActionCommand(SelectMethod);
            QueryCommand = new ActionCommand(QueryWeightRecords);
            SupplementCommand = new ActionCommand(SupplementMethod);
            ShowPictureCommand = new ActionCommand(ShowPictureMethod);
            ShowVideoCommand = new ActionCommand(ShowVideoMethod);

            #region 获取配置文件路径
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ClientConfig");
            var configPath = System.IO.Path.Combine(basePath, "SystemConfig.xml");
            #endregion

            #region 获取等待任务结果过时时间
            int waitPrintResultTime = 1;
            string waitPrintResultTimeItem = ConfigurationManager.AppSettings["WaitPrintResultTime"].ToString();
            string getWaitPrintResultTimeItem = XpathHelper.GetValue(configPath, waitPrintResultTimeItem);
            if (!string.IsNullOrEmpty(getWaitPrintResultTimeItem))
            {
                waitPrintResultTime = Convert.ToInt32(getWaitPrintResultTimeItem);
            }
            #endregion

            #region 注册等待打印结果计时器
            waitPrintReusltTimer = new Calculagraph("");
            waitPrintReusltTimer.Timeout = waitPrintResultTime;
            waitPrintReusltTimer.TimeOver += new TimeoutCaller(waitPrintReusltTimer_TimeOver);
            #endregion
        }

        #region 方法
        /// <summary>
        /// 记录类型(G:计毛;S:净重;T:计皮)
        /// </summary>
        private string measureType;

        /// <summary>
        /// 查看图片方法
        /// </summary>
        /// <param name="obj"></param>
        private void ShowPictureMethod()
        {
            this.ShowBusy = true;
            if (SelectedTabControlIndex == 0)
            {
                if (this.SelectedJingRecord == null)
                {
                    this.ShowBusy = false;
                    this.ShowMessage("提示", "请先选择一条净重记录信息!", true, false);
                    return;
                }
                //根据选择的净重记录，查询到该条记录对应的计皮、计毛图片信息集合；服务暂无，暂时空着。
                ShowCurTabScrollViewer();
                GetJingRecordImages();
            }
            else if (SelectedTabControlIndex == 1)
            {
                if (this.SelectedMaoRecord == null)
                {
                    this.ShowBusy = false;
                    this.ShowMessage("提示", "请先选择一条毛重记录信息!", true, false);
                    return;
                }
                //根据选择的毛重记录，查询到该条记录对应的计毛时的图片信息集合
                ShowCurTabScrollViewer();
                GetMaoRecordImages();
            }
            else if (SelectedTabControlIndex == 2)
            {
                if (this.SelectedPiRecord == null)
                {
                    this.ShowBusy = false;
                    this.ShowMessage("提示", "请先选择一条皮重记录信息!", true, false);
                    return;
                }
                //根据选择的皮重记录，查询到该条记录对应的计皮图片信息集合
                ShowCurTabScrollViewer();
                GetPiRecordImages();
            }
        }

        /// <summary>
        /// 获取净重记录对应的图片列表信息
        /// </summary>
        private void GetJingRecordImages()
        {
            this.measureType = "";//净重情况下，类型为空，这样，服务器将返回皮、毛的图片
            GetImages(this.SelectedJingRecord.matchid, "", "");
        }

        /// <summary>
        /// 获取毛重记录对应的图片列表信息
        /// </summary>
        private void GetMaoRecordImages()
        {
            this.measureType = "G";
            GetImages(this.SelectedMaoRecord.matchid, this.SelectedMaoRecord.grossweighid, this.SelectedMaoRecord.grossweigh);
        }
        /// <summary>
        /// 获取皮重记录对应的图片列表信息
        /// </summary>
        private void GetPiRecordImages()
        {
            this.measureType = "T";
            GetImages(this.SelectedMaoRecord.matchid, this.SelectedMaoRecord.tareweighid, this.SelectedMaoRecord.tareweigh);
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
                measuretype = this.measureType,
                equcode = equcode,
                equname = ""
            };//存储照片时，未存储秤体名称……传递参数不再使用 2016-3-3 15:00:00……
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
                List<PictureModel> pms = InfoExchange.DeConvert(typeof(List<PictureModel>), strResult) as List<PictureModel>;
                if (pms == null || pms.Count == 0)
                {
                    this.ShowBusy = false;
                    this.ShowMessage("信息提示", "未获取到图片信息！", true, false);
                    SetCurTabScrollViewer(Visibility.Collapsed);
                    return;
                }
                if (this.measureType == "G")
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(
                    new System.Threading.ThreadStart(() =>
                        {
                            this.MaoPictures = (from r in pms where r.measuretype == "G" select r).ToList();
                            DownloadImags(this.MaoPictures);
                        }
                    ));
                }
                else if (this.measureType == "T")
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(
                   new System.Threading.ThreadStart(() =>
                       {
                           this.PiPictures = (from r in pms where r.measuretype == "T" select r).ToList();
                           DownloadImags(this.PiPictures);
                       }
                   ));
                }
                else if (this.measureType == "")//类型为空字符串，净重查询图片的结果(皮、毛图片都有)
                {

                    System.Windows.Application.Current.Dispatcher.Invoke(
                  new System.Threading.ThreadStart(() =>
                  {
                      this.PiPictures = (from r in pms where r.measuretype == "T" select r).ToList();
                      this.MaoPictures = (from r in pms where r.measuretype == "G" select r).ToList();
                      DownloadImags(this.PiPictures);
                      DownloadImags(this.MaoPictures);
                  }
                  ));

                }
                this.ShowBusy = false;
            }
            catch (Exception ex)
            {
                this.ShowBusy = false;
                this.ShowMessage("系统提示", "获取图片失败,原因:" + ex.Message, true, false);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_数据查询窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "通过服务读取计量时的图片信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            this.measureType = "-1";
        }

        /// <summary>
        /// 显示当前tab中滚动条的可见性
        /// </summary>
        private void ShowCurTabScrollViewer()
        {
            if (SelectedTabControlIndex == 0)
            {
                JingScrollViewerVisibility = Visibility.Visible;
                MZMaoScrollViewerVisibility = Visibility.Collapsed;
                PZPiScrollViewerVisibility = Visibility.Collapsed;
            }
            else if (SelectedTabControlIndex == 1)
            {
                JingScrollViewerVisibility = Visibility.Collapsed;
                MZMaoScrollViewerVisibility = Visibility.Visible;
                PZPiScrollViewerVisibility = Visibility.Collapsed;
            }
            else if (SelectedTabControlIndex == 2)
            {
                JingScrollViewerVisibility = Visibility.Collapsed;
                MZMaoScrollViewerVisibility = Visibility.Collapsed;
                PZPiScrollViewerVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 设置滚动条的可见性
        /// </summary>
        /// <param name="vb"></param>
        private void SetCurTabScrollViewer(Visibility vb)
        {
            JingScrollViewerVisibility = vb;
            MZMaoScrollViewerVisibility = vb;
            PZPiScrollViewerVisibility = vb;
        }

        /// <summary>
        /// 查询净重记录
        /// </summary>
        private void GetJingRecords()
        {
            this.ShowBusy = true;
            string serviceUrl = ConfigurationManager.AppSettings["getSearchInfo"].ToString();
            string eCode = isAllClient ? "" : this.SelectedCarMeasure.equcode;
            var Params = new
            {
                matchid = string.IsNullOrEmpty(this.BusinessId) ? "" : this.BusinessId,
                carno = string.IsNullOrEmpty(this.CarNum) ? "" : this.CarNum,
                begintime = this.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                endtime = this.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                mrecord = "S",//表示净重
                tareweighid = eCode,
                selecttime = "suttletime"
            };
            string Url = string.Format(serviceUrl, JsonConvert.SerializeObject(Params));
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_OutIn,
                FunctionName = "坐席_数据查询窗体_获取净重记录",
                Level = LogConstParam.LogLevel_Error,
                Msg = Url,
                Data = Params,
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(Url, 10);
            request.BeginGetResponse(new AsyncCallback(GetJingRecordsCallback), request);
        }

        /// <summary>
        /// 查询净重记录回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void GetJingRecordsCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                this.JingRecords = InfoExchange.DeConvert(typeof(List<WeightRecord>), strResult) as List<WeightRecord>;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_数据查询窗体_获取净重反馈的结果",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取净重反馈的结果",
                    Data = strResult,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                this.ShowBusy = false;
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_数据查询窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取净重信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    ParamList = new List<DataParam>() 
                    { 
                        new DataParam() { ParamName = "matchid", ParamValue = this.BusinessId },
                        new DataParam() { ParamName = "carno", ParamValue = this.CarNum },
                        new DataParam() { ParamName = "begintime", ParamValue = this.StartDateTime.ToString() },
                        new DataParam() { ParamName = "endtime", ParamValue = this.EndDateTime.ToString() },
                        new DataParam() { ParamName = "mrecord", ParamValue = "S" },
                        new DataParam() { ParamName = "tareweighid", ParamValue = this.SelectedCarMeasure.equcode },
                        new DataParam() { ParamName = "selecttime", ParamValue = "suttletime" }
                    }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 查询皮重记录
        /// </summary>
        private void GetPiRecords()
        {
            this.ShowBusy = true;
            string serviceUrl = ConfigurationManager.AppSettings["getSearchInfo"].ToString();
            string eCode = isAllClient ? "" : this.SelectedCarMeasure.equcode;
            var Params = new
            {
                matchid = string.IsNullOrEmpty(this.BusinessId) ? "" : this.BusinessId,
                carno = string.IsNullOrEmpty(this.CarNum) ? "" : this.CarNum,
                begintime = this.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                endtime = this.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                mrecord = "T",//表示皮重
                tareweighid = eCode,
                selecttime = "taretime"
            };
            string Url = string.Format(serviceUrl, JsonConvert.SerializeObject(Params));
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_OutIn,
                FunctionName = "坐席_数据查询窗体_获取皮重记录",
                Level = LogConstParam.LogLevel_Error,
                Msg = Url,
                Data = Params,
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(Url, 10);
            request.BeginGetResponse(new AsyncCallback(GetPiRecordsCallback), request);
        }

        /// <summary>
        /// 查询皮重回调方法
        /// </summary>
        /// <param name="asyc"></param>
        private void GetPiRecordsCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                this.PiRecords = InfoExchange.DeConvert(typeof(List<WeightRecord>), strResult) as List<WeightRecord>;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_数据查询窗体_获取皮重反馈的结果",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取皮重反馈的结果",
                    Data = strResult,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_数据查询窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取皮重信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    ParamList = new List<DataParam>()
                    { 
                        new DataParam() { ParamName = "matchid", ParamValue = this.BusinessId },
                        new DataParam() { ParamName = "carno", ParamValue = this.CarNum },
                        new DataParam() { ParamName = "begintime", ParamValue = this.StartDateTime.ToString() },
                        new DataParam() { ParamName = "endtime", ParamValue = this.EndDateTime.ToString() },
                        new DataParam() { ParamName = "mrecord", ParamValue = "T" },
                        new DataParam() { ParamName = "tareweighid", ParamValue = this.SelectedCarMeasure.equcode },
                        new DataParam() { ParamName = "selecttime", ParamValue = "taretime" }
                    }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            this.ShowBusy = false;
        }

        /// <summary>
        /// 查询毛重记录
        /// </summary>
        private void GetMaoRecords()
        {
            this.ShowBusy = true;
            string serviceUrl = ConfigurationManager.AppSettings["getSearchInfo"].ToString();
            string eCode = isAllClient ? "" : this.SelectedCarMeasure.equcode;
            var Params = new
            {
                matchid = string.IsNullOrEmpty(this.BusinessId) ? "" : this.BusinessId,
                carno = string.IsNullOrEmpty(this.CarNum) ? "" : this.CarNum,
                begintime = this.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                endtime = this.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                mrecord = "G",//表示毛重
                tareweighid = eCode,
                selecttime = "grosstime"
            };
            string Url = string.Format(serviceUrl, JsonConvert.SerializeObject(Params));
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_OutIn,
                FunctionName = "坐席_数据查询窗体_获取毛重记录",
                Level = LogConstParam.LogLevel_Error,
                Msg = Url,
                Data = Params,
                Origin = "汽车衡_" + LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(Url, 10);
            request.BeginGetResponse(new AsyncCallback(GetMaoRecordsCallback), request);
        }

        /// <summary>
        /// 查询毛重回调方法
        /// </summary>
        /// <param name="asyc"></param>
        private void GetMaoRecordsCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                this.MaoRecords = InfoExchange.DeConvert(typeof(List<WeightRecord>), strResult) as List<WeightRecord>;
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_数据查询窗体_获取毛重反馈的结果",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取毛重反馈的结果",
                    Data = strResult,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席数据查询窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取毛重信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    ParamList = new List<DataParam>()
                    { 
                        new DataParam() { ParamName = "matchid", ParamValue = this.BusinessId },
                        new DataParam() { ParamName = "carno", ParamValue = this.CarNum },
                        new DataParam() { ParamName = "begintime", ParamValue = this.StartDateTime.ToString() },
                        new DataParam() { ParamName = "endtime", ParamValue = this.EndDateTime.ToString() },
                        new DataParam() { ParamName = "mrecord", ParamValue = "G" },
                        new DataParam() { ParamName = "tareweighid", ParamValue = this.SelectedCarMeasure.equcode },
                        new DataParam() { ParamName = "selecttime", ParamValue = "grosstime" }
                    }
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            this.ShowBusy = false;
        }

        /// <summary>
        /// 查询所有称量记录
        /// </summary>
        private void QueryWeightRecords()
        {
            if (!IsAllClient)
            {
                if (SelectedCarMeasure == null)
                {
                    ShowMessage("提示", "请选择衡器", true, false);
                    return;
                }
            }

            SelectedTabControlIndex = SelectedTabControlIndex;
            JingScrollViewerVisibility = Visibility.Collapsed;
            MZMaoScrollViewerVisibility = Visibility.Collapsed;
            PZPiScrollViewerVisibility = Visibility.Collapsed;
        }

        public void InitData(int weightIndex)
        {
            this.ShowBusy = true;
            this.WeightSelectedIndex = weightIndex;
            //修改为 开始时间与结束时间为 当天开始 与结束 2016-4-6 19:36:24……
            this.StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
            this.EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
            this.MaoPictures = new List<PictureModel>();
            this.PiPictures = new List<PictureModel>();
            JingScrollViewerVisibility = Visibility.Collapsed;
            MZMaoScrollViewerVisibility = Visibility.Collapsed;
            PZPiScrollViewerVisibility = Visibility.Collapsed;
            GetCarMeasuresFromServer();
        }

        /// <summary>
        /// 从服务器获取衡器信息
        /// </summary>
        private void GetCarMeasuresFromServer()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getSeatClient"].ToString();
            var param = new
            {
                seatname = LoginUser.Role.Name,
                seatid = LoginUser.Role.Code,
                seattype = equType
            };
            string getUrl = string.Format(serviceUrl, "[" + JsonConvert.SerializeObject(param) + "]");
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(GetCarMeasuresCallback), request);
        }

        /// <summary>
        /// 获取衡器回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void GetCarMeasuresCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                var SeatAttentionInfos = InfoExchange.DeConvert(typeof(List<SeatAttentionWeightModel>), strResult) as List<SeatAttentionWeightModel>;
                var equs = (from r in SeatAttentionInfos
                            select r).OrderBy(c => c.equcode).ToList();
                CarMeasures = new ObservableCollection<SeatAttentionWeightModel>(equs);
                this.ShowBusy = false;
            }
            catch (Exception ex)
            {
                this.ShowBusy = false;
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_数据查询窗体",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取衡器信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 衡器选择后触发的方法
        /// </summary>
        /// <param name="obj"></param>
        private void SelectMethod(object obj)
        {
            if (obj != null)
            {
                SelectedCarMeasure = obj as SeatAttentionWeightModel;
                int count = SelectedTabControlIndex;
                SelectedTabControlIndex = count;
            }
        }

        /// <summary>
        /// 远程补票
        /// </summary>
        private void SupplementMethod()
        {
            //SendPrintTicket();
            if (SelectedTabControlIndex == 0)
            {
                if (this.SelectedJingRecord == null)
                {
                    this.ShowMessage("提示", "请先选择一条净重记录信息!", true, false);
                    return;
                }
            }
            else if (SelectedTabControlIndex == 1)
            {
                if (this.SelectedMaoRecord == null)
                {
                    this.ShowMessage("提示", "请先选择一条毛重记录信息!", true, false);
                    return;
                }
            }
            else if (selectedTabControlIndex == 2)
            {
                if (this.SelectedPiRecord == null)
                {
                    this.ShowMessage("提示", "请先选择一条皮重记录信息!", true, false);
                    return;
                }
            }
            else
            {
                this.ShowMessage("提示", "请先选择一条净重、毛重或皮重记录信息!", true, false);
                return;
            }
            SelectMeasureView smv = new SelectMeasureView(this.CarMeasures.ToList());
            smv.ShowDialog();
            if (smv.FormState == 1)
            {
                selectedMeasure = smv.SelectedMeasure;
                SendPrintTicket(smv.memoInfos);
            }
        }

        /// <summary>
        /// 向任务服务器发送打印票据信息
        /// </summary>
        private void SendPrintTicket(string userMemo)
        {
            string carNo = string.Empty;
            string matchid = string.Empty;
            if (SelectedTabControlIndex == 0)
            {
                carNo = this.SelectedJingRecord.carno;
                matchid = this.SelectedJingRecord.matchid;
            }
            else if (SelectedTabControlIndex == 1)
            {
                carNo = this.SelectedMaoRecord.carno;
                matchid = this.SelectedMaoRecord.matchid;
            }
            if (string.IsNullOrEmpty(userMemo))
            {
                userMemo = "补打";
            }
            PrintInfo pm = new PrintInfo()
            {
                carno = carNo,
                clientcode = this.selectedMeasure.equcode,
                clientname = this.selectedMeasure.equname,
                matchid = matchid,
                opcode = LoginUser.Code,
                opname = LoginUser.Name,
                printtype = userMemo + "(" + LoginUser.Code + ")",
                TicketType = SelectedTabControlIndex
            };

            int unm = CommonMethod.CommonMethod.GetRandom();
            var para = new
            {
                clientid = this.selectedMeasure.equcode,
                cmd = ParamCmd.Supplement,
                msg = pm,
                msgid = unm
            };
            SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
            SocketCls.listenEvent += SocketCls_listenEvent;
            isShowChildFormMsg = false;
            waitPrintReusltTimer.Start();
        }

        /// <summary>
        /// 查看录像
        /// </summary>
        private void ShowVideoMethod()
        {
            if (this.SelectedTabControlIndex == 0)
            {
                if (this.SelectedJingRecord == null)
                {
                    this.ShowMessage("提示", "请先选择一条净重记录信息!", true, false);
                    return;
                }
                ShowVideoTapeView videoView = new ShowVideoTapeView(this.SelectedJingRecord, 0);
                videoView.ShowDialog();
            }
            else if (this.SelectedTabControlIndex == 1)
            {
                if (this.SelectedMaoRecord == null)
                {
                    this.ShowMessage("提示", "请先选择一条毛重记录信息!", true, false);
                    return;
                }
                ShowVideoTapeView videoView = new ShowVideoTapeView(this.SelectedMaoRecord, 2);
                videoView.ShowDialog();
            }
            else if (this.SelectedTabControlIndex == 2)
            {
                if (this.SelectedPiRecord == null)
                {
                    this.ShowMessage("提示", "请先选择一条皮重记录信息!", true, false);
                    return;
                }
                ShowVideoTapeView videoView = new ShowVideoTapeView(this.SelectedPiRecord, 1);
                videoView.ShowDialog();
            }
        }
        #endregion
        #region 私有 新增

        #region Socket回调事件
        public void SocketCls_listenEvent(object sender, CallBackEventArgs e)
        {
            switch (e.EventName)
            {
                case "realData":
                    GetRealData(e.Message);
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 处理cmd命令
        /// </summary>
        /// <param name="msg"></param>
        private void GetRealData(string realDataJsonStr)
        {
            if (!string.IsNullOrEmpty(realDataJsonStr))
            {
                WeighterClientModel weighterClient = JsonConvert.DeserializeObject<WeighterClientModel>(realDataJsonStr);
                if (weighterClient != null)
                {
                    if (this.selectedMeasure.equcode == weighterClient.ClientId && weighterClient.EquTag != null && weighterClient.EquTag.Equals("P") && !isShowChildFormMsg)
                    {
                        #region 写打印结果的日志
                        LogModel timerLog = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_OutIn,
                            FunctionName = "坐席_数据查询窗体",
                            Level = LogConstParam.LogLevel_Error,
                            Msg = "在实时数据中获取到打印结果信息，等待打印结果计时器停止计时",
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                            OperateUserName = LoginUser.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog));
                        #endregion
                        waitPrintReusltTimer.Stop();
                        SocketCls.listenEvent -= SocketCls_listenEvent;
                        printError = weighterClient.PrintState;
                        string msg = "打印成功!";
                        if (!string.IsNullOrEmpty(printError))
                        {
                            msg = "打印失败,原因:" + printError;
                        }
                        #region 写打印结果的日志
                        LogModel timerLog1 = new LogModel()
                        {
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Direction = LogConstParam.Directions_OutIn,
                            FunctionName = "数据查询",
                            Msg = msg + ",当前isShowChildFormMsg(信息子窗体是否已经打开)值为:" + isShowChildFormMsg.ToString(),
                            Origin = "汽车衡_" + LoginUser.Role.Name,
                            OperateUserName = LoginUser.Name
                        };
                        Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog1));
                        #endregion

                        isShowChildFormMsg = true;
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            //打印失败业务处理
                            ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", msg, true, false, "确定", "");
                            confirmBox.ShowDialog();
                        }));
                        isShowChildFormMsg = false;
                    }
                }
            }
        }

        /// <summary>
        /// 等待打印结果计时器时间到触发的事件
        /// </summary>
        /// <param name="userdata"></param>
        private void waitPrintReusltTimer_TimeOver(object userdata)
        {
            try
            {
                #region 写计时器计时结束的日志
                LogModel timerLog = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_数据查询窗体_等待打印结果计时器时间到触发的事件",
                    Level = LogConstParam.LogLevel_Warning,
                    Msg = "等待打印结果计时器计时时间到,isShowChildFormMsg(当前信息框是否已经弹出)值为:" + isShowChildFormMsg.ToString(),
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(timerLog));
                #endregion

                if (!isShowChildFormMsg)
                {
                    bool isWait = false;//是否等待
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        isShowChildFormMsg = true;
                        ConfirmMessageBox confirmBox = new ConfirmMessageBox("系统提示", "等待" + waitPrintReusltTimer.Timeout + "秒依然无打印结果.", true, true, "继续等待", "关闭");
                        confirmBox.ShowDialog();
                        isWait = confirmBox.IsOk;
                        isShowChildFormMsg = false;
                    }));
                    if (isWait)
                    {
                        waitPrintReusltTimer.Start();
                        //SocketCls.listenEvent += SocketCls_listenEvent;
                    }
                    else
                    {
                        SocketCls.listenEvent -= SocketCls_listenEvent;
                    }
                }
            }
            catch (Exception)
            {

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
        #endregion
    }
}
