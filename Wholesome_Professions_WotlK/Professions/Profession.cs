using System.Collections.Generic;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Items;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using wManager;

public abstract class Profession : IProfession
{
    public SkillLine Name { get; set; }
    public Step CurrentStep { get; set; }
    public List<Step> AllSteps { get; set; } = new List<Step>();

    public Npc ProfessionTrainer { get; set; } = new Npc();
    public Npc SuppliesVendor { get; set; } = new Npc();
    public int MinimumCharLevel { get; set; }
    public string ProfessionSpell { get; set; }
    public string City { get; set; }
    public List<WoWItem> ItemsToDisenchant { get; set; } = new List<WoWItem>();
    public List<Item> PrerequisiteItems { get; set; } = new List<Item>();

    // Flags
    public bool MustRecalculateStep { get; set; }
    public bool HasCheckedIfWeKnowRecipe { get; set; }

    public int Phase { get; set; }
    public Item ItemToFarm { get; set; }
    public int AmountOfItemToFarm { get; set; }
    public WoWItem ItemToDelete { get; set; }
    public string ItemToSplit { get; set; }
    public IProfession OtherProfession { get; set; }

    protected Profession(SkillLine professionName)
    {
        CurrentStep = null;
        Name = professionName;
        ItemToDelete = null;
        ItemToSplit = null;

        // Reset save if prof level is 0
        if (ToolBox.GetProfessionLevel(Name) == 0)
            ToolBox.ClearProfessionFromSavedList(Name.ToString());
    }

    // ************************ Should sell items ************************
    public bool ShouldSellItems()
    {
        // Reset Debug frame since it's the top state
        FrameHelper.ClearDebugString();

        return Bag.GetContainerNumFreeSlots <= 1;
    }

    // ************************ SET CURRENT STEP ************************
    public bool ShouldSetCurrentStep()
    {
        string keyName = $"{Name}.SETSTEP";
        if (!MustRecalculateStep)
        {
            FrameHelper.UpdateDebugFrame(keyName, "Has already set current step");
            return false;
        }
        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    // ************************ SPLIT ITEMS ************************
    public bool ShouldSplitItem()
    {
        string keyName = $"{Name}.SPLITITEM";
        ItemToSplit = null;

        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }

        Item _tempItemToGetAfterSplit = CurrentStep.ItemoCraft.Materials.Find(i => i.Item.SplitsInto != null).Item;

        if (CurrentStep.ItemoCraft.SplitsInto == null && _tempItemToGetAfterSplit == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No item to merge");
            return false;
        }
        Item _itemToSplit = _tempItemToGetAfterSplit.SplitsInto;
        foreach (WoWItem item in Bag.GetBagItem())
        {
            if (item.Name == _itemToSplit.Name
                && ItemsManager.GetItemCountById(_itemToSplit.ItemId) > _itemToSplit.AmountRequiredToSplit)
            {
                ItemToSplit = _itemToSplit.Name;
                break;
            }
        }
        if (ItemToSplit == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No item to merge");
            return false;
        }
        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    // ************************ DISENCHANT ************************
    public abstract bool ShouldDisenchant();

    // ************************ ENCHANT ************************
    public abstract bool ShouldEnchant();

    // ************************ DELETE ITEMS ************************
    public bool ShouldFilterLoot()
    {
        string keyName = $"{Name}.FILTERLOOT";
        bool weKnowEnchanting = Main.primaryProfession.Name == SkillLine.Enchanting || Main.secondaryProfession.Name == SkillLine.Enchanting;
        ItemToDelete = null;
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }
        // Search for item to delete in bag
        foreach (WoWItem item in Bag.GetBagItem())
        {
            if (ToolBox.vendorSellQuality.Contains((WoWItemQuality)item.GetItemInfo.ItemRarity)
                && !wManagerSetting.CurrentSetting.DoNotSellList.Contains(item.Name)
                || (weKnowEnchanting && item.IsEquippableItem && item.GetItemInfo.ItemRarity > 1))
            {
                ItemToDelete = item;
                break;
            }
        }
        if (Bot.ProfileName == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No Bot profile loaded");
            return false;
        }
        if (ItemToDelete == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No item to delete");
            return false;
        }
        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    // ************************ TRAVEL ************************
    public bool ShouldTravel()
    {
        string keyName = $"{Name}.TRAVEL";
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }
        if (Phase > OtherProfession.Phase)
        {
            FrameHelper.UpdateDebugFrame(keyName, $"Waiting for {OtherProfession.Name} to catch up");
            return false;
        }
        if (!Bot.HasSetContinent)
        {
            FrameHelper.UpdateDebugFrame(keyName, "Bot hasn't set continent");
            return false;
        }
        if (Bot.Continent == (ContinentId)Usefuls.ContinentId)
        {
            FrameHelper.UpdateDebugFrame(keyName, "We are on good continent");
            return false;
        }
        if (!MyLevelIsHighEnough())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Level up first");
            return false;
        }
        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    // ************************ LEARN PROFESSION ************************
    public bool ShouldLearnProfession()
    {
        string keyName = $"{Name}.LEARNPROF";
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }
        if (Phase > OtherProfession.Phase)
        {
            FrameHelper.UpdateDebugFrame(keyName, $"Waiting for {OtherProfession.Name} to catch up");
            return false;
        }
        if (!ConfirmVendor())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Wait before vendors");
            return false;
        }
        if (!MyLevelIsHighEnough())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Level up first");
            return false;
        }

        bool shoulTrain = (ToolBox.GetProfessionLevel(Name) >= 0 && ToolBox.GetProfessionMaxLevel(Name) < 75
            || ToolBox.GetProfessionLevel(Name) >= 75 && ToolBox.GetProfessionMaxLevel(Name) < 150
            || ToolBox.GetProfessionLevel(Name) >= 150 && ToolBox.GetProfessionMaxLevel(Name) < 225
            || ToolBox.GetProfessionLevel(Name) >= 225 && ToolBox.GetProfessionMaxLevel(Name) < 300
            || ToolBox.GetProfessionLevel(Name) >= 300 && ToolBox.GetProfessionMaxLevel(Name) < 350
            || ToolBox.GetProfessionLevel(Name) >= 350 && ToolBox.GetProfessionMaxLevel(Name) < 450);

        if (!shoulTrain)
            return false;

        FrameHelper.UpdateDebugFrame($"{Name}.LEARNPROF", "RUNNING");
        return true;
    }

    // ************************ BUY AND LEARN RECIPE ************************
    public bool ShouldBuyAndLearnRecipe()
    {
        string keyName = $"{Name}.BUY&LEARN";
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }
        if (Phase > OtherProfession.Phase)
        {
            FrameHelper.UpdateDebugFrame(keyName, $"Waiting for {OtherProfession.Name} to catch up");
            return false;
        }
        if (!ConfirmVendor())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Wait before vendors");
            return false;
        }
        if (CurrentStep.KnownRecipe)
        {
            FrameHelper.UpdateDebugFrame(keyName, "We already know the recipe");
            return false;
        }
        var RecipeVendor = CurrentStep.ItemoCraft.RecipeVendor;
        if (RecipeVendor == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "Not a recipe we buy");
            return false;
        }
        if (!MyLevelIsHighEnough())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Level up first");
            return false;
        }

        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    // ************************ LEARN RECIPE FROM TRAINER ************************
    public bool ShouldLearnRecipeFromTrainer()
    {
        string keyName = $"{Name}.LEARNRECIPE";
        Logger.LogDebug($"{Name} : ShouldLearnRecipeFromTrainer()");
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }
        if (Phase > OtherProfession.Phase)
        {
            FrameHelper.UpdateDebugFrame(keyName, $"Waiting for {OtherProfession.Name} to catch up");
            return false;
        }
        if (!ConfirmVendor())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Wait before vendoring");
            return false;
        }
        if (!HasCheckedIfWeKnowRecipe)
            CurrentStep.KnownRecipe = ToolBox.RecipeIsKnown(CurrentStep.ItemoCraft.Name, this);

        if (CurrentStep.KnownRecipe)
        {
            FrameHelper.UpdateDebugFrame(keyName, "We already know the recipe");
            return false;
        }
        var RecipeVendor = CurrentStep.ItemoCraft.RecipeVendor;
        if (RecipeVendor != null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "This recipe should be bought");
            return false;
        }
        if (!MyLevelIsHighEnough())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Level up first");
            return false;
        }

        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    // ************************ BUY MATERIALS ************************
    public bool ShouldBuyMaterials()
    {
        string keyName = $"{Name}.BUYMATS";
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }
        if (Phase > OtherProfession.Phase)
        {
            FrameHelper.UpdateDebugFrame(keyName, $"Waiting for {OtherProfession.Name} to catch up");
            return false;
        }
        if (!ConfirmVendor())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Wait before vendors");
            return false;
        }
        if (!CurrentStep.CanBuyRemainingMats())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Remaining mats are not buyable");
            return false;
        }
        if (CurrentStep.HasMatsToCraftOne())
        {
            FrameHelper.UpdateDebugFrame(keyName, "We have mats to craft one");
            return false;
        }
        if (!MyLevelIsHighEnough())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Level up first");
            return false;
        }

        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    // ************************ CRAFT ONE ************************
    public bool ShouldCraftOne()
    {
        string keyName = $"{Name}.CRAFTONE";
        Logger.LogDebug($"{Name} : ShouldCraftOne()");
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }
        if (Phase > OtherProfession.Phase)
        {
            FrameHelper.UpdateDebugFrame(keyName, $"Waiting for {OtherProfession.Name} to catch up");
            return false;
        }
        if (Bot.ProfileName == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No Bot profile loaded");
            return false;
        }
        if (!CurrentStep.HasMatsToCraftOne())
        {
            FrameHelper.UpdateDebugFrame(keyName, "We don't have mats to craft one");
            return false;
        }
        if (CurrentStep.Type != Step.StepType.CraftAll)
        {
            FrameHelper.UpdateDebugFrame(keyName, "Current step is not CraftAll");
            return false;
        }
        if (AmountOfItemToFarm <= 0)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No item left to farm");
            return false;
        }

        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    // ************************ CRAFT ************************
    public bool ShouldCraft()
    {
        string keyName = $"{Name}.CRAFT";
        Logger.LogDebug($"{Name} : Checking if should craft");
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }
        if (Phase > OtherProfession.Phase)
        {
            FrameHelper.UpdateDebugFrame(keyName, $"Waiting for {OtherProfession.Name} to catch up");
            return false;
        }
        if (!MyLevelIsHighEnough())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Level up first");
            return false;
        }
        if (CurrentStep.ItemoCraft.IsEnchant)
        {
            FrameHelper.UpdateDebugFrame(keyName, "This is an enchant");
            return false;
        }
        if (CurrentStep.Type == Step.StepType.ListPreCraft)
        {
            FrameHelper.UpdateDebugFrame(keyName, "Current step is a ListPrecraft");
            return false;
        }
        if (AmountOfItemToFarm > 0)
        {
            FrameHelper.UpdateDebugFrame(keyName, "There are still items to farm");
            return false;
        }
        if (!CurrentStep.HasAllMats())
        {
            FrameHelper.UpdateDebugFrame(keyName, "We're missing mats");
            return false;
        }
        if (CurrentStep.Type == Step.StepType.CraftAll || CurrentStep.Type == Step.StepType.CraftToLevel)
        {
            FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
            return true;
        }
        FrameHelper.UpdateDebugFrame(keyName, "WARNING : NO STEP AFTER CHECK");
        return false;
    }

    // ************************ LOAD PROFILE ************************
    public bool ShouldLoadProfile()
    {
        string keyName = $"{Name}.LOADPROFILE";

        if (AmountOfItemToFarm <= 0 && Bot.ProfileName != null && Bot.ProfileProfession == Name.ToString())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Unloading profile");
            ProfileHandler.UnloadCurrentProfile();
            return false;
        }
        if (AmountOfItemToFarm <= 0)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No Items to farm");
            return false;
        }
        if (Bot.ProfileName != null)
        {
            FrameHelper.UpdateDebugFrame(keyName, $"{Bot.ProfileName}");
            return false;
        }

        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    // Farm before vendor
    public bool ConfirmVendor()
    {
        return (!CurrentStep.ItemoCraft.VendorFirst && AmountOfItemToFarm <= 0 && OtherProfession.AmountOfItemToFarm <= 0)
            || CurrentStep.ItemoCraft.VendorFirst || City == ToolBox.GetCurrentCity();
    }

    // Returns whether our level is at least the minimum level of current context
    public bool MyLevelIsHighEnough()
    {
        return ObjectManager.Me.Level >= MinimumCharLevel;
    }

    // Reevaluates if we need to modify or change the current step
    public void ReevaluateStep()
    {
        if (CurrentStep.ItemoCraft.IsAPrerequisiteItem || (ToolBox.GetProfessionLevel(Name) - CurrentStep.LevelToReach <= 0))
            RegenerateSteps();
        else
            ItemHelper.CalculateFarmAmountFor(this, CurrentStep.ItemoCraft);

        if (AmountOfItemToFarm <= 0)
            RegenerateSteps();
    }

    // Automatically added generated step
    public void AddGeneratedStep(Step step)
    {
        if (!AllSteps.Contains(step))
        {
            AllSteps.Add(step);
            MustRecalculateStep = true;
            ToolBox.SetSellListForOneItem(step.ItemoCraft);
            foreach (Item.Mat mat in step.ItemoCraft.Materials)
            {
                ToolBox.SetSellListForOneItem(mat.Item);
            }
        }
    }

    // Set the other chosen profession by the user
    public void SetOtherProfession()
    {
        if (Main.primaryProfession != null && Main.secondaryProfession != null)
        {
            if (Name == Main.primaryProfession.Name)
                OtherProfession = Main.secondaryProfession;
            else
                OtherProfession = Main.primaryProfession;
            Logger.LogDebug($"{Name} : Other profession is {OtherProfession.Name}");
        }
    }

    public abstract void SetContext();
    public abstract void RegenerateSteps();
}
