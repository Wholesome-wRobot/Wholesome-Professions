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
    int Phase { get; set; }

    bool MustRecalculateStepFlag { get; set; }
    bool HasCheckedIfWeKnowRecipeFlag { get; set; }
    bool UserMustBuyManuallyFlag { get; set; }

    void RegenerateSteps();
    void SetContext();

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
    void ReevaluateStep();

    // CHECKS
    bool WeHaveItemsToFarm(string key = null);
    bool MyCharLevelIsHighEnough(string key = null);
    bool CurrenStepIsNull(string key = null);
    bool UserMustBuyManuallyIsTrue(string key = null);
    bool MustRecalculateIsTrue(string key = null);
    bool BotProfileNameIsNull(string key = null);
    bool VendorIsConfirmed(string key = null);
    bool WeAreOnRightContinent(string key = null);
    bool WeNeedToTrain(string key = null);
    bool WeKnowTheRecipe(string key = null);
    bool WeHaveAllMats(string key = null);
    bool RecipeMustBeBought(string key = null);
    bool WeCanBuyRemainingMats(string key = null);
    bool WeHaveMatsToCraftOne(string key = null);
    bool CurrentStepIsACraftAll(string key = null);
    bool CurrentStepIsACraftToLevel(string key = null);
    bool CurrentStepIsAListPrecraft(string key = null);
    bool ItemToCraftIsAnEnchant(string key = null);
}
