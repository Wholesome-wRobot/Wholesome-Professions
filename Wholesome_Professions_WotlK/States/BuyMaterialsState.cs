using System.Collections.Generic;
using System.Threading;
using robotManager.FiniteStateMachine;
using Wholesome_Professions_WotlK.Helpers;
using wManager;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

namespace Wholesome_Professions_WotlK.States
{
    class BuyMaterialsState : State
    {
        public override string DisplayName
        {
            get { return "Buying materials"; }
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

                if (Main.currentProfession.ShouldBuyMaterials())
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
            Logger.LogDebug("************ RUNNING BUY MATERIALS STATE ************");
            Broadcaster.autoBroadcast = false;

            Step currentStep = Main.currentProfession.CurrentStep;
            foreach (Item.Mat mat in currentStep.itemoCraft.Materials)
            {
                int amountMissing = currentStep.GetAmountMissingMaterial(mat);
                if (mat.item.canBeBought)
                {
                    Npc vendor = mat.item.vendor ?? Main.currentProfession.SuppliesVendor;
                    Logger.Log($"Buying {amountMissing} {mat.item.name} from NPC {vendor.Entry}");
                    int estimatedPrice = mat.item.estimatedPrice * mat.amount * amountMissing;
                    Logger.LogDebug($"Estimated price : {estimatedPrice}");
                    if (ObjectManager.Me.GetMoneyCopper >= mat.amount * amountMissing)
                    {
                        if (GoToTask.ToPositionAndIntecractWithNpc(vendor.Position, vendor.Entry, vendor.GossipOption))
                        {
                            Vendor.SellItems(wManagerSetting.CurrentSetting.ForceSellList, wManagerSetting.CurrentSetting.DoNotSellList, ToolBox.vendorQuality);

                            int amountToHaveInBag = amountMissing + ItemsManager.GetItemCountById(mat.item.itemId);
                            while (ItemsManager.GetItemCountById(mat.item.itemId) < amountToHaveInBag && Bag.GetContainerNumFreeSlots > 1)
                            {
                                Logger.LogDebug($"Buying {mat.item.name}");
                                Vendor.BuyItem(ItemsManager.GetNameById(mat.item.itemId), amountMissing);
                                Thread.Sleep(200);
                            }
                        }
                    }
                    else
                        Logger.Log($"You don't have enough money to buy {mat.amount * amountMissing} x {mat.item.name} ({estimatedPrice} Copper).");
                }
            }

            Broadcaster.autoBroadcast = true;
        }
    }
}
