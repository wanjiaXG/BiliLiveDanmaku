using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiliLive
{
    public class Util
    {
        public static T GetJTokenValue<T>(JToken json, params object[] keys)
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
