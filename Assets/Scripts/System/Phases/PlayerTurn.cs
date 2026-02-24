using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerTurnPhase : PhaseScript
{
    public List<ActorThing> Players = new List<ActorThing>();
    
    public PlayerTurnPhase()
    {
        Type = Phases.PlayerTurn;
    }

    public override void Begin()
    {
        foreach (ActorThing a in God.GM.GetActors())
        {
            if (a.Has(Traits.Player)) Players.Add(a);
        }
        Debug.Log("PLAYER TURN: " + Players.Count);
    }

    public override void OnRun()
    {
        if(Players.Count == 0) God.GM.StartPhase();
    }

    public override void TileClick(TileThing t)
    {
        if (Players.Count == 0 || t.Contents != null) return;
        Players[0].Walk(t);
        Players.RemoveAt(0);
    }

    public override PhaseScript NextPhase()
    {
        return new EnemyTurnPhase();
    }
}