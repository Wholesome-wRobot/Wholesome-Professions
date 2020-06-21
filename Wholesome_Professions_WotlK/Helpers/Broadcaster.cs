using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static string professionNameStringPrim = null;
        public static string currentStepStringPrim = null;
        public static string requiredLevelStringPrim = null;
        public static string farmsNeededStringPrim = null;

        public static string professionNameStringSecond = null;
        public static string currentStepStringSecond = null;
        public static string requiredLevelStringSecond = null;
        public static string farmsNeededStringSecond = null;

        public static int timerInterval;
        public static System.Timers.Timer broadcastTimer = new System.Timers.Timer();
        public static bool timerReady;

        public static void InitializeTimer()
        {
            timerReady = false;
            ClearBroadCastMessages();
            timerInterval = WholesomeProfessionsSettings.CurrentSetting.BroadcasterInterval * 1000;
            broadcastTimer = new System.Timers.Timer(timerInterval);
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
            currentStepStringPrim = null;
            requiredLevelStringPrim = null;
            farmsNeededStringPrim = null;
            professionNameStringPrim = null;

            currentStepStringSecond = null;
            requiredLevelStringSecond = null;
            farmsNeededStringSecond = null;
            professionNameStringSecond = null;
        }

        private static bool IsBroadcastEmpty()
        {
            return currentStepStringPrim == null && requiredLevelStringPrim == null
                && farmsNeededStringPrim == null && professionNameStringPrim == null
                && currentStepStringSecond == null && requiredLevelStringSecond == null
                && farmsNeededStringSecond == null && professionNameStringSecond == null;
        }

        private static void ResetTimer()
        {
            timerReady = false;
            broadcastTimer.Stop();
            broadcastTimer.Start();
        }

        public static void BroadCastSituation(bool forceBroadcast = false)
        {
            FillStrings(Main.primaryProfession);
            FillStrings(Main.secondaryProfession);
            
            if (!IsBroadcastEmpty() && (timerReady || forceBroadcast))
            {
                Logger.LogLineBroadcast("********** BROADCAST ************", Color.DimGray);

                if (professionNameStringPrim != null)
                    Logger.LogLineBroadcast(professionNameStringPrim.ToUpper(), Color.Brown);
                Logger.LogLineBroadcast(currentStepStringPrim, Color.Brown);
                Logger.LogLineBroadcastImportant(requiredLevelStringPrim);
                Logger.LogLineBroadcastImportant(farmsNeededStringPrim);

                if (professionNameStringSecond != null)
                    Logger.LogLineBroadcast(professionNameStringSecond.ToUpper(), Color.Peru);
                Logger.LogLineBroadcast(currentStepStringSecond, Color.Peru);
                Logger.LogLineBroadcastImportant(requiredLevelStringSecond);
                Logger.LogLineBroadcastImportant(farmsNeededStringSecond);

                Logger.LogLineBroadcast("*************************************", Color.DimGray);
                
                FrameHelper.ClearBroadcastString();
                FrameHelper.UpdateBroadcastFrame("Broadcasttitle", "**** WHOLESOME PROFESSIONS ****");

                if (professionNameStringPrim != null)
                    FrameHelper.UpdateBroadcastFrame("professionNameStringPrim", "\\r" + professionNameStringPrim.ToUpper());
                FrameHelper.UpdateBroadcastFrame("currentStepStringPrim", currentStepStringPrim);
                FrameHelper.UpdateBroadcastFrame("requiredLevelStringPrim", requiredLevelStringPrim);
                FrameHelper.UpdateBroadcastFrame("farmsNeededStringPrim", farmsNeededStringPrim);

                if (professionNameStringSecond != null)
                    FrameHelper.UpdateBroadcastFrame("professionNameStringSecond", "\\r" + professionNameStringSecond.ToUpper());
                FrameHelper.UpdateBroadcastFrame("currentStepStringSecond", currentStepStringSecond);
                FrameHelper.UpdateBroadcastFrame("requiredLevelStringSecond", requiredLevelStringSecond);
                FrameHelper.UpdateBroadcastFrame("farmsNeededStringSecond", farmsNeededStringSecond);

                FrameHelper.UpdateBroadcastFrame("BotStatus", "\\rCurrent Status : " + Logging.Status);
                FrameHelper.UpdateBroadcastFrame("CurrentProfile", "Current profile : " + Bot.ProfileName);
                
                ResetTimer();
            }
        }

        private static void FillStrings(IProfession prof)
        {
            string profNameS = null;
            string farmsNeededS = null;
            string currentstepS = null;
            string requiredLevelS = null;

            if (prof != null)
            {
                profNameS = prof.Name.ToString();

                if (prof.CurrentStep != null)
                {
                    // Requires player action
                    if (prof.CurrentStep.ItemoCraft.UserMustBuyManually)
                        farmsNeededS = $"WARNING: You need tu buy {prof.CurrentStep.ItemoCraft.Name} to proceed";
                    else if (prof.ItemToFarm != null && prof.AmountOfItemToFarm > 0)
                        farmsNeededS = $"{prof.AmountOfItemToFarm} {prof.ItemToFarm.Name} required";

                    // Current step (craft all)
                    if (prof.CurrentStep.Type == Step.StepType.CraftAll)
                        currentstepS = $"Craft all {prof.CurrentStep.ItemoCraft.Name} x {prof.CurrentStep.EstimatedAmountOfCrafts}";
                    else if (prof.CurrentStep.Type == Step.StepType.CraftToLevel)
                        currentstepS = $"Craft {prof.CurrentStep.ItemoCraft.Name} until lvl {prof.CurrentStep.LevelToReach}";

                    // Minimum level required not met
                    if (!prof.MyCharLevelIsHighEnough())
                        requiredLevelS = $"You must be at least level {prof.MinimumCharLevel} to progress";

                    if (prof == Main.primaryProfession)
                    {
                        professionNameStringPrim = profNameS;
                        farmsNeededStringPrim = farmsNeededS;
                        currentStepStringPrim = currentstepS;
                        requiredLevelStringPrim = requiredLevelS;
                    }
                    else if (prof == Main.secondaryProfession)
                    {
                        professionNameStringSecond = profNameS;
                        farmsNeededStringSecond = farmsNeededS;
                        currentStepStringSecond = currentstepS;
                        requiredLevelStringSecond = requiredLevelS;
                    }
                }
                else
                    currentstepS = "No progression is currently possible";
            }
        }
    }
}
