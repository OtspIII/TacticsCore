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
        if (Input.GetKeyDown(KeyCode.Alpha1) && Selected != null)
        {
            SelectAction(new AttackAction(Selected));
        }
    }

    public override void TileClick(GameTile t)
    {
        if (t.Contents != null && (Selected?.SelectedAction == null || !Selected.SelectedAction.Info.Opts.Contains(t)))
        {
            if (Players.Contains(t.Contents))
            {
                SelectPlayer(t.Contents);
                return;
            }
        }
        if (Selected?.SelectedAction != null)
            Selected.SelectedAction.TileClick(t);
    }

    public void SelectPlayer(ActorThing p)
    {
        if (p.ActionsLeft.Count == 0) return;
        WipeTint();
        Selected = p;
        if (Selected.ActionsLeft.Contains(ActionCost.Move))
        {
            SelectAction(Selected.MoveAction);
        }
        SetTint(TileTints.ActiveThing,p.Location);
    }

    public void SelectAction(ActionScript a)
    {
        if (a.Who != Selected)
        {
            God.LogWarning("TRIED TO SET ACTION OF INACTIVE PLAYER: " + a + " / " + a.Who);
            return;
        }
        WipeTint();
        Selected.SelectedAction = a;
        Selected.SelectedAction.BeginSelect();
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

    public override void WipeTint()
    {
        base.WipeTint();
        if (Selected?.SelectedAction != null) Selected.SelectedAction.WipeTint();
    }
}