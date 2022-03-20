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
    public class Danmaku : ITimeStampedCommand
    {
        public CommandType CommandType => CommandType.DANMU_MSG;

        public DateTime TimeStamp { get; private set; }

        public User User { get; private set; }
        public string Message { get; private set; }
        public uint Type { get; private set; }

        public string RawData { get; private set; }

        public Danmaku(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            User = new User((uint)json["info"][2][0], Regex.Unescape(json["info"][2][1].ToString()));
            try
            {
                Message = Regex.Unescape(json["info"][1].ToString());
            }
            catch (Exception)
            {
                Message = json["info"][1].ToString();
            }

            Type = (uint)json["info"][0][9];

            TimeStamp = new DateTime(1970, 01, 01).AddMilliseconds(double.Parse(json["info"][0][4].ToString()));
        }
    }
}
