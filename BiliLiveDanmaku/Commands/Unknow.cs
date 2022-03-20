using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class Unknow : IData
    {
        public string RawData { get; private set; }

        public Unknow(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
        }
    }
}
