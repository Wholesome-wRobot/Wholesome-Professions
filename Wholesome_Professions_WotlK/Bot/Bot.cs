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
            Fsm.AddState(new Pause { Priority = 26 });
            Fsm.AddState(new Resurrect { Priority = 23 });
            Fsm.AddState(new MyMacro { Priority = 22 });
            Fsm.AddState(new IsAttacked { Priority = 21 });
            Fsm.AddState(new Regeneration { Priority = 20 });
            Fsm.AddState(new Looting { Priority = 19 });
            Fsm.AddState(new Farming { Priority = 18 });
            Fsm.AddState(new ToTown { Priority = 17 });

            Fsm.AddState(new SellItemsState { Priority = 16 });

            Fsm.AddState(new SetCurrentStepState { Priority = 13 });
            Fsm.AddState(new TravelState { Priority = 12 });
            Fsm.AddState(new LearnProfessionState { Priority = 11 });
            Fsm.AddState(new BuyAndLearnRecipeState { Priority = 10 });
            Fsm.AddState(new LearnRecipeFromTrainerState { Priority = 9 });
            Fsm.AddState(new BuyMaterialsState { Priority = 8 });
            Fsm.AddState(new CraftOneState { Priority = 7 });
            Fsm.AddState(new CraftState { Priority = 6 });
            Fsm.AddState(new LoadProfileState { Priority = 5 });

            Fsm.AddState(Grinding);
            Fsm.AddState(MovementLoop);

            Fsm.AddState(new Idle { Priority = 0 });

            Fsm.States.Sort();
            Fsm.StartEngine(10, "_Profession"); // Fsm.StartEngine(25);

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
                Main.primaryProfession.HasSetCurrentStep = false;
            if (Main.secondaryProfession != null)
                Main.secondaryProfession.HasSetCurrentStep = false;
        }
    }
}