using System.Collections.Generic;
using UnityEngine;

public class PlayerTrait : TraitThing
{
    public PlayerTrait()
    {
        Type = Traits.Player;
        AddListen(EventTypes.StartTurn);
    }

    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.StartTurn:
            {
                i.Who.ActionsLeft.Clear();
                i.Who.ActionsLeft.Add(ActionCost.Major);
                i.Who.ActionsLeft.Add(ActionCost.Bonus);
                i.Who.ActionsLeft.Add(ActionCost.Move);
                break;
            }
        }
    }
}

public class MobileTrait : TraitThing
{
    public MobileTrait()
    {
        Type = Traits.Mobile;
        AddListen(EventTypes.Setup);
    }
    
    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.Setup:
            {
                i.Who.MoveAction = new WalkAction(i.Who);
                break;
            }
        }
    }
}

public class HealthTrait : TraitThing
{
    public HealthTrait()
    {
        Type = Traits.Health;
        AddListen(EventTypes.Damage);
        AddListen(EventTypes.Death);
    }
    
    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.Damage:
            {
                God.GM.AddCut(new HeadtextCut(i.Who,"Ouch"));
                i.Who.TakeEvent(EventTypes.Death);
                break;
            }
            case EventTypes.Death:
            {
                i.Who.Destruct();
                break;
            }
        }
    }
}