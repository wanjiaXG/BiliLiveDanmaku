using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

//已检查无误
namespace BiliLive
{
    public class Util
    {
        public static T GetJTokenValue<T>(JToken json,  params object[] keys)
        {
            if (json == null)
            {
                return default;
            }

            if(keys == null || keys.Length <= 0)
            {
                return ChangeType<T>(json);
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
                return ChangeType<T>(current, default);
            }

            return default;

        }

        public static T ChangeType<T>(object obj) => ChangeType<T>(obj, default);

        public static T ChangeType<T>(object obj, T dvalue)
        {
            if (obj != null)
            {
                try
                {
                    if (typeof(string).Equals(typeof(T)))
                    {
                        return (T)Convert.ChangeType(Regex.Unescape(obj.ToString()), typeof(T));
                    }

                    return (T)Convert.ChangeType(obj, typeof(T));
                }
                catch
                {
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
            return dvalue;
        }


    }
}
