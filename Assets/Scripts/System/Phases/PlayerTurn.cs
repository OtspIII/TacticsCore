using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerTurnPhase : PhaseScript
{
    public List<ActorThing> Players = new List<ActorThing>();
    public ActorThing Selected;
    public ActionScript Action;
    
    public PlayerTurnPhase()
    {
        Type = Phases.PlayerTurn;
    }

    public override void Begin()
    {
        foreach (ActorThing a in God.GM.GetActors())
        {
            if (a.Has(Traits.Player))
            {
                Players.Add(a);
                a.TakeEvent(EventTypes.StartTurn);
            }
        }
        Debug.Log("PLAYER TURN: " + Players.Count);
    }

    public override void OnRun()
    {
        if(Players.Count == 0) God.GM.StartPhase();
    }

    public override void TileClick(TileThing t)
    {
        if (t.Contents != null)
        {
            if (Players.Contains(t.Contents))
            {
                if(Selected != null)
                    Selected.Location.WipeTint();
                Selected = t.Contents;
                if (Selected.ActionsLeft.Contains(ActionCost.Move))
                {
                    Action = Selected.MoveAction;
                    Action.Begin();
                }
                t.SetTint(Color.cornflowerBlue);
            }
            return;
        }
        if (Selected == null) return;
        Debug.Log("ACT: " + Action?.Type);
        if (Action != null && Action.TileClick(t))
        {
            Selected.Location.WipeTint();
            if (Selected.ActionsLeft.Count == 0) Players.Remove(Selected);
            Selected = null;
            Action = null;
        }
    }

    public override PhaseScript NextPhase()
    {
        return new EnemyTurnPhase();
    }
}