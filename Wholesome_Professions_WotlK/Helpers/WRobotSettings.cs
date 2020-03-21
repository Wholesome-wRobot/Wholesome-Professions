using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wholesome_Professions_WotlK.Helpers;
using static wManager.wManagerSetting;

static class WRobotSettings
{
    public static bool saveCloseIfPlayerTeleported;
    public static float saveSearchRadiusMobs;
    public static float saveSearchRadiusObjects;

    private static void ChangeSetting<T>(ref T setting, ref T value)
    {
        Logger.Log($"Setting {setting} to {value}");
    }

    public static void SetRecommendedWRobotSettings()
    {
        Logger.Log("Setting recommended configuration");

        Logger.LogDebug($"Setting SearchRadiusObjects to 250 - was {CurrentSetting.SearchRadiusObjects}");
        saveSearchRadiusObjects = CurrentSetting.SearchRadiusObjects ;
        CurrentSetting.SearchRadiusObjects = 250;

        Logger.LogDebug($"Setting CloseIfPlayerTeleported to false - was {CurrentSetting.CloseIfPlayerTeleported}");
        saveCloseIfPlayerTeleported = CurrentSetting.CloseIfPlayerTeleported;
        CurrentSetting.CloseIfPlayerTeleported = false;

        Logger.LogDebug($"Setting SearchRadiusMobs to 120 - was {CurrentSetting.SearchRadiusMobs}");
        saveSearchRadiusMobs = CurrentSetting.SearchRadiusMobs;
        CurrentSetting.SearchRadiusMobs = 120;
    }

    public static void RestoreUserWRobotSettings()
    {
        Logger.Log("Restoring user configuration");

        CurrentSetting.CloseIfPlayerTeleported = saveCloseIfPlayerTeleported;
        CurrentSetting.SearchRadiusMobs = saveSearchRadiusMobs;
        CurrentSetting.SearchRadiusObjects = saveSearchRadiusObjects;
    }
}
