using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Talent.ClinetLog;
using Talent.Measure.DomainModel;
using Talent.Video.Interface;

namespace Talent.Video.Controller
{
    /// <summary>
    /// 摄像头控制类
    /// </summary>
    public class VideoPlayBackController
    {
        /// <summary>
        /// 弹出错误消息
        /// </summary>
        public event Action<string> OnShowErrMsg;
        /// <summary>
        /// 当前视频配置
        /// </summary>
        VideoConfig _curVideoConfig;


        /// <summary>
        /// 显示视屏控件的句柄
        /// </summary>
        IntPtr _controlHandel;

        /// <summary>
        /// 摄像头
        /// </summary>
        ICameraPlayBack _curICameraPlayBack;




        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pVideoConfig">视频配置</param>
        /// <param name="pControlHandel">控件句柄</param>
        public VideoPlayBackController(VideoConfig pVideoConfig, IntPtr pControlHandel)
        {
            VideoPlayBackLogger.Debug("==========================开始=============================", "");
            _controlHandel = pControlHandel;
            _curVideoConfig = pVideoConfig;
        }


          /// <summary>
        /// 设置播放录像的时间段
        /// </summary>
        /// <param name="pStartTime">录像开始时间</param>
        /// <param name="pEndTime">录像结束时间</param>
        /// <returns></returns>
        public void PlayByTime(DateTime pStartTime, DateTime pEndTime)
        {
            CreateCamera();
            if (_curICameraPlayBack != null)
            {
                if (_curICameraPlayBack.PlayByTime(pStartTime, pEndTime))
                {
                   // _curICameraPlayBack.PlayStart();
                }
            }
        }

        /// <summary>
        /// 开始回放
        /// </summary>
        /// <returns></returns>
        public void PlayStart()
        {
            if (_curICameraPlayBack != null)
            {
                _curICameraPlayBack.PlayStart();
            }
        }
           /// <summary>
        /// 加快回放速度
        /// </summary>
        /// <returns></returns>
        public bool PlayFast()
        {
           bool rtn = false;
            if (_curICameraPlayBack != null)
            {
                rtn=_curICameraPlayBack.PlayFast();
            }
            return rtn;
        }

        /// <summary>
        /// 降低回放速度
        /// </summary>
        /// <returns></returns>
        public bool PlaySlow()
        {
            bool rtn = false;
            if (_curICameraPlayBack != null)
            {
                rtn=_curICameraPlayBack.PlaySlow();
            }
            return rtn;
        }

        /// <summary>
        /// 暂停回放
        /// </summary>
        /// <returns></returns>
        public void PlayPause()
        {
            if (_curICameraPlayBack != null)
            {
                _curICameraPlayBack.PlayPause();
            }
        }

        /// <summary>
        /// 停止回放
        /// </summary>
        /// <returns></returns>
        public bool PlayStop()
        {
            bool rtn = false;
            if (_curICameraPlayBack != null)
            {
               rtn= _curICameraPlayBack.PlayStop();
            }
            return rtn;
        }

        #region Private

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
        /// 根据配置创建摄像头
        /// </summary>
        private void CreateCamera()
        {
            if (_curVideoConfig != null && _curVideoConfig.CameraList != null )
            {
                if(_curVideoConfig.CameraList.Count>0)
                {
                    _curICameraPlayBack = CreateInstance(_curVideoConfig.CameraList[0], _controlHandel);
                    _curICameraPlayBack.OnShowErrMsg += ShowMsg;
                }
            }
        }
        /// <summary>
        /// 创建摄像头实例
        /// </summary>
        /// <param name="pCameraCfg">摄像头配置</param>
        /// <param name="pControlHandel">视频控件句柄</param>
        /// <param name="pDrawStr">视频叠加的字符串</param>
        /// <returns></returns>
        private ICameraPlayBack CreateInstance(Camera pCameraCfg, IntPtr pControlHandel)
        {
            //string driverPath = System.AppDomain.CurrentDomain.BaseDirectory;
            //driverPath = Path.Combine(driverPath, _curVideoConfig.VideoDriver);
            //Assembly assembly = Assembly.LoadFile(driverPath);

            //string name = assembly.FullName.Split(',')[0] + ".CameraControl";
            //Type type = assembly.GetType(name);
            //ICamera cam = Activator.CreateInstance(type, pCameraCfg, pControlHandel) as ICamera;
            ICameraPlayBack cam;
            cam = new Talent.Video.HKVideo.CameraPlayBackControl(pCameraCfg, pControlHandel);
           
            return cam;
        }
        #endregion

    }
}
