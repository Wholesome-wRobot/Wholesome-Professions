using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class FilterLootState : State
    {
        public override string DisplayName
        {
            get { return "Filtering Loot"; }
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
                    || Conditions.IsAttackedAndCannotIgnore || Main.amountProfessionsSelected <= 0 
                    || !WholesomeProfessionsSettings.CurrentSetting.FilterLoot)
                    return false;
                
                if (Main.primaryProfession.ShouldFilterLoot())
                {
                    profession = Main.primaryProfession;
                    return true;
                }
                if (Main.secondaryProfession.ShouldFilterLoot())
                {
                    profession = Main.secondaryProfession;
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
            Logger.LogDebug("************ RUNNING FILTER LOOT STATE ************");
            MovementManager.StopMoveNewThread();

            // Deactivate broadcast
            Broadcaster.autoBroadcast = false;
            Logger.Log($"Deleting {profession.ItemToDelete.Name}");
            ToolBox.DeleteItemByName(profession.ItemToDelete);

            Broadcaster.autoBroadcast = true;
        }
    }
}
