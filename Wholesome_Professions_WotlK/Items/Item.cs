using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Class;

public class Item
{
    public string name;
    public uint itemId;
    public int compo4Amount;
    public bool canBeFarmed;
    public Npc vendor;
    public int estimatedPrice;
    public bool canBeBought;
    public int amountToCraft;
    public List<Mat> Materials;
    public bool forceSell; 
    public Npc RecipeVendor;
    public int RecipeItemId;
    public struct Mat
    {
        public Item item;
        public int amount;
    }

    public Item()
    {
        Materials = new List<Mat>();
        vendor = null;
        RecipeVendor = null;
    }

    public void AddMaterial(Item item, int amountToCraft)
    {
        Mat matToAdd = new Mat
        {
            item = item,
            amount = amountToCraft
        };
        Materials.Add(matToAdd);
        Logger.LogDebug($"ItemDB : Material {matToAdd.item.name} x {matToAdd.amount} added to item {name} in DB");
    }
}
