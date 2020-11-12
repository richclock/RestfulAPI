using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;

namespace ITRI
{
    [ServiceContract(Name = "RestFulService")]
    public interface IRestServer
    {
        [OperationContract]
        [WebGet(UriTemplate = "api/get", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Task<Stream> Get();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "api/post", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Task<Stream> Post(Stream body);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestServer : ApiController, IRestServer
    {
        public RestServer()
        {

        }
        public async Task<Stream> Get()
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            return await Task.Run(() =>
            {
                string ret = new JavaScriptSerializer().Serialize(new
                {
                    error = "Restful Get Test"
                });

                byte[] resultBytes = Encoding.UTF8.GetBytes(ret);
                return new MemoryStream(resultBytes);
            });
        }
        public async Task<Stream> Post(Stream body)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            string content = null;
            using (StreamReader reader = new StreamReader(body, Encoding.UTF8))
            {
                content = await reader.ReadToEndAsync();
            }
            return await Task.Run(() =>
            {

                object json = new JavaScriptSerializer().DeserializeObject(content);
                string ret = new JavaScriptSerializer().Serialize(new
                {
                    echo = json
                });

                byte[] resultBytes = Encoding.UTF8.GetBytes(ret);
                return new MemoryStream(resultBytes);
            });

        }
    }

    public class RestClient
    {
        #region 成員變數
        public string Url { get; set; }
        public string ContentType { get; private set; }
        public int TimeOut { get; set; }
        #endregion
        #region 初始化
        public RestClient()
        {
            this.ContentType = "application/json";
            this.TimeOut = 3000;
        }
        public RestClient(string url)
        {
            this.Url = url;
            this.ContentType = "application/json";
            this.TimeOut = 3000;
        }
        #endregion
        #region Function
        public string HttpRequest(string postData = null)
        {
            var request = WebRequest.Create(Url) as HttpWebRequest;

            request.Timeout = this.TimeOut;
            request.ContentLength = 0;
            request.ContentType = this.ContentType;
            request.Method = "GET";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36";
            request.CookieContainer = new CookieContainer();
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip; //解壓Gzip Content Type
            request.AllowAutoRedirect = true;                                                                                    //request.ServicePoint.ConnectionLeaseTimeout = 0;

            //发送POST数据  
            if (!string.IsNullOrWhiteSpace(postData))
            {
                request.Method = "POST";
                byte[] data = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {

                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = string.Format("请求数据失败. 返回的 HTTP 状态码：{0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }
                return responseValue;
            }
        }
        #endregion
    }
}
