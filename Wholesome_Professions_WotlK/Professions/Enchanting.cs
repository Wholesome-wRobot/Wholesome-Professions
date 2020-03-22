using wManager.Wow.Enums;
using Wholesome_Professions_WotlK.Helpers;
using System.Collections.Generic;

public class Enchanting : Profession
{
    public Enchanting() : base (SkillLine.Enchanting)
    {
        RegenerateSteps();
    }

    public new void RegenerateSteps()
    {
        Logger.LogDebug("REGENERATING STEPS");
        SetContext();

        AllSteps.Clear();
        /*
        AllSteps.Add(new Step(0, 45, ItemDB.BoltOfLinenCloth)); // Force precraft
        AllSteps.Add(new Step(45, 70, ItemDB.HeavyLinenGloves, 35));*/

        HasSetCurrentStep = false;
    }

    public new void SetContext()
    {
        // ek = 0, kalimdor = 1, Outlands = 530, Northrend 571
        int profLevel = ToolBox.GetProfessionLevel(ProfessionName);

        if (ToolBox.IsHorde()) // Horde
        {
            if (profLevel < 75)
            {
                MinimumCharLevel = 20;
                Continent = (int)ContinentId.Kalimdor;
                ProfessionSpell = "Apprentice Enchanter";
                ProfessionTrainer = VendorDB.OGEnchantingTrainer;
                SuppliesVendor = VendorDB.OGEnchantingSupplies;
                PrerequisiteItems = new List<Item>()
                {
                    ItemDB.RunedCopperRod
                };
            }
        }
    }
}
