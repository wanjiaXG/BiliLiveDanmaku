using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class OnlineUser : Command
    {
        public OnlineUser(JToken json) : base(json)
        {

        }

        public static OnlineUser NewInstance(string cookie)
        {
            //HTTP API + COOKIE

            JToken token =null;
            return new OnlineUser(token);
        }

    }
}
