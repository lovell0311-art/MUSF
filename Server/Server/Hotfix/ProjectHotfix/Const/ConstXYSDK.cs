using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public static class ConstDouYinSDK
    {
        public const int app_id = 516466;
        public const string secretKey = "lN5mPCAh2DZ1evvjIywfxXffLh0AbVYW";
        public const string AppCallbackUrl = "http://quanzhi.api.hjsk.cn:65001/api/Pay/PayInfoDY";
    }
    public static class ConstXYSDK
    {
        public const int Gid = 681;
        public const string Key = "00c69f30252b33534d13fee924061d12";
        public const string AppCallbackUrl = "http://120.24.45.40:15080/api/Pay/PayInfo";
    }
    public static class RealName
    {
        public const bool EnableOrNot = false;
        public const string appId = "ba653009f5154ac3ae6cb4ff38b843d7";
        public const string bizId = "1199021506";
        public const string Key = "264c011f9a0d493b178baa200200cba9";
        public const string AppCallbackUrl = "https://api.wlc.nppa.gov.cn/idcard/authentication/check";
        public const string LoginOut = "http://api2.wlc.nppa.gov.cn/behavior/collection/loginout";
        public const string QueryUrl = "http://api2.wlc.nppa.gov.cn/idcard/authentication/query";
        /// <summary>
        /// 实名认证接口说明 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="IdNum"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static async Task<string> RealNameRequest(string UserId, string IdNum, string Name)
        {
            // 请求参数
            string timestamps = Help_TimeHelper.GetNow().ToString();
            var pleyarRealNameInfo = new { ai = UserId, name = Name, idNum = IdNum };

            string requestBody = Help_JsonSerializeHelper.Serialize(pleyarRealNameInfo);
            //数据加密
            string Data = Encrypt(requestBody);
            var Info = new { data = Data };
            string NewData = Help_JsonSerializeHelper.Serialize(Info);
            string signString = Key + "appId" + appId + "bizId" + bizId + "timestamps" + timestamps + NewData;
            string sign = ComputeSHA256Hash(signString);

            try
            {
                using HttpClient client = new HttpClient();
                // 设置超时时间
                client.Timeout = TimeSpan.FromSeconds(5);
                // 设置请求头
                var request1 = new HttpRequestMessage(HttpMethod.Post, AppCallbackUrl);

                // 设置请求头
                request1.Headers.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                request1.Headers.TryAddWithoutValidation("appId", appId);
                request1.Headers.TryAddWithoutValidation("bizId", bizId);
                request1.Headers.TryAddWithoutValidation("timestamps", timestamps);
                request1.Headers.TryAddWithoutValidation("sign", sign);
                // 设置请求内容
                request1.Content = new StringContent(NewData, Encoding.UTF8, "application/json");

                // 发送请求
                HttpResponseMessage response1 = await client.SendAsync(request1);

                // 获取响应内容
                string responseBody = await response1.Content.ReadAsStringAsync();

                //Stream st = await response1.Content.ReadAsStreamAsync();
                //StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                //Console.WriteLine(reader.ReadToEnd());

                return responseBody;
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine($"请求发生异常：{ex.Message}");
            }
            return "";
        }
        /// <summary>
        /// 实名认证结果查询接口说明
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static async Task<string> AuthenticationAuery(string pi)
        {
            // 请求参数
            string timestamps = Help_TimeHelper.GetNow().ToString();

            //数据加密
            string signString = Key + "ai" + pi + "appId" + appId + "bizId" + bizId + "timestamps" + timestamps;
            string sign = ComputeSHA256Hash(signString);
            string requestUrl = $"{QueryUrl}?ai={pi}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 设置超时时间
                    client.Timeout = TimeSpan.FromSeconds(5);
                    //设置包头
                    client.DefaultRequestHeaders.Add("appId", appId);
                    client.DefaultRequestHeaders.Add("bizId", bizId);
                    client.DefaultRequestHeaders.Add("timestamps", timestamps);
                    client.DefaultRequestHeaders.Add("sign", sign);

                    // 发送请求
                    HttpResponseMessage response1 = await client.GetAsync(requestUrl);

                    // 获取响应内容
                    string responseBody = await response1.Content.ReadAsStringAsync();

                    //Stream st = await response1.Content.ReadAsStreamAsync();
                    //StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                    //Console.WriteLine(reader.ReadToEnd());

                    return responseBody;

                }
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine($"请求发生异常：{ex.Message}");
            }
            return "";
        }
        /// <summary>
        /// 游戏用户行为数据上报接口说明
        /// </summary>
        /// <param name="GameUserId"></param>
        /// <param name="Status"></param>
        /// <param name="Type"></param>
        /// <param name="Di"></param>
        /// <param name="Pi"></param>
        /// <returns></returns>
        public static async Task<string> LoginOrOut(string GameUserId, int Status, int Type, string Di, string Pi)
        {
            // 请求参数
            string timestamps = Help_TimeHelper.GetNow().ToString();
            Console.WriteLine(timestamps);
            List<BehaviorData> behaviorData = new List<BehaviorData>();
            BehaviorData behaviorData1 = new BehaviorData();
            behaviorData1.si = GameUserId; //游 戏内 部会 话标识
            behaviorData1.bt = Status;//游戏用户行为类型0：下线1：上线
            behaviorData1.ot = Help_TimeHelper.GetNowSecond();//行为发生时间戳，单位秒
            behaviorData1.ct = Type;//用户行为数据上报类型0：已认证通过用户2：游客用户
            behaviorData1.di = Di;//游客模式设备标识，由游戏运营单位生成，游客用户下必填
            behaviorData1.pi = Pi;//已通过实名认证用户的唯一标识，已认证通过用户必填
            behaviorData.Add(behaviorData1);
            var Body = new { collections = behaviorData };
            string requestBody = Help_JsonSerializeHelper.Serialize(Body);
            //数据加密
            string Data = Encrypt(requestBody);
            var Info = new { data = Data };
            string NewData = Help_JsonSerializeHelper.Serialize(Info);
            string signString = Key + "appId" + appId + "bizId" + bizId + "timestamps" + timestamps + NewData;

            string sign = ComputeSHA256Hash(signString);

            try
            {
                HttpClient client = new HttpClient();

                // 设置超时时间
                client.Timeout = TimeSpan.FromSeconds(5);

                // 构建 HttpRequestMessage 对象
                var request1 = new HttpRequestMessage(HttpMethod.Post, LoginOut);

                // 设置请求头
                request1.Headers.TryAddWithoutValidation("Content-Type", "application/json;charset=utf-8");
                request1.Headers.TryAddWithoutValidation("appId", appId);
                request1.Headers.TryAddWithoutValidation("bizId", bizId);
                request1.Headers.TryAddWithoutValidation("timestamps", timestamps);
                request1.Headers.TryAddWithoutValidation("sign", sign);

                // 设置请求内容
                request1.Content = new StringContent(NewData, Encoding.UTF8, "application/json");

                // 发送请求
                HttpResponseMessage response1 = await client.SendAsync(request1);

                // 获取响应内容
                string responseBody = await response1.Content.ReadAsStringAsync();

                //Stream st = await response1.Content.ReadAsStreamAsync();
                //StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                //Console.WriteLine(reader.ReadToEnd());
                return responseBody;
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine($"请求发生异常：{ex.Message}");
            }
            return "";
        }
        public static string ComputeSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "", StringComparison.CurrentCulture);
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            return returnBytes;
        }
        public static string Encrypt(string plain)
        {
            //1.将16进制的密钥转化为byte数组
            var key = StrToHexByte(Key);

            //2.将string格式的原文转化为byte数组
            var plaintext = Encoding.Default.GetBytes(plain);

            //2.1密文与原文等长
            var ciphertext = new byte[plaintext.Length];

            //3.随机数,给nonce赋值随机数
            Random rand = new Random();
            var nonce = new byte[12];
            var tag = new byte[16];
            rand.NextBytes(nonce);

            //4.加密
            using var aesGcm = new AesGcm(key);
            aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

            //5.将nonce（12位）拼在密文前，将tag（16位拼在密文后）
            var cipher = new byte[ciphertext.Length + 28];
            Array.ConstrainedCopy(nonce, 0, cipher, 0, 12);
            Array.ConstrainedCopy(ciphertext, 0, cipher, 12, ciphertext.Length);
            Array.ConstrainedCopy(tag, 0, cipher, 12 + ciphertext.Length, 16);

            return Convert.ToBase64String(cipher);
        }
    }

    public static class V4PayTopUp
    {
        public const string URL = "https://gateway.jxpays.com/pay.order/create";
        public const string method = "pay.order/create";
        public const string Mchid = "880810755891347";
        public const string PublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjrIgVjjEUNh4PW8akrqMzp37dsZdScJIXZKoLrzzm0zGFTnbAo+cLZGY/7ezi1FOxhBGgAXImaa6tzs7efo0hAGZIbj9kpN/VyYTfyRIOe2hAOPKOcvh2RYgHisFJtbNIWxDCDN4ekrW+AJ4oSQQWp3GBXlvJ3wLIbDzSkMOKcq2yUJRaMjLsmy7YxpeQq7tPjHNzS1SeEmh1HZCLg0bidvJ+zhHrRJI2rJyoA/onIOMhZmIT7rNwvWiIMyejGxHC5a+20KbBrjDthCEDzwWfBxva9v7dntvBcAFB+qk3z0+TxoW/Wnlr/1L1htOQ0NkEJ1G1PCGNShMcwrXLEL98QIDAQAB";
        public const string PrivateKey = "MIIEpgIBAAKCAQEAjrIgVjjEUNh4PW8akrqMzp37dsZdScJIXZKoLrzzm0zGFTnbAo+cLZGY/7ezi1FOxhBGgAXImaa6tzs7efo0hAGZIbj9kpN/VyYTfyRIOe2hAOPKOcvh2RYgHisFJtbNIWxDCDN4ekrW+AJ4oSQQWp3GBXlvJ3wLIbDzSkMOKcq2yUJRaMjLsmy7YxpeQq7tPjHNzS1SeEmh1HZCLg0bidvJ+zhHrRJI2rJyoA/onIOMhZmIT7rNwvWiIMyejGxHC5a+20KbBrjDthCEDzwWfBxva9v7dntvBcAFB+qk3z0+TxoW/Wnlr/1L1htOQ0NkEJ1G1PCGNShMcwrXLEL98QIDAQABAoIBAQCEV7QiA3gfuwSZhafRme7p2h/U4ti2hdfz4QbWgiw1RpkYKpZy45pnsPeDg26jsX8rtwCLz9Fin/3f3rGkyWdXXtwVKKsU/HNH8mp2qsHJ6BhA6QMvY2ZNnnWfRSr8AJAzhWMOAh6k3TBOyIv0d6wrPtTfUubCagsw7hpWL6a9Hn7ES5DzLFsiiNnMcl9obA7uxO+FicKFIT+2kYY70/cssKAwC1e47PF+kT1HFP42mLElNkkkmdp6pU/R3WQ0Vt3i7wiUhLAN24q5LfiRoJpVGBiLDfpeUuhtSobXThE6CqKPhEI7kyswBjwRRNQ8yFxl71bzkvEZrL0rWDa5gtQBAoGBANQF1p0cOeOkCkPrtRkGRvt+SKQ4GH9CP32tPWM9xwvC5BbwumIa42gTFe43vi5up9jIBdIOl5dUHXjMT0cur9PtbCXmAILho5uH821WFeIJUEZ42WeWd7DxyhhlXKJwfCoBdF0rG8rbbm0x4ed52g1NzUE+E0hkKdC0QfX1BPSRAoGBAKxLGxXxErEJyzqZzhf7N+ymYSvllpTDeviYh831nQWoZBTaNcIslXvGaYNnW5pFwIMR1xLTUvG+ywtFFhXMZEmzOcDlXbRMWtv+mhY6WmOQBoDCh8XvVxCnNWl9T3Zh3eTLOwaeozi2RyJnEMS2AEis8je7GCclYcBWfKegCqNhAoGBAIc/yi2iqdLq+5lkLLvv4yve3NvU5NuZGQNElZpO+EAqbHGt0lduq43iTuUNLgZUlEGXJw9eO5lPERXTzuarg4H/PDPYyo6y/TjjuocOmRr9sfWidZy8wVFgi/iQAE2mz63EC8S7ERmQbJq/bUnJ0y0Vak+qF4bgTkRSGJmbWTPhAoGBAKkmL1hSTgx7VgeZOom2vIfcLKghBb3VUKRBc4qTqO4GDQOf24lfpf5Xo/06+uaxtje3yxQTXmox6zuMPwt1l7v3dirXxDAfilBTXEmYTmdS3d+JnCyKNksGLdz8BohtuqBFmhR8qDgkVCAFWaQWHp87E639Q9Ai1h4zirwy1l7hAoGBALfxwhb9jdVMUjJLZEyA+jrce0eIJdQiJYuLA8UyuX6LueWbxiIFsgb6QVZA5DOGGDqUTP/4zrI9AdO13tU26O+if+HJTIKFa6Tx3PZaeikh8dsSUPUhcmNzsdbB+VSUqoQhzIV7oRbguQBYTkP1dWelA0kqqpkvkFW70zDUWiQZ";
        public const string V4Key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAigLUoKx2TmHwgI4beRwgp07q7QncR+BQcAs/j4dtYH0W9Gq3pKwhmEerHUozEXjWtGuT119AzGlIY8UnXA+NRX7h04OEm6tpLAl6yVo3gLeSYrKIyjXPIa4S3mCzApXrjx4/KWNcvX0xK9ly9iV3R0nCoYb9ZR/HttS03zt+9JQ3MVTXm9kBUDnC6A13lnDXKO54tBoRIXKANTxoVi7uSwuim5qFofh6AowKEwyBDEvVnburdDo7ZL7FL0ntglA2NfUF2Viyby0HNMmcvbl6fbm+z5jXLcK++RNJQZoiTtvej6S19eH38ihll0cdUqtibO5oi9xzLyjEJq/bTexdQQIDAQAB";
        public const string V4Md5Key = "d1dd64a1afe3e0143facd050c19e547c";
#if DEVELOP
        public const string AppCallbackUrl = "http://test.api.ieg123.com:15080/api/Pay/MyV4PayInfo";
#else
        public const string AppCallbackUrl = "http://dy2.api.ieg123.com:65012/api/Pay/MyV4PayInfo";
#endif

        public static Dictionary<string, string> ParseFormData(string formDataString)
        {
            var formData = new Dictionary<string, string>();

            string[] formFields = formDataString.Split(new string[] { "--------------------------" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var field in formFields)
            {
                string[] lines = field.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                string fieldName = null;
                string fieldValue = null;

                foreach (var line in lines)
                {
                    if (line.StartsWith("Content-Disposition: form-data;"))
                    {
                        string[] parts = line.Split(new string[] { "name=\"" }, StringSplitOptions.RemoveEmptyEntries);
                        fieldName = parts[1].Split('"')[0];
                        fieldValue = "";
                    }
                    else
                    {
                        fieldValue = line;
                    }
                }

                if (!string.IsNullOrEmpty(fieldName))
                {
                    formData[fieldName] = fieldValue;
                }
            }
            return formData;
        }
    }
    public static class MyPayTopUp
    {
        public const string URL = "https://openapi.alipay.com/gateway.do";
        public const string AppId = "2021004159608618";
        public const string AppPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAizTNYmhiwMX0TnbMPYFMGg8yrttICl4jHV7VhS4qIR0xK4VvGJRXBUj5pfVxmgdWuR7UMxzKuoyoIEP1mrHHwBSFGpUbPDINWHhkKI3GJHZ+tqSU0mEUJ/saqfOmo3GAGse42VzV0Bv3ZLwSVruUVFKilA/xOraqdyDwJ9tgQCH/xxAnUkhkbeUDwmCK9iFCoIreCgILCMDBb8yrBY8C7HtrqziSE+m1WB+u0fH1c4IyokAV/lXENPoUtcuIoQ2sJVSB+17/Yt3yTfJ5QWqQgni9D+YjUWd3o2tv+Ks3lgfNR23f/zNyxwqKIzhZ2VCPj7vrb137siWV7fEDp1RVPQIDAQAB";
        public const string PrivateKey = "MIIEogIBAAKCAQEAzXenjJvkPW1eVuF1L6tg8thJVgREZqyTUTLDHenmRbjIemP7Soy5FdFkdXlmFmjPwCtlyE7eI4z4FsVxqctaQvvMfwV2SkC4aljImszOnStjMWvbPFElwoUe2vUttwy4GNmVDkglkFuweuiPm4McILCdPhZM++LbJ6h4Q84donN1L70IHqxrjVIPqrRlB7IX9+BqO0ZHL/WuRSGcKXLCcA6fQMJX6WaMnIxX11UWfyxC5FSsZFVydAKfhzLteZIU10LHCm7syMOCJHOrIzRO4Bv9TWw1tbCFxZjm/I8b7NdtAfFEYDVoZ9sramVUMKyI21QpLRBSDvYWsMJ4bZtSmwIDAQABAoIBAFo6rY2F+BkxqjglEj042X5LlQj9HryoFJmX3S+Jw+HiX8e/mF8IpU77gU5FXZuBs5oEdNIeMFcUVO5LDrst3hmEUi2FzIiXG2U4UAAK6MEEiK7vWnuGdzqRExe8i7LTFxVyl46KCFT4ruuKrWXxkayZYrQdPvluiidESGweW802dJs1OqW7fkd4kOdNUjY0+AvEm+t36gIPn/3WN1RuTr6+Kk4xJkK05t8Ijv17wzme/2EPoWkb+BwRN8vOn4Gca0DDj1narkLiXFhMqVLG6BJMoS38UfTQOAYZyVYmCrlU5TXhjD/+teDtFSoN4VNUr7mtk/bj7lSCUGkV1MWggIECgYEA+aX5Kh7kinoUXkfbFh7F5CvokZ8Cq6KnyMkq0UHgV9kMJkpoBTM4XI7kP2hRg2L9sxYwffMrfFeBy7S2KkLzdaPxeGzy5wgiWxrj/Z5SuHBu/Epf+xXxKoDEtFtb2uP50iatfJ1XySW/Ly0sm+dLQsKR/FcUz/4ct3zlnMipXssCgYEA0rHrPO/AKH50YF78hOzFyos0zENr8UhoB5e7ioQug6okzFLdycO1V9UpBTO/kw/sKPpOJOZje9tzYRoyFe91smc9YZEeuuCuiZtJ/0maOU/cXazb6Vk+XuCeZ8mEp2eVbO7qhqvRpoUUox5GdZk0Re7wqfetinV24KYgaIwHEXECgYAw8QjX6RaEz0oO8adRvtas1K7TXYj4fPrHsihivbdtgW+QUiXyXwg9nQXCKIFScKWr5j27c2CjD8SyWssbneR8u5crNLCp+j5B/hOUNOWiougfLbWDU9njqzrk4MQxNWBUgqCgPXhoRq4kYYbNSZrHi71y6t95pyeaETIyemQ54QKBgEJPgKuzARU9hz71ZiAnrLBsU3eeGJDdqvAIzxtvMnx5xg8QXYEsuRwlmD1s5fRQ5JCZBpMSd2j5zjxXVEAXF0HIwEa+t581K3lfiFByR7mV6tMInkW4sqqFoxUjFT7imOKp5uHvnZH8/FvCIYbNaug/pTq5GsMi6QEXB8gfmfHhAoGAZFe7ngZNc/cYWX9AdbxUyHnM8JeK1v0t+wjAsd7+lp/5Gw1GoIe4w2t46NOQ3lkNJ8oMIUgkUWBbvz2uC8L8PqMc/mGkMVfnXaU1wIeAAf/27Hnsb1P9orQ3gh7Pt7RBXiSS+ZNsTYAopcH8hH/F96wF+Eufey9gJfp8piFFWHU=";
        public const string AppCallbackUrl = "http://pjzh.api.zzws.top:65001/api/Pay/MyPayInfo";
    }
}
