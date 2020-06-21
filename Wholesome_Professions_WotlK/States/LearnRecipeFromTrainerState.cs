using System.Collections.Generic;
using System.Threading;
using robotManager.FiniteStateMachine;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class LearnRecipeFromTrainerState : State
    {
        public override string DisplayName
        {
            get { return "Learning recipe from trainer"; }
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
                    || Conditions.IsAttackedAndCannotIgnore)
                    return false;

                if (Main.primaryProfession.ShouldLearnRecipeFromTrainer())
                {
                    profession = Main.primaryProfession;
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
            Logger.LogDebug("************ RUNNING LEARN RECIPE FROM TRAINER ************");
            Broadcaster.autoBroadcast = false;

            Step currentStep = profession.CurrentStep;
            Npc trainer = profession.ProfessionTrainer;

            Logger.Log($"Learning {currentStep.ItemoCraft.Name} at NPC {trainer.Entry}");

            // Check if continent ok
            if ((ContinentId)Usefuls.ContinentId != trainer.ContinentId)
            {
                Logger.Log($"The trainer is on continent {trainer.ContinentId}, launching traveler");
                Bot.SetContinent(trainer.ContinentId);
                return;
            }

            if (GoToTask.ToPositionAndIntecractWithNpc(trainer.Position, trainer.Entry, trainer.GossipOption))
            {
                ToolBox.LearnthisSpell(currentStep.ItemoCraft.Name);
                Thread.Sleep(1000);
            }

            currentStep.KnownRecipe = ToolBox.RecipeIsKnown(currentStep.ItemoCraft.Name, profession);

            Broadcaster.autoBroadcast = true;
        }
    }
}
