using System.Collections.Generic;
using System.Threading;
using robotManager.FiniteStateMachine;
using Wholesome_Professions_WotlK.Helpers;
using wManager;
using wManager.Wow.Bot.Tasks;
using wManager.Wow.Class;
using wManager.Wow.Enums;
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
        private IProfession profession;

        public override bool NeedToRun
        {
            get
            {
                if (!Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause || !ObjectManager.Me.IsValid
                    || Conditions.IsAttackedAndCannotIgnore)
                    return false;
                
                if (Main.primaryProfession.ShouldBuyMaterials())
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
            Logger.LogDebug("************ RUNNING BUY MATERIALS STATE ************");
            Broadcaster.autoBroadcast = false;

            Step currentStep = profession.CurrentStep;
            foreach (Item.Mat mat in currentStep.ItemoCraft.Materials)
            {
                int amountMissing = currentStep.GetAmountMissingMaterial(mat);
                if (mat.Item.CanBeBought)
                {
                    Npc vendor = mat.Item.Vendor ?? profession.SuppliesVendor;
                    Logger.Log($"Buying {amountMissing} {mat.Item.Name} from NPC {vendor.Entry}");

                    // Check if continent ok
                    if ((ContinentId)Usefuls.ContinentId != vendor.ContinentId)
                    {
                        Logger.Log($"The vendor is on continent {vendor.ContinentId}, launching traveler");
                        Bot.SetContinent(vendor.ContinentId);
                        return;
                    }

                    int estimatedPrice = mat.Item.EstimatedPrice * mat.Amount * amountMissing;
                    Logger.Log($"Estimated price : {estimatedPrice}");
                    if (ObjectManager.Me.GetMoneyCopper >= mat.Amount * amountMissing)
                    {
                        if (GoToTask.ToPositionAndIntecractWithNpc(vendor.Position, vendor.Entry, vendor.GossipOption))
                        {
                            //Vendor.SellItems(wManagerSetting.CurrentSetting.ForceSellList, wManagerSetting.CurrentSetting.DoNotSellList, ToolBox.vendorQuality);

                            int amountToHaveInBag = amountMissing + ItemsManager.GetItemCountById(mat.Item.ItemId);
                            while (ItemsManager.GetItemCountById(mat.Item.ItemId) < amountToHaveInBag && Bag.GetContainerNumFreeSlots > 1)
                            {
                                Logger.LogDebug($"Buying {mat.Item.Name}");
                                Vendor.BuyItem(ItemsManager.GetNameById(mat.Item.ItemId), amountMissing);
                                Thread.Sleep(200);
                            }
                        }
                    }
                    else
                        Logger.Log($"You don't have enough money to buy {mat.Amount * amountMissing} x {mat.Item.Name} ({estimatedPrice} Copper).");
                }
            }

            Broadcaster.autoBroadcast = true;
        }
    }
}
