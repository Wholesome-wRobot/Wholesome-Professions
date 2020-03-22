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
                    || Conditions.IsAttackedAndCannotIgnore || Main.amountProfessionsSelected <= 0)
                    return false;

                if (Main.primaryProfession.ShouldSetCurrentStep())
                {
                    profession = Main.primaryProfession;
                    return true;
                }
                if (Main.secondaryProfession.ShouldSetCurrentStep())
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
            Logger.LogDebug("************ RUNNING SET CURRENT STEP STATE ************");
            Logger.Log("Calculating next step. Please wait...");
            Broadcaster.autoBroadcast = false;
            
            // Reset current profile if there is one loaded
            ProfileHandler.UnloadCurrentProfile(profession);

            Step selectedStep = null;
            int currentLevel = ToolBox.GetProfessionLevel(profession.ProfessionName);

            // First check if we need a prerequisite item
            Logger.LogDebug($"*** Checking for prerequisite items step");
            if (profession.CurrentStep != null && profession.PrerequisiteItems.Count > 0)
            {
                foreach(Item item in profession.PrerequisiteItems)
                {
                    if (ItemsManager.GetItemCountById(item.ItemId) < 1)
                    {
                        Logger.LogDebug($"We need 1 {item.Name} to proceed");
                        profession.AddGeneratedStep(new Step(item, 1));
                        return;
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
                // Log current step information
                if (selectedStep.Type == Step.StepType.CraftAll)
                    Logger.LogDebug($"SELECTED STEP : Craft all {selectedStep.ItemoCraft.Name} x {selectedStep.EstimatedAmountOfCrafts}");
                else if (selectedStep.Type == Step.StepType.CraftToLevel)
                {
                    Logger.LogDebug($"SELECTED STEP : Craft to level {selectedStep.ItemoCraft.Name} x {selectedStep.GetRemainingProfessionLevels()}");
                    selectedStep.EstimatedAmountOfCrafts = selectedStep.GetRemainingProfessionLevels();
                }

                // If the selected step is a forced list, we generate and reset
                if (selectedStep.Type == Step.StepType.ListPreCraft)
                {
                    Logger.LogDebug("ADDING FORCED CRAFT");
                    profession.AddGeneratedStep(new Step(selectedStep.ItemoCraft, ItemHelper.GetTotalNeededMat(selectedStep.ItemoCraft, profession)));
                    return;
                }

                // If the selected step requires a precraft, we generate and reset
                foreach (Item.Mat materialToPreCraft in selectedStep.ItemoCraft.Materials)
                {
                    if (!materialToPreCraft.Item.CanBeBought && !materialToPreCraft.Item.CanBeFarmed)
                    {
                        int amountMatNeeded = ItemHelper.GetTotalNeededMat(materialToPreCraft.Item, profession);
                        Logger.LogDebug($"We need to PRECRAFT {amountMatNeeded} {materialToPreCraft.Item.Name}");
                        if (amountMatNeeded > 0)
                        {
                            profession.AddGeneratedStep(new Step(materialToPreCraft.Item, amountMatNeeded));
                            return;
                        }
                    }
                }

                if (ItemHelper.NeedToFarmItemFor(selectedStep.ItemoCraft, profession))
                    Logger.LogDebug($"{profession.AmountOfItemToFarm} more {profession.ItemToFarm.Name} needed");

                // Set the knowRecipe flag of the selected step
                selectedStep.KnownRecipe = ToolBox.RecipeIsKnown(selectedStep.ItemoCraft.Name, profession.ToString());
                Logger.LogDebug($"Recipe is known : {selectedStep.KnownRecipe}");

                profession.CurrentStep = selectedStep;
            }

            profession.HasSetCurrentStep = true;
            Broadcaster.autoBroadcast = true;
            Broadcaster.BroadCastSituation(true);
        }
    }
}
