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
        public static void CalculateFarmAmountFor(IProfession profession, Item itemToCraft)
        {
            // TIMER
            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (Item.Mat mat in itemToCraft.Materials)
            {
                if (mat.Item.CanBeFarmed)
                {
                    int amountOfItemsToFarm = GetTotalNeededMat(profession, mat.Item);
                    if (amountOfItemsToFarm > 0)
                    {
                        profession.ItemToFarm = mat.Item;
                        profession.AmountOfItemToFarm = amountOfItemsToFarm;
                        Logger.LogDebug($"Found item that needs to be farmed for {itemToCraft.Name} : {profession.AmountOfItemToFarm} {mat.Item.Name}");
                        return;
                    }
                }
                //Recursion
                CalculateFarmAmountFor(profession, mat.Item);
            }
            profession.ItemToFarm = null;
            profession.AmountOfItemToFarm = 0;

            // TIMER
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.LogLineBroadcastImportant($"CalculateFarmAmountFor() {profession.Name} - {itemToCraft} - {elapsedMs} MS");

            return;
        }

        // Get the total amount of a specific needed mat in all steps
        public static int GetTotalNeededMat(IProfession profession, Item itemToSearch)
        {
            // TIMER
            var watch = System.Diagnostics.Stopwatch.StartNew();

            int amount = 0;
            foreach (Step s in profession.AllSteps)
            {
                int pickFromVirtualBag = 0;

                // If it's the current step make sure we mitigate to match amount goal
                if (s == profession.CurrentStep && s.Type == Step.StepType.CraftToLevel && s.EstimatedAmountOfCrafts != 0)
                    s.EstimatedAmountOfCrafts = s.GetRemainingProfessionLevels();
                else if (s == profession.CurrentStep && s.Type == Step.StepType.CraftAll)
                    pickFromVirtualBag = PickFromVirtualBag(s.ItemoCraft, s.EstimatedAmountOfCrafts);

                // We search the targetted mat in the current step item or its children
                Item.Mat searchedMat = s.ItemoCraft.Materials.Find(i => i.Item.ItemId == itemToSearch.ItemId || i.Item.Materials.Exists(it => it.Item.ItemId == itemToSearch.ItemId));
                if (searchedMat.Item != null)
                {
                    if (ToolBox.GetProfessionLevel(profession.Name) < s.LevelToReach || s.ItemoCraft.IsAPrerequisiteItem)
                        amount += GetMaterialAmountInItem(s.ItemoCraft, itemToSearch, s.EstimatedAmountOfCrafts - pickFromVirtualBag);
                }
            }
            virtualBag.Clear();

            // TIMER
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.LogLineBroadcastImportant($"GetTotalNeededMat() {profession.Name} - {itemToSearch.Name} - {elapsedMs} MS");

            Logger.Log("RETURNS " + amount);
            return amount;
        }

        // Return the amount of the specified material needed to craft in one specified item
        public static int GetMaterialAmountInItem(Item item, Item materialToSearch, int amountToCraft)
        {
            // TIMER
            var watch = System.Diagnostics.Stopwatch.StartNew();

            int amount = 0;
            foreach (Item.Mat mat in item.Materials)
            {
                //Logger.Log($"Checking if mat {mat.Item.Name} is the searched material {materialToSearch.Name} to craft {amountToCraft} {item.Name}");
                int amountInVirtualBag = PickFromVirtualBag(mat.Item, mat.Amount * amountToCraft);
                if (mat.Item == materialToSearch)
                {
                    Logger.Log($"Material {materialToSearch.Name} : {mat.Amount} * {amountToCraft} found in Item {item.Name} : " +
                        $"{mat.Amount * amountToCraft - amountInVirtualBag} (+ {amountInVirtualBag} in bag)");
                    amount += mat.Amount * amountToCraft - amountInVirtualBag;
                }

                // Recursion
                amount += GetMaterialAmountInItem(mat.Item, materialToSearch, amountToCraft * mat.Amount - amountInVirtualBag);
            }

            // TIMER
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Logger.LogLineBroadcastImportant($"GetMaterialAmountInItem() {item.Name} - {materialToSearch.Name} - {elapsedMs} MS");

            return amount;
        }

        // Use selected amount of specified item in virtual bag (used for recursive search)
        public static int PickFromVirtualBag(Item item, int amountNeeded)
        {
            // TIMER
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // If I have the item in my bags and it's not already added to virtual bag, add it
            int itemAlreadyInBags = ItemsManager.GetItemCountById(item.ItemId);
            if (itemAlreadyInBags > 0)
            {
                ItemInVirtualBag itemInBag = new ItemInVirtualBag();

                if (!virtualBag.Exists(i => i.id == item.ItemId))
                {
                    itemInBag.id = item.ItemId;
                    itemInBag.amount = itemAlreadyInBags;
                    virtualBag.Add(itemInBag);
                }
                else
                {
                    itemInBag = virtualBag.Find(i => i.id == item.ItemId);
                }

                // TIMER
                watch.Stop();
                var elavcbcvbpsedMs = watch.ElapsedMilliseconds;
                Logger.LogLineBroadcastImportant($"PickFromVirtualBag() {item.Name} - {elavcbcvbpsedMs} MS");

                // if I have more items than needed
                if (itemInBag.amount >= amountNeeded)
                {
                    itemInBag.amount = itemInBag.amount - amountNeeded;
                    return amountNeeded;
                }
                else
                {
                    int amountInBag = itemInBag.amount;
                    itemInBag.amount = 0;
                    return amountInBag;
                }
            }

            return 0;
        }
    }
}
