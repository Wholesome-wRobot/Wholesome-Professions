using System;
using robotManager.FiniteStateMachine;
using robotManager.Helpful;
using Wholesome_Professions_WotlK.States;
using wManager.Wow.Bot.States;
using wManager.Wow.Helpers;

namespace Wholesome_Professions_WotlK.Bot
{
    internal static class Bot
    {
        private static readonly Engine Fsm = new Engine();

        internal static bool Pulse()
        {
            try
            {
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

                Fsm.AddState(new SellItemsState { Priority = 11 });

                Fsm.AddState(new SetCurrentStepState { Priority = 10 });
                Fsm.AddState(new TravelState { Priority = 9 });
                Fsm.AddState(new LearnProfessionState { Priority = 8 });
                Fsm.AddState(new BuyAndLearnRecipeState { Priority = 7 });
                Fsm.AddState(new LearnRecipeFromTrainerState { Priority = 6 });
                Fsm.AddState(new BuyMaterialsState { Priority = 5 });
                Fsm.AddState(new CraftState { Priority = 4 });

                Fsm.AddState(new Grinding { Priority = 2 });
                Fsm.AddState(new MovementLoop { Priority = 1 });

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
            }
            catch (Exception e)
            {
                Logging.WriteError("Bot > Bot  > Dispose(): " + e);
            }
        }
        
        private static void OnLevelUp()
        {
            Logging.Write("Level UP! Reload Fight Class.");
            // Update spell list
            SpellManager.UpdateSpellBook();

            // Load CC:
            CustomClass.ResetCustomClass();
        }
    }
}