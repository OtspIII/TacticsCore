using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TurnStartPhase : PhaseScript
{
    public List<ActorThing> Queued = new List<ActorThing>();
    
    public TurnStartPhase()
    {
        Type = Phases.Environment;
    }

    public override void Begin()
    {
        foreach (ActorThing a in God.GM.GetActors())
        {
            a.SelectedAction = null;
            a.TakeEvent(EventTypes.StartTurn);
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
        ActorThing a = Queued.Random();
        Queued.Remove(a);
        if (a == null || a.Destroyed) return;
        a.SelectedAction = a.PickAction(ActionCost.Major);
        ActionScript before = a.GetAct(ActionSlot.BeforePlayers);
        if (before != null && (before.Cost == ActionCost.None || a.ActionsLeft.Contains(before.Cost)))
        {
            before.AISelect(a.SelectedAction);
            before.Execute();
        }
    }

    public override PhaseScript NextPhase()
    {
        return new PlayerTurnPhase();
    }
}