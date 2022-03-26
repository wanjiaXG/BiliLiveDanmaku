//已检查无运行异常
namespace BiliLive.Commands.Attribute
{
    public class CommandAttribute : System.Attribute
    {
        public System.Type Type { get; private set; } 

        public CommandAttribute(System.Type type)
        {
            Type = type;
        }
    }
}
