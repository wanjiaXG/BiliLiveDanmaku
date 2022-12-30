using BiliLive.Commands.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

//已检查无运行异常
namespace BiliLive.Commands
{
    public class Command
    {
        private static Dictionary<uint, string> Faces = new Dictionary<uint, string>();

        public static string GetFace(uint id)
        {
            string result;
            if (!Faces.ContainsKey(id))
            {
                result = GetFaceOnNet(id);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    Faces[id] = result;
                }
            }
            else
            {
                result = Faces[id];
            }
            return result;
        }

        private static string GetFaceOnNet(uint id)
        {
            //https://api.bilibili.com/x/space/wbi/acc/info?mid=155977074
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                    client.Headers["Accept-Encoding"] = "gzip, deflate, br";
                    client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36";
                    byte[] data = client.DownloadData($"https://api.bilibili.com/x/space/wbi/acc/info?mid={id}");
                    using(MemoryStream ms = new MemoryStream(data))
                    {
                        using(GZipStream gzs = new GZipStream(ms, CompressionMode.Decompress))
                        {
                            StreamReader reader = new StreamReader(gzs, Encoding.UTF8);
                            var result = JObject.Parse(reader.ReadToEnd());
                            return Util.GetJTokenValue<string>(result, "data", "face");
                        }
                    }
                    
                    
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public CommandType CommandType { get; } = CommandType.UNKNOW;

        public string RawData { get; }

        protected JToken Json { get; }

        public Command(JToken json)
        {
            Json = json ?? new JObject();
            RawData = Json.ToString(Newtonsoft.Json.Formatting.None);
            CommandType = GetValue<CommandType>("cmd");
        }

        protected T GetValue<T>(params object[] keys)
        {
            T obj = Util.GetJTokenValue<T>(Json, keys);

            Type type = typeof(T);
            if (obj == null)
            {
                if (type.Equals(typeof(string)))
                {
                    return (T)((object)string.Empty);
                }
                else if (type.Equals(typeof(CommandType)))
                {
                    return (T)((object)CommandType.UNKNOW);
                }
                else if (type.Equals(typeof(Identities)))
                {
                    return (T)((object)Identities.Unknown);
                }
                else if (type.Equals(typeof(MessageTypes)))
                {
                    return (T)((object)MessageTypes.Unknown);
                }
                else if (type.Equals(typeof(JObject)))
                {
                    return (T)((object)new JObject());
                }
                else if (type.Equals(typeof(JArray)))
                {
                    return (T)((object)new JArray());
                }
            }

            return obj;

        }
    }

}
