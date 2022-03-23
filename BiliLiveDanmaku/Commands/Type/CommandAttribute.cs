namespace BiliLive.Commands
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
