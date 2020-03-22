using System.Collections.Generic;
using wManager.Wow.Class;
using wManager.Wow.Enums;

public class Empty : IProfession
{
    public SkillLine ProfessionName { get; set; }
    public Step CurrentStep { get; set; }
    public List<Step> AllSteps { get; set; } = new List<Step>();
    public List<Item> PrerequisiteItems { get; set; } = new List<Item>();

    public Npc ProfessionTrainer { get; set; } = new Npc();
    public Npc SuppliesVendor { get; set; } = new Npc();
    public int Continent { get; set; }
    public int MinimumCharLevel { get; set; }
    public string ProfessionSpell { get; set; }
    public string CurrentProfile { get; set; }
    public bool HasSetCurrentStep { get; set; }
    public Item ItemToFarm { get; set; }
    public int AmountOfItemToFarm { get; set; }
    public Empty() { return; }

    public void RegenerateSteps() { return; }
    public bool ShouldSelectProfile() { return false; }
    public bool ShouldSetCurrentStep() { return false; }
    public bool ShouldBuyMaterials() { return false; }
    public bool ShouldTravel() { return false; }
    public bool ShouldLearnRecipeFromTrainer() { return false; }
    public bool ShouldBuyAndLearnRecipe() { return false; }
    public bool ShouldLearnProfession() { return false; }
    public bool ShouldSellItems() { return false; }
    public bool ShouldCraftOne() { return false; }
    public bool ShouldCraft() { return false; }
    public bool MyLevelIsHighEnough() { return true; }
    public void AddGeneratedStep(Step step) { return; }
    public void SetContext() { return; }
}
