using UnityEngine;

public class BuildLevelPhase : PhaseScript
{
    public BuildLevelPhase()
    {
        Type = Phases.BuildLevel;
    }
    
    public override void Begin()
    {
        LevelThing l = new LevelThing();
        God.GM.Level = l;
        foreach (TileThing t in l.AllTiles)
        {
            God.GM.SpawnTile(t);
            if (t.Contents != null)
            {
                God.GM.SpawnActor(t.Contents);
            }
        }
        God.GM.StartPhase();
    }

}
