using System.Collections.Generic;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Class;

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
    }

    public void AddMaterial(Item item, int amountToCraft)
    {
        Mat matToAdd = new Mat
        {
            Item = item,
            Amount = amountToCraft
        };
        Materials.Add(matToAdd);
        Logger.LogDebug($"ItemDB : Material {matToAdd.Item.Name} x {matToAdd.Amount} added to item {Name} in DB");
    }
}
