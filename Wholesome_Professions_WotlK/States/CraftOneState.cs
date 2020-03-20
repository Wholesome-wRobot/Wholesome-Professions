using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class CraftOneState : State
    {
        public override string DisplayName
        {
            get { return "Crafting one"; }
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
                    || Conditions.IsAttackedAndCannotIgnore || Main.currentProfession == null || ObjectManager.Me.IsMounted)
                    return false;

                if (Main.currentProfession.ShouldCraftOne())
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
            Logger.LogDebug("************ RUNNING CRAFT ONE STATE ************");
            Step currentStep = Main.currentProfession.CurrentStep;

            MovementManager.StopMoveNewThread();

            // Deactivate broadcast
            Broadcaster.autoBroadcast = false;

            // Craft
            int itemInBagsBeforeCraft = ItemsManager.GetItemCountById(1);

            Logger.Log($"Crafting {currentStep.itemoCraft.name}");
            ToolBox.Craft(Main.currentProfession.ProfessionName.ToString(), currentStep.itemoCraft, 1);
            Logger.Log("Craft complete");
            Lua.RunMacroText("/stopcasting");

            Broadcaster.autoBroadcast = true;
        }
    }
}
