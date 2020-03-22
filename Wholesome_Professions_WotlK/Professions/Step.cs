using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Helpers;

public class Step
{
    public int Minlevel { get; set; }
    public int LevelToReach { get; set; }
    public Item ItemoCraft { get; set; }
    public int EstimatedAmountOfCrafts { get; set; }
    public bool KnownRecipe { get; set; }
    public StepType Type { get; set; }
    public enum StepType
    {
        CraftToLevel,
        CraftAll, // auto generated when checking steps
        ListPreCraft // forced precraft from the list
    }

    // Craft to level
    public Step(int minlevel, int levelToReach, Item itemoCraft, int estimatedAmountOfCrafts = 0)
    {
        Minlevel = minlevel;
        LevelToReach = levelToReach;
        ItemoCraft = itemoCraft;
        EstimatedAmountOfCrafts = (estimatedAmountOfCrafts + WholesomeProfessionsSettings.CurrentSetting.ServerRate - 1) / WholesomeProfessionsSettings.CurrentSetting.ServerRate;

        if (estimatedAmountOfCrafts == 0)
            Type = StepType.ListPreCraft;
        else
            Type = StepType.CraftToLevel;
    }

    // Craft all
    public Step(Item itemoCraft, int estimatedAmountOfCrafts)
    {
        ItemoCraft = itemoCraft;
        EstimatedAmountOfCrafts = estimatedAmountOfCrafts;
        Type = StepType.CraftAll;
    }

    public void LogMissingMaterials()
    {
        foreach (Item.Mat mat in ItemoCraft.Materials)
        {
            if (GetAmountMissingMaterial(mat) > 0)
                Logger.Log($"{mat.Item.Name} x {GetAmountMissingMaterial(mat)}");
        }
    }

    public int GetRemainingProfessionLevels()
    {
        int remainingLevels =  LevelToReach - ToolBox.GetProfessionLevel(Main.primaryProfession.ProfessionName);
        return (remainingLevels + WholesomeProfessionsSettings.CurrentSetting.ServerRate - 1) / WholesomeProfessionsSettings.CurrentSetting.ServerRate;
    }

    public bool HasAllMats()
    {
        foreach (Item.Mat mat in ItemoCraft.Materials)
        {
            if (GetAmountMissingMaterial(mat) > 0)
                return false;
        }
        return true;
    }

    public bool CanBuyRemainingMats()
    {
        foreach (Item.Mat mat in ItemoCraft.Materials)
        {
            if (GetAmountMissingMaterial(mat) > 0 && !mat.Item.CanBeBought)
                return false;
        }
        return true;
    }

    public bool HasMatsToCraftOne()
    {
        bool hasMatsForOne = true;
        foreach (Item.Mat mat in ItemoCraft.Materials)
        {
            if (ItemsManager.GetItemCountById(mat.Item.ItemId) < mat.Amount)
                hasMatsForOne =  false;
        }
        return hasMatsForOne;
    }

    // Returns the amount of the current step item I can craft
    public int GetAmountICanCurrentlyCraft()
    {
        int amount = 0;
        foreach (Item.Mat mat in ItemoCraft.Materials)
        {
            if (ItemsManager.GetItemCountById(mat.Item.ItemId) > mat.Amount)
            {
                int estimatedWithCurrentMat = ItemsManager.GetItemCountById(mat.Item.ItemId) / mat.Amount;
                if (amount == 0 || estimatedWithCurrentMat < amount)
                    amount = estimatedWithCurrentMat;
            }
            else
                return 0;
        }
        return amount;
    }

    public int GetAmountMissingMaterial(Item.Mat mat)
    {
        // If craft all items
        if (Type == StepType.CraftAll)
        {
            return (EstimatedAmountOfCrafts * mat.Amount) - ItemsManager.GetItemCountById(mat.Item.ItemId);
            //return Math.Max(0, (estimatedAmountOfCrafts * mat.amount) - ToolBox.GetAlreadyCrafted(Main.currentProfession.ProfessionName.ToString(), mat.item.name));
        }
        // or if we only need to craft until we level up
        else
        {
            Logger.LogDebug($"{GetRemainingProfessionLevels()} levels to gain");
            return (GetRemainingProfessionLevels() * mat.Amount) - ItemsManager.GetItemCountById(mat.Item.ItemId);
        }
    }
}