using System.Collections.Generic;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.ObjectManager;

public interface IProfession
{
    SkillLine Name { get; }

    Step CurrentStep { get; set; }
    List<Step> AllSteps { get; }
    List<Item> PrerequisiteItems { get; set; }

    Npc ProfessionTrainer { get; }
    Npc SuppliesVendor { get; }
    string ProfessionSpell { get; set; }
    Item ItemToFarm { get; set; }
    int AmountOfItemToFarm { get; set; }
    int MinimumCharLevel { get; set; }
    List<WoWItem> ItemsToDisenchant { get; set; }
    WoWItem ItemToDelete { get; set; }
    string ItemToSplit { get; set; }
    string City { get; set; }
    IProfession OtherProfession { get; set; }
    int Phase { get; set; }

    bool MustRecalculateStep { get; set; }
    bool HasCheckedIfWeKnowRecipe { get; set; }

    void RegenerateSteps();
    void SetContext();
    void SetOtherProfession();

    bool ShouldCraft();
    bool ShouldSellItems();
    bool ShouldLearnProfession();
    bool ShouldBuyAndLearnRecipe();
    bool ShouldLearnRecipeFromTrainer();
    bool ShouldBuyMaterials();
    bool ShouldTravel();
    bool ShouldSetCurrentStep();
    bool ShouldLoadProfile();
    bool ShouldCraftOne();
    bool ShouldDisenchant();
    bool ShouldFilterLoot();
    bool ShouldSplitItem();
    bool ShouldEnchant();

    void AddGeneratedStep(Step step);
    bool MyLevelIsHighEnough();
}
