using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    [Serializable]
    public class Welcome : Command
    {
        public override CommandType CommandType => CommandType.WELCOME;

        public uint UID { get; }
        public string Username { get; }
        public bool Svip { get; private set; }

        public Welcome(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            Svip = GetValue<int>("data", "svip") != 0;
        }
    }
}
