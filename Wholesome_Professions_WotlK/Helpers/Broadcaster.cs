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
            PreCheckBroadcast();
            if (!IsBroadcastEmpty() && (timerReady || forceBroadcast))
            {
                Logger.LogLineBroadcast("********** BROADCAST ************", Color.DimGray);

                Logger.LogLineBroadcast(professionNameStringPrim.ToUpper(), Color.Brown);
                Logger.LogLineBroadcast(currentStepStringPrim, Color.Brown);
                Logger.LogLineBroadcastImportant(requiredLevelStringPrim);
                Logger.LogLineBroadcastImportant(farmsNeededStringPrim);

                Logger.LogLineBroadcast(professionNameStringSecond.ToUpper(), Color.Peru);
                Logger.LogLineBroadcast(currentStepStringSecond, Color.Peru);
                Logger.LogLineBroadcastImportant(requiredLevelStringSecond);
                Logger.LogLineBroadcastImportant(farmsNeededStringSecond);

                Logger.LogLineBroadcast("*************************************", Color.DimGray);

                FrameHelper.ClearBroadcastString();
                FrameHelper.UpdateBroadcastFrame("Broadcasttitle", "**** WHOLESOME PROFESSIONS ****");
                FrameHelper.UpdateBroadcastFrame("professionNameStringPrim", "\\r" + professionNameStringPrim.ToUpper());
                FrameHelper.UpdateBroadcastFrame("currentStepStringPrim", currentStepStringPrim);
                FrameHelper.UpdateBroadcastFrame("requiredLevelStringPrim", requiredLevelStringPrim);
                FrameHelper.UpdateBroadcastFrame("farmsNeededStringPrim", farmsNeededStringPrim);
                FrameHelper.UpdateBroadcastFrame("professionNameStringSecond", "\\r" + professionNameStringSecond.ToUpper());
                FrameHelper.UpdateBroadcastFrame("currentStepStringSecond", currentStepStringSecond);
                FrameHelper.UpdateBroadcastFrame("requiredLevelStringSecond", requiredLevelStringSecond);
                FrameHelper.UpdateBroadcastFrame("farmsNeededStringSecond", farmsNeededStringSecond);
                FrameHelper.UpdateBroadcastFrame("BotStatus", "\\rCurrent Status : " + Logging.Status);
                FrameHelper.UpdateBroadcastFrame("CurrentProfile", "Current profile : " + Bot.ProfileName);

                ResetTimer();
            }
        }

        private static void PreCheckBroadcast()
        {
            IProfession prof1 = Main.primaryProfession;
            IProfession prof2 = Main.secondaryProfession;

            if (prof1 != null)
            {
                // Primary profession
                // profession name
                professionNameStringPrim = prof1.Name.ToString();

                if (prof1.CurrentStep != null)
                {
                    // Farms needed
                    if (prof1.ItemToFarm != null && prof1.AmountOfItemToFarm > 0)
                        farmsNeededStringPrim = $"You need {prof1.AmountOfItemToFarm} more {prof1.ItemToFarm.Name} to proceed";
                    else
                        farmsNeededStringPrim = null;

                    // Current step (craft all)
                    if (prof1.CurrentStep.Type == Step.StepType.CraftAll)
                        currentStepStringPrim = $"Craft all {prof1.CurrentStep.ItemoCraft.Name} x {prof1.CurrentStep.EstimatedAmountOfCrafts}";
                    else if (prof1.CurrentStep.Type == Step.StepType.CraftToLevel)
                        currentStepStringPrim = $"Craft {prof1.CurrentStep.ItemoCraft.Name} until lvl {prof1.CurrentStep.LevelToReach}";
                    else
                        currentStepStringPrim = null;

                    // Minimum level required not met
                    if (!Main.primaryProfession.MyLevelIsHighEnough())
                        requiredLevelStringPrim = $"You must be level {prof1.MinimumCharLevel} to progress";
                    else
                        requiredLevelStringPrim = null;
                }
                else
                {
                    currentStepStringPrim = "No progression is currently possible";
                }
            }

            if (prof2 != null)
            {
                // Secondary profession
                // Profession name
                professionNameStringSecond = prof2.Name.ToString();

                if (prof2.CurrentStep != null)
                {
                    // Farms needed
                    if (prof2.ItemToFarm != null && prof2.AmountOfItemToFarm > 0)
                        farmsNeededStringSecond = $"{prof2.AmountOfItemToFarm} more {prof2.ItemToFarm.Name} to proceed";
                    else
                        farmsNeededStringSecond = null;

                    // Current step (craft all)
                    if (prof2.CurrentStep.Type == Step.StepType.CraftAll)
                        currentStepStringSecond = $"Craft all {prof2.CurrentStep.ItemoCraft.Name} x {prof2.CurrentStep.EstimatedAmountOfCrafts}";
                    else if (prof2.CurrentStep.Type == Step.StepType.CraftToLevel)
                        currentStepStringSecond = $"Craft {prof2.CurrentStep.ItemoCraft.Name} until lvl {prof2.CurrentStep.LevelToReach}";
                    else
                        currentStepStringSecond = null;

                    // Minimum level required not met
                    if (!Main.primaryProfession.MyLevelIsHighEnough())
                        requiredLevelStringSecond = $"You must be at least level {prof2.MinimumCharLevel} to progress";
                    else
                        requiredLevelStringSecond = null;
                }
                else
                {
                    currentStepStringSecond = "No progression is currently possible";
                }
            }
        }
    }
}
