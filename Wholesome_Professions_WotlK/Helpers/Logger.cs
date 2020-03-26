using robotManager.Helpful;
using System.Drawing;

namespace Wholesome_Professions_WotlK.Helpers
{
    public static class Logger
    {
        public static void Log(string str)
        {
            Logging.Write($"[{Main.productName}] " + str, Logging.LogType.Normal, Color.DodgerBlue);
        }

        public static void LogLineBroadcast(string str, Color c)
        {
            if (str != null)
                Logging.Write($"[{Main.productName}] " + str, Logging.LogType.Normal, c);
        }

        public static void LogLineBroadcastImportant(string str)
        {
            if (str != null)
                Logging.Write($"[{Main.productName}] " + str, Logging.LogType.Normal, Color.Tomato);
        }

        public static void LogDebug(string str)
        {
            if (WholesomeProfessionsSettings.CurrentSetting.LogDebug)
                Logging.Write($"[{Main.productName}] " + str, Logging.LogType.Debug, Color.BlueViolet);
        }
    }
}
