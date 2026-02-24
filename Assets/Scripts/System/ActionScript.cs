using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScript
{
    public Actions Type;
    public ActionCost Cost;
    public ActorThing Who;
    public ActionInfo Info;
    public List<ActionPhase> Phases = new List<ActionPhase>();
    public int PhaseI = 0;
    public ActionPhase Phase {get{return Phases.Count > PhaseI ? Phases[PhaseI] : null;}}
    
    public virtual void Begin()
    {
        PhaseI = 0;
        Info = new ActionInfo(this, PhaseI);
        Debug.Log("BEGIN ACT: " + Type);
    }

    public virtual void EndSelect()
    {
        
    }

    public virtual void Run()
    {
        CheckForReady();
    }
    
    public virtual void Execute(ActionPhase p,ActionInfo i)
    {
        EndSelect();
    }
    
    public void End()
    {
        if(Cost == ActionCost.Major) Who.ActionsLeft.Clear();
        else Who.ActionsLeft.Remove(Cost);
        OnEnd();
    }
    
    public virtual void OnEnd()
    {
        
    }
    
    public virtual bool TileClick(TileThing t)
    {
        Debug.Log("PHASES: " + Phases.Count + PhaseI);
        Debug.Log("P: " + Phases[PhaseI]);
        if (Info.Tiles.Count >= Phase.Tiles) return false;
        Info.Tiles.Add(t);
        return CheckForReady();
    }

    public bool CheckForReady()
    {
        if (Info.Tiles.Count >= Phase.Tiles)
        {
            Execute(Phase,Info);
            PhaseI++;
            if (PhaseI >= Phases.Count)
            {
                End();
                return true;
            }
        }

        return false;
    }
}

public class ActionInfo
{
    public int Phase;
    public ActorThing Src;
    public ActionScript Action;
    public List<TileThing> Tiles = new List<TileThing>();

    public ActionInfo(ActionScript act, int phase)
    {
        Phase = phase;
        Action = act;
        Src = act.Who;
    }
}

public class ActionPhase
{
    public ActorThing Src;
    public ActionScript Action;
    public int Tiles;
    public int Range;

    public ActionPhase(ActionScript act,int t, int rng)
    {
        Action = act;
        Src = act.Who;
        Tiles = t;
        Range = rng;
    }
}


public enum ActionCost
{
    None=0,
    Major=1,
    Bonus=2,
    Move=3
}