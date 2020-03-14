using robotManager.FiniteStateMachine;
using robotManager.Helpful;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class LearnProfessionState : State
    {
        public override string DisplayName
        {
            get { return "Learning profession"; }
        }

        public override int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        private int _priority;

        public override bool NeedToRun
        {
            get
            {
                if (!Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause || !ObjectManager.Me.IsValid
                    || Conditions.IsAttackedAndCannotIgnore || Main.currentProfession == null || Main.currentProfession.CurrentStep == null)
                    return false;

                if (Main.currentProfession.ShouldLearnProfession())
                    return true;

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
            Logger.LogDebug("************ RUNNING LEARN PROFESSION STATE ************");
            Broadcaster.autoBroadcast = false;

            Npc trainer = Main.currentProfession.ProfessionTrainer;

            // Learn Profession
            Logger.Log($"Learning {Main.currentProfession.ProfessionSpell} at NPC {trainer.Entry}");
            if (GoToTask.ToPositionAndIntecractWithNpc(trainer.Position, trainer.Entry, trainer.GossipOption))
            {
                ToolBox.LearnthisSpell(Main.currentProfession.ProfessionSpell);
                Thread.Sleep(1000);
            }

            Broadcaster.autoBroadcast = true;
        }
    }
}
