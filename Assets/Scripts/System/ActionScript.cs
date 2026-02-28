using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScript : Thing
{
    public Actions Type;
    public ActionCost Cost;
    public ActorThing Who;
    public ActionInfo Info;
    public ActionSlot Slot;
    public List<ActionPhase> Phases = new List<ActionPhase>();
    public int PhaseI = 0;
    public ActionPhase Phase {get{return Phases.Count > PhaseI ? Phases[PhaseI] : null;}}
    public Dictionary<TileTints,TileTint> Tints = new Dictionary<TileTints, TileTint>();

    public void Imprint(ActionPrefab p)
    {
        Type = p.Type;
        Cost = p.Cost;
        Slot = p.Slot;
        foreach (ActionPhase ap in p.Phases) Phases.Add(new ActionPhase(ap,this));
    }
    
    public virtual void Begin()
    {
        PhaseI = 0;
        Info = new ActionInfo(this, PhaseI);
    }
    
    public virtual void BeginSelect()
    {
        Begin();
        Info.Opts = Who.Location.Flood(Phase.Range.V(Who), GetNeighborMode(), Who);
        SetTint(TileTints.GoodOption,Info.Opts);
    }

    public virtual void RunSelect()
    {
        CheckForReady();
    }
    
    public virtual void EndSelect()
    {
        WipeTint();
    }
    
    public virtual void AISelect()
    {
        Begin();
        Info.Opts = Who.Location.Flood(Phase.Range.V(Who), GetNeighborMode(), Who);
        if (Info.Opts.Count == 0) return;
        Info.Tiles.AddRange(FindBest(Info.Opts,Phase.Tiles));
    }

    public List<GameTile> FindBest(List<GameTile> opts,int howMany=1)
    {
        List<GameTile> o = new List<GameTile>();
        o.AddRange(opts);
        List<GameTile> r = new List<GameTile>();
        for (int n = 0; n < howMany; n++)
        {
            if (o.Count == 0) break;
            GameTile chosen = o.Random();
            r.Add(chosen);
            if (Phase.UniqueTiles) o.Remove(chosen);
        }
        return r;
    }

    public void Execute()
    {
        Execute(Phase,Info);
    }
    
    public void Execute(ActionPhase p,ActionInfo i)
    {
        foreach (GameTile t in i.Tiles)
        {
            
            Cutscene cut = ActCut(p, t);
            if (cut != null)
            {
                God.GM.AddCut(cut);
            }
            ActorThing who = t.Contents;
            foreach (ActionEvent a in p.Events)
            {
                ActorThing targ = who;
                if (a.Target == ActEventTarget.Self) targ = Who;
                if (targ == null) continue;
                foreach (EventInfo e in a.Events)
                {
                    targ.TakeEvent(e.Set("Target",t));
                }
            }
        }

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
    
    public void WipeTint(TileTints type=TileTints.None)
    {
        if (type != TileTints.None)
        {
            if (Tints.ContainsKey(type))
            {
                Tints[type].End();
                Tints.Remove(type);
            }
            return;
        }
        foreach (TileTint t in Tints.Values) t.End();
        Tints.Clear();
    }
    public void SetTint(TileTints t, params GameTile[] tiles)
    {
        TileTint tt = new TileTint(t, tiles);
        if (Tints.ContainsKey(t))
        {
            Tints[t] = tt;
            return;
        }
        Tints.Add(t,tt);
    }
    public void SetTint(TileTints t, List<GameTile> tiles)
    {
        TileTint tt = new TileTint(t, tiles);
        if (Tints.ContainsKey(t))
        {
            Tints[t] = tt;
            return;
        }
        Tints.Add(t,tt);
    }

    public override void ImprintCard(CardScript c)
    {
        c.Imprint(null,Type.ToString(),"");
    }

    public override string ToString()
    {
        return "Action["+Type+"/"+Who+"]";
    }

    public Cutscene ActCut(ActionPhase p,GameTile i)
    {
        switch (p.Cut)
        {
            case Cutscenes.Attack:
            {
                return new AttackCut(Who, i);
            }
        }
        return null;
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
    public Number Range;
    public TargetType Target = TargetType.Tile;
    public List<ActionEvent> Events =  new List<ActionEvent>();
    public bool UniqueTiles =false;
    public Cutscenes Cut = Cutscenes.None;

    public ActionPhase(int rng,Cutscenes cut=Cutscenes.None,TargetType targ =TargetType.Tile,int t=1)
    {
        Tiles = t;
        Range = God.N(rng);
        Cut = cut;
    }
    
    public ActionPhase(Number rng,Cutscenes cut=Cutscenes.None,TargetType targ =TargetType.Tile,int t=1)
    {
        Tiles = t;
        Range = rng;
        Cut = cut;
    }

    public ActionPhase(ActionPhase ap,ActionScript act)
    {
        Tiles = ap.Tiles;
        Range = ap.Range;
        Target = ap.Target;
        Events = ap.Events;
        Cut = ap.Cut;
        UniqueTiles = ap.UniqueTiles;
        Action = act;
        Src = act.Who;
    }

    public ActionPhase Add(ActionEvent e)
    {
        Events.Add(e);
        return this;
    }
    
    public ActionPhase Add(ActPattern p,int size,ActEventTarget t, params EventInfo[] events)
    {
        Events.Add(new ActionEvent(p,size,t,events));
        return this;
    }
    
    public ActionPhase Add(ActPattern p,int size,params EventInfo[] events)
    {
        Events.Add(new ActionEvent(p,size,events));
        return this;
    }
    
    public ActionPhase Add(ActEventTarget t, params EventInfo[] events)
    {
        Events.Add(new ActionEvent(ActPattern.None,1,t,events));
        return this;
    }
    
    public ActionPhase Add(params EventInfo[] events)
    {
        Events.Add(new ActionEvent(events));
        return this;
    }
    
    public ActionPhase SetUnique(bool u)
    {
        UniqueTiles = u;
        return this;
    }
}

public class ActionEvent
{
    public List<EventInfo> Events =  new List<EventInfo>();
    public ActPattern Pattern = ActPattern.None;
    public int Size=1;
    public ActEventTarget  Target = ActEventTarget.Everything;

    public ActionEvent(ActPattern p,int size,ActEventTarget t, params EventInfo[] events)
    {
        Pattern = p;
        Size=size;
        Target = t;
        Events.AddRange(events);
    }
    
    public ActionEvent(ActPattern p,int size,params EventInfo[] events)
    {
        Pattern = p;
        Size=size;
        Events.AddRange(events);
    }
    
    public ActionEvent(params EventInfo[] events)
    {
        Events.AddRange(events);
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

public enum ActEventTarget
{
    None=0,
    Everything=1,
    Characters=2,
    Floor=3,
    Allies=4,
    Enemies=5,
    Tile=6,
    Self=7,
}

public enum ActionSlot
{
    None=0,
    BasicMove=1,
    BasicAttack=2,
    Secondary=3,
    Utility=4,
    Ultimate=5
}