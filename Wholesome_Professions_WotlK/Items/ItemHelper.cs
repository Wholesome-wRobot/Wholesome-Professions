using System.Collections.Generic;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Helpers;

namespace Wholesome_Professions_WotlK.Items
{
    public static class ItemHelper
    {
        private class ItemInVirtualBag
        {
            public uint id;
            public int amount;
        }
        private static List<ItemInVirtualBag> virtualBag = new List<ItemInVirtualBag>();
        
        // Search if a base item should be farmed (ex : Linen Cloth)
        public static bool NeedToFarmItemFor(Item itemToCraft, IProfession profession)
        {

            foreach (Item.Mat mat in itemToCraft.Materials)
            {
                int amountOfItemsToFarm = GetTotalNeededMat(mat.item, profession);
                if (mat.item.canBeFarmed && amountOfItemsToFarm > 0)
                {
                    profession.ItemToFarm = mat.item;
                    profession.AmountOfItemToFarm = amountOfItemsToFarm;
                    Logger.LogDebug($"Found item that needs to be farmed for {itemToCraft.name} : {profession.AmountOfItemToFarm} {mat.item.name}");
                    return true;
                }
                //Recursion
                return NeedToFarmItemFor(mat.item, profession);
            }
            profession.ItemToFarm = null;
            profession.AmountOfItemToFarm = 0;
            return false;
        }

        // Get the total amount of a specific needed mat in all steps
        public static int GetTotalNeededMat(Item itemToSearch, IProfession profession)
        {
            int amount = 0;
            foreach (Step s in profession.AllSteps)
            {
                int pickFromVirtualBag = 0;

                // If it's current step and it's a level step, make sure we mitigate to match amount goal
                if (s == Main.currentProfession.CurrentStep && s.stepType == Step.StepType.CraftToLevel && s.estimatedAmountOfCrafts != 0)
                    s.estimatedAmountOfCrafts = s.GetRemainingProfessionLevels();
                else if (s == Main.currentProfession.CurrentStep && s.stepType == Step.StepType.CraftAll)
                    pickFromVirtualBag = PickFromVirtualBag(s.itemoCraft, s.estimatedAmountOfCrafts);

                if (ToolBox.GetProfessionLevel(profession.ProfessionName) < s.levelToReach)
                    amount += GetMaterialAmountInItem(s.itemoCraft, itemToSearch, s.estimatedAmountOfCrafts - pickFromVirtualBag);
            }
            virtualBag.Clear();
            return amount;
        }

        // Return the amount of the specified material needed to craft a set number of items
        public static int GetMaterialAmountInItem(Item item, Item itemToSearch, int amountToCraft)
        {
            int amount = 0;
            foreach (Item.Mat mat in item.Materials)
            {
                int amountInVirtualBag = PickFromVirtualBag(mat.item, mat.amount * amountToCraft);
                if (mat.item == itemToSearch)
                {
                    Logger.LogDebug($"Material {itemToSearch.name} : {mat.amount} * {amountToCraft} found in Item {item.name} : " +
                        $"{mat.amount * amountToCraft - amountInVirtualBag} (+ {amountInVirtualBag} in bag)");
                    amount += mat.amount * amountToCraft - amountInVirtualBag;
                }

                // Recursion
                amount += GetMaterialAmountInItem(mat.item, itemToSearch, amountToCraft * mat.amount - amountInVirtualBag);
            }
            return amount;
        }

        // Use selected amount of specified item in virtual bag (used for recursive search)
        public static int PickFromVirtualBag(Item item, int amountNeeded)
        {
            //Logger.Log($"Checking {item.name} (we have {ItemsManager.GetItemCountById(item.itemId)})");
            // If I have the item in my bags and it's not already added to virtual bag, add it
            int itemAlreadyInBags = ItemsManager.GetItemCountById(item.itemId);
            if (itemAlreadyInBags > 0)
            {
                ItemInVirtualBag itemInBag = new ItemInVirtualBag();

                if (!virtualBag.Exists(i => i.id == item.itemId))
                {
                    itemInBag.id = item.itemId;
                    itemInBag.amount = itemAlreadyInBags;
                    //Logger.Log($"Adding {itemInBag.amount} {item.name} in virtual bag");
                    virtualBag.Add(itemInBag);
                }
                else
                {
                    itemInBag = virtualBag.Find(i => i.id == item.itemId);
                    //Logger.Log($"{itemInBag.amount} {item.name} already in virtual bag");
                }

                //Logger.Log($"We need {amountNeeded} {item.name}");
                // if I have more items than needed
                if (itemInBag.amount >= amountNeeded)
                {
                    //Logger.Log($"Using {amountNeeded} {item.name} from virtual bag");
                    itemInBag.amount = itemInBag.amount - amountNeeded;
                    //Logger.Log($"{itemInBag.amount} {item.name} left in virtual bag");
                    return amountNeeded;
                }
                // if we have less items than needed, use remaining
                else
                {
                    //Logger.Log($"Using remaining {itemInBag.amount} {item.name} from virtual bag");
                    int amountInBag = itemInBag.amount;
                    itemInBag.amount = 0;
                    //Logger.Log($"{itemInBag.amount} {item.name} left in virtual bag");
                    return amountInBag;
                }
            }

            return 0;
        }
    }
}
