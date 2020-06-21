using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Items;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class SetCurrentStepState : State
    {
        public override string DisplayName
        {
            get { return "Setting current step"; }
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

                if (Main.primaryProfession.ShouldSetCurrentStep())
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
            Logger.LogDebug("************ RUNNING SET CURRENT STEP STATE ************");
            Logger.Log("Calculating next step. Please wait...");
            Broadcaster.autoBroadcast = false;
            
            // Reset current profile if there is one loaded
            ProfileHandler.UnloadCurrentProfile();

            Step selectedStep = null;
            int currentLevel = ToolBox.GetProfessionLevel(profession.Name);
            
            // Check if user action is required
            if (profession.CurrentStep != null && profession.CurrentStep.ItemoCraft.UserMustBuyManually 
                && !profession.UserMustBuyManuallyFlag)
            {
                profession.UserMustBuyManuallyFlag = true;
                return;
            }
            
            // Check if we need a prerequisite item
            if (profession.PrerequisiteItems.Count > 0)
            {
                foreach (Item item in profession.PrerequisiteItems)
                {
                    if (ItemsManager.GetItemCountById(item.ItemId) < 1)
                    {
                        Step stepToAdd = new Step(profession, item, 1);
                        if (!profession.AllSteps.Exists(s => s.ItemoCraft.Name == item.Name))
                        {
                            Logger.LogDebug($"Adding prerequisite step {item}");
                            profession.AddGeneratedStep(stepToAdd);
                            return;
                        }
                    }
                }
            }
            
            // Search for Priority Steps
            Logger.LogDebug($"*** Checking for priority steps");
            foreach (Step step in profession.AllSteps)
            {
                Logger.LogDebug($"Checking {step.ItemoCraft.Name}");
                if (step.Type == Step.StepType.CraftAll)
                {
                    selectedStep = step;
                    break;
                }
            }

            if (selectedStep == null)
            {
                // Search for Normal Steps
                Logger.LogDebug($"*** Checking for normal steps");
                foreach (Step step in profession.AllSteps)
                {
                    Logger.LogDebug($"Checking {step.ItemoCraft.Name}");
                    if (step.Type != Step.StepType.CraftAll && currentLevel >= step.Minlevel && currentLevel < step.LevelToReach)
                    {
                        selectedStep = step;
                        break;
                    }
                }
            }

            if (selectedStep == null)
            {
                Logger.LogDebug("No step selected");
                profession.CurrentStep = null;
            }
            else
            {
                // If the selected step is a forced list, we generate and reset
                if (selectedStep.Type == Step.StepType.ListPreCraft)
                {
                    Logger.LogDebug("ADDING FORCED CRAFT");
                    profession.AddGeneratedStep(new Step(profession, selectedStep.ItemoCraft, ItemHelper.GetTotalNeededMat(profession, selectedStep.ItemoCraft)));
                    return;
                }

                // Log current step information
                if (selectedStep.Type == Step.StepType.CraftAll)
                    Logger.LogDebug($"SELECTED STEP : Craft all {selectedStep.ItemoCraft.Name} x {selectedStep.EstimatedAmountOfCrafts}");
                else if (selectedStep.Type == Step.StepType.CraftToLevel)
                {
                    Logger.LogDebug($"SELECTED STEP : Craft to level {selectedStep.ItemoCraft.Name} x {selectedStep.GetRemainingProfessionLevels()}");
                    selectedStep.EstimatedAmountOfCrafts = selectedStep.GetRemainingProfessionLevels();
                }

                // If the selected step requires a precraft, we generate and reset
                foreach (Item.Mat materialToPreCraft in selectedStep.ItemoCraft.Materials)
                {
                    if (!materialToPreCraft.Item.CanBeBought && !materialToPreCraft.Item.CanBeFarmed)
                    {
                        int amountMatNeeded = ItemHelper.GetTotalNeededMat(profession, materialToPreCraft.Item);
                        if (amountMatNeeded > 0)
                        {
                            Logger.LogDebug($"We need to PRECRAFT {amountMatNeeded} {materialToPreCraft.Item.Name}");
                            profession.AddGeneratedStep(new Step(profession, materialToPreCraft.Item, amountMatNeeded));
                            return;
                        }
                    }
                }

                profession.CurrentStep = selectedStep;

                // Set the amount of items needed to farm
                ItemHelper.CalculateFarmAmountFor(profession, selectedStep.ItemoCraft);
                if (profession.AmountOfItemToFarm > 0)
                    Logger.LogDebug($"{profession.AmountOfItemToFarm} more {profession.ItemToFarm.Name} needed");


                // Set the knowRecipe flag of the selected step
                profession.CurrentStep.KnownRecipe = ToolBox.RecipeIsKnown(selectedStep.ItemoCraft.Name, profession);
                Logger.LogDebug($"Recipe is known : {selectedStep.KnownRecipe}");
            }

            profession.MustRecalculateStepFlag = false;
            Broadcaster.autoBroadcast = true;
            Broadcaster.BroadCastSituation(true);
        }
    }
}
