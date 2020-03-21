using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Items;
using Wholesome_Professions_WotlK.Profile;
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

        public override bool NeedToRun
        {
            get
            {
                if (!Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause || !ObjectManager.Me.IsValid
                    || Conditions.IsAttackedAndCannotIgnore || Main.amountProfessionsSelected <= 0)
                    return false;

                if (Main.primaryProfession.ShouldSetCurrentStep())
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
            Logger.LogDebug("************ RUNNING SET CURRENT STEP STATE ************");
            Logger.Log("Calculating next step. Please wait...");
            Broadcaster.autoBroadcast = false;
            
            // Reset current profile if there is one loaded
            ProfileHandler.UnloadCurrentProfile();

            Step selectedStep = null;
            IProfession profession = Main.primaryProfession;
            int currentLevel = ToolBox.GetProfessionLevel(profession.ProfessionName);

            // Search for Priority Steps
            Logger.LogDebug($"*** Checking for priority steps");
            foreach (Step step in profession.AllSteps)
            {
                Logger.LogDebug($"Checking {step.itemoCraft.name}");
                if (step.stepType == Step.StepType.CraftAll)
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
                    Logger.LogDebug($"Checking {step.itemoCraft.name}");
                    if (step.stepType != Step.StepType.CraftAll && currentLevel >= step.minlevel && currentLevel < step.levelToReach)
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
                if (selectedStep.stepType == Step.StepType.CraftAll)
                    Logger.LogDebug($"SELECTED STEP : Craft all {selectedStep.itemoCraft.name} x {selectedStep.estimatedAmountOfCrafts}");
                else if (selectedStep.stepType == Step.StepType.CraftToLevel)
                {
                    Logger.LogDebug($"SELECTED STEP : Craft to level {selectedStep.itemoCraft.name} x {selectedStep.GetRemainingProfessionLevels()}");
                    selectedStep.estimatedAmountOfCrafts = selectedStep.GetRemainingProfessionLevels();
                }

                // If the selected step is a forced list, we generate and reset
                if (selectedStep.stepType == Step.StepType.ListPreCraft)
                {
                    Logger.LogDebug("ADDING FORCED CRAFT");
                    profession.AddGeneratedStep(new Step(selectedStep.itemoCraft, ItemHelper.GetTotalNeededMat(selectedStep.itemoCraft, profession)));
                    return;
                }

                // If the selected step requires a precraft, we generate and reset
                foreach (Item.Mat materialToPreCraft in selectedStep.itemoCraft.Materials)
                {
                    if (!materialToPreCraft.item.canBeBought && !materialToPreCraft.item.canBeFarmed)
                    {
                        int amountMatNeeded = ItemHelper.GetTotalNeededMat(materialToPreCraft.item, profession);
                        Logger.LogDebug($"We need to PRECRAFT {amountMatNeeded} {materialToPreCraft.item.name}");
                        if (amountMatNeeded > 0)
                        {
                            profession.AddGeneratedStep(new Step(materialToPreCraft.item, amountMatNeeded));
                            return;
                        }
                    }
                }

                if (ItemHelper.NeedToFarmItemFor(selectedStep.itemoCraft, profession))
                    Logger.LogDebug($"{profession.AmountOfItemToFarm} more {profession.ItemToFarm.name} needed");

                // Set the knowRecipe flag of the selected step
                selectedStep.knownRecipe = ToolBox.RecipeIsKnown(selectedStep.itemoCraft.name, profession.ToString());
                Logger.LogDebug($"Recipe is known : {selectedStep.knownRecipe}");

                profession.CurrentStep = selectedStep;
            }

            Main.primaryProfession.HasSetCurrentStep = true;
            Broadcaster.autoBroadcast = true;
            Broadcaster.BroadCastSituation(true);
        }
    }
}
