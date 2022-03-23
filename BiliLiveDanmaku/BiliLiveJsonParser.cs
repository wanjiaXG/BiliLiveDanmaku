using BiliLive.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BiliLive
{
    public class BiliLiveJsonParser
    {
        private BiliLiveJsonParser() { }

        private static readonly Dictionary<string, ConstructorInfo> CommandConstructors;

        static BiliLiveJsonParser()
        {
            CommandConstructors = new Dictionary<string, ConstructorInfo>();
            foreach(MemberInfo info in typeof(CommandType).GetMembers())
            {
                if(info.GetCustomAttribute<CommandAttribute>() is CommandAttribute attr)
                {
                    CommandConstructors.Add(info.Name, 
                        attr.Type.GetConstructor(new Type[] { typeof(JToken) }));
                }
            }
        }

        public static Command Parse(JToken json) { 
        
            if(json.Type == JTokenType.Object && 
                json is JObject obj && 
                obj.ContainsKey("cmd") &&
                CommandConstructors.ContainsKey(json["cmd"].ToString()))
            {
                try
                {
                    return (Command)CommandConstructors[json["cmd"].ToString()].Invoke(new object[] { json });
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
