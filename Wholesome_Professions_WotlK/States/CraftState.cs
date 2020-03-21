using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class CraftState : State
    {
        public override string DisplayName
        {
            get { return "Crafting"; }
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
                    || Conditions.IsAttackedAndCannotIgnore || Main.amountProfessionsSelected <= 0)
                    return false;

                if (Main.primaryProfession.ShouldCraft())
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
            Logger.LogDebug("************ RUNNING CRAFT STATE ************");
            Step currentStep = Main.primaryProfession.CurrentStep;

            MovementManager.StopMoveNewThread();

            // Deactivate broadcast
            Broadcaster.autoBroadcast = false;

            //Debugger.Launch();
            //Debugger.Break();

            // amountAlreadyCrafted NOT USED ANYMORE ?????
            //int amountAlreadyCrafted = ToolBox.GetAlreadyCrafted(Main.currentProfession.ProfessionName.ToString(), currentStep.itemoCraft.name);
            //Logger.Log($"We've already crafted {amountAlreadyCrafted} {currentStep.itemoCraft.name}");

            // Craft
            int itemInBagsBeforeCraft = ItemsManager.GetItemCountById(currentStep.itemoCraft.itemId);
            int amountToCraft = 0;

            if (currentStep.stepType == Step.StepType.CraftAll)
                amountToCraft = currentStep.estimatedAmountOfCrafts;
            else if (currentStep.stepType == Step.StepType.CraftToLevel)
                amountToCraft = currentStep.GetRemainingProfessionLevels();

            int goalAmount = amountToCraft + itemInBagsBeforeCraft;

            Logger.Log($"Crafting {amountToCraft} x {currentStep.itemoCraft.name}");
            ToolBox.Craft(Main.primaryProfession.ProfessionName.ToString(), currentStep.itemoCraft, amountToCraft);
            Thread.Sleep(100);
            while (ItemsManager.GetItemCountById(currentStep.itemoCraft.itemId) < goalAmount && currentStep.HasMatsToCraftOne()
                && Bag.GetContainerNumFreeSlots > 1)
            {
                // Log
                if (ItemsManager.GetItemCountById(currentStep.itemoCraft.itemId) != itemInBagsBeforeCraft)
                {
                    itemInBagsBeforeCraft = ItemsManager.GetItemCountById(currentStep.itemoCraft.itemId);
                    if (currentStep.stepType == Step.StepType.CraftAll)
                        Logger.Log($"Crafted : {currentStep.itemoCraft.name} {itemInBagsBeforeCraft}/{goalAmount}");
                    else if (currentStep.stepType == Step.StepType.CraftToLevel)
                    {
                        Logger.Log($"Craft {currentStep.itemoCraft.name} until level up : " +
                            $"{ToolBox.GetProfessionLevel(Main.primaryProfession.ProfessionName)}/{currentStep.levelToReach}");
                        // record item
                        ToolBox.AddCraftedItemToSettings(Main.primaryProfession.ProfessionName.ToString(), currentStep.itemoCraft);
                    }
                }
                Thread.Sleep(200);

                // Failsafe if craft is interrupted
                if (!ObjectManager.Me.IsCast)
                {
                    Thread.Sleep(200);
                    if (!ObjectManager.Me.IsCast)
                        break;
                }
            }

            Logger.Log("Craft complete");
            Lua.RunMacroText("/stopcasting");
            Main.primaryProfession.RegenerateSteps();

            Broadcaster.autoBroadcast = true;
        }
    }
}
