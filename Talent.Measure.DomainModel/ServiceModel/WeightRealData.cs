using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.ServiceModel
{
   public class WeightRealData
    {
       /// <summary>
       /// 称点ID
       /// </summary>
       private string _clientid;
       public string clientid
       {
           set { _clientid = value; }
           get { return _clientid; }
       }
       /// <summary>
       /// 过磅单号
       /// </summary>
       private string _matchid;
       public string matchid
       {
           set { _matchid = value; }
           get { return _matchid; }
       }

       /// <summary>
       /// 车号
       /// </summary>
       private string _carno;
       public string carno
       {
           set { _carno = value; }
           get { return _carno; }
       }
       /// <summary>
       /// 业务开始时间
       /// </summary>
       private string _begintime;
       public string begintime
       {
           set { _begintime = value; }
           get { return _begintime; }
       }
       /// <summary>
       /// 业务结束时间
       /// </summary>
       private string _endtime;
       public string endtime
       {
           set { _endtime = value; }
           get { return _endtime; }
       }
       private string _realdata;
       public string realdata
       {
           set { _realdata = value; }
           get { return _realdata; }
       }
    }
   public class WeightRecordData
   {
       /// <summary>
       /// 记录时间
       /// </summary>
       private string _recordtime;
       public string recordtime
       {
           set { _recordtime = value; }
           get { return _recordtime; }
       }
       /// <summary>
       /// 重量记录
       /// </summary>
       private decimal _recorddata;
       public decimal recorddata
       {
           set { _recorddata = value; }
           get { return _recorddata; }
       }

   }
}
