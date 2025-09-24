using UnityEngine;

public class BuildLevelPhase : PhaseScript
{
    public BuildLevelPhase()
    {
        Type = Phases.BuildLevel;
    }
    
    public override void Begin()
    {
        Debug.Log("HI");
        End();
    }
}
