using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Talent.ClinetLog;
//using System.Timers;
using Talent.Measure.DomainModel;
using Talent.Video.Interface;

namespace Talent.Video.HKVideo
{
    /// <summary>
    /// 摄像头控制类
    /// </summary>
    public class CameraPlayBackControl : ICameraPlayBack
    {
       
        /// <summary>
        /// 回放定时器
        /// </summary>
       Timer _playbackTimer;
    
        
        /// <summary>
        /// 回放视频控件的句柄
        /// </summary>
        IntPtr _controlHandel;
        /// <summary>
        /// 登录用户唯一编码
        /// </summary>
        private Int32 _userID = -1;
        /// <summary>
        /// sdk是否已经初始化
        /// </summary>
        private bool _initSDK = false;

        /// <summary>
        /// 摄像头配置
        /// </summary>
        private Camera _cameraCfg;

        /// <summary>
        /// 回放句柄
        /// </summary>
        private Int32 _playBackHandel = -1;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pCameraCfg">摄像头配置</param>
        /// <param name="pControlHandel">预览视频控件的句柄</param>
        public CameraPlayBackControl(Camera pCameraCfg, IntPtr pControlHandel)
        {
            VideoPlayBackLogger.Debug("创建回放对象.", pCameraCfg.Ip);
            _cameraCfg = pCameraCfg;
            _controlHandel = pControlHandel;
            _playbackTimer = new Timer();
            _playbackTimer.Interval = 1000;
           // _playbackTimer.Elapsed += new ElapsedEventHandler(_playbackTimer_Elapsed);
            _playbackTimer.Tick += new EventHandler(_playbackTimer_Elapsed);
            
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _playbackTimer_Elapsed(object sender, EventArgs e)
        {
            //ShowMsg("timer");
            uint iOutValue = 0;
            int iPos = 0;

            IntPtr lpOutBuffer = Marshal.AllocHGlobal(4);

            //获取回放进度
            CHCNetSDK.NET_DVR_PlayBackControl_V40(_playBackHandel, CHCNetSDK.NET_DVR_PLAYGETPOS, IntPtr.Zero, 0, lpOutBuffer, ref iOutValue);

            iPos = (int)Marshal.PtrToStructure(lpOutBuffer, typeof(int));
            //ShowMsg(iPos.ToString());
            if (iPos == 100)  //回放结束
            {
                if (!CHCNetSDK.NET_DVR_StopPlayBack(_playBackHandel))
                {
                    return;
                }
                //_playBackHandel = -1;
                _playbackTimer.Stop();
                VideoPlayBackLogger.Debug("视频回放结束", _cameraCfg.Channel);
            }

            if (iPos == 200) //网络异常，回放失败
            {
                _playbackTimer.Stop();
                ShowMsg("网络异常，回放失败");
                VideoPlayBackLogger.Error("网络异常，回放失败", _cameraCfg.Channel);
            }
            Marshal.FreeHGlobal(lpOutBuffer);
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

        /// <summary>
        /// 设置播放录像的时间段
        /// </summary>
        /// <param name="pStartTime">录像开始时间</param>
        /// <param name="pEndTime">录像结束时间</param>
        /// <returns></returns>
        public bool PlayByTime(DateTime pStartTime, DateTime pEndTime)
        {
            bool rtn = false;
            if (!_initSDK)
            {
                _initSDK = CHCNetSDK.NET_DVR_Init();
                VideoPlayBackLogger.Debug("按时间回放时，初始化设备。", _cameraCfg.Channel);
            }
            else
            {
                if (_playBackHandel >= 0)
                {
                    CHCNetSDK.NET_DVR_StopPlayBack(_playBackHandel);
                    VideoPlayBackLogger.Debug("按时间回放时，初始化设备。", _cameraCfg.Channel);
                    _playBackHandel = -1;
                }

                if (_userID >= 0)
                {
                    CHCNetSDK.NET_DVR_Logout(_userID);
                    VideoPlayBackLogger.Debug("按时间回放时，强制退出当前用户。", _cameraCfg.Channel);
                    _userID = -1;
                }
            }

            if (_initSDK)
            {
                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
                //登录设备 
                VideoPlayBackLogger.Debug(string.Format("按时间回放时，登录设备。_cameraCfg.Ip={0},_cameraCfg.Port={1},_cameraCfg.UserName={2},_cameraCfg.PassWord={3}",
                   _cameraCfg.Ip, _cameraCfg.Port, _cameraCfg.UserName, _cameraCfg.PassWord), _cameraCfg.Channel);
                _userID = CHCNetSDK.NET_DVR_Login_V30(_cameraCfg.Ip, int.Parse(_cameraCfg.Port), _cameraCfg.UserName, _cameraCfg.PassWord, ref DeviceInfo);
               
                if (_userID >= 0)
                {
                    VideoPlayBackLogger.Debug("按时间回放时，登录成功。", _cameraCfg.Channel);
                   
                    CHCNetSDK.NET_DVR_VOD_PARA tPara = new CHCNetSDK.NET_DVR_VOD_PARA();
                    tPara.dwSize = (uint)Marshal.SizeOf(tPara);
                    tPara.struIDInfo.dwChannel = uint.Parse((int.Parse(_cameraCfg.Channel) + (int)DeviceInfo.byStartDChan).ToString()); //通道号 Channel number  
                    tPara.hWnd = _controlHandel;//回放窗口句柄
                    VideoPlayBackLogger.Debug("设置回放通道号 tPara.struIDInfo.dwChannel=" + tPara.struIDInfo.dwChannel, _cameraCfg.Channel);
                    //设置回放的开始时间 
                    tPara.struBeginTime.dwYear = (uint)pStartTime.Year;
                    tPara.struBeginTime.dwMonth = (uint)pStartTime.Month;
                    tPara.struBeginTime.dwDay = (uint)pStartTime.Day;
                    tPara.struBeginTime.dwHour = (uint)pStartTime.Hour;
                    tPara.struBeginTime.dwMinute = (uint)pStartTime.Minute;
                    tPara.struBeginTime.dwSecond = (uint)pStartTime.Second;
                    VideoPlayBackLogger.Debug(string.Format("设置回放开始时间dwYear={0},dwMonth={1},dwDay={2},dwHour={3},dwMinute={4},dwSecond={5}",
                        pStartTime.Year, pStartTime.Month, pStartTime.Day, pStartTime.Hour, pStartTime.Minute,pStartTime.Second), 
                        _cameraCfg.Channel);


                    //设置回放的结束时间 
                    tPara.struEndTime.dwYear = (uint)pEndTime.Year;
                    tPara.struEndTime.dwMonth = (uint)pEndTime.Month;
                    tPara.struEndTime.dwDay = (uint)pEndTime.Day;
                    tPara.struEndTime.dwHour = (uint)pEndTime.Hour;
                    tPara.struEndTime.dwMinute = (uint)pEndTime.Minute;
                    tPara.struEndTime.dwSecond = (uint)pEndTime.Second;
                    VideoPlayBackLogger.Debug(string.Format("设置回放结束时间dwYear={0},dwMonth={1},dwDay={2},dwHour={3},dwMinute={4},dwSecond={5}",
                       pEndTime.Year, pEndTime.Month, pEndTime.Day, pEndTime.Hour, pEndTime.Minute, pEndTime.Second),
                       _cameraCfg.Channel);
                    _playBackHandel = CHCNetSDK.NET_DVR_PlayBackByTime_V40(_userID, ref tPara);
                    if (_playBackHandel < 0)
                    {
                        uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        ShowMsg("回放初始化失败，错误编码=" + iLastErr);
                        VideoPlayBackLogger.Error("回放初始化失败，错误编码=" + iLastErr, _cameraCfg.Channel);
                    }
                    else
                    {
                        uint iOutValue = 0;
                        if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(_playBackHandel, CHCNetSDK.NET_DVR_PLAYSTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                        {
                            uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                            ShowMsg("启动回放失败，错误码：" + iLastErr);
                            VideoPlayBackLogger.Debug("启动回放失败，错误码：" + iLastErr, _cameraCfg.Channel);
                        }
                        else
                        {
                            _playbackTimer.Start();
                            VideoPlayBackLogger.Debug("开始按照时间段回放。", _cameraCfg.Channel);
                           //ShowMsg("启动定时器成功");
                            rtn = true;
                        }
                    }
                }
            }

            return rtn;
        }


        /// <summary>
        /// 继续播放录像
        /// </summary>
        /// <returns></returns>
        public bool PlayStart()
        {
            bool rtn = false;
            if (_initSDK && _userID >= 0 && _playBackHandel >= 0)
            {
                uint iOutValue = 0;
                if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(_playBackHandel, CHCNetSDK.NET_DVR_PLAYRESTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    ShowMsg("启动回放失败，错误码：" + iLastErr);
                    VideoPlayBackLogger.Error("启动回放失败，错误码：" + iLastErr, _cameraCfg.Channel);
                }
                else
                {
                    _playbackTimer.Start();
                    //ShowMsg("启动定时器成功");
                    VideoPlayBackLogger.Debug("启动回放成功。" , _cameraCfg.Channel);
                    rtn = true;
                    
                }
            }
            return rtn;
        }


        /// <summary>
        /// 加快播放速度
        /// </summary>
        /// <returns></returns>
        public bool PlayFast()
        {
            bool rtn = false;
            if (_initSDK && _userID >= 0 && _playBackHandel >= 0)
            {
                uint iOutValue = 0;
                if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(_playBackHandel, CHCNetSDK.NET_DVR_PLAYFAST, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    ShowMsg("加快回放速度失败，错误码：" + iLastErr);
                    VideoPlayBackLogger.Error("加快回放速度失败，错误码：" + iLastErr, _cameraCfg.Channel);
                }
                else
                {
                    rtn = true;
                    VideoPlayBackLogger.Debug("加快回放速度失败。", _cameraCfg.Channel);
                }
            }
            return rtn;
        }

        /// <summary>
        /// 降低播放速度
        /// </summary>
        /// <returns></returns>
        public bool PlaySlow()
        {
            bool rtn = false;
            if (_initSDK && _userID >= 0 && _playBackHandel >= 0)
            {
                uint iOutValue = 0;
                if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(_playBackHandel, CHCNetSDK.NET_DVR_PLAYSLOW, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    ShowMsg("减缓回放速度失败，错误码：" + iLastErr);
                    VideoPlayBackLogger.Error("减缓回放速度失败，错误码：" + iLastErr, _cameraCfg.Channel);
                }
                else
                {
                    rtn = true;
                    VideoPlayBackLogger.Debug("降低回放速度。", _cameraCfg.Channel);
                }
            }
            return rtn;
        }

        /// <summary>
        /// 暂停播放录像
        /// </summary>
        /// <returns></returns>
        public bool PlayPause()
        {
            bool rtn = false;
            if (_initSDK && _userID >= 0 && _playBackHandel >= 0)
            {
                uint iOutValue = 0;
                if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(_playBackHandel, CHCNetSDK.NET_DVR_PLAYPAUSE, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    ShowMsg("暂定回放失败，错误码：" + iLastErr);
                    VideoPlayBackLogger.Error("暂定回放失败，错误码：" + iLastErr, _cameraCfg.Channel);
                }
                else
                {
                    rtn = true;
                    VideoPlayBackLogger.Debug("暂定回放。", _cameraCfg.Channel);
                }
            }
            return rtn;
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        /// <returns></returns>
        public bool PlayStop()
        {
            if (_playBackHandel >= 0)
            {
                if (!CHCNetSDK.NET_DVR_StopPlayBack(_playBackHandel))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    ShowMsg("停止回放失败，错误码：" + iLastErr);
                    VideoPlayBackLogger.Error("停止回放失败，错误码：" + iLastErr, _cameraCfg.Channel);
                }
                else
                {
                    _playBackHandel = -1;
                    _playbackTimer.Stop();
                    VideoPlayBackLogger.Debug("停止回放。", _cameraCfg.Channel);
                }

            }
            if (_userID >= 0)
            {
                if (!CHCNetSDK.NET_DVR_Logout(_userID))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    ShowMsg("停止回放时退出登录错误，错误码：" + iLastErr);
                    VideoPlayBackLogger.Error("停止回放时退出登录错误，错误码：" + iLastErr, _cameraCfg.Channel);
                }
                else
                {
                    _userID = -1;
                    VideoPlayBackLogger.Debug("停止回放时退出登录。", _cameraCfg.Channel);
                }
            }
            if (_initSDK)
            {
                 CHCNetSDK.NET_DVR_Cleanup();
                 VideoPlayBackLogger.Debug("停止回放时,清理设备。", _cameraCfg.Channel);
                _initSDK = false;
            }
            return true;
        }


        /// <summary>
        /// 按照进度播放
        /// </summary>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public bool SetPlayPos(int pPos)
        {
            uint iOutValue = 0;
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(lpOutBuffer, pPos); ;
            //设置回放进度
            if (_playBackHandel >= 0)
            {
                return CHCNetSDK.NET_DVR_PlayBackControl_V40(_playBackHandel, CHCNetSDK.NET_DVR_PLAYSETPOS, IntPtr.Zero, 0, lpOutBuffer, ref iOutValue);
            }
            else
            {
                return false;
            }
        }

    }
}
