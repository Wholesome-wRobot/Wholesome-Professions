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
        private IProfession profession;

        public override bool NeedToRun
        {
            get
            {
                if (!Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause || !ObjectManager.Me.IsValid
                    || Conditions.IsAttackedAndCannotIgnore || !WholesomeProfessionsSettings.CurrentSetting.Autofarm)
                    return false;
                
                if (Main.primaryProfession.ShouldLoadProfile())
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
            Logger.LogDebug("************ RUNNING LOAD PROFILE STATE ************");
            Step currentStep = profession.CurrentStep;

            string faction = ToolBox.IsHorde() ? "Horde" : "Alliance";

            // Setting profile name
            string profileName;
            if (profession.ItemToFarm.Profile != null)
                profileName = profession.ItemToFarm.Profile;
            else
                profileName = $"{faction} - {profession.ItemToFarm.Name}.xml";

            Logger.Log($"Loading profile {profileName}");
            ProfileHandler.LoadNewProfile(profession.Name.ToString(), profileName);
        }
    }
}
