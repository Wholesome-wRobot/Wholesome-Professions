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
    public bool MustRecalculateStepFlag { get; set; }
    public bool HasCheckedIfWeKnowRecipeFlag { get; set; }
    public bool UserMustBuyManuallyFlag { get; set; }

    public int Phase { get; set; }
    public Item ItemToFarm { get; set; }
    public int AmountOfItemToFarm { get; set; }
    public WoWItem ItemToDelete { get; set; }
    public string ItemToSplit { get; set; }

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
        string key = $"{Name}.SETSTEP";

        if (!CurrenStepIsNull(key) 
            && UserMustBuyManuallyIsTrue(key) 
            && ItemsManager.GetItemCountById(CurrentStep.ItemoCraft.ItemId) <= 0)
            return false;
        if (!MustRecalculateIsTrue(key))
            return false;

        FrameHelper.UpdateDebugFrame(key, "RUNNING");
        return true;
    }

    // ************************ SPLIT ITEMS ************************
    public bool ShouldSplitItem()
    {
        string key = $"{Name}.SPLITITEM";

        if (CurrenStepIsNull(key) 
            || UserMustBuyManuallyIsTrue(key) 
            || !WeHaveAnItemToSplit(key))
            return false;

        FrameHelper.UpdateDebugFrame(key, "RUNNING");
        return true;
    }

    // ************************ DISENCHANT ************************
    public abstract bool ShouldDisenchant();

    // ************************ ENCHANT ************************
    public abstract bool ShouldEnchant();

    // ************************ DELETE ITEMS ************************
    public bool ShouldFilterLoot()
    {
        string key = $"{Name}.FILTERLOOT";
        ItemToDelete = null;

        if (CurrenStepIsNull(key) 
            || UserMustBuyManuallyIsTrue(key) 
            || !WeHaveAnItemToDelete(key) 
            || BotProfileNameIsNull(key))
            return false;

        FrameHelper.UpdateDebugFrame(key, "RUNNING");
        return true;
    }

    // ************************ TRAVEL ************************
    public bool ShouldTravel()
    {
        string key = $"{Name}.TRAVEL";

        if (CurrenStepIsNull(key) 
            || UserMustBuyManuallyIsTrue(key)
            || !MyCharLevelIsHighEnough(key) 
            || WeAreOnRightContinent(key))
            return false;

        FrameHelper.UpdateDebugFrame(key, "RUNNING");
        return true;
    }

    // ************************ LEARN PROFESSION ************************
    public bool ShouldLearnProfession()
    {
        string key = $"{Name}.LEARNPROF";

        if (CurrenStepIsNull(key) 
            || !WeNeedToTrain(key) 
            || UserMustBuyManuallyIsTrue(key) 
            || !MyCharLevelIsHighEnough(key) 
            || !VendorIsConfirmed(key))
            return false;

        FrameHelper.UpdateDebugFrame($"{Name}.LEARNPROF", "RUNNING");
        return true;
    }

    // ************************ BUY AND LEARN RECIPE ************************
    public bool ShouldBuyAndLearnRecipe()
    {
        string key = $"{Name}.BUY&LEARN";

        if (CurrenStepIsNull(key) 
            || UserMustBuyManuallyIsTrue(key)
            || !MyCharLevelIsHighEnough(key) 
            || !VendorIsConfirmed(key) 
            || WeKnowTheRecipe(key))
            return false;
        if (CurrentStep.ItemoCraft.RecipeVendor == null)
        {
            FrameHelper.UpdateDebugFrame(key, "Not a recipe we buy");
            return false;
        }

        FrameHelper.UpdateDebugFrame(key, "RUNNING");
        return true;
    }

    // ************************ LEARN RECIPE FROM TRAINER ************************
    public bool ShouldLearnRecipeFromTrainer()
    {
        string key = $"{Name}.LEARNRECIPE";
        Logger.LogDebug($"{Name} : ShouldLearnRecipeFromTrainer()");

        if (CurrenStepIsNull(key) 
            || UserMustBuyManuallyIsTrue(key)
            || !MyCharLevelIsHighEnough(key) 
            || !VendorIsConfirmed(key) 
            || WeKnowTheRecipe(key) 
            || RecipeMustBeBought(key))
            return false;

        FrameHelper.UpdateDebugFrame(key, "RUNNING");
        return true;
    }

    // ************************ BUY MATERIALS ************************
    public bool ShouldBuyMaterials()
    {
        string key = $"{Name}.BUYMATS";

        if (CurrenStepIsNull(key) 
            || UserMustBuyManuallyIsTrue(key)
            || !MyCharLevelIsHighEnough(key) 
            || !VendorIsConfirmed(key) 
            || !WeCanBuyRemainingMats(key)
            || WeHaveMatsToCraftOne(key))
            return false;

        FrameHelper.UpdateDebugFrame(key, "RUNNING");
        return true;
    }

    // ************************ CRAFT ONE ************************
    public bool ShouldCraftOne()
    {
        string key = $"{Name}.CRAFTONE";
        Logger.LogDebug($"{Name} : ShouldCraftOne()");

        if (CurrenStepIsNull(key)
            || UserMustBuyManuallyIsTrue(key) 
            || BotProfileNameIsNull(key)
            || !WeHaveItemsToFarm(key) 
            || !WeHaveMatsToCraftOne(key) 
            || !CurrentStepIsACraftAll(key))
            return false;

        FrameHelper.UpdateDebugFrame(key, "RUNNING");
        return true;
    }

    // ************************ CRAFT ************************
    public bool ShouldCraft()
    {
        string key = $"{Name}.CRAFT";
        Logger.LogDebug($"{Name} : Checking if should craft");

        if (CurrenStepIsNull(key) 
            || UserMustBuyManuallyIsTrue(key)
            || !MyCharLevelIsHighEnough(key) 
            || WeHaveItemsToFarm(key) 
            || !WeHaveAllMats(key)
            || CurrentStepIsAListPrecraft(key) 
            || ItemToCraftIsAnEnchant(key))
            return false;
        if (CurrentStepIsACraftAll(key) 
            || CurrentStepIsACraftToLevel(key))
            return true;

        FrameHelper.UpdateDebugFrame(key, "WARNING : NO STEP AFTER CHECK");
        return false;
    }

    // ************************ LOAD PROFILE ************************
    public bool ShouldLoadProfile()
    {
        string key = $"{Name}.LOADPROFILE";

        if (UserMustBuyManuallyIsTrue(key))
            return false;
        if (!WeHaveItemsToFarm(key) 
            && !BotProfileNameIsNull(key) 
            && Bot.ProfileProfession == Name.ToString())
        {
            FrameHelper.UpdateDebugFrame(key, "We're done farming. Unloading profile");
            ProfileHandler.UnloadCurrentProfile();
            return false;
        }
        if (!WeHaveItemsToFarm(key) || !BotProfileNameIsNull(key))
            return false;

        FrameHelper.UpdateDebugFrame(key, "RUNNING");
        return true;
    }

    // ********************************* CHECKS *********************************
    // Is CurrentStep null ?
    public bool CurrenStepIsNull(string key = null)
    {
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(key, "No step");
            return true;
        }
        return false;
    }

    // User must buy manually flag is on ?
    public bool UserMustBuyManuallyIsTrue(string key = null)
    {
        if (UserMustBuyManuallyFlag)
        {
            FrameHelper.UpdateDebugFrame(key, "User Must buy manually");
            return true;
        }
        return false;
    }

    // Must recalcultae flag is on ?
    public bool MustRecalculateIsTrue(string key = null)
    {
        if (MustRecalculateStepFlag)
            return true;

        FrameHelper.UpdateDebugFrame(key, "The recalculate step flag is False");
        return false;
    }

    // Is Bot.ProfileName null ?
    public bool BotProfileNameIsNull(string key = null)
    {
        if (Bot.ProfileName == null)
        {
            FrameHelper.UpdateDebugFrame(key, "Bot.ProfileName is null");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, $"Bot.ProfileName is {Bot.ProfileName}");
        return false;
    }
/*
    // Must wait for other profession to catch up ?
    public bool OtherProfessionShouldCatchUp(string key = null)
    {
        if (Phase > OtherProfession.Phase)
        {
            FrameHelper.UpdateDebugFrame(key, $"Waiting for {OtherProfession.Name} to catch up");
            return true;
        }
        return false;
    }
    */

    // Is our char level high enough ?
    public bool MyCharLevelIsHighEnough(string key = null)
    {
        if (ObjectManager.Me.Level >= MinimumCharLevel)
            return true;

        FrameHelper.UpdateDebugFrame(key, $"Your character must be {MinimumCharLevel} to proceed");
        return false;
    }

    // Is vendor confirmed ?
    public bool VendorIsConfirmed(string key = null)
    {
        if ((!CurrentStep.ItemoCraft.VendorFirst && !WeHaveItemsToFarm(key))
            || CurrentStep.ItemoCraft.VendorFirst 
            || City == ToolBox.GetCurrentCity())
            return true;

        FrameHelper.UpdateDebugFrame(key, $"We must wait before going to vendors");
        return false;
    }

    // We still have items to farm ?
    public bool WeHaveItemsToFarm(string key = null)
    {
        if (AmountOfItemToFarm > 0)
        {
            FrameHelper.UpdateDebugFrame(key, $"We still have an item to farm");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, $"No item left to farm");
        return false;
    }

    // We are on good continent ?
    public bool WeAreOnRightContinent(string key = null)
    {
        if (!Bot.HasSetContinent)
        {
            FrameHelper.UpdateDebugFrame(key, "Bot hasn't set continent");
            return true;
        }
        if (Bot.Continent == (ContinentId)Usefuls.ContinentId)
        {
            FrameHelper.UpdateDebugFrame(key, "We are on good continent");
            return true;
        }
        return false;
    }

    // We need to train ?
    public bool WeNeedToTrain(string key = null)
    {
        if (ToolBox.GetProfessionLevel(Name) >= 0 && ToolBox.GetProfessionMaxLevel(Name) < 75
            || ToolBox.GetProfessionLevel(Name) >= 75 && ToolBox.GetProfessionMaxLevel(Name) < 150
            || ToolBox.GetProfessionLevel(Name) >= 150 && ToolBox.GetProfessionMaxLevel(Name) < 225
            || ToolBox.GetProfessionLevel(Name) >= 225 && ToolBox.GetProfessionMaxLevel(Name) < 300
            || ToolBox.GetProfessionLevel(Name) >= 300 && ToolBox.GetProfessionMaxLevel(Name) < 350
            || ToolBox.GetProfessionLevel(Name) >= 350 && ToolBox.GetProfessionMaxLevel(Name) < 450)
        {
            FrameHelper.UpdateDebugFrame(key, "We need to train");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, "No need to train");
        return false;
    }

    // We know the recipe ?
    public bool WeKnowTheRecipe(string key = null)
    {
        if (!HasCheckedIfWeKnowRecipeFlag)
            CurrentStep.KnownRecipe = ToolBox.RecipeIsKnown(CurrentStep.ItemoCraft.Name, this);

        if (CurrentStep.KnownRecipe)
        {
            FrameHelper.UpdateDebugFrame(key, "We already know the recipe");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, "We don't know the recipe");
        return false;
    }

    // We have all materials ?
    public bool WeHaveAllMats(string key = null)
    {
        if (CurrentStep.HasAllMats())
        {
            FrameHelper.UpdateDebugFrame(key, "We have all materials");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, "We don't have all materials");
        return false;
    }

    // Current recipe must be bought ?
    public bool RecipeMustBeBought(string key = null)
    {
        if (CurrentStep.ItemoCraft.RecipeVendor != null)
        {
            FrameHelper.UpdateDebugFrame(key, "The recipe must be bought");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, "The recipe can't be bought");
        return false;
    }

    // Remaining mats can be bought ?
    public bool WeCanBuyRemainingMats(string key = null)
    {
        if (CurrentStep.CanBuyRemainingMats())
        {
            FrameHelper.UpdateDebugFrame(key, "We can buy the remaining mats");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, "The remaining mats can't be bought");
        return false;
    }

    // We have mats to craft one ?
    public bool WeHaveMatsToCraftOne(string key = null)
    {
        if (CurrentStep.HasMatsToCraftOne())
        {
            FrameHelper.UpdateDebugFrame(key, "We have mats to craft one");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, "We don't have mats to craft one");
        return false;
    }

    // Current step is a CraftAll ?
    public bool CurrentStepIsACraftAll(string key = null)
    {
        if (CurrentStep.Type == Step.StepType.CraftAll)
        {
            FrameHelper.UpdateDebugFrame(key, "Current step is a CraftAll");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, "Current step is not a CraftAll");
        return false;
    }

    // Current step is a CraftToLevel ?
    public bool CurrentStepIsACraftToLevel(string key = null)
    {
        if (CurrentStep.Type == Step.StepType.CraftToLevel)
        {
            FrameHelper.UpdateDebugFrame(key, "Current step is a CraftToLevel");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, "Current step is not a CraftToLevel");
        return false;
    }

    // Current step is a ListPrecraft ?
    public bool CurrentStepIsAListPrecraft(string key = null)
    {
        if (CurrentStep.Type == Step.StepType.ListPreCraft)
        {
            FrameHelper.UpdateDebugFrame(key, "Current step is a ListPreCraft");
            return true;
        }
        FrameHelper.UpdateDebugFrame(key, "Current step is not a ListPreCraft");
        return false;
    }

    // Is the item to craft an enchant ?
    public bool ItemToCraftIsAnEnchant(string key = null)
    {
        if (CurrentStep.ItemoCraft.IsAnEnchant)
        {
            FrameHelper.UpdateDebugFrame(key, "Item to craft is an enchant");
            return true;
        }
        return false;

    }

    // ********************************* HELPERS *********************************

    // Reevaluates if we need to modify or change the current step
    public void ReevaluateStep()
    {
        if (CurrentStep == null)
            return;

        if (CurrentStep.ItemoCraft.IsAPrerequisiteItem || 
            ((ToolBox.GetProfessionLevel(Name) - CurrentStep.LevelToReach >= 0) && CurrentStep.EstimatedAmountOfCrafts != 0
            && CurrentStep.LevelToReach != 0))
            RegenerateSteps();
        else
            ItemHelper.CalculateFarmAmountFor(this, CurrentStep.ItemoCraft);

        if (!WeHaveItemsToFarm())
            RegenerateSteps();
    }

    // Automatically added generated step
    public void AddGeneratedStep(Step step)
    {
        if (!AllSteps.Contains(step))
        {
            AllSteps.Add(step);
            MustRecalculateStepFlag = true;
            ToolBox.SetSellListForOneItem(step.ItemoCraft);
            foreach (Item.Mat mat in step.ItemoCraft.Materials)
            {
                ToolBox.SetSellListForOneItem(mat.Item);
            }
        }
    }

    // Search for items to split in inventory
    private bool WeHaveAnItemToSplit(string key)
    {
        ItemToSplit = null;
        Item _tempItemToGetAfterSplit = CurrentStep.ItemoCraft.Materials.Find(i => i.Item.SplitsInto != null).Item;

        if (CurrentStep.ItemoCraft.SplitsInto == null && _tempItemToGetAfterSplit == null)
        {
            FrameHelper.UpdateDebugFrame(key, "No item to split/merge");
            return false;
        }

        Item _itemToSplit = _tempItemToGetAfterSplit.SplitsInto;
        foreach (WoWItem item in Bag.GetBagItem())
        {
            if (item.Name == _itemToSplit.Name
                && ItemsManager.GetItemCountById(_itemToSplit.ItemId) > _itemToSplit.AmountRequiredToSplit)
            {
                FrameHelper.UpdateDebugFrame(key, $"Item to split/merge found : {_itemToSplit.Name}");
                ItemToSplit = _itemToSplit.Name;
                return true;
            }
        }
        FrameHelper.UpdateDebugFrame(key, "No item to split/merge");
        return false;
    }

    // Do we know enchanting ?
    private bool WeKnowEnchanting()
    {
        return Main.primaryProfession.Name == SkillLine.Enchanting;
    }

    // Search for item to delete in inventory
    private bool WeHaveAnItemToDelete(string key)
    {
        // Search for item to delete in bag
        foreach (WoWItem item in Bag.GetBagItem())
        {
            if (ToolBox.vendorSellQuality.Contains((WoWItemQuality)item.GetItemInfo.ItemRarity)
                && !wManagerSetting.CurrentSetting.DoNotSellList.Contains(item.Name)
                || (WeKnowEnchanting() && item.IsEquippableItem && item.GetItemInfo.ItemRarity > 1))
            {
                FrameHelper.UpdateDebugFrame(key, $"Item to delete found : {item.Name}");
                ItemToDelete = item;
                return true;
            }
        }
        FrameHelper.UpdateDebugFrame(key, "No item to delete");
        return false;
    }

    public abstract void SetContext();
    public abstract void RegenerateSteps();
}
