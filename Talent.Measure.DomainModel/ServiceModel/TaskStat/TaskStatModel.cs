using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// （个人统计）计量任务流水
    /// </summary>
    public class PersonalStatisticsModel
    {
        /// <summary>
        /// 任务开始时间
        /// </summary>
        private DateTime _taskbegintime;
        public DateTime taskbegintime
        {
            get
            {
                return _taskbegintime;
            }
            set
            {
                _taskbegintime = DateTime.Parse(value.ToString());
            }
        }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string carno { get; set; }
        ///// <summary>
        ///// 业务名称
        ///// </summary>
        //public string BusinessName { get; set; }
        /// <summary>
        /// 衡器名称
        /// </summary>
        public string equname { get; set; }
        /// <summary>
        /// 任务时间
        /// </summary>
        public double timecount { get; set; }
    }
    /// <summary>
    /// 衡器（汇总统计）
    /// </summary>
    public class WeighingApparatusStatisticsModel
    {
        /// <summary>
        /// 衡器名称
        /// </summary>
        public string equname { get; set; }

        /// <summary>
        /// 计毛车数
        /// </summary>
        public string gCount { get; set; }
        /// <summary>
        /// 计皮车数
        /// </summary>
        public string tCount { get; set; }

        /// <summary>
        /// 总车数
        /// </summary>
        public string TotalCount
        {
            get
            {
                return (int.Parse(gCount) + int.Parse(tCount)).ToString();
            }
        }
    }
    /// <summary>
    /// （衡器统计）计量员计量平均时间对比
    /// </summary>
    public class SummaryStatisticsModel
    {
        /// <summary>
        /// 计量员
        /// </summary>
        public string opname { get; set; }
        /// <summary>
        /// 车数
        /// </summary>
        public double totalcount { get; set; }

        /// <summary>
        /// 平均速度
        /// </summary>
        public double timecount { get; set; }

        /// <summary>
        /// 任务处理结果 2016-3-7 08:40:56……
        /// </summary>
        public string taskdoresult { get; set; }
    }


    public class ChartClass
    {
        public ChartClass()
        {
        }
        public string Name { get; set; }

        public double Value { get; set; }//int 变为double 2016-3-7 08:49:25……
    }

    /// <summary>
    /// 任务状态模型
    /// </summary>
    public class TaskStatusModel
    {
        private string _matchid;
        /// <summary>
        /// 任务唯一标识
        /// </summary>
        public string matchid
        {
            get { return _matchid; }
            set { _matchid = value; }
        }

        private string _weightno;
        /// <summary>
        /// 衡器号
        /// </summary>
        public string weightno
        {
            get { return _weightno; }
            set { _weightno = value; }
        }

        private string _taskstatus;
        /// <summary>
        /// 任务状态,1:待处理;2:处理中;3:终止;4:完成
        /// </summary>
        public string taskstatus
        {
            get { return _taskstatus; }
            set { _taskstatus = value; }
        }

        private string _isorprint;
        /// <summary>
        /// 是否打印,0:不打印;1:打印
        /// </summary>
        public string isorprint
        {
            get { return _isorprint; }
            set { _isorprint = value; }
        }

        private string _printstaus;
        /// <summary>
        /// 打印状态,0:打印失败;1:打印成功
        /// </summary>
        public string printstaus
        {
            get { return _printstaus; }
            set { _printstaus = value; }
        }

        private string _printmsg;
        /// <summary>
        /// 打印的结果信息
        /// </summary>
        public string printmsg
        {
            get { return _printmsg; }
            set { _printmsg = value; }
        }
    }

    /// <summary>
    /// 任务状态枚举类
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// 待处理状态
        /// </summary>
        WaitHandle = 1,
        /// <summary>
        /// 处理中状态
        /// </summary>
        Handleing = 2,
        /// <summary>
        /// 终止状态
        /// </summary>
        Stop = 3,
        /// <summary>
        /// 完成状态
        /// </summary>
        Complete = 4
    }
}
