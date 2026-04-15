
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using static ETModel.SMSMessageComponent;

using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;
using TencentCloud.Faceid.V20180301;
using TencentCloud.Faceid.V20180301.Models;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ETHotfix
{

    public static class SMSMessageComponentSystem
    {
        /// <summary>
        /// 验证短信
        /// </summary>
        /// <param name="self"></param>
        /// <param name="b_Phone"></param>
        /// <param name="b_Code"></param>
        /// <returns></returns>
        public static bool Verify(this SMSMessageComponent self, ESMSCodeType b_CodeType, string b_Phone, string b_Code)
        {
            if (self.IsVerify == 0) return true;

            if (self.SmsCodeDict[(int)b_CodeType].TryGetValue(b_Phone, out (string, long) value))
            {
                if (value.Item1 != b_Code) return false;

                self.SmsCodeDict[(int)b_CodeType].Remove(b_Phone);
                return true;
            }
            return false;
        }

        public static async Task<string> CreateCode(this SMSMessageComponent self, ESMSCodeType b_CodeType, string b_Phone, string b_CN = "+86")
        {
            string code = new Random().Next(1000, 9999).ToString();

            string mCN_Phone = $"{b_CN}{b_Phone}";
            List<string> mSendResult = await self.SendSms(b_CodeType, new string[] { mCN_Phone }, new string[] { code });
            //List<string> mSendResult = new List<string>();// await self.SendSms(new string[] { mCN_Phone }, new string[] { code });
            if (mSendResult != null)
            {
                self.SaveCode(b_CodeType, b_Phone, code).Coroutine();
                return mCN_Phone;
            }
            return null;
        }
        public static async Task<List<string>> CreateCode(this SMSMessageComponent self, ESMSCodeType b_CodeType, string[] b_Phones, string b_CN = "+86")
        {
            string[] Phones = new string[b_Phones.Length];
            string[] codes = new string[b_Phones.Length];
            for (int i = 0, len = b_Phones.Length; i < len; i++)
            {
                string code = new Random().Next(1000, 9999).ToString();
                Phones[i] = $"{b_CN}{b_Phones[i]}";
                codes[i] = code;
            }

            List<string> mresult;
            List<string> mSendResult = await self.SendSms(b_CodeType, Phones, codes);
            if (mSendResult != null)
            {
                mresult = new List<string>();

                for (int j = 0, jlen = mSendResult.Count; j < jlen; j++)
                {
                    string mSavePhone = mSendResult[j];

                    for (int i = 0, len = Phones.Length; i < len; i++)
                    {
                        string mPhone = Phones[i];
                        if (mPhone == mSavePhone)
                        {
                            string code = codes[i];
                            mresult.Add(mPhone);
                            self.SaveCode(b_CodeType, mPhone, code).Coroutine();
                        }
                    }
                }
                return mresult;
            }
            else
            {

            }
            return null;
        }

        private static async Task SaveCode(this SMSMessageComponent self, ESMSCodeType b_CodeType, string b_Phone, string b_Code)
        {

            long instanceId = self.Id;
            long codeId = IdGeneraterNew.Instance.GenerateId();
            self.SmsCodeDict[(int)b_CodeType][b_Phone] = (b_Code, codeId);
            await ETModel.ET.TimerComponent.Instance.WaitAsync(1000 * 60 * 30);
            if (self.Id != instanceId) return;
            if (self.SmsCodeDict[(int)b_CodeType].TryGetValue(b_Phone, out (string, long) value))
            {
                if (value.Item2 == codeId)
                {
                    // 还是原来的哪个验证码，到期删除
                    self.SmsCodeDict[(int)b_CodeType].Remove(b_Phone);
                }
            }

        }


        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }


        public static async Task<SFZYZ2class> SFZYZ2(this SMSMessageComponent self, string b_Phone, string b_Name)
        {
            try
            {
                string host = "https://jmidcardv1.market.alicloudapi.com";
                string path = "/idcard/validate";
                string method = "POST";
                string appcode = "a525ee9c98ee4d68b883742d1177aaca";

                string querys = "";
                string bodys = $"idCardNo={b_Phone}&name={b_Name}";
                string url = host + path;
                HttpWebRequest httpRequest = null;
                HttpWebResponse httpResponse = null;

                if (0 < querys.Length)
                {
                    url = url + "?" + querys;
                }

                if (host.Contains("https://"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                }
                else
                {
                    httpRequest = (HttpWebRequest)WebRequest.Create(url);
                }
                httpRequest.Method = method;
                httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
                //根据API的要求，定义相对应的Content-Type
                httpRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
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

                //Console.WriteLine(httpResponse.StatusCode);
                //Console.WriteLine(httpResponse.Method);
                //Console.WriteLine(httpResponse.Headers);
                Stream st = httpResponse.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                //Console.WriteLine(reader.ReadToEnd());
                //Console.WriteLine("\n");

                string mResultStr = reader.ReadToEnd();

                var mResult = Help_JsonSerializeHelper.DeSerialize<SFZYZ2class>(mResultStr);
               
                return mResult;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return null;
        }
        public static async Task<bool> SFZYZ(this SMSMessageComponent self, string b_Phone, string b_Name)
        {
            try
            {
                Credential cred = new Credential()
                {
                    SecretId = "AKID3y8BVmnUdi6IfF1lUKb1r2tR8Xd2kDb6WlW",
                    SecretKey = "c2Uk8Y0YFzjGEKyI7O5y2j6h58lA2LZnhXAtS7O"
                };

                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("faceid.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                FaceidClient client = new FaceidClient(cred, "", clientProfile);
                IdCardOCRVerificationRequest req = new IdCardOCRVerificationRequest();
                req.IdCard = "370481199511284231";
                req.Name = "刘航洋";
                IdCardOCRVerificationResponse resp = client.IdCardOCRVerificationSync(req);

                string mResultStr = AbstractModel.ToJsonString(resp);

                Console.WriteLine(mResultStr);
                return true;
            }
            catch (TencentCloudSDKException e)
            {
                Log.Error(e);
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e);

                return false;
                throw;
            }
        }

        public static async Task<List<string>> SendSms(this SMSMessageComponent self, ESMSCodeType b_CodeType, string[] b_Phone, string[] b_Code)
        {
            try
            {
                List<string> mResult_Phone = new List<string>();
                // 实例化一个认证对象，入参需要传入腾讯云账户secretId，secretKey,此处还需注意密钥对的保密
                // 密钥可前往https://console.cloud.tencent.com/cam/capi网站进行获取
                Credential cred = new Credential()
                {
                    SecretId = "AKIDb1B4g27vSiiRs2xby4uTlmW1FCsHPFo7",
                    SecretKey = "3qMina6LJzPXP0JL7cqmRpwmKccLn0vZ"
                };

                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("sms.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                SmsClient client = new SmsClient(cred, "ap-guangzhou", clientProfile);
                SendSmsRequest req = new SendSmsRequest();
                req.SmsSdkAppId = "1400603263";
                req.SignName = "欢聚时刻网络";
                switch (b_CodeType)
                {
                    case ESMSCodeType.Register:
                        req.TemplateId = "1278593";
                        break;
                    case ESMSCodeType.ResetPasswd:
                        req.TemplateId = "1278592";
                        break;
                    default:
                        return null;
                }
                req.PhoneNumberSet = b_Phone;
                req.TemplateParamSet = b_Code;
                SendSmsResponse resp = client.SendSmsSync(req);
                string mResultStr = AbstractModel.ToJsonString(resp);
                SendResult sendResult = Newtonsoft.Json.JsonConvert.DeserializeObject<SendResult>(mResultStr);

                //Console.WriteLine(mResultStr);

                for (int i = 0, len = sendResult.SendStatusSet.Count; i < len; i++)
                {
                    var mStatusResult = sendResult.SendStatusSet[i];

                    switch (mStatusResult.Code)
                    {
                        case "Ok":
                            {
                                mResult_Phone.Add(mStatusResult.PhoneNumber);
                            }
                            break;
                        default:
                            {

                            }
                            break;
                    }
                }

                await Task.CompletedTask;

                return mResult_Phone.Count > 0 ? mResult_Phone : null;
            }
            catch (TencentCloudSDKException e)
            {
                Log.Error(e);
                return null;
            }
        }

        public static async Task<bool> SendSmsHttp(this SMSMessageComponent self, string b_Phone)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan toNow = DateTime.UtcNow.Subtract(dtStart);
            string timeStamp = toNow.Ticks.ToString();
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            
            string contentTemp = "您的注册验证码: {0}, 如非本人操作,请忽略本短信!";
            string code = new Random().Next(1000, 9999).ToString();
            string contentStr = string.Format(contentTemp, code);
            string messageStr = $"Action=SendSms"
                            + $"&Version=2019-07-11"
                            + $"&PhoneNumberSet.0=+86{b_Phone}"
                            + $"&TemplateID={1278593.ToString()}"
                            + $"&SmsSdkAppid={"1400603263"}"
                            + $"&Sign={"欢聚时刻网络"}"
                            + $"&TemplateParamSet.0={code}"
                            + $"&Timestamp={timeStamp}"
                            + $"&Nonce=1533"
                            + $"&SecretId=AKIDb1B4g27vSiiRs2xby4uTlmW1FCsHPFo7";



            //byte[] data = Encoding.GetEncoding("GB2312").GetBytes(messageStr);

            string url = "https://sms.tencentcloudapi.com";
            HttpClient mHttpClient = new HttpClient();
            mHttpClient.BaseAddress = new Uri(url);
            mHttpClient.Timeout = new TimeSpan(0, 0, 0, 3);
            string result = "";
            try
            {
                StringContent json = new StringContent(messageStr, Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage httpResponseMessage = await mHttpClient.PostAsync(url, json);
                //byte[] buffer = await httpResponseMessage.Content.ReadAsByteArrayAsync();
                result = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Log.Error("SendInland False:" + result);
                Log.Error(e);
                return false;
            }

            if (result != "")
            {
                Log.Debug(result);
            }
            return true;
        }
    }

}