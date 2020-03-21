using robotManager.Helpful;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Profile;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class ProfileHandler
{
    internal static GrinderProfile Profile = new GrinderProfile();
    internal static int ZoneIdProfile;

    public static void LoadNewProfile(IProfession profession)
    {
        Profile = new GrinderProfile();
        string filePath = Application.StartupPath + "\\Profiles\\Wholesome Professions\\" + profession.CurrentProfile;

        // If grinder School Load Profile
        if (!string.IsNullOrWhiteSpace(profession.CurrentProfile) &&
            File.Exists(filePath))
        {
            Profile = XmlSerializer.Deserialize<GrinderProfile>(filePath);
            if (Profile.GrinderZones.Count <= 0)
            {
                Logger.Log($"Profile '{filePath}' seems incorrect. Please use a Grinder profile.");
                profession.CurrentProfile = null;
            }
            else
                Logger.Log("Profile loaded");
        }
        else
        {
            Logger.LogLineBroadcastImportant($"Profile file '{filePath}' not found");
            profession.CurrentProfile = null;
            return;
        }

        SelectZone();

        // Black List:
        var blackListDic =
            Profile.GrinderZones.SelectMany(zone => zone.BlackListRadius).ToDictionary(b => b.Position,
                                                                                        b => b.Radius);
        //wManager.wManagerSetting.AddRangeBlackListZone(blackListDic);

        // Add Npc
        foreach (var zone in Profile.GrinderZones)
        {
            NpcDB.AddNpcRange(zone.Npc);
        }

        //TravelHelper.GetContinentFromZoneName(Profile.GrinderZones[ZoneIdProfile].Name);

        // Go to first hotspot or travel
        if (Profile.GrinderZones.Count > 0)
        {
            string zoneName = Profile.GrinderZones[ZoneIdProfile].Name;
            int continentId = TravelHelper.GetContinentFromZoneName(zoneName);
            Logger.LogDebug($"Zone {zoneName} is on continent {continentId.ToString()}");
            if (continentId == -1)
            {
                Logger.LogLineBroadcastImportant($"ERROR : The zone name {zoneName} from your profile is incorrect. Please use default zone names.");
                UnloadCurrentProfile(profession);
                return;
            }

            if (continentId != Usefuls.ContinentId)
            {
                Logger.Log($"{Profile.GrinderZones[ZoneIdProfile].Name} is on another continent ({continentId}). Launching traveler.");
                profession.Continent = continentId;
                return;
            }

            Logger.Log($"Heading to first spot {Profile.GrinderZones[ZoneIdProfile].Vectors3[0]}");
            Broadcaster.autoBroadcast = false;
            GoToTask.ToPosition(Profile.GrinderZones[ZoneIdProfile].Vectors3[0], 50);
            Broadcaster.autoBroadcast = true;
        }
        else
            Logger.LogDebug("No grinder zone found");
    }

    internal static void SelectZone()
    {
        for (int i = 0; i <= Profile.GrinderZones.Count - 1; i++)
        {
            if (Profile.GrinderZones[i].MaxLevel >= ObjectManager.Me.Level &&
                Profile.GrinderZones[i].MinLevel <= ObjectManager.Me.Level &&
                Profile.GrinderZones[i].IsValid())
            {
                ZoneIdProfile = i;
                break;
            }
        }

        if (Profile.GrinderZones[ZoneIdProfile].Hotspots)
        {
            var vectors3Temps = new List<Vector3>();
            for (int i = 0; i <= Profile.GrinderZones[ZoneIdProfile].Vectors3.Count - 1; i++)
            {
                if (i + 1 > Profile.GrinderZones[ZoneIdProfile].Vectors3.Count - 1)
                    vectors3Temps.AddRange(PathFinder.FindPath(Profile.GrinderZones[ZoneIdProfile].Vectors3[i],
                                                                Profile.GrinderZones[ZoneIdProfile].Vectors3[0]));
                else
                    vectors3Temps.AddRange(PathFinder.FindPath(Profile.GrinderZones[ZoneIdProfile].Vectors3[i],
                                                                Profile.GrinderZones[ZoneIdProfile].Vectors3[i + 1]));
            }
            Profile.GrinderZones[ZoneIdProfile].Hotspots = false;
            Profile.GrinderZones[ZoneIdProfile].Vectors3.Clear();
            Profile.GrinderZones[ZoneIdProfile].Vectors3.AddRange(vectors3Temps);
        }

        Bot.Grinding.EntryTarget = Profile.GrinderZones[ZoneIdProfile].TargetEntry;
        Bot.Grinding.FactionsTarget = Profile.GrinderZones[ZoneIdProfile].TargetFactions;
        Bot.Grinding.MaxTargetLevel = Profile.GrinderZones[ZoneIdProfile].MaxTargetLevel;
        Bot.Grinding.MinTargetLevel = Profile.GrinderZones[ZoneIdProfile].MinTargetLevel;

        Bot.MovementLoop.PathLoop = Profile.GrinderZones[ZoneIdProfile].Vectors3;
    }

    public static void UnloadCurrentProfile(IProfession profession)
    {
        if (profession != null && profession.CurrentProfile != null)
        {
            Logger.Log($"Unloading profile {profession.CurrentProfile}");
            profession.CurrentProfile = null;

            Profile = new GrinderProfile();
            Bot.MovementLoop.PathLoop.Clear();
            Bot.Grinding.FactionsTarget.Clear();
            Bot.Grinding.EntryTarget.Clear();
            Bot.Grinding.MaxTargetLevel = 0;
            Bot.Grinding.MinTargetLevel = 0;
        }
    }

}