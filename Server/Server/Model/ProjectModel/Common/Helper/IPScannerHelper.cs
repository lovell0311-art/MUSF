using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ETModel
{
    public class IPScannerHelper
    {

        public static IPScannerHelper Instance = new IPScannerHelper("./qqwry.dat");

        private byte[] data;
        Regex regex = new Regex(@"^(1\d{2}|2[0-4]\d|25[0-5]|[1-9]\d|[1-9])(\.(1\d{2}|2[0-4]\d|25[0-5]|[1-9]\d|\d)){3}$");
        long firstStartIpOffset;
        long lastStartIpOffset;
        long ipCount;

        public long Count { get { return ipCount; } }

        public IPScannerHelper(string dataPath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (FileStream fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }
            byte[] buffer = new byte[8];
            Array.Copy(data, 0, buffer, 0, 8);
            firstStartIpOffset = ((buffer[0] + (buffer[1] * 0x100)) + ((buffer[2] * 0x100) * 0x100)) + (((buffer[3] * 0x100) * 0x100) * 0x100);
            lastStartIpOffset = ((buffer[4] + (buffer[5] * 0x100)) + ((buffer[6] * 0x100) * 0x100)) + (((buffer[7] * 0x100) * 0x100) * 0x100);
            ipCount = Convert.ToInt64((double)(((double)(lastStartIpOffset - firstStartIpOffset)) / 7.0));

            if (ipCount <= 1L)
            {
                throw new ArgumentException("ip FileDataError");
            }
        }
        private static long IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            if (ip.Split(separator).Length == 3)
            {
                ip = ip + ".0";
            }
            string[] strArray = ip.Split(separator);
            long num2 = ((long.Parse(strArray[0]) * 0x100L) * 0x100L) * 0x100L;
            long num3 = (long.Parse(strArray[1]) * 0x100L) * 0x100L;
            long num4 = long.Parse(strArray[2]) * 0x100L;
            long num5 = long.Parse(strArray[3]);
            return (((num2 + num3) + num4) + num5);
        }
        private static string IntToIP(long ip_Int)
        {
            long num = (long)(((ulong)ip_Int & 0xff000000L) >> 0x18);
            if (num < 0L)
            {
                num += 0x100L;
            }
            long num2 = (long)(((ulong)ip_Int & 0xff0000L) >> 0x10);
            if (num2 < 0L)
            {
                num2 += 0x100L;
            }
            long num3 = (long)(((ulong)ip_Int & 0xff00L) >> 8);
            if (num3 < 0L)
            {
                num3 += 0x100L;
            }
            long num4 = (long)((ulong)ip_Int & 0xffL);
            if (num4 < 0L)
            {
                num4 += 0x100L;
            }
            return (num.ToString() + "." + num2.ToString() + "." + num3.ToString() + "." + num4.ToString());
        }

        /// <summary>
        /// IP合法性检测
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>合法返回true，否则false</returns>
        public bool CheckIP(string ip)
        {
            return regex.Match(ip).Success;
        }

        public IPLocationInfo Query(string ip)
        {
            if (!CheckIP(ip))
            {
                ip = "300.300.300.300";
            }
            IPLocationInfo ipLocation = new IPLocationInfo() { IP = ip };
            long intIP = IpToInt(ip);
            if ((intIP >= IpToInt("127.0.0.1") && (intIP <= IpToInt("127.255.255.255"))))
            {
                ipLocation.Country = "本机内部环回地址";
                ipLocation.Local = "";
            }
            else
            {
                if ((((intIP >= IpToInt("0.0.0.0")) && (intIP <= IpToInt("2.255.255.255"))) || ((intIP >= IpToInt("64.0.0.0")) && (intIP <= IpToInt("126.255.255.255")))) ||
                ((intIP >= IpToInt("58.0.0.0")) && (intIP <= IpToInt("60.255.255.255"))))
                {
                    ipLocation.Country = "网络保留地址";
                    ipLocation.Local = "";
                }
            }
            long right = ipCount;
            long left = 0L;
            long middle = 0L;
            long startIp = 0L;
            long endIpOff = 0L;
            long endIp = 0L;
            int countryFlag = 0;
            while (left < (right - 1L))
            {
                middle = (right + left) / 2L;
                startIp = GetStartIp(middle, out endIpOff);
                if (intIP == startIp)
                {
                    left = middle;
                    break;
                }
                if (intIP > startIp)
                {
                    left = middle;
                }
                else
                {
                    right = middle;
                }
            }
            startIp = GetStartIp(left, out endIpOff);
            endIp = GetEndIp(endIpOff, out countryFlag);
            if ((startIp <= intIP) && (endIp >= intIP))
            {
                string local;
                ipLocation.Country = GetCountry(endIpOff, countryFlag, out local);
                //if (ipLocation.Country.Contains("山东"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("江苏"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("上海"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("浙江"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("安徽"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("福建"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("江西"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("广东"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("广西"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("海南"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("河南"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("湖南"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("湖北"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("北京"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("天津"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("河北"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("山西"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("内蒙古"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("宁夏"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("青海"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("陕西"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("甘肃"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("新疆"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("四川"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("贵州"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("云南"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("重庆"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("西藏"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("辽宁"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("吉林"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("黑龙江"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("香港"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("澳门"))
                //{
                //    ipLocation.Country = "中国";
                //}
                //if (ipLocation.Country.Contains("台湾"))
                //{
                //    ipLocation.Country = "中国";
                //}
                ipLocation.Local = local;
            }
            else
            {
                ipLocation.Country = "未知的IP地址";
                ipLocation.Local = "";
            }
            return ipLocation;
        }
        private long GetStartIp(long left, out long endIpOff)
        {
            long leftOffset = firstStartIpOffset + (left * 7L);
            byte[] buffer = new byte[7];
            Array.Copy(data, leftOffset, buffer, 0, 7);
            endIpOff = (Convert.ToInt64(buffer[4].ToString()) + (Convert.ToInt64(buffer[5].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[6].ToString()) * 0x100L) * 0x100L);
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) + (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
        }
        private long GetEndIp(long endIpOff, out int countryFlag)
        {
            byte[] buffer = new byte[5];
            Array.Copy(data, endIpOff, buffer, 0, 5);
            countryFlag = buffer[4];
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) + (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
        }
        /// <summary>
        /// Gets the country.
        /// </summary>
        /// <param name="endIpOff">The end ip off.</param>
        /// <param name="countryFlag">The country flag.</param>
        /// <param name="local">The local.</param>
        /// <returns>country</returns>
        private string GetCountry(long endIpOff, int countryFlag, out string local)
        {
            string country = "";
            long offset = endIpOff + 4L;
            switch (countryFlag)
            {
                case 1:
                case 2:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    offset = endIpOff + 8L;
                    local = (1 == countryFlag) ? "" : GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
                default:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    local = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
            }
            return country;
        }
        private string GetFlagStr(ref long offset, ref int countryFlag, ref long endIpOff)
        {
            int flag = 0;
            byte[] buffer = new byte[3];

            while (true)
            {
                //用于向前累加偏移量
                long forwardOffset = offset;
                flag = data[forwardOffset++];
                //没有重定向
                if (flag != 1 && flag != 2)
                {
                    break;
                }
                Array.Copy(data, forwardOffset, buffer, 0, 3);
                forwardOffset += 3;
                if (flag == 2)
                {
                    countryFlag = 2;
                    endIpOff = offset - 4L;
                }
                offset = (Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) + ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L);
            }
            if (offset < 12L)
            {
                return "";
            }
            return GetStr(ref offset);
        }
        private string GetStr(ref long offset)
        {
            byte lowByte = 0;
            byte highByte = 0;
            StringBuilder stringBuilder = new StringBuilder();
            byte[] bytes = new byte[2];
            Encoding encoding = Encoding.GetEncoding("GB2312");
            while (true)
            {
                lowByte = data[offset++];
                if (lowByte == 0)
                {
                    return stringBuilder.ToString();
                }
                if (lowByte > 0x7f)
                {
                    highByte = data[offset++];
                    bytes[0] = lowByte;
                    bytes[1] = highByte;
                    if (highByte == 0)
                    {
                        return stringBuilder.ToString();
                    }
                    stringBuilder.Append(encoding.GetString(bytes));
                }
                else
                {
                    stringBuilder.Append((char)lowByte);
                }
            }
        }
    }

    public class IPLocationInfo
    {
        public string IP { get; set; }
        public string Country { get; set; }
        public string Local { get; set; }
    }

    //public class RedirectMode
    //{
    //    public static readonly int Mode_1 = 1;
    //    public static readonly int Mode_2 = 2;
    //}

    //public class IPFormat
    //{
    //    public static readonly int HeaderLength = 8;
    //    public static readonly int IndexRecLength = 7;
    //    public static readonly int IndexOffset = 3;
    //    public static readonly int RecOffsetLength = 3;

    //    public static readonly string UnknownCountry = "未知的国家";
    //    public static readonly string UnknownZone = "未知的地区";

    //    public static uint ToUint(byte[] val)
    //    {
    //        if (val.Length > 4) throw new ArgumentException();
    //        if (val.Length < 4)
    //        {
    //            byte[] copyBytes = new byte[4];
    //            Array.Copy(val, 0, copyBytes, 0, val.Length);
    //            return BitConverter.ToUInt32(copyBytes, 0);
    //        }
    //        else
    //        {
    //            return BitConverter.ToUInt32(val, 0);
    //        }
    //    }
    //}

    //public class IPLocation
    //{
    //    private IPAddress m_ip;
    //    private string m_country;
    //    private string m_loc;

    //    public IPLocation(IPAddress ip, string country, string loc)
    //    {
    //        m_ip = ip;
    //        m_country = country;
    //        m_loc = loc;
    //    }

    //    public IPAddress IP
    //    {
    //        get { return m_ip; }
    //    }

    //    public string Country
    //    {
    //        get { return m_country; }
    //    }

    //    public string Zone
    //    {
    //        get { return m_loc; }
    //    }
    //}

    ///// <summary> 
    ///// This class used to control ip seek 
    ///// </summary> 
    //public class IPSeeker
    //{
    //    private string m_libPath;
    //    private uint m_indexStart;
    //    private uint m_indexEnd;
    //    public IPSeeker(string libPath)
    //    {
    //        m_libPath = libPath;
    //        //Locate the index block 
    //        using (FileStream fs = new FileStream(m_libPath, FileMode.Open, FileAccess.Read))
    //        {

    //            BinaryReader reader = new BinaryReader(fs);
    //            Byte[] header = reader.ReadBytes(IPFormat.HeaderLength);
    //            m_indexStart = BitConverter.ToUInt32(header, 0);
    //            m_indexEnd = BitConverter.ToUInt32(header, 4);

    //        }
    //    }

    //    /// <summary> 
    //    /// 输入IP地址，获取IP所在的地区信息 
    //    /// </summary> 
    //    /// <param name="ip">待查询的IP地址</param> 
    //    /// <returns></returns> 
    //    public IPLocation GetLocation(IPAddress ip)
    //    {
    //        using (FileStream fs = new FileStream(m_libPath, FileMode.Open, FileAccess.Read))
    //        {
    //            BinaryReader reader = new BinaryReader(fs);
    //            //Because it is network order(BigEndian), so we need to transform it into LittleEndian 
    //            Byte[] givenIpBytes = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ip.GetAddressBytes(), 0)));
    //            uint offset = FindStartPos(fs, reader, m_indexStart, m_indexEnd, givenIpBytes);
    //            return GetIPInfo(fs, reader, offset, ip, givenIpBytes);
    //        }
    //    }

    //    #region private method
    //    private uint FindStartPos(FileStream fs, BinaryReader reader, uint m_indexStart, uint m_indexEnd, byte[] givenIp)
    //    {
    //        uint givenVal = BitConverter.ToUInt32(givenIp, 0);
    //        fs.Position = m_indexStart;

    //        while (fs.Position <= m_indexEnd)
    //        {
    //            Byte[] bytes = reader.ReadBytes(IPFormat.IndexRecLength);
    //            uint curVal = BitConverter.ToUInt32(bytes, 0);
    //            if (curVal > givenVal)
    //            {
    //                fs.Position = fs.Position - 2 * IPFormat.IndexRecLength;
    //                bytes = reader.ReadBytes(IPFormat.IndexRecLength);
    //                byte[] offsetByte = new byte[4];
    //                Array.Copy(bytes, 4, offsetByte, 0, 3);
    //                return BitConverter.ToUInt32(offsetByte, 0);
    //            }
    //        }
    //        return 0;
    //    }

    //    private IPLocation GetIPInfo(FileStream fs, BinaryReader reader, long offset, IPAddress ipToLoc, Byte[] ipBytes)
    //    {
    //        fs.Position = offset;
    //        //To confirm that the given ip is within the range of record IP range 
    //        byte[] endIP = reader.ReadBytes(4);
    //        uint endIpVal = BitConverter.ToUInt32(endIP, 0);
    //        uint ipVal = BitConverter.ToUInt32(ipBytes, 0);
    //        if (endIpVal < ipVal) return null;

    //        string country;
    //        string zone;
    //        //Read the Redirection pattern byte 
    //        Byte pattern = reader.ReadByte();
    //        if (pattern == RedirectMode.Mode_1)
    //        {
    //            Byte[] countryOffsetBytes = reader.ReadBytes(IPFormat.RecOffsetLength);
    //            uint countryOffset = IPFormat.ToUint(countryOffsetBytes);

    //            if (countryOffset == 0) return GetUnknownLocation(ipToLoc);

    //            fs.Position = countryOffset;
    //            if (fs.ReadByte() == RedirectMode.Mode_2)
    //            {
    //                return ReadMode2Record(fs, reader, ipToLoc);
    //            }
    //            else
    //            {
    //                fs.Position--;
    //                country = ReadString(reader);
    //                zone = ReadZone(fs, reader, Convert.ToUInt32(fs.Position));
    //            }
    //        }
    //        else if (pattern == RedirectMode.Mode_2)
    //        {
    //            return ReadMode2Record(fs, reader, ipToLoc);
    //        }
    //        else
    //        {
    //            fs.Position--;
    //            country = ReadString(reader);
    //            zone = ReadZone(fs, reader, Convert.ToUInt32(fs.Position));
    //        }
    //        return new IPLocation(ipToLoc, country, zone);

    //    }

    //    //When it is in Mode 2 
    //    private IPLocation ReadMode2Record(FileStream fs, BinaryReader reader, IPAddress ip)
    //    {
    //        uint countryOffset = IPFormat.ToUint(reader.ReadBytes(IPFormat.RecOffsetLength));
    //        uint curOffset = Convert.ToUInt32(fs.Position);
    //        if (countryOffset == 0) return GetUnknownLocation(ip);
    //        fs.Position = countryOffset;
    //        string country = ReadString(reader);
    //        string zone = ReadZone(fs, reader, curOffset);
    //        return new IPLocation(ip, country, zone);
    //    }

    //    //return a Unknown Location 
    //    private IPLocation GetUnknownLocation(IPAddress ip)
    //    {
    //        string country = IPFormat.UnknownCountry;
    //        string zone = IPFormat.UnknownZone;
    //        return new IPLocation(ip, country, zone);
    //    }
    //    //Retrieve the zone info 
    //    private string ReadZone(FileStream fs, BinaryReader reader, uint offset)
    //    {
    //        fs.Position = offset;
    //        byte b = reader.ReadByte();
    //        if (b == RedirectMode.Mode_1 || b == RedirectMode.Mode_2)
    //        {
    //            uint zoneOffset = IPFormat.ToUint(reader.ReadBytes(3));
    //            if (zoneOffset == 0) return IPFormat.UnknownZone;
    //            return ReadZone(fs, reader, zoneOffset);
    //        }
    //        else
    //        {
    //            fs.Position--;
    //            return ReadString(reader);
    //        }
    //    }

    //    private string ReadString(BinaryReader reader)
    //    {
    //        List<byte> stringLst = new List<byte>();
    //        byte byteRead = 0;
    //        while ((byteRead = reader.ReadByte()) != 0)
    //        {
    //            stringLst.Add(byteRead);
    //        }
    //        return Encoding.GetEncoding("gb2312").GetString(stringLst.ToArray());
    //    }
    //    #endregion
    //}

}
