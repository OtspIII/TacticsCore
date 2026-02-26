using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction: ActionScript
{
    public AttackAction(ActorThing a)
    {
        Type = Actions.GenericAttack;
        Who = a;
        Cost = ActionCost.Major;
        Phases.Add(new ActionPhase(this,1,1,TargetType.Character));
    }

    public override void OnExecute(ActionPhase p, ActionInfo i)
    {
        God.GM.AddCut(new AttackCut(Who,i.GetTile()));
        foreach (GameTile t in i.Tiles)
        {
            t.TakeEvent(God.E(EventTypes.Damage));
        }
    }

    public override void AISelect()
    {
        base.AISelect();
        GameTile t = God.GM.AllTiles.Random().Info;
        if (t.Contents == null)
        {
            Info.Tiles.Add(t);
        }
        Execute(Phase,Info);
    }
}