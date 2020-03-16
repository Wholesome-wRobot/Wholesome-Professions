using System;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Helpers;

public class Step
{
    public int minlevel;
    public int levelToReach;
    public Item itemoCraft;
    public int estimatedAmountOfCrafts;
    public bool knownRecipe;
    public enum StepType
    {
        CraftToLevel,
        CraftAll, // auto generated when checking steps
        ListPreCraft // forced precraft from the list
    }
    public StepType stepType;

    // Craft to level
    public Step(int minlevel, int levelToReach, Item itemoCraft, int estimatedAmountOfCrafts = 0)
    {
        this.minlevel = minlevel;
        this.levelToReach = levelToReach;
        this.itemoCraft = itemoCraft;
        this.estimatedAmountOfCrafts = (estimatedAmountOfCrafts + WholesomeProfessionsSettings.CurrentSetting.ServerRate - 1) / WholesomeProfessionsSettings.CurrentSetting.ServerRate;

        if (estimatedAmountOfCrafts == 0)
            stepType = StepType.ListPreCraft;
        else
            stepType = StepType.CraftToLevel;
    }

    // Craft all
    public Step(Item itemoCraft, int estimatedAmountOfCrafts)
    {
        this.itemoCraft = itemoCraft;
        this.estimatedAmountOfCrafts = estimatedAmountOfCrafts;
        stepType = StepType.CraftAll;
    }

    public void LogMissingMaterials()
    {
        foreach (Item.Mat mat in itemoCraft.Materials)
        {
            if (GetAmountMissingMaterial(mat) > 0)
                Logger.Log($"{mat.item.name} x {GetAmountMissingMaterial(mat)}");
        }
    }

    public int GetRemainingProfessionLevels()
    {
        int remainingLevels =  levelToReach - ToolBox.GetProfessionLevel(Main.currentProfession.ProfessionName);
        return (remainingLevels + WholesomeProfessionsSettings.CurrentSetting.ServerRate - 1) / WholesomeProfessionsSettings.CurrentSetting.ServerRate;
    }

    public bool HasAllMats()
    {
        foreach (Item.Mat mat in itemoCraft.Materials)
        {
            if (GetAmountMissingMaterial(mat) > 0)
                return false;
        }
        return true;
    }

    public bool CanBuyRemainingMats()
    {
        foreach (Item.Mat mat in itemoCraft.Materials)
        {
            if (GetAmountMissingMaterial(mat) > 0 && !mat.item.canBeBought)
                return false;
        }
        return true;
    }

    public bool HasMatsToCraftOne()
    {
        bool hasMatsForOne = true;
        foreach (Item.Mat mat in itemoCraft.Materials)
        {
            if (ItemsManager.GetItemCountById(mat.item.itemId) < mat.amount)
                hasMatsForOne =  false;
        }
        return hasMatsForOne;
    }

    public int GetAmountMissingMaterial(Item.Mat mat)
    {
        // If craft all items
        if (stepType == StepType.CraftAll)
        {
            return (estimatedAmountOfCrafts * mat.amount) - ItemsManager.GetItemCountById(mat.item.itemId);
            //return Math.Max(0, (estimatedAmountOfCrafts * mat.amount) - ToolBox.GetAlreadyCrafted(Main.currentProfession.ProfessionName.ToString(), mat.item.name));
        }
        // or if we only need to craft until we level up
        else
        {
            Logger.LogDebug($"{GetRemainingProfessionLevels()} levels to gain");
            return (GetRemainingProfessionLevels() * mat.amount) - ItemsManager.GetItemCountById(mat.item.itemId);
        }
    }
}