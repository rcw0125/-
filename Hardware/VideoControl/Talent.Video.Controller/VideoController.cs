using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Talent.Measure.DomainModel;
using Talent.Video.Interface;

namespace Talent.Video.Controller
{
    /// <summary>
    /// 摄像头控制类
    /// </summary>
    public class VideoController
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
        /// 照片相关配置
        /// </summary>
        FtpConfig _curPhotoConfig;
        /// <summary>
        /// 显示视屏控件的句柄
        /// </summary>
        List<IntPtr> _controlHandelList;

        /// <summary>
        /// 摄像头集合
        /// </summary>
        List<ICamera> _curCameraList;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pVideoConfig">视频配置</param>
        /// <param name="pControlHandel">控件句柄</param>
        public VideoController(FtpConfig pPhotoConfig, VideoConfig pVideoConfig, List<IntPtr> pControlHandelList)
        {
            _curPhotoConfig = pPhotoConfig;
            _controlHandelList = pControlHandelList;
            _curVideoConfig = pVideoConfig;
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pVideoConfig">视频配置</param>
        /// <param name="pControlHandel">控件句柄</param>
        public VideoController(VideoConfig pVideoConfig, List<IntPtr> pControlHandelList)
        {
            _curPhotoConfig = new FtpConfig();
            _controlHandelList = pControlHandelList;
            _curVideoConfig = pVideoConfig;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pConfigFile">配置文件</param>
        /// <param name="pControlHandel">控件句柄</param>
        public VideoController(string pConfigFile, List<IntPtr> pControlHandelList)
        {
            //读取配置
            ConfigReader cfgReader = new ConfigReader(pConfigFile);
            _curPhotoConfig = ConfigReader.ReadPhotoConfig();
            _curVideoConfig = ConfigReader.ReadVideoConfig(); ;
            _controlHandelList = pControlHandelList;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pConfigFile">配置文件</param>
        /// <param name="pControlHandel">控件句柄</param>
        public VideoController(string pConfigFile)
        {
            //读取配置
            ConfigReader cfgReader = new ConfigReader(pConfigFile);
            _curPhotoConfig = ConfigReader.ReadPhotoConfig();
            _curVideoConfig = ConfigReader.ReadVideoConfig(); ;
            _controlHandelList = null;
        }

        /// <summary>
        /// 初始化摄像头
        /// </summary>
        public void Open()
        {
            CreateCamera();
            foreach (ICamera cam in _curCameraList)
            {
                cam.Open();
                break;
            }
        }
        /// <summary>
        /// 初始化传递进来的所有摄像头
        /// </summary>
        public void OpenAll()
        {
            CreateCameraAll();
            foreach (ICamera cam in _curCameraList)
            {
                cam.Open();
                break;
            }
        }
        /// <summary>
        /// 启动同步写入数据
        /// </summary>
        public void BeginStart()
        {
            foreach (ICamera cam in _curCameraList)
            {
                cam.BeginStart();
                break;
            }
        }
        /// <summary>
        /// 启动摄像头，登录，预览
        /// </summary>
        public void Start()
        {
            foreach (ICamera cam in _curCameraList)
            {
                cam.Start();
            }
        }

        /// <summary>
        /// 停止摄像头。停止预览，退出登录
        /// </summary>
        public void Stop()
        {
            foreach (ICamera cam in _curCameraList)
            {
                cam.Stop();
            }
        }

        /// <summary>
        /// 关闭摄像头
        /// </summary>
        public void Close()
        {
            foreach (ICamera cam in _curCameraList)
            {
                cam.Close();
                break;
            }
        }

        /// <summary>
        /// 抓图
        /// <param name="pBusiness">物流ID</param>
        /// <param name="pBusiness">业务类型</param>
        /// </summary>
        public void CapturePicture(string pLogisticsId, string pBusinessType)
        {
            foreach (ICamera cam in _curCameraList)
            {
                cam.CapturePicture(pLogisticsId, pBusinessType);
            }
        }
        /// <summary>
        /// 往视频写入信息……
        /// </summary>
        /// <param name="infos"></param>
        public void WriteTxtToVideo(string infos)
        {
            foreach (ICamera cam in _curCameraList)
            {
                cam.WriteTxtToVideo(infos);
                break;
            }
        }
        /// <summary>
        /// 同步时间到DVR
        /// </summary>
        /// <returns></returns>
        public bool SyncDateTime()
        {
            bool rtn = false;
            try
            {
                List<string> ips = new List<string>();
                foreach (Camera item in _curVideoConfig.CameraList)
                {
                    if (!ips.Contains(item.Ip))
                    {
                        ips.Add(item.Ip);
                    }
                }
                foreach (ICamera cam in _curCameraList)
                {
                    if (ips.Count > 0)
                    {
                        if (ips.IndexOf(cam.IP) >= 0)
                        {
                            ips.Remove(cam.IP);
                        }
                        rtn = cam.SyncDateTime();
                    }
                    else
                    {
                        break;
                    }
                }
                rtn = true;
            }
            catch //(Exception ex)
            {
                rtn = false;
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
            if (_curVideoConfig != null && _curVideoConfig.CameraList != null && _curVideoConfig.CameraList.Count > 0)
            {
                _curCameraList = new List<ICamera>();
                int start = 0;
                if (_controlHandelList != null)
                {
                    start = _controlHandelList.Count > 1 ? 1 : 0;
                }
                for (int count = start; count < _curVideoConfig.CameraList.Count; count++)
                {
                    int index = int.Parse(_curVideoConfig.CameraList[count].Position) - 1;//配置视频顺序 必须从1 开始依次递增……2016-3-2 10:06:08
                    if (_controlHandelList != null)
                    {
                        if (_controlHandelList.Count == 1 && count >= 1)//第一次只能打开第一个视频 ……2016-3-2 10:05:31
                        {
                            break;
                        }
                        if (start == 1)//打开剩余视频时，减一
                        {
                            index = index - 1;
                        }
                        if (index < _controlHandelList.Count)
                        {
                            ICamera cam;
                            //按照配置中Position的位置和界面控件做对应
                            if (_controlHandelList == null || _controlHandelList.Count <= 0)
                            {
                                cam = CreateInstance(_curVideoConfig.CameraList[count], null);
                            }
                            else
                            {
                                cam = CreateInstance(_curVideoConfig.CameraList[count], _controlHandelList[index]);
                            }
                            cam.OnShowErrMsg += ShowMsg;
                            _curCameraList.Add(cam);
                        }
                    }
                    else
                    {
                        ICamera cam;
                        //按照配置中Position的位置和界面控件做对应
                        cam = CreateInstance(_curVideoConfig.CameraList[count], null);
                        cam.OnShowErrMsg += ShowMsg;
                        _curCameraList.Add(cam);
                    }


                }
            }
        }

        /// <summary>
        /// 根据配置创建摄像头
        /// </summary>
        private void CreateCameraAll()
        {
            if (_curVideoConfig != null && _curVideoConfig.CameraList != null && _curVideoConfig.CameraList.Count > 0)
            {
                _curCameraList = new List<ICamera>();
                int start = 0;
                for (int count = start; count < _curVideoConfig.CameraList.Count; count++)
                {
                    if (_controlHandelList != null)
                    {
                        ICamera cam;
                        //按照配置中Position的位置和界面控件做对应
                        if (_controlHandelList == null || _controlHandelList.Count <= 0)
                        {
                            cam = CreateInstance(_curVideoConfig.CameraList[count], null);
                        }
                        else
                        {
                            cam = CreateInstance(_curVideoConfig.CameraList[count], _controlHandelList[count]);
                        }
                        cam.OnShowErrMsg += ShowMsg;
                        _curCameraList.Add(cam);
                    }
                    else
                    {
                        ICamera cam;
                        //按照配置中Position的位置和界面控件做对应
                        cam = CreateInstance(_curVideoConfig.CameraList[count], null);
                        cam.OnShowErrMsg += ShowMsg;
                        _curCameraList.Add(cam);
                    }


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
        private ICamera CreateInstance(Camera pCameraCfg, IntPtr? pControlHandel)
        {
            //string driverPath = System.AppDomain.CurrentDomain.BaseDirectory;
            //driverPath = Path.Combine(driverPath, _curVideoConfig.VideoDriver);
            //Assembly assembly = Assembly.LoadFile(driverPath);

            //string name = assembly.FullName.Split(',')[0] + ".CameraControl";
            //Type type = assembly.GetType(name);
            //ICamera cam = Activator.CreateInstance(type, pCameraCfg, pControlHandel) as ICamera;
            ICamera cam;
            if (pControlHandel != null)
            {
                cam = new Talent.Video.HKVideo.CameraControl(_curPhotoConfig, pCameraCfg, pControlHandel);
            }
            else
            {
                cam = new Talent.Video.HKVideo.CameraControl(_curPhotoConfig, pCameraCfg, null);
            }
            return cam;
        }
        #endregion

    }
}
