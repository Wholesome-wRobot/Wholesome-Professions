using System.Collections.Generic;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.ObjectManager;

public class Empty : IProfession
{
    public SkillLine Name { get; set; }
    public Step CurrentStep { get; set; }
    public List<Step> AllSteps { get; set; } = new List<Step>();
    public List<Item> PrerequisiteItems { get; set; } = new List<Item>();

    public Npc ProfessionTrainer { get; set; } = new Npc();
    public Npc SuppliesVendor { get; set; } = new Npc();
    public List<WoWItem> ItemsToDisenchant { get; set; } = null;
    public IProfession OtherProfession { get; set; }
    public int MinimumCharLevel { get; set; }
    public string ProfessionSpell { get; set; }
    public string City { get; set; }
    public string CurrentProfile { get; set; }
    public bool MustRecalculateStep { get; set; }
    public bool HasCheckedIfWeKnowRecipe { get; set; }
    public Item ItemToFarm { get; set; }
    public int AmountOfItemToFarm { get; set; }
    public WoWItem ItemToDelete { get; set; }
    public int Phase { get; set; }
    public string ItemToSplit { get; set; }
    public Empty() { return; }

    public void RegenerateSteps() { return; }

    public bool ShouldLoadProfile() { return false; }
    public bool ShouldSetCurrentStep() { return false; }
    public bool ShouldBuyMaterials() { return false; }
    public bool ShouldTravel() { return false; }
    public bool ShouldLearnRecipeFromTrainer() { return false; }
    public bool ShouldBuyAndLearnRecipe() { return false; }
    public bool ShouldLearnProfession() { return false; }
    public bool ShouldSellItems() { return false; }
    public bool ShouldCraftOne() { return false; }
    public bool ShouldCraft() { return false; }
    public bool ShouldDisenchant() { return false; }
    public bool ShouldFilterLoot() { return false; }
    public bool ShouldSplitItem() { return false; }
    public bool ShouldEnchant() { return false; }

    public bool MyLevelIsHighEnough() { return true; }
    public void AddGeneratedStep(Step step) { return; }
    public void SetContext() { return; }
    public void SetOtherProfession() { return; }
}
