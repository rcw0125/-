using Microsoft.Expression.Interactivity.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Talent.Audio.Controller;
using Talent.ClientCommonLib;
using Talent.Measure.DomainModel.CommonModel;
using Talent.RemoteCarMeasure.Model;
using Talent.CommonMethod;

namespace Talent.RemoteCarMeasure.ViewModel
{
    public class VideoMonitorViewModel : Only_ViewModelBase
    {
        #region 属性
        private WeighterClientModel curClientModel;
        /// <summary>
        /// 当前绑定终端模型
        /// </summary>
        public WeighterClientModel CurClientModel
        {
            get { return curClientModel; }
            set
            {
                curClientModel = value;
                this.RaisePropertyChanged("CurClientModel");
            }
        }

        private string inputVoiceInfo;
        /// <summary>
        /// 输入的声音信息
        /// </summary>
        public string InputVoiceInfo
        {
            get { return inputVoiceInfo; }
            set
            {
                inputVoiceInfo = value;
                this.RaisePropertyChanged("InputVoiceInfo");
            }
        }

        private ObservableCollection<BaseModel> videos;
        /// <summary>
        /// 视频列表
        /// </summary>
        public ObservableCollection<BaseModel> Videos
        {
            get { return videos; }
            set
            {
                videos = value;
                this.RaisePropertyChanged("Videos");
            }
        }
        private string selectedVideo;
        public string SelectedVideo
        {
            get { return selectedVideo; }
            set
            {
                selectedVideo = value;
                this.RaisePropertyChanged("SelectedVideo");
            }
        }

        private bool isVoiceOpend;
        /// <summary>
        /// 语音通话是否已打开
        /// </summary>
        public bool IsVoiceOpend
        {
            get { return isVoiceOpend; }
            set
            {
                isVoiceOpend = value;
                if (value)
                {
                    VoiceMsg = "正在通话中....";
                    VoiceTalkButtonCotent = "关闭对讲";
                }
                else
                {
                    VoiceMsg = string.Empty;
                    VoiceTalkButtonCotent = "打开对讲";
                }
            }
        }

        private string voiceMsg;
        /// <summary>
        /// 语音信息
        /// </summary>
        public string VoiceMsg
        {
            get { return voiceMsg; }
            set
            {
                voiceMsg = value;
                this.RaisePropertyChanged("VoiceMsg");
            }
        }

        private string voiceTalkButtonCotent;
        /// <summary>
        /// 语音对讲按钮显示的内容
        /// </summary>
        public string VoiceTalkButtonCotent
        {
            get { return voiceTalkButtonCotent; }
            set
            {
                voiceTalkButtonCotent = value;
                this.RaisePropertyChanged("VoiceTalkButtonCotent");
            }
        }

        private ObservableCollection<VoiceModel> voiceCollections;
        /// <summary>
        /// 声音集合
        /// </summary>
        public ObservableCollection<VoiceModel> VoiceCollections
        {
            get { return voiceCollections; }
            set
            {
                voiceCollections = value;
                this.RaisePropertyChanged("VoiceCollections");
            }
        }

        public AudioController audioController;
        /// <summary>
        /// 称点配置
        /// </summary>
        private configlist measureConfig;

        /// <summary>
        /// 配置文件名称
        /// </summary>
        private string configPath;
        #endregion

        #region 命令
        /// <summary>
        /// 打开语音对讲命令
        /// </summary>
        public ICommand OpenVoiceTalkCommand { get; private set; }
        /// <summary>
        /// 语音提示"声音"点击绑定的命令
        /// </summary>
        public ICommand VoiceCommand { get; private set; }
        /// <summary>
        /// 发送输入的语音提示信息命令
        /// </summary>
        public ICommand SendInputVoiceCommand { get; private set; }
        #endregion

        public VideoMonitorViewModel()
        {
            if (this.IsInDesignMode)
                return;
            Videos = new ObservableCollection<BaseModel>();
            OpenVoiceTalkCommand = new ActionCommand(OpenVoiceTalk);
            VoiceCommand = new ActionCommand(SendVoiceRemind);
            SendInputVoiceCommand = new ActionCommand(SendInputVoice);
        }

        /// <summary>
        /// 发送输入的语音信息
        /// </summary>
        private void SendInputVoice()
        {
            if (!string.IsNullOrEmpty(this.InputVoiceInfo))
            {
                VoiceModel vm = new VoiceModel() { Content=this.InputVoiceInfo, };
                VoiceRemindHelper.SendVoiceInfoToMeasure(vm, CurClientModel.ClientId);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_视频监控窗体_发送输入的语音信息",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "发送语音提示命令【" + SeatSendCmdEnum.cmd2client + "】给任务服务器",
                    Origin = "汽车衡_"+LoginUser.Role.Name,
                    Data = new { clientid = CurClientModel.ClientId, cmd = ParamCmd.Voice_Prompt, msg = vm },
                    IsDataValid = LogConstParam.DataValid_Ok,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 发送语音提示
        /// </summary>
        /// <param name="obj">选择的语音提示对象</param>
        private void SendVoiceRemind(object obj)
        {
            VoiceRemindHelper.SendVoiceInfoToMeasure(obj, CurClientModel.ClientId);
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_视频监控窗体_发送语音提示",
                Level = LogConstParam.LogLevel_Info,
                Msg = "发送语音提示命令【" + SeatSendCmdEnum.cmd2client + "】给任务服务器",
                Origin = "汽车衡_"+LoginUser.Role.Name,
                Data = new { clientid = CurClientModel.ClientId, cmd = ParamCmd.Voice_Prompt, msg = obj },
                IsDataValid = LogConstParam.DataValid_Ok,
                ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        public void init()
        {
            #region 语音对讲初始化
            IsVoiceOpend = false;
            string basePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "CarMeasureClient");
            configPath = System.IO.Path.Combine(basePath, CurClientModel.ClientId + ".xml");
            audioController = new AudioController(configPath);
            audioController.OnShowErrMsg += audioController_OnShowErrMsg;
            audioController.Open();
            #endregion

            #region 语音提示初始化
            string basePath1 = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ClientConfig");
            var configPath1 = System.IO.Path.Combine(basePath1, "SystemConfig.xml");
            VoiceCollections = VoiceRemindHelper.ReadVoiceConfig(string.Empty, configPath1, out this.measureConfig);
            #endregion
        }

        #region 方法

        /// <summary>
        /// 打开语音通话
        /// </summary>
        private void OpenVoiceTalk()
        {
            IsVoiceOpend = !IsVoiceOpend;
            if (IsVoiceOpend)
            {
                
                audioController.Start();
                VoiceRemindHelper.SendVoiceTalkStartToMeasure(CurClientModel.ClientId);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_视频监控窗体_打开语音通话",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "发送【打开语音对讲】命令【" + SeatSendCmdEnum.cmd2client + "】给任务服务器",
                    Origin = "汽车衡_"+LoginUser.Role.Name,
                    Data = new { clientid = CurClientModel.ClientId, cmd = ParamCmd.Voice_Prompt, msg = "语音对讲开始" },
                    IsDataValid = LogConstParam.DataValid_Ok,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
            else
            {
                audioController.Stop();
                VoiceRemindHelper.SendVoiceTalkEndToMeasure(CurClientModel.ClientId);
                #region 写日志
                LogModel log = new LogModel()
                {
                    CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direction = LogConstParam.Directions_Out,
                    FunctionName = "坐席_视频监控窗体_打开语音通话",
                    Level = LogConstParam.LogLevel_Info,
                    Msg = "发送【关闭语音对讲】命令【" + SeatSendCmdEnum.cmd2client + "】给任务服务器",
                    Origin = "汽车衡_"+LoginUser.Role.Name,
                    Data = new { clientid = CurClientModel.ClientId, cmd = ParamCmd.Voice_Prompt, msg = "语音对讲结束" },
                    IsDataValid = LogConstParam.DataValid_Ok,
                    ParamList = new List<DataParam>() { new DataParam() { ParamName = "cmd", ParamValue = SeatSendCmdEnum.cmd2client } },
                    OperateUserName = LoginUser.Name
                };
                Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
                #endregion
            }
        }

        /// <summary>
        /// 语音对讲异常信息
        /// </summary>
        /// <param name="msg"></param>
        void audioController_OnShowErrMsg(string msg)
        {
            ShowMessage("提示", msg, true, false);
            #region 写日志
            LogModel log = new LogModel()
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Direction = LogConstParam.Directions_Out,
                FunctionName = "坐席_视频监控窗体_语音对讲异常信息",
                Level = LogConstParam.LogLevel_Error,
                Msg = "语音对讲异常:" + msg,
                Origin = "汽车衡_"+LoginUser.Role.Name,
                OperateUserName = LoginUser.Name
            };
            Talent.ClinetLog.SysLog.Log(JsonConvert.SerializeObject(log));
            #endregion
        }

        #endregion
    }
}
