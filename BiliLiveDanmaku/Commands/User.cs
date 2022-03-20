using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    [Serializable]
    public class User
    {
        public uint Id;
        public string Name;

        public User(uint id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
