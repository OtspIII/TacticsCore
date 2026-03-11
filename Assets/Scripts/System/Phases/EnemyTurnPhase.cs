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
            if (!a.Has(Traits.Player) && a.Ask(EventTypes.CanAct).GetBool()) Queued.Add(a);
        }
        God.GM.CalcMapPDist();
    }

    public override void OnRun()
    {
        if (Queued.Count == 0)
        {
            God.GM.StartPhase();
            return;
        }

        ActorThing a = Queued[0];
        Queued.Remove(a);
        if (a == null || a.Destroyed) return;

        ActionScript main = a.SelectedAction;
        ActionScript move = a.PickAction(ActionCost.Move,main);
        ActionScript bonus = a.PickAction(ActionCost.Bonus);
        if (move != null && a.ActionsLeft.Contains(ActionCost.Move) && (main == null || !main.Tags.Contains(ATags.DontMove)))
        {
            move.AISelect(main);
            move.Execute();
        }
        if (bonus != null && a.ActionsLeft.Contains(ActionCost.Bonus))
        {
            bonus.AISelect();
            bonus.Execute();
        }
        if (main != null && a.ActionsLeft.Contains(ActionCost.Major))
        {
            main.AISelect();
            main.Execute();
        }
    }

    public void SortQueue()
    {
        List<ActorThing> fast = new List<ActorThing>();
        List<ActorThing> normal = new List<ActorThing>();
        List<ActorThing> slow = new List<ActorThing>();
        foreach (ActorThing a in Queued)
        {
            if (a.SelectedAction == null) normal.Add(a);
            else if(a.SelectedAction.Has(ATags.Fast)) fast.Add(a);
            else if(a.SelectedAction.Has(ATags.Slow)) slow.Add(a);
            else normal.Add(a);
        }
        List<ActorThing> r = new List<ActorThing>();
        r.AddRange(fast.Shuffle());
        r.AddRange(normal.Shuffle());
        r.AddRange(slow.Shuffle());
        Queued = r;
    }

    public override PhaseScript NextPhase()
    {
        return new TurnStartPhase();
    }
}