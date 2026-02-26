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
    public List<TileTint> Tints = new List<TileTint>();

    public virtual void Begin()
    {
        PhaseI = 0;
        Info = new ActionInfo(this, PhaseI);
    }
    
    public virtual void BeginSelect()
    {
        Begin();
        Info.Opts = Who.Location.Flood(Phase.Range, GetNeighborMode(), Who);
        SetTint(TileTints.GoodOption,Info.Opts);
    }

    public virtual void RunSelect()
    {
        CheckForReady();
    }
    
    public virtual void EndSelect()
    {
        foreach(TileTint t in Tints) t.End();
    }
    
    public virtual void AISelect()
    {
        Begin();
    }

    public void Execute(ActionPhase p,ActionInfo i)
    {
        OnExecute(p,i);
        EndSelect();
        PhaseI++;
        if (PhaseI >= Phases.Count)
        {
            End();
        }
        else
        {
            Info = new ActionInfo(this, PhaseI);
        }
    }
    
    public virtual void OnExecute(ActionPhase p,ActionInfo i)
    {
        
    }
    
    public void End()
    {
        Who.SelectedAction = null;
        if(Cost == ActionCost.Major) Who.ActionsLeft.Clear();
        else Who.ActionsLeft.Remove(Cost);
        God.GM.TakeEvent(God.E(EventTypes.ActionEnd).Set(Who));
    }
    
    
    public virtual bool TileClick(GameTile t)
    {
        if (Info.Tiles.Count >= Phase.Tiles) return false;
        if(!Info.Opts.Contains(t)) return false;
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

    public virtual NeighborMode GetNeighborMode()
    {
        switch (Phase.Target)
        {
            case TargetType.EmptyTile: return NeighborMode.Walking;
        }
        return NeighborMode.None;
    }
    
    public void WipeTint()
    {
        foreach (TileTint t in Tints) t.End();
        Tints.Clear();
    }
    public void SetTint(TileTints t, params GameTile[] tiles)
    {
        Tints.Add(new TileTint(t, tiles));
    }
    public void SetTint(TileTints t, List<GameTile> tiles)
    {
        Tints.Add(new TileTint(t, tiles));
    }
}

public class ActionInfo
{
    public int Phase;
    public ActorThing Src;
    public ActionScript Action;
    public List<GameTile> Tiles = new List<GameTile>();
    public List<GameTile> Opts = new List<GameTile>();

    public ActionInfo(ActionScript act, int phase)
    {
        Phase = phase;
        Action = act;
        Src = act.Who;
    }

    public GameTile GetTile(int n = 0)
    {
        return Tiles.Count > n ? Tiles[n] : null;
    }
}

public class ActionPhase
{
    public ActorThing Src;
    public ActionScript Action;
    public int Tiles;
    public int Range;
    public TargetType Target = TargetType.Tile;

    public ActionPhase(ActionScript act,int t, int rng,TargetType targ =TargetType.Tile)
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

public enum TargetType
{
    None=0,
    Character=1,
    Tile=2,
    EmptyTile=3,
}