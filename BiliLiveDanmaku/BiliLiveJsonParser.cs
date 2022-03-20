using BiliLive.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BiliLive
{
    public class BiliLiveJsonParser
    {
        public static IData Parse(JToken json)
        {
            string[] cmd = (json["cmd"].ToString()).Split(':');
            switch (cmd[0])
            {
                case "DANMU_MSG":
                    return new Danmaku(json);
                case "SUPER_CHAT_MESSAGE":
                    return new SuperChat(json);
                case "SEND_GIFT":
                    return new Gift(json);
                case "COMBO_SEND":
                    return new ComboSend(json);
                case "WELCOME":
                    return new Welcome(json);
                case "WELCOME_GUARD":
                    return new WelcomeGuard(json);
                case "GUARD_BUY":
                    return new GuardBuy(json);
                case "INTERACT_WORD":
                    return new InteractWord(json);
                case "ROOM_BLOCK_MSG":
                    return new RoomBlock(json);
                case "WATCHED_CHANGE":
                    return new WatchedChanged(json);
                case "LIVE":
                    return new Live(json);
                case "PREPARING":
                    return new Preparing(json);
                case "SPECIAL_GIFT":
                case "USER_TOAST_MSG":
                case "GUARD_MSG":
                case "GUARD_LOTTERY_START":
                case "ENTRY_EFFECT":
                case "SYS_MSG":
                case "ROOM_RANK":
                case "TV_START":
                case "NOTICE_MSG":
                case "SYS_GIFT":
                case "ROOM_REAL_TIME_MESSAGE_UPDATE":
                default:
                    return new Unknow(json);
            }
        }
    }
}
