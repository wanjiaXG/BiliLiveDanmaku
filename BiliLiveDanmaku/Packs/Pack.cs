using BiliLive.Packs.Enums;

//已检查无误
namespace BiliLive.Packs
{
    public class Pack
    {
        public virtual PackTypes PackType { get; private set; } = PackTypes.Unknow;
        public byte[] RawData { get; private set; }

        public Pack(byte[] buffer)
        {
            RawData = buffer;
        }

    }
}
