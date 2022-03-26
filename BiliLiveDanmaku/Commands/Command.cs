using BiliLive.Commands.Enums;
using Newtonsoft.Json.Linq;
using System;

//已检查无运行异常
namespace BiliLive.Commands
{
    public class Command
    {
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

            if(obj == null)
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
