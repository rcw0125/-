using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Talent.ClientCommonLib;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Commom;
using Talent.CommonMethod;
using System.Net;
using Talent.Measure.DomainModel;
using System.IO;
using Newtonsoft.Json;
using Talent.RemoteCarMeasure.Model;
using Talent.ClientCommonLib.Controls;
using System.Xml;
using System.Xml.Serialization;
using Talent_LT.HelpClass;
using Talent.Measure.WPF.Remote;
using Talent.Measure.WPF.Log;
using Talent_LT.Model;
using Newtonsoft.Json.Linq;

namespace Talent.RemoteCarMeasure.ViewModel
{
    public class LoginViewModel : Only_ViewModelBase
    {
        LogsHelpClass logH = new LogsHelpClass();
        public LoginViewModel()
        {
            if (this.IsInDesignMode)
                return;
            //log.SaveLog("测试记录操作日志内容");
            //log.SaveLog("测试记录操作日志内容", "登录");
            //log.SaveLog("测试记录操作日志内容", "登录",LogTypes.Info);
            InitSeatInfo();
            LoginCommand = new ActionCommand(LoginMethod);
            this.ShowBusy = Visibility.Collapsed;
        }

        /// <summary>
        /// 初始化坐席信息
        /// </summary>
        private void InitSeatInfo()
        {
            string configSet = ConfigurationManager.AppSettings["SysConfigFileName"].ToString();
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string configUrl = basePath + configSet;

            #region 读取坐席ID
            string seatIdMark = ConfigurationManager.AppSettings["SeatId"].ToString();
            var seatId = XpathHelper.GetValue(configUrl, seatIdMark);
            #endregion

            #region 读取坐席名称
            string seatNameMark = ConfigurationManager.AppSettings["SeatName"].ToString();
            var seatName = XpathHelper.GetValue(configUrl, seatNameMark);
            #endregion
            LoginUser.Role = new Role() { Code = seatId, Name = seatName };
        }

        public ICommand LoginCommand { get; private set; }

        private bool _IsSaveUser;

        public bool IsSaveUser
        {
            get { return _IsSaveUser; }
            set
            {
                _IsSaveUser = value;
                this.RaisePropertyChanged("IsSaveUser");
            }
        }
        /// <summary>
        /// 坐席关注的汽车衡客户端集合
        /// </summary>
        private List<SeatAttentionWeightModel> SeatAttentionInfos;
        private bool storyBoardEnable;
        /// <summary>
        /// 动画是否可用
        /// </summary>
        public bool StoryBoardEnable
        {
            get { return storyBoardEnable; }
            set
            {
                storyBoardEnable = value;
                this.RaisePropertyChanged("StoryBoardEnable");
            }
        }

        private string errMsg;
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrMsg
        {
            get { return errMsg; }
            set
            {
                errMsg = value;
                this.RaisePropertyChanged("ErrMsg");
            }
        }

        /// <summary>
        /// 是否登录
        /// </summary>
        private bool isLogin = false;

        private Visibility _ShowBusy;

        public Visibility ShowBusy
        {
            get { return _ShowBusy; }
            set
            {
                _ShowBusy = value;
                this.RaisePropertyChanged("ShowBusy");
                if (value == Visibility.Visible)
                    ShowInput = Visibility.Collapsed;
                else
                    ShowInput = Visibility.Visible;
            }
        }

        private Visibility _ShowInput = Visibility.Visible;

        public Visibility ShowInput
        {
            get { return _ShowInput; }
            set
            {
                _ShowInput = value;
                this.RaisePropertyChanged("ShowInput");
            }
        }

        private string _UserName;

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                if (_UserName != value)
                {
                    _UserName = value;
                    //CheckUser();                    
                }
                this.RaisePropertyChanged("UserName");
                this.ErrMsg = string.Empty;
            }
        }

        private void CheckUser()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                this.AddError("UserName", "请输入用户名");
                this.ErrMsg = "请输入用户名!";
            }
            else
            {
                this.ClearError("UserName");
                this.ErrMsg = string.Empty;
            }
            RaisePropertyChanged("UserName");
        }

        private string _Password;

        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    //CheckPassword();
                }
                this.RaisePropertyChanged("Password");
                this.ErrMsg = string.Empty;
            }
        }
        /// <summary>
        /// 动画是否已准备
        /// </summary>
        public bool IsStoryReady = false;

        private void CheckPassword()
        {
            if (string.IsNullOrEmpty(Password))
            {
                this.AddError("Password", "请输入密码");
                this.ErrMsg = "请输入密码!";
            }
            else
            {
                this.ClearError("Password");
                this.ErrMsg = string.Empty;
            }
            RaisePropertyChanged("Password");
        }

        public void ReadUserInfo()
        {
            string userName = Properties.Settings.Default.LocalUserName;
            if (!string.IsNullOrEmpty(userName))
            {
                this.IsSaveUser = true;
                this.UserName = userName;
                this.Password = PassWordHelpClass.PassWordDecrypt(Properties.Settings.Default.LocalPassword);
            }
        }

        public void LoginMethod()
        {
           // tmService.ServiceSoapClient tm = new tmService.ServiceSoapClient();
           //var tmWeight = tm.GetWeightForWl("冀E2055Z");
            CheckUser();
            if (string.IsNullOrEmpty(this.ErrMsg))
            {
                CheckPassword();
            }
            if (!ISSucceed)
            {
                return;
            }
            IsStoryReady = true;
            if (!isLogin)
            {
                RemoteLoginMethod();
            }
        }

        /// <summary>
        /// 远程登录
        /// </summary>
        private void RemoteLoginMethod()
        {
            try
            {
                StoryBoardEnable = true;
                isLogin = true;
                string serviceUrl = ConfigurationManager.AppSettings["getLoginInfo"].ToString().Replace('$', '&');
               // string getUrl = string.Format(serviceUrl, UserName, Password);
                string getUrl = string.Format(serviceUrl, UserName.ToUpper(), PassWordHelpClass.LesPassWordMD5(UserName.ToUpper() + Password));
               
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_登录窗体_调用服务登录系统",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "调用服务登录系统",
                    Data=getUrl,
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
                request.BeginGetResponse(new AsyncCallback(getMeasureInfoCallback), request);
            }
            catch
            {
                string st = "";
            }
           
        }

        /// <summary>
        /// 通过服务获取业务信息的回调方法
        /// </summary>
        /// <param name="asyc"></param>
        public void getMeasureInfoCallback(IAsyncResult asyc)
        {
            try
            {
                LoginServiceModel mServiceModel;
                string strResult = ComHelpClass.ResponseStr(asyc);
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_登录窗体_通过服务获取业务信息的回调方法",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "得到服务反馈的登录结果",
                    Origin = "汽车衡_" + LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name,
                    Data = strResult
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
                mServiceModel = GetLoginServiceModel(strResult);
                //mServiceModel = InfoExchange.DeConvert(typeof(LoginServiceModel), strResult) as LoginServiceModel;
                System.Threading.Thread.Sleep(3000);//时间太短，动画出不来，故而延迟4秒
                if (mServiceModel.success)
                {
                    
                    InitLoginUserInfo(mServiceModel);
                    GetWeighterClientInfos();
                    //logH.SaveLog("登录成功");
                    #region 日志
                    LogModel log1 = new LogModel()
                    {
                        CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Direction = LogConstParam.Directions_OutIn,
                        FunctionName = "坐席_登录窗体_通过服务获取业务信息的回调方法",
                        Level = LogConstParam.LogLevel_Info,
                        Msg = "用户远程登录成功",
                        Origin = "汽车衡_"+LoginUser.Role.Name,
                        OperateUserName = LoginUser.Name,
                        Data = mServiceModel,
                        IsDataValid = LogConstParam.DataValid_Ok,
                        ParamList = new List<DataParam>() { new DataParam() { ParamName = "userName", ParamValue = UserName }, new DataParam() { ParamName = "password", ParamValue = CommonTranslationHelper.MD5(UserName + Password) } }
                    };
                    Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log1));
                    #endregion
                    
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        StoryBoardEnable = false;
                        this.ErrMsg = mServiceModel.msg;
                        isLogin = false;
                        return;
                    }));
                }
            }
            catch (Exception ex)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    StoryBoardEnable = false;
                    this.ErrMsg = ex.Message;
                    isLogin = false;
                    return;
                }));
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_登录窗体_通过服务获取业务信息的回调方法",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "用户远程登录失败！原因：" + ex.Message,
                    Origin = "汽车衡_"+LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 获取关注的汽车衡称点信息
        /// </summary>
        private void GetWeighterClientInfos()
        {
            string serviceUrl = ConfigurationManager.AppSettings["getSeatClient"].ToString();
            var param = new
            {
                seatname = LoginUser.Role.Name,
                seatid = LoginUser.Role.Code,
                seattype = ""
            };
            string getUrl = string.Format(serviceUrl, "[" + JsonConvert.SerializeObject(param) + "]");
            HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);
            request.BeginGetResponse(new AsyncCallback(GetWeighterClientInfoCallback), request);
        }
        /// <summary>
        /// 获取关注的汽车衡称点信息回调函数
        /// </summary>
        /// <param name="asyc"></param>
        private void GetWeighterClientInfoCallback(IAsyncResult asyc)
        {
            try
            {
                string strResult = ComHelpClass.ResponseStr(asyc); 
                SeatAttentionInfos = InfoExchange.DeConvert(typeof(List<SeatAttentionWeightModel>), strResult) as List<SeatAttentionWeightModel>;
                var clientInfos = (from r in SeatAttentionInfos
                                   where r.isinseat == "是"
                                       && r.seatid == LoginUser.Role.Code
                                   select r).OrderBy(c => c.equcode).ToList();
                ConfigSynchronous(clientInfos);         
            }
            catch (Exception ex)
            {
                #region 日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_OutIn,
                    FunctionName = "坐席_登录窗体_获取关注的汽车衡称点信息回调函数",
                    Level = LogConstParam.LogLevel_Error,
                    Msg = "获取坐席关注的称点信息失败！原因：" + ex.Message,
                    Origin = "汽车衡_"+LoginUser.Role.Name,
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 配置文件同步
        /// </summary>
        private void ConfigSynchronous(List<SeatAttentionWeightModel> ClientInfos)
        {
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            foreach (var item in ClientInfos)
            {
                string clientConfigPath = System.IO.Path.Combine(basePath, item.equcode + ".xml");
                if (File.Exists(clientConfigPath))
                {
                    File.Delete(clientConfigPath);
                    ConfigFileDownLoad(clientConfigPath, item.versionnum, item.equcode, item.equname);
                }
                else
                {
                    ConfigFileDownLoad(clientConfigPath, item.versionnum, item.equcode, item.equname);
                }
            }
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                StoryBoardEnable = false;
                this.ErrMsg = string.Empty;
                ShowBusy = Visibility.Visible;
                UserLoginSucceed();
            }));
        }

        /// <summary>
        /// 下载配置文件
        /// </summary>
        /// <param name="configFileName">配置文件名称(带路径)</param>
        /// <param name="versionNum">版本号</param>
        /// <param name="clientCode">称点编号</param>
        /// <param name="ClientName">称点名称</param>
        private void ConfigFileDownLoad(string configFileName, string versionNum, string clientCode, string ClientName)
        {
            try
            {
                string serviceUrl = ConfigurationManager.AppSettings["getEquParamInfo"].ToString();
                var param = new
                {
                    versionnum = -1,
                    equcode = clientCode,
                    equname = ""//ClientName
                };
                string getUrl = string.Format(serviceUrl, "[" + JsonConvert.SerializeObject(param) + "]");
                HttpWebRequest request = WebRequestCommon.GetHttpGetWebRequest(getUrl, 10);

                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                String strResult = sr.ReadToEnd();
                sr.Close();
                if(string.IsNullOrEmpty(strResult))
                {
                    return;
                }
                List<EquModel> equModels = InfoExchange.DeConvert(typeof(List<EquModel>), strResult) as List<EquModel>;
                if (equModels.Count > 0)
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(configFileName);
                    sw.WriteLine(equModels.First().paraminfos.Replace("GBK", "UTF-8"));
                    sw.Close();
                }
            }
            catch //(Exception ex)
            { 
            
            }
           
        }

        private void UserLoginSucceed()
        {
            if (this.IsSaveUser)
            {
                Properties.Settings.Default.LocalUserName = this.UserName.Trim();
                Properties.Settings.Default.LocalPassword = PassWordHelpClass.PassWordEncrypts(this.Password.Trim());
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.LocalUserName = string.Empty;
                Properties.Settings.Default.LocalPassword = string.Empty;
                Properties.Settings.Default.Save();
            }

            MainWindow mwin = new MainWindow(SeatAttentionInfos);
            mwin.Show();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<string>("成功", "LoginViewModel");
        }

        /// <summary>
        /// 构造登录用户信息
        /// </summary>
        private void InitLoginUserInfo(LoginServiceModel mServiceModel)
        {
            LoginUser.Code = mServiceModel.more.usercode;
            LoginUser.LoginName = UserName;
            LoginUser.Name = mServiceModel.more.username;
            LoginUser.Password = Password;
            LoginUser.Modules = mServiceModel.data;
        }

        private LoginServiceModel GetLoginServiceModel(string str)
        {
            LoginServiceModel lg = new LoginServiceModel();
            try
            {
                JObject jo = JsonHelpClass.JsonStringToJObject(str);
                lg.success = (bool)jo["success"];
                lg.msg = jo["msg"].ToString();
                //lg.mtype = jo["mtype"].ToString();
                lg.data = GetListModule(jo["data"].ToString());
                lg.total = int.Parse(jo["total"].ToString());
                lg.mfunc = int.Parse(jo["mfunc"].ToString());
                lg.flags = GetListFlagmsg(jo["flags"].ToString());
                lg.rows = GetListBullinfo(jo["rows"].ToString());
                //lg.hardwarectrl = GetListhardwarectrlCls(jo["hardwarectrl"].ToString());
                lg.more = GetUserInfoModel(jo["more"].ToString());
            }
            catch(Exception ex)
            {
                
            }
            return lg;
        }
        private List<Module> GetListModule(string str)
        {
            List<Module> list = new List<Module>();
            try
            {
                if (str!="[]")
                {
                    //JObject jo = JsonHelpClass.JsonStringToJObject(str);

                    List<moudleQx> equModels = InfoExchange.DeConvert(typeof(List<moudleQx>), str) as List<moudleQx>;
                    foreach (var item in equModels)
                    {
                        Module mm = new Module();
                        mm.Resourcecode = item.RESOURCECODE;
                        mm.Resourcename = item.RESOURCENAME;
                        mm.Resourcetype = item.RESOURCECODE;
                        mm.Resourcememo = item.RESOURCECODE;
                        mm.modulecode = item.RESOURCECODE;
                        mm.modulename = item.RESOURCECODE;
                        //mm.Children = GetListButtonmemu(jo["children"].ToString());
                        list.Add(mm);
                    }
                   
                }
            }
            catch(Exception ex)
            {

            }
            return list;
        }
        public class moudleQx
        {
            public string RESOURCENAME { get; set; }
            public string RESOURCECODE { get; set; }
        }
        private List<ButtonMemu> GetListButtonmemu(string str)
        {
            List<ButtonMemu> list = new List<ButtonMemu>();
            if (str!="[]")
            {
                JObject jo = JsonHelpClass.JsonStringToJObject(str);
                ButtonMemu memu = new ButtonMemu();
                memu.Resourcecode = jo["resourcecode"].ToString();
                memu.Resourcename = jo["resourcename"].ToString();
                memu.Resourcetype = jo["resourcetype"].ToString();
                memu.Resourcememo = jo["resourcememo"].ToString();
                list.Add(memu);
            }
            return list;
        }
        private List<flagMsg> GetListFlagmsg(string str)
        {
            List<flagMsg> list = new List<flagMsg>();
            if (str!="[]")
            {
                JObject jo = JsonHelpClass.JsonStringToJObject(str);
                flagMsg fs = new flagMsg();
                fs.Msg = jo["msg"].ToString();
                fs.success = (bool)jo["success"];
                fs.flag = int.Parse(jo["flag"].ToString());
                fs.list = GetListBullinfo(jo["list"].ToString());
                fs.count = int.Parse(jo["count"].ToString());
                list.Add(fs);
            }
            return list;
        }
        private List<BullInfo> GetListBullinfo(string str)
        {
            List<BullInfo> list = new List<BullInfo>();
            if (str != "[]")
            {
                JObject jo = JsonHelpClass.JsonStringToJObject(str);
                BullInfo info = new BullInfo();
                info.recordtype = jo["recordtype"].ToString();
                info.rfid = jo["rfid"].ToString();
                info.rfidid = jo["rfidid"].ToString();
                info.rfidtype = jo["rfidtype"].ToString();
                info.ruleflag = int.Parse(jo["ruleflag"].ToString());
                info.basket = jo["basket"].ToString();
                info.batchcode = jo["batchcode"].ToString();
                info.bflag = int.Parse(jo["bflag"].ToString());
                info.caller = jo["caller"].ToString();
                info.carno = jo["carno"].ToString();
                info.clientid = jo["clientid"].ToString();
                info.deduction = decimal.Parse(jo["deduction"].ToString());
                info.deduction2 = decimal.Parse(jo["deduction2"].ToString());
                info.deductioncode = jo["deductioncode"].ToString();
                info.deductionname = jo["deductionname"].ToString();
                info.deductionoperacode = jo["deductionoperacode"].ToString();
                info.deductionoperaname = jo["deductionoperaname"].ToString();
                info.deductiontime = jo["deductiontime"].ToString();
                info.dflag = int.Parse(jo["dflag"].ToString());
                info.dvalue = double.Parse(jo["dvalue"].ToString());
                info.flag = int.Parse(jo["flag"].ToString());
                info.gflag = int.Parse(jo["gflag"].ToString());
                info.gross = decimal.Parse(jo["gross"].ToString());
                info.grossb = decimal.Parse(jo["grossb"].ToString());
                info.grossgroupno = jo["grossgroupno"].ToString();
                info.grosslogid = jo["grosslogid"].ToString();
                info.grosslogidb = jo["grosslogidb"].ToString();
                info.grossoperacode = jo["grossoperacode"].ToString();
                info.grossoperacodeb = jo["grossoperacodeb"].ToString();
                info.grossoperaname = jo["grossoperaname"].ToString();
                info.grossoperanameb = jo["grossoperanameb"].ToString();
                info.grossserial = int.Parse(jo["grossserial"].ToString());
                info.grosstime = jo["grosstime"].ToString();
                info.grosstimeb = jo["grosstimeb"].ToString();
                info.grossweigh = jo["grossweigh"].ToString();
                info.grossweighb = jo["grossweighb"].ToString();
                info.grossweighgroup = jo["grossweighgroup"].ToString();
                info.grossweighid = jo["grossweighid"].ToString();
                info.grossweighidb = jo["grossweighidb"].ToString();
                info.icid = jo["icid"].ToString();
                info.matchid = jo["matchid"].ToString();
                info.matchidb = jo["matchidb"].ToString();
                info.materialcode = jo["materialcode"].ToString();
                info.materialcount = int.Parse(jo["materialcount"].ToString());
                info.materialname = jo["materialname"].ToString();
                info.materialspec = jo["materialspec"].ToString();
                info.materialspeccode = jo["materialspeccode"].ToString();
                info.measurestate = jo["measurestate"].ToString();
                info.mflag = int.Parse(jo["mflag"].ToString());
                info.mtypes = jo["mtypes"].ToString();
                info.operaname = jo["operaname"].ToString();
                info.operatype = jo["operatype"].ToString();
                info.planid = jo["planid"].ToString();
                info.planweight = decimal.Parse(jo["planweight"].ToString());
                info.sflag = int.Parse(jo["sflag"].ToString());
                info.ship = jo["ship"].ToString();
                info.shipcode = jo["shipcode"].ToString();
                info.snumber = int.Parse(jo["snumber"].ToString());
                info.sourcecode = jo["sourcecode"].ToString();
                info.sourcename = jo["sourcename"].ToString();
                info.sourceplace = jo["sourceplace"].ToString();
                info.suttle = decimal.Parse(jo["suttle"].ToString());
                info.suttleapp = decimal.Parse(jo["suttleapp"].ToString());
                info.suttleb = decimal.Parse(jo["suttleb"].ToString());
                info.suttleoperacode = jo["suttleoperacode"].ToString();
                info.suttleoperaname = jo["suttleoperaname"].ToString();
                info.suttletime = jo["suttletime"].ToString();
                info.suttleweigh = jo["suttleweigh"].ToString();
                info.suttleweighid = jo["suttleweighid"].ToString();
                info.sysmemo = jo["sysmemo"].ToString();
                info.tare = decimal.Parse(jo["tare"].ToString());
                info.tareb = decimal.Parse(jo["tareb"].ToString());
                info.taregroupno = decimal.Parse(jo["taregroupno"].ToString());
                info.tarelogid = jo["tarelogid"].ToString();
                info.tarelogidb = jo["tarelogidb"].ToString();
                info.tareoperacode = jo["tareoperacode"].ToString();
                info.tareoperacodeb = jo["tareoperacodeb"].ToString();
                info.tareoperaname = jo["tareoperaname"].ToString();
                info.tareoperanameb = jo["tareoperanameb"].ToString();
                info.tareserial = decimal.Parse(jo["tareserial"].ToString());
                info.tarespeed = decimal.Parse(jo["tarespeed"].ToString());
                info.taretime = jo["taretime"].ToString();
                info.taretimeb = jo["taretimeb"].ToString();
                info.tareweigh = jo["tareweigh"].ToString();
                info.tareweighb = jo["tareweighb"].ToString();
                info.tareweighgroup = jo["tareweighgroup"].ToString();
                info.tareweighid = jo["tareweighid"].ToString();
                info.tareweighidb = jo["tareweighidb"].ToString();
                info.usermemo = jo["usermemo"].ToString();
                info.targetcode = jo["targetcode"].ToString();
                info.targetname = jo["targetname"].ToString();
                info.targetplace = jo["targetplace"].ToString();
                info.taskcode = jo["taskcode"].ToString();
                list.Add(info);
            }
            return list;
        }
        private List<hardwarectrlCls> GetListhardwarectrlCls(string str)
        {
            List<hardwarectrlCls> list = new List<hardwarectrlCls>();
            if (str!="[]")
            {
                JObject jo = JsonHelpClass.JsonStringToJObject(str);
                hardwarectrlCls cls = new hardwarectrlCls();
                cls.name = jo["name"].ToString();
                cls.check = jo["check"].ToString();
                cls.roles = GetroleCls(jo["roles"].ToString());
                list.Add(cls);
            }
            return list;
        }
        private UserInfoModel GetUserInfoModel(string str)
        {
            UserInfoModel model = new UserInfoModel();
            if (str != "[]")
            {
                JObject jo = JsonHelpClass.JsonStringToJObject(str);
                //model.usercode = jo["usercode"].ToString();
                model.username = jo["username"].ToString();
            }

            return model;
        }
        private roleCls GetroleCls(string str)
        {
            roleCls ro = new roleCls();
            if (str!="[]")
            {
                JObject jo = JsonHelpClass.JsonStringToJObject(str);
                ro.mfunc = int.Parse(jo["mfunc"].ToString());
                ro.otherParams = jo["otherParams"].ToString();
            }
            return ro;
        }
    }
}
