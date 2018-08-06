using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel
{
    /// <summary>
    /// 登录服务模型
    /// </summary>
    public class LoginServiceModel
    {
        private bool _success;
        public bool success
        {
            set { _success = value; }
            get { return _success; }
        }
        private string _msg;
        public string msg
        {
            set { _msg = value; }
            get { return _msg; }
        }
        private string _mtype;
        /// <summary>
        /// 模块类型
        /// </summary>
        public string mtype
        {
            get { return _mtype; }
            set { _mtype = value; }
        }
        /// <summary>
        /// 0自动计量，1手动，2远程
        /// </summary>
        private int _mfunc;
        public int mfunc
        {
            set { _mfunc = value; }
            get { return _mfunc; }
        }
        private List<Module> _data;
        public List<Module> data
        {
            set { _data = value; }
            get { return _data; }
        }
        private List<flagMsg> _flags;
        /// <summary>
        /// 业务调用过程错误信息
        /// </summary>
        public List<flagMsg> flags
        {
            set { _flags = value; }
            get { return _flags; }
        }
        private int _total;
        public int total
        {
            set { _total = value; }
            get { return _total; }
        }
        private List<BullInfo> _rows;
        /// <summary>
        /// 业务信息集合
        /// </summary>
        public List<BullInfo> rows
        {
            set { _rows = value; }
            get { return _rows; }
        }
        private List<hardwarectrlCls> _hardwarectrl;
        /// <summary>
        /// 硬件设置
        /// </summary>
        public List<hardwarectrlCls> hardwarectrl
        {
            set { _hardwarectrl = value; }
            get { return _hardwarectrl; }
        }

        private UserInfoModel _more;
        /// <summary>
        /// 备注信息
        /// </summary>
        public UserInfoModel more
        {
            get { return _more; }
            set { _more = value; }
        }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfoModel
    {
        private string _usercode;
        /// <summary>
        /// 用户编号
        /// </summary>
        public string usercode
        {
            get { return _usercode; }
            set { _usercode = value; }
        }
        private string _username;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string username
        {
            get { return _username; }
            set { _username = value; }
        }
    }

    public class Module
    {
        private string _modulecode;
        /// <summary>
        /// 功能编码
        /// </summary>
        public string modulecode
        {
            get { return _modulecode; }
            set { _modulecode = value; }
        }
        private string _modulename;
        /// <summary>
        /// 功能名称
        /// </summary>
        public string modulename
        {
            get { return _modulename; }
            set { _modulename = value; }
        }
        /// <summary>
        /// 模块类型
        /// </summary>
        private string _resourcetype;

        public string Resourcetype
        {
            get { return _resourcetype; }
            set { _resourcetype = value; }
        }
        /// <summary>
        /// 模块编码
        /// </summary>
        private string _resourcecode;

        public string Resourcecode
        {
            get { return _resourcecode; }
            set { _resourcecode = value; }
        }
        /// <summary>
        /// 模块名称
        /// </summary>
        private string _resourcename;

        public string Resourcename
        {
            get { return _resourcename; }
            set { _resourcename = value; }
        }
        /// <summary>
        /// 模块描述
        /// </summary>
        private string _resourcememo;

        public string Resourcememo
        {
            get { return _resourcememo; }
            set { _resourcememo = value; }
        }
        /// <summary>
        /// 模块对应的按钮权限
        /// </summary>
        private List<ButtonMemu> _children;

        public List<ButtonMemu> Children
        {
            get { return _children; }
            set { _children = value; }
        }
    }
    /// <summary>
    /// 按钮权限
    /// </summary>
    public class ButtonMemu
    {
        /// <summary>
        /// 按钮类型
        /// </summary>
        private string _resourcetype;

        public string Resourcetype
        {
            get { return _resourcetype; }
            set { _resourcetype = value; }
        }
        /// <summary>
        /// 按钮编码
        /// </summary>
        private string _resourcecode;

        public string Resourcecode
        {
            get { return _resourcecode; }
            set { _resourcecode = value; }
        }
        /// <summary>
        /// 按钮名称
        /// </summary>
        private string _resourcename;

        public string Resourcename
        {
            get { return _resourcename; }
            set { _resourcename = value; }
        }
        /// <summary>
        /// 按钮描述
        /// </summary>
        private string _resourcememo;

        public string Resourcememo
        {
            get { return _resourcememo; }
            set { _resourcememo = value; }
        }
    }
}
