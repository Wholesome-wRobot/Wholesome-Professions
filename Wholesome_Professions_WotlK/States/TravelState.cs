using robotManager.FiniteStateMachine;
using robotManager.Helpful;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class TravelState : State
    {
        public override string DisplayName
        {
            get { return "Travel"; }
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
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && ObjectManager.Me.IsValid
                    && !Conditions.IsAttackedAndCannotIgnore && Main.currentProfession != null 
                    && Main.currentProfession.Continent != Usefuls.ContinentId
                    && ObjectManager.Me.Level >= Main.currentProfession.MinimumCharLevel
                    && Main.currentProfession.CurrentStep != null
                    && Main.currentProfession.ItemToFarm == null)
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
            // ek = 0, kalimdor = 1, Outlands = 530, Northrend 571
            Logger.LogDebug("************ RUNNING TRAVEL STATE ************");
            Broadcaster.autoBroadcast = false;
            Step currentStep = Main.currentProfession.CurrentStep;
            int destinationContinent = Main.currentProfession.Continent;

            // To Outlands
            if (destinationContinent == 530)
            {
                Logger.Log("Traveling to Outland");
                KalimdorToEK();
                EKToOutland();
            }

            MovementManager.StopMoveNewThread();
            Main.currentProfession.RegenerateSteps();
            Broadcaster.autoBroadcast = true;
        }

        private void KalimdorToEK()
        {
            if (Usefuls.ContinentId == 1)
            {
                Vector3 position = new Vector3(1472.55f, -4215.7f, 59.221f);
                if (GoToTask.ToPositionAndIntecractWithGameObject(position, 195142))
                    Thread.Sleep(1500);
            }
        }

        private void EKToOutland()
        {
            if (Usefuls.ContinentId == 0)
            {
                if (GoToTask.ToPosition(new Vector3(-11920.39, -3206.81, -15.35475f)))
                    Thread.Sleep(5000);
                if (GoToTask.ToPosition(new Vector3(-182.5485, 1023.459, 54.23014)))
                    Thread.Sleep(1500);
            }
        }
    }
}
