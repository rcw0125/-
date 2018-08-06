using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace Talent.CommonMethod
{
    public delegate void ReceiveDataChanged(long current, long total);
    public delegate void DownloadDataCompleted(byte[] data);
    public class FtpManager
    {
        /// <summary>
        /// 下载进度事件
        /// </summary>
        public event ReceiveDataChanged OnReceiveDataChanged;
        /// <summary>
        /// 下载完毕事件
        /// </summary>
        public event DownloadDataCompleted OnDownloadDataCompleted;
        string ftpServerIP;
        string ftpUserID;
        string ftpPassword;
        FtpWebRequest reqFTP;

        /// <summary>
        /// 当前工作目录
        /// </summary>
        private string _DirectoryPath;

        /// <summary>
        /// 当前工作目录
        /// </summary>
        public string DirectoryPath
        {
            get { return _DirectoryPath; }
            set { _DirectoryPath = value; }
        }

        //测试ftp连接是否正常
        public void TestConnect()
        {
            FtpWebResponse ftpResponse = null;
            try
            {
                Connect("ftp://" + ftpServerIP + "/");
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.KeepAlive = false;
                reqFTP.Timeout = 3000;
                ftpResponse = (FtpWebResponse)reqFTP.GetResponse();
            }
            catch
            {
                throw new Exception("无法连接ftp服务器 IP:" + ftpServerIP + " User ID:" + ftpUserID);
            }
            finally
            {
                if (ftpResponse != null)
                {
                    ftpResponse.Close();

                }

            }

        }

        //连接ftp
        private void Connect(String path)
        {
            // 根据uri创建FtpWebRequest对象
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
            // 指定数据传输类型
            reqFTP.UseBinary = true;
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }

        public void FtpUpDown(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIP = ftpServerIP;
            this.ftpUserID = ftpUserID;
            this.ftpPassword = ftpPassword;
        }

        //都调用这个
        //上面的代码示例了如何从ftp服务器上获得文件列表
        private string[] GetFileList(string path, string WRMethods)
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            try
            {
                Connect(path);
                reqFTP.Method = WRMethods;
                reqFTP.KeepAlive = false;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);//中文文件名
                string line = reader.ReadLine();

                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }

                // to remove the trailing '' '' 
                if (result.ToString() != "")
                {
                    result.Remove(result.ToString().LastIndexOf("\n"), 1);
                }
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }

            catch (Exception ex)
            {
                //  System.Windows.Forms.MessageBox.Show(ex.Message);
                downloadFiles = null;
                throw new Exception("获取文件列表失败。原因： " + ex.Message);
                return downloadFiles;
            }
        }

        //上面的代码示例了如何从ftp服务器上获得文件列表
        public string[] GetFileList(string path)
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectory);

        }

        public string[] GetFileList()//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectory);

        }

        public void Upload(string filename) //上面的代码实现了从ftp服务器上载文件的功能
        {

            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
            Connect(uri);//连接          
            // 默认为true，连接不会被关闭
            // 在一个命令之后被执行
            reqFTP.KeepAlive = false;
            // 指定执行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 上传文件时通知服务器文件的大小
            reqFTP.ContentLength = fileInf.Length;
            // 缓冲大小设置为kb 
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打开一个文件流(System.IO.FileStream) 去读上传的文件
            FileStream fs = fileInf.OpenRead();

            try
            {

                int allbye = (int)fileInf.Length;
                int startbye = 0;
                //pb.Maximum = (int)allbye;
                //pb.Minimum = 0;
                //pb.Visible = true;

                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的kb 
                contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);

                    startbye += buffLength;

                    // Application.DoEvents();
                }

                //pb.Visible = false;
                // 关闭两个流
                strm.Close();
                fs.Close();

            }

            catch (Exception ex)
            {
                this.WriteLog("上传失败,原因: " + ex.Message);

                fs.Close();
            }

        }

        public bool Download(string filePath, string fileName, out string errorinfo)/**/////上面的代码实现了从ftp服务器下载文件的功能
        {
            try
            {
                String onlyFileName = Path.GetFileName(fileName);
                string newFileName = filePath + @"\" + onlyFileName;
                if (File.Exists(newFileName))
                {
                    File.Delete(newFileName);
                    //errorinfo = string.Format("本地文件{0}已存在,无法下载", newFileName);
                    // return false;
                }

                string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//连接  
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                FileStream outputStream = new FileStream(newFileName, FileMode.Create);

                int allbye = (int)GetFileSize(fileName);
                int startbye = 0;
                //pb.Maximum = (int)allbye;
                //pb.Minimum = 0;
                //pb.Visible = true;

                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    startbye += readCount;
                    //  pb.Value = startbye;
                    //Application.DoEvents();
                }
                //pb.Visible = false;
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                errorinfo = "";
                MoveFile(fileName, "Read Completed");
                return true;
            }
            catch (Exception ex)
            {
                //errorinfo = string.Format("因{0},无法下载", ex.Message);
                throw new Exception("下载失败，原因: " + ex.Message);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public void DownloadAsync(string fileName)/**/////上面的代码实现了从ftp服务器下载文件的功能
        {
            System.Threading.ThreadPool.QueueUserWorkItem((s) =>
            {
                // Byte[] rtn = null;
                try
                {
                    long allbye = (long)GetFileSize(fileName);
                    string url = "ftp://" + ftpServerIP + "/" + fileName;
                    Connect(url);//连接  
                    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                    reqFTP.KeepAlive = false;


                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();
                    long cl = response.ContentLength;
                    int bufferSize = 2048;
                    int readCount = 0;
                    byte[] buffer = new byte[bufferSize];
                    readCount = ftpStream.Read(buffer, 0, bufferSize);

                    MemoryStream mstream = new MemoryStream();

                    int startbye = 0;

                    while (readCount > 0)
                    {
                        mstream.Write(buffer, 0, readCount);

                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                        startbye += readCount;
                        if (OnReceiveDataChanged != null)
                        {
                            OnReceiveDataChanged(startbye, allbye);
                        }
                    }
                    if (OnReceiveDataChanged != null)
                    {
                        OnReceiveDataChanged(allbye, allbye);
                    }
                    ftpStream.Close();
                    response.Close();
                    // rtn = mstream.ToArray();
                    mstream.Close();
                    if (OnDownloadDataCompleted != null)
                    {
                        Thread.Sleep(50);
                        OnDownloadDataCompleted(mstream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    //errorinfo = string.Format("因{0},无法下载", ex.Message);
                    throw new Exception("下载失败，原因: " + ex.Message);
                }
            });

        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Byte[] Download(string fileName)/**/////上面的代码实现了从ftp服务器下载文件的功能
        {
            Byte[] rtn = null;
            try
            {
                long allbye = (long)GetFileSize(fileName);
                string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//连接  
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;


                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount = 0;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);

                MemoryStream mstream = new MemoryStream();

                int startbye = 0;

                while (readCount > 0)
                {
                    mstream.Write(buffer, 0, readCount);

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    startbye += readCount;
                }
                rtn = mstream.ToArray();
                ftpStream.Close();
                response.Close();

                mstream.Close();

                return rtn;
            }
            catch (Exception ex)
            {
                //errorinfo = string.Format("因{0},无法下载", ex.Message);
                throw new Exception("下载失败，原因: " + ex.Message);
            }
        }

        /// <summary>
        /// 检查目录,如果没有目录则创建目录
        /// </summary>
        /// <param name="remoteDirName"></param>
        public void CheckDirectoryAndMake(string remoteDirName)
        {
            if (!DirectoryExist("", remoteDirName))
                MakeDir(remoteDirName);
            if (!DirectoryExist(remoteDirName, DateTime.Now.ToString("yyyy-MM-dd")))
                MakeDir(remoteDirName + "/" + DateTime.Now.ToString("yyyy-MM-dd"));
        }


        public void CheckDirectoryAndMakeMyWilson(string remoteDirName)
        {
            try
            {
                if (!DirectoryExist("/", remoteDirName))
                    MakeDir(remoteDirName);
            }
            catch //(Exception ex)
            {

                //  ShowSysMessage(ex.ToString(), "FTP", Msg_Level.MSG_LEVEL_ERROR);
            }
        }



        public void CheckDirectoryAndMakeMyWilson2(string rootDir, string remoteDirName)
        {
            if (!DirectoryExist(rootDir, remoteDirName))
                MakeDir(rootDir + "\\" + remoteDirName);
            //if (!DirectoryExist(remoteDirName, DateTime.Now.ToString("yyyyMMddhhmm")))
            //    MakeDir(remoteDirName + "/" + DateTime.Now.ToString("yyyyMMddhhmm"));
        }

        /// <summary>
        /// 判断文件的目录是否存,不存则创建
        /// </summary>
        /// <param name="destFilePath">本地文件目录</param>
        public void CheckDirectoryAndMakeMyWilson3(string destFilePath)
        {
            string fullDir = destFilePath.IndexOf(':') > 0 ? destFilePath.Substring(destFilePath.IndexOf(':') + 1) : destFilePath;
            fullDir = fullDir.Replace('\\', '/');
            string[] dirs = fullDir.Split('/');
            string curDir = "/";
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i] == "") continue;
                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空 
                if (dir != null && dir.Length > 0)
                {
                    try
                    {

                        CheckDirectoryAndMakeMyWilson2(curDir, dir);
                        curDir += dir + "/";
                        //FtpMakeDir(curDir); 
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        public void MoveFile(string sourceFile, string destDirName)
        {
            CheckDirectoryAndMake(destDirName);

            int i = 0;
            string fileSup = sourceFile.Substring(0, sourceFile.IndexOf('.'));
            string fileSub = sourceFile.Substring(sourceFile.IndexOf('.'));

            string destFile = sourceFile;
            destDirName = destDirName + "/" + DateTime.Now.ToString("yyyy-MM-dd");
            while (true)
            {
                if (i != 0)
                    destFile = fileSup + i + fileSub;

                if (!FileExist(destDirName, destFile))
                {
                    Rename(sourceFile, destDirName + "/" + destFile);
                    break;
                }
                i++;
            }
        }

        //获取子目录
        public string[] GetDirectoryList(string dirName)
        {
            string[] drectory = GetFilesDetailList(dirName);
            List<string> strList = new List<string>();
            if (drectory.Length > 0)
            {
                foreach (string str in drectory)
                {
                    if (str.Trim().Length == 0)
                        continue;
                    //会有两种格式的详细信息返回
                    //一种包含<DIR>
                    //一种第一个字符串是drwxerwxx这样的权限操作符号
                    //现在写代码包容两种格式的字符串
                    if (str.Trim().Contains("<DIR>"))
                    {
                        strList.Add(str.Substring(39).Trim());
                    }
                    else
                    {
                        if (str.Trim().Substring(0, 1).ToUpper() == "D")
                        {
                            strList.Add(str.Substring(55).Trim());
                        }
                    }
                }
            }
            return strList.ToArray();
        }

        /// <summary>
        /// 判断当前目录下指定的子目录是否存在
        /// </summary>
        /// <param name="RemoteDirectoryName">指定的目录名</param>
        public bool DirectoryExist(string rootDir, string RemoteDirectoryName)
        {
            string[] dirList = GetDirectoryList(rootDir);
            if (dirList.Length > 0)
            {
                foreach (string str in dirList)
                {
                    if (str.Trim() == RemoteDirectoryName.Trim())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断当前目录下指定的文件是否存在
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        public bool FileExist(string path, string RemoteFileName)
        {
            string[] fileList = GetFileList(path);
            for (int i = 0; i < fileList.Length; i++)
            {
                if (fileList[i].LastIndexOf('/') != -1)
                    fileList[i] = fileList[i].Substring(fileList[i].LastIndexOf('/') + 1);
            }
            foreach (string str in fileList)
            {
                if (str.Trim() == RemoteFileName.Trim())
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteFileName(string fileName)
        {
            try
            {
                FileInfo fileInf = new FileInfo(fileName);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                Connect(uri);//连接         
                // 默认为true，连接不会被关闭
                // 在一个命令之后被执行
                reqFTP.KeepAlive = false;
                // 指定执行什么命令
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("删除文件失败，原因: " + ex.Message);
            }

        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dirName"></param>
        public void MakeDir(string dirName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//连接       
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }

            catch (Exception ex)
            {
                throw new Exception("创建文件失败，原因: " + ex.Message);
            }

        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dirName"></param>
        public void delDir(string dirName)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//连接       
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("删除文件失败，原因: " + ex.Message);
            }

        }

        /// <summary>
        /// 获得文件大小
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public long GetFileSize(string filename)
        {
            long fileSize = 0;
            try
            {
                FileInfo fileInf = new FileInfo(filename);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf;
                Connect(uri);//连接       
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                fileSize = response.ContentLength;
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("获取文件大小失败，原因: " + ex.Message);
            }
            return fileSize;
        }

        /// <summary>
        /// 文件改名
        /// </summary>
        /// <param name="currentFilename"></param>
        /// <param name="newFilename"></param>
        public void Rename(string currentFilename, string newFilename)
        {
            try
            {
                FileInfo fileInf = new FileInfo(currentFilename);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                Connect(uri);//连接
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                reqFTP.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("重命名文件失败，原因: " + ex.Message);
            }

        }

        /// <summary>
        /// 获得文件明晰
        /// </summary>
        /// <returns></returns>
        public string[] GetFilesDetailList()
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        /// <summary>
        /// 获得文件明晰
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetFilesDetailList(string path)
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="sErrorInfo"></param>
        public void WriteLog(string sErrorInfo)
        {
            //StreamWriter sWrite = new StreamWriter(Application.StartupPath + "\\FTPInfo" + DateTime.Now.ToString("yyyyMMdd") + ".log", true);
            //sWrite.WriteLine(System.DateTime.Now + "  " + sErrorInfo);
            //sWrite.Close();
        }


    }
}
