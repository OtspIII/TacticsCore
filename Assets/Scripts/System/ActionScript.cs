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
    public Dictionary<EventTypes, List<Traits>> TakeListen = new Dictionary<EventTypes, List<Traits>>();
    public Dictionary<EventTypes, List<Traits>> AuditEvents = new Dictionary<EventTypes, List<Traits>>();
    public List<Traits> TraitList = new List<Traits>();

    public void Imprint(ActionPrefab p)
    {
        Type = p.Type;
        Cost = p.Cost;
        Slot = p.Slot;
        foreach (ActionPhase ap in p.Phases) Phases.Add(new ActionPhase(ap,this));
        foreach (Traits t in p.Trait) AddTrait(t);
        foreach (EventTypes e in TakeListen.Keys) SortListen(e);
        foreach (EventTypes e in AuditEvents.Keys) SortListen(e,true);
        
    }
    
    ///Adds a new trait to the Thing
    public void AddTrait(Traits t)
    {
        TraitList.Add(t);
        TraitThing tr = Parser.Get(t);
        tr.Init(this);
    }
    
    public void SortListen(EventTypes e,bool audit=false)
    {
        List<Traits> l = audit ? AuditEvents[e] : TakeListen[e];
        if (l.Count <= 1) return;
        Dictionary<Traits, float> prio = new Dictionary<Traits, float>();
        foreach (Traits t in l)
        {
            TraitThing tr = Parser.Get(t);
            float pr = tr.TakeListen[e];
            prio.Add(t,pr);
        }
        l.Sort((a, b) => { return prio[a] > prio[b] ? 1 : -1; });
    }

    public void TakeEvent(EventInfo e)
    {
        TakeListen.TryGetValue(e.Type, out List<Traits> take);
        if(take != null)
            foreach (Traits t in take)
            {
                Parser.Get(t).TakeEvent(this,e);
                if(e.Abort) break;
            }
    }
    public void AuditEvent(EventInfo e)
    {
        if (e.Texts.ContainsKey("Roll"))
        {
            string dmg = e.GetString("Roll");
            e.Roll = new DieRoll(dmg,Who);
        }
        AuditEvents.TryGetValue(e.Type, out List<Traits> take);
        if (!e.Numbers.ContainsKey("") && e.Roll.Setup)
        {
            e.Set(e.Roll.Roll(Who));
        }
        if(take != null)
            foreach (Traits t in take)
            {
                Parser.Get(t).TakeEvent(this,e);
                if(e.Abort) break;
            }
    }
    
    public virtual void Begin()
    {
        PhaseI = 0;
        Info = new ActionInfo(this, PhaseI);
    }
    
    public virtual void BeginSelect()
    {
        Begin();
        FindOptions();
        SetTint(TileTints.GoodOption,Info.GoodOpts);
        SetTint(TileTints.OkayOption,Info.BadOpts);
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
        FindOptions();
        // Info.Opts = Who.Location.Flood(Phase.Range.V(Who), GetNeighborMode(), Who);
        if (Info.GoodOpts.Count == 0) return;
        Info.Tiles.AddRange(FindBest(Info.GoodOpts,Phase.Tiles));
    }

    public void FindOptions()
    {
        Dictionary<GameTile, bool> r = new Dictionary<GameTile, bool>();
        List<GameTile> opts = Who.Location.Flood(Phase.Range.V(Who), GetNeighborMode(), Who);
        foreach (GameTile t in opts)
        {
            if (Phase.Target == TargetType.Character && t.Contents == null) continue;
            if (Phase.Target == TargetType.EmptyTile && t.Contents != null) continue;
            if (Phase.AI == AITarget.Enemies && t == Who.Location) continue;
            Info.Opts.Add(t);
            if(IsGood(t))
                Info.GoodOpts.Add(t);
            else
                Info.BadOpts.Add(t);
        }
    }

    public bool IsGood(GameTile o)
    {
        switch (Phase.AI)
        {
            case AITarget.Enemies: return Who.IsEnemy(o.Contents);
            case AITarget.Allies: return o.Contents != null && Who.Team == o.Contents.Team;
            case AITarget.Empty: return o.Contents == null;
            case AITarget.Anyone: return o.Contents != null;
        }
        return true;
    }
    
    public void AddListen(EventTypes e, Traits t,bool audit = false)
    {
        //Which dictionary are we adding this to? Pre or normal listen?
        Dictionary<EventTypes, List<Traits>> d = audit ? AuditEvents : TakeListen;
        if(!d.ContainsKey(e)) d.Add(e,new List<Traits>()); //If the dictionary doesn't already have this event, add it
        if(!d[e].Contains(t)) d[e].Add(t);                 //If the event doesn't have this trait, add it
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
                    EventInfo ae = God.E();
                    ae.Clone(e);
                    ae.Set("Target", t).Set("Source", Who);
                    AuditEvent(ae);
                    targ.TakeEvent(ae);
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
    public List<GameTile> GoodOpts = new List<GameTile>();
    public List<GameTile> BadOpts = new List<GameTile>();

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
    public AITarget AI = AITarget.Enemies;

    public ActionPhase(int rng,Cutscenes cut=Cutscenes.None,TargetType targ =TargetType.Tile,AITarget ai=AITarget.Enemies,int t=1)
    {
        Tiles = t;
        Range = God.N(rng);
        Cut = cut;
        Target = targ;
        AI = ai;
    }
    
    public ActionPhase(Number rng,Cutscenes cut=Cutscenes.None,TargetType targ =TargetType.Tile,AITarget ai=AITarget.Enemies,int t=1)
    {
        Tiles = t;
        Range = rng;
        Cut = cut;
        Target = targ;
        AI = ai;
    }

    public ActionPhase(ActionPhase ap,ActionScript act)
    {
        Tiles = ap.Tiles;
        Range = ap.Range;
        Target = ap.Target;
        Events = ap.Events;
        Cut = ap.Cut;
        AI = ap.AI;
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

public enum AITarget
{
    None=0,
    Empty=1,
    Enemies=2,
    Allies=3,
    Anyone=4,
}