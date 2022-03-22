using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class Danmaku : Command
    {
        public override CommandType CommandType => CommandType.DANMU_MSG;

        public DateTime TimeStamp { get; private set; }

        public uint UID { get; }
        public string Username { get; }

        public string Message { get; private set; }

        public uint Type { get; private set; }

        public Danmaku(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            Message = GetValue<string>("info", 1);
            Type = GetValue<uint>("info", 0, 9);
            TimeStamp = GetTimeStamp(GetValue<double>("info", 0, 4));
        }
    }
}
