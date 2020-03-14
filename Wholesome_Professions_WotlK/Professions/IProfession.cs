using System.Collections.Generic;
using System.Diagnostics;
using wManager.Wow.Class;
using wManager.Wow.Enums;

public interface IProfession
{
    SkillLine ProfessionName { get; }

    Step CurrentStep { get; }
    List<Step> AllSteps { get; }
    
    Npc ProfessionTrainer { get; }
    Npc SuppliesVendor { get; }
    string ProfessionSpell { get; set; }
    Item ItemToFarm { get; set; }
    int AmountOfItemToFarm { get; set; }
    int Continent { get; set; }
    int MinimumCharLevel { get; set; }

    void SetCurrentStep();
    void SetTrainer();
    bool ShouldCraft();
    void RegenerateSteps();
}
