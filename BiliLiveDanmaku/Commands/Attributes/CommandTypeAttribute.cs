//已检查无运行异常
using BiliLive.Commands.Enums;

namespace BiliLive.Commands.Attribute
{
    public class CommandTypeAttribute : System.Attribute
    {
        public CommandType Type { get; private set; }
        public CommandTypeAttribute(CommandType type)
        {
            Type = type;
        }
    }
}
