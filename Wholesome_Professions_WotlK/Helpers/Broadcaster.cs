using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.Helpers
{
    public static class Broadcaster
    {
        public static bool autoBroadcast = true;
        public static string currentStepString = null;
        public static string requiredLevelString = null;
        public static string farmsNeededString = null;
        public static string professionNameString = null;

        public static int timerInterval;
        public static Timer broadcastTimer = new Timer();
        public static bool timerReady;

        public static void InitializeTimer()
        {
            timerReady = false;
            ClearBroadCastMessages();
            timerInterval = WholesomeProfessionsSettings.CurrentSetting.BroadcasterInterval * 1000;
            broadcastTimer = new Timer(timerInterval);
            broadcastTimer.AutoReset = false;
            broadcastTimer.Elapsed += new ElapsedEventHandler(SetTimerReady);
            broadcastTimer.Start();
        }

        // Method attached to the timer ready event
        public static void SetTimerReady(object sender, EventArgs e)
        {
            timerReady = true;
        }

        public static void ClearBroadCastMessages()
        {
            currentStepString = null;
            requiredLevelString = null;
            farmsNeededString = null;
            professionNameString = null;
        }

        private static bool IsBroadcastEmpty()
        {
            return currentStepString == null && requiredLevelString == null
                && farmsNeededString == null && professionNameString == null;
        }

        private static void ResetTimer()
        {
            timerReady = false;
            broadcastTimer.Stop();
            broadcastTimer.Start();
        }

        public static void BroadCastSituation(bool forceBroadcast = false)
        {
            PreCheckBroadcast();
            if (!IsBroadcastEmpty() && (timerReady || forceBroadcast))
            {
                Logger.LogLineBroadcast("********** BROADCAST ************");

                Logger.LogLineBroadcast(professionNameString.ToUpper());
                Logger.LogLineBroadcast(currentStepString);
                Logger.LogLineBroadcastImportant(requiredLevelString);
                Logger.LogLineBroadcastImportant(farmsNeededString);

                Logger.LogLineBroadcast("*************************************");
                ResetTimer();
            }
        }

        private static void PreCheckBroadcast()
        {
            IProfession profession = Main.primaryProfession;
            if (profession != null)
            {
                // profession name
                professionNameString = profession.ProfessionName.ToString();

                if (profession.CurrentStep != null)
                {
                    // Farms needed
                    if (profession.ItemToFarm != null && profession.AmountOfItemToFarm > 0)
                        farmsNeededString = $"You need {profession.AmountOfItemToFarm} more {profession.ItemToFarm.name} to proceed";
                    else
                        farmsNeededString = null;

                    // Current step (craft all)
                    if (profession.CurrentStep.stepType == Step.StepType.CraftAll)
                        currentStepString = $"STEP : Craft all {profession.CurrentStep.itemoCraft.name} x {profession.CurrentStep.estimatedAmountOfCrafts}";
                    else if (profession.CurrentStep.stepType == Step.StepType.CraftToLevel)
                        currentStepString = $"STEP : Craft {profession.CurrentStep.itemoCraft.name} to reach level {profession.CurrentStep.levelToReach}";
                    else
                        currentStepString = null;

                    // Minimum level required not met
                    if (!Main.primaryProfession.MyLevelIsHighEnough())
                        requiredLevelString = $"You must be at least level {profession.MinimumCharLevel} to progress";
                    else
                        requiredLevelString = null;
                }
                else
                {
                    currentStepString = "No progression is currently possible";
                }
            }
            else
                professionNameString = null;
        }
    }
}
