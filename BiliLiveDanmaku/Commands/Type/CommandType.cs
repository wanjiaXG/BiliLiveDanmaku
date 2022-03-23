namespace BiliLive.Commands
{
    public enum CommandType
    {
        [Command(typeof(Command))]
        UNKNOW,

        [Command(typeof(Live))]
        LIVE,

        [Command(typeof(Preparing))]
        PREPARING,

        [Command(typeof(Danmaku))]
        DANMU_MSG,

        [Command(typeof(Gift))]
        SEND_GIFT,

        [Command(typeof(GuardBuy))]
        GUARD_BUY,
        
        [Command(typeof(Welcome))]
        WELCOME,

        [Command(typeof(WelcomeGuard))]
        WELCOME_GUARD,

        [Command(typeof(RoomBlock))]
        ROOM_BLOCK_MSG,

        [Command(typeof(ComboSend))]
        COMBO_SEND,

        [Command(typeof(SuperChat))]
        SUPER_CHAT_MESSAGE,

        [Command(typeof(InteractWord))]
        INTERACT_WORD,

        [Command(typeof(WatchedChanged))]
        WATCHED_CHANGE,

        [Command(typeof(WidgetBanner))]
        WIDGET_BANNER,


        //ENTRY_EFFECT,
        //SYS_MSG,
        //ROOM_RANK,
        //TV_START,
        //NOTICE_MSG,
        //SYS_GIFT,
        //ROOM_REAL_TIME_MESSAGE_UPDATE,
        //SUPER_CHAT_ENTRANCE,
        //SUPER_CHAT_MESSAGE_DELETE,
        //SPECIAL_GIFT,
        //USER_TOAST_MSG,
        //GUARD_LOTTERY_START,
        //GUARD_MSG,




        #region UnDanmakuServerMessage

        [Command(typeof(OnlineUser))]
        HTTP_API_ONLINE_USER

        #endregion
    }
}
