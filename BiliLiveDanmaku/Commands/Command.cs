using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace BiliLive.Commands
{
    public class Command
    {
        public CommandType CommandType { get; } = CommandType.UNKNOW;

        public string RawData { get; }

        protected JToken Json { get; }

        public Command(JToken json)
        {
            Json = json;
            RawData = Json?.ToString(Newtonsoft.Json.Formatting.None);
            CommandType = GetValue<CommandType>("cmd");
        }
        protected T GetValue<T>(params object[] keys)
        {
            return GetJTokenValue<T>(Json, keys);
        }

        protected T GetJTokenValue<T>(JToken json, params object[] keys)
        {
            if (json == null)
            {
                return default;
            }

            bool isOK = true;
            JToken current = json;
            foreach (var key in keys)
            {
                if (current.Type == JTokenType.Array)
                {
                    JArray arr = current as JArray;
                    if (key != null &&
                        int.TryParse(key.ToString(), out int index) &&
                        arr.Count > index)
                    {
                        current = arr[index];
                    }
                    else
                    {
                        //出错了
                        isOK = false;
                        break;
                    }
                }
                else if (current.Type == JTokenType.Object)
                {
                    JObject obj = current as JObject;
                    if (key != null &&
                        obj.ContainsKey(key.ToString()))
                    {
                        current = obj[key.ToString()];
                    }
                    else
                    {
                        //出错了
                        isOK = false;
                        break;
                    }
                }
                else 
                {
                    //出错了
                    isOK = false;
                    break;
                }
            }

            if (isOK)
            {
                try
                {
                    if (typeof(string).Equals(typeof(T)))
                    {
                        return (T)Convert.ChangeType(Regex.Unescape(current.ToString()), typeof(T));
                    }
                    return (T)Convert.ChangeType(current, typeof(T));
                }
                catch
                {
                    //尝试方法二
                    object obj = current;
                    try
                    {
                        return (T)obj;
                    }
                    catch
                    {
                        //出错了
                    }
                }
            }

            return default;

        }

    }

}
