using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class CraftState : State
    {
        public override string DisplayName
        {
            get { return "Craft"; }
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
                    && Main.currentProfession.ShouldCraft()
                    && ObjectManager.Me.Level >= Main.currentProfession.MinimumCharLevel)
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
            Logger.LogDebug("************ RUNNING CRAFT STATE ************");
            Step currentStep = Main.currentProfession.CurrentStep;

            MovementManager.StopMoveNewThread();

            // Deactivate broadcast
            Broadcaster.BroadCastSituation();
            Broadcaster.autoBroadcast = false;

            // If we must travel // rework
            if (Main.currentProfession.Continent != Usefuls.ContinentId)
                return;

            //Debugger.Launch();
            //Debugger.Break();
            // Sell items if we need bag space
            if (Bag.GetContainerNumFreeSlots <= 1)
            {
                Logger.Log("Selling items to make space in bags, please wait");
                wManager.wManagerSetting.CurrentSetting.Selling = true;
                wManager.Wow.Bot.States.ToTown.ForceToTown = true;
                return;
            }

            // Learn Profession
            if (ToolBox.GetProfessionLevel(Main.currentProfession.ProfessionName) == ToolBox.GetProfessionMaxLevel(Main.currentProfession.ProfessionName))
            {
                Logger.Log($"Learning {Main.currentProfession.ProfessionSpell} at NPC {Main.currentProfession.ProfessionTrainer.Entry}");
                if (GoToTask.ToPositionAndIntecractWithNpc(Main.currentProfession.ProfessionTrainer.Position, Main.currentProfession.ProfessionTrainer.Entry))
                {
                    Usefuls.SelectGossipOption(Main.currentProfession.ProfessionTrainer.GossipOption);
                    Thread.Sleep(200);
                    ToolBox.LearnthisSpell(Main.currentProfession.ProfessionSpell);
                    Thread.Sleep(1000);
                }
            }

            // Buy and Learn Recipe
            var RecipeVendor = currentStep.itemoCraft.RecipeVendor;
            if (!ToolBox.RecipeIsKnown(currentStep.itemoCraft.name, Main.currentProfession.ProfessionName.ToString()) && RecipeVendor != null)
            {
                Logger.Log($"Buying {currentStep.itemoCraft.name} recipe at NPC {Main.currentProfession.ProfessionTrainer.Entry}");
                if (GoToTask.ToPositionAndIntecractWithNpc(RecipeVendor.Position, RecipeVendor.Entry))
                {
                    Usefuls.SelectGossipOption(Main.currentProfession.ProfessionTrainer.GossipOption);
                    Thread.Sleep(200);
                    Vendor.BuyItem(ItemsManager.GetNameById(currentStep.itemoCraft.RecipeItemId), 1);
                    Thread.Sleep(2000);
                    ItemsManager.UseItemByNameOrId(currentStep.itemoCraft.RecipeItemId.ToString());
                    Usefuls.WaitIsCasting();
                    Thread.Sleep(300);
                }
            }

            // Learn Recipe
            if (!ToolBox.RecipeIsKnown(currentStep.itemoCraft.name, Main.currentProfession.ProfessionName.ToString()) && RecipeVendor == null)
            {
                Logger.Log($"Learning {currentStep.itemoCraft.name} at NPC {Main.currentProfession.ProfessionTrainer.Entry}");
                if (GoToTask.ToPositionAndIntecractWithNpc(Main.currentProfession.ProfessionTrainer.Position, Main.currentProfession.ProfessionTrainer.Entry))
                {
                    Usefuls.SelectGossipOption(Main.currentProfession.ProfessionTrainer.GossipOption);
                    Thread.Sleep(200);
                    ToolBox.LearnthisSpell(currentStep.itemoCraft.name);
                    Thread.Sleep(1000);
                }
            }

            // Buy mats
            if (currentStep.CanBuyRemainingMats() && !currentStep.HasMatsToCraftOne())
            {
                foreach (Item.Mat mat in currentStep.itemoCraft.Materials)
                {
                    int amountMissing = currentStep.GetAmountMissingMaterial(mat);
                    if (mat.item.canBeBought)
                    {
                        Logger.Log($"Buying {amountMissing} {mat.item.name} from NPC {mat.item.vendor.Entry}");
                        int estimatedPrice = mat.item.estimatedPrice * mat.amount * amountMissing;
                        Logger.LogDebug($"Estimated price : {estimatedPrice}");
                        if (ObjectManager.Me.GetMoneyCopper >= mat.amount * amountMissing)
                        {
                            if (GoToTask.ToPositionAndIntecractWithNpc(mat.item.vendor.Position, mat.item.vendor.Entry))
                            {
                                int amountToHaveInBag = amountMissing + ItemsManager.GetItemCountById(mat.item.itemId);
                                while (ItemsManager.GetItemCountById(mat.item.itemId) < amountToHaveInBag && Bag.GetContainerNumFreeSlots > 1)
                                {
                                    Logger.LogDebug($"Buying {mat.item.name}");
                                    Usefuls.SelectGossipOption(mat.item.vendor.GossipOption);
                                    Thread.Sleep(200);
                                    Vendor.BuyItem(ItemsManager.GetNameById(mat.item.itemId), amountMissing);
                                    Thread.Sleep(200);
                                }
                            }
                        }
                        else
                            Broadcaster.ClearAndAddBroadCastMessage($"You don't have enough money to buy {mat.amount * amountMissing} x {mat.item.name} ({estimatedPrice} Copper).");
                    }
                }
            }

            // amountAlreadyCrafted NOT USED ANYMORE ?????
            //int amountAlreadyCrafted = ToolBox.GetAlreadyCrafted(Main.currentProfession.ProfessionName.ToString(), currentStep.itemoCraft.name);
            //Logger.Log($"We've already crafted {amountAlreadyCrafted} {currentStep.itemoCraft.name}");

            // Craft
            int itemInBagsBeforeCraft = ItemsManager.GetItemCountById(currentStep.itemoCraft.itemId);
            int amountToCraft = 0;

            if (currentStep.stepType == Step.StepType.CraftAll)
                amountToCraft = currentStep.estimatedAmountOfCrafts;
            else if (currentStep.stepType == Step.StepType.CraftToLevel)
                amountToCraft = currentStep.levelToReach - ToolBox.GetProfessionLevel(Main.currentProfession.ProfessionName);

            int goalAmount = amountToCraft + itemInBagsBeforeCraft;

            Logger.Log($"Crafting {amountToCraft} x {currentStep.itemoCraft.name}");
            ToolBox.Craft(Main.currentProfession.ProfessionName.ToString(), currentStep.itemoCraft, amountToCraft);
            while (ItemsManager.GetItemCountById(currentStep.itemoCraft.itemId) < goalAmount && currentStep.HasMatsToCraftOne()
                && Bag.GetContainerNumFreeSlots > 1)
            {
                // Log
                if (ItemsManager.GetItemCountById(currentStep.itemoCraft.itemId) != itemInBagsBeforeCraft)
                {
                    itemInBagsBeforeCraft = ItemsManager.GetItemCountById(currentStep.itemoCraft.itemId);
                    if (currentStep.stepType == Step.StepType.CraftAll)
                        Logger.Log($"Crafted : {currentStep.itemoCraft.name} {itemInBagsBeforeCraft}/{goalAmount}");
                    else if (currentStep.stepType == Step.StepType.CraftToLevel)
                    {
                        Logger.Log($"Craft {currentStep.itemoCraft.name} until level up : " +
                            $"{ToolBox.GetProfessionLevel(Main.currentProfession.ProfessionName)}/{currentStep.levelToReach}");
                        // record item
                        ToolBox.AddCraftedItemToSettings(Main.currentProfession.ProfessionName.ToString(), currentStep.itemoCraft);
                    }
                }
                Thread.Sleep(200);
            }

            Logger.Log("Craft complete");
            Lua.RunMacroText("/stopcasting");
            Main.currentProfession.RegenerateSteps();

            Broadcaster.autoBroadcast = true;
            //Broadcaster.BroadCastSituation();
        }
    }
}
