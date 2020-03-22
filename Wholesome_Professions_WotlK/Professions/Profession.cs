using System.Collections.Generic;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Items;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class Profession : IProfession
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
    public List<Item> PrerequisiteItems { get; set; } = new List<Item>();

    // Flags
    public bool HasSetCurrentStep { get; set; }

    public Item ItemToFarm { get; set; }
    public int AmountOfItemToFarm { get; set; }
    
    protected Profession(SkillLine professionName)
    {
        CurrentStep = null;
        CurrentProfile = null;
        ProfessionName = professionName;

        // Manage sell list
        ToolBox.ManageSellList(AllSteps);

        // Reset save if prof level is 0
        if (ToolBox.GetProfessionLevel(ProfessionName) == 0)
            ToolBox.ClearProfessionFromSavedList(ProfessionName.ToString());
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
        if (CurrentStep == null)
            return false;

        return CurrentStep.CanBuyRemainingMats() && !CurrentStep.HasMatsToCraftOne() && MyLevelIsHighEnough();
    }

    // Should travel
    public bool ShouldTravel()
    {
        if (CurrentStep == null)
            return false;

        Logger.LogDebug($"You are on continent {Usefuls.ContinentId}, you should be on {Continent}, your level is enough ? : {MyLevelIsHighEnough()}");
        return Continent != Usefuls.ContinentId && MyLevelIsHighEnough();
    }

    // Should learn recipe from trainer
    public bool ShouldLearnRecipeFromTrainer()
    {
        if (CurrentStep == null)
            return false;

        var RecipeVendor = CurrentStep.ItemoCraft.RecipeVendor;
        return !CurrentStep.KnownRecipe && RecipeVendor == null && MyLevelIsHighEnough();
    }

    // Should buy and learn recipe
    public bool ShouldBuyAndLearnRecipe()
    {
        if (CurrentStep == null)
            return false;

        var RecipeVendor = CurrentStep.ItemoCraft.RecipeVendor;
        return !CurrentStep.KnownRecipe && RecipeVendor != null && MyLevelIsHighEnough();
    }

    // Should learn profession
    public bool ShouldLearnProfession()
    {
        if (CurrentStep == null)
            return false;

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
        if (CurrentStep == null || CurrentStep.Type == Step.StepType.ListPreCraft || !MyLevelIsHighEnough())
            return false;

        // If items needed to farm
        if (ItemHelper.NeedToFarmItemFor(CurrentStep.ItemoCraft, this))
            return false;

        // Craft 
        if (CurrentStep.Type == Step.StepType.CraftAll || CurrentStep.Type == Step.StepType.CraftToLevel)
        {
            Logger.LogDebug($"Should run {CurrentStep.Type.ToString()} {CurrentStep.ItemoCraft.Name}");
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

    public void SetContext() { }
    public void RegenerateSteps() { }
}
