using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wManager.Wow.Helpers;

namespace Wholesome_Professions_WotlK.Helpers
{
    public static class Broadcaster
    {
        public static List<string> broadcastMessages = new List<string>();
        public static bool autoBroadcast = true;

        public static void BroadCastSituation()
        {
            if (broadcastMessages != null && broadcastMessages.Count > 0)
            {
                Logger.Log("********** BROADCAST ************");
                foreach (string message in broadcastMessages)
                {
                    Lua.LuaDoString($"DEFAULT_CHAT_FRAME: AddMessage('{message}')");
                    Logger.Log(message);
                }
                Logger.Log("*************************************");
            }
        }

        public static void AddBroadCastMessage(string message)
        {
            if (broadcastMessages != null)
                broadcastMessages.Add(message);
        }

        public static void ClearBroadCastMessages()
        {
            if (broadcastMessages != null)
                broadcastMessages.Clear();
        }

        public static void ClearAndAddBroadCastMessage(string message)
        {
            if (broadcastMessages != null)
            {
                broadcastMessages.Clear();
                broadcastMessages.Add(message);
            }
        }

        public static void ClearAndAddBroadCastMessagesList(List<string> groupedMessages)
        {
            if (broadcastMessages != null)
            {
                broadcastMessages.Clear();
                broadcastMessages = groupedMessages;
            }
        }
    }
}
