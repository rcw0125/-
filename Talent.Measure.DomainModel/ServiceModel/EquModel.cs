using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel
{
    /// <summary>
    /// 服务端返回的设备模型
    /// </summary>
    public class EquModel
    {
        private string _id;
        /// <summary>
        /// 设备ID
        /// </summary>
        public string id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _equcode;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string equcode
        {
            get { return _equcode; }
            set { _equcode = value; }
        }
        private string _equname;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string equname
        {
            get { return _equname; }
            set { _equname = value; }
        }
        private string _equtype;
        /// <summary>
        /// 设备类型(T:火车衡;C:汽车衡;RC:坐席(或者汽车衡坐席);RDI:动态轨道衡;TASK:任务服务器)
        /// </summary>
        public string equtype
        {
            get { return _equtype; }
            set { _equtype = value; }
        }
        private string _paraminfos;
        /// <summary>
        /// 参数信息(设备配置信息)
        /// </summary>
        public string paraminfos
        {
            get { return _paraminfos; }
            set { _paraminfos = value; }
        }
        private string _versionnum;
        /// <summary>
        /// 版本号
        /// </summary>
        public string versionnum
        {
            get { return _versionnum; }
            set { _versionnum = value; }
        }

    }

    public class EquTypeEnum
    {
        /// <summary>
        /// 设备类型"火车衡"="T"
        /// </summary>
        public static string Type_Train = "T";
        /// <summary>
        /// 设备类型"汽车衡"="C"
        /// </summary>
        public static string Type_Car = "C";
        /// <summary>
        /// 设备类型_坐席(汽车衡)="RC"
        /// </summary>
        public static string Type_Car_Seat = "RC";
        /// <summary>
        /// 设备类型"动态轨道衡"="RDI"
        /// </summary>
        public static string Type_Dynamic_Orbit = "RDI";
        /// <summary>
        /// 设备类型"任务服务器"="TASK"
        /// </summary>
        public static string Type_Task_Server = "TASK";
    }

}
