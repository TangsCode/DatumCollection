using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DatumCollection.ImageRecognition
{
    /// <summary>
    /// Alibaba Cloud OCR
    /// </summary>
    public class AliCloudOCR : IRecognizer
    {
        private const String host = "https://ocrapi-advanced.taobao.com";
        private const String path = "/ocrservice/advanced";
        private const String method = "POST";
        private const String appcode = "3b1db8fd14fb4cfca0502d6cb16038fa";

        public string Recognize(string imageUrl)
        {
            String querys = "";

            String bodys = "{\"img\":\"\",\"url\":\"" +
                imageUrl +
                "\",\"prob\":false,\"charInfo\":false,\"rotate\":false,\"table\":false}";
            String url = host + path;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }

            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CertificateValidation);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
            //根据API的要求，定义相对应的Content-Type
            httpRequest.ContentType = "application/json; charset=UTF-8";
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }

            if(httpResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream st = httpResponse.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                string ret = reader.ReadToEnd();
                return ret;
            }

            return null;
        }

        private bool CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
