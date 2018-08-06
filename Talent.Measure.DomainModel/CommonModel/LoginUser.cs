using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 登录用户对象
    /// </summary>
    public class LoginUser
    {
        private static string name;
        /// <summary>
        /// 用户名称
        /// </summary>
        public static string Name
        {
            get { return name; }
            set { name = value; }
        }
        private static string code;
        /// <summary>
        /// 用户编码
        /// </summary>
        public static string Code
        {
            get { return code; }
            set { code = value; }
        }
        private static string loginName;
        /// <summary>
        /// 登录名
        /// </summary>
        public static string LoginName
        {
            get { return loginName; }
            set { loginName = value; }
        }
        private static string password;
        /// <summary>
        /// 登录密码
        /// </summary>
        public static string Password
        {
            get { return password; }
            set { password = value; }
        }
        private static Role role;
        /// <summary>
        /// 角色
        /// </summary>
        public static Role Role
        {
            get { return role; }
            set { role = value; }
        }
        private static List<Module> _modules;
        /// <summary>
        /// 用户所拥有的功能集合
        /// </summary>
        public static List<Module> Modules 
        {
            set { _modules = value; }
            get { return _modules; }
        }
    }
}
