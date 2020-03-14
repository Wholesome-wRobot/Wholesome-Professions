using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;

public class ToolBox
{
    // Manage WRobot sell/not sell lists
    public static void ManageSellList(List<Step> allSteps)
    {
        foreach (Step step in allSteps)
        {
            wManager.wManagerSetting.CurrentSetting.Selling = false;
            wManager.Wow.Bot.States.ToTown.ForceToTown = false;
            ToolBox.RemoveFromSellAndNotSellList(step.itemoCraft);
            if (step.itemoCraft.forceSell)
                ToolBox.AddItemToSellList(step.itemoCraft);
        }
    }

    // Add +x to crafted item to settings
    public static void AddCraftedItemToSettings(string profession, Item itemToAdd, int amountToAdd = 1)
    {
        string itemInList = SearchForCraftedItemInSavedList(profession, itemToAdd.name);
        List<string> savedList = WholesomeProfessionsSave.CurrentSetting.AlreadyCrafted;

        // Item not found
        if (itemInList == null)
        {
            //Logging.Write($"Adding item {profession}_{itemToAdd}*{amountToAdd} to list");
            savedList.Add($"{profession}_{itemToAdd.name}*{amountToAdd}");
        }
        // Item found
        else
        {
            //Logging.Write($"We already have {GetAlreadyCrafted(profession, itemToAdd)}");
            int newAmount = GetAlreadyCrafted(profession, itemToAdd.name) + amountToAdd;
            savedList.Remove(itemInList);
            savedList.Add($"{profession}_{itemToAdd.name}*{newAmount}");
            
            //Logging.Write($"New value {profession}_{itemToAdd}*{newAmount} to list");
        }
        
        //Logging.Write($"Saving");
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

    public static void AddItemToDoNotSellList(Item item)
    {
        if (!wManager.wManagerSetting.CurrentSetting.DoNotSellList.Contains(item.name))
            wManager.wManagerSetting.CurrentSetting.DoNotSellList.Add(item.name);
    }

    public static void AddItemToSellList(Item item)
    {
        if (!wManager.wManagerSetting.CurrentSetting.ForceSellList.Contains(item.name))
            wManager.wManagerSetting.CurrentSetting.ForceSellList.Add(item.name);
    }

    public static void RemoveFromSellAndNotSellList(Item item)
    {
        if (!wManager.wManagerSetting.CurrentSetting.ForceSellList.Contains(item.name))
            wManager.wManagerSetting.CurrentSetting.ForceSellList.Remove(item.name);

        if (!wManager.wManagerSetting.CurrentSetting.DoNotSellList.Contains(item.name))
            wManager.wManagerSetting.CurrentSetting.DoNotSellList.Remove(item.name);
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
            if (name == '{item.name}') then
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
        Thread.Sleep(500);
        SpellManager.CastSpellByNameLUA(profession);
        return recipeIsKnown;
    }
}

