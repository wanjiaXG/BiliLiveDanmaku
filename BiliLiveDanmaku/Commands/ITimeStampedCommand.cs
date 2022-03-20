using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    interface ITimeStampedCommand : ICommand
    {
        DateTime TimeStamp { get; }
    }
}
