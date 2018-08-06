using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Talent.ClientCommMethod;
using Talent.ClientCommonLib;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.CommonMethod;
using System.Configuration;
using System.Net;
using System.IO;
using Talent.Measure.DomainModel.ServiceModel.TaskStat;
using Talent.Measure.WPF.Remote;
using System.Windows.Controls.DataVisualization.Charting;

namespace Talent.RemoteCarMeasure.ViewModel
{
    public class TaskStatViewModel : Only_ViewModelBase
    {
        #region 命令
        public ICommand SelectUserCommand { get; private set; }
        /// <summary>
        /// 查询命令
        /// </summary>
        public ICommand QueryCommand { get; private set; }
        #endregion

        public TaskStatViewModel()
        {
            if (this.IsInDesignMode)
                return;
            SelectUserCommand = new ActionCommand(SelectMethod);
            QueryCommand = new ActionCommand(QueryStatRecords);

            UserList = new ObservableCollection<MeasureUserModel>();

            getAllMeasureUser();
            DateTimeBegin = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
            DateTimeEnd = DateTime.Now;

            _tabSelectedIndex = 0;

            Tab1Title = "个人统计";
        }


        #region 属性

        private Chart pjsdChart;
        /// <summary>
        /// 汇总统计中的曲线图
        /// </summary>
        public Chart PjsdChart
        {
            get { return pjsdChart; }
            set { pjsdChart = value; }
        }

        private Chart csChart;
        /// <summary>
        /// 车数图表
        /// </summary>
        public Chart CsChart
        {
            get { return csChart; }
            set { csChart = value; }
        }

        private int sumChartSelectedIndex;
        /// <summary>
        /// 汇总统计中的图表tab页
        /// </summary>
        public int SumChartSelectedIndex
        {
            get { return sumChartSelectedIndex; }
            set
            {
                sumChartSelectedIndex = value;
                this.RaisePropertyChanged("SumChartSelectedIndex");
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SetSumChart();
                }));
            }
        }

        /// <summary>
        /// tab标签选择的索引
        /// </summary>
        private int _tabSelectedIndex;
        /// <summary>
        /// tab标签选择的索引
        /// </summary>
        public int TabSelectedIndex
        {
            get { return _tabSelectedIndex; }
            set
            {
                _tabSelectedIndex = value;
                this.RaisePropertyChanged("TabSelectedIndex");
            }
        }

        private string _tab1Title;

        /// <summary>
        /// 个人统计tab标签名称
        /// </summary>
        public string Tab1Title
        {
            get { return _tab1Title; }
            set
            {

                _tab1Title = value;
                this.RaisePropertyChanged("Tab1Title");
            }
        }

        private DateTime _dateTimeBegin;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime DateTimeBegin
        {
            get { return _dateTimeBegin; }
            set
            {
                _dateTimeBegin = value;
                this.RaisePropertyChanged("DateTimeBegin");
            }
        }

        private DateTime _dateTimeEnd;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime DateTimeEnd
        {
            get { return _dateTimeEnd; }
            set
            {
                _dateTimeEnd = value;
                this.RaisePropertyChanged("DateTimeEnd");
            }
        }

        public ObservableCollection<MeasureUserModel> _userList;
        /// <summary>
        /// 坐席列表
        /// </summary>
        public ObservableCollection<MeasureUserModel> UserList
        {
            get
            {
                return _userList;
            }
            set
            {
                _userList = value;
                this.RaisePropertyChanged("UserList");
            }
        }

        private MeasureUserModel _selectedUser;
        /// <summary>
        /// 已选择的坐席
        /// </summary>
        public MeasureUserModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                this.RaisePropertyChanged("SelectedUser");
            }
        }
        private ObservableCollection<ChartClass> _chartClassList3;

        public ObservableCollection<ChartClass> ChartClassList3
        {
            get { return _chartClassList3; }
            set
            {
                _chartClassList3 = value;
                this.RaisePropertyChanged("ChartClassList3");
            }
        }
        private ObservableCollection<ChartClass> _chartClassList2;
        /// <summary>
        /// 人员计量任务汇总统计图表
        /// </summary>
        public ObservableCollection<ChartClass> ChartClassList2
        {
            get { return _chartClassList2; }
            set
            {
                _chartClassList2 = value;
                this.RaisePropertyChanged("ChartClassList2");
            }
        }
        private ObservableCollection<ChartClass> _chartClassList1;
        /// <summary>
        /// 计量员任务流水数据
        /// </summary>
        public ObservableCollection<ChartClass> ChartClassList1
        {
            get { return _chartClassList1; }
            set
            {
                _chartClassList1 = value;
                this.RaisePropertyChanged("ChartClassList1");
            }
        }
        private ObservableCollection<PersonalStatisticsModel> _personalStatisticsModelList;
        /// <summary>
        /// 个人统计
        /// </summary>
        public ObservableCollection<PersonalStatisticsModel> PersonalStatisticsModelList
        {
            get { return _personalStatisticsModelList; }
            set
            {
                _personalStatisticsModelList = value;
                this.RaisePropertyChanged("PersonalStatisticsModelList");
            }
        }

        private ObservableCollection<SummaryStatisticsModel> _summaryStatisticsModelList;
        /// <summary>
        /// 汇总统计
        /// </summary>
        public ObservableCollection<SummaryStatisticsModel> SummaryStatisticsModelList
        {
            get { return _summaryStatisticsModelList; }
            set { _summaryStatisticsModelList = value; this.RaisePropertyChanged("SummaryStatisticsModelList"); }
        }
        private ObservableCollection<WeighingApparatusStatisticsModel> _weighingApparatusStatisticsModelList;
        /// <summary>
        /// 衡器统计
        /// </summary>
        public ObservableCollection<WeighingApparatusStatisticsModel> WeighingApparatusStatisticsModelList
        {
            get { return _weighingApparatusStatisticsModelList; }
            set { _weighingApparatusStatisticsModelList = value; this.RaisePropertyChanged("WeighingApparatusStatisticsModelList"); }
        }
        #endregion

        #region 选择坐席
        /// <summary>
        /// 选择坐席
        /// </summary>
        /// <param name="obj"></param>
        private void SelectMethod(object obj)
        {
            if (obj != null)
            {
                _selectedUser = obj as MeasureUserModel;
                if (_selectedUser != null && _userList != null)
                {
                    string temp = string.Format("{0}的个人统计", _selectedUser.DisplayName, _userList.Count);
                    Tab1Title = temp;
                }
                //QueryStatRecords();
            }
        }

        /// <summary>
        /// 报表查询
        /// </summary>
        private void QueryStatRecords()
        {

            //if (SelectedUser == null)
            //{
            //    ShowMessage("提示", "请选择zuo", true, false);
            //    return;
            //}
            if (TabSelectedIndex == 0)
            {
                #region 个人统计
                //PersonalStatisticsModelList = new ObservableCollection<PersonalStatisticsModel>();
                getTaskDoResult();
                //PersonalStatisticsModelList.Add(new PersonalStatisticsModel() { TaskStartTime = DateTime.Now.AddHours(-1), WeighingApparatusName = "1号汽车衡", BusinessName = "", TruckNo = "陕A12345", TaskDuration = "20" });
                //PersonalStatisticsModelList.Add(new PersonalStatisticsModel() { TaskStartTime = DateTime.Now.AddHours(-2), WeighingApparatusName = "3号汽车衡", BusinessName = "", TruckNo = "陕B12345", TaskDuration = "30" });
                //PersonalStatisticsModelList.Add(new PersonalStatisticsModel() { TaskStartTime = DateTime.Now.AddHours(-3), WeighingApparatusName = "4号汽车衡", BusinessName = "", TruckNo = "陕C12345", TaskDuration = "50" });
                //PersonalStatisticsModelList.Add(new PersonalStatisticsModel() { TaskStartTime = DateTime.Now.AddHours(-4), WeighingApparatusName = "2号汽车衡", BusinessName = "", TruckNo = "陕D12345", TaskDuration = "23" });
                #endregion
            }
            else if (TabSelectedIndex == 1)
            {
                #region 汇总统计
                getAllTaskDoResultAvgTime();
                #endregion
            }
            else if (TabSelectedIndex == 2)
            {
                #region 衡器统计
                //WeighingApparatusStatisticsModelList = new ObservableCollection<WeighingApparatusStatisticsModel>();
                //WeighingApparatusStatisticsModelList.Add(new WeighingApparatusStatisticsModel() { WeighingApparatusName = "1号汽车衡", TareTruckNum = "10", GrossWeightTruckNum = "59", TotalTruckNum = "69" });
                //WeighingApparatusStatisticsModelList.Add(new WeighingApparatusStatisticsModel() { WeighingApparatusName = "2号汽车衡", TareTruckNum = "55", GrossWeightTruckNum = "63", TotalTruckNum = "118" });
                //WeighingApparatusStatisticsModelList.Add(new WeighingApparatusStatisticsModel() { WeighingApparatusName = "3号汽车衡", TareTruckNum = "20", GrossWeightTruckNum = "46", TotalTruckNum = "66" });
                //WeighingApparatusStatisticsModelList.Add(new WeighingApparatusStatisticsModel() { WeighingApparatusName = "4号汽车衡", TareTruckNum = "40", GrossWeightTruckNum = "30", TotalTruckNum = "70" });
                getCarWeightTaskCount();
                #endregion
            }
        }
        #endregion

        #region 绑定计量员列表
        /// <summary>
        /// 获取计量员集合
        /// </summary>
        private void getAllMeasureUser()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getAllMeasureUser"].Trim();
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(serviceUrl, 10);
            request.BeginGetResponse(new AsyncCallback(BindUserCallback), request);
        }

        /// <summary>
        /// 绑定计量员
        /// </summary>
        /// <param name="asyc"></param>
        private void BindUserCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                UserList = InfoExchange.DeConvert(typeof(ObservableCollection<MeasureUserModel>), strResult) as ObservableCollection<MeasureUserModel>;
            }
            catch //(Exception ex)
            {
            }
        }
        #endregion

        #region 绑定衡器统计数据
        /// <summary>
        /// 获取计量员集合
        /// </summary>
        private void getCarWeightTaskCount()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getCarWeightTaskCount"].Trim();
            var temp = new { equcode = "", equname = "", begintime = DateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss"), endtime = DateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss") };

            string jsonStr = InfoExchange.ConvertToJsonIgnoreRef1(temp);

            serviceUrl = string.Format(serviceUrl, jsonStr);
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(serviceUrl, 10);
            request.BeginGetResponse(new AsyncCallback(BindCarWeightTaskCount), request);
        }

        /// <summary>
        /// 绑定计量员
        /// </summary>
        /// <param name="asyc"></param>
        private void BindCarWeightTaskCount(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                WeighingApparatusStatisticsModelList = InfoExchange.DeConvert(typeof(ObservableCollection<WeighingApparatusStatisticsModel>), strResult) as ObservableCollection<WeighingApparatusStatisticsModel>;
                //构造统计图信息
                var chartData = from r in WeighingApparatusStatisticsModelList
                                select new ChartClass()
                                {
                                    Name = r.equname,
                                    Value = int.Parse(r.TotalCount)
                                };
                List<ChartClass> tempList = chartData.ToList<ChartClass>();
                ChartClassList3 = new ObservableCollection<ChartClass>();
                tempList.ForEach(m =>
                {
                    ChartClassList3.Add(m);
                });
            }
            catch //(Exception ex)
            {
            }
        }
        #endregion

        #region 绑定计量员任务汇总统计
        /// <summary>
        /// 获取计量员集合
        /// </summary>
        private void getAllTaskDoResultAvgTime()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getAllTaskDoResultAvgTime"].Trim();
            var temp = new { taskbegintime = DateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss"), taskendtime = DateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss") };

            string jsonStr = "[" + InfoExchange.ConvertToJsonIgnoreRef1(temp) + "]";

            serviceUrl = string.Format(serviceUrl, jsonStr);
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(serviceUrl, 10);
            request.BeginGetResponse(new AsyncCallback(BindAllTaskDoResultAvgTime), request);
        }

        /// <summary>
        /// 绑定计量员
        /// </summary>
        /// <param name="asyc"></param>
        private void BindAllTaskDoResultAvgTime(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                List<SummaryStatisticsModel> ssms = InfoExchange.DeConvert(typeof(List<SummaryStatisticsModel>), strResult) as List<SummaryStatisticsModel>;
                if (ssms != null && ssms.Count > 0)
                {
                    SummaryStatisticsModelList = new ObservableCollection<SummaryStatisticsModel>(ssms.OrderBy(r => r.opname).ToList());
                }
                else
                {
                    SummaryStatisticsModelList = new ObservableCollection<SummaryStatisticsModel>();
                }
                SumChartSelectedIndex = 0;
            }
            catch //(Exception ex)
            {
            }
        }
        #endregion

        /// <summary>
        /// 设置统计曲线图
        /// </summary>
        private void SetSumChart()
        {
            if (PjsdChart.Series.Count > 0)
            {
                PjsdChart.Series.Clear();
            }
            if (CsChart.Series.Count > 0)
            {
                CsChart.Series.Clear();
            }
            if (SummaryStatisticsModelList != null && SummaryStatisticsModelList.Count > 0)
            {
                var results = _summaryStatisticsModelList.Select(r => r.taskdoresult).Distinct();
                foreach (var result in results)
                {
                    List<ChartClass> ccList = null;
                    if (sumChartSelectedIndex == 0)
                    {
                        ccList = (from r in SummaryStatisticsModelList where r.taskdoresult == result select new ChartClass { Name = r.opname, Value = r.timecount }).ToList();
                    }
                    else if (sumChartSelectedIndex == 1)
                    {
                        ccList = (from r in SummaryStatisticsModelList where r.taskdoresult == result select new ChartClass { Name = r.opname, Value = r.totalcount }).ToList();
                    }
                    LineSeries serie = new LineSeries();
                    serie.ItemsSource = ccList;
                    serie.IndependentValueBinding = new System.Windows.Data.Binding("Name");
                    serie.DependentValueBinding = new System.Windows.Data.Binding("Value");
                    if (string.IsNullOrEmpty(result))
                    {
                        serie.Title = "空";
                    }
                    else
                    {
                        serie.Title = result;
                    }
                    if (sumChartSelectedIndex == 0)
                    {
                        PjsdChart.Series.Add(serie);
                    }
                    else if (sumChartSelectedIndex == 1)
                    {
                        CsChart.Series.Add(serie);
                    }
                }
            }
        }

        #region 计量员计量任务流水
        /// <summary>
        /// 获取计量任务流水数据
        /// </summary>
        private void getTaskDoResult()
        {
            if (SelectedUser == null || string.IsNullOrWhiteSpace(SelectedUser.LogName)) return;
            string serviceUrl = ConfigurationManager.AppSettings["getTaskDoResult"].Trim();
            var temp = new
            {
                equcode = "",
                equname = "",
                equtype = "",
                opname = "",
                opcode = SelectedUser.LogName,
                carno = "",
                taskdoresult = "",
                memo = "",
                taskbegintime = DateTimeBegin.ToString("yyyy-MM-dd HH:mm:ss"),
                taskendtime = DateTimeEnd.ToString("yyyy-MM-dd HH:mm:ss")
            };

            string jsonStr = "[" + InfoExchange.ConvertToJsonIgnoreRef1(temp) + "]";

            serviceUrl = string.Format(serviceUrl, jsonStr);
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(serviceUrl, 10);
            request.BeginGetResponse(new AsyncCallback(BindTaskDoResult), request);
        }

        /// <summary>
        /// 绑定计量任务流水
        /// </summary>
        /// <param name="asyc"></param>
        private void BindTaskDoResult(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc);
                PersonalStatisticsModelList = InfoExchange.DeConvert(typeof(ObservableCollection<PersonalStatisticsModel>), strResult) as ObservableCollection<PersonalStatisticsModel>;
                string temp = string.Format("{0}的个人统计（{1}车）", _selectedUser.DisplayName, PersonalStatisticsModelList.Count);
                //构造统计图信息
                var chartData = from r in PersonalStatisticsModelList
                                select new ChartClass()
                                {
                                    Name = r.taskbegintime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Value = Convert.ToInt32(r.timecount)
                                };
                List<ChartClass> tempList = chartData.ToList<ChartClass>();
                ChartClassList1 = new ObservableCollection<ChartClass>();

                tempList.ForEach(m =>
                {
                    ChartClassList1.Add(m);
                });
            }
            catch //(Exception ex)
            {
            }
        }
        #endregion
    }
}
