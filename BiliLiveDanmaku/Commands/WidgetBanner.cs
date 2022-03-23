using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class WidgetBanner : Command
    {
        public Widget[] WidgetList { get; private set; }

        public WidgetBanner(JToken json) : base(json)
        {
            try
            {
                if (json["data"]["widget_list"] is JObject obj)
                {
                    List<Widget> list = new List<Widget>();
                    foreach (var prop in obj.Properties())
                    {
                        list.Add(JsonConvert.DeserializeObject<Widget>(obj[prop.Name].ToString()));
                    }
                    WidgetList = list.ToArray();
                }
            }
            catch
            {

            }
        }
        public class Widget
        {
            [JsonProperty("id")]
            public int Id { get; internal set; }

            [JsonProperty("title")]
            public string Title { get; internal set; }

            [JsonProperty("cover")]
            public string Cover { get; internal set; }

            [JsonProperty("web_cover")]
            public string WebCover { get; internal set; }

            [JsonProperty("tip_text")]
            public string TipText { get; internal set; }

            [JsonProperty("tip_text_color")]
            public string TipTextColor { get; internal set; }

            [JsonProperty("tip_bottom_color")]
            public string TipBottomColor { get; internal set; }

            [JsonProperty("jump_url")]
            public string JumpUrl { get; internal set; }

            [JsonProperty("url")]
            public string Url { get; internal set; }

            [JsonProperty("stay_time")]
            public int StayTime { get; internal set; }

            [JsonProperty("site")]
            public int Site { get; internal set; }

            [JsonProperty("platform_in")]
            public string[] PlatformIn { get; internal set; }

            [JsonProperty("type")]
            public int Type { get; internal set; }

            [JsonProperty("band_id")]
            public int BandId { get; internal set; }

            [JsonProperty("sub_key")]
            public string SubKey { get; internal set; }

            [JsonProperty("sub_data")]
            public string SubData { get; internal set; }

            [JsonProperty("is_add")]
            public bool IsAdd { get; internal set; }
        }

    }
}
