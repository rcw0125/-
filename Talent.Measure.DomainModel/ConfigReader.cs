using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Talent.Measure.DomainModel.CommonModel;

namespace Talent.Measure.DomainModel
{

    public class ConfigReader
    {
        static XmlDocument _docConfig;
        static string _file;
        /// <summary>
        /// 初始化配置文件
        /// </summary>
        /// <param name="_pFileName">配置文件名+路径</param>
        public ConfigReader(string _pFileName)
        {
            _file = _pFileName;
            if (_docConfig == null)
            {
                _docConfig = new XmlDocument();
                _docConfig.Load(_pFileName);
            }
        }

        /// <summary>
        /// 读取IO配置
        /// </summary>
        /// <returns></returns>
        public static IOconfig ReadIoConfig()
        {
            IOconfig rtnConfig = new IOconfig();
            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == IoConfigParam.Model_Code_IoConfig)
                        {
                            #region IO设置
                            //IO设置
                            rtnConfig.UsePassCarType = subItem.Params.Find(m => m.Name == IoConfigParam.DetectEqu).Value;
                            rtnConfig.ConType = subItem.Params.Find(m => m.Name == IoConfigParam.LinkType).Value;
                            rtnConfig.EquDll = subItem.Params.Find(m => m.Name == IoConfigParam.EquDriverName).Value;
                            rtnConfig.Comport = subItem.Params.Find(m => m.Name == IoConfigParam.Comport).Value;
                            rtnConfig.Ip = subItem.Params.Find(m => m.Name == IoConfigParam.Ip).Value;
                            rtnConfig.Port = subItem.Params.Find(m => m.Name == IoConfigParam.Port).Value;
                            rtnConfig.Baudrate = subItem.Params.Find(m => m.Name == IoConfigParam.Baudrate).Value;
                            rtnConfig.EquLoginName = subItem.Params.Find(m => m.Name == IoConfigParam.EquLoginName).Value;
                            rtnConfig.EquLoginPwd = subItem.Params.Find(m => m.Name == IoConfigParam.EquLoginPwd).Value;

                            rtnConfig.InDeviceList = new List<Device>();
                            rtnConfig.OutDeviceList = new List<Device>();
                            Device temp;
                            //加载端口对应的设备
                            foreach (Row device in subItem.GridRow.RowList)
                            {
                                bool isused = device.Params.Find(m => m.Name == IoConfigParam.Row_IsUse).Value == YesNo.Yes ? true : false;
                                if (isused)
                                {
                                    temp = new Device();
                                    temp.EquName = device.Params.Find(m => m.Name == IoConfigParam.Row_EquName).Value;
                                    temp.Code = device.Params.Find(m => m.Name == IoConfigParam.Row_Code).Value;
                                    temp.EquTypeCode = device.Params.Find(m => m.Name == IoConfigParam.Row_EquTypeCode).Value;
                                    temp.Port = device.Params.Find(m => m.Name == IoConfigParam.Row_Port).Value;
                                    temp.Type = device.Params.Find(m => m.Name == IoConfigParam.Row_Type).Value;
                                    temp.PortType = device.Params.Find(m => m.Name == IoConfigParam.Row_PortType).Value;

                                    if (temp.PortType == IoConfigParam.Row_PortType_In) //输入
                                    {
                                        rtnConfig.InDeviceList.Add(temp);
                                    }
                                    else //输出
                                    {
                                        rtnConfig.OutDeviceList.Add(temp);
                                    }
                                }
                            }
                            #endregion
                            break;
                        }
                    }
                    break;
                }
            }
            return rtnConfig;
        }

        /// <summary>
        /// 读取Ic卡配置
        /// </summary>
        /// <returns></returns>
        public static List<ICCard> ReadIcCard()
        {
            List<ICCard> rtnList = new List<ICCard>();
            ICCard tempCard;
            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == IcCardConfigParam.Model_Code_IcConfig)
                        {
                            //加载端口对应的设备
                            foreach (Row device in subItem.GridRow.RowList)
                            {
                                bool isused = device.Params.Find(m => m.Name == IcCardConfigParam.Row_IsUse).Value == YesNo.Yes ? true : false;
                                if (isused)
                                {
                                    tempCard = new ICCard();
                                    tempCard.ConType = device.Params.Find(m => m.Name == IcCardConfigParam.Col_ConType).Value;
                                    tempCard.Ip = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Ip).Value;
                                    tempCard.Port = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Port).Value;

                                    tempCard.ComPort = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Comport).Value.ToLower().Replace("com", "");
                                    tempCard.ComPort = (int.Parse(tempCard.ComPort) - 1).ToString();
                                    tempCard.Baudrate = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Baudrate).Value.ToLower().Replace("bps", "");
                                    tempCard.Driver = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Driver).Value;

                                    tempCard.Interval = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Interval).Value;
                                    tempCard.ICReadType = device.Params.Find(m => m.Name == IcCardConfigParam.Row_ICReadType).Value;
                                    tempCard.ICWriteTemp = device.Params.Find(m => m.Name == IcCardConfigParam.Row_ICWriteTemp).Value == YesNo.Yes ? true : false;
                                    tempCard.IsUse = true;
                                    rtnList.Add(tempCard);
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return rtnList;
        }

        /// <summary>
        /// 读取RFID卡配置
        /// </summary>
        /// <returns></returns>
        public static RfidCfg ReadRfidConfig()
        {
            RfidCfg rtnConfig = new RfidCfg();
            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == RfidConfigParam.Model_Code_RfidConfig)
                        {
                            #region IO设置
                            //IO设置
                            rtnConfig.UsePassCarType = subItem.Params.Find(m => m.Name == RfidConfigParam.DetectEqu).Value;
                            rtnConfig.ConType = subItem.Params.Find(m => m.Name == RfidConfigParam.LinkType).Value;
                            rtnConfig.EquDll = subItem.Params.Find(m => m.Name == RfidConfigParam.EquDriverName).Value;
                            rtnConfig.ComPort = subItem.Params.Find(m => m.Name == RfidConfigParam.Comport).Value;
                            rtnConfig.Ip = subItem.Params.Find(m => m.Name == RfidConfigParam.Ip).Value;
                            rtnConfig.Port = subItem.Params.Find(m => m.Name == RfidConfigParam.Port).Value;
                            rtnConfig.Baudrate = subItem.Params.Find(m => m.Name == RfidConfigParam.Baudrate).Value;
                            rtnConfig.IsUse = subItem.Params.Find(m => m.Name == RfidConfigParam.IsUse).Value == YesNo.Yes ? true : false;
                            rtnConfig.Power = int.Parse(subItem.Params.Find(m => m.Name == RfidConfigParam.Power).Value);
                            rtnConfig.Interval = int.Parse(subItem.Params.Find(m => m.Name == RfidConfigParam.Interval).Value); ;

                            rtnConfig.AntennaPortList = new List<AntennaPort>();

                            AntennaPort temp;
                            //加载端口对应的设备
                            foreach (Row device in subItem.GridRow.RowList)
                            {
                                bool isused = device.Params.Find(m => m.Name == RfidConfigParam.Row_IsUse).Value == "1" ? true : false;
                                if (isused)
                                {
                                    temp = new AntennaPort();
                                    temp.AntennaName = device.Params.Find(m => m.Name == RfidConfigParam.Row_AntennaName).Value;
                                    temp.Port = device.Params.Find(m => m.Name == RfidConfigParam.Row_Port).Value;
                                    temp.IsUse = isused;
                                    rtnConfig.AntennaPortList.Add(temp);
                                }
                            }
                            #endregion
                            break;
                        }
                    }
                    break;
                }
            }
            return rtnConfig;
        }

        /// <summary>
        /// 读取RFID卡配置
        /// </summary>
        /// <returns></returns>
        public static List<RfidCfg> ReadListRfidConfig()
        {
            List<RfidCfg> rtList = new List<RfidCfg>();
            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == MultiRfidConfigParam.Model_Code_RfidConfig)
                        {

                            //加载端口对应的设备
                            foreach (Row device in subItem.GridRow.RowList)
                            {
                                bool isused = device.Params.Find(m => m.Name == IcCardConfigParam.Row_IsUse).Value == YesNo.Yes ? true : false;
                                if (isused)
                                {
                                    RfidCfg rtnConfig = new RfidCfg();
                                    rtnConfig.UsePassCarType = device.Params.Find(m => m.Name == IcCardConfigParam.Row_ICReadType).Value;
                                    rtnConfig.ConType = device.Params.Find(m => m.Name == IcCardConfigParam.Col_ConType).Value;
                                    rtnConfig.EquDll = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Driver).Value;
                                    rtnConfig.ComPort = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Comport).Value;
                                    rtnConfig.Ip = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Ip).Value;
                                    rtnConfig.Port = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Port).Value;
                                    rtnConfig.Baudrate = device.Params.Find(m => m.Name == IcCardConfigParam.Row_Baudrate).Value;
                                    rtnConfig.IsUse = device.Params.Find(m => m.Name == IcCardConfigParam.Row_IsUse).Value == YesNo.Yes ? true : false;
                                    rtnConfig.Power = 0;
                                    rtnConfig.Interval = 1000;
                                    rtList.Add(rtnConfig);
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return rtList;
        }

        /// <summary>
        /// 衡器配置
        /// </summary>
        /// <returns></returns>
        public static WeighingApparatus ReadWeighingApparatusCfg()
        {
            WeighingApparatus rtnConfig = new WeighingApparatus();
            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == WeighterConfigParam.Model_Code_WeighterConfig)
                        {
                            //设备驱动相关
                            string temp = subItem.Params.Find(m => m.Name == WeighterConfigParam.DeviceName).Value;
                            string[] data = temp.Split(new Char[] { '@' });
                            if (data.Length == 2)
                            {
                                rtnConfig.DeviceName = data[0];
                                rtnConfig.DriverName = data[1];
                            }
                            //串口相关
                            rtnConfig.ComPort = subItem.Params.Find(m => m.Name == WeighterConfigParam.Comport).Value;
                            rtnConfig.Baudrate = subItem.Params.Find(m => m.Name == WeighterConfigParam.Baudrate).Value.ToLower().Replace("bps", "");
                            rtnConfig.StopSize = subItem.Params.Find(m => m.Name == WeighterConfigParam.Stopsize).Value;
                            rtnConfig.Parity = subItem.Params.Find(m => m.Name == WeighterConfigParam.Parity).Value;
                            rtnConfig.ByteSize = int.Parse(subItem.Params.Find(m => m.Name == WeighterConfigParam.ByteSize).Value);
                            int tempMaxWeight = 50000;
                            int.TryParse(subItem.Params.Find(m => m.Name == WeighterConfigParam.MaxWeight).Value,out tempMaxWeight);
                            rtnConfig.MaxWeight =tempMaxWeight;
                            ////数据相关
                            //rtnConfig.DataMarkType = subItem.Params.Find(m => m.Name == WeighterConfigParam.DataMarkType).Value;
                            //rtnConfig.DataMarkChar = Convert.ToChar(HexStringToByteArray(subItem.Params.Find(m => m.Name == WeighterConfigParam.DataMarkChar).Value)[0]);
                            //rtnConfig.DataMark = HexStringToString(subItem.Params.Find(m => m.Name == WeighterConfigParam.DataMark).Value, System.Text.Encoding.ASCII);// subItem.Params.Find(m => m.Name == WeighterConfigParam.DataMark).Value;
                            //rtnConfig.Direction = subItem.Params.Find(m => m.Name == WeighterConfigParam.Direction).Value;
                            //rtnConfig.DataOrder = subItem.Params.Find(m => m.Name == WeighterConfigParam.DataOrder).Value;
                            //rtnConfig.CharLength = int.Parse(subItem.Params.Find(m => m.Name == WeighterConfigParam.CharLength).Value);
                            //rtnConfig.DataPostion = int.Parse(subItem.Params.Find(m => m.Name == WeighterConfigParam.DataPostion).Value);
                            //rtnConfig.DataLength = int.Parse(subItem.Params.Find(m => m.Name == WeighterConfigParam.DataLength).Value);
                            //rtnConfig.Multiple = int.Parse(subItem.Params.Find(m => m.Name == WeighterConfigParam.Multiple).Value);
                            //rtnConfig.VideoAlarm = int.Parse(subItem.Params.Find(m => m.Name == WeighterConfigParam.VideoAlarm).Value);
                            ////命令
                            //rtnConfig.ClearCmd = subItem.Params.Find(m => m.Name == WeighterConfigParam.ClearCmd).Value;
                            //rtnConfig.WeightCmd = subItem.Params.Find(m => m.Name == WeighterConfigParam.WeightCmd).Value;
                            ////间隔
                            //rtnConfig.InterVal = int.Parse(subItem.Params.Find(m => m.Name == WeighterConfigParam.InterVal).Value);
                        }
                    }
                    break;
                }
            }
            return rtnConfig;
        }

        /// <summary>
        /// 视频配置
        /// </summary>
        /// <returns></returns>
        public static VideoConfig ReadVideoConfig()
        {
            VideoConfig rtnConfig = new VideoConfig();
            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == VideoConfigParam.Model_Code_VideoConfig)
                        {
                            #region 视频设备设置
                            //IO设置
                            rtnConfig.VideoType = subItem.Params.Find(m => m.Name == VideoConfigParam.VideoType).Value;
                            rtnConfig.IONum = subItem.Params.Find(m => m.Name == VideoConfigParam.IONum).Value;
                            rtnConfig.DialogNum = subItem.Params.Find(m => m.Name == VideoConfigParam.DialogNum).Value;
                            rtnConfig.VideoDriver = subItem.Params.Find(m => m.Name == VideoConfigParam.VideoDriver).Value;

                            rtnConfig.CameraList = new List<Camera>();
                            Camera temp;
                            //加载摄像头配置
                            foreach (Row device in subItem.GridRow.RowList)
                            {
                                bool isused = device.Params.Find(m => m.Name == IoConfigParam.Row_IsUse).Value == YesNo.Yes ? true : false;
                                if (isused)
                                {
                                    temp = new Camera();
                                    temp.VideoName = device.Params.Find(m => m.Name == VideoConfigParam.Row_VideoName).Value;
                                    temp.Control = device.Params.Find(m => m.Name == VideoConfigParam.Row_Control).Value;
                                    temp.Ip = device.Params.Find(m => m.Name == VideoConfigParam.Row_Ip).Value;
                                    temp.Port = device.Params.Find(m => m.Name == VideoConfigParam.Row_Port).Value;
                                    temp.UserName = device.Params.Find(m => m.Name == VideoConfigParam.Row_UserName).Value;
                                    temp.PassWord = device.Params.Find(m => m.Name == VideoConfigParam.Row_PassWord).Value;
                                    temp.Channel = device.Params.Find(m => m.Name == VideoConfigParam.Row_Channel).Value;
                                    temp.Dialog = device.Params.Find(m => m.Name == VideoConfigParam.Row_Dialog).Value;
                                    temp.Position = device.Params.Find(m => m.Name == VideoConfigParam.Row_Position).Value;
                                    try
                                    {
                                        temp.DvrChannel = device.Params.Find(m => m.Name == VideoConfigParam.Row_DvrChannel).Value;
                                    }
                                    catch
                                    { }
                                    rtnConfig.CameraList.Add(temp);
                                }
                            }
                            #endregion
                            break;
                        }
                    }
                    break;
                }
            }
            return rtnConfig;
        }

        /// <summary>
        /// 读取键盘相关配置项
        /// </summary>
        /// <returns></returns>
        public static List<KeyboardConfig> ReadKeyboardConfig()
        {
            List<KeyboardConfig> rtnConfigList = new List<KeyboardConfig>();
            KeyboardConfig tempHostConfig = new KeyboardConfig();//主机
            KeyboardConfig tempAuxiliaryConfig = new KeyboardConfig();//辅机

            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == KeyboardConfigParam.Model_Code_KeyboardConfig)
                        {
                            tempHostConfig.Baudrate = int.Parse(subItem.Params.Find(m => m.Name == KeyboardConfigParam.HostBaudrate).Value.ToLower().Replace("bps", ""));
                            tempHostConfig.ComPort = subItem.Params.Find(m => m.Name == KeyboardConfigParam.HostComport).Value;
                            tempHostConfig.IsUse = subItem.Params.Find(m => m.Name == KeyboardConfigParam.HostIsUse).Value;

                            tempAuxiliaryConfig.Baudrate = int.Parse(subItem.Params.Find(m => m.Name == KeyboardConfigParam.AuxiliaryBaudrate).Value.ToLower().Replace("bps", ""));
                            tempAuxiliaryConfig.ComPort = subItem.Params.Find(m => m.Name == KeyboardConfigParam.AuxiliaryComport).Value;
                            tempAuxiliaryConfig.IsUse = subItem.Params.Find(m => m.Name == KeyboardConfigParam.AuxiliaryIsUse).Value;

                            //tempHostConfig.KeyOk = tempAuxiliaryConfig.KeyOk = subItem.Params.Find(m => m.Name == KeyboardConfigParam.KeyOk).Value;
                            //tempHostConfig.KeyHelp = tempAuxiliaryConfig.KeyHelp = subItem.Params.Find(m => m.Name == KeyboardConfigParam.KeyHelp).Value;
                            //tempHostConfig.KeyDelete = tempAuxiliaryConfig.KeyDelete = subItem.Params.Find(m => m.Name == KeyboardConfigParam.KeyDelete).Value;
                            //tempHostConfig.KeyCancel = tempAuxiliaryConfig.KeyCancel = subItem.Params.Find(m => m.Name == KeyboardConfigParam.KeyCancel).Value;
                            //tempHostConfig.KeyClear = tempAuxiliaryConfig.KeyClear = subItem.Params.Find(m => m.Name == KeyboardConfigParam.KeyClear).Value;

                            KeyDefine temp;
                            //加载摄像头配置
                            foreach (Row keyItem in subItem.GridRow.RowList)
                            {
                                temp = new KeyDefine();
                                temp.KeyCode = keyItem.Params.Find(m => m.Name == KeyboardConfigParam.Row_KeyCode).Value;
                                temp.KeyName = keyItem.Params.Find(m => m.Name == KeyboardConfigParam.Row_KeyName).Value;
                                temp.KeyValue = keyItem.Params.Find(m => m.Name == KeyboardConfigParam.Row_KeyValue).Value;
                                temp.AvailableIn = keyItem.Params.Find(m => m.Name == KeyboardConfigParam.Row_AvailableIn).Value.Trim() == KeyUseRange.Metering ? KeyUseRange.Metering : KeyUseRange.NoRange;
                                if (temp.KeyCode == KeyboardConfigParam.KeyOk)
                                {
                                    tempHostConfig.KeyOk = tempAuxiliaryConfig.KeyOk = temp;
                                }
                                else if (temp.KeyCode == KeyboardConfigParam.KeyHelp)
                                {
                                    tempHostConfig.KeyHelp = tempAuxiliaryConfig.KeyHelp = temp;
                                }
                                else if (temp.KeyCode == KeyboardConfigParam.KeyDelete)
                                {
                                    tempHostConfig.KeyDelete = tempAuxiliaryConfig.KeyDelete = temp;
                                }
                                else if (temp.KeyCode == KeyboardConfigParam.KeyCancel)
                                {
                                    tempHostConfig.KeyCancel = tempAuxiliaryConfig.KeyCancel = temp;
                                }
                                else if (temp.KeyCode == KeyboardConfigParam.KeyClear)
                                {
                                    tempHostConfig.KeyClear = tempAuxiliaryConfig.KeyClear = temp;
                                }
                            }

                            rtnConfigList.Add(tempHostConfig);
                            rtnConfigList.Add(tempAuxiliaryConfig);
                            break;
                        }
                    }
                    break;
                }
            }
            return rtnConfigList;
        }

        /// <summary>
        /// 读取对讲相关配置项
        /// </summary>
        /// <returns></returns>
        public static AudioConfig ReadAudioConfig()
        {
            AudioConfig rtnCfg = new AudioConfig();

            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == AudioConfigParam.Model_Code_AudioConfig)
                        {
                            rtnCfg.Ip = subItem.Params.Find(m => m.Name == AudioConfigParam.Ip).Value;
                            rtnCfg.Port = subItem.Params.Find(m => m.Name == AudioConfigParam.Port).Value;
                            rtnCfg.PassWord = subItem.Params.Find(m => m.Name == AudioConfigParam.PassWord).Value;
                            rtnCfg.UserName = subItem.Params.Find(m => m.Name == AudioConfigParam.UserName).Value;
                            rtnCfg.IsUse = subItem.Params.Find(m => m.Name == AudioConfigParam.IsUse).Value == YesNo.Yes ? true : false;
                            break;
                        }
                    }
                    break;
                }
            }
            return rtnCfg;
        }


        #region 系统设置
        /// <summary>
        /// 读取照片相关设置
        /// </summary>
        /// <returns></returns>
        public static FtpConfig ReadPhotoConfig()
        {
            FtpConfig rtnConfig = new FtpConfig();
            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_SystemConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == FtpConfigParam.Model_Code_FtpConfig)
                        {
                            //rtnConfig.LogSavePath = subItem.Params.Find(m => m.Name == FtpConfigParam.LogSavePath).Value;
                            rtnConfig.LogSavePath = AppDomain.CurrentDomain.BaseDirectory + "\\Log";
                            rtnConfig.PictureSavePath = subItem.Params.Find(m => m.Name == FtpConfigParam.PictureSavePath).Value;
                            rtnConfig.FtpIp = subItem.Params.Find(m => m.Name == FtpConfigParam.FtpIp).Value;
                            rtnConfig.FtpPort = subItem.Params.Find(m => m.Name == FtpConfigParam.FtpPort).Value;
                            rtnConfig.FtpUserName = subItem.Params.Find(m => m.Name == FtpConfigParam.FtpUserName).Value;
                            rtnConfig.FtpPassWord = subItem.Params.Find(m => m.Name == FtpConfigParam.FtpPassWord).Value;
                            break;
                        }
                    }
                    break;
                }
            }
            return rtnConfig;
        }
        #endregion

        /// <summary>
        /// 读取打印机配置
        /// </summary>
        /// <returns></returns>
        public static List<PrinterConfig> ReadPrinterConfig()
        {
            List<PrinterConfig> rtnList = new List<PrinterConfig>();
            PrinterConfig tempPrinterConfig;
            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == PrinterConfigParam.Model_Code_PrinterConfig)
                        {
                            //加载打印机
                            foreach (Row device in subItem.GridRow.RowList)
                            {
                                bool isused = device.Params.Find(m => m.Name == PrinterConfigParam.Row_IsUse).Value == YesNo.Yes ? true : false;
                                if (isused)
                                {
                                    tempPrinterConfig = new PrinterConfig();
                                    tempPrinterConfig.PrinterName = device.Params.Find(m => m.Name == PrinterConfigParam.Row_PrinterName).Value;
                                    tempPrinterConfig.ComPort = device.Params.Find(m => m.Name == PrinterConfigParam.Row_Comport).Value;
                                    tempPrinterConfig.Baudrate = int.Parse(device.Params.Find(m => m.Name == PrinterConfigParam.Row_Baudrate).Value.ToLower().Replace("bps", ""));

                                    tempPrinterConfig.PageMaxCount = 1;// int.Parse(device.Params.Find(m => m.Name == PrinterConfigParam.Row_PageMaxCount).Value);
                                    tempPrinterConfig.Notch = false;// device.Params.Find(m => m.Name == PrinterConfigParam.Row_Notch).Value == YesNo.Yes ? true : false;
                                    tempPrinterConfig.IsUse = device.Params.Find(m => m.Name == PrinterConfigParam.Row_IsUse).Value == YesNo.Yes ? true : false;
                                    tempPrinterConfig.Driver = device.Params.Find(m => m.Name == PrinterConfigParam.Row_Driver).Value;

                                    rtnList.Add(tempPrinterConfig);
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return rtnList;
        }

        /// <summary>
        /// 防作弊配置
        /// </summary>
        /// <returns></returns>
        public static CheatApparatus ReadCheatApparatusCfg()
        {
            CheatApparatus rtnConfig = new CheatApparatus();
            XmlSerializer configSer = new XmlSerializer(typeof(configlist));
            StreamReader sr = new StreamReader(File.OpenRead(_file));
            configlist conList = (configlist)configSer.Deserialize(sr);

            foreach (var item in conList.Modules)
            {
                if (item.Code == IoConfigParam.Model_Code_ExtrnEquConfigs)
                {
                    foreach (var subItem in item.SubModules)
                    {
                        if (subItem.Code == CheatConfigParam.Model_Code_CheatConfig)
                        {
                            //设备驱动相关
                            //string temp = subItem.Params.Find(m => m.Name == CheatConfigParam.DeviceName).Value;
                            //string[] data = temp.Split(new Char[] { '@' });
                            //if (data.Length == 2)
                            //{
                            //    rtnConfig.DeviceName = data[0];
                            //    rtnConfig.DriverName = data[1];
                            //}
                            //串口相关
                            rtnConfig.ComPort = subItem.Params.Find(m => m.Name == CheatConfigParam.Comport).Value;
                            rtnConfig.Baudrate = subItem.Params.Find(m => m.Name == CheatConfigParam.Baudrate).Value.ToLower().Replace("bps", "");
                            rtnConfig.StopSize = subItem.Params.Find(m => m.Name == CheatConfigParam.Stopsize).Value;
                            rtnConfig.Parity = subItem.Params.Find(m => m.Name == CheatConfigParam.Parity).Value;
                            rtnConfig.ByteSize = int.Parse(subItem.Params.Find(m => m.Name == CheatConfigParam.ByteSize).Value);
                            rtnConfig.IsUse = subItem.Params.Find(m => m.Name == CheatConfigParam.IsUse).Value=="是"?true:false;

                        }
                    }
                    break;
                }
            }
            return rtnConfig;
        }

        #region 十六进制转换
        private static string StringToHexString(string s, Encoding encode)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                result += Convert.ToString(b[i], 16);
            }
            return result;
        }
        private static string HexStringToString(string hs, Encoding encode)
        {
            //以%分割字符串，并去掉空字符
            char[] chars = hs.ToCharArray();
            byte[] b = new byte[chars.Length];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < chars.Length; i++)
            {
                b[i] = Convert.ToByte(chars[i]);
            }
            //按照指定编码将字节数组变为字符串
            return encode.GetString(b);
        }



        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] HexStringToByteArray(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        #endregion
    }
    #region IO配置
    public class IOconfig
    {
        private string _usePassCarType;

        /// <summary>
        /// 检测设备
        /// </summary>
        public string UsePassCarType
        {
            get { return _usePassCarType; }
            set { _usePassCarType = value; }
        }

        private string _conType;
        /// <summary>
        /// 连接方式
        /// </summary>
        public string ConType
        {
            get { return _conType; }
            set { _conType = value; }
        }

        private string _equDll;

        /// <summary>
        /// 设备驱动 dll
        /// </summary>
        public string EquDll
        {
            get { return _equDll; }
            set { _equDll = value; }
        }
        private string _equLoginName;

        /// <summary>
        /// 用户名
        /// </summary>
        public string EquLoginName
        {
            get { return _equLoginName; }
            set { _equLoginName = value; }
        }
        private string _equLoginPwd;

        /// <summary>
        /// 密码
        /// </summary>
        public string EquLoginPwd
        {
            get { return _equLoginPwd; }
            set { _equLoginPwd = value; }
        }

        private string _comport;

        /// <summary>
        /// 串口
        /// </summary>
        public string Comport
        {
            get { return _comport; }
            set { _comport = value; }
        }

        private string _ip;
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        private string _port;

        /// <summary>
        /// IP上使用的端口
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private string _baudrate;
        /// <summary>
        /// 串口波特率
        /// </summary>
        public string Baudrate
        {
            get { return _baudrate; }
            set { _baudrate = value; }
        }

        private List<Device> _outDeviceList;
        /// <summary>
        /// 输出设备
        /// </summary>
        public List<Device> OutDeviceList
        {
            get { return _outDeviceList; }
            set { _outDeviceList = value; }
        }

        private List<Device> _inDeviceList;
        /// <summary>
        /// 输入设备
        /// </summary>
        public List<Device> InDeviceList
        {
            get { return _inDeviceList; }
            set { _inDeviceList = value; }
        }

        //private Device _leftInfrared;
        ///// <summary>
        ///// 左红外
        ///// </summary>
        //public Device LeftInfrared
        //{
        //    get { return _leftInfrared; }
        //    set { _leftInfrared = value; }
        //}
        //private Device _rightInfrared;
        ///// <summary>
        ///// 右红外
        ///// </summary>
        //public Device RightInfrared
        //{
        //    get { return _rightInfrared; }
        //    set { _rightInfrared = value; }
        //}
    }

    /// <summary>
    /// IO控制器上连接的设备。
    /// 红绿灯，红外，照明灯
    /// </summary>
    public class Device
    {
        public Device()
        {
            _receiveSignalTime = DateTime.MinValue;
        }

        private string _code;
        /// <summary>
        /// 设备编码
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _port;
        /// <summary>
        /// 设备端口端口
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private string _portType;
        /// <summary>
        /// 设备端口类型，输入 或 输出
        /// </summary>
        public string PortType
        {
            get { return _portType; }
            set { _portType = value; }
        }

        private string _equTypeCode;

        /// <summary>
        /// 设备类型编码
        /// </summary>
        public string EquTypeCode
        {
            get { return _equTypeCode; }
            set { _equTypeCode = value; }
        }
        private string _type;

        /// <summary>
        /// 设备编码类型
        /// </summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _equName;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquName
        {
            get { return _equName; }
            set { _equName = value; }
        }

        private DateTime _receiveSignalTime;

        /// <summary>
        /// 接收到最新报警的事件，默认为当前时间
        /// </summary>
        public DateTime ReceiveSignalTime
        {
            get { return _receiveSignalTime; }
            set { _receiveSignalTime = value; }
        }
    }
    #endregion

    #region IC卡

    public class ICCard
    {
        private string _conType;

        /// <summary>
        /// 连接方式 网口 串口
        /// </summary>
        public string ConType
        {
            get { return _conType; }
            set { _conType = value; }
        }

        private string _comPort;

        /// <summary>
        /// com口
        /// </summary>
        public string ComPort
        {
            get { return _comPort; }
            set { _comPort = value; }
        }

        private string _baudrate;
        /// <summary>
        /// 串口波特率
        /// </summary>
        public string Baudrate
        {
            get { return _baudrate; }
            set { _baudrate = value; }
        }
        private string _ip;
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        private string _port;

        /// <summary>
        /// IP上使用的端口
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private string _interval;

        /// <summary>
        /// 寻卡时间，以毫秒为单位
        /// </summary>
        public string Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        private string _iCReadType;
        /// <summary>
        /// 读卡器类型
        /// </summary>
        public string ICReadType
        {
            get { return _iCReadType; }
            set { _iCReadType = value; }
        }

        private string driver;
        /// <summary>
        /// 驱动
        /// </summary>
        public string Driver
        {
            get { return driver; }
            set { driver = value; }
        }


        private bool _iCWriteTemp;
        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool ICWriteTemp
        {
            get { return _iCWriteTemp; }
            set { _iCWriteTemp = value; }
        }

        private bool _isUse;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse
        {
            get { return _isUse; }
            set { _isUse = value; }
        }
    }
    #endregion

    #region RFID
    public class RfidCfg
    {
        private string _usePassCarType;

        /// <summary>
        /// 检测设备
        /// </summary>
        public string UsePassCarType
        {
            get { return _usePassCarType; }
            set { _usePassCarType = value; }
        }


        private string _equDll;

        /// <summary>
        /// 设备驱动 dll
        /// </summary>
        public string EquDll
        {
            get { return _equDll; }
            set { _equDll = value; }
        }

        private string _conType;

        /// <summary>
        /// 连接方式 网口 串口
        /// </summary>
        public string ConType
        {
            get { return _conType; }
            set { _conType = value; }
        }

        private string _comPort;

        /// <summary>
        /// com口
        /// </summary>
        public string ComPort
        {
            get { return _comPort; }
            set { _comPort = value; }
        }

        private string _baudrate;
        /// <summary>
        /// 串口波特率
        /// </summary>
        public string Baudrate
        {
            get { return _baudrate; }
            set { _baudrate = value; }
        }
        private string _ip;
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        private string _port;

        /// <summary>
        /// IP上使用的端口
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private int _interval;

        /// <summary>
        /// 寻卡时间，以毫秒为单位
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }


        private bool _isUse;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse
        {
            get { return _isUse; }
            set { _isUse = value; }
        }

        private int _power;
        /// <summary>
        /// 功率
        /// </summary>
        public int Power
        {
            get { return _power; }
            set { _power = value; }
        }
        private List<AntennaPort> _antennaPortList;

        /// <summary>
        /// 天线列表
        /// </summary>
        public List<AntennaPort> AntennaPortList
        {
            get { return _antennaPortList; }
            set { _antennaPortList = value; }
        }
    }

    public class AntennaPort
    {
        private string _antennaName;
        /// <summary>
        /// 天线名称
        /// </summary>
        public string AntennaName
        {
            get { return _antennaName; }
            set { _antennaName = value; }
        }

        private string _port;

        /// <summary>
        /// 天线的端口
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private bool _isUse;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse
        {
            get { return _isUse; }
            set { _isUse = value; }
        }
    }
    #endregion

    #region 衡器
    /// <summary>
    /// 衡器配置
    /// </summary>
    public class WeighingApparatus
    {
        #region 设备及驱动配置

        private string _deviceName;
        //设备名称
        public string DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }

        private string _driverName;

        /// <summary>
        /// 驱动名称
        /// </summary>
        public string DriverName
        {
            get { return _driverName; }
            set { _driverName = value; }
        }

        #endregion

        #region 串口相关配置
        private string _comport;

        /// <summary>
        /// 串口
        /// </summary>
        public string ComPort
        {
            get { return _comport; }
            set { _comport = value; }
        }

        private string _baudrate;
        /// <summary>
        /// 串口波特率
        /// </summary>
        public string Baudrate
        {
            get { return _baudrate; }
            set { _baudrate = value; }
        }

        private string _stopSize;
        /// <summary>
        /// 停止位
        /// </summary>
        public string StopSize
        {
            get { return _stopSize; }
            set { _stopSize = value; }
        }

        private string _parity;
        /// <summary>
        /// 奇偶校验
        /// </summary>
        public string Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        private int _byteSize;
        /// <summary>
        /// 数据位
        /// </summary>
        public int ByteSize
        {
            get { return _byteSize; }
            set { _byteSize = value; }
        }

        #endregion



        #region 数据解析配置 已取消
        //private string _dataMarkType;
        ///// <summary>
        ///// 标记类型
        ///// </summary>
        //public string DataMarkType
        //{
        //    get { return _dataMarkType; }
        //    set { _dataMarkType = value; }
        //}

        //private char _dataMarkChar;
        ///// <summary>
        ///// char标记
        ///// </summary>
        //public char DataMarkChar
        //{
        //    get { return _dataMarkChar; }
        //    set { _dataMarkChar = value; }
        //}

        //private string _dataMark;
        ///// <summary>
        ///// 字符标记
        ///// </summary>
        //public string DataMark
        //{
        //    get { return _dataMark; }
        //    set { _dataMark = value; }
        //}

        //private string _direction;
        ///// <summary>
        ///// 取数方向
        ///// </summary>
        //public string Direction
        //{
        //    get { return _direction; }
        //    set { _direction = value; }
        //}

        //private string _dataOrder;
        ///// <summary>
        ///// 反向取数
        ///// </summary>
        //public string DataOrder
        //{
        //    get { return _dataOrder; }
        //    set { _dataOrder = value; }
        //}

        //private int _charLength;
        ///// <summary>
        ///// 数据长度
        ///// </summary>
        //public int CharLength
        //{
        //    get { return _charLength; }
        //    set { _charLength = value; }
        //}
        //private int _dataPostion;
        ///// <summary>
        ///// 取数位置
        ///// </summary>
        //public int DataPostion
        //{
        //    get { return _dataPostion; }
        //    set { _dataPostion = value; }
        //}
        //private int _dataLength;
        ///// <summary>
        ///// 取数长度
        ///// </summary>
        //public int DataLength
        //{
        //    get { return _dataLength; }
        //    set { _dataLength = value; }
        //}

        //private int _multiple;
        ///// <summary>
        ///// 倍数
        ///// </summary>
        //public int Multiple
        //{
        //    get { return _multiple; }
        //    set { _multiple = value; }
        //}

        //private int _videoAlarm;
        ///// <summary>
        ///// 录像重量
        ///// </summary>
        //public int VideoAlarm
        //{
        //    get { return _videoAlarm; }
        //    set { _videoAlarm = value; }
        //}
        #endregion

        #region 命令配置 已取消
        //private string _clearCmd;
        ///// <summary>
        ///// 清零命令
        ///// </summary>
        //public string ClearCmd
        //{
        //    get { return _clearCmd; }
        //    set { _clearCmd = value; }
        //}

        //private string _weightCmd;
        ///// <summary>
        ///// 清零命令
        ///// </summary>
        //public string WeightCmd
        //{
        //    get { return _weightCmd; }
        //    set { _weightCmd = value; }
        //}
        #endregion

        #region 其他 已取消
        //private int _interVal;
        ///// <summary>
        ///// 发送间隔
        ///// </summary>
        //public int InterVal
        //{
        //    get { return _interVal; }
        //    set { _interVal = value; }
        //}
        #endregion

        #region 最大量程
        private int _maxWeight;
        /// <summary>
        /// 奇偶校验
        /// </summary>
        public int MaxWeight
        {
            get { return _maxWeight; }
            set { _maxWeight = value; }
        }
        #endregion
    }
    #endregion

    #region 键盘设置
    public class KeyboardConfig
    {
        private string _comport;

        /// <summary>
        /// 串口
        /// </summary>
        public string ComPort
        {
            get { return _comport; }
            set { _comport = value; }
        }

        private int _baudrate;
        /// <summary>
        /// 串口波特率
        /// </summary>
        public int Baudrate
        {
            get { return _baudrate; }
            set { _baudrate = value; }
        }
        private string _isUse;
        /// <summary>
        /// 是否启用
        /// </summary>
        public string IsUse
        {
            get { return _isUse; }
            set { _isUse = value; }
        }

        private KeyDefine _keyOk;
        /// <summary>
        /// 确定
        /// </summary>
        public KeyDefine KeyOk
        {
            get { return _keyOk; }
            set { _keyOk = value; }
        }

        private KeyDefine _keyHelp;
        /// <summary>
        /// 求助
        /// </summary>
        public KeyDefine KeyHelp
        {
            get { return _keyHelp; }
            set { _keyHelp = value; }
        }

        private KeyDefine _keyDelete;
        /// <summary>
        /// 删除
        /// </summary>
        public KeyDefine KeyDelete
        {
            get { return _keyDelete; }
            set { _keyDelete = value; }
        }

        private KeyDefine _keyClear;
        /// <summary>
        /// 清空
        /// </summary>
        public KeyDefine KeyClear
        {
            get { return _keyClear; }
            set { _keyClear = value; }
        }

        private KeyDefine _keyCancel;
        /// <summary>
        /// 取消
        /// </summary>
        public KeyDefine KeyCancel
        {
            get { return _keyCancel; }
            set { _keyCancel = value; }
        }

    }

    /// <summary>
    /// 键定义
    /// </summary>
    public class KeyDefine
    {
        public string KeyCode { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
        public string AvailableIn { get; set; }
    }
    #endregion

    #region 视频配置
    /// <summary>
    /// 视频配置
    /// </summary>
    public class VideoConfig
    {
        private string _videoType;

        /// <summary>
        /// 视频厂家
        /// </summary>
        public string VideoType
        {
            get { return _videoType; }
            set { _videoType = value; }
        }

        private string _dialogNum;

        /// <summary>
        /// 对讲设备
        /// </summary>
        public string DialogNum
        {
            get { return _dialogNum; }
            set { _dialogNum = value; }
        }

        private string _iONum;

        /// <summary>
        /// IO设备
        /// </summary>
        public string IONum
        {
            get { return _iONum; }
            set { _iONum = value; }
        }

        private string _videoDriver;

        /// <summary>
        /// 视频驱动
        /// </summary>
        public string VideoDriver
        {
            get { return _videoDriver; }
            set { _videoDriver = value; }
        }

        private List<Camera> _cameraList;

        /// <summary>
        /// 摄像头列表
        /// </summary>
        public List<Camera> CameraList
        {
            get { return _cameraList; }
            set { _cameraList = value; }
        }
    }

    /// <summary>
    /// 摄像头
    /// </summary>
    public class Camera
    {
        private string _isUse;
        /// <summary>
        /// 是否启用
        /// </summary>
        public string IsUse
        {
            get { return _isUse; }
            set { _isUse = value; }
        }

        private string _videoName;
        /// <summary>
        /// 摄像头名称
        /// </summary>
        public string VideoName
        {
            get { return _videoName; }
            set { _videoName = value; }
        }

        private string _position;
        /// <summary>
        /// 摄像头位置
        /// </summary>
        public string Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private string _control;
        /// <summary>
        /// 云台控制
        /// </summary>
        public string Control
        {
            get { return _control; }
            set { _control = value; }
        }

        private string _ip;
        /// <summary>
        /// IP
        /// </summary>
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }
        private string _port;
        /// <summary>
        /// 端口
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }
        private string _userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        private string _passWord;
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get { return _passWord; }
            set { _passWord = value; }
        }
        private string _channel;
        /// <summary>
        /// 通道号
        /// </summary>
        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }
        private string _dvrChannel;
        /// <summary>
        /// 录像回放通道号
        /// </summary>
        public string DvrChannel
        {
            get { return _dvrChannel; }
            set { _dvrChannel = value; }
        }
        private string _dialog;
        /// <summary>
        /// 远程对讲
        /// </summary>
        public string Dialog
        {
            get { return _dialog; }
            set { _dialog = value; }
        }
        private string _photograph;
        /// <summary>
        /// 是否拍照
        /// </summary>
        public string Photograph
        {
            get { return _photograph; }
            set { _photograph = value; }
        }
    }
    #endregion

    #region 音频配置
    /// <summary>
    /// 音频配置
    /// </summary>
    public class AudioConfig
    {
        private string _ip;
        /// <summary>
        /// IP
        /// </summary>
        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }
        private string _port;
        /// <summary>
        /// 端口
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }
        private string _userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        private string _passWord;
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get { return _passWord; }
            set { _passWord = value; }
        }

        private bool _isUse;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse
        {
            get { return _isUse; }
            set { _isUse = value; }
        }
    }
    #endregion

    #region 系统设置
    #region 照片设置
    public class FtpConfig
    {
        /// <summary>
        /// 日志保存路径
        /// </summary>
        public string LogSavePath { get; set; }
        /// <summary>
        /// 照片保存路径
        /// </summary>
        public string PictureSavePath { get; set; }
        /// <summary>
        /// ftp地址
        /// </summary>
        public string FtpIp { get; set; }
        /// <summary>
        /// ftp端口
        /// </summary>
        public string FtpPort { get; set; }
        /// <summary>
        /// ftp用户名
        /// </summary>
        public string FtpUserName { get; set; }
        /// <summary>
        /// ftp密码
        /// </summary>
        public string FtpPassWord { get; set; }

    }
    #endregion
    #endregion

    #region 打印机配置
    /// <summary>
    /// 打印机配置
    /// </summary>
    public class PrinterConfig
    {
        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrinterName { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public string ComPort { get; set; }
        /// <summary>
        ///波特率
        /// </summary>
        public int Baudrate { get; set; }
        /// <summary>
        /// 缺纸时最大票数
        /// </summary>
        public int PageMaxCount { get; set; }
        /// <summary>
        /// 启用黑标
        /// </summary>
        public bool Notch { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public string Driver { get; set; }
    }
    #endregion

    #region 防作弊
    /// <summary>
    /// 防作弊配置
    /// </summary>
    public class CheatApparatus
    {
        //#region 设备及驱动配置

        //private string _deviceName;
        ////设备名称
        //public string DeviceName
        //{
        //    get { return _deviceName; }
        //    set { _deviceName = value; }
        //}

        //private string _driverName;

        ///// <summary>
        ///// 驱动名称
        ///// </summary>
        //public string DriverName
        //{
        //    get { return _driverName; }
        //    set { _driverName = value; }
        //}

        //#endregion

        #region 串口相关配置
        private string _comport;

        /// <summary>
        /// 串口
        /// </summary>
        public string ComPort
        {
            get { return _comport; }
            set { _comport = value; }
        }

        private string _baudrate;
        /// <summary>
        /// 串口波特率
        /// </summary>
        public string Baudrate
        {
            get { return _baudrate; }
            set { _baudrate = value; }
        }

        private string _stopSize;
        /// <summary>
        /// 停止位
        /// </summary>
        public string StopSize
        {
            get { return _stopSize; }
            set { _stopSize = value; }
        }

        private string _parity;
        /// <summary>
        /// 奇偶校验
        /// </summary>
        public string Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        private int _byteSize;
        /// <summary>
        /// 数据位
        /// </summary>
        public int ByteSize
        {
            get { return _byteSize; }
            set { _byteSize = value; }
        }

        private bool _isUse;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse
        {
            get { return _isUse; }
            set { _isUse = value; }
        }

        #endregion
    }
    #endregion
}
