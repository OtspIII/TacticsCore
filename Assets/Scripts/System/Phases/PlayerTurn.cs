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
        Listeners.Add(EventTypes.Death);
        Listeners.Add(EventTypes.SelectCard);
    }

    public override void Begin()
    {
        foreach (ActorThing a in God.GM.GetActors())
        {
            if (a.Has(Traits.Player) && !a.Has(CTags.Corpse))
            {
                Players.Add(a);
                a.TakeEvent(EventTypes.StartTurn);
            }
        }
    }

    public override void OnRun()
    {
        if(Players.Count == 0 || Input.GetKeyDown(KeyCode.Return)) God.GM.StartPhase();
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
        if(Selected != null) UnselectPlayer();
        WipeTint();
        Selected = p;
        if (Selected.ActionsLeft.Contains(ActionCost.Move))
        {
            SelectAction(Selected.GetAct(ActionSlot.BasicMove));
        }
        SetTint(TileTints.ActiveThing,p.Location);
        foreach (ActionSlot a in God.ActSlots)
        {
            if (!Selected.KnownActions.ContainsKey(a)) continue;
            God.GM.SpawnCard(Selected.KnownActions[a]);
        }
    }

    public void UnselectPlayer()
    {
        if (Selected == null) return;
        WipeTint();
        God.GM.WipeCards();
        Selected.SelectedAction = null;
        Selected = null;
    }

    public void SelectAction(ActionScript a)
    {
        if (a.Who != Selected)
        {
            God.LogWarning("TRIED TO SET ACTION OF INACTIVE PLAYER: " + a + " / " + a.Who + " / " + Selected);
            return;
        }
        WipeTint();
        Selected.SelectedAction = a;
        Selected.SelectedAction?.BeginSelect();
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
                    UnselectPlayer();
                }
                break;
            }
            case EventTypes.SelectCard:
            {
                Thing t = e.GetThing();
                if (t is ActionScript && Selected != null)
                {
                    SelectAction((ActionScript)t);
                }
                break;
            }
            case EventTypes.Death:
            {
                ActorThing t = e.GetActor("Who");
                Players.Remove(t);
                break;
            }
        }
    }

    public override void WipeTint(TileTints type=TileTints.None)
    {
        base.WipeTint(type);
        if (Selected?.SelectedAction != null) Selected.SelectedAction.WipeTint(type);
    }
}