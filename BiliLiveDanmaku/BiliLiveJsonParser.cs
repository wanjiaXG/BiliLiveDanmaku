using BiliLive.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace BiliLive
{
    public class BiliLiveJsonParser
    {
        public static Command Parse(JToken json) { 
        
            if(json.Type == JTokenType.Object && 
                json is JObject obj && 
                obj.ContainsKey("cmd"))
            {
                try
                {
                    if (typeof(CommandType).GetMember(json["cmd"].ToString()) is MemberInfo[] infos &&
                        infos.Length > 0 &&
                        infos[0].GetCustomAttribute<CommandAttribute>() is CommandAttribute attr)
                    {
                        ConstructorInfo constructor = attr.Type.GetConstructor(new Type[] { typeof(JToken) });
                        return constructor.Invoke(new object[] { json }) as Command;
                    }
                }
                catch
                {
                    //出错了
                }
            }
            return new Command(json);
        }
    }
}
