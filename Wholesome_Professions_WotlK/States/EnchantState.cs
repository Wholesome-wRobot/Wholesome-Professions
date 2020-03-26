using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class EnchantState : State
    {
        public override string DisplayName
        {
            get { return "Enchanting"; }
        }

        public override int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        private int _priority;
        private IProfession profession;

        public override bool NeedToRun
        {
            get
            {
                if (!Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause || !ObjectManager.Me.IsValid
                    || Conditions.IsAttackedAndCannotIgnore || Main.amountProfessionsSelected <= 0)
                    return false;
                
                if (Main.primaryProfession.ShouldEnchant())
                {
                    profession = Main.primaryProfession;
                    return true;
                }
                if (Main.secondaryProfession.ShouldEnchant())
                {
                    profession = Main.secondaryProfession;
                    return true;
                }

                return false;
            }
        }

        public override List<State> NextStates
        {
            get { return new List<State>(); }
        }

        public override List<State> BeforeStates
        {
            get { return new List<State>(); }
        }

        public override void Run()
        {
            Logger.LogDebug("************ RUNNING ENCHANT STATE ************");
            Step currentStep = profession.CurrentStep;

            MovementManager.StopMoveNewThread();

            // Deactivate broadcast
            Broadcaster.autoBroadcast = false;

            //Debugger.Launch();
            //Debugger.Break();

            // amountAlreadyCrafted NOT USED ANYMORE ?????
            //int amountAlreadyCrafted = ToolBox.GetAlreadyCrafted(profession.ProfessionName.ToString(), currentStep.itemoCraft.name);
            //Logger.Log($"We've already crafted {amountAlreadyCrafted} {currentStep.itemoCraft.name}");

            // Craft (has to be craft to level)
            int amountEnchant = currentStep.GetRemainingProfessionLevels();
            Logger.Log($"Enchanting x {amountEnchant}");

            for (int i = 0; i < amountEnchant; i++)
            {
                ToolBox.Craft(profession.Name.ToString(), currentStep.ItemoCraft, 1);
                Thread.Sleep(100);
            }

            Logger.Log("Enchant complete");
            ToolBox.CloseProfessionFrame();
            Lua.RunMacroText("/stopcasting");

            profession.RegenerateSteps();

            Broadcaster.autoBroadcast = true;
        }
    }
}
