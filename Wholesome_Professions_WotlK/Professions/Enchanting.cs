using wManager.Wow.Enums;
using Wholesome_Professions_WotlK.Helpers;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using wManager.Wow.Class;

public class Enchanting : Profession
{
    readonly Spell Disenchant = new Spell("Disenchant");

    public Enchanting() : base (SkillLine.Enchanting)
    {
        RegenerateSteps();
        ToolBox.ManageSellList(AllSteps);
    }

    public override void RegenerateSteps()
    {
        Logger.LogDebug("REGENERATING ENCHANTING STEPS");
        SetContext();

        AllSteps.Clear();
        AllSteps.Add(new Step(this, 0, 50, ItemDB.EnchantBracerMinorHealth, 20));
        AllSteps.Add(new Step(this, 50, 75, ItemDB.EnchantBracerMinorHealth, 20));
        AllSteps.Add(new Step(this, 75, 90, ItemDB.EnchantBracerMinorHealth, 20));
        AllSteps.Add(new Step(this, 90, 120, ItemDB.EnchantBracerMinorStamina, 30));

        MustRecalculateStepFlag = true;
        HasCheckedIfWeKnowRecipeFlag = false;

        if (Bot.ProfileName != null)
            ProfileHandler.UnloadCurrentProfile();
    }

    public override bool ShouldDisenchant()
    {
        string keyName = $"{Name}.DISENCHANT";
        /*
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }*/
        if (!Disenchant.KnownSpell)
        {
            FrameHelper.UpdateDebugFrame(keyName, "We don't know Disenchant");
            return false;
        }
        ItemsToDisenchant.Clear();
        foreach (WoWItem item in Bag.GetBagItem())
        {
            if (item.GetItemInfo.ItemRarity > 1 && item.IsEquippableItem && LevelIsEnoughToDisenchant(item))
                ItemsToDisenchant.Add(item);
        }
        if (ItemsToDisenchant.Count > 0)
        {
            FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
            return true;
        }
        else
        {
            FrameHelper.UpdateDebugFrame(keyName, "No item to disenchant");
            return false;
        }
    }

    public override bool ShouldEnchant()
    {
        string keyName = $"{Name}.ENCHANT";
        if (CurrentStep == null)
        {
            FrameHelper.UpdateDebugFrame(keyName, "No step");
            return false;
        }
        if (!MyCharLevelIsHighEnough())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Level up first");
            return false;
        }
        if (!CurrentStep.ItemoCraft.IsAnEnchant)
        {
            FrameHelper.UpdateDebugFrame(keyName, "Not an enchant");
            return false;
        }
        if (!CurrentStep.HasMatsToCraftOne())
        {
            FrameHelper.UpdateDebugFrame(keyName, "Not enough materials");
            return false;
        }
        if (!CurrentStep.KnownRecipe)
        {
            FrameHelper.UpdateDebugFrame(keyName, "We don't know the recipe");
            return false;
        }

        FrameHelper.UpdateDebugFrame(keyName, "RUNNING");
        return true;
    }

    public override void SetContext()
    {
        // ek = 0, kalimdor = 1, Outlands = 530, Northrend 571
        int profLevel = ToolBox.GetProfessionLevel(Name);

        if (ToolBox.IsHorde()) // Horde
        {
            if (profLevel < 75)
            {
                MinimumCharLevel = 20;
                ProfessionSpell = "Apprentice Enchanter";
                ProfessionTrainer = VendorDB.OGEnchantingTrainer;
                SuppliesVendor = VendorDB.OGEnchantingSupplies;
                PrerequisiteItems = new List<Item>()
                {
                    ItemDB.RunedCopperRod
                };
                Phase = 1;
                City = "Orgrimmar";
            }
            else if (profLevel < 150)
            {
                MinimumCharLevel = 25;
                ProfessionSpell = "Journeyman Enchanter";
                ProfessionTrainer = VendorDB.OGEnchantingTrainer;
                SuppliesVendor = VendorDB.OGEnchantingSupplies;
                PrerequisiteItems = new List<Item>()
                {
                    ItemDB.RunedCopperRod,
                    ItemDB.RunedSilverRod
                };
                Phase = 2;
                City = "Orgrimmar";
            }
        }
    }

    private bool LevelIsEnoughToDisenchant(WoWItem item)
    {
        int profLevel = ToolBox.GetProfessionLevel(Name);
        int itemLevel = item.GetItemInfo.ItemLevel;

        // Uncommon
        if (item.GetItemInfo.ItemRarity == 2)
        {
            if (itemLevel <= 20)
                return true;
            if (itemLevel <= 25 && profLevel >= 25)
                return true;
            if (itemLevel <= 30 && profLevel >= 50)
                return true;
            if (itemLevel <= 35 && profLevel >= 75)
                return true;
            if (itemLevel <= 40 && profLevel >= 100)
                return true;
            if (itemLevel <= 45 && profLevel >= 125)
                return true;
            if (itemLevel <= 50 && profLevel >= 150)
                return true;
            if (itemLevel <= 55 && profLevel >= 175)
                return true;
            if (itemLevel <= 60 && profLevel >= 200)
                return true;
            if (itemLevel <= 99 && profLevel >= 225)
                return true;
            if (itemLevel <= 120 && profLevel >= 275)
                return true;
            if (itemLevel <= 150 && profLevel >= 325)
                return true;
            if (profLevel >= 350)
                return true;
        }

        // Rare
        if (item.GetItemInfo.ItemRarity == 2)
        {
            if (itemLevel <= 20)
                return true;
            if (itemLevel <= 25 && profLevel >= 25)
                return true;
            if (itemLevel <= 30 && profLevel >= 50)
                return true;
            if (itemLevel <= 35 && profLevel >= 75)
                return true;
            if (itemLevel <= 40 && profLevel >= 100)
                return true;
            if (itemLevel <= 45 && profLevel >= 125)
                return true;
            if (itemLevel <= 50 && profLevel >= 150)
                return true;
            if (itemLevel <= 55 && profLevel >= 175)
                return true;
            if (itemLevel <= 60 && profLevel >= 200)
                return true;
            if (itemLevel <= 99 && profLevel >= 225)
                return true;
            if (itemLevel <= 120 && profLevel >= 275)
                return true;
            if (itemLevel <= 200 && profLevel >= 325)
                return true;
            if (profLevel >= 350)
                return true;
        }

        // Epic
        if (item.GetItemInfo.ItemRarity == 2)
        {
            if (itemLevel <= 89 && profLevel >= 225)
                return true;
            if (itemLevel <= 92 && profLevel >= 300)
                return true;
            if (itemLevel <= 151 && profLevel >= 300)
                return true;
            if (itemLevel <= 277 && profLevel >= 375)
                return true;
            if (profLevel >= 375)
                return true;
        }

        Logger.LogDebug($"Item {item.Name} can't be disenchanted.");
        return false;
    }
}
