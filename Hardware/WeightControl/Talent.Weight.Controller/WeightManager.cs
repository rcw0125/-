using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Talent.ClinetLog;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Weight.Interface;

namespace Talent.Weight.Controller
{
    /// <summary>
    /// 秤体管理类
    /// </summary>
    public class WeightManager
    {

        public event ReceivedWeightData OnReceivedWeightData;
        public event Action<ErrorType, string> OnShowErrorMsg;

        ILog _log;
        /// <summary>
        /// 当前表头
        /// </summary>
        static IWeightController _curWeightDev;
        /// <summary>
        /// 当前配置信息
        /// </summary>
        WeighingApparatus _curWeightCfg;

        /// <summary>
        /// 是否计量完毕
        /// </summary>
        bool isFinish = false;

        /// <summary>
        /// 是否计量完毕
        /// </summary>
        public bool IsFinish
        {
            get { return isFinish; }
            set { isFinish = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pConfigFile">配置文件</param>
        public WeightManager(string pConfigFile)
        {
            WeightDeviceLogger.Debug("--------------------------开始----------------------------");
            ConfigReader cfgReader = new ConfigReader(pConfigFile);
            try
            {
                _curWeightCfg = ConfigReader.ReadWeighingApparatusCfg();
                WeightDeviceLogger.Debug("读取衡器配置成功。");
            }
            catch (Exception ex)
            {
                WeightDeviceLogger.Error("读取衡器配置异常。", ex);
                throw ex;
            }

        }

        /// <summary>
        /// 打开表头设备
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool rtn = false;
           
            CreateInstance();
            if (_curWeightDev != null)
            {
               rtn= _curWeightDev.Open();
            }
            return rtn;
        }
        /// <summary>
        /// 开始读取重量
        /// </summary>
        public void Start()
        {
            if (_curWeightDev != null)
            {
                _curWeightDev.Start();
            }
        }
        /// <summary>
        /// 停止读取重量
        /// </summary>
        public void Stop()
        {
            if (_curWeightDev != null)
            {
                _curWeightDev.Stop();
            }
        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        public void Close()
        {
            if (_curWeightDev != null)
            {
                _curWeightDev.Close();
            }
        }

        /// <summary>
        /// 表头清零
        /// </summary>
        public void ClearZero()
        {
            if (_curWeightDev != null)
            {
                _curWeightDev.ClearZero();
            }
        }

        /// <summary>
        /// 主界面测试拖动重量用
        /// </summary>
        /// <param name="weight"></param>
        public void reciveWeightData(int weight)
        {
            string ss = "1";
            if (weight > 30000)
            {
                ss = "0";
            }
            _curWeightDev_OnReceivedWeightData(ss,weight.ToString(), "");
           // _curWeight = weight;
            //OnReceivedWeightData(weight, "");
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        private void CreateInstance()
        {
            try
            {
                string driverPath = System.AppDomain.CurrentDomain.BaseDirectory;
                driverPath = Path.Combine(driverPath, _curWeightCfg.DriverName);
                Assembly assembly = Assembly.LoadFile(driverPath);
                WeightDeviceLogger.Debug("加载衡器硬件封装。DLL=" + driverPath);

                string name = assembly.FullName.Split(',')[0] + ".WeightController";
                Type type = assembly.GetType(name);

                _curWeightDev = Activator.CreateInstance(type, _curWeightCfg) as IWeightController;
                _curWeightDev.OnReceivedWeightData += _curWeightDev_OnReceivedWeightData;
                _curWeightDev.OnShowErrorMsg += _curWeightDev_OnShowErrorMsg;
                WeightDeviceLogger.Debug("创建衡器实例成功。");
            }
            catch (Exception ex)
            {
                WeightDeviceLogger.Error("创建衡器实例异常。", ex);
                throw ex;
            }
        }

        void _curWeightDev_OnShowErrorMsg(ErrorType pErrorType, string obj)
        {
            if (OnReceivedWeightData != null)
            {
                OnShowErrorMsg(pErrorType,obj);
            }
        }

        void _curWeightDev_OnReceivedWeightData(string pTag, string pWeight, string pRawData)
        {
            if (OnReceivedWeightData != null)
            {
                OnReceivedWeightData(pTag, pWeight, pRawData);
            }
        }
    }
}
