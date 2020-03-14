using System;
using robotManager.Helpful;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using System.ComponentModel;
using System.IO;
using robotManager;
using System.Collections.Generic;

[Serializable]
public class WholesomeProfessionsSave : Settings
{
    public static WholesomeProfessionsSave CurrentSetting { get; set; }

    public List<string> AlreadyCrafted { get; set; }

    public WholesomeProfessionsSave()
    {
        AlreadyCrafted = new List<string>();
    }

    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("WholesomeProfessionsSave",
                ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("WholesomeProfessionsSave > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("WholesomeProfessionsSave",
                ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting = Load<WholesomeProfessionsSave>(
                    AdviserFilePathAndName("WholesomeProfessionsSave",
                    ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new WholesomeProfessionsSave();
        }
        catch (Exception e)
        {
            Logging.WriteError("WholesomeProfessionsSave > Load(): " + e);
        }
        return false;
    }
}
