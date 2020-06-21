using robotManager.FiniteStateMachine;
using System.Collections.Generic;
using Wholesome_Professions_WotlK.Helpers;
using Wholesome_Professions_WotlK.Items;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

class DisenchantState : State
{
    public override string DisplayName
    {
        get { return "Disenchanting"; }
    }

    public override int Priority
    {
        get { return _priority; }
        set { _priority = value; }
    }

    private int _priority;
    private IProfession profession;
    readonly Spell Disenchant = new Spell("Disenchant");

    public override bool NeedToRun
    {
        get
        {
            if (!Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause || !ObjectManager.Me.IsValid
                || Conditions.IsAttackedAndCannotIgnore || ObjectManager.Me.MountDisplayId != 0)
                return false;
                
            if (Main.primaryProfession.ShouldDisenchant())
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
        Logger.LogDebug("************ RUNNING DISENCHANT STATE ************");
        Step currentStep = profession.CurrentStep;
        List<WoWItem> bagItems = Bag.GetBagItem();
        List<WoWItem> itemsToDisenchant = profession.ItemsToDisenchant;

        MovementManager.StopMoveNewThread();

        // Deactivate broadcast
        Broadcaster.autoBroadcast = false;
        
        foreach (WoWItem item in itemsToDisenchant)
        {
            List<int> bagAndSlot = Bag.GetItemContainerBagIdAndSlot(item.Name);
            Logger.Log($"Disenchanting item {item.Name} (iLvl {item.GetItemInfo.ItemLevel})");
            Disenchant.Launch();
            if (Main.wowVersion > 1)
                Lua.LuaDoString("UseContainerItem(" + bagAndSlot[0] + "," + bagAndSlot[1] + ")");
            else
                Lua.LuaDoString("PickupContainerItem(" + bagAndSlot[0] + "," + bagAndSlot[1] + ")");
            Usefuls.WaitIsCasting();
        }

        profession.ReevaluateStep();

        Broadcaster.autoBroadcast = true;
    }
}
