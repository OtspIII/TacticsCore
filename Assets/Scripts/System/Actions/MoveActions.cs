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
        Phases.Add(new ActionPhase(this,1,2,TargetType.EmptyTile));
    }

    public override void OnExecute(ActionPhase p, ActionInfo i)
    {
        foreach (GameTile t in i.Tiles)
        {
            Who.Walk(t);
        }
    }

    public override void AISelect()
    {
        base.AISelect();
        List<GameTile> opts = Who.Location.Flood(Phase.Range,NeighborMode.Walking,Who);//God.GM.AllTiles.Random().Info;
        GameTile t = opts.Random();
        if (t != null && t.Contents == null)
        {
            Info.Tiles.Add(t);
        }
        Execute(Phase,Info);
    }

    public override NeighborMode GetNeighborMode()
    {
        return NeighborMode.Walking;
    }
}
