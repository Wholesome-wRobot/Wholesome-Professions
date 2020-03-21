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
    
    bool HasSetCurrentStep { get; set; }
    string CurrentProfile { get; set; }

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
    bool ShouldSelectProfile();
    bool ShouldCraftOne();

    void AddGeneratedStep(Step step);
    bool MyLevelIsHighEnough();
}
