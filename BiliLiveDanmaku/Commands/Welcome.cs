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
    public class Welcome : ICommand
    {
        public CommandType CommandType => CommandType.WELCOME;

        public string RawData { get; private set; }
        
        public User User { get; private set; }
        public bool Svip { get; private set; }

        public Welcome(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["uname"].ToString()));
            Svip = int.Parse(json["data"]["svip"].ToString()) != 0;
        }
    }
}
