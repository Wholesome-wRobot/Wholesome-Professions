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

        public override bool NeedToRun
        {
            get
            {
                if (!Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause || !ObjectManager.Me.IsValid
                    || Conditions.IsAttackedAndCannotIgnore || Main.amountProfessionsSelected <= 0 || Main.primaryProfession.CurrentStep == null)
                    return false;

                if (Main.primaryProfession.ShouldTravel())
                    return true;

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

            int destinationContinent = Main.primaryProfession.Continent;

            // HORDE
            if (ToolBox.IsHorde())
            {
                // From EK
                if (Usefuls.ContinentId == (int)ContinentId.Azeroth)
                {
                    // To Kalimdor
                    if (destinationContinent == (int)ContinentId.Kalimdor)
                    {
                        Logger.Log("Traveling to Kalimdor");
                        TravelHelper.HordeEKToKalimdor();
                    }
                    // To Outlands
                    if (destinationContinent == (int)ContinentId.Expansion01)
                    {
                        Logger.Log("Traveling to Outland");
                        TravelHelper.HordeEKToOutland();
                    }
                    // To Northrend
                    if (destinationContinent == (int)ContinentId.Northrend)
                    {
                        Logger.Log("Traveling to Northrend");
                        TravelHelper.HordeEKToKalimdor();
                    }
                }

                // From Kalimdor
                if (Usefuls.ContinentId == (int)ContinentId.Kalimdor)
                {
                    // To EK
                    if (destinationContinent == (int)ContinentId.Azeroth)
                    {
                        Logger.Log("Traveling to Eastern Kingdoms");
                        TravelHelper.HordeKalimdorToEK();
                    }
                    // To Outlands
                    if (destinationContinent == (int)ContinentId.Expansion01)
                    {
                        Logger.Log("Traveling to Outland");
                        TravelHelper.HordeKalimdorToEK();
                    }
                    // To Northrend
                    if (destinationContinent == (int)ContinentId.Northrend)
                    {
                        Logger.Log("Traveling to Northrend");
                        TravelHelper.HordeKalimdorToNorthrend();
                    }
                }

                // From Outlands
                if (Usefuls.ContinentId == (int)ContinentId.Expansion01)
                {
                    // To Kalimdor
                    if (destinationContinent == (int)ContinentId.Kalimdor)
                    {
                        Logger.Log("Traveling to Kalimdor");
                        TravelHelper.HordeOutlandToKalimdor();
                    }
                    // To EK
                    if (destinationContinent == (int)ContinentId.Azeroth)
                    {
                        Logger.Log("Traveling to Eastern Kingdoms");
                        TravelHelper.HordeOutlandToKalimdor();
                    }
                    // To Northrend
                    if (destinationContinent == (int)ContinentId.Northrend)
                    {
                        Logger.Log("Traveling to Northrend");
                        TravelHelper.HordeOutlandToKalimdor();
                    }
                }

                // From Northrend
                if (Usefuls.ContinentId == (int)ContinentId.Northrend)
                {
                    // To Kalimdor
                    if (destinationContinent == (int)ContinentId.Kalimdor)
                    {
                        Logger.Log("Traveling to Kalimdor");
                        TravelHelper.HordeNorthrendToKalimdor();
                    }
                    // To EK
                    if (destinationContinent == (int)ContinentId.Azeroth)
                    {
                        Logger.Log("Traveling to Eastern Kingdoms");
                        TravelHelper.HordeNorthrendToEK();
                    }
                    // To Outland
                    if (destinationContinent == (int)ContinentId.Expansion01)
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
