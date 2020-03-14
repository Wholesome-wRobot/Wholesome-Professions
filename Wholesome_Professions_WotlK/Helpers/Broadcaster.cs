using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static void BroadCastSituation()
        {
            PreCheckBroadcast();
            Logger.LogLineBroadcast("********** BROADCAST ************");

            Logger.LogLineBroadcast(professionNameString);
            Logger.LogLineBroadcast(currentStepString);
            Logger.LogLineBroadcast(requiredLevelString);
            Logger.LogLineBroadcast(farmsNeededString);

            Logger.LogLineBroadcast("*************************************");
        }

        private static void PreCheckBroadcast()
        {
            IProfession profession = Main.currentProfession;
            if (profession != null)
            {
                // profession name
                professionNameString = profession.ProfessionName.ToString();

                if (profession.CurrentStep != null)
                {
                    // Farms needed
                    if (profession.ItemToFarm != null && profession.AmountOfItemToFarm > 0)
                        farmsNeededString = $"You need {profession.AmountOfItemToFarm} more {profession.ItemToFarm.name} in your bags to proceed";
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
                    if (!Main.currentProfession.MyLevelIsHighEnough())
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

        /*
        public static void ClearBroadCastMessages()
        {
            if (broadcastMessages != null)
                broadcastMessages.Clear();
        }*/
    }
}
