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
    public class ComboSend : ICommand
    {
        public User User { get; private set; }
        public string GiftName { get; private set; }
        public uint Number { get; private set; }
        public uint GiftId { get; private set; }
        public string Action { get; private set; }

        public string RawData { get; private set; }

        public CommandType CommandType => CommandType.COMBO_SEND;

        public ComboSend(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["uname"].ToString()));
            GiftName = Regex.Unescape(json["data"]["gift_name"].ToString());
            Number = uint.Parse(json["data"]["total_num"].ToString());
            GiftId = uint.Parse(json["data"]["gift_id"].ToString());
            Action = json["data"]["action"].ToString();
        }
    }
}
