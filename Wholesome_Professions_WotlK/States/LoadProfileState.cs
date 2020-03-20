using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class LoadProfileState : State
    {
        public override string DisplayName
        {
            get { return "Starting profile"; }
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
                    || Conditions.IsAttackedAndCannotIgnore || Main.currentProfession == null || !WholesomeProfessionsSettings.CurrentSetting.Autofarm)
                    return false;

                if (Main.currentProfession.ShouldSelectProfile())
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
            Logger.LogDebug("************ RUNNING LOAD PROFILE STATE ************");
            Step currentStep = Main.currentProfession.CurrentStep;

            string faction = ToolBox.IsHorde() ? "Horde" : "Alliance";

            string profileName = $"{faction} - {Main.currentProfession.ItemToFarm.name}.xml";

            Logger.Log($"Loading profile {profileName}");
            Main.currentProfession.CurrentProfile = profileName;

            ProfileHandler.LoadNewProfile();
        }
    }
}
