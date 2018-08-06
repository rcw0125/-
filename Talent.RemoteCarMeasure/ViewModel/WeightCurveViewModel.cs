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
using System.Windows.Input;
using Talent.ClientCommonLib;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.Measure.DomainModel.ServiceModel;
using Talent.RemoteCarMeasure.Model;
using Talent.RemoteCarMeasure.Report;
using Telerik.Reporting;
using Microsoft.Research.DynamicDataDisplay;
using Talent.Measure.WPF.Remote;

namespace Talent.RemoteCarMeasure.ViewModel
{
    public class WeightCurveViewModel : Only_ViewModelBase
    {
        #region 命令
        /// <summary>
        /// 查询命令
        /// </summary>
        public ICommand QueryCommand { get; private set; }
        #endregion

        #region 属性
        private List<WeightRealData> weightRealDataList;
        /// <summary>
        /// 实时数据集合
        /// </summary>
        public List<WeightRealData> WeightRealDataList
        {
            get { return weightRealDataList; }
            set
            {
                weightRealDataList = value;
                this.RaisePropertyChanged("WeightRealDataList");
                if (weightRealDataList.Count==0)//清空 原有展示数据 2016-3-7 11:05:43……
                {
                    ClearGridInfos();
                }
            }
        }
        private WeightRealData selectedWeightRealData;
        /// <summary>
        /// 选择的实时数据
        /// </summary>
        public WeightRealData SelectedWeightRealData
        {
            get { return selectedWeightRealData; }
            set
            {
                selectedWeightRealData = value;
                this.RaisePropertyChanged("SelectedWeightRealData");
                if (WeightRecordDataList != null && WeightRecordDataList.Count > 0)
                {
                    WeightRecordDataList.Clear();
                }
                if (value != null && !string.IsNullOrEmpty(value.realdata))
                {
                    var dataList = InfoExchange.DeConvert(typeof(List<WeightRecordData>), value.realdata) as List<WeightRecordData>;
                    if (dataList.Count > 0)
                    {
                        WeightCurveDataList = (from r in dataList select new WeightRecordModel() { recordtime = r.recordtime, recorddata = r.recorddata }).ToList();
                        WeightRecordDataList = (from r in WeightCurveDataList
                                                group r by new { r.WeightTime, r.recorddata } into g
                                                select new WeightRecordModel()
                                                {
                                                    recorddata = g.Key.recorddata,
                                                    recordtime = g.Key.WeightTime,
                                                    weightCount = g.Count(),
                                                    WeightTime = g.Key.WeightTime
                                                }).ToList();
                        IsReportShow = true;
                    }

                }
            }
        }

        private List<WeightRecordModel> weightCurveDataList;
        /// <summary>
        /// 实时数据曲线图数据源
        /// </summary>
        public List<WeightRecordModel> WeightCurveDataList
        {
            get { return weightCurveDataList; }
            set
            {
                weightCurveDataList = value;
                this.RaisePropertyChanged("WeightCurveDataList");
            }
        }

        private List<WeightRecordModel> weightRecordDataList;
        /// <summary>
        /// 实时数据明细数据源
        /// </summary>
        public List<WeightRecordModel> WeightRecordDataList
        {
            get { return weightRecordDataList; }
            set
            {
                weightRecordDataList = value;
                this.RaisePropertyChanged("WeightRecordDataList");
            }
        }

        private bool isReportShow;
        /// <summary>
        /// 用于给cs中发起传值的信号
        /// </summary>
        public bool IsReportShow
        {
            get { return isReportShow; }
            set
            {
                isReportShow = value;
                this.RaisePropertyChanged("IsReportShow");
            }
        }

        //private ReportSource weightReportSource;
        ///// <summary>
        ///// 重量报表数据源
        ///// </summary>
        //public ReportSource WeightReportSource
        //{
        //    get { return weightReportSource; }
        //    set
        //    {
        //        weightReportSource = value;
        //        this.RaisePropertyChanged("WeightReportSource");
        //    }
        //}


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
                }//else 如果坐席主窗体选择的是其他衡器.......
            }
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        private string equType = string.Empty;
        /// <summary>
        /// 测试数据对象
        /// </summary>
        private WeightRealData wrd;

        public ChartPlotter plotter;
        #endregion

        #region 构造
        public WeightCurveViewModel()
        {
            if (this.IsInDesignMode)
                return;
            QueryCommand = new ActionCommand(QueryWeightRecords);
        }
        #endregion

        #region 方法

        public void InitData(int weightIndex)
        {
            this.ShowBusy = true;
            this.WeightSelectedIndex = weightIndex;
            this.StartDateTime = DateTime.Now.AddDays(-1);
            this.EndDateTime = DateTime.Now;
            //InitTestData1();不再使用模拟数据^lt 2016-2-17 11:07:20……
            GetCarMeasuresFromServer();
        }

        /// <summary>
        /// 构建测试数据
        /// </summary>
        private void InitTestData1()
        {
            wrd = new WeightRealData() { begintime = "2015-12-16 15:17:34.50", endtime = "2015-12-16 15:17:49.800" };
            List<WeightRecordData> reDatas = new List<WeightRecordData>();
            StreamReader sr = new StreamReader(@"D:\work\远程计量\重量数据.txt", Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] lineArray = line.Split('=');
                WeightRecordData rd = new WeightRecordData() { recordtime = lineArray[0], recorddata = decimal.Parse(lineArray[1]) };
                reDatas.Add(rd);
            }
            wrd.realdata = JsonConvert.SerializeObject(reDatas);
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
                            where r.isinseat == "是"
                                && r.seatid == LoginUser.Role.Code
                                && r.seattype == equType
                            select r).OrderBy(c => c.equcode).ToList();
                CarMeasures = new ObservableCollection<SeatAttentionWeightModel>(equs);
                if (CarMeasures.Count > 0)
                {
                    this.SelectedCarMeasure = CarMeasures.First();
                }
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_重量日志窗体_获取衡器回调函数",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "获取衡器信息成功!",
                    Origin = "汽车衡_"+LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = SeatAttentionInfos,
                    IsDataValid = "有效"
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
                    FunctionName = "坐席_重量日志窗体_获取衡器回调函数",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取衡器信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            this.ShowBusy = false;
        }

        /// <summary>
        /// 查询重量信息
        /// </summary>
        /// <param name="obj"></param>
        private void QueryWeightRecords()
        {
            if (SelectedCarMeasure == null)
            {
                ShowMessage("提示", "请选择衡器", true, false);
                return;
            }
            this.ShowBusy = true;
            //InitTestData();
            //查询实时数据
            GetWeightRealDataRecords();
        }

        private void InitTestData()
        {
            WeightRealDataList = new List<WeightRealData>();
            DateTime nowTime = DateTime.Now;
            for (int i = 0; i < 5; i++)
            {
                WeightRealData data = new WeightRealData() { clientid = "102", matchid = "gbd000" + i, begintime = nowTime.AddMinutes(-5 - i).ToString("HH:mm:ss"), endtime = nowTime.AddMinutes(-1 - i).ToString("HH:mm:ss") };
                List<WeightRecordData> records = new List<WeightRecordData>();
                for (int j = 0; j < 100; j++)
                {
                    WeightRecordData recordD = new WeightRecordData();
                    recordD.recordtime = DateTime.Parse(data.begintime).AddSeconds(j / 10).ToString("HH:mm:ss");
                    recordD.recorddata = 100 * (j / 10);
                    //if (j < 30)
                    //{
                    //    recordD.recorddata = j;
                    //}
                    //else if (j >= 30 && j <= 50)
                    //{
                    //    recordD.recorddata = 50M;
                    //}
                    //else
                    //{
                    //    recordD.recorddata = 50 - (j - 50M);
                    //}
                    records.Add(recordD);
                }
                data.realdata = InfoExchange.ConvertToJson(records);
                WeightRealDataList.Add(data);
            }
        }

        /// <summary>
        /// 获取实时数据日志信息
        /// </summary>
        private void GetWeightRealDataRecords()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getRealTimeWeightInfo"].ToString().Replace('$', '&');
            var param = new
            {
                matchid = "",
                clientid = this.SelectedCarMeasure.equcode,
                begintime = this.StartDateTime,
                endtime = this.EndDateTime
            };
            string getUrl = string.Format(serviceUrl, "", this.SelectedCarMeasure.equcode, this.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss"), this.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(GetWeightRealDataRecordsCallback), request);
        }

        /// <summary>
        /// 获取实时重量数据日志信息函数
        /// </summary>
        /// <param name="asyc"></param>
        private void GetWeightRealDataRecordsCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc); 
                var serviceModel = InfoExchange.DeConvert(typeof(WeightRealServiceModel), strResult) as WeightRealServiceModel;
                WeightRealDataList = serviceModel.rows;
                #region 测试用
                if (wrd != null && !WeightRealDataList.Contains(wrd))
                {
                    WeightRealDataList.Add(wrd);
                }
                #endregion 测试用
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_重量日志窗体_获取实时重量数据日志信息函数",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取实时重量信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_"+LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            this.ShowBusy = false;
        }
        #endregion
        /// <summary>
        /// 清除界面信息
        /// </summary>
        private void ClearGridInfos()
        {
            SelectedWeightRealData = null;
            WeightRecordDataList = null;
            WeightCurveDataList = null;
            WeightRecordDataList = null;
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                plotter.Children.RemoveAll(typeof(LineGraph));
                plotter.Children.RemoveAll(typeof(MarkerPointsGraph));
                plotter.Children.RemoveAll(typeof(ElementMarkerPointsGraph));
                plotter.Children.RemoveAll(typeof(Microsoft.Research.DynamicDataDisplay.Charts.Navigation.CursorCoordinateGraph));
            }));
          
        }

    }
}
