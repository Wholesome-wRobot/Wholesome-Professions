using System.Collections.Generic;
using wManager.Wow.Class;

public static class ItemDB
{
    public static Item LinenCloth = new Item();
    public static Item BoltOfLinenCloth = new Item();
    public static Item CoarseThread = new Item();
    public static Item HeavyLinenGloves = new Item();
    public static Item ReinforcedLinenCape = new Item();
    public static Item BoltofWoolenCloth = new Item();
    public static Item WoolCloth = new Item();
    public static Item SilkCloth = new Item();
    public static Item BoltOfSilkCloth = new Item();
    public static Item SimpleKilt = new Item();
    public static Item DoublestitchedWoolenShoulders = new Item();
    public static Item FineThread = new Item();
    public static Item AzureSilkHood = new Item();
    public static Item BlueDye = new Item();
    public static Item SilkHeadband = new Item();
    public static Item FormalWhiteShirt = new Item();
    public static Item Bleach = new Item();
    public static Item MageweaveCloth = new Item();
    public static Item BoltOfMageweave = new Item();
    public static Item CrimsonSilkVest = new Item();
    public static Item RedDye = new Item();
    public static Item CrimsonSilkPantaloons = new Item();
    public static Item SilkenThread = new Item();
    public static Item HeavySilkenThread = new Item();
    public static Item BlackMageweaveLeggings = new Item();
    public static Item BlackMageweaveGloves = new Item();
    public static Item BlackMageweaveHeadband = new Item();
    public static Item Runecloth = new Item();
    public static Item BoltofRunecloth = new Item();
    public static Item RuneclothBelt = new Item();
    public static Item RuneThread = new Item();
    public static Item RuneclothGloves = new Item();
    public static Item RuneclothHeadband = new Item();
    public static Item NetherweaveCloth = new Item();
    public static Item BoltofNetherweave = new Item();
    public static Item BoltofImbuedNetherweave = new Item();
    public static Item NetherweaveBoots = new Item();
    public static Item NetherweaveTunic = new Item();
    public static Item NetherweavePants = new Item();
    public static Item NetherweaveRobe = new Item();
    public static Item FrostweaveCloth = new Item();
    public static Item BoltofFrostweave = new Item();
    public static Item EterniumThread = new Item();
    public static Item FrostwovenBelt = new Item();
    public static Item FrostwovenBoots = new Item();
    public static Item FrostwovenCowl = new Item();
    public static Item DuskweaveBelt = new Item();
    public static Item DuskweaveCowl = new Item();
    public static Item DuskweaveWristwraps = new Item();
    public static Item DuskweaveGloves = new Item();
    public static Item DuskweaveBoots = new Item();
    public static Item RunedCopperRod = new Item();
    public static Item CopperRod = new Item();
    public static Item StrangeDust = new Item();
    public static Item LesserMagicEssence = new Item();
    public static Item GreaterMagicEssence = new Item();
    public static Item EnchantBracerMinorHealth = new Item();
    public static Item EnchantBracerMinorStamina = new Item();

    static ItemDB()
    {
        // *************** ENCHANTING ***************
        // Enchants are treated as items to facilitate mats calculation
        
        EnchantBracerMinorStamina.Name = "Enchant Bracer - Minor Stamina";
        EnchantBracerMinorStamina.IsEnchant = true;
        EnchantBracerMinorStamina.Spell = new Spell("Enchant Bracer - Minor Stamina");
        EnchantBracerMinorStamina.AddMaterial(StrangeDust, 3);
        EnchantBracerMinorStamina.EnchantGearType = "INVTYPE_WRIST";

        EnchantBracerMinorHealth.Name = "Enchant Bracer - Minor Health";
        EnchantBracerMinorHealth.IsEnchant = true;
        EnchantBracerMinorHealth.Spell = new Spell("Enchant Bracer - Minor Health");
        EnchantBracerMinorHealth.AddMaterial(StrangeDust, 1);
        EnchantBracerMinorHealth.EnchantGearType = "INVTYPE_WRIST";
        EnchantBracerMinorHealth.VendorFirst = true;

        GreaterMagicEssence.Name = "Greater Magic Essence";
        GreaterMagicEssence.ItemId = 10939;
        GreaterMagicEssence.CanBeFarmed = true;
        GreaterMagicEssence.Profile = "Horde - Enchanting 1.xml";
        GreaterMagicEssence.SplitsInto = LesserMagicEssence;

        LesserMagicEssence.Name = "Lesser Magic Essence";
        LesserMagicEssence.ItemId = 10938;
        LesserMagicEssence.CanBeFarmed = true;
        LesserMagicEssence.Profile = "Horde - Enchanting 1.xml";
        LesserMagicEssence.SplitsInto = GreaterMagicEssence;
        LesserMagicEssence.AmountRequiredToSplit = 3;

        StrangeDust.Name = "Strange Dust";
        StrangeDust.ItemId = 10940;
        StrangeDust.CanBeFarmed = true;
        StrangeDust.Profile = "Horde - Enchanting 1.xml";

        CopperRod.Name = "Copper Rod";
        CopperRod.ItemId = 6217;
        CopperRod.CanBeBought = true;
        CopperRod.EstimatedPrice = 124;

        RunedCopperRod.Name = "Runed Copper Rod";
        RunedCopperRod.ItemId = 6218;
        RunedCopperRod.AddMaterial(CopperRod, 1);
        RunedCopperRod.AddMaterial(StrangeDust, 3);
        RunedCopperRod.AddMaterial(LesserMagicEssence, 1);
        RunedCopperRod.IsAPrerequisiteItem = true;
        ToolBox.SetSellListForOneItem(RunedCopperRod);
        RunedCopperRod.VendorFirst = true;

        // *************** TAILORING ***************

        DuskweaveBoots.Name = "Duskweave Boots";
        DuskweaveBoots.ItemId = 41544;
        DuskweaveBoots.AddMaterial(EterniumThread, 1);
        DuskweaveBoots.AddMaterial(BoltofFrostweave, 10);
        DuskweaveBoots.ForceSell = true;

        DuskweaveGloves.Name = "Duskweave Gloves";
        DuskweaveGloves.ItemId = 41545;
        DuskweaveGloves.AddMaterial(EterniumThread, 1);
        DuskweaveGloves.AddMaterial(BoltofFrostweave, 9);
        DuskweaveGloves.ForceSell = true;

        DuskweaveWristwraps.Name = "Duskweave Wristwraps";
        DuskweaveWristwraps.ItemId = 41551;
        DuskweaveWristwraps.AddMaterial(EterniumThread, 1);
        DuskweaveWristwraps.AddMaterial(BoltofFrostweave, 8);
        DuskweaveWristwraps.ForceSell = true;

        DuskweaveBelt.Name = "Duskweave Belt";
        DuskweaveBelt.ItemId = 41543;
        DuskweaveBelt.AddMaterial(EterniumThread, 1);
        DuskweaveBelt.AddMaterial(BoltofFrostweave, 7);
        DuskweaveBelt.ForceSell = true;

        FrostwovenCowl.Name = "Frostwoven Cowl";
        FrostwovenCowl.ItemId = 41521;
        FrostwovenCowl.AddMaterial(EterniumThread, 1);
        FrostwovenCowl.AddMaterial(BoltofFrostweave, 5);
        FrostwovenCowl.ForceSell = true;

        FrostwovenBoots.Name = "Frostwoven Boots";
        FrostwovenBoots.ItemId = 41520;
        FrostwovenBoots.AddMaterial(EterniumThread, 1);
        FrostwovenBoots.AddMaterial(BoltofFrostweave, 4);
        FrostwovenBoots.ForceSell = true;

        FrostwovenBelt.Name = "Frostwoven Belt";
        FrostwovenBelt.ItemId = 41522;
        FrostwovenBelt.AddMaterial(EterniumThread, 1);
        FrostwovenBelt.AddMaterial(BoltofFrostweave, 3);
        FrostwovenBelt.ForceSell = true;

        EterniumThread.Name = "Eternium Thread";
        EterniumThread.ItemId = 38426;
        EterniumThread.CanBeBought = true;
        EterniumThread.EstimatedPrice = 30000;

        BoltofFrostweave.Name = "Bolt of Frostweave";
        BoltofFrostweave.ItemId = 41510;
        BoltofFrostweave.AddMaterial(FrostweaveCloth, 5);
        BoltofFrostweave.VendorFirst = true;

        FrostweaveCloth.Name = "Frostweave Cloth";
        FrostweaveCloth.ItemId = 33470;
        FrostweaveCloth.CanBeFarmed = true;

        NetherweaveRobe.Name = "Netherweave Robe";
        NetherweaveRobe.ItemId = 21854;
        NetherweaveRobe.AddMaterial(BoltofNetherweave, 8);
        NetherweaveRobe.AddMaterial(RuneThread, 2);
        NetherweaveRobe.RecipeVendor = VendorDB.ShattrathTailoringSupplies;
        NetherweaveRobe.RecipeItemId = 21896;
        NetherweaveRobe.ForceSell = true;

        NetherweavePants.Name = "Netherweave Pants";
        NetherweavePants.ItemId = 21852;
        NetherweavePants.AddMaterial(BoltofNetherweave, 6);
        NetherweavePants.AddMaterial(RuneThread, 1);
        NetherweavePants.ForceSell = true;

        NetherweaveTunic.Name = "Netherweave Tunic";
        NetherweaveTunic.ItemId = 21855;
        NetherweaveTunic.AddMaterial(BoltofNetherweave, 8);
        NetherweaveTunic.AddMaterial(RuneThread, 2);
        NetherweaveTunic.ForceSell = true;

        BoltofNetherweave.Name = "Bolt of Netherweave";
        BoltofNetherweave.ItemId = 21840;
        BoltofNetherweave.AddMaterial(NetherweaveCloth, 5);
        BoltofNetherweave.VendorFirst = true;

        NetherweaveCloth.Name = "Netherweave Cloth";
        NetherweaveCloth.ItemId = 21877;
        NetherweaveCloth.CanBeFarmed = true;

        RuneclothHeadband.Name = "Runecloth Headband";
        RuneclothHeadband.ItemId = 13866;
        RuneclothHeadband.AddMaterial(BoltofRunecloth, 6);
        RuneclothHeadband.AddMaterial(RuneThread, 2);
        RuneclothHeadband.ForceSell = true;

        RuneclothGloves.Name = "Runecloth Gloves";
        RuneclothGloves.ItemId = 13863;
        RuneclothGloves.AddMaterial(BoltofRunecloth, 5);
        RuneclothGloves.AddMaterial(RuneThread, 2);
        RuneclothGloves.ForceSell = true;

        RuneThread.Name = "Rune Thread";
        RuneThread.ItemId = 14341;
        RuneThread.CanBeBought = true;
        RuneThread.EstimatedPrice = 4750;

        RuneclothBelt.Name = "Runecloth Belt";
        RuneclothBelt.ItemId = 13856;
        RuneclothBelt.AddMaterial(BoltofRunecloth, 3);
        RuneclothBelt.AddMaterial(RuneThread, 1);
        RuneclothBelt.ForceSell = true;

        BoltofRunecloth.Name = "Bolt of Runecloth";
        BoltofRunecloth.ItemId = 14048;
        BoltofRunecloth.AddMaterial(Runecloth, 4);
        BoltofRunecloth.VendorFirst = true;

        Runecloth.Name = "Runecloth";
        Runecloth.ItemId = 14047;
        Runecloth.CanBeFarmed = true;

        BlackMageweaveHeadband.Name = "Black Mageweave Headband";
        BlackMageweaveHeadband.ItemId = 10024;
        BlackMageweaveHeadband.AddMaterial(BoltOfMageweave, 3);
        BlackMageweaveHeadband.AddMaterial(HeavySilkenThread, 2);
        BlackMageweaveHeadband.ForceSell = true;

        HeavySilkenThread.Name = "Heavy Silken Thread";
        HeavySilkenThread.ItemId = 8343;
        HeavySilkenThread.CanBeBought = true;
        HeavySilkenThread.EstimatedPrice = 1900;

        BlackMageweaveGloves.Name = "Black Mageweave Gloves";
        BlackMageweaveGloves.ItemId = 10003;
        BlackMageweaveGloves.AddMaterial(BoltOfMageweave, 2);
        BlackMageweaveGloves.AddMaterial(HeavySilkenThread, 2);
        BlackMageweaveGloves.ForceSell = true;

        BlackMageweaveLeggings.Name = "Black Mageweave Leggings";
        BlackMageweaveLeggings.ItemId = 9999;
        BlackMageweaveLeggings.AddMaterial(BoltOfMageweave, 2);
        BlackMageweaveLeggings.AddMaterial(SilkenThread, 3);
        BlackMageweaveLeggings.ForceSell = true;

        SilkenThread.Name = "Silken Thread";
        SilkenThread.ItemId = 4291;
        SilkenThread.CanBeBought = true;
        SilkenThread.EstimatedPrice = 500;

        CrimsonSilkPantaloons.Name = "Crimson Silk Pantaloons";
        CrimsonSilkPantaloons.ItemId = 7062;
        CrimsonSilkPantaloons.AddMaterial(BoltOfSilkCloth, 4);
        CrimsonSilkPantaloons.AddMaterial(RedDye, 2);
        CrimsonSilkPantaloons.AddMaterial(SilkenThread, 2);
        CrimsonSilkPantaloons.ForceSell = true;
        
        RedDye.Name = "Red Dye";
        RedDye.ItemId = 2604;
        RedDye.CanBeBought = true;
        RedDye.EstimatedPrice = 47;

        CrimsonSilkVest.Name = "Crimson Silk Vest";
        CrimsonSilkVest.ItemId = 7058;
        CrimsonSilkVest.AddMaterial(BoltOfSilkCloth, 4);
        CrimsonSilkVest.AddMaterial(FineThread, 2);
        CrimsonSilkVest.AddMaterial(RedDye, 2);
        CrimsonSilkVest.ForceSell = true;

        BoltOfMageweave.Name = "Bolt of Mageweave";
        BoltOfMageweave.ItemId = 4339;
        BoltOfMageweave.AddMaterial(MageweaveCloth, 4);
        BoltOfMageweave.VendorFirst = true;

        MageweaveCloth.Name = "Mageweave Cloth";
        MageweaveCloth.ItemId = 4338;
        MageweaveCloth.CanBeFarmed = true;

        Bleach.Name = "Bleach";
        Bleach.ItemId = 2324;
        Bleach.CanBeBought = true;
        Bleach.EstimatedPrice = 23;

        FormalWhiteShirt.Name = "Formal White Shirt";
        FormalWhiteShirt.ItemId = 4334;
        FormalWhiteShirt.AddMaterial(BoltOfSilkCloth, 3);
        FormalWhiteShirt.AddMaterial(FineThread, 1);
        FormalWhiteShirt.AddMaterial(Bleach, 2);
        FormalWhiteShirt.ForceSell = true;

        SilkHeadband.Name = "Silk Headband";
        SilkHeadband.ItemId = 7050;
        SilkHeadband.AddMaterial(BoltOfSilkCloth, 3);
        SilkHeadband.AddMaterial(FineThread, 2);
        SilkHeadband.ForceSell = true;

        BlueDye.Name = "Blue Dye";
        BlueDye.ItemId = 6260;
        BlueDye.CanBeBought = true;
        BlueDye.EstimatedPrice = 47;

        AzureSilkHood.Name = "Azure Silk Hood";
        AzureSilkHood.ItemId = 7048;
        AzureSilkHood.AddMaterial(BoltOfSilkCloth, 2);
        AzureSilkHood.AddMaterial(FineThread, 1);
        AzureSilkHood.AddMaterial(BlueDye, 2);
        AzureSilkHood.ForceSell = true;

        BoltOfSilkCloth.Name = "Bolt of Silk Cloth";
        BoltOfSilkCloth.ItemId = 4305;
        BoltOfSilkCloth.AddMaterial(SilkCloth, 4);
        BoltOfSilkCloth.VendorFirst = true;

        DoublestitchedWoolenShoulders.Name = "Double-stitched Woolen Shoulders";
        DoublestitchedWoolenShoulders.ItemId = 4314;
        DoublestitchedWoolenShoulders.AddMaterial(BoltofWoolenCloth, 3);
        DoublestitchedWoolenShoulders.AddMaterial(FineThread, 2);
        DoublestitchedWoolenShoulders.ForceSell = true;

        FineThread.Name = "Fine Thread";
        FineThread.ItemId = 2321;
        FineThread.CanBeBought = true;
        FineThread.EstimatedPrice = 95;

        SimpleKilt.Name = "Simple Kilt";
        SimpleKilt.ItemId = 10047;
        SimpleKilt.AddMaterial(BoltOfLinenCloth, 4);
        SimpleKilt.AddMaterial(FineThread, 1);
        SimpleKilt.ForceSell = true;

        SilkCloth.Name = "Silk Cloth";
        SilkCloth.ItemId = 4306;
        SilkCloth.CanBeFarmed = true;

        WoolCloth.Name = "Wool Cloth";
        WoolCloth.ItemId = 2592;
        WoolCloth.CanBeFarmed = true;

        BoltofWoolenCloth.Name = "Bolt of Woolen Cloth";
        BoltofWoolenCloth.ItemId = 2997;
        BoltofWoolenCloth.AddMaterial(WoolCloth, 3);
        BoltofWoolenCloth.VendorFirst = true;

        LinenCloth.Name = "Linen Cloth";
        LinenCloth.ItemId = 2589;
        LinenCloth.CanBeFarmed = true;

        BoltOfLinenCloth.Name = "Bolt of Linen Cloth";
        BoltOfLinenCloth.ItemId = 2996;
        BoltOfLinenCloth.AddMaterial(LinenCloth, 2);
        BoltOfLinenCloth.VendorFirst = true;

        CoarseThread.Name = "Coarse Thread";
        CoarseThread.ItemId = 2320;
        CoarseThread.CanBeBought = true;
        CoarseThread.EstimatedPrice = 9;

        HeavyLinenGloves.Name = "Heavy Linen Gloves";
        HeavyLinenGloves.ItemId = 4307;
        HeavyLinenGloves.AddMaterial(BoltOfLinenCloth, 2);
        HeavyLinenGloves.AddMaterial(CoarseThread, 1);
        HeavyLinenGloves.ForceSell = true;

        ReinforcedLinenCape.Name = "Reinforced Linen Cape";
        ReinforcedLinenCape.ItemId = 2580;
        ReinforcedLinenCape.AddMaterial(BoltOfLinenCloth, 2);
        ReinforcedLinenCape.AddMaterial(CoarseThread, 3);
        ReinforcedLinenCape.ForceSell = true;
    }

}
