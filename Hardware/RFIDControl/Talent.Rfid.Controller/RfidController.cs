using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Talent.ClinetLog;
using Talent.Measure.DomainModel;
using Talent.Rfid.Interface;
using Talent_LT.HelpClass;

namespace Talent.Rfid.Controller
{
    public class RfidController
    {
        /// <summary>
        /// 识别出卡号的事件
        /// </summary>
        public event ReceivedData OnReceivedData;
        public event Action<String> OnShowErrorMsg;
        private List<RfidCfg> curCfgList;
        private List<IRfidController> rfidList;
        private List<string> cardId;
       // public bool Is
        /// <summary>
        /// 所有设备是否启用
        /// </summary>
        public bool IsAllDeviceEnable
        {
            get { return (curCfgList!=null && curCfgList.Count>0)?true:false; }
        }
        /// <summary>
        /// 卡号
        /// </summary>
        public List<string> CardId
        {
            get { return cardId; }
            set { cardId = value; }
        }

        #region 构造
        /// <summary>
        ///  配置文件名
        /// </summary>
        /// <param name="pConfigFile"></param>
        /// <param name="isMulti"></param>
        public RfidController(string pConfigFile)
        {
            RFIDReaderLogger.Debug("-----------------------------开始-------------------------------", "");
            //读取配置
            ConfigReader cfgReader = new ConfigReader(pConfigFile);
            if (curCfgList == null)
            {
                curCfgList = ConfigReader.ReadListRfidConfig();
                RFIDReaderLogger.Debug("读取配置文件。pConfigFile=" + pConfigFile, "");
            }
            rfidList = new List<IRfidController>();
            for (int i = 0; i < curCfgList.Count; i++)
            {
                IRfidController rfid = CreateInstance(curCfgList[i]);
                rfid.OnShowErrorMsg += rfid_OnShowErrorMsg;
                rfid.onReceivedData += ReceivedDataMethod;
                rfidList.Add(rfid);
            }
        }
        public RfidController(RfidCfg  pConfig)
        {
            curCfgList = new List<RfidCfg>();
            curCfgList.Add(pConfig);
            rfidList = new List<IRfidController>();
            for (int i = 0; i < curCfgList.Count; i++)
            {
                IRfidController rfid = CreateInstance(curCfgList[i]);
                rfid.OnShowErrorMsg += rfid_OnShowErrorMsg;
                rfid.onReceivedData += ReceivedDataMethod;
                rfidList.Add(rfid);
            }
        }
        #endregion

        /// <summary>
        /// 接收到的数据
        /// </summary>
        /// <param name="pCardId">卡号</param>
        private void ReceivedDataMethod(List<string> pCardIdList)
        {
            CardId = pCardIdList;
            if (OnReceivedData != null)
            {
                OnReceivedData(pCardIdList);
            }
        }

        /// <summary>
        /// 触发错误消息提示
        /// </summary>
        /// <param name="pMsg"></param>
        private void rfid_OnShowErrorMsg(string pMsg)
        {
            if (OnShowErrorMsg != null)
            {
                OnShowErrorMsg(pMsg);
            }
        }

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool rtn = false;
            if (rfidList.Count > 0)
            {
                for (int i = 0; i < rfidList.Count; i++)
                {
                    rtn = rfidList[i].Open();
                }
            }

            return rtn;
        }

        /// <summary>
        /// 启动数据读取功能
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            bool rtn = true;
            if (rfidList.Count > 0)
            {
                for (int i = 0; i < rfidList.Count; i++)
                {
                    rtn = rfidList[i].Start() && rtn;
                }
            }
            return rtn;
        }
        /// <summary>
        /// 读取一次 不再开timer关timer
        /// </summary>
        /// <returns></returns>
        public string ReadRFIDCardOrgNo()
        {
            string rtn = string.Empty;
            if (rfidList.Count > 0)
            {
                for (int i = 0; i < rfidList.Count; i++)
                {
                    rtn = rfidList[i].ReadRFIDCardOrgNo();
                    break;
                }
            }
            return rtn;
        }

        /// <summary>
        /// 停止数据读取功能
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            bool rtn = true;
            if (rfidList.Count > 0)
            {
                for (int i = 0; i < rfidList.Count; i++)
                {
                    rtn = rfidList[i].Stop() && rtn;
                }
            }
            return rtn;
        }

        /// <summary>
        /// 关闭注销设备
        /// </summary>
        /// <returns></returns>
        public bool Close()
        { 
            bool rtn = true;
            if (rfidList.Count > 0)
            {
                for (int i = 0; i < rfidList.Count; i++)
                {
                    rtn = rfidList[i].Close() && rtn;
                }
            }
            return rtn;
        }


        /// <summary>
        /// 创建RFID实例
        /// </summary>
        /// <returns></returns>
        private IRfidController CreateInstance(RfidCfg rf)
        {
            try
            {
                string driverPath = System.AppDomain.CurrentDomain.BaseDirectory;
                driverPath = Path.Combine(driverPath, rf.EquDll);
                Assembly assembly = Assembly.LoadFile(driverPath);
                RFIDReaderLogger.Debug("载入硬件封装模块。driverPath=" + driverPath, "");

                string name = assembly.FullName.Split(',')[0] + ".RfidController";
                Type type = assembly.GetType(name);
                IRfidController irfid = Activator.CreateInstance(type, rf) as IRfidController;
               
                return irfid;
            }
            catch (Exception ex)
            {
                RFIDReaderLogger.Error("CreateInstance异常。", "",ex);
                return null;

            }
        }
    }
}
