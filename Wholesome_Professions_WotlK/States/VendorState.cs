using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using robotManager.FiniteStateMachine;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class VendorState : State
    {
        public override string DisplayName
        {
            get { return "Vendor"; }
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
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && ObjectManager.Me.IsValid
                    && !Conditions.IsAttackedAndCannotIgnore && Main.currentProfession != null
                    && Main.currentProfession.Continent != Usefuls.ContinentId
                    && Main.currentProfession.CurrentStep != null)
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
            // ek = 0, kalimdor = 1, Outlands = 530, Northrend 571
            Logger.LogDebug("************ RUNNING TRAVEL STATE ************");
            Broadcaster.autoBroadcast = false;
            Step currentStep = Main.currentProfession.CurrentStep;



            MovementManager.StopMoveNewThread();
            Main.currentProfession.RegenerateSteps();
            Broadcaster.autoBroadcast = true;
        }
    }
}
