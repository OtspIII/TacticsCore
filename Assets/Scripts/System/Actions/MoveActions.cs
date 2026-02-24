using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAction: ActionScript
{
    public WalkAction(ActorThing a)
    {
        Type = Actions.Walk;
        Who = a;
        Cost = ActionCost.Major;
        Phases.Add(new ActionPhase(this,1,2));
    }

    public override void Execute(ActionPhase p, ActionInfo i)
    {
        base.Execute(p,i);
        Debug.Log("EXECUTE WALK");
        foreach (TileThing t in i.Tiles)
        {
            Who.Walk(t);
        }
    }
}
