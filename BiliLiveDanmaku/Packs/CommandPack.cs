using BiliLive.Packs.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//已检查无误
namespace BiliLive.Packs
{
    public class CommandPack : Pack
    {
        public override PackTypes PackType => PackTypes.Command;
        public JToken Value { get; private set; }

        public CommandPack(byte[] payload) : base(payload)
        {
            try
            {
                if(payload != null)
                {
                    string jstr = Encoding.UTF8.GetString(payload, 0, payload.Length);
                    Value = JObject.Parse(jstr);
                }
            }
            catch
            {
                Value = new JObject();
            }
        }
    }
}
