using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EnemyTurnPhase : PhaseScript
{
    public List<ActorThing> Queued = new List<ActorThing>();
    
    public EnemyTurnPhase()
    {
        Type = Phases.EnemyTurn;
    }

    public override void Begin()
    {
        foreach (ActorThing a in God.GM.GetActors())
        {
            if (!a.Has(Traits.Player)) Queued.Add(a);
        }
    }

    public override void OnRun()
    {
        if (Queued.Count == 0)
        {
            God.GM.StartPhase();
            return;
        }
        ActorThing a = Queued.Random();
        Queued.Remove(a);
        ActionScript act = a.MoveAction;
        act.AISelect();
    }

    public override PhaseScript NextPhase()
    {
        return new EnvironmentPhase();
    }
}