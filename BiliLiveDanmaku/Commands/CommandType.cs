using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public enum CommandType
    {
        UNKNOW,
        LIVE,
        PREPARING,
        DANMU_MSG,
        SEND_GIFT,
        SPECIAL_GIFT,
        USER_TOAST_MSG,
        GUARD_MSG,
        GUARD_BUY,
        GUARD_LOTTERY_START,
        WELCOME,
        WELCOME_GUARD,
        ENTRY_EFFECT,
        SYS_MSG,
        ROOM_BLOCK_MSG,
        COMBO_SEND,
        ROOM_RANK,
        TV_START,
        NOTICE_MSG,
        SYS_GIFT,
        ROOM_REAL_TIME_MESSAGE_UPDATE,
        SUPER_CHAT_ENTRANCE,
        SUPER_CHAT_MESSAGE,
        SUPER_CHAT_MESSAGE_DELETE,
        INTERACT_WORD,
        WATCHED_CHANGE
    }
}
