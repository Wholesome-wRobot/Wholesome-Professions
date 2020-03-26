using wManager.Wow.Enums;
using Wholesome_Professions_WotlK.Helpers;

public class Tailoring : Profession
{
    public Tailoring() : base (SkillLine.Tailoring)
    {
        RegenerateSteps();
        // Manage sell list
        ToolBox.ManageSellList(AllSteps);
    }

    public override void RegenerateSteps()
    {
        Logger.LogDebug("REGENERATING STEPS");
        SetContext();

        AllSteps.Clear();

        AllSteps.Add(new Step(this, 0, 45, ItemDB.BoltOfLinenCloth)); // Force precraft
        AllSteps.Add(new Step(this, 45, 70, ItemDB.HeavyLinenGloves, 35));
        AllSteps.Add(new Step(this, 70, 75, ItemDB.ReinforcedLinenCape, 5));
        AllSteps.Add(new Step(this, 75, 97, ItemDB.BoltofWoolenCloth)); // Force precraft
        AllSteps.Add(new Step(this, 97, 110, ItemDB.SimpleKilt, 15));
        AllSteps.Add(new Step(this, 110, 125, ItemDB.DoublestitchedWoolenShoulders, 15));
        AllSteps.Add(new Step(this, 125, 145, ItemDB.BoltOfSilkCloth)); // Force precraft
        AllSteps.Add(new Step(this, 145, 150, ItemDB.AzureSilkHood, 5));
        AllSteps.Add(new Step(this, 150, 160, ItemDB.AzureSilkHood, 15));
        AllSteps.Add(new Step(this, 160, 170, ItemDB.SilkHeadband, 10));
        AllSteps.Add(new Step(this, 170, 175, ItemDB.FormalWhiteShirt, 5));
        AllSteps.Add(new Step(this, 175, 185, ItemDB.BoltOfMageweave)); // Force precraft
        AllSteps.Add(new Step(this, 185, 200, ItemDB.CrimsonSilkVest, 15));
        AllSteps.Add(new Step(this, 200, 215, ItemDB.CrimsonSilkPantaloons, 15));
        AllSteps.Add(new Step(this, 215, 220, ItemDB.BlackMageweaveLeggings, 5));
        AllSteps.Add(new Step(this, 220, 225, ItemDB.BlackMageweaveGloves, 5));
        AllSteps.Add(new Step(this, 225, 230, ItemDB.BlackMageweaveGloves, 5));
        AllSteps.Add(new Step(this, 230, 250, ItemDB.BlackMageweaveHeadband, 23));
        AllSteps.Add(new Step(this, 250, 260, ItemDB.BoltofRunecloth)); // Force precraft
        AllSteps.Add(new Step(this, 260, 280, ItemDB.RuneclothBelt, 30));
        AllSteps.Add(new Step(this, 280, 295, ItemDB.RuneclothGloves, 20));
        AllSteps.Add(new Step(this, 295, 300, ItemDB.RuneclothHeadband, 5));
        AllSteps.Add(new Step(this, 300, 325, ItemDB.BoltofNetherweave)); // Force precraft
        AllSteps.Add(new Step(this, 325, 340, ItemDB.NetherweavePants, 20));
        AllSteps.Add(new Step(this, 340, 350, ItemDB.NetherweaveRobe, 10));
        AllSteps.Add(new Step(this, 350, 375, ItemDB.BoltofFrostweave));
        AllSteps.Add(new Step(this, 375, 380, ItemDB.FrostwovenBelt, 5));
        AllSteps.Add(new Step(this, 380, 385, ItemDB.FrostwovenBoots, 5));
        AllSteps.Add(new Step(this, 385, 395, ItemDB.FrostwovenCowl, 15));
        AllSteps.Add(new Step(this, 395, 400, ItemDB.DuskweaveBelt, 5));
        AllSteps.Add(new Step(this, 400, 410, ItemDB.DuskweaveWristwraps, 10));
        AllSteps.Add(new Step(this, 410, 415, ItemDB.DuskweaveGloves, 5));
        AllSteps.Add(new Step(this, 415, 425, ItemDB.DuskweaveBoots, 15));

        MustRecalculateStep = true;
        HasCheckedIfWeKnowRecipe = false;

        if (Bot.ProfileName != null)
            ProfileHandler.UnloadCurrentProfile();
    }

    public override void SetContext()
    {
        // ek = 0, kalimdor = 1, Outlands = 530, Northrend 571
        int profLevel = ToolBox.GetProfessionLevel(Name);

        if (ToolBox.IsHorde()) // Horde
        {
            if (profLevel < 75)
            {
                MinimumCharLevel = 20;
                ProfessionSpell = "Apprentice Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
                SuppliesVendor = VendorDB.OGTailoringSupplies;
                Phase = 1;
                City = "Orgrimmar";
            }
            else if (profLevel >= 75 && profLevel < 150)
            {
                MinimumCharLevel = 25;
                ProfessionSpell = "Journeyman Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
                SuppliesVendor = VendorDB.OGTailoringSupplies;
                Phase = 2;
                City = "Orgrimmar";
            }
            else if (profLevel >= 150 && profLevel < 225)
            {
                MinimumCharLevel = 30;
                ProfessionSpell = "Expert Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
                SuppliesVendor = VendorDB.OGTailoringSupplies;
                Phase = 3;
                City = "Orgrimmar";
            }
            else if (profLevel >= 225 && profLevel < 300)
            {
                MinimumCharLevel = 35;
                ProfessionSpell = "Artisan Tailor";
                ProfessionTrainer = VendorDB.OGTailoringTrainer;
                SuppliesVendor = VendorDB.OGTailoringSupplies;
                Phase = 4;
                City = "Orgrimmar";
            }
            else if (profLevel >= 300 && profLevel < 350)
            {
                MinimumCharLevel = 68;
                ProfessionSpell = "Master Tailor";
                ProfessionTrainer = VendorDB.ThrallmarTailoringTrainer;
                SuppliesVendor = VendorDB.ShattrathTailoringSupplies;
                Phase = 5;
                City = "Thrallmar";
            }
            else if (profLevel >= 350)
            {
                MinimumCharLevel = 80;
                ProfessionSpell = "Grand Master Tailor";
                ProfessionTrainer = VendorDB.WarsongHoldTailoringTrainer;
                SuppliesVendor = VendorDB.WarsongHoldTailoringSupplies;
                Phase = 6;
                City = "Warsong Hold";
            }
        }
    }

    public override bool ShouldDisenchant() { return false; }
    public override bool ShouldEnchant() { return false; }
}
