using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Enums;
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
        private IProfession profession;

        public override bool NeedToRun
        {
            get
            {
                if (!Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause || !ObjectManager.Me.IsValid
                    || Conditions.IsAttackedAndCannotIgnore || Main.amountProfessionsSelected <= 0)
                    return false;
                
                if (Main.primaryProfession.ShouldCraft())
                {
                    profession = Main.primaryProfession;
                    return true;
                }
                if (Main.secondaryProfession.ShouldCraft())
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
            Logger.LogDebug("************ RUNNING CRAFT STATE ************");
            Step currentStep = profession.CurrentStep;

            MovementManager.StopMoveNewThread();

            // Deactivate broadcast
            Broadcaster.autoBroadcast = false;

            //Debugger.Launch();
            //Debugger.Break();

            // amountAlreadyCrafted NOT USED ANYMORE ?????
            //int amountAlreadyCrafted = ToolBox.GetAlreadyCrafted(profession.ProfessionName.ToString(), currentStep.itemoCraft.name);
            //Logger.Log($"We've already crafted {amountAlreadyCrafted} {currentStep.itemoCraft.name}");

            // Craft
            int itemInBagsBeforeCraft = ItemsManager.GetItemCountById(currentStep.ItemoCraft.ItemId);
            int amountToCraft = 0;

            if (currentStep.Type == Step.StepType.CraftAll)
                amountToCraft = currentStep.EstimatedAmountOfCrafts;
            else if (currentStep.Type == Step.StepType.CraftToLevel)
                amountToCraft = currentStep.GetRemainingProfessionLevels();

            int goalAmount = amountToCraft + itemInBagsBeforeCraft;

            ToolBox.Craft(profession.Name.ToString(), currentStep.ItemoCraft, amountToCraft);
            Thread.Sleep(100);
            while (ItemsManager.GetItemCountById(currentStep.ItemoCraft.ItemId) < goalAmount && currentStep.HasMatsToCraftOne()
                && Bag.GetContainerNumFreeSlots > 1)
            {
                // Log
                if (ItemsManager.GetItemCountById(currentStep.ItemoCraft.ItemId) != itemInBagsBeforeCraft)
                {
                    itemInBagsBeforeCraft = ItemsManager.GetItemCountById(currentStep.ItemoCraft.ItemId);
                    if (currentStep.Type == Step.StepType.CraftAll)
                        Logger.Log($"Crafted : {currentStep.ItemoCraft.Name} {itemInBagsBeforeCraft}/{goalAmount}");
                    else if (currentStep.Type == Step.StepType.CraftToLevel)
                    {
                        Logger.Log($"Craft {currentStep.ItemoCraft.Name} until level up : " +
                            $"{ToolBox.GetProfessionLevel(profession.Name)}/{currentStep.LevelToReach}");
                        // record item
                        ToolBox.AddCraftedItemToSettings(profession.Name.ToString(), currentStep.ItemoCraft);
                    }
                }
                Thread.Sleep(200);

                // Failsafe if craft is interrupted
                if (!ObjectManager.Me.IsCast)
                {
                    Thread.Sleep(200);
                    if (!ObjectManager.Me.IsCast)
                    {
                        Logger.Log("The craft has been interrupted");
                        break;
                    }
                }
            }

            Logger.Log("Craft complete");
            ToolBox.CloseProfessionFrame();
            Lua.RunMacroText("/stopcasting");
            profession.RegenerateSteps();

            Broadcaster.autoBroadcast = true;
        }
    }
}
