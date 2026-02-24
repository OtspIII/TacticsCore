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

    public override void OnExecute(ActionPhase p, ActionInfo i)
    {
        foreach (TileThing t in i.Tiles)
        {
            Who.Walk(t);
        }
    }

    public override void AISelect()
    {
        base.AISelect();
        TileThing t = God.GM.AllTiles.Random().Info;
        if (t.Contents == null)
        {
            Info.Tiles.Add(t);
        }
        Execute(Phase,Info);
    }
}
