using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Talent.Io.HKVideo;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.CommonModel;
using Talent.CommonMethod;

namespace Talent.Io.Controller
{
    /// <summary>
    /// Io控制类
    /// </summary>
    public class IoController
    {
        /// <summary>
        /// 弹出错误消息
        /// </summary>
        public event Action<string> OnShowErrMsg;
        /// <summary>
        /// 报警事件
        /// </summary>
        public event ReceiveAlarmSignal OnReceiveAlarmSignal;

        private static IIoController ioc;
        /// <summary>
        /// 设备驱动接口对象
        /// </summary>
        public static IIoController Ioc
        {
            get { return ioc; }
            set { ioc = value; }
        }
        private IOconfig ioConfig;
        private string ConfigPath { get; set; }

        public IoController(string configPath)
        {
            this.ConfigPath = configPath;
            if (Ioc == null)
            {
                Ioc = CreateInstance();
                Ioc.OnShowErrMsg += ShowMsg;
                Ioc.OnReceiveAlarmSignal += AlarmMethod;
            }            
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="obj"></param>
        private void ShowMsg(string msg)
        {
            if (OnShowErrMsg != null)
            {
                OnShowErrMsg(msg);
            }
        }

        /// <summary>
        /// 报警处理
        /// </summary>
        /// <param name="pDeviceCode">设备编号</param>
        /// <param name="pValue">报警值控制状态 0:未遮挡;1:遮挡</param>
        private void AlarmMethod(string pDeviceCode, string pValue)
        {
            if (OnReceiveAlarmSignal!=null)
            {
                OnReceiveAlarmSignal(pDeviceCode, pValue);
            }
        }

        /// <summary>
        /// 测试用执行命令方法
        /// </summary>
        /// <param name="commandStr">命令字符串</param>
        /// <returns>真假</returns>
        public bool TestExecCommand(string commandStr)
        {
            try
            {
                if (ioc == null)
                {
                    ioc = CreateInstance();
                }
                return ioc.ExecTestCommand(commandStr); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="pCommandList">命令集合</param>
        /// <returns>真假</returns>
        public bool ExecCommand(List<IoCommand> pCommandList)
        {
            try
            {
                if (ioc == null)
                {
                    ioc = CreateInstance();
                }
                return ioc.ExecCommand(pCommandList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="pJsonCmd">命令字符串</param>
        /// <returns>真假</returns>
        public bool ExecCommand(string pJsonCmd)
        {
            try
            {
                if (ioc == null)
                {
                    ioc = CreateInstance();
                }
                return ioc.ExecCommand(pJsonCmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        private IIoController CreateInstance()
        {
            string driverPath = System.AppDomain.CurrentDomain.BaseDirectory;

            ConfigReader cfgReader = new ConfigReader(this.ConfigPath);
            ioConfig = ConfigReader.ReadIoConfig();
            driverPath = Path.Combine(driverPath, ioConfig.EquDll);
            Assembly assembly = Assembly.LoadFile(driverPath);

            string name = assembly.FullName.Split(',')[0] + ".NVRController";
             Type type = assembly.GetType(name);
            // IIoController io = Activator.CreateInstance(type, ioConfig) as IIoController;
            IIoController io =new NVRController(ioConfig);
            return io;
        }
    }
}
