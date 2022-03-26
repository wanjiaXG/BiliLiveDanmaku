using BiliLive.Packs.Enums;

//已检查无误
namespace BiliLive.Packs
{
    public class HeartbeatPack : Pack
    {
        public override PackTypes PackType => PackTypes.Heartbeat;

        public HeartbeatPack(byte[] payload) : base(payload){}
    }
}
