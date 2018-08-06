using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Talent.ClinetLog;
using Talent.Measure.DomainModel;

namespace Talent.Ic.Controller
{
    public class IcCardsController
    {

        public event Action<string> OnShowErrorMsg;
        /// <summary>
        /// 读取卡号
        /// </summary>
        public event ReadCardNo OnReadCardNo;

        /// <summary>
        /// 移卡
        /// </summary>
        public event RemoveCard OnRemoveCard;

        List<ICCard> _curCfgList;
        bool _canReadData = false;
        List<IIcController> _curIcReaderList;

        public IcCardsController(string pConfigFile, bool pCanReadData)
        {
            //读取配置
            IcCardReaderLogger.Debug(string.Format("---------------------开始------------------------"));
            ConfigReader cfgReader = new ConfigReader(pConfigFile);
            IcCardReaderLogger.Debug(string.Format("加载配置文件成功！file={0}", pConfigFile));
            List<ICCard> icCardList = ConfigReader.ReadIcCard();
            IcCardReaderLogger.Debug(string.Format("读取配置文件成功！"));
            InnerInit(icCardList, pCanReadData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pICCardList"></param>
        /// <param name="pCanReadData"></param>
        public IcCardsController(List<ICCard> pICCardList, bool pCanReadData)
        {
            InnerInit(pICCardList, pCanReadData);
        }

        /// <summary>
        /// 内部初始化,注册事件等
        /// </summary>
        /// <param name="pICCardList"></param>
        /// <param name="pCanReadData"></param>
        private void InnerInit(List<ICCard> pICCardList, bool pCanReadData)
        {
            _curCfgList = pICCardList;
            _canReadData = pCanReadData;
            _curIcReaderList = new List<IIcController>();
            IIcController tempIIcController;
            foreach (ICCard item in pICCardList)
            {
                tempIIcController = CreateInstance(item);
                if (tempIIcController != null)
                {
                    tempIIcController.OnShowErrMsg += tempIIcController_OnShowErrMsg;
                    tempIIcController.OnReadCardNo += con_OnReadCardNo;
                    tempIIcController.OnRemoveCard += tempIIcController_OnRemoveCard;
                    _curIcReaderList.Add(tempIIcController);
                }
            }
        }

        void tempIIcController_OnRemoveCard(string pComPortNo)
        {
            if (OnRemoveCard != null)
            {
                OnRemoveCard(pComPortNo);
            }
        }

        /// <summary>
        /// 展示错误消息
        /// </summary>
        /// <param name="obj"></param>
        void tempIIcController_OnShowErrMsg(string obj)
        {
            if (OnShowErrorMsg != null)
            {
                OnShowErrorMsg(obj);
            }
        }

        /// <summary>
        /// 读到数据处理
        /// </summary>
        /// <param name="pComPortNo"></param>
        /// <param name="pCardNo"></param>
        void con_OnReadCardNo(string pComPortNo, string pCardNo)
        {
            if (OnReadCardNo != null)
            {
                OnReadCardNo(pComPortNo, pCardNo);
            }
        }

        /// <summary>
        /// 打开所有RFID设备
        /// </summary>
        public bool Open()
        {
            bool rtn = false;
            //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(m =>
            //{
            foreach (IIcController item in _curIcReaderList)
            {
                rtn = item.Open();
                if (rtn == false)
                    break;
            }
            //}));
            return rtn;
        }

        /// <summary>
        ///启动RFID设备扫描卡片功能
        /// </summary>
        public bool Start()
        {
            bool rtn = false;
            // System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(m =>
            //{
            foreach (IIcController item in _curIcReaderList)
            {
                item.Start();
                if (rtn == false)
                    break;
            }
            //}));
            return rtn;
        }

        /// <summary>
        /// 停止RFID设备扫描卡片功能
        /// </summary>
        public void Stop()
        {
            foreach (IIcController item in _curIcReaderList)
            {
                item.Stop();
            }
        }

        /// <summary>
        /// 关闭所有RFID设备
        /// </summary>
        public void Close()
        {
            foreach (IIcController item in _curIcReaderList)
            {
                item.Close();
            }
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        private IIcController CreateInstance(ICCard pICCardConfig)
        {
            IIcController io = null;
            try
            {
                string driverPath = System.AppDomain.CurrentDomain.BaseDirectory;
                driverPath = Path.Combine(driverPath, pICCardConfig.Driver);
                Assembly assembly = Assembly.LoadFile(driverPath);

                string name = assembly.FullName.Split(',')[0] + ".IcController";
                Type type = assembly.GetType(name);
                IcCardReaderLogger.Debug(string.Format("载入硬件封装模块！DLL={0}", driverPath));
                // IIcController io = new IcController(pICCardConfig, _canReadData);


                io = Activator.CreateInstance(type, pICCardConfig, _canReadData) as IIcController;
                IcCardReaderLogger.Debug("创建IC读卡器实例成功！");
            }
            catch (Exception ex)
            {
                IcCardReaderLogger.Error("创建IC读卡器实例失败！原因：" + ex.Message);
                throw ex;
            }
            return io;//
        }
    }
}
