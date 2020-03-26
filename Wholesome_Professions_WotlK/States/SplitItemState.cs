using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Items;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class SplitItemState : State
    {
        public override string DisplayName
        {
            get { return "Spliting Item"; }
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
                    || Conditions.IsAttackedAndCannotIgnore || Main.amountProfessionsSelected <= 0 || ObjectManager.Me.MountDisplayId != 0)
                    return false;
                
                if (Main.primaryProfession.ShouldSplitItem())
                {
                    profession = Main.primaryProfession;
                    return true;
                }
                if (Main.secondaryProfession.ShouldSplitItem())
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
            Logger.LogDebug("************ RUNNING SPLIT ITEM STATE ************");

            if (profession.ItemToSplit != null)
                ItemsManager.UseContainerItemByNameOrId(profession.ItemToSplit);
        }
    }
}
