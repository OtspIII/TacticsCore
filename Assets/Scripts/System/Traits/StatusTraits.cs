using System.Collections.Generic;
using UnityEngine;

public class StunTrait : TraitThing
{
    public StunTrait()
    {
        Type = Traits.Stunned;
        AddListen(EventTypes.StartTurn,5);
        AddListen(EventTypes.CanAct);
    }

    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.StartTurn:
            {
                i.Who.ActionsLeft.Clear();
                break;
            }
            case EventTypes.CanAct:
            {
                e.Set(false);
                break;
            }
        }
    }
}