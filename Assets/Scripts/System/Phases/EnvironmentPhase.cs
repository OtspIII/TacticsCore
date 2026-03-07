using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TurnStartPhase : PhaseScript
{
    public TurnStartPhase()
    {
        Type = Phases.Environment;
    }

    public override void Begin()
    {
        foreach (ActorThing a in God.GM.GetActors())
        {
            a.TakeEvent(EventTypes.StartTurn);
        }
    }

    public override void OnRun()
    {
        God.GM.StartPhase();
    }

    public override PhaseScript NextPhase()
    {
        return new PlayerTurnPhase();
    }
}