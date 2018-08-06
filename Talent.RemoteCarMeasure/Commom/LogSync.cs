using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Talent.CommonMethod;

namespace Talent.RemoteCarMeasure.Commom
{
    /// <summary>
    /// 日志同步到ftp服务器
    /// </summary>
    public class LogSync
    {
        string _seatId;
        string _ftpIP;
        string _ftpPort;
        string _ftpUserName;
        string _ftpPassWord;
        string _logSavePath;
         /// <summary>
        /// 是否正在同步文件
        /// </summary>
        bool _fileUploadRunning = false;
        /// <summary>
        /// 当前要上传的日志文件
        /// </summary>
        FileInfo[] _curUploadLogFiles;
        /// <summary>
        /// 
        /// </summary>
        FtpManager _curFtpServices;

        /// <summary>
        /// 文件同步计时器
        /// </summary>
        DispatcherTimer curUploadTimer;

        public LogSync()
        {

            _seatId = AppConfigReader.ReadCfg("SeatId");
            _ftpIP = AppConfigReader.ReadCfg("FtpIp");
            _ftpPort = AppConfigReader.ReadCfg("FtpPort");
            _ftpUserName = AppConfigReader.ReadCfg("FtpUserName");
            _ftpPassWord = AppConfigReader.ReadCfg("FtpPassWord");
            _logSavePath = AppConfigReader.ReadCfg("LogSavePath");

             _curFtpServices = new FtpManager();
             _curFtpServices.FtpUpDown(_ftpIP, _ftpUserName, _ftpPassWord);
        }

        /// <summary>
        /// 启动日志同步
        /// </summary>
        public void StartSyncLog()
        {
            //定时器相关
            curUploadTimer = new DispatcherTimer();
            curUploadTimer.Interval = new TimeSpan(0, 1, 0); //分钟检测一次
            curUploadTimer.Tick += curUploadJpgTimer_Tick;
            curUploadTimer.Start();
        }

        
       

        /// <summary>
        /// 定时处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void curUploadJpgTimer_Tick(object sender, EventArgs e)
        {
            if (!_fileUploadRunning)
            {
                _curUploadLogFiles = GetLogFile(_logSavePath);
            }
            UpLoadFile();
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        private void UpLoadFile()
        {
            _fileUploadRunning = true;
            //上传日志文件
            if (_curUploadLogFiles != null && _curUploadLogFiles.Length > 0)
            {
                foreach (FileInfo file in _curUploadLogFiles)
                {
                    try
                    {
                        string date = file.CreationTime.ToString("yyyy-MM-dd");
                        string path = string.Format("ServerLog/{0}/{1}/{2}", _seatId, file.CreationTime.Year, date);
                        //设置当前目录
                        _curFtpServices.FtpUpDown(_ftpIP, _ftpUserName, _ftpPassWord);
                        //创建目录
                        _curFtpServices.CheckDirectoryAndMakeMyWilson3(path);
                        //设置目录
                        _curFtpServices.FtpUpDown(_ftpIP + "/" + path + "/", _ftpUserName, _ftpPassWord);
                        //上传文件
                        _curFtpServices.Upload(file.FullName);
                        file.Delete();
                    }
                    catch //(Exception ex)
                    {
                    }
                }
            }
            _fileUploadRunning = false;
        }
        /// <summary>
        /// 获取日志文件
        /// </summary>
        /// <param name="pDir"></param>
        /// <returns></returns>
        private FileInfo[] GetLogFile(string pDir)
        {
            List<FileInfo> rtn = new List<FileInfo>();
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(pDir);
                if (Directory.Exists(pDir) == false) return null;
                FileInfo[] rtnList = dirInfo.GetFiles("*.log", SearchOption.AllDirectories);
                //获取今天之前的文件
                if (rtnList != null && rtnList.Length > 0)
                {
                    for (int count = 0; count < rtnList.Length; count++)
                    {
                        if (rtnList[count].CreationTime.Date < DateTime.Now.Date)
                        {
                            rtn.Add(rtnList[count]);
                        }
                    }
                }
            }
            catch //(Exception ex)
            {
            }

            return rtn.ToArray();
        }
    }
}
