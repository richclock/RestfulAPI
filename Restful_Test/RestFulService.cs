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
using System;


/************************************************************************************
* Autor：clock
* Email：clarkliao@itri.org.tw
* Version：V1.0.0.0
* CreateTime：2019/3/25 13:36:40
* Description：
* Company：ITRI
* Copyright © 2018  All Rights Reserved
************************************************************************************/
namespace ITRI
{
    [ServiceContract(Name = "RestFulService")]
    public interface IRestServer
    {
        [OperationContract]
        [WebGet(UriTemplate = "api/MachineTool", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        System.IO.Stream Get();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "api/MachineTool", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        System.IO.Stream Post(string value);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestServer : ApiController, IRestServer
    {
        public RestServer()
        {

        }
        public Stream Get()
        {
            string ret = new JavaScriptSerializer().Serialize(new {
                error = "Restful Get Test"
            });

            byte[] resultBytes = Encoding.UTF8.GetBytes(ret);

       
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            return new MemoryStream(resultBytes);


        }
        public Stream Post(string value)
        {
            string ret = new JavaScriptSerializer().Serialize(new {
                error = "Error"
            });

            byte[] resultBytes = Encoding.UTF8.GetBytes(ret);


            WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            return new MemoryStream(resultBytes);


        }
    }

    public class RestClient
    {
        #region 成員變數
        public string Url { get; set; }
        public string ContentType { get; private set; }
        #endregion
        #region 初始化
        public RestClient()
        {
            this.ContentType = "application/json";
        }
        public RestClient(string url)
        {
            this.Url = url;
            this.ContentType = "application/json";
        }
        #endregion
        #region Function
        public string HttpRequest(string postData = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);

            
            request.ContentLength = 0;
            request.ContentType = this.ContentType;
            request.Method = "GET";
            if (!string.IsNullOrEmpty(postData)) {
                request.Method = "POST";
                var bytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream()) {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse()) {

                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK) {
                    var message = string.Format("请求数据失败. 返回的 HTTP 状态码：{0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                using (var responseStream = response.GetResponseStream()) {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream)) {
                            responseValue = reader.ReadToEnd();
                        }
                }
                return responseValue;
            }
        }
        #endregion
    }
}
