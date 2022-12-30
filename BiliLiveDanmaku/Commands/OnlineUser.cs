using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;

//已检查无运行异常
namespace BiliLive.Commands
{
    public class OnlineUser : Command
    {
        public uint Count { get; private set; }

        public User[] Users;

        public OnlineUser(JToken token) : base(token)
        {
            Count = GetValue<uint>("num");

            List<User> list = new List<User>();
            foreach(var item in GetValue<JArray>("list"))
            {
                try
                {
                    list.Add(JsonConvert.DeserializeObject<User>(item.ToString()));
                }
                catch
                {

                }
            }
            Users = list.ToArray();
        }

        private OnlineUser(List<User> users) : base("")
        {
            if(users != null)
            {
                Users = users.ToArray();
                Count = (uint)users.Count;
            }
            else
            {
                Users = new User[0];
            }
        }

        public class User
        {
            [JsonProperty("uid")]
            public uint UID;

            [JsonProperty("name")]
            public string Username;

            [JsonProperty("face")]
            public string Face;
        }


        public static OnlineUser NewInstance(string cookie, uint RoomId, uint uid)
        {
            Dictionary<uint, User> dic = new Dictionary<uint, User>();

            JToken json = null;
            try
            {
                int page = 1;
                int limit = 50;

                do
                {
                    string url = $"https://api.live.bilibili.com/xlive/general-interface/v1/rank/getOnlineRank?page={page}&pageSize={limit}&platform=pc_link&roomId={RoomId}&ruid={uid}";
                    using (WebClient client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        client.Headers["Cookie"] = cookie;
                        json = JToken.Parse(client.DownloadString(url));

                        var item = Util.GetJTokenValue<JArray>(json, "data", "item");
                        if (json == null) break;
                        if (item == null) break;
                        if (item.Count <= 0) break;

                        foreach (var u in item)
                        {
                            User user = u.ToObject<User>();
                            if (user != null && !dic.ContainsKey(user.UID))
                            {
                                dic.Add(user.UID, user);
                            }
                        }
                        page++;
                    }
                } while (true);

            }
            catch
            {

            }


            List<User> result = new List<User>();
            foreach (var item in dic)
            {
                result.Add(item.Value);
            }

            return new OnlineUser(result);
        }

    }
}
