using robotManager.Helpful;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Profile;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class ProfileHandler
{
    public static GrinderProfile Profile = new GrinderProfile();
    internal static int ZoneIdProfile;

    public static void LoadNewProfile(string profession, string profileName)
    {
        Profile = new GrinderProfile();
        string filePath = Application.StartupPath + "\\Profiles\\Wholesome Professions\\" + profileName;
        Bot.ProfileName = profileName;

        // If grinder School Load Profile
        if (!string.IsNullOrWhiteSpace(profileName) && File.Exists(filePath))
        {
            Profile = XmlSerializer.Deserialize<GrinderProfile>(filePath);
            if (Profile.GrinderZones.Count <= 0)
            {
                Logger.Log($"Profile '{filePath}' seems incorrect. Please use a Grinder profile.");
                UnloadCurrentProfile();
                return;
            }
            else
                Logger.Log("Profile loaded");
        }
        else
        {
            Logger.LogLineBroadcastImportant($"Profile file '{filePath}' not found");
            UnloadCurrentProfile();
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

        // Go to first hotspot or travel
        if (Profile.GrinderZones.Count > 0)
        {
            string zoneName = Profile.GrinderZones[ZoneIdProfile].Name;
            int continentId = TravelHelper.GetContinentFromZoneName(zoneName);
            Logger.LogDebug($"Zone {zoneName} is on continent {continentId.ToString()}");
            if (continentId == -1)
            {
                Logger.LogLineBroadcastImportant($"ERROR : The zone name {zoneName} from your profile is incorrect. Please use default zone names.");
                UnloadCurrentProfile();
                return;
            }

            if (continentId != Usefuls.ContinentId)
            {
                Logger.Log($"{Profile.GrinderZones[ZoneIdProfile].Name} is on another continent ({continentId}). Launching traveler.");
                Bot.SetContinent((ContinentId)continentId);
                return;
            }

            Bot.ProfileProfession = profession;
            Logger.Log($"Heading to first spot {Profile.GrinderZones[ZoneIdProfile].Vectors3[0]} in {Profile.GrinderZones[ZoneIdProfile].Name}");
            Broadcaster.autoBroadcast = false;
            Broadcaster.BroadCastSituation();
            GoToTask.ToPosition(Profile.GrinderZones[ZoneIdProfile].Vectors3[0], 50);
            Broadcaster.autoBroadcast = true;
        }
        else
        {
            Logger.LogDebug("No grinder zone found");
            UnloadCurrentProfile();
        }
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

    public static void UnloadCurrentProfile()
    {
        if (Bot.ProfileName != null)
        {
            Logger.Log($"Unloading profile {Bot.ProfileName}");
            Bot.ProfileName = null;
            Bot.ProfileProfession = null;

            Profile = new GrinderProfile();
            Bot.MovementLoop.PathLoop.Clear();
            Bot.Grinding.FactionsTarget.Clear();
            Bot.Grinding.EntryTarget.Clear();
            Bot.Grinding.MaxTargetLevel = 0;
            Bot.Grinding.MinTargetLevel = 0;
        }
    }

}