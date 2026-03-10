using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnPhase : PhaseScript
{
    public List<ActorThing> Players = new List<ActorThing>();
    public ActorThing Selected;
    public bool MidEvent=false;
    
    public PlayerTurnPhase()
    {
        Type = Phases.PlayerTurn;
        // Listeners.Add(EventTypes.ActionEnd);
        // Listeners.Add(EventTypes.Death);
        // Listeners.Add(EventTypes.SelectCard);
        // Listeners.Add(EventTypes.SelectCard);
    }

    public override void Begin()
    {
        foreach (ActorThing a in God.GM.GetActors())
        {
            if (a.Has(Traits.Player) && a.Ask(EventTypes.CanAct).GetBool())
            {
                Players.Add(a);
            }
        }
    }

    public override void OnRun()
    {
        if(Players.Count == 0 || Input.GetKeyDown(KeyCode.Return)) God.GM.StartPhase();
        if(Input.GetKeyDown(KeyCode.Tilde) && Selected != null) Selected.DebugText();
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
        God.GM.SpawnCard(God.E(EventTypes.EndTurn).Set(Selected),"End Turn","Yes");
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
        if (a.Cost != ActionCost.None && !Selected.ActionsLeft.Contains(a.Cost))
        {
            //Give some feedback that the select failed
            return;
        }
        if (MidEvent && Selected.SelectedAction != a)
        {
            Selected.SelectedAction.End();
        }
        WipeTint();
        Selected.SelectedAction = a;
        Selected.SelectedAction?.Begin();
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
            case EventTypes.NewPhase: //Only called for phases after the first
            {
                MidEvent = true;
                ActorThing who = e.GetActor();
                if (Selected != who) break;
                if(God.GM.Cuts.Count == 0) Selected.SelectedAction?.BeginSelect();
                break;
            }
            case EventTypes.PostAudit: 
            {
                if(Selected?.SelectedAction != null) Selected.SelectedAction?.BeginSelect();
                break;
            }
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
                MidEvent = false;
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
            case EventTypes.Death:case EventTypes.BecomeIncap:
            {
                ActorThing t = e.GetActor("Who");
                Players.Remove(t);
                break;
            }
            case EventTypes.EndTurn:
            {
                ActorThing a = e.GetActor();
                if (a != null && Selected == a)
                {
                    UnselectPlayer();
                    Players.Remove(a);
                }
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