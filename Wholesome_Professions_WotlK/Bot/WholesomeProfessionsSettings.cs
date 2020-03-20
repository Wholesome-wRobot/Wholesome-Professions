using System;
using robotManager.Helpful;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using System.ComponentModel;
using System.IO;
using robotManager;
using Wholesome_Professions_WotlK.Helpers;

[Serializable]
public class WholesomeProfessionsSettings : Settings
{
    public static WholesomeProfessionsSettings CurrentSetting { get; set; }
    
    public bool LogDebug { get; set; }
    public int ServerRate { get; set; }
    public int BroadcasterInterval { get; set; }
    public double LastUpdateDate { get; set; }
    public bool Autofarm { get; set; }

    public WholesomeProfessionsSettings()
    {
        LogDebug = false;
        ServerRate = 1;
        BroadcasterInterval = 5;
        LastUpdateDate = 0;
        Autofarm = true;
    }

    public bool Save()
    {
        try
        {
            Logger.LogDebug("Saving settings");
            return Save(AdviserFilePathAndName("WholesomeProfessionsSettings",
                ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("WholesomeProfessionsSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            Logger.Log("Loading settings");
            if (File.Exists(AdviserFilePathAndName("WholesomeProfessionsSettings",
                ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting = Load<WholesomeProfessionsSettings>(
                    AdviserFilePathAndName("WholesomeProfessionsSettings",
                    ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new WholesomeProfessionsSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("WholesomeProfessionsSettings > Load(): " + e);
        }
        return false;
    }
}
