using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Talent.Measure.DomainModel;
using Talent.Video.Interface;
using Talent_LT.HelpClass;

namespace Talent.Video.HKVideo
{
    /// <summary>
    /// 摄像头控制类
    /// </summary>
    public class CameraControl : ICamera
    {
        CHCNetSDK.VOICEDATACALLBACKV30 m_voice;

        System.Threading.Timer _drawWeightTimer;
        /// <summary>
        /// 
        /// </summary>
        CHCNetSDK.NET_DVR_SHOWSTRING_V30 m_struShowStrCfg;
        CHCNetSDK.DRAWFUN DarwString;
        /// <summary>
        /// 视频上要叠加的字符串
        /// </summary>
        string _drawStr = string.Empty;
        /// <summary>
        /// 预览视频控件的句柄
        /// </summary>
        IntPtr? _controlHandel;
        /// <summary>
        /// 登录用户唯一编码
        /// </summary>
        private Int32 _userID = -1;
        /// <summary>
        /// sdk是否已经初始化
        /// </summary>
        private bool _initSDK = false;
        /// <summary>
        /// 播放控件句柄
        /// </summary>
        private Int32 _realHandle = -1;

        public string IP
        {
            get
            {
                return _cameraCfg.Ip;
            }
        }

        private Camera _cameraCfg;

        /// <summary>
        /// 照片相关配置
        /// </summary>
        FtpConfig _curPhotoConfig;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pCameraCfg">摄像头配置</param>
        /// <param name="pControlHandel">预览视频控件的句柄</param>
        /// <param name="pDrawStr">视频上要叠加的字符串</param>
        public CameraControl(FtpConfig pPhotoConfig, Camera pCameraCfg, IntPtr? pControlHandel)
        {
            _curPhotoConfig = pPhotoConfig;
            _cameraCfg = pCameraCfg;
            _controlHandel = pControlHandel;
            _drawStr = "";
            // _drawStr = "123456kg";
            DarwString = new CHCNetSDK.DRAWFUN(cbDrawFun);
            m_voice = new CHCNetSDK.VOICEDATACALLBACKV30(Voice);



        }
        /// <summary>
        /// 开始定时写入数据
        /// </summary>
        public void BeginStart()
        {
            TimerCallback callBack = new TimerCallback(ReadData);
            _drawWeightTimer = new System.Threading.Timer(callBack);
            _drawWeightTimer = new Timer(callBack, null, Timeout.Infinite, 1000);
            //_drawWeightTimer.Change(-1, -1);
            _drawWeightTimer.Change(1000, 1000);
        }
        Random rm = new Random();
        private void ReadData(object paramObject)
        {
            DrawString(rm.Next(1000).ToString());
        }
        /// <summary>
        /// 写数据
        /// </summary>
        public void WriteTxtToVideo(string pData)
        {
            try
            {
                Thread thsend1 = new Thread(new ParameterizedThreadStart(DrawString));
                thsend1.Start(pData);
            }
            catch
            {

            }
        }
        /// <summary>
        /// 写数据到视频界面
        /// </summary>
        /// <param name="pData"></param>
        public void DrawString(object pData)
        {
            try
            {
                _drawStr = pData.ToString();
                //System.Diagnostics.Debug.WriteLine(_drawStr);
                if (m_struShowStrCfg.struStringInfo != null)
                {
                    m_struShowStrCfg.struStringInfo[0].wShowString = 1;
                    //m_struShowStrCfg.struStringInfo[0].sString = _cameraCfg.VideoName + "  " + _drawStr;
                    m_struShowStrCfg.struStringInfo[0].sString = "重量:" + _drawStr;
                    m_struShowStrCfg.struStringInfo[0].wStringSize = (ushort)(2 * m_struShowStrCfg.struStringInfo[0].sString.Length);//System.Text.ASCIIEncoding.Default.GetByteCount(_drawStr);//.Length;
                    m_struShowStrCfg.struStringInfo[0].wShowStringTopLeftX = 520;
                    m_struShowStrCfg.struStringInfo[0].wShowStringTopLeftY = 520;

                    Int32 nSize1 = Marshal.SizeOf(m_struShowStrCfg);
                    IntPtr ptrShowStrCfg1 = Marshal.AllocHGlobal(nSize1);
                    Marshal.StructureToPtr(m_struShowStrCfg, ptrShowStrCfg1, false);

                    if (!CHCNetSDK.NET_DVR_SetDVRConfig(_userID, CHCNetSDK.NET_DVR_SET_SHOWSTRING_V30, int.Parse(_cameraCfg.Channel), ptrShowStrCfg1, (UInt32)nSize1))
                    {
                        //设置字符叠加参数失败，输出错误号 Failed to set overlay parameters and output the error code
                        // ShowMsg("字符叠加失败");
                    }
                    Marshal.FreeHGlobal(ptrShowStrCfg1);
                }
            }
            catch
            {

            }

        }

        /// <summary>
        /// 修改DVR系统时间
        /// </summary>
        public bool SyncDateTime()
        {
            bool rtn = false;
            CHCNetSDK.NET_DVR_TIME time = new CHCNetSDK.NET_DVR_TIME();
            UInt32 dwReturn = 0;
            Int32 nSize = Marshal.SizeOf(time);
            IntPtr ptrTimeCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(time, ptrTimeCfg, false);
            if (CHCNetSDK.NET_DVR_GetDVRConfig(_userID, CHCNetSDK.NET_DVR_GET_TIMECFG, int.Parse(_cameraCfg.Channel), ptrTimeCfg, (UInt32)nSize, ref dwReturn))
            {
                time = (CHCNetSDK.NET_DVR_TIME)Marshal.PtrToStructure(ptrTimeCfg, typeof(CHCNetSDK.NET_DVR_TIME));
                Marshal.FreeHGlobal(ptrTimeCfg);
                time.dwYear = (uint)DateTime.Now.Year;
                time.dwMonth = (uint)DateTime.Now.Month;
                time.dwDay = (uint)DateTime.Now.Day;
                time.dwHour = (uint)DateTime.Now.Hour;
                time.dwMinute = (uint)DateTime.Now.Minute;
                time.dwSecond = (uint)DateTime.Now.Second;

                Int32 nSize1 = Marshal.SizeOf(time);
                IntPtr ptrShowStrCfg1 = Marshal.AllocHGlobal(nSize1);
                Marshal.StructureToPtr(time, ptrShowStrCfg1, false);
                if (CHCNetSDK.NET_DVR_SetDVRConfig(_userID, CHCNetSDK.NET_DVR_SET_TIMECFG, int.Parse(_cameraCfg.Channel), ptrShowStrCfg1, (UInt32)nSize1))
                {
                    rtn = true;
                }
                Marshal.FreeHGlobal(ptrShowStrCfg1);
            }

            return rtn;
        }

        /// <summary>
        /// 获取字符叠加配置
        /// </summary>
        private void GetConfig()
        {
            UInt32 dwReturn = 0;
            Int32 nSize = Marshal.SizeOf(m_struShowStrCfg);
            IntPtr ptrShowStrCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(m_struShowStrCfg, ptrShowStrCfg, false);
            if (!CHCNetSDK.NET_DVR_GetDVRConfig(_userID, CHCNetSDK.NET_DVR_GET_SHOWSTRING_V30, int.Parse(_cameraCfg.Channel), ptrShowStrCfg, (UInt32)nSize, ref dwReturn))
            {
            }
            else
            {
                m_struShowStrCfg = (CHCNetSDK.NET_DVR_SHOWSTRING_V30)Marshal.PtrToStructure(ptrShowStrCfg, typeof(CHCNetSDK.NET_DVR_SHOWSTRING_V30));
            }
            Marshal.FreeHGlobal(ptrShowStrCfg);

            //

        }
        /// <summary>
        /// 初始化摄像头
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            bool rtn = CHCNetSDK.NET_DVR_Init();
            if (false == rtn)
            {
                ShowMsg("NET_DVR_Init() 初始化失败。");
            }
            return rtn;
        }
        /// <summary>
        /// 初始化摄像头
        /// </summary>
        /// <returns></returns>
        public bool OpenAll()
        {
            return Open();
        }
        private void Voice(int lVoiceComHandle, string pRecvDataBuffer, uint dwBufSize, byte byAudioFlag, System.IntPtr pUser)
        {
        }

        /// <summary>
        /// 启动摄像头
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            bool rtn = false;
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
            //登录设备 Login the device
            _userID = CHCNetSDK.NET_DVR_Login_V30(_cameraCfg.Ip, int.Parse(_cameraCfg.Port), _cameraCfg.UserName, _cameraCfg.PassWord, ref DeviceInfo);
            //登录成功
            if (_userID >= 0)
            {
                IntPtr user = IntPtr.Zero;
                //int voiceHandel=CHCNetSDK.NET_DVR_StartVoiceCom_V30(_userID, 1, false, m_voice, user);
                GetConfig();
                if (_controlHandel != null && _realHandle < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo.hPlayWnd = _controlHandel.Value;//预览窗口
                    lpPreviewInfo.lChannel = 1; //int.Parse(_cameraCfg.Channel);//预te览的设备通道
                    lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo.dwDisplayBufNum = 15; //播放库显示缓冲区最大帧数
                    CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    IntPtr pUser = new IntPtr();//用户数据
                    //打开预览 Start live view 
                    _realHandle = CHCNetSDK.NET_DVR_RealPlay_V40(_userID, ref lpPreviewInfo, null/*RealData*/, pUser);
                    //预览成功
                    if (_realHandle >= 0)
                    {
                        // GetConfig();                       
                        // 写叠加字符成功
                        //if (CHCNetSDK.NET_DVR_RigisterDrawFun(_realHandle, DarwString, 0))
                        //{
                        //   // _testTimer.Change(0, 1000);
                        //}
                        //else
                        //{
                        //    // ShowMsg("视频叠加字符失败。");
                        //}
                    }
                    else//预览失败
                    {
                        ShowMsg("预览失败 返回_realHandle值：" + _realHandle);
                    }
                    //_drawWeightTimer.Change(0, 2000);
                }
            }
            else
            {
                ShowMsg("登录失败:" + " IP :" + _cameraCfg.Ip + " 端口:" + _cameraCfg.Port + " 用户名:" + _cameraCfg.UserName + " 密码:" + _cameraCfg.PassWord + "  返回_userID：" + _userID);
                try
                {
                    if (!CHCNetSDK.NET_DVR_Logout(_userID))
                    {
                    }
                }
                catch //(Exception ex)
                { 
                
                } 
                CHCNetSDK.NET_DVR_Cleanup();
                _userID = -1;
            }
            return rtn;
        }


        /// <summary>
        /// 叠加字符到视频
        /// </summary>
        /// <param name="lRealHandle"></param>
        /// <param name="hDC"></param>
        /// <param name="dwUser"></param>
        private void cbDrawFun(int lRealHandle, IntPtr hDC, uint dwUser)
        {
            ReadData(11);
            //写入字符不存在则退出
            if (_drawStr.Trim().Length <= 0) return;

            //Point[] pa = new Point[5];
            //pa[0].X = 50;
            //pa[0].Y = 50;
            //pa[1].X = 50;
            //pa[1].Y = 150;
            //pa[2].X = 150;
            //pa[2].Y = 150;
            //pa[3].X = 150;
            //pa[3].Y = 50;
            //pa[4].X = 50;
            //pa[4].Y = 50;

            //Rectangle rect = new Rectangle(50, 50, 100, 100);
            Point p = new Point(10, 40);

            Font font = new Font("黑体", 20, FontStyle.Bold);

            System.Drawing.Graphics pDC = System.Drawing.Graphics.FromHdc(hDC);
            Brush hBrush = new SolidBrush(Color.White); ;
            // Pen pen = new Pen(hBrush);
            //pDC.DrawLines(pen, pa);

            pDC.DrawString(_cameraCfg.VideoName + "_" + _drawStr, font, hBrush, p);


            // System.Diagnostics.Debug.WriteLine(DateTime.Now);
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
        }
        /// <summary>
        /// 停止视屏录像
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            bool rtn = false;
            //if (!CapturePicture(@"D:\\123.jpg"))
            //{
            //    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
            //    ShowMsg("视频抓图失败。错误码=" + iLastErr.ToString());
            //}
            try
            {
                _drawWeightTimer.Change(-1, -1);
            }
            catch //(Exception ex)
            {

            } 
            if (_realHandle >= 0)
            {
                bool stopB =  CHCNetSDK.NET_DVR_StopRealPlay(_realHandle);
                //FileHelpClass.WriteLog("摄像头 NET_DVR_StopRealPlay 返回：" + stopB, "DVR");
                _realHandle = -1;
            }
            if (_userID >= 0)
            {
               bool logOutB= CHCNetSDK.NET_DVR_Logout(_userID);
               //FileHelpClass.WriteLog("摄像头 NET_DVR_Logout 返回：" + logOutB,"DVR");
                _userID = -1;
            }

            return rtn;
        }

        /// <summary>
        /// 关闭摄像头
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            bool rtn = false;
            Stop();
            //FileHelpClass.WriteLog("摄像头 stop 返回："+stopB);
            //if (_initSDK == true)
            {
                rtn = CHCNetSDK.NET_DVR_Cleanup();
                //FileHelpClass.WriteLog("摄像头 NET_DVR_Cleanup 返回：" + rtn);
                _initSDK = false;
                System.GC.Collect();
            }
            try
            {
                _drawWeightTimer.Change(-1, -1);
            }
            catch //(Exception ex)
            {

            }
            return rtn;
        }

        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="pBusiness">物流ID</param>
        /// <param name="pBusiness">业务类型</param>
        /// <returns></returns>
        public bool CapturePicture(string pLogisticsId, string pBusinessType)
        {
            CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
            lpJpegPara.wPicQuality = 0; //图像质量 Image quality
            lpJpegPara.wPicSize = 0xff; //抓图分辨率 Picture size: 2- 4CIF，0xff- Auto(使用当前码流分辨率)，抓图分辨率需要设备支持，更多取值请参考SDK文档

            string tFileName = string.Format("{0}_{1}_{2}_{3}.jpg", pLogisticsId, pBusinessType, _cameraCfg.Position, DateTime.Now.ToString("HHmmss"));
            //return CHCNetSDK.NET_DVR_CaptureJPEGPicture(_userID, int.Parse(_cameraCfg.Channel), ref lpJpegPara, _curPhotoConfig.PictureSavePath + tFileName);
            return CHCNetSDK.NET_DVR_CaptureJPEGPicture(_userID, 1, ref lpJpegPara, _curPhotoConfig.PictureSavePath + tFileName);

        }

        /// <summary>
        /// 弹出错误消息
        /// </summary>
        public event Action<string> OnShowErrMsg;

        /// <summary>
        /// 消息回调
        /// </summary>
        /// <param name="pMsg"></param>
        private void ShowMsg(string pMsg)
        {
            if (OnShowErrMsg != null)
            {
                OnShowErrMsg(pMsg);
            }
        }
    }
}
