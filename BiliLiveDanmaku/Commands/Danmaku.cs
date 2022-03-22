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

        //public DateTime TimeStamp { get; private set; }

        public uint UID { get; }
        public string Username { get; }

        public string Message { get; private set; }

        public Danmaku(JToken json) : base(json)
        {

            UID = GetValue<uint>("info", 2, 0);
            Username = GetValue<string>("info", 2, 1);
            Message = GetValue<string>("info", 1);
            //TimeStamp = GetTimeStamp(GetValue<long>("info", 0, 4));
        }
    }
}
