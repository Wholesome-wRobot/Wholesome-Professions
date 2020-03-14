using System.Collections.Generic;
using robotManager.FiniteStateMachine;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class SellItemsState : State
    {
        public override string DisplayName
        {
            get { return "Selling Items"; }
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
                    || Conditions.IsAttackedAndCannotIgnore || Main.currentProfession == null)
                    return false;

                if (Main.currentProfession.ShouldSellItems())
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
            Logger.LogDebug("************ RUNNING SELL ITEMS STATE ************");
            Broadcaster.autoBroadcast = false;

            // Sell items if we need bag space
            wManager.wManagerSetting.CurrentSetting.Selling = true;
            wManager.Wow.Bot.States.ToTown.ForceToTown = true;
            
            Broadcaster.autoBroadcast = true;
        }
    }
}
