using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Talent.ClientCommMethod;
using Talent.Measure.DomainModel.CommonModel;
using Talent.CommonMethod;
using Talent.RemoteCarMeasure.Commom;
using Talent.Measure.WPF;

namespace Talent.RemoteCarMeasure
{
    /// <summary>
    /// 语音提示帮助类
    /// added by wangc on 20151112
    /// </summary>
    public class VoiceRemindHelper
    {
        /// <summary>
        /// 根据任务中称点的id读取该称点的声音配置
        /// </summary>
        /// <param name="measureClientId">称点ID</param>
        /// <param name="configPath">配置文件名称</param>
        /// <param name="measureConfig">称点配置对象</param>
        public static ObservableCollection<VoiceModel> ReadVoiceConfig(string measureClientId, string configPath, out configlist measureConfig)
        {
            ObservableCollection<VoiceModel> vms = null;
            measureConfig = XmlHelper.ReadXmlToObj<configlist>(configPath);
            var sysConfigs = (from r in measureConfig.Modules where r.Code == IoConfigParam.Model_Code_SystemConfigs select r).ToList();
            if (sysConfigs.Count > 0)
            {
                var voiceConfigs = (from r in sysConfigs.First().SubModules
                                    where r.Code == IoConfigParam.Model_Code_VoiceConfig
                                    select r).ToList();
                if (voiceConfigs.Count > 0)
                {
                    List<VoiceModel> models = (from r in voiceConfigs.First().Params
                                               select new VoiceModel()
                                               {
                                                   Id = r.Name,
                                                   Name = r.Lab,
                                                   Content = r.Value
                                               }).ToList();
                    vms = new ObservableCollection<VoiceModel>(models);
                }
            }
            return vms;
        }

        /// <summary>
        /// 向任务服务器发送语音提示的方法
        /// </summary>
        /// <param name="obj"></param>
        public static void SendVoiceInfoToMeasure(object obj,string measureClientId)
        {
            VoiceModel vm = obj as VoiceModel;
            int unm = CommonMethod.CommonMethod.GetRandom();
            var para = new
            {
                clientid = measureClientId,
                cmd = ParamCmd.Voice_Prompt,
                msg = vm,
                msgid = unm
            };
            SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
        }

        /// <summary>
        /// 向任务服务器发送语音对讲开始的方法
        /// </summary>
        /// <param name="obj"></param>
        public static void SendVoiceTalkStartToMeasure(string measureClientId)
        {
            int unm = CommonMethod.CommonMethod.GetRandom();
            var para = new
            {
                clientid = measureClientId,
                cmd = ParamCmd.Voice_Talk_Start,
                msg = "语音对讲开始",
                msgid = unm
            };
            SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
           // SocketCls.Emit(SeatSendCmdEnum.cmd2client, paraJsonStr);
        }

        /// <summary>
        /// 向任务服务器发送语音对讲结束的方法
        /// </summary>
        /// <param name="obj"></param>
        public static void SendVoiceTalkEndToMeasure(string measureClientId)
        {
            int unm = CommonMethod.CommonMethod.GetRandom();
            var para = new
            {
                clientid = measureClientId,
                cmd = ParamCmd.Voice_Talk_End,
                msg = "语音对讲结束",
                msgid = unm
            };
            SocketCls.Emit(SeatSendCmdEnum.cmd2client, JsonConvert.SerializeObject(para));
            // SocketCls.Emit(SeatSendCmdEnum.cmd2client, paraJsonStr)
        }
    }
}
