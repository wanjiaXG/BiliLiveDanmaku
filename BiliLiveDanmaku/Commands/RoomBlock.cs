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
    public class RoomBlock : ICommand
    {
        public CommandType CommandType => CommandType.ROOM_BLOCK_MSG;

        public User User { get; private set; }
        public uint Operator { get; private set; }

        public string RawData { get; private set; }
        

        public RoomBlock(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["uname"].ToString()));
            Operator = uint.Parse(json["data"]["operator"].ToString());
        }
    }
}
