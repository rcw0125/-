using log4net;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using Talent.ClinetLog;
using Talent.CommonMethod;
using Talent.Measure.DomainModel;
using Talent.Measure.DomainModel.PrintModel;
using Talent.Printer.Interface;

namespace Talent.Printer.CustomVKP800II
{
    /// <summary>
    /// 打印机控制类
    /// </summary>
    public class PrinterControl : IPrint
    {
        /// <summary>
        /// 日志
        /// </summary>
        //ILog _log;

        bool _isComportReceiveData = false;
        /// <summary>
        /// 是否可以打印
        /// </summary>
        AutoResetEvent _canPrint;

        /// <summary>
        /// 缓存串口读取到的数据
        /// </summary>
        Byte[] _curComBufferData = null;
        /// <summary>
        /// 串口是否打开
        /// </summary>
        public bool IsOpen
        {
            get { return _spCom.IsOpen; }
        }

        /// <summary>
        /// 打印模板
        /// </summary>
        static List<Bill> _curBillList;
        /// <summary>
        /// 要打印的数据集合
        /// </summary>
        List<Dictionary<string, string>> _curDicPrintDataList;
        /// <summary>
        /// 串口类
        /// </summary>
        private SerialPort _spCom;
        /// <summary>
        /// 打印机配置
        /// </summary>
        PrinterConfig _curPrinterConfig;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPrinterConfig">打印机配置</param>
        public PrinterControl(PrinterConfig pPrinterConfig)
        {
            _curPrinterConfig = pPrinterConfig;
            //_log = pLog;
            _canPrint = new AutoResetEvent(true);
            _curComBufferData = new Byte[100];
            _spCom = new SerialPort(_curPrinterConfig.ComPort, _curPrinterConfig.Baudrate);
            _spCom.DataBits = 8;
            _spCom.StopBits = StopBits.One;
            _spCom.Parity = Parity.None;
            _spCom.DataReceived += _spCom_DataReceived;
            _spCom.ErrorReceived += _spCom_ErrorReceived;
            _spCom.ReadBufferSize = 100;
        }


        void _spCom_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
        }


        //public void Print(List<Dictionary<string, string>> pDicPrintDataList)
        //{
        //    _curDicPrintDataList = pDicPrintDataList;
        //    ThreadStart ts = new ThreadStart(PrintInner);
        //    Thread tPrint = new Thread(ts);
        //    tPrint.IsBackground = true;
        //    tPrint.Start();
        //}

        /// <summary>
        /// 显示错误消息
        /// </summary>
        public event Action<ErrorType, string> OnShowErrMsg;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pData"></param>
        public bool Print(List<Bill> pBillList)
        {
            _curBillList = pBillList;
            return PrintInner2();
            //ThreadStart ts = new ThreadStart(PrintInner2);
            //Thread tPrint = new Thread(ts);
            //tPrint.IsBackground = true;
            //tPrint.Start();
        }


        /// <summary>
        /// 打印
        /// </summary>
        /// <returns></returns>
        //private void PrintInner1()
        //{
        //    _canPrint.WaitOne();
        //    if (_spCom.IsOpen)
        //    {
        //        _spCom.Close();
        //        Thread.Sleep(100);
        //    }
        //    bool printerState = CheckPrintState();
        //    //如果打印机可用
        //    if (printerState)
        //    {
        //        _spCom.Open();
        //        Thread.Sleep(100);
        //        WriteComData(PrinterCommand.CLEAR_PRINTER_BUFFER);
        //        WriteComData(PrinterCommand.INIT_PRINTER);
        //        if (_curPrinterConfig.Notch)
        //        {
        //            WriteComData(PrinterCommand.SET_PRINTER_NOTCH_DISTANCE);//
        //            WriteComData(PrinterCommand.SET_PRINTER_HERDER_NOTCH_ALIGNMENT);
        //        }
        //        foreach (Dictionary<string, string> item in _curDicPrintDataList)
        //        {
        //            Bill tBill = _curBillList.Find(m => m.Code == item["code"]);
        //            if (tBill != null)
        //            {
        //                foreach (Row row in tBill.RowList)
        //                {
        //                    string drawData = row.Value;
        //                    if (row.Params != null)
        //                    {
        //                        foreach (Param p in row.Params)
        //                        {
        //                            drawData = drawData.Replace(p.Value, item[p.Value]);
        //                        }
        //                    }
        //                    //设置字符大小
        //                    WriteComData(PrinterCommand.JoinCommand(PrinterCommand.SELECT_PRINTER_CHRARACTER_SIZE, Convert.ToByte(row.Width + row.Height, 8)));
        //                    WriteComData(System.Text.Encoding.UTF8.GetBytes(drawData));//打印数据
        //                    WriteComData(PrinterCommand.PRINT_NEWLINE);//换行
        //                    for (int lineCount = 0; lineCount < row.Line; lineCount++)
        //                    {
        //                        WriteComData(PrinterCommand.PRINT_NEWLINE);//换行
        //                    }

        //                    if (_curPrinterConfig.Notch)
        //                    {
        //                        WriteComData(PrinterCommand.SET_PRINTER_CUTPAPER_NOTCH_ALIGNMENT);
        //                        Thread.Sleep(200);
        //                    }

        //                    WriteComData(PrinterCommand.PRINTER_TOTAL_CUT);//等待
        //                    WriteComData(PrinterCommand.PRINTER_EJECTOR_TICKET);//出票
        //                    WriteComData(PrinterCommand.PRINTER_PRESENTS_TICKET);//提票
        //                    Thread.Sleep(200);
        //                    WriteComData(PrinterCommand.CLEAR_PRINTER_BUFFER);//清除缓存
        //                }
        //            }
        //        }

        //        Thread.Sleep(200);
        //        _spCom.Close();
        //        _canPrint.Set();
        //    }
        //}

        /// <summary>
        /// 打印2
        /// </summary>
        /// <returns></returns>
        private bool PrintInner2()
        {
            bool rtn = false;
            _canPrint.WaitOne();
            if (_spCom.IsOpen)
            {
                _spCom.Close();
                Thread.Sleep(100);
            }
            string msg = string.Empty;
            bool tCanPrint = CheckPrinterState(out msg);
            PrinterDevLogger.Debug(string.Format("检查打印机是否可打印，tCanPrint={0},msg={1}", tCanPrint, msg), _curPrinterConfig.ComPort);
            //if (!tCanPrint)
            //{
            //    _log.Debug(msg);
            //}
            //如果打印机可用
            if (tCanPrint)
            {
                PrinterDevLogger.Debug("==================开始打印====================", _curPrinterConfig.ComPort);
                #region print
                _spCom.Open();
                PrinterDevLogger.Debug("打开串口。， _spCom.IsOpen=" + _spCom.IsOpen.ToString(), _curPrinterConfig.ComPort);
                Thread.Sleep(50);
                WriteComData(PrinterCommand.CLEAR_PRINTER_BUFFER);//0X18
                Thread.Sleep(50);
                if (_curPrinterConfig.Notch)
                {
                    PrinterDevLogger.Debug("黑标对齐。", _curPrinterConfig.ComPort);
                    WriteComData(PrinterCommand.SET_PRINTER_HERDER_NOTCH_ALIGNMENT);
                    Thread.Sleep(1000);
                }
                PrinterDevLogger.Debug("打印内容开始。", _curPrinterConfig.ComPort);
                foreach (Bill item in _curBillList)
                {
                    foreach (Row row in item.RowList)
                    {
                        //设置字符大小
                        WriteComData(PrinterCommand.JoinCommand(PrinterCommand.SELECT_PRINTER_CHRARACTER_SIZE, Convert.ToByte(int.Parse(row.Width) * 16 + int.Parse(row.Height))));
                        WriteComData(System.Text.Encoding.GetEncoding("GB18030").GetBytes(row.Value));//打印数据
                        WriteComData(PrinterCommand.PRINT_NEWLINE);//换行 0X0A
                    }
                    if (item.RowList.Count > 0)
                    {
                        WriteComData(PrinterCommand.PRINT_NEWLINE);//换行 0X0A
                    }
                }
                PrinterDevLogger.Debug("打印内容结束。", _curPrinterConfig.ComPort);
                if (_curPrinterConfig.Notch)
                {
                    WriteComData(PrinterCommand.SET_PRINTER_CUTPAPER_NOTCH_ALIGNMENT);
                    Thread.Sleep(1000);
                }
                
                WriteComData(PrinterCommand.PRINTER_TOTAL_CUT);//切票 0X1B 0X69
                PrinterDevLogger.Debug("切纸。", _curPrinterConfig.ComPort);
                WriteComData(PrinterCommand.PRINTER_PRESENTS_TICKET);//提票 0X1D, 0X65, 0X03, 0X0F 
                PrinterDevLogger.Debug("出票。", _curPrinterConfig.ComPort);
                Thread.Sleep(100);
                _spCom.Close();
                PrinterDevLogger.Debug("关闭串口， _spCom.IsOpen=" + _spCom.IsOpen.ToString(), _curPrinterConfig.ComPort);
                #endregion
                rtn = true;
            }
            else
            {
                ShowErrorMsg(ErrorType.Error, msg);
            }
            return rtn;
        }
        public bool IsPrintSuccess(out string pMsg)
        {
            bool result = true;
            this._isComportReceiveData = false;
            pMsg = "";
            this.WriteComData(PrinterCommand.JoinCommand(PrinterCommand.DETECTION_PRINTER, 17));
            int num = 0;
            this._curComBufferData = new byte[0];
            while (!this._isComportReceiveData)
            {
                num++;
                if (num >= 50)
                {
                    break;
                }
                Thread.Sleep(20);
            }
            if (num < 50)
            {
                byte b = this._curComBufferData[0];
                if ((b & 0x20) == 32)
                {
                    pMsg += "打印失败，未出纸。";
                    result = false;
                }
            }
           
            return result;
        }
        void _spCom_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //16,15,68
            _spCom.Read(_curComBufferData, 0, 100);
            _isComportReceiveData = true;
        }



        /// <summary>
        /// 检测打印机
        /// </summary>
        /// <param name="pErrorMsg"></param>
        public bool CheckPrinterState(out string pMsg)
        {
            bool tCanPrint = true;
            try
            {
                _isComportReceiveData = false;
                pMsg = string.Empty;//错误信息
                if (_spCom.IsOpen)
                {
                    _spCom.Close();
                    Thread.Sleep(100);
                }
                _spCom.Open();
                Thread.Sleep(100);
                if (_spCom.IsOpen)
                {
                    #region COM口打开
                    WriteComData(PrinterCommand.CLEAR_PRINTER_BUFFER);
                    WriteComData(PrinterCommand.INIT_PRINTER);
                    WriteComData(PrinterCommand.JoinCommand(PrinterCommand.DETECTION_PRINTER, (Byte)20));

                    int count = 0;
                    while (!_isComportReceiveData)
                    {
                        count++;
                        if (count >= 100)
                        {
                            break;
                        }
                        Thread.Sleep(20);
                    }
                    //超时
                    if (count < 100)
                    {
                        byte[] tempArray = new byte[] { _curComBufferData[0], _curComBufferData[1], _curComBufferData[2], _curComBufferData[3], _curComBufferData[4], _curComBufferData[5] };

                        PrinterDevLogger.Debug("打印机状态：" + ByteArrayToHexString(tempArray), _curPrinterConfig.ComPort);
                        #region check printer state
                        #region paper status
                        byte tPaperStatus = _curComBufferData[2];
                        //if (tPaperStatus != 0)
                        //{
                            if ((tPaperStatus & 0X01) == 1)
                            {
                                pMsg += "没有打印纸，缺纸。";
                                tCanPrint = false;
                            }
                            if ((tPaperStatus & 0X04) == 4)
                            {
                                pMsg += "即将缺纸。";
                            }
                            if ((tPaperStatus & 0X20) == 32)
                            {
                                pMsg += "弹纸盒里面有纸，未弹出。";
                                WriteComData(PrinterCommand.PRINTER_EJECTOR_TICKET);//出票 0X1D, 0X65, 0X05
                                //tCanPrint = false;
                            }
                            //if (!string.IsNullOrEmpty(pMsg))
                            //{
                            //    pMsg = "未知错误";
                            //    tCanPrint = false;
                            //}
                       // }
                        
                        #endregion
                        #region user status
                        byte tUserStatus =_curComBufferData[3];
                        //if (tUserStatus != 0)
                        //{
                            if ((tUserStatus & 0X01) == 1 || (tUserStatus & 0X02) == 2)
                            {
                                pMsg += "打印机盖没有合上。";
                                tCanPrint = false;
                            }
                            //if (string.IsNullOrEmpty(pMsg))
                            //{
                            //    pMsg = "未知错误";
                            //    tCanPrint = false;
                            //}
                       // }
                        //switch (tUserStatus)
                        //{
                        //    case 1://打印机盖没有合上
                        //    case 2:
                        //        pMsg += GetErrorMsg(5);
                        //        tCanPrint = false;
                        //        break;
                        //    default:
                        //        break;
                        //}
                        #endregion
                        #region  5= Unrecoverable error status
                        byte tOtherStatus = _curComBufferData[5];
                        //if (tOtherStatus != 0)
                        //{
                            if ((tUserStatus & 0X01) == 1)
                            {
                                pMsg += "切纸错误。";
                                tCanPrint = false;
                            }
                        //    if (string.IsNullOrEmpty(pMsg))
                        //    {
                        //        pMsg = "打印机出现严重错误。";
                        //        tCanPrint = false;
                        //    }
                        //}
                     
                        //switch (tOtherStatus)
                        //{
                        //    case 0:
                        //        break;
                        //    case 1://切刀卡纸错误
                        //        pMsg += GetErrorMsg(6);
                        //        tCanPrint = false;
                        //        break;
                        //    default:
                        //        //tCanPrint = false;
                        //        //pMsg += GetErrorMsg(8);
                        //        break;
                        //}
                        #endregion
                        #endregion
                    }
                    else
                    {
                        tCanPrint = false;
                        pMsg += "无法获取打印机状态，超时。";
                        PrinterDevLogger.Error("无法获取打印机状态，超时。", _curPrinterConfig.ComPort);
                    }
                    _spCom.Close();
                    Thread.Sleep(100);
                    #endregion
                }
                else
                {
                    tCanPrint = false;
                    pMsg += "打印机串口无法打开。";
                    PrinterDevLogger.Error("检测打印机状态时，串口无法打开。", _curPrinterConfig.ComPort);
                }

            }
            catch (Exception ex)
            {
                pMsg = ex.Message;
                tCanPrint = false;
                PrinterDevLogger.Error("检测打印机状态异常。", _curPrinterConfig.ComPort,ex);
            }
            return tCanPrint;
        }

        private void ShowErrorMsg(ErrorType pErrorType, string pMsg)
        {
            if (OnShowErrMsg != null)
            {
                OnShowErrMsg(pErrorType, pMsg);
            }
        }
        /// <summary>
        /// 获取打印机错误
        /// </summary>
        /// <param name="pErrorCode"></param>
        /// <returns></returns>
        private string GetErrorMsg(int pErrorCode)
        {
            string rtn = string.Empty;
            switch (pErrorCode)
            {
                case -1:
                    rtn = "纸将尽，还能打印。";
                    break;
                case 0:
                    rtn = "打印机正常";
                    break;
                case 1:
                    rtn = "通讯线路故障，打印机没有连接。";
                    break;
                case 2:
                    rtn = "配置文件中打印机没有启用。";
                    break;
                case 4:
                    rtn = "弹纸盒里面有纸(卡纸)，未弹出。";
                    break;
                case 5:
                    rtn = "打印机盖没有合上。";
                    break;
                case 6:
                    rtn = "切刀卡纸错误。";
                    break;
                case 7:
                    rtn = "无法连接打印机。";
                    break;
                case 8:
                    rtn = "打印机无响应，获取打印机状态失败。";
                    break;
                case 9:
                    rtn = "RAM寄存器错误。";
                    break;
                case 10:
                    rtn = "ERPROM寄存器错误。";
                    break;
                case 11:
                    rtn = "Flash错误。";
                    break;
                case 12:
                    rtn = "缺纸。";
                    break;
                case 15:
                    rtn = "纸已经用完，不能打印。";
                    break;
                default:
                    rtn = "未知错误代码："; break;
            }

            return rtn + string.Format(".错误编码:{0}", pErrorCode);
        }

        /// <summary>
        /// 向串口发送数据
        /// </summary>
        /// <param name="pData"></param>
        private void WriteComData(byte[] pData)
        {
            _spCom.Write(pData, 0, pData.Length);
        }

        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }
    }


}
