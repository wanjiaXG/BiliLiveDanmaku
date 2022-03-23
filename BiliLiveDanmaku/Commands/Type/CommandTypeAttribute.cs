using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class CommandTypeAttribute : Attribute
    {
        public CommandType Type { get; private set; }
        public CommandTypeAttribute(CommandType type)
        {
            Type = type;
        }
    }
}
