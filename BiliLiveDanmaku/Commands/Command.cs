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
            return Util.GetJTokenValue<T>(Json, keys);
        }

        

    }

}
