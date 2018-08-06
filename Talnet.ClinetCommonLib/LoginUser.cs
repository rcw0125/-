using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talent.ClientCommonLib
{
    public class LoginUser1
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public static string UserId;
        /// <summary>
        /// 用户登录名
        /// </summary>
        public static string UserEnglishName;
        /// <summary>
        /// 用户对应的人员名
        /// </summary>
        public static string PersonName
        { get; set; }
        /// <summary>
        /// 用户对应的人员Id
        /// </summary>
        public static string PersonId
        { get; set; }


        static List<FunctionOfCurrentUser> functions = new List<FunctionOfCurrentUser>();
        /// <summary>
        /// 当前用户所用有的功能点
        /// </summary>
        public static List<FunctionOfCurrentUser> Functions
        {
            get
            {
                return functions;
            }
            set
            {
                functions = value;
            }
        }

        /// <summary>
        /// 当前功能点Id
        /// </summary>
        public static string CurrentFuncId;
    }

    public class FunctionOfCurrentUser
    {
        /// <summary>
        /// 功能点名称
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// 功能点url
        /// </summary>
        public string FunctionUrl { get; set; }
        /// <summary>
        /// 功能点父Id
        /// </summary>
        public string ParentId { get; set; }

        public string Id { get; set; }

    }
}
