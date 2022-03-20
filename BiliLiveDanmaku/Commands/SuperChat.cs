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
    public class SuperChat : ITimeStampedCommand
    {
        public CommandType CommandType => CommandType.SUPER_CHAT_MESSAGE;

        public DateTime TimeStamp { get; private set; }

        public uint Price { get; private set; }
        public string Message { get; private set; }
        public bool TransMark { get; private set; }
        public string MessageTrans { get; private set; }
        public User User { get; private set; }
        public string Face { get; private set; }
        public TimeSpan Duration { get; private set; }

        public string RawData { get; private set; }
        
        public SuperChat(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            TimeStamp = new DateTime(1970, 01, 01).AddMilliseconds(double.Parse(json["data"]["ts"].ToString()));

            Price = uint.Parse(json["data"]["price"].ToString());
            try
            {
                Message = Regex.Unescape(json["data"]["message"].ToString());
            }
            catch (Exception)
            {
                Message = json["data"]["message"].ToString();
            }
            TransMark = int.Parse(json["data"]["trans_mark"].ToString()) != 0;
            MessageTrans = json["data"]["message_trans"].ToString();
            User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["user_info"]["uname"].ToString()));
            Face = json["data"]["user_info"]["face"].ToString();
            Duration = TimeSpan.FromSeconds(double.Parse(json["data"]["time"].ToString()));
        }
    }
}
