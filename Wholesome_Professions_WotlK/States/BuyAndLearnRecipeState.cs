using System;
using System.Collections.Generic;
using System.Threading;
using robotManager.FiniteStateMachine;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class BuyAndLearnRecipeState : State
    {
        public override string DisplayName
        {
            get { return "Buying and learning recipe"; }
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

                if (Main.primaryProfession.ShouldBuyAndLearnRecipe())
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
            Logger.LogDebug("************ RUNNING BUY AND LEARN RECIPE STATE ************");
            Broadcaster.autoBroadcast = false;

            Step currentStep = Main.primaryProfession.CurrentStep;
            var RecipeVendor = currentStep.itemoCraft.RecipeVendor;

            Logger.Log($"Buying {currentStep.itemoCraft.name} recipe at NPC {Main.primaryProfession.ProfessionTrainer.Entry}");
            if (GoToTask.ToPositionAndIntecractWithNpc(RecipeVendor.Position, RecipeVendor.Entry, RecipeVendor.GossipOption))
            {
                Vendor.BuyItem(ItemsManager.GetNameById(currentStep.itemoCraft.RecipeItemId), 1);
                Thread.Sleep(2000);

                ItemsManager.UseItemByNameOrId(currentStep.itemoCraft.RecipeItemId.ToString());
                Usefuls.WaitIsCasting();
                Thread.Sleep(300);
            }

            currentStep.knownRecipe = ToolBox.RecipeIsKnown(currentStep.itemoCraft.name, Main.primaryProfession.ProfessionName.ToString());

            Broadcaster.autoBroadcast = true;
        }
    }
}
