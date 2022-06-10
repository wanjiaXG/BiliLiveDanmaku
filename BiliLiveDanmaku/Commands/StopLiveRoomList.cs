using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BiliLive.Commands
{
    public class StopLiveRoomList : Command
    {
        private readonly List<uint> list;

        public StopLiveRoomList(JToken json) : base(json)
        {
            try
            {
                list = new List<uint>();
                foreach (var item in Util.GetJTokenValue<JArray>(json, "data", "room_id_list"))
                {
                    list.Add(Util.ChangeType<uint>(item.ToString()));
                }
            }
            catch
            {

            }
        }

        public uint[] RoomIdList => list.ToArray();

    }
}
