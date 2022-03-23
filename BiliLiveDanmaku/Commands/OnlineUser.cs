using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class OnlineUser : Command
    {
        public int Count { get; private set; }

        public User[] Users;

        public OnlineUser(JToken token) : base(token)
        {
            Count = GetValue<int>("num");

            List<User> list = new List<User>();
            foreach(var item in GetValue<JArray>("list"))
            {
                list.Add(JsonConvert.DeserializeObject<User>(item.ToString()));
            }
            Users = list.ToArray();
        }

        public class User
        {
            [JsonProperty("uid")]
            public string UID;

            [JsonProperty("name")]
            public string Username;

            [JsonProperty("face")]
            public string Face;
        }


        public static OnlineUser NewInstance(string cookie, uint RoomId, uint uid)
        {
            //HTTP API + COOKIE
            JToken json = null;
            try
            {
                int page = 1;
                int limit = 50;

                JArray list = new JArray();
                do
                {
                    string url = $"https://api.live.bilibili.com/xlive/general-interface/v1/rank/getOnlineRank?page={page}&pageSize={limit}&platform=pc_link&roomId={RoomId}&ruid={uid}";
                    WebClient client = new WebClient();
                    client.Headers["Cookie"] = cookie;
                    json = JToken.Parse(client.DownloadString(url));

                    var item = Util.GetJTokenValue<JArray>(json, "data", "item");

                    if(item != null)
                    {
                        if(item.Count <= 0) 
                        {
                            int num = Util.GetJTokenValue<int>(json, "data", "onlineNum");
                            JObject result = new JObject();
                            result.Add("num", num);
                            result.Add("list", list);
                            return new OnlineUser(result);
                        }
                        else
                        {
                            foreach (var user in item)
                            {
                                list.Add(user as JObject);
                            }
                        }
                    }
                    page++;
                } while (true);

                

            }
            catch
            {

            }
            
            return new OnlineUser(json);
        }


    }
}
