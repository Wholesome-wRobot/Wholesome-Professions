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
    public static Item ArcaneDust = new Item();
    public static Item NetherweaveBoots = new Item();
    public static Item KnothideLeather = new Item();
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

    static ItemDB()
    {
        DuskweaveBoots.name = "Duskweave Boots";
        DuskweaveBoots.itemId = 41544;
        DuskweaveBoots.AddMaterial(EterniumThread, 1);
        DuskweaveBoots.AddMaterial(BoltofFrostweave, 10);
        DuskweaveBoots.forceSell = true;

        DuskweaveGloves.name = "Duskweave Gloves";
        DuskweaveGloves.itemId = 41545;
        DuskweaveGloves.AddMaterial(EterniumThread, 1);
        DuskweaveGloves.AddMaterial(BoltofFrostweave, 9);
        DuskweaveGloves.forceSell = true;

        DuskweaveWristwraps.name = "Duskweave Wristwraps";
        DuskweaveWristwraps.itemId = 41551;
        DuskweaveWristwraps.AddMaterial(EterniumThread, 1);
        DuskweaveWristwraps.AddMaterial(BoltofFrostweave, 8);
        DuskweaveWristwraps.forceSell = true;

        DuskweaveBelt.name = "Duskweave Belt";
        DuskweaveBelt.itemId = 41543;
        DuskweaveBelt.AddMaterial(EterniumThread, 1);
        DuskweaveBelt.AddMaterial(BoltofFrostweave, 7);
        DuskweaveBelt.forceSell = true;

        FrostwovenCowl.name = "Frostwoven Cowl";
        FrostwovenCowl.itemId = 41521;
        FrostwovenCowl.AddMaterial(EterniumThread, 1);
        FrostwovenCowl.AddMaterial(BoltofFrostweave, 5);
        FrostwovenCowl.forceSell = true;

        FrostwovenBoots.name = "Frostwoven Boots";
        FrostwovenBoots.itemId = 41520;
        FrostwovenBoots.AddMaterial(EterniumThread, 1);
        FrostwovenBoots.AddMaterial(BoltofFrostweave, 4);
        FrostwovenBoots.forceSell = true;

        FrostwovenBelt.name = "Frostwoven Belt";
        FrostwovenBelt.itemId = 41522;
        FrostwovenBelt.AddMaterial(EterniumThread, 1);
        FrostwovenBelt.AddMaterial(BoltofFrostweave, 3);
        FrostwovenBelt.forceSell = true;

        EterniumThread.name = "Eternium Thread";
        EterniumThread.itemId = 38426;
        EterniumThread.canBeBought = true;
        EterniumThread.estimatedPrice = 30000;

        BoltofFrostweave.name = "Bolt of Frostweave";
        BoltofFrostweave.itemId = 41510;
        BoltofFrostweave.AddMaterial(FrostweaveCloth, 5);

        FrostweaveCloth.name = "Frostweave Cloth";
        FrostweaveCloth.itemId = 33470;
        FrostweaveCloth.canBeFarmed = true;

        NetherweaveRobe.name = "Netherweave Robe";
        NetherweaveRobe.itemId = 21854;
        NetherweaveRobe.AddMaterial(BoltofNetherweave, 8);
        NetherweaveRobe.AddMaterial(RuneThread, 2);
        NetherweaveRobe.RecipeVendor = VendorDB.ShattrathTailoringSupplies;
        NetherweaveRobe.RecipeItemId = 21896;
        NetherweaveRobe.forceSell = true;

        NetherweavePants.name = "Netherweave Pants";
        NetherweavePants.itemId = 21852;
        NetherweavePants.AddMaterial(BoltofNetherweave, 6);
        NetherweavePants.AddMaterial(RuneThread, 1);
        NetherweavePants.forceSell = true;

        NetherweaveTunic.name = "Netherweave Tunic";
        NetherweaveTunic.itemId = 21855;
        NetherweaveTunic.AddMaterial(BoltofNetherweave, 8);
        NetherweaveTunic.AddMaterial(RuneThread, 2);
        NetherweaveTunic.forceSell = true;

        KnothideLeather.name = "Knothide Leather";
        KnothideLeather.itemId = 21887;
        KnothideLeather.canBeFarmed = true; // change this

        ArcaneDust.name = "Arcane Dust";
        ArcaneDust.itemId = 22445;
        ArcaneDust.canBeFarmed = true; // change this

        NetherweaveBoots.name = "Netherweave Boots";
        NetherweaveBoots.itemId = 21853;
        NetherweaveBoots.AddMaterial(BoltofNetherweave, 6);
        NetherweaveBoots.AddMaterial(KnothideLeather, 2);
        NetherweaveBoots.AddMaterial(RuneThread, 1);
        NetherweaveBoots.forceSell = true;

        BoltofImbuedNetherweave.name = "Bolt of Imbued Netherweave";
        BoltofImbuedNetherweave.itemId = 21842;
        BoltofImbuedNetherweave.AddMaterial(BoltofNetherweave, 3);
        BoltofImbuedNetherweave.AddMaterial(ArcaneDust, 2);
        BoltofImbuedNetherweave.RecipeVendor = VendorDB.ShattrathTailoringSupplies;
        BoltofImbuedNetherweave.RecipeItemId = 21892;

        BoltofNetherweave.name = "Bolt of Netherweave";
        BoltofNetherweave.itemId = 21840;
        BoltofNetherweave.AddMaterial(NetherweaveCloth, 5);

        NetherweaveCloth.name = "Netherweave Cloth";
        NetherweaveCloth.itemId = 21877;
        NetherweaveCloth.canBeFarmed = true;

        RuneclothHeadband.name = "Runecloth Headband";
        RuneclothHeadband.itemId = 13866;
        RuneclothHeadband.AddMaterial(BoltofRunecloth, 6);
        RuneclothHeadband.AddMaterial(RuneThread, 2);
        RuneclothHeadband.forceSell = true;

        RuneclothGloves.name = "Runecloth Gloves";
        RuneclothGloves.itemId = 13863;
        RuneclothGloves.AddMaterial(BoltofRunecloth, 5);
        RuneclothGloves.AddMaterial(RuneThread, 2);
        RuneclothGloves.forceSell = true;

        RuneThread.name = "Rune Thread";
        RuneThread.itemId = 14341;
        RuneThread.canBeBought = true;
        RuneThread.estimatedPrice = 4750;

        RuneclothBelt.name = "Runecloth Belt";
        RuneclothBelt.itemId = 13856;
        RuneclothBelt.AddMaterial(BoltofRunecloth, 3);
        RuneclothBelt.AddMaterial(RuneThread, 1);
        RuneclothBelt.forceSell = true;

        BoltofRunecloth.name = "Bolt of Runecloth";
        BoltofRunecloth.itemId = 14048;
        BoltofRunecloth.AddMaterial(Runecloth, 4);

        Runecloth.name = "Runecloth";
        Runecloth.itemId = 14047;
        Runecloth.canBeFarmed = true;

        BlackMageweaveHeadband.name = "Black Mageweave Headband";
        BlackMageweaveHeadband.itemId = 10024;
        BlackMageweaveHeadband.AddMaterial(BoltOfMageweave, 3);
        BlackMageweaveHeadband.AddMaterial(HeavySilkenThread, 2);
        BlackMageweaveHeadband.forceSell = true;

        HeavySilkenThread.name = "Heavy Silken Thread";
        HeavySilkenThread.itemId = 8343;
        HeavySilkenThread.canBeBought = true;
        HeavySilkenThread.estimatedPrice = 1900;

        BlackMageweaveGloves.name = "Black Mageweave Gloves";
        BlackMageweaveGloves.itemId = 10003;
        BlackMageweaveGloves.AddMaterial(BoltOfMageweave, 2);
        BlackMageweaveGloves.AddMaterial(HeavySilkenThread, 2);
        BlackMageweaveGloves.forceSell = true;

        BlackMageweaveLeggings.name = "Black Mageweave Leggings";
        BlackMageweaveLeggings.itemId = 9999;
        BlackMageweaveLeggings.AddMaterial(BoltOfMageweave, 2);
        BlackMageweaveLeggings.AddMaterial(SilkenThread, 3);
        BlackMageweaveLeggings.forceSell = true;

        SilkenThread.name = "Silken Thread";
        SilkenThread.itemId = 4291;
        SilkenThread.canBeBought = true;
        SilkenThread.estimatedPrice = 500;

        CrimsonSilkPantaloons.name = "Crimson Silk Pantaloons";
        CrimsonSilkPantaloons.itemId = 7062;
        CrimsonSilkPantaloons.AddMaterial(BoltOfSilkCloth, 4);
        CrimsonSilkPantaloons.AddMaterial(RedDye, 2);
        CrimsonSilkPantaloons.AddMaterial(SilkenThread, 2);
        CrimsonSilkPantaloons.forceSell = true;
        
        RedDye.name = "Red Dye";
        RedDye.itemId = 2604;
        RedDye.canBeBought = true;
        RedDye.estimatedPrice = 47;

        CrimsonSilkVest.name = "Crimson Silk Vest";
        CrimsonSilkVest.itemId = 7058;
        CrimsonSilkVest.AddMaterial(BoltOfSilkCloth, 4);
        CrimsonSilkVest.AddMaterial(FineThread, 2);
        CrimsonSilkVest.AddMaterial(RedDye, 2);
        CrimsonSilkVest.forceSell = true;

        BoltOfMageweave.name = "Bolt of Mageweave";
        BoltOfMageweave.itemId = 4339;
        BoltOfMageweave.AddMaterial(MageweaveCloth, 4);

        MageweaveCloth.name = "Mageweave Cloth";
        MageweaveCloth.itemId = 4338;
        MageweaveCloth.canBeFarmed = true;

        Bleach.name = "Bleach";
        Bleach.itemId = 2324;
        Bleach.canBeBought = true;
        Bleach.estimatedPrice = 23;

        FormalWhiteShirt.name = "Formal White Shirt";
        FormalWhiteShirt.itemId = 4334;
        FormalWhiteShirt.AddMaterial(BoltOfSilkCloth, 3);
        FormalWhiteShirt.AddMaterial(FineThread, 1);
        FormalWhiteShirt.AddMaterial(Bleach, 2);
        FormalWhiteShirt.forceSell = true;

        SilkHeadband.name = "Silk Headband";
        SilkHeadband.itemId = 7050;
        SilkHeadband.AddMaterial(BoltOfSilkCloth, 3);
        SilkHeadband.AddMaterial(FineThread, 2);
        SilkHeadband.forceSell = true;

        BlueDye.name = "Blue Dye";
        BlueDye.itemId = 6260;
        BlueDye.canBeBought = true;
        BlueDye.estimatedPrice = 47;

        AzureSilkHood.name = "Azure Silk Hood";
        AzureSilkHood.itemId = 7048;
        AzureSilkHood.AddMaterial(BoltOfSilkCloth, 2);
        AzureSilkHood.AddMaterial(FineThread, 1);
        AzureSilkHood.AddMaterial(BlueDye, 2);
        AzureSilkHood.forceSell = true;

        BoltOfSilkCloth.name = "Bolt of Silk Cloth";
        BoltOfSilkCloth.itemId = 4305;
        BoltOfSilkCloth.AddMaterial(SilkCloth, 4);

        DoublestitchedWoolenShoulders.name = "Double-stitched Woolen Shoulders";
        DoublestitchedWoolenShoulders.itemId = 4314;
        DoublestitchedWoolenShoulders.AddMaterial(BoltofWoolenCloth, 3);
        DoublestitchedWoolenShoulders.AddMaterial(FineThread, 2);
        DoublestitchedWoolenShoulders.forceSell = true;

        FineThread.name = "Fine Thread";
        FineThread.itemId = 2321;
        FineThread.canBeBought = true;
        FineThread.estimatedPrice = 95;

        SimpleKilt.name = "Simple Kilt";
        SimpleKilt.itemId = 10047;
        SimpleKilt.AddMaterial(BoltOfLinenCloth, 4);
        SimpleKilt.AddMaterial(FineThread, 1);
        SimpleKilt.forceSell = true;

        SilkCloth.name = "Silk Cloth";
        SilkCloth.itemId = 4306;
        SilkCloth.canBeFarmed = true;

        WoolCloth.name = "Wool Cloth";
        WoolCloth.itemId = 2592;
        WoolCloth.canBeFarmed = true;

        BoltofWoolenCloth.name = "Bolt of Woolen Cloth";
        BoltofWoolenCloth.itemId = 2997;
        BoltofWoolenCloth.AddMaterial(WoolCloth, 3);

        LinenCloth.name = "Linen Cloth";
        LinenCloth.itemId = 2589;
        LinenCloth.canBeFarmed = true;

        BoltOfLinenCloth.name = "Bolt of Linen Cloth";
        BoltOfLinenCloth.itemId = 2996;
        BoltOfLinenCloth.AddMaterial(LinenCloth, 2);

        CoarseThread.name = "Coarse Thread";
        CoarseThread.itemId = 2320;
        CoarseThread.canBeBought = true;
        CoarseThread.estimatedPrice = 9;

        HeavyLinenGloves.name = "Heavy Linen Gloves";
        HeavyLinenGloves.itemId = 4307;
        HeavyLinenGloves.AddMaterial(BoltOfLinenCloth, 2);
        HeavyLinenGloves.AddMaterial(CoarseThread, 1);
        HeavyLinenGloves.forceSell = true;

        ReinforcedLinenCape.name = "Reinforced Linen Cape";
        ReinforcedLinenCape.itemId = 2580;
        ReinforcedLinenCape.AddMaterial(BoltOfLinenCloth, 2);
        ReinforcedLinenCape.AddMaterial(CoarseThread, 3);
        ReinforcedLinenCape.forceSell = true;
    }

}
