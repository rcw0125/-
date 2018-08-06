using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Talent.CommonMethod
{
    public class WebRequestCommon
    {
        static WebRequestCommon()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 150;
        }
        /// <summary>
        /// get请求服务
        /// </summary>
        /// <param name="requestUrl">地址(地址？+参数)</param>
        /// <param name="timeout">超时设置</param>
        /// <returns>HttpWebRequest</returns>
        public static HttpWebRequest GetHttpGetWebRequest(string requestUrl, int timeout)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "GET";
                request.Timeout = timeout * 1000;
                request.ReadWriteTimeout = timeout * 1000;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = UrlEncode(requestUrl);
                request.KeepAlive = false;

                return request;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string UrlEncode(string url)
        {
            byte[] bs = Encoding.GetEncoding("GB2312").GetBytes(url);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bs.Length; i++)
            {
                if (bs[i] < 128)
                    sb.Append((char)bs[i]);
                else
                {
                    sb.Append("%" + bs[i++].ToString("x").PadLeft(2, '0'));
                    sb.Append("%" + bs[i].ToString("x").PadLeft(2, '0'));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// post请求服务
        /// </summary>
        /// <param name="requestUrl">地址</param>
        /// <param name="timeout">超时设置</param>
        /// <param name="requestXML">参数</param>
        /// <param name="encoding">编码格式 例如:utf-8</param>
        /// <returns>HttpWebRequest</returns>
        public static HttpWebRequest GetHttpPostWebRequest(string requestUrl, int timeout, string requestXML, string encoding)
        {
            try
            {
                encoding = encoding == "" ? "utf-8" : encoding;
                byte[] bytes = System.Text.Encoding.GetEncoding(encoding).GetBytes(requestXML);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Method = "POST";
                request.Timeout = timeout * 1000;
                request.ReadWriteTimeout = timeout * 1000;
                request.Referer = requestUrl;
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                request.KeepAlive = false;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                return request;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取请求结果
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="timeout">超时时间(秒)</param>
        /// <param name="requestXML">请求xml内容</param>
        /// <param name="isPost">是否post提交</param>
        /// <param name="msg">抛出的错误信息</param>
        /// <returns>返回请求结果</returns>
        public static string HttpPostWebRequest(string requestUrl, int timeout, string requestXML, bool isPost, out string msg)
        {
            return HttpPostWebRequest(requestUrl, timeout, requestXML, isPost, "utf-8", out msg);
        }
        /// <summary>
        /// 获取请求结果
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="timeout">超时时间(秒)</param>
        /// <param name="requestXML">请求xml内容</param>
        /// <param name="isPost">是否post提交</param>
        /// <param name="encoding">编码格式 例如:utf-8</param>
        /// <param name="msg">抛出的错误信息</param>
        /// <returns>返回请求结果</returns>
        public static string HttpPostWebRequest(string requestUrl, int timeout, string requestXML, bool isPost, string encoding, out string msg)
        {
            msg = string.Empty;
            string result = string.Empty;
            try
            {
                byte[] bytes = System.Text.Encoding.GetEncoding(encoding).GetBytes(requestXML);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = requestUrl;
                request.Method = isPost ? "POST" : "GET";
                request.ContentLength = bytes.Length;
                request.Timeout = timeout * 1000;
                request.KeepAlive = false;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding(encoding));
                    result = reader.ReadToEnd();
                    reader.Close();
                    responseStream.Close();
                    request.Abort();
                    response.Close();
                    return result.Trim();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message + ex.StackTrace;
            }

            return result;
        }

        /// <summary>
        /// get方式输出结果
        /// </summary>
        /// <param name="responseUrl"></param>
        /// <param name="timeOut">秒</param>
        /// <param name="msg"> </param>
        public static void HttpGetWebResponse(string responseUrl, int timeOut, out string msg)
        {
            msg = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(responseUrl);
                request.Method = "GET";
                request.Timeout = timeOut * 1000;
                request.ContentType = "application/x-www-form-urlencoded";
                request.KeepAlive = false;
                request.GetResponse();
                request.Abort();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
        }

        /// <summary>
        /// get方式输出结果
        /// </summary>
        /// <param name="responseUrl"></param>
        /// <param name="timeOut">秒</param>
        /// <param name="responseParams">输出参数</param>
        /// <param name="msg"> </param>
        public static void HttpGetWebResponse(string responseUrl, int timeOut, string responseParams, out string msg)
        {
            string url = responseUrl;
            if (url.Trim().Contains("?"))
            {
                url += "&" + responseParams;
            }
            else
            {
                url += "?" + responseParams;
            }

            HttpGetWebResponse(url, timeOut, out msg);
        }

        /// <summary>
        /// 获取通知地址
        /// </summary>
        /// <param name="responseUrl"></param>
        /// <param name="responseParams"></param>
        /// <returns></returns>
        public static string GetWebResponseUrl(string responseUrl, string responseParams)
        {
            string url = responseUrl;
            if (url.Trim().Contains("?"))
            {
                url += "&" + responseParams;
            }
            else
            {
                url += "?" + responseParams;
            }

            return url;
        }

        /// <summary>
        /// 获取节点值
        /// </summary>
        /// <param name="xmlD"></param>
        /// <param name="selectSingleNode"></param>
        /// <returns></returns>
        public static string GetSingleNodeValue(XmlDocument xmlD, string selectSingleNode)
        {
            string result = string.Empty;
            if (xmlD != null)
            {
                var node = xmlD.SelectSingleNode(selectSingleNode);
                if (node != null)
                {
                    result = node.InnerText;
                }
            }

            return result;
        }
    }
}

