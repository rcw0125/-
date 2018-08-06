using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public class User
    {
        private string name;
        /// <summary>
        /// 标识
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string code;
        /// <summary>
        /// 编码
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("code")]
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        private string loginName;
        /// <summary>
        /// 登录名
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("loginName")]
        public string LoginName
        {
            get { return loginName; }
            set { loginName = value; }
        }
        private string password;
        /// <summary>
        /// 密码
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("password")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        private Role role;
        /// <summary>
        /// 角色
        /// </summary>
        [System.Xml.Serialization.XmlElement("Role")]
        public Role Role
        {
            get { return role; }
            set { role = value; }
        }
    }
}
