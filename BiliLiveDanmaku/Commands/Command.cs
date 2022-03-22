using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class Command
    {
        public virtual CommandType CommandType { get; } = CommandType.UNKNOW;

        public string RawData { get; }

        protected JToken Json { get; }

        public Command(JToken json)
        {
            Json = json;
            RawData = Json?.ToString(Newtonsoft.Json.Formatting.None);
        }
        protected T GetValue<T>(params object[] keys)
        {
            return GetValue<T>(Json, keys);
        }

        protected T GetValue<T>(JToken json, params object[] keys)
        {
            if (json == null)
            {
                return default;
            }

            bool isOK = true;
            JToken current = json;
            foreach (var key in keys)
            {
                if (current is JArray arr)
                {
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
                else if (current is JObject obj)
                {
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
                    //出错了
                }
            }

            return default;

        }


        protected DateTime GetTimeStamp(double time)
        {
            return new DateTime(1970, 01, 01).AddMilliseconds(time);
        }

    }

}
