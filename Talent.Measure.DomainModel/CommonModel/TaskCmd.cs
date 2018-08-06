using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Measure.DomainModel.CommonModel
{
    #region 计量部分
    /// <summary>
    /// 终端监听命令
    /// </summary>
    public class ClientlistenCmdEnum
    {
        /// <summary>
        /// 发送命令给秤点
        /// </summary>
        public static string sendCMD = "sendCMD";

        /// <summary>
        /// 发送通知给秤点
        /// </summary>
        public static string sendMSG = "sendMSG";
        /// <summary>
        /// 重新登陆
        /// </summary>
        public static string relogin = "relogin";
        /// <summary>
        /// 重新连接
        /// </summary>
        public static string reconn = "reconn";
        /// <summary>
        /// 登录成功信息
        /// </summary>
        public static string loginok = "loginok";

        /// <summary>
        /// 坐席回复命令="sendReply"
        /// </summary>
        public static string sendReply = "sendReply";
        /// <summary>
        /// 任务服务返回随机数
        /// </summary>
        public static string reply = "reply";

    }
    /// <summary>
    /// 终端发送命令
    /// </summary>
    public class ClientSendCmdEnum
    {
        /// <summary>
        /// 登陆
        /// </summary>
        public static string login = "login";
        /// <summary>
        /// 发送计量数据
        /// </summary>
        public static string measureData = "measureData";

        /// <summary>
        /// 退出
        /// </summary>
        public static string logout = "logout";

        /// <summary>
        /// "计量端实时数据"命令="realData"
        /// </summary>
        public static string realData = "realData";

        /// <summary>
        ///车下称 若任务未处理 任务服务器自动移除 2016-3-8 09:56:26……
        /// </summary>
        public static string backtask = "backtask";
 
    }

    #endregion

    #region 坐席部分
    /// <summary>
    /// 坐席监听命令
    /// </summary>
    public class SeatlistenCmdEnum
    {
        /// <summary>
        /// 完成任务
        /// </summary>
        public static string endtask = "endTask";

        /// <summary>
        /// 发送新任务通知给坐席
        /// </summary>
        public static string newTask = "newTask";

        /// <summary>
        /// 发送任务给坐席命令="task"
        /// </summary>
        public static string Task = "task";

        /// <summary>
        /// 发送提示给坐席命令="txt"
        /// </summary>
        public static string Txt = "txt";
        /// <summary>
        /// 登录成功命令="loginok"
        /// </summary>
        public static string logok = "loginok";
        /// <summary>
        /// "等待的任务"命令="waitingTask"
        /// </summary>
        public static string waitingTask = "waitingTask";
        /// <summary>
        /// "计量端实时数据"命令="realData"
        /// </summary>
        public static string realData = "realData";
        /// <summary>
        /// "转发任务"命令="redirectTask"
        /// </summary>
        public static string redirectTask = "redirectTask";
        /// <summary>
        /// "转发任务"反馈后需要监听的命令="reply"
        /// </summary>
        public static string reply = "reply";

   
    }
    /// <summary>
    /// 坐席操作命令
    /// </summary>
    public class SeatSendCmdEnum
    {
        /// <summary>
        /// 登陆命令=login
        /// </summary>
        public static string login = "login";
        /// <summary>
        ///接收计量任务命令="getTask"
        /// </summary>
        public static string getTask = "getTask";

        /// <summary>
        /// 暂停接收任务命令="pause"
        /// </summary>
        public static string pause = "pause";

        /// <summary>
        /// 恢复接收任务命令="resume"
        /// </summary>
        public static string resume = "resume";

        /// <summary>
        ///退回任务命令="backTask"
        /// </summary>
        public static string backTask = "backTask";

        /// <summary>
        /// 发送命令到称点命令="cmd2client"
        /// </summary>
        public static string cmd2client = "cmd2client";
        /// <summary>
        /// 抢任务命令="getTask2"
        /// </summary>
        public static string getTask2 = "getTask2";
        
    
    }

   #endregion

    #region 参数命令
    public class ParamCmd
    {
        /// <summary>
        /// 同步坐席修改后的称点信息到称点="UpdateInfo"
        /// </summary>
        public static string Update_Seat_To_Measure = "UpdateInfo";

        /// <summary>
        /// 语音提示="VoicePrompt"
        /// </summary>
        public static string Voice_Prompt = "VoicePrompt";

        /// <summary>
        /// 语音对讲开始="VoiceTalkStart"
        /// </summary>
        public static string Voice_Talk_Start = "VoiceTalkStart";

        /// <summary>
        /// 语音对讲结束="VoiceTalkEnd"
        /// </summary>
        public static string Voice_Talk_End = "VoiceTalkEnd";

        /// <summary>
        /// 表头清零="MeasureWeightClear"
        /// </summary>
        public static string MeasureWeightClear = "MeasureWeightClear";

        /// <summary>
        /// 终端重启="ClientRestart"
        /// </summary>
        public static string ClientRestart = "ClientRestart";
        /// <summary>
        /// 终端更新="ClientUpdate"
        /// </summary>
        public static string ClientUpdate = "ClientUpdate";

        /// <summary>
        /// 远程补票="Supplement"
        /// </summary>
        public static string Supplement = "Supplement";

        /// <summary>
        /// 坐席暂停终端计量 lt 2016-2-17 14:34:45……
        /// </summary>
        public static string ClientStop = "ClientStop";

        /// <summary>
        /// 终端显示坐席发送的通知 lt 2016-2-17 14:34:45……
        /// </summary>
        public static string UserNotice = "UserNotice";
        /// <summary>
        /// ClientState 秤点计量状态  例如等待计量还是  正在计量……
        /// </summary>
        public static string ClientState = "ClientState";
        /// <summary>
        /// 称点全屏="FullScreen"
        /// </summary>
        public static string ClientFullScreen = "FullScreen";
    }
    #endregion
}
