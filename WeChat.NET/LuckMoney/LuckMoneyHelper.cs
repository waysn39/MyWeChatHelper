using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace WeChat.NET.LuckMoney
{
    /// <summary>
    /// 红包帮助类
    /// </summary>
    public static class LuckMoneyHelper
    {
        /// <summary>
        /// 抢红包
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isError"></param>
        /// <returns></returns>
        public static string RobLuckMoney(string url ,ref bool isError)
        {
            isError = false;
            string result = string.Empty;
            string baseUrl = "";
            NameValueCollection nvc = new NameValueCollection();
            nvc = ParseUrl(url, out baseUrl);
            StringBuilder sb = new StringBuilder();
            string urlKey = nvc["sn"];
            string luckyCount = nvc["lucky_number"];
            if (string.IsNullOrEmpty(urlKey) || string.IsNullOrEmpty(luckyCount))
            {
                result = @"抱歉，没有找到红包链接";
                isError = true;
            }
            else
            {
                LuckMoneyType album = new LuckMoneyType()
                {
                    type = 2,
                    data =
                        new LuckMoney()
                        {
                            urlKey = urlKey,
                            luckyCount = Convert.ToInt32(luckyCount)
                        }
                };
                // serialize to string            
                try
                {
                    string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(album, Newtonsoft.Json.Formatting.Indented);
                    string message = PostUrl(json2, "http://fa-ge.me:3000/changeIntoDahongbao");
                    ServerResult serverResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerResult>(message);
                    if (serverResult.Code == 0)
                    {
                        result = @"下一个就是大红包了， 赶紧去领取！";
                    }
                    else
                    {
                        result = serverResult.Message; ;
                        isError = true;
                    }
                }
                catch (Exception ex)
                {
                    result = @"红包接口调用异常，请稍后再试！";
                    isError = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 取红包
        /// </summary>
        /// <param name="isError"></param>
        /// <returns></returns>
        public static string GetLuckMoney(ref bool isError) {
            isError = false;
            string result = string.Empty;
            GetLuckMoney data = new GetLuckMoney()
            {
                type = 2
            };
            try
            {
                string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                string message = PostUrl(json2, "http://fa-ge.me:3000/createDahongbao");
                ServerResultEx serverResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerResultEx>(message);
                if (serverResult.code == 0)
                {
                    result = serverResult.data.url;
                }
                else
                {
                    result = @"领取出错,请查看返回Json:" + message;
                    isError = true;
                }
            }
            catch (Exception ex)
            {
                result = @"红包接口调用异常，请稍后再试！";
                isError = true;
            }


            return result;

        }



        /// <summary>
        /// 分析url链接，返回参数集合
        /// </summary>
        /// <param name="url">url链接</param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static System.Collections.Specialized.NameValueCollection ParseUrl(string url, out string baseUrl)
        {
            baseUrl = "";
            if (string.IsNullOrEmpty(url))
                return null;
            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

            try
            {
                int questionMarkIndex = url.IndexOf('?');

                if (questionMarkIndex == -1)
                    baseUrl = url;
                else
                    baseUrl = url.Substring(0, questionMarkIndex);
                if (questionMarkIndex == url.Length - 1)
                    return null;
                string ps = url.Substring(questionMarkIndex + 1);

                // 开始分析参数对   
                System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", System.Text.RegularExpressions.RegexOptions.Compiled);
                System.Text.RegularExpressions.MatchCollection mc = re.Matches(ps);

                foreach (System.Text.RegularExpressions.Match m in mc)
                {
                    nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
                }

            }
            catch { }
            return nvc;
        }

        /*
*  url:POST请求地址
*  postData:json格式的请求报文,例如：{"key1":"value1","key2":"value2"}
*/

        public static string PostUrl(string postData, string url)
        {

            string serviceAddress = url;// "http://fa-ge.me:3000/changeIntoDahongbao";

            string result = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(serviceAddress);

            req.Method = "POST";

            req.ContentType = "application/json";

            byte[] data = Encoding.UTF8.GetBytes(postData);

            req.ContentLength = data.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);

                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Stream stream = resp.GetResponseStream();

            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

    }

    public class GetLuckMoney
    {
        public int type { get; set; }
    }

    public class LuckMoneyType
    {
        public int type { get; set; }
        public LuckMoney data { get; set; }
    }

    public class LuckMoney
    {
        public string urlKey { set; get; }
        public int luckyCount { set; get; }
    }

    public class ServerResult
    {
        public int Code { get; set; }
        public string Message { get; set; }

    }

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
    }

    public class ServerResultEx
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
    }
}