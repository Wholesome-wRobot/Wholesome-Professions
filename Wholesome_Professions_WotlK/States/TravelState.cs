using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class TravelState : State
    {
        public override string DisplayName
        {
            get { return "Traveling"; }
        }

        public override int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        private int _priority;
        private IProfession profession;

        public override bool NeedToRun
        {
            get
            {
                if (!Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause || !ObjectManager.Me.IsValid
                    || Conditions.IsAttackedAndCannotIgnore)
                    return false;

                if (Main.primaryProfession.ShouldTravel())
                {
                    profession = Main.primaryProfession;
                    return true;
                }

                return false;
            }
        }

        public override List<State> NextStates
        {
            get { return new List<State>(); }
        }

        public override List<State> BeforeStates
        {
            get { return new List<State>(); }
        }

        public override void Run()
        {
            Logger.LogDebug("************ RUNNING TRAVEL STATE ************");
            Broadcaster.autoBroadcast = false;

            ContinentId destinationContinent = Bot.Continent;

            // HORDE
            if (ToolBox.IsHorde())
            {
                // From EK
                if ((ContinentId)Usefuls.ContinentId == ContinentId.Azeroth)
                {
                    // To Kalimdor
                    if (destinationContinent == ContinentId.Kalimdor)
                    {
                        Logger.Log("Traveling to Kalimdor");
                        TravelHelper.HordeEKToKalimdor();
                    }
                    // To Outlands
                    if (destinationContinent == ContinentId.Expansion01)
                    {
                        Logger.Log("Traveling to Outland");
                        TravelHelper.HordeEKToOutland();
                    }
                    // To Northrend
                    if (destinationContinent == ContinentId.Northrend)
                    {
                        Logger.Log("Traveling to Northrend");
                        TravelHelper.HordeEKToKalimdor();
                    }
                }

                // From Kalimdor
                if ((ContinentId)Usefuls.ContinentId == ContinentId.Kalimdor)
                {
                    // To EK
                    if (destinationContinent == ContinentId.Azeroth)
                    {
                        Logger.Log("Traveling to Eastern Kingdoms");
                        TravelHelper.HordeKalimdorToEK();
                    }
                    // To Outlands
                    if (destinationContinent == ContinentId.Expansion01)
                    {
                        Logger.Log("Traveling to Outland");
                        TravelHelper.HordeKalimdorToEK();
                    }
                    // To Northrend
                    if (destinationContinent == ContinentId.Northrend)
                    {
                        Logger.Log("Traveling to Northrend");
                        TravelHelper.HordeKalimdorToNorthrend();
                    }
                }

                // From Outlands
                if ((ContinentId)Usefuls.ContinentId == ContinentId.Expansion01)
                {
                    // To Kalimdor
                    if (destinationContinent == ContinentId.Kalimdor)
                    {
                        Logger.Log("Traveling to Kalimdor");
                        TravelHelper.HordeOutlandToKalimdor();
                    }
                    // To EK
                    if (destinationContinent == ContinentId.Azeroth)
                    {
                        Logger.Log("Traveling to Eastern Kingdoms");
                        TravelHelper.HordeOutlandToKalimdor();
                    }
                    // To Northrend
                    if (destinationContinent == ContinentId.Northrend)
                    {
                        Logger.Log("Traveling to Northrend");
                        TravelHelper.HordeOutlandToKalimdor();
                    }
                }

                // From Northrend
                if ((ContinentId)Usefuls.ContinentId == ContinentId.Northrend)
                {
                    // To Kalimdor
                    if (destinationContinent == ContinentId.Kalimdor)
                    {
                        Logger.Log("Traveling to Kalimdor");
                        TravelHelper.HordeNorthrendToKalimdor();
                    }
                    // To EK
                    if (destinationContinent == ContinentId.Azeroth)
                    {
                        Logger.Log("Traveling to Eastern Kingdoms");
                        TravelHelper.HordeNorthrendToEK();
                    }
                    // To Outland
                    if (destinationContinent == ContinentId.Expansion01)
                    {
                        Logger.Log("Traveling to Outland");
                        TravelHelper.HordeNorthrendToOutland();
                    }
                }
            }
            Broadcaster.autoBroadcast = true;
        }
    }
}
