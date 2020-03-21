using System.Collections.Generic;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Items;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class Enchanting : IProfession
{
    public SkillLine ProfessionName { get; set; }
    public Step CurrentStep { get; set; }
    public List<Step> AllSteps { get; set; } = new List<Step>();

    public Npc ProfessionTrainer { get; set; } = new Npc();
    public Npc SuppliesVendor { get; set; } = new Npc();
    public int Continent { get; set; }
    public int MinimumCharLevel { get; set; }
    public string ProfessionSpell { get; set; }
    public string CurrentProfile { get; set; }

    // Flags
    public bool HasSetCurrentStep { get; set; }

    public Item ItemToFarm { get; set; }
    public int AmountOfItemToFarm { get; set; }

    public Enchanting()
    {
        CurrentStep = null;
        CurrentProfile = null;
        ProfessionName = SkillLine.Enchanting;
        RegenerateSteps();
        SetContext();

        // Manage sell list
        ToolBox.ManageSellList(AllSteps);

        // Reset save if prof level is 0
        if (ToolBox.GetProfessionLevel(ProfessionName) == 0)
            ToolBox.ClearProfessionFromSavedList(ProfessionName.ToString());
    }

    public void RegenerateSteps()
    {
        Logger.LogDebug("REGENERATING STEPS");
        SetContext();

        AllSteps.Clear();

        AllSteps.Add(new Step(0, 45, ItemDB.BoltOfLinenCloth)); // Force precraft
        AllSteps.Add(new Step(45, 70, ItemDB.HeavyLinenGloves, 35));

        HasSetCurrentStep = false;
    }

    // Should Select profile
    public bool ShouldSelectProfile()
    {
        return AmountOfItemToFarm > 0 && ItemToFarm != null && CurrentProfile == null;
    }

    // Should set current step
    public bool ShouldSetCurrentStep()
    {
        return !HasSetCurrentStep;
    }

    // Should buy Materials
    public bool ShouldBuyMaterials()
    {
        return CurrentStep.CanBuyRemainingMats() && !CurrentStep.HasMatsToCraftOne() && MyLevelIsHighEnough();
    }

    // Should travel
    public bool ShouldTravel()
    {
        Logger.LogDebug($"You are on continent {Usefuls.ContinentId}, you should be on {Continent}, your level is enough ? : {MyLevelIsHighEnough()}");
        return Continent != Usefuls.ContinentId && MyLevelIsHighEnough() /*&& CurrentProfile != null*/;
    }

    // Should learn recipe from trainer
    public bool ShouldLearnRecipeFromTrainer()
    {
        var RecipeVendor = CurrentStep.itemoCraft.RecipeVendor;
        return !CurrentStep.knownRecipe && RecipeVendor == null && MyLevelIsHighEnough();
    }

    // Should buy and learn recipe
    public bool ShouldBuyAndLearnRecipe()
    {
        var RecipeVendor = CurrentStep.itemoCraft.RecipeVendor;
        return !CurrentStep.knownRecipe && RecipeVendor != null && MyLevelIsHighEnough();
    }

    // Should learn profession
    public bool ShouldLearnProfession()
    {
        return (ToolBox.GetProfessionLevel(ProfessionName) >= 0 && ToolBox.GetProfessionMaxLevel(ProfessionName) < 75
            || ToolBox.GetProfessionLevel(ProfessionName) >= 75 && ToolBox.GetProfessionMaxLevel(ProfessionName) < 150
            || ToolBox.GetProfessionLevel(ProfessionName) >= 150 && ToolBox.GetProfessionMaxLevel(ProfessionName) < 225
            || ToolBox.GetProfessionLevel(ProfessionName) >= 225 && ToolBox.GetProfessionMaxLevel(ProfessionName) < 300
            || ToolBox.GetProfessionLevel(ProfessionName) >= 300 && ToolBox.GetProfessionMaxLevel(ProfessionName) < 350
            || ToolBox.GetProfessionLevel(ProfessionName) >= 350 && ToolBox.GetProfessionMaxLevel(ProfessionName) < 450)
            && MyLevelIsHighEnough();
    }

    // Should sell items
    public bool ShouldSellItems()
    {
        return Bag.GetContainerNumFreeSlots <= 1;
    }

    // Should craft One
    public bool ShouldCraftOne()
    {
        return CurrentStep != null && CurrentProfile != null && CurrentStep.HasMatsToCraftOne();
    }

    // Should craft
    public bool ShouldCraft()
    {
        // If basic conditions are not met
        if (CurrentStep == null || CurrentStep.stepType == Step.StepType.ListPreCraft || !MyLevelIsHighEnough())
            return false;

        // If items needed to farm
        if (ItemHelper.NeedToFarmItemFor(CurrentStep.itemoCraft, this))
            return false;

        // Craft 
        if (CurrentStep.stepType == Step.StepType.CraftAll || CurrentStep.stepType == Step.StepType.CraftToLevel)
        {
            Logger.LogDebug($"Should run {CurrentStep.stepType.ToString()} {CurrentStep.itemoCraft.name}");
            return true;
        }

        Logger.Log("WARNING: No step to run after check");
        return false;
    }

    public bool MyLevelIsHighEnough()
    {
        return ObjectManager.Me.Level >= MinimumCharLevel;
    }

    public void AddGeneratedStep(Step step)
    {
        AllSteps.Add(step);
        HasSetCurrentStep = false;
    }

    public void SetContext()
    {
        // ek = 0, kalimdor = 1, Outlands = 530, Northrend 571
        int profLevel = ToolBox.GetProfessionLevel(ProfessionName);

        if (ToolBox.IsHorde()) // Horde
        {
            if (profLevel < 75)
            {
                MinimumCharLevel = 20;
                Continent = (int)ContinentId.Kalimdor;
                ProfessionSpell = "Apprentice Enchanter";
                ProfessionTrainer = VendorDB.OGEnchantingTrainer;
                SuppliesVendor = VendorDB.OGEnchantingSupplies;
            }
        }
    }
}
