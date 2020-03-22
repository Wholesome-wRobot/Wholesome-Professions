using System;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class ToolBox
{
    public static List<WoWItemQuality> vendorQuality = new List<WoWItemQuality>();

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
        if (wManagerSetting.CurrentSetting.MailGray)
            vendorQuality.Add(WoWItemQuality.Poor);
        if (wManagerSetting.CurrentSetting.MailWhite)
            vendorQuality.Add(WoWItemQuality.Common);
        if (wManagerSetting.CurrentSetting.MailGreen)
            vendorQuality.Add(WoWItemQuality.Uncommon);
        if (wManagerSetting.CurrentSetting.MailBlue)
            vendorQuality.Add(WoWItemQuality.Rare);
        if (wManagerSetting.CurrentSetting.MailPurple)
            vendorQuality.Add(WoWItemQuality.Epic);

        wManagerSetting.CurrentSetting.Selling = false;
        wManager.Wow.Bot.States.ToTown.ForceToTown = false;
        foreach (Step step in allSteps)
        {
            SetSellListForOneItem(step.ItemoCraft);
            foreach (Item.Mat mat in step.ItemoCraft.Materials)
            {
                SetSellListForOneItem(mat.Item);
            }
        }
    }

    private static void SetSellListForOneItem(Item item)
    {
        RemoveFromSellAndNotSellList(item);
        if (item.ForceSell)
            AddItemToSellList(item);
        else
            AddItemToDoNotSellList(item);
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
        SpellManager.CastSpellByNameLUA(skillName);
        Lua.LuaDoString($@"
            if (TradeSkillFrame:IsVisible() ) then
             TradeSkillFrame:Hide();
             end
            if not TradeSkillFrame then
             CastSpellByName('{skillName}')
             end
             if TradeSkillFrame:IsVisible() == nil then
             CastSpellByName('{skillName}')
             end 
            for i=1,GetNumTradeSkills() do
             local name, _, _, _ = GetTradeSkillInfo(i)
            if (name == '{item.Name}') then
            DoTradeSkill(i, {quantity})
            end
             end
             if (TradeSkillFrame:IsVisible() and {quantity} < 2) then
            TradeSkillFrame:Hide();
            end
            ");
        Usefuls.WaitIsCasting();
    }

    public static bool IsTradeSkillOpen()
    {
        return Lua.LuaDoString<bool>($"if TradeSkillFrame:IsVisible() then return true end");
    }

    public static bool RecipeIsKnown(string recipeName, string profession)
    {
        SpellManager.CastSpellByNameLUA(profession);
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
        SpellManager.CastSpellByNameLUA(profession);
        return recipeIsKnown;
    }

    public static bool IsProfessionFrameOpen()
    {
        return Lua.LuaDoString<bool>($"return TradeSkillFrame:IsVisible()");
    }

    public static void OpenProfessionFrame(string skillName)
    {
        if (!IsProfessionFrameOpen())
        {
            CloseProfessionFrame();
            Lua.LuaDoString($"CastSpellByName('{skillName}')");
        }
        else
            Lua.LuaDoString($"CastSpellByName('{skillName}')");
    }

    public static void CloseProfessionFrame()
    {
        if (!IsProfessionFrameOpen())
            Lua.LuaDoString("TradeSkillFrame:Hide()");
    }
}

