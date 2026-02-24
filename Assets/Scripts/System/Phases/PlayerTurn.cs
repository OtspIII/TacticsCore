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
            if (a.IsPlayer()) Players.Add(a);
        }
        Debug.Log("PLAYER TURN: " + Players.Count);
    }

    public override void OnRun()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (ActorThing a in Players)
            {
                TileThing t = a.Location;
                TileThing nt = t.Neighbor(0, -1);
                if (nt == null) continue;
                a.SetLocation(nt);
                God.GM.AddCut(new MoveCut(a,t,nt));
                Debug.Log("MOVE: " + a);
            }
        }
    }
    
    public override PhaseScript NextPhase()
    {
        return new PlayerTurnPhase();
    }
}
