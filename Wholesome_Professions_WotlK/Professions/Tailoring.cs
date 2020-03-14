using System.Collections.Generic;
using System.Diagnostics;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Items;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class Tailoring : IProfession
{
    public SkillLine ProfessionName { get; set; }
    public Step CurrentStep { get; set; }
    public List<Step> AllSteps { get; set; } = new List<Step>();

    public Npc ProfessionTrainer { get; set; } = new Npc();
    public Npc SuppliesVendor { get; set; } = new Npc();
    public int Continent { get; set; }
    public int MinimumCharLevel { get; set; }
    public string ProfessionSpell { get; set; }

    public Item ItemToFarm { get; set; }
    public int AmountOfItemToFarm { get; set; }

    public Tailoring()
    {
        CurrentStep = null;
        ProfessionName = SkillLine.Tailoring;
        RegenerateSteps();

        // Reset save if prof level is 0
        if (ToolBox.GetProfessionLevel(ProfessionName) == 0)
            ToolBox.ClearProfessionFromSavedList(ProfessionName.ToString());
    }

    public void RegenerateSteps()
    {
        SetTrainer();

        AllSteps.Clear();

        AllSteps.Add(new Step(0, 45, ItemDB.BoltOfLinenCloth)); // Force precraft
        AllSteps.Add(new Step(45, 70, ItemDB.HeavyLinenGloves, 35));
        AllSteps.Add(new Step(70, 75, ItemDB.ReinforcedLinenCape, 5));
        AllSteps.Add(new Step(75, 97, ItemDB.BoltofWoolenCloth)); // Force precraft
        AllSteps.Add(new Step(97, 110, ItemDB.SimpleKilt, 15));
        AllSteps.Add(new Step(110, 125, ItemDB.DoublestitchedWoolenShoulders, 15));
        AllSteps.Add(new Step(125, 145, ItemDB.BoltOfSilkCloth)); // Force precraft
        AllSteps.Add(new Step(145, 150, ItemDB.AzureSilkHood, 5));
        AllSteps.Add(new Step(150, 160, ItemDB.AzureSilkHood, 15));
        AllSteps.Add(new Step(160, 170, ItemDB.SilkHeadband, 10));
        AllSteps.Add(new Step(170, 175, ItemDB.FormalWhiteShirt, 5));
        AllSteps.Add(new Step(175, 185, ItemDB.BoltOfMageweave)); // Force precraft
        AllSteps.Add(new Step(185, 200, ItemDB.CrimsonSilkVest, 15));
        AllSteps.Add(new Step(200, 215, ItemDB.CrimsonSilkPantaloons, 15));
        AllSteps.Add(new Step(215, 220, ItemDB.BlackMageweaveLeggings, 5));
        AllSteps.Add(new Step(220, 225, ItemDB.BlackMageweaveGloves, 5));
        AllSteps.Add(new Step(225, 230, ItemDB.BlackMageweaveGloves, 5));
        AllSteps.Add(new Step(230, 250, ItemDB.BlackMageweaveHeadband, 23));
        AllSteps.Add(new Step(250, 260, ItemDB.BoltofRunecloth)); // Force precraft
        AllSteps.Add(new Step(260, 280, ItemDB.RuneclothBelt, 25));
        AllSteps.Add(new Step(280, 295, ItemDB.RuneclothGloves, 18));
        AllSteps.Add(new Step(295, 300, ItemDB.RuneclothHeadband, 5));
        AllSteps.Add(new Step(300, 325, ItemDB.BoltofNetherweave)); // Force precraft
        AllSteps.Add(new Step(325, 335, ItemDB.BoltofImbuedNetherweave, 15));
        AllSteps.Add(new Step(335, 345, ItemDB.NetherweaveBoots, 10));
        AllSteps.Add(new Step(345, 350, ItemDB.NetherweaveTunic, 5));
    }

    public bool ShouldCraft()
    {
        Logger.LogDebug("************* Checking if should craft");

        SetCurrentStep();

        // If no step has been selected
        if (CurrentStep == null)
        {
            Broadcaster.ClearAndAddBroadCastMessage("No progression is currently possible");
            return false;
        }

        // If we don't have the minimum level
        if (ObjectManager.Me.Level < MinimumCharLevel)
        {
            Broadcaster.AddBroadCastMessage($"You must be at least level {MinimumCharLevel} to progress");
            return false;
        }

        // If current step is a forced pre craft from the list, we generate a craft all
        if (CurrentStep.stepType == Step.StepType.ListPreCraft)
        {
            Logger.LogDebug("ADDING FORCED CRAFT");
            AllSteps.Add(new Step(CurrentStep.itemoCraft, ItemHelper.GetTotalNeededMat(CurrentStep.itemoCraft, this)));
            return false;
        }

        if (ItemToFarm != null)
            return false;

        // If current step requires a precraft
        foreach (Item.Mat materialToPreCraft in CurrentStep.itemoCraft.Materials)
        {
            if (!materialToPreCraft.item.canBeBought && !materialToPreCraft.item.canBeFarmed)
            {
                int amountMatNeeded = ItemHelper.GetTotalNeededMat(materialToPreCraft.item, this);
                Logger.LogDebug($"We need to PRECRAFT {amountMatNeeded} {materialToPreCraft.item.name}");
                if (amountMatNeeded > 0)
                {
                    AllSteps.Add(new Step(materialToPreCraft.item, amountMatNeeded));
                    return false;
                }
            }
        }

        // Craft all
        if (CurrentStep.stepType == Step.StepType.CraftAll)
        {
            Logger.LogDebug($"Should run {CurrentStep.stepType.ToString()} {CurrentStep.itemoCraft.name}");
            return true;
        }

        // Craft to level
        if (CurrentStep.stepType == Step.StepType.CraftToLevel)
        {
            Logger.LogDebug($"Should run {CurrentStep.stepType.ToString()} {CurrentStep.itemoCraft.name}");
            return true;
        }

        Logger.LogDebug("No step to run after check");
        return false;
    }

    public void SetCurrentStep()
    {
        Logger.LogDebug("************* Setting current Step");
        int currentLevel = ToolBox.GetProfessionLevel(ProfessionName);

        // Manage sell list
        ManageSellList();

        // Search for Priority Steps
        Logger.LogDebug($"*** Checking for priority steps");
        foreach (Step step in AllSteps)
        {
            Logger.LogDebug($"Checking {step.itemoCraft.name}");

            // Search for priority step
            if (step.stepType == Step.StepType.CraftAll)
            {
                List<string> groupedMessages = new List<string>();
                groupedMessages.Add($"STEP : Craft all {step.itemoCraft.name} x {step.estimatedAmountOfCrafts}");
                Logger.LogDebug($"STEP : Craft all {step.itemoCraft.name} x {step.estimatedAmountOfCrafts}");

                CurrentStep = step;

                if (ItemHelper.NeedToFarmItemFor(CurrentStep.itemoCraft, this))
                    groupedMessages.Add($"You need {AmountOfItemToFarm} more {ItemToFarm.name} in your bags to proceed");

                Broadcaster.ClearAndAddBroadCastMessagesList(groupedMessages);
                return;
            }
        }

        // Search for Normal Steps
        Logger.LogDebug($"*** Checking for normal steps");
        foreach (Step step in AllSteps)
        {
            Logger.LogDebug($"Checking {step.itemoCraft.name}");

            // Search for craft to level step
            if (step.stepType != Step.StepType.CraftAll && currentLevel >= step.minlevel && currentLevel < step.levelToReach)
            {
                List<string> groupedMessages = new List<string>();
                groupedMessages.Add($"STEP : Craft {step.itemoCraft.name} to reach level {step.levelToReach}");
                Logger.LogDebug($"STEP : Craft to level {step.itemoCraft.name} x {step.GetRemainingProfessionLevels()}");

                CurrentStep = step;

                if (ItemHelper.NeedToFarmItemFor(CurrentStep.itemoCraft, this))
                    groupedMessages.Add($"You need {AmountOfItemToFarm} more {ItemToFarm.name} in your bags to proceed");

                Broadcaster.ClearAndAddBroadCastMessagesList(groupedMessages);
                return;
            }
        }

        Logger.LogDebug("No step selected");
        CurrentStep = null;
    }

    private void ManageSellList()
    {
        foreach (Step step in AllSteps)
        {
            wManager.wManagerSetting.CurrentSetting.Selling = false;
            wManager.Wow.Bot.States.ToTown.ForceToTown = false;
            ToolBox.RemoveFromSellAndNotSellList(step.itemoCraft);
            if (step.itemoCraft.forceSell)
                ToolBox.AddItemToSellList(step.itemoCraft);
        }
    }

    public void SetTrainer()
    {
        // ek = 0, kalimdor = 1, Outlands = 530, Northrend 571
        int profLevel = ToolBox.GetProfessionLevel(ProfessionName);

        if (ObjectManager.Me.Faction == 116) // Horde
        {
            if (profLevel < 75)
            {
                ProfessionSpell = "Apprentice Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
            }
            else if (profLevel >= 75 && profLevel < 150)
            {
                ProfessionSpell = "Journeyman Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
            }
            else if (profLevel >= 150 && profLevel < 225)
            {
                ProfessionSpell = "Expert Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
            }
            else if (profLevel >= 225 && profLevel < 300) // requires lvl 35
            {
                MinimumCharLevel = 35;
                ProfessionSpell = "Artisan Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
            }
            else if (profLevel >= 300 && profLevel < 350) // requires lvl 58 min
            {
                MinimumCharLevel = 58;
                ProfessionSpell = "Master Tailor";
                ProfessionTrainer = VendorDB.ThrallmarTailoringTrainer;
                SuppliesVendor = VendorDB.ShattrathTailoringSupplies;
                Continent = 530;
            }
        }
    }
}
