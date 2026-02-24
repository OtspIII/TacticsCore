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
        Debug.Log("ENEMY TURN: " + Queued.Count);
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
        TileThing t = God.GM.AllTiles.Random().Info;
        if(t.Contents == null) a.Walk(t);
    }

    public override PhaseScript NextPhase()
    {
        return new EnvironmentPhase();
    }
}