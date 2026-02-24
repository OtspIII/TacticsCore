using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EnvironmentPhase : PhaseScript
{
    public List<ActorThing> Queued = new List<ActorThing>();
    
    public EnvironmentPhase()
    {
        Type = Phases.Environment;
    }

    public override void Begin()
    {
    }

    public override void OnRun()
    {
        if(Queued.Count == 0) God.GM.StartPhase();
    }

    public override PhaseScript NextPhase()
    {
        return new PlayerTurnPhase();
    }
}