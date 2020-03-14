using System.Collections.Generic;
using System.Diagnostics;
using wManager.Wow.Class;
using wManager.Wow.Enums;

public interface IProfession
{
    SkillLine ProfessionName { get; }

    Step CurrentStep { get; set; }
    List<Step> AllSteps { get; }
    
    Npc ProfessionTrainer { get; }
    Npc SuppliesVendor { get; }
    string ProfessionSpell { get; set; }
    Item ItemToFarm { get; set; }
    int AmountOfItemToFarm { get; set; }
    int Continent { get; set; }
    int MinimumCharLevel { get; set; }

    bool ShouldCraftFlag { get; set; }
    bool HasSetCurrentStep { get; set; }

    void RegenerateSteps();
    
    void SetTrainer();

    bool ShouldCraft();
    bool ShouldSellItems();
    bool ShouldLearnProfession();
    bool ShouldBuyAndLearnRecipe();
    bool ShouldLearnRecipeFromTrainer();
    bool ShouldBuyMaterials();
    bool ShouldTravel();
    bool ShouldSetCurrentStep();
    void AddGeneratedStep(Step step);
    bool MyLevelIsHighEnough();
}
