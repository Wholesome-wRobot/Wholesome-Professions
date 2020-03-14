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

    // Flags
    public bool ShouldCraftFlag { get; set; }
    public bool HasSetCurrentStep { get; set; }

    public Item ItemToFarm { get; set; }
    public int AmountOfItemToFarm { get; set; }

    public Tailoring()
    {
        CurrentStep = null;
        ProfessionName = SkillLine.Tailoring;
        RegenerateSteps();
        SetTrainer();

        // Reset save if prof level is 0
        if (ToolBox.GetProfessionLevel(ProfessionName) == 0)
            ToolBox.ClearProfessionFromSavedList(ProfessionName.ToString());
    }

    public void RegenerateSteps()
    {
        Logger.LogDebug("REGENERATING STEPS");
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
        AllSteps.Add(new Step(260, 280, ItemDB.RuneclothBelt, 30));
        AllSteps.Add(new Step(280, 295, ItemDB.RuneclothGloves, 20));
        AllSteps.Add(new Step(295, 300, ItemDB.RuneclothHeadband, 5));
        AllSteps.Add(new Step(300, 325, ItemDB.BoltofNetherweave)); // Force precraft
        AllSteps.Add(new Step(325, 335, ItemDB.BoltofImbuedNetherweave, 15));
        AllSteps.Add(new Step(335, 345, ItemDB.NetherweaveBoots, 10));
        AllSteps.Add(new Step(345, 350, ItemDB.NetherweaveTunic, 5));

        HasSetCurrentStep = false;
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
        return Continent != Usefuls.ContinentId && MyLevelIsHighEnough();
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
        return ToolBox.GetProfessionLevel(ProfessionName) == ToolBox.GetProfessionMaxLevel(ProfessionName) && MyLevelIsHighEnough();
    }

    // Should sell items
    public bool ShouldSellItems()
    {
        return Bag.GetContainerNumFreeSlots <= 1;
    }

    // Should craft
    public bool ShouldCraft()
    {
        bool shouldCraft = CheckIfShouldCraft();
        ShouldCraftFlag = shouldCraft;
        return ShouldCraftFlag;
    }

    private bool CheckIfShouldCraft()
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

    public void SetTrainer()
    {
        // ek = 0, kalimdor = 1, Outlands = 530, Northrend 571
        int profLevel = ToolBox.GetProfessionLevel(ProfessionName);

        if (ObjectManager.Me.Faction == 116) // Horde
        {
            if (profLevel < 75)
            {
                Continent = 1;
                ProfessionSpell = "Apprentice Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
            }
            else if (profLevel >= 75 && profLevel < 150)
            {
                Continent = 1;
                ProfessionSpell = "Journeyman Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
            }
            else if (profLevel >= 150 && profLevel < 225)
            {
                Continent = 1;
                ProfessionSpell = "Expert Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
            }
            else if (profLevel >= 225 && profLevel < 300)
            {
                Continent = 1;
                MinimumCharLevel = 35;
                ProfessionSpell = "Artisan Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
            }
            else if (profLevel >= 300 && profLevel < 350)
            {
                Continent = 530;
                MinimumCharLevel = 58;
                ProfessionSpell = "Master Tailor";
                ProfessionTrainer = VendorDB.ThrallmarTailoringTrainer;
                SuppliesVendor = VendorDB.ShattrathTailoringSupplies;
            }
        }
    }
}
