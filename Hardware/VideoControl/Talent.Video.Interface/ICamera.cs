using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talent.Video.Interface
{
    public interface ICamera
    {
        string IP{get;}
        /// <summary>
        /// 弹出错误消息
        /// </summary>
        event Action<string> OnShowErrMsg;
        /// <summary>
        /// 初始化摄像头
        /// </summary>
        /// <returns></returns>
        bool Open();
        /// <summary>
        /// 初始化摄像头
        /// </summary>
        /// <returns></returns>
        bool OpenAll();
        /// <summary>
        /// 登录启动预览。
        /// </summary>
        /// <returns></returns>
        bool Start();
        /// <summary>
        /// 停止预览，退出登录
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// 关闭摄像头
        /// </summary>
        /// <returns></returns>
        bool Close();
        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="pLogisticsNo">物流Id</param>
        /// <param name="pBusinessType">业务类型</param>
        /// 
        /// <returns></returns>
        bool CapturePicture(string pLogisticsId, string pBusinessType);

        bool SyncDateTime();

        /// <summary>
        /// 往视频写入信息定时器//测试使用
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        void BeginStart();
        /// <summary>
        /// 往视频写入信息
        /// </summary>
        /// <param name="infos"></param>
        void WriteTxtToVideo(string infos);
    }

    /// <summary>
    /// 录像回放接口
    /// </summary>
    public interface ICameraPlayBack
    {
        /// <summary>
        /// 弹出错误消息
        /// </summary>
        event Action<string> OnShowErrMsg;

        /// <summary>
        /// 设置播放录像的时间段
        /// </summary>
        /// <param name="pStartTime">录像开始时间</param>
        /// <param name="pEndTime">录像结束时间</param>
        /// <returns></returns>
        bool PlayByTime(DateTime pStartTime, DateTime pEndTime);

        /// <summary>
        /// 按照时间下载视频文件
        /// </summary>
        /// <param name="pStartTime">开始时间</param>
        /// <param name="pEndTime">停止时间</param>
        /// <param name="pFile">下载的文件名及路径</param>
        /// <returns></returns>
        //bool DownLoadVideoByTime(DateTime pStartTime, DateTime pEndTime,string pFile);
        /// <summary>
        /// 播放录像
        /// </summary>
        /// <returns></returns>
        bool PlayStart();

        /// <summary>
        /// 加快播放速度
        /// </summary>
        /// <returns></returns>
        bool PlayFast();

        /// <summary>
        /// 降低播放速度
        /// </summary>
        /// <returns></returns>
        bool PlaySlow();

        /// <summary>
        /// 暂停播放录像
        /// </summary>
        /// <returns></returns>
        bool PlayPause();
        /// <summary>
        /// 停止播放
        /// </summary>
        /// <returns></returns>
        bool PlayStop();
    }
}
