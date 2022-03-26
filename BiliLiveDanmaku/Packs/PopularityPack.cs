using BiliLive.Packs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//已检查无误
namespace BiliLive.Packs
{
    public class PopularityPack : Pack
    {
        public override PackTypes PackType => PackTypes.Popularity;
        public uint Popularity { get; private set; }

        public PopularityPack(byte[] payload) : base(payload)
        {
            if(payload != null && payload.Length >= 4)
            {
                Popularity = BitConverter.ToUInt32(payload.Take(4).Reverse().ToArray(), 0);
            }
        }
    }
}
