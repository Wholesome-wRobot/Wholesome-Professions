using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class ToolBox
{
    public static List<WoWItemQuality> vendorSellQuality = new List<WoWItemQuality>();

    // Check if Horde
    public static bool IsHorde()
    {
        Logger.LogDebug($"We are faction {ObjectManager.Me.Faction}");
        return ObjectManager.Me.Faction == (uint)PlayerFactions.Orc || ObjectManager.Me.Faction == (uint)PlayerFactions.Tauren
            || ObjectManager.Me.Faction == (uint)PlayerFactions.Undead || ObjectManager.Me.Faction == (uint)PlayerFactions.BloodElf
            || ObjectManager.Me.Faction == (uint)PlayerFactions.Troll;
    }

    // Add +x to crafted item to settings
    public static void AddCraftedItemToSettings(string profession, Item itemToAdd, int amountToAdd = 1)
    {
        string itemInList = SearchForCraftedItemInSavedList(profession, itemToAdd.Name);
        List<string> savedList = WholesomeProfessionsSave.CurrentSetting.AlreadyCrafted;

        // Item not found
        if (itemInList == null)
        {
            savedList.Add($"{profession}_{itemToAdd.Name}*{amountToAdd}");
        }
        // Item found
        else
        {
            int newAmount = GetAlreadyCrafted(profession, itemToAdd.Name) + amountToAdd;
            savedList.Remove(itemInList);
            savedList.Add($"{profession}_{itemToAdd.Name}*{newAmount}");
        }
        
        WholesomeProfessionsSave.CurrentSetting.Save();
    }

    // get amount of items already crafted
    public static int GetAlreadyCrafted(string profession, string item)
    {
        string propertyName = SearchForCraftedItemInSavedList(profession, item);
        if (propertyName == null)
            return 0;
        string amount = propertyName.Replace($"{profession}_{item}*", "");
        return Int16.Parse(amount);
    }

    // search for crafted item in saved list
    public static string SearchForCraftedItemInSavedList(string profession, string item)
    {
        List<string> savedList = WholesomeProfessionsSave.CurrentSetting.AlreadyCrafted;
        // Search for item
        foreach (string listItem in savedList)
        {
            if (listItem.StartsWith($"{profession}_{item}"))
            {
                //Logging.Write($"Item {profession}_{item} found");
                return listItem;
            }
        }
        return null;
    }

    // Clear profession from saved list
    public static void ClearProfessionFromSavedList(string profession)
    {
        if (WholesomeProfessionsSave.CurrentSetting.AlreadyCrafted.RemoveAll(i => i.StartsWith(profession)) > 0)
            Logger.LogDebug($"Cleared all saved items from already crafted list of profession {profession}");
        WholesomeProfessionsSave.CurrentSetting.Save();
    }

    // Gets Character's profession level
    public static int GetProfessionLevel(SkillLine skill)
    {
        return Skill.GetValue(skill);
    }

    // Gets Character's profession max level
    public static int GetProfessionMaxLevel(SkillLine skill)
    {
        return Skill.GetMaxValue(skill);
    }

    // Returns wether the profession is known
    public static bool KnowsProfession(SkillLine skill)
    {
        if (Skill.GetValue(skill) > 0)
            return true;
        return false;
    }

    public static void LearnthisSpell(string skillname)
    {
        Lua.LuaDoString(string.Format(@"
        for i=1,GetNumTrainerServices() do
        local name = GetTrainerServiceInfo(i)
        if (name == '{0}') then
            BuyTrainerService(i)
         end
        end
        ", skillname.Replace("'", "\'")));
    }

    // Manage WRobot sell/not sell lists
    public static void ManageSellList(List<Step> allSteps)
    {
        if (wManagerSetting.CurrentSetting.SellGray)
            vendorSellQuality.Add(WoWItemQuality.Poor);
        if (wManagerSetting.CurrentSetting.SellWhite)
            vendorSellQuality.Add(WoWItemQuality.Common);
        if (wManagerSetting.CurrentSetting.SellGreen)
            vendorSellQuality.Add(WoWItemQuality.Uncommon);
        if (wManagerSetting.CurrentSetting.SellBlue)
            vendorSellQuality.Add(WoWItemQuality.Rare);
        if (wManagerSetting.CurrentSetting.SellPurple)
            vendorSellQuality.Add(WoWItemQuality.Epic);

        wManagerSetting.CurrentSetting.Selling = false;

        if (!wManagerSetting.CurrentSetting.DoNotSellList.Contains("Hearthstone"))
            wManagerSetting.CurrentSetting.DoNotSellList.Add("Hearthstone");

        wManager.Wow.Bot.States.ToTown.ForceToTown = false;
        foreach (Step step in allSteps)
        {
            foreach (Item.Mat mat in step.ItemoCraft.Materials)
            {
                SetSellListForOneItem(mat.Item);
            }
        }
    }

    public static void SetSellListForOneItem(Item item)
    {
        RemoveFromSellAndNotSellList(item);
        if (item.IsEnchant)
            return;

        if (item.ForceSell)
            AddItemToSellList(item);
        else
        {
            AddItemToDoNotSellList(item);
            if (item.SplitsInto != null)
                AddItemToDoNotSellList(item.SplitsInto);
        }
    }

    private static void AddItemToDoNotSellList(Item item)
    {
        if (!wManagerSetting.CurrentSetting.DoNotSellList.Contains(item.Name))
        {
            Logger.LogDebug($"Add items {item.Name} to Do not Sell List");
            wManagerSetting.CurrentSetting.DoNotSellList.Add(item.Name);
        }
    }

    private static void AddItemToSellList(Item item)
    {
        if (!wManagerSetting.CurrentSetting.ForceSellList.Contains(item.Name))
        {
            Logger.LogDebug($"Add items {item.Name} to Force Sell List");
            wManagerSetting.CurrentSetting.ForceSellList.Add(item.Name);
        }
    }

    private static void RemoveFromSellAndNotSellList(Item item)
    {
        if (wManagerSetting.CurrentSetting.ForceSellList.Contains(item.Name))
            wManagerSetting.CurrentSetting.ForceSellList.Remove(item.Name);

        if (wManagerSetting.CurrentSetting.DoNotSellList.Contains(item.Name))
            wManagerSetting.CurrentSetting.DoNotSellList.Remove(item.Name);
    }

    public static void Craft(string skillName, Item item, int quantity)
    {
        OpenProfessionFrame(skillName);
        if (item.IsEnchant)
        {
            UseProfessionSkill(item, 1);
            WoWItem itemToEnchant = EquippedItems.GetEquippedItems().Find(i => i.GetItemInfo.ItemEquipLoc == item.EnchantGearType);
            for (var i = 0; i < quantity; i++)
            {
                int itemSlot = GetGearSlot(itemToEnchant);
                Logger.Log($"Enchanting {itemToEnchant.Name}");
                Lua.RunMacroText($"/use {itemSlot}");
                Lua.LuaDoString("ReplaceEnchant()");
                Usefuls.WaitIsCasting();
            }
        }
        else
        {
            UseProfessionSkill(item, quantity);
        }
        Usefuls.WaitIsCasting();
    }

    public static bool ProfessionFrameOpen()
    {
        return Lua.LuaDoString<bool>($"return TradeSkillFrame:IsVisible()");
    }

    public static void OpenProfessionFrame(string skillName)
    {
        if (!ProfessionFrameOpen())
        {
            CloseProfessionFrame();
            Lua.LuaDoString($"CastSpellByName('{skillName}')");
        }
        else
            Lua.LuaDoString($"CastSpellByName('{skillName}')");
    }

    public static void CloseProfessionFrame()
    {
        Lua.LuaDoString("TradeSkillFrame:Hide()");
    }

    public static void UseProfessionSkill(Item item, int quantity)
    {
        Logger.Log($"Crafting {quantity} x {item.Name}");
        Lua.LuaDoString($@"
            for i=1,GetNumTradeSkills() do
                local name, _, _, _ = GetTradeSkillInfo(i)
                if (name == '{item.Name}') then
                    DoTradeSkill(i, {quantity})
                end
            end");
    }

    public static bool RecipeIsKnown(string recipeName, IProfession profession)
    {
        SpellManager.CastSpellByNameLUA(profession.Name.ToString());
        bool recipeIsKnown =  Lua.LuaDoString <bool> (@"
                                    tradeskillName, currentLevel, maxLevel, skillLineModifier = GetTradeSkillLine();
                                    tradeItemCount = GetNumTradeSkills()
                                    for i = 1, tradeItemCount do
                                            tradeItemName, tradeItemType, _, _ = GetTradeSkillInfo(i)
                                            if tradeItemName == """ + recipeName + @""" then
                                                return true;
                                            end
                                    end");
        Thread.Sleep(300);
        SpellManager.CastSpellByNameLUA(profession.Name.ToString());
        profession.HasCheckedIfWeKnowRecipe = true;
        return recipeIsKnown;
    }

    public static void DeleteItemByName(WoWItem item)
    {
        List<int> itemContainerBagIdAndSlot = Bag.GetItemContainerBagIdAndSlot(item.Entry);
        Lua.LuaDoString(string.Format("PickupContainerItem({0}, {1})", itemContainerBagIdAndSlot[0], itemContainerBagIdAndSlot[1]), false);
        Lua.LuaDoString("DeleteCursorItem()", false);
        Thread.Sleep(100);
    }

    public static int GetGearSlot(WoWItem item)
    {
        switch (item.GetItemInfo.ItemEquipLoc)
        {
            case "INVTYPE_AMMO": return 0;
            case "INVTYPE_HEAD": return 1;
            case "INVTYPE_NECK": return 2;
            case "INVTYPE_SHOULDER": return 3;
            case "INVTYPE_BODY": return 4;
            case "INVTYPE_CHEST": return 5;
            case "INVTYPE_ROBE": return 5;
            case "INVTYPE_WAIST": return 6;
            case "INVTYPE_LEGS": return 7;
            case "INVTYPE_FEET": return 8;
            case "INVTYPE_WRIST": return 9;
            case "INVTYPE_HAND": return 10;
            case "INVTYPE_FINGER": return 11;
            case "INVTYPE_TRINKET": return 13;
            case "INVTYPE_CLOAK": return 15;
            case "INVTYPE_WEAPON": return 16;
            case "INVTYPE_SHIELD": return 17;
            case "INVTYPE_2HWEAPON": return 16;
            case "INVTYPE_WEAPONMAINHAND": return 16;
            case "INVTYPE_WEAPONOFFHAND": return 17;
            case "INVTYPE_HOLDABLE": return 17;
            case "INVTYPE_RANGED": return 18;
            case "INVTYPE_THROWN": return 18;
            case "INVTYPE_RANGEDRIGHT": return 18;
            case "INVTYPE_RELIC": return 18;
            case "INVTYPE_TABARD": return 19;
            default: return -1;
        }
    }

    public static string GetCurrentCity()
    {
        return Lua.LuaDoString<string>("return GetZoneText();");
    }
}

