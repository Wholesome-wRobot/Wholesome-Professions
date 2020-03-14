using System;
using robotManager.Helpful;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using System.ComponentModel;
using System.IO;
using robotManager;

[Serializable]
public class WholesomeProfessionsSettings : Settings
{
    public static WholesomeProfessionsSettings CurrentSetting { get; set; }

    public WholesomeProfessionsSettings()
    {
        LogDebug = false;

        ConfigWinForm(
            new System.Drawing.Point(400, 400), "Wholesome Professions "
            + Translate.Get("Settings")
        );
    }

    public bool Save()
    {
        try
        {
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

    [Category("Misc")]
    [DefaultValue(false)]
    [DisplayName("Log Debug")]
    [Description("For Development purpose")]
    public bool LogDebug { get; set; }
}
