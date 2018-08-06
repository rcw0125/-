using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.ServiceModel.TaskStat
{
    /// <summary>
    /// 计量员集合
    /// </summary>
    public class MeasureUserModel
    {
        /// <summary>
        /// 登录id
        /// </summary>
        public string LogName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        private string _username;
        public string username
        {
            set { _username = value; DisplayName = value; }
            get
            {
                return _username;
            }
        }

      
        /// <summary>
        /// 用户编码
        /// </summary>
        private string _usercode;
        public string usercode
        {
            set { _usercode = value; LogName = value; }
            get
            {
                return _usercode;
            }
        }
    }
}
