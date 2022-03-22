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
    public class RoomBlock : Command
    {
        public override CommandType CommandType => CommandType.ROOM_BLOCK_MSG;

        public uint UID { get; }
        public string Username { get; }

        public uint Operator { get; private set; }

        public RoomBlock(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            Operator = GetValue<uint>("data", "operator");
        }
    }
}
