using System.Collections.Generic;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Class;
using wManager.Wow.ObjectManager;

public class Item
{
    public string Name { get; set; }
    public uint ItemId { get; set; }
    public bool CanBeFarmed { get; set; }
    public Npc Vendor { get; set; }
    public int EstimatedPrice { get; set; }
    public bool CanBeBought { get; set; }
    public int AmountToCraft { get; set; }
    public List<Mat> Materials { get; set; }
    public bool ForceSell { get; set; }
    public Npc RecipeVendor { get; set; }
    public int RecipeItemId { get; set; }
    public string Profile { get; set; }
    public Spell Spell { get; set; }
    public bool IsAnEnchant { get; set; }
    public bool IsAPrerequisiteItem { get; set; }
    public bool ContainsBuyableMats { get; set; }
    public bool VendorFirst { get; set; }
    public string EnchantGearType { get; set; }
    public Item SplitsInto { get; set; }
    public int AmountRequiredToSplit { get; set; }
    public bool UserMustBuyManually { get; set; }

    public struct Mat
    {
        public Item Item { get; set; }
        public int Amount { get; set; }
    }

    public Item()
    {
        Materials = new List<Mat>();
        Vendor = null;
        RecipeVendor = null;
        Profile = null;
        EnchantGearType = null;
        SplitsInto = null;
        ItemDB.AlllItems.Add(this);
    }

    public void AddMaterial(Item item, int amountToCraft)
    {
        Mat matToAdd = new Mat
        {
            Item = item,
            Amount = amountToCraft
        };
        Materials.Add(matToAdd);
        if (item.CanBeBought)
            ContainsBuyableMats = true;
        //Logger.LogDebug($"ItemDB : Material {matToAdd.Item.Name} x {matToAdd.Amount} added to item {Name} in DB");
    }
}
