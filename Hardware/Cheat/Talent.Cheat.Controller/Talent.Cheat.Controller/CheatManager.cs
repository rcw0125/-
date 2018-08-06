using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Talent.Cheat.AnGuoAQD108;
using Talent.Cheat.Interface;
using Talent.Measure.DomainModel;

namespace Talent.Cheat.Controller
{
     /// <summary>
        /// 防作弊管理类
        /// </summary>
    public class CheatManager
    {

        public event ReceivedCheatData OnReceivedCheatData;
        public event OnShowError OnShowErrorMsg;

            /// <summary>
            /// 当前表头
            /// </summary>
        static ICheatController _curCheatDev;
            /// <summary>
            /// 当前配置信息
            /// </summary>
            CheatApparatus _curCheatCfg;

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
            public CheatManager(string pConfigFile)
            {
                ConfigReader cfgReader = new ConfigReader(pConfigFile);
                _curCheatCfg = ConfigReader.ReadCheatApparatusCfg();
            }

            /// <summary>
            /// 打开表头设备
            /// </summary>
            /// <returns></returns>
            public bool Open()
            {
                bool rtn = false;
                if (false == _curCheatCfg.IsUse)
                {
                    _curCheatDev_OnShowErrorMsg("设备未启用。");
                    return false;
                }
                CreateInstance();
                if (_curCheatDev != null)
                {
                    rtn = _curCheatDev.Open();
                }
                return rtn;
            } 
            /// <summary>
            /// 关闭设备
            /// </summary>
            public void Close()
            {
                if (_curCheatDev != null)
                {
                    _curCheatDev.Close();
                }
            }
 
            /// <summary>
            /// 创建实例
            /// </summary>
            private void CreateInstance()
            {
                //string driverPath = System.AppDomain.CurrentDomain.BaseDirectory;
                //driverPath = Path.Combine(driverPath, _curCheatCfg.DriverName);
                //Assembly assembly = Assembly.LoadFile(driverPath);

                //string name = assembly.FullName.Split(',')[0] + ".CheatController";
                //Type type = assembly.GetType(name);
                //_curCheatDev = Activator.CreateInstance(type, _curCheatCfg) as ICheatController; 
                _curCheatDev = new CheatController(_curCheatCfg);
                _curCheatDev.OnReceivedCheatData += _curCheatDev_OnReceivedCheatData;
                _curCheatDev.OnShowErrorMsg += _curCheatDev_OnShowErrorMsg;
            }

            void _curCheatDev_OnShowErrorMsg(string obj)
            {
                if (OnShowErrorMsg != null)
                {
                    OnShowErrorMsg(obj);
                }
            }

            void _curCheatDev_OnReceivedCheatData(string pRawData)
            {
                if (OnReceivedCheatData != null)
                {
                    OnReceivedCheatData(pRawData);
                }
            }
       
    }
}
