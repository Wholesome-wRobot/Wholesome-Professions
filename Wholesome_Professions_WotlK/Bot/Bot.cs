using System;
using robotManager.FiniteStateMachine;
using robotManager.Helpful;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Profile;
using Wholesome_Professions_WotlK.States;
using wManager.Wow.Bot.States;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;

internal static class Bot
{
    private static readonly Engine Fsm = new Engine();
    internal static GrinderProfile Profile = new GrinderProfile();
    internal static readonly Grinding Grinding = new Grinding { Priority = 2 };
    internal static readonly MovementLoop MovementLoop = new MovementLoop { Priority = 1 };

    public static string ProfileName { get; set; } = null;
    public static string ProfileProfession { get; set; }
    public static ContinentId Continent { get; set; }
    public static bool HasSetContinent { get; set; }

    internal static bool Pulse()
    {
        try
        {
            // Attach onlevelup for spell book:
            EventsLua.AttachEventLua(LuaEventsId.PLAYER_LEVEL_UP, m => OnLevelUp());
            EventsLua.AttachEventLua(LuaEventsId.PLAYER_ENTERING_WORLD, m => ScreenReloaded());

            // Update spell list
            SpellManager.UpdateSpellBook();

            // Load CC:
            CustomClass.LoadCustomClass();

            // FSM
            Fsm.States.Clear();

            Fsm.AddState(new Relogger { Priority = 200 });
            Fsm.AddState(new Pause { Priority = 32 });
            Fsm.AddState(new Resurrect { Priority = 31 });
            Fsm.AddState(new MyMacro { Priority = 30 });
            Fsm.AddState(new IsAttacked { Priority = 29 });
            Fsm.AddState(new Regeneration { Priority = 28 });
            Fsm.AddState(new Looting { Priority = 27 });
            Fsm.AddState(new Farming { Priority = 26 });
            Fsm.AddState(new ToTown { Priority = 25 });

            Fsm.AddState(new SellItemsState { Priority = 24 });

            Fsm.AddState(new SetCurrentStepState { Priority = 21 });
            Fsm.AddState(new SplitItemState { Priority = 19 });
            Fsm.AddState(new DisenchantState { Priority = 18 });
            Fsm.AddState(new FilterLootState { Priority = 17 });
            Fsm.AddState(new TravelState { Priority = 16 });
            Fsm.AddState(new LearnProfessionState { Priority = 15 });
            Fsm.AddState(new BuyAndLearnRecipeState { Priority = 14 });
            Fsm.AddState(new LearnRecipeFromTrainerState { Priority = 13 });
            Fsm.AddState(new BuyMaterialsState { Priority = 12 });
            Fsm.AddState(new CraftOneState { Priority = 11 });
            Fsm.AddState(new EnchantState { Priority = 10 });
            Fsm.AddState(new CraftState { Priority = 9 });
            Fsm.AddState(new LoadProfileState { Priority = 8 });

            Fsm.AddState(Grinding);
            Fsm.AddState(MovementLoop);

            Fsm.AddState(new Idle { Priority = 0 });

            Fsm.States.Sort();
            Fsm.StartEngine(10, "_Profession"); // Fsm.StartEngine(25);

            wManager.wManagerSetting.AddBlackListZone(new Vector3(1731.702, -4423.403, 36.86293, "None"), 5.00f, true);
            wManager.wManagerSetting.AddBlackListZone(new Vector3(1669.99, -4359.609, 29.23425, "None"), 5.00f, true);

            StopBotIf.LaunchNewThread();

            return true;
        }
        catch (Exception e)
        {
            try
            {
                Dispose();
            }
            catch
            {
            }
            Logging.WriteError("Bot > Bot  > Pulse(): " + e);
            return false;
        }
    }

    internal static void Dispose()
    {
        try
        {
            CustomClass.DisposeCustomClass();
            Fsm.StopEngine();
            Fight.StopFight();
            MovementManager.StopMove();
            ProfileHandler.UnloadCurrentProfile();
        }
        catch (Exception e)
        {
            Logging.WriteError("Bot > Bot  > Dispose(): " + e);
        }
    }

    public static void SetContinent(ContinentId continent)
    {
        HasSetContinent = true;
        Continent = continent;
    }

    private static void OnLevelUp()
    {
        Logging.Write("Level UP! Reload Fight Class.");
        SpellManager.UpdateSpellBook();
        CustomClass.ResetCustomClass();
    }

    private static void ScreenReloaded()
    {
        Logger.Log("Reloading steps");
        if (Main.amountProfessionsSelected > 0)
        {
            if (Main.primaryProfession != null)
                Main.primaryProfession.MustRecalculateStep = true;
            if (Main.secondaryProfession != null)
                Main.secondaryProfession.MustRecalculateStep = true;
        }
    }
}