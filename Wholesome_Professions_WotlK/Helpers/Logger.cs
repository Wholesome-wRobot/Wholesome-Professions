using robotManager.Helpful;
using System.Drawing;

namespace Wholesome_Professions_WotlK.Helpers
{
    public static class Logger
    {
        public static void Log(string str)
        {
            Logging.Write("[Wholesome Professions WotlK] " + str, Logging.LogType.Normal, Color.DodgerBlue);
        }

        public static void LogDebug(string str)
        {
            Logging.Write("[Wholesome Professions WotlK] " + str, Logging.LogType.Debug, Color.BlueViolet);
        }
    }
}
