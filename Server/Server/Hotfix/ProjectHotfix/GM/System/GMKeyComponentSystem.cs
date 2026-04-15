using Aop.Api.Domain;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    [EventMethod(typeof(GMKeyComponent), EventSystemType.INIT)]
    public class GMKeyComponentSystemEventOnInit : ITEventMethodOnInit<GMKeyComponent>
    {
        public void OnInit(GMKeyComponent b_Component)
        {
            b_Component.OnInit().Coroutine();
        }
    }
    public static class GMKeyComponentSystem
    {
        public static async Task OnInit(this GMKeyComponent self)
        {
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            await mTimerComponent.WaitAsync(10000);
            self.gMToolKey = new GMToolKey();
            self.AccountDic = new Dictionary<string, DBGMToolAccount>();
            self.Index = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, 0);
            if (mDBProxy == null) return;

            var InfoList = await mDBProxy.Query<GMToolKey>(p => p.Id == 111111);
            if (InfoList == null) return;

            if (InfoList.Count >= 1)
            {
                self.gMToolKey = InfoList[0] as GMToolKey;
            }
            
        }
        public static async Task SetInitKey(this GMKeyComponent self,string Key)
        {
            self.gMToolKey = new GMToolKey();
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, 0);
            if (mDBProxy == null) return;

            self.gMToolKey.Key = Key;
            long Id = 111111;
            self.gMToolKey.Id = Id;
            var InfoList = await mDBProxy.Query<GMToolKey>(p => p.Id == 111111);
            if (InfoList == null) return;

            await mDBProxy.Save(self.gMToolKey);
        }
        public static void Add(this GMKeyComponent self, string AccountKey,DBGMToolAccount dBGMToolAccount)
        {
            var List = new Dictionary<string, DBGMToolAccount>(self.AccountDic);
            foreach (var Info in List)
            {
                if (Info.Value.Account == dBGMToolAccount.Account)
                {
                    self.AccountDic.Remove(Info.Key);
                }
            }
            self.AccountDic.Add(AccountKey, dBGMToolAccount);
        }
        public static string GetKey(this GMKeyComponent self)
        {
            string Key;
            do
            {
                Random random = new Random();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 11; i++)
                {
                    int index = random.Next(self.Index.Length);
                    char c = self.Index[index];
                    sb.Append(c);
                }
                Key = sb.ToString();
            } while (self.AccountDic.ContainsKey(Key));
            return Key;
        }
        public static bool CheckLevel(this GMKeyComponent self,string Behavior, string AccountKey)
        {
            if (self.AccountDic.TryGetValue(AccountKey, out var Info))
            {
                if (Info.AccountLevel == 0) return false;

                switch (Behavior)
                {
                    case "GetEquipmentInfo":
                    case "GetPlayerEquipment":
                    case "Kick":
                    case "SetPlayerInfo":
                    case "SearchPlayerInfo":
                    case "GetPlayerInfo": 
                    case "GetPlayer":
                    case "UpDataIdCard":
                    case "UpDataPassword":
                    case "EditAccount":
                    case "SearchAccount":
                    case "ViewPlayer": if (Info.AccountLevel <= 1) return true; else return false;
                    
                }
            }
            return false;
        }
        /// <summary>
        /// AES解密操作
        /// </summary>
        /// <param name="self"></param>
        /// <param name="decryptStr"></param>
        /// <returns></returns>
        public static string Decrypt(this GMKeyComponent self,string decryptStr)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(self.gMToolKey.Key);

                // 仅使用前16字节作为密钥
                aesAlg.Key = keyArray.Take(16).ToArray();

                // 根据需要调整IV
                aesAlg.IV = new byte[16];

                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] toDecryptArray = Convert.FromBase64String(decryptStr);

                using (MemoryStream msDecrypt = new MemoryStream(toDecryptArray))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// AES加密操作
        /// </summary>
        /// <param name="self"></param>
        /// <param name="encryptStr"></param>
        /// <returns></returns>
        public static string Encrypt(this GMKeyComponent self,string encryptStr)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(self.gMToolKey.Key);

                // 仅使用前16字节作为密钥
                aesAlg.Key = keyArray.Take(16).ToArray();

                // 根据需要调整IV
                aesAlg.IV = new byte[16];

                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(encryptStr);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static Dictionary<string, string> GetRequest(this GMKeyComponent self,string Msg)
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                Msg = self.Decrypt(Msg);
                Dictionary<string, string> data = Msg.Split('&').Select(item => item.Split('=')).ToDictionary(parts => parts[0], parts => parts[1]);
                return data;
            }
            return new Dictionary<string, string>();
        }
    }

}
