using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnPhase : PhaseScript
{
    public List<ActorThing> Players = new List<ActorThing>();
    public ActorThing Selected;
    
    public PlayerTurnPhase()
    {
        Type = Phases.PlayerTurn;
        Listeners.Add(EventTypes.ActionEnd);
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
                SelectPlayer(t.Contents);
            return;
        }
        if (Selected == null) return;
        if (Selected.SelectedAction != null)
            Selected.SelectedAction.TileClick(t);
    }

    public void SelectPlayer(ActorThing p)
    {
        if (p.ActionsLeft.Count == 0) return;
        if(Selected != null)
            Selected.Location.WipeTint();
        Selected = p;
        if (Selected.ActionsLeft.Contains(ActionCost.Move))
        {
            Selected.SelectedAction = Selected.MoveAction;
            Selected.SelectedAction.BeginSelect();
        }
        p.Location.SetTint(Color.cornflowerBlue);
    }

    public override PhaseScript NextPhase()
    {
        return new EnemyTurnPhase();
    }

    public override void TakeEvent(EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.ActionEnd:
            {
                ActorThing who = e.GetActor();
                if (Selected != who) break;
                Selected.Location.WipeTint();
                if (Selected.ActionsLeft.Count == 0)
                {
                    Players.Remove(Selected);
                    Selected.SelectedAction = null;
                    Selected = null;
                }
                break;
            }
        }
    }
}