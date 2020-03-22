using wManager.Wow.Enums;
using Wholesome_Professions_WotlK.Helpers;

public class Tailoring : Profession
{
    public Tailoring() : base (SkillLine.Tailoring)
    {
        RegenerateSteps();
    }

    public new void RegenerateSteps()
    {
        Logger.LogDebug("REGENERATING STEPS");
        SetContext();

        AllSteps.Clear();

        AllSteps.Add(new Step(0, 45, ItemDB.BoltOfLinenCloth)); // Force precraft
        AllSteps.Add(new Step(45, 70, ItemDB.HeavyLinenGloves, 35));
        AllSteps.Add(new Step(70, 75, ItemDB.ReinforcedLinenCape, 5));
        AllSteps.Add(new Step(75, 97, ItemDB.BoltofWoolenCloth)); // Force precraft
        AllSteps.Add(new Step(97, 110, ItemDB.SimpleKilt, 15));
        AllSteps.Add(new Step(110, 125, ItemDB.DoublestitchedWoolenShoulders, 15));
        AllSteps.Add(new Step(125, 145, ItemDB.BoltOfSilkCloth)); // Force precraft
        AllSteps.Add(new Step(145, 150, ItemDB.AzureSilkHood, 5));
        AllSteps.Add(new Step(150, 160, ItemDB.AzureSilkHood, 15));
        AllSteps.Add(new Step(160, 170, ItemDB.SilkHeadband, 10));
        AllSteps.Add(new Step(170, 175, ItemDB.FormalWhiteShirt, 5));
        AllSteps.Add(new Step(175, 185, ItemDB.BoltOfMageweave)); // Force precraft
        AllSteps.Add(new Step(185, 200, ItemDB.CrimsonSilkVest, 15));
        AllSteps.Add(new Step(200, 215, ItemDB.CrimsonSilkPantaloons, 15));
        AllSteps.Add(new Step(215, 220, ItemDB.BlackMageweaveLeggings, 5));
        AllSteps.Add(new Step(220, 225, ItemDB.BlackMageweaveGloves, 5));
        AllSteps.Add(new Step(225, 230, ItemDB.BlackMageweaveGloves, 5));
        AllSteps.Add(new Step(230, 250, ItemDB.BlackMageweaveHeadband, 23));
        AllSteps.Add(new Step(250, 260, ItemDB.BoltofRunecloth)); // Force precraft
        AllSteps.Add(new Step(260, 280, ItemDB.RuneclothBelt, 30));
        AllSteps.Add(new Step(280, 295, ItemDB.RuneclothGloves, 20));
        AllSteps.Add(new Step(295, 300, ItemDB.RuneclothHeadband, 5));
        AllSteps.Add(new Step(300, 325, ItemDB.BoltofNetherweave)); // Force precraft
        AllSteps.Add(new Step(325, 340, ItemDB.NetherweavePants, 20));
        AllSteps.Add(new Step(340, 350, ItemDB.NetherweaveRobe, 10));
        AllSteps.Add(new Step(350, 375, ItemDB.BoltofFrostweave));
        AllSteps.Add(new Step(375, 380, ItemDB.FrostwovenBelt, 5));
        AllSteps.Add(new Step(380, 385, ItemDB.FrostwovenBoots, 5));
        AllSteps.Add(new Step(385, 395, ItemDB.FrostwovenCowl, 15));
        AllSteps.Add(new Step(395, 400, ItemDB.DuskweaveBelt, 5));
        AllSteps.Add(new Step(400, 410, ItemDB.DuskweaveWristwraps, 10));
        AllSteps.Add(new Step(410, 415, ItemDB.DuskweaveGloves, 5));
        AllSteps.Add(new Step(415, 425, ItemDB.DuskweaveBoots, 15));

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
                ProfessionSpell = "Apprentice Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
                SuppliesVendor = VendorDB.OGTailoringSupplies;
            }
            else if (profLevel >= 75 && profLevel < 150)
            {
                MinimumCharLevel = 25;
                Continent = (int)ContinentId.Kalimdor;
                ProfessionSpell = "Journeyman Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
                SuppliesVendor = VendorDB.OGTailoringSupplies;
            }
            else if (profLevel >= 150 && profLevel < 225)
            {
                MinimumCharLevel = 30;
                Continent = (int)ContinentId.Kalimdor;
                ProfessionSpell = "Expert Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
                SuppliesVendor = VendorDB.OGTailoringSupplies;
            }
            else if (profLevel >= 225 && profLevel < 300)
            {
                Continent = (int)ContinentId.Kalimdor;
                MinimumCharLevel = 35;
                ProfessionSpell = "Artisan Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
                SuppliesVendor = VendorDB.OGTailoringSupplies;
            }
            else if (profLevel >= 300 && profLevel < 350)
            {
                Continent = (int)ContinentId.Expansion01;
                MinimumCharLevel = 68;
                ProfessionSpell = "Master Tailor";
                ProfessionTrainer = VendorDB.ThrallmarTailoringTrainer;
                SuppliesVendor = VendorDB.ShattrathTailoringSupplies;
            }
            else if (profLevel >= 350)
            {
                Continent = (int)ContinentId.Northrend;
                MinimumCharLevel = 80;
                ProfessionSpell = "Grand Master Tailor";
                ProfessionTrainer = VendorDB.WarsongHoldTailoringTrainer;
                SuppliesVendor = VendorDB.WarsongHoldTailoringSupplies;
            }
        }
    }
}
