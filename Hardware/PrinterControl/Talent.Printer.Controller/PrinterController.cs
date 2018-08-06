using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Talent.ClinetLog;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.PrintModel;
using Talent.Printer.Interface;


namespace Talent.Printer.Controller
{
    /// <summary>
    /// 打印控制类
    /// </summary>
    public class PrinterController
    {
        List<string> errList;
        ILog _log;
        List<IPrint> _curPrinterList;
        private string ConfigPath { get; set; }
        private List<PrinterConfig> _curConfigList;

        /// <summary>
        /// 显示错误消息
        /// </summary>
        public event Action<ErrorType, string> OnShowErrMsg;

        public PrinterController(string pConfigPath)
        {
            errList = new List<string>();
            ConfigPath = pConfigPath;
           // _log = LogHelper.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "ClientConfig\\Log4Net.config", "PRINTER_LOG");
            CreateInstance();
            if (_curPrinterList == null || _curPrinterList.Count<=0)
            {
                PrinterDevLogger.Error("打印机对象创建失败。","");
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="pBillList"></param>
        public void Print(List<Bill> pBillList)
        {
            errList.Clear();
            bool rtn = false;
            if (_curPrinterList != null)
            {
                foreach (IPrint p in _curPrinterList)
                {

                    try
                    {
                        rtn = p.Print(pBillList);
                    }
                    catch
                    {
                        rtn = false;
                    }
                    if (rtn)
                    {
                        break;
                    }
                }
            }
            if (rtn == false)
            {
                string temp = string.Join("。", errList.ToArray());
                OnShowErrMsg(ErrorType.Error, temp);
            }
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="pBillList"></param>
        public void Print(List<string> pBillList)
        {
            if (_curPrinterList != null)
            {
                List<Bill> tBillList = new List<Bill>();
                XmlSerializer configSer = new XmlSerializer(typeof(Bill));
                MemoryStream sr;
                foreach (string item in pBillList)
                {
                    sr = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(item));
                    Bill tbill = (Bill)configSer.Deserialize(sr);
                    tBillList.Add(tbill);
                    sr.Dispose();

                }
                Print(tBillList);
            }
        }


        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        private void CreateInstance()
        {
            _curPrinterList = new List<IPrint>();
            IPrint io = null;
            try
            {
                string tBaseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                ConfigReader cfgReader = new ConfigReader(this.ConfigPath);
                _curConfigList = ConfigReader.ReadPrinterConfig();
                PrinterDevLogger.Debug("读取打印机配置.ConfigPath=" + ConfigPath, "");

                string msg = string.Empty;
                foreach (PrinterConfig cfg in _curConfigList)
                {
                    string driverPath = Path.Combine(tBaseDirectory, cfg.Driver);
                    Assembly assembly = Assembly.LoadFile(driverPath);
                    PrinterDevLogger.Debug("加载组件.driverPath=" + driverPath, "");

                    string name = assembly.FullName.Split(',')[0] + ".PrinterControl";
                    Type type = assembly.GetType(name);
                    io = Activator.CreateInstance(type, cfg) as IPrint;
                    io.OnShowErrMsg += ShowErrorMsg;
                    _curPrinterList.Add(io);
                    //if(io.CheckPrinterState(out msg))
                    //{
                    //    _log.Info("打印机对象创建成功");
                    //    break;
                    //}
                }
            }
            catch (Exception ex)
            {
                ShowErrorMsg(ErrorType.Error, "创建打印机对象失败，请检查打印插件相关。");
                PrinterDevLogger.Error("打印机对象创建失败，请检查打印插件相关。","", ex);
            }
        }



        private void ShowErrorMsg(ErrorType pErrorType, string pMsg)
        {
            if (OnShowErrMsg != null)
            {
                errList.Add(pMsg);
                // OnShowErrMsg(pErrorType, pMsg);
            }
        }
     

    }
}
