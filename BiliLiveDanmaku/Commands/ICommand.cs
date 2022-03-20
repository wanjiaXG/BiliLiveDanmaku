using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public interface ICommand : IData
    {
        CommandType CommandType { get; }

    }
}
