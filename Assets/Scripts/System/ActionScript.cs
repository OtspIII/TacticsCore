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
    public int Range { get { return Phases.Count > 0 ? Phases[0].Range.V(Who) : 10; } }
    public string Icon= "";
    public string Name= "";
    public int MaxUses = 0;
    public int Uses = 0;
    public List<ATags> Tags = new List<ATags>();

    public void Imprint(ActionPrefab p)
    {
        Name = p.Name;
        Icon = p.Icon;
        MaxUses = (int)p.Uses;
        Uses = MaxUses;
        Type = p.Type;
        Cost = p.Cost;
        Slot = p.Slot;
        Tags.AddRange(p.Tags);
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
            e.DRoll = new DieRoll(dmg,Who);
        }
        AuditEvents.TryGetValue(e.Type, out List<Traits> take);
        if (!e.Numbers.ContainsKey("") && e.DRoll.Setup)
        {
            e.Set(e.DRoll.Roll(Who));
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
    
    public virtual void AISelect(ActionScript main=null)
    {
        Begin();
        FindOptions();
        // Info.Opts = Who.Location.Flood(Phase.Range.V(Who), GetNeighborMode(), Who);
        if (Info.GoodOpts.Count == 0) return;
        Info.Tiles.AddRange(FindBest(Info.GoodOpts,Phase.Tiles,main));
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

    public List<GameTile> FindBest(List<GameTile> opts,int howMany=1,ActionScript main=null)
    {
        //if main is supplied, this is a walk action
        List<GameTile> o = new List<GameTile>();
        List<GameTile> backup = new List<GameTile>();
        float best = 0;
        foreach (GameTile t in opts)
        {
            float v = GetValue(t, main);
            if (v > best)
            {
                backup.AddRange(o);
                if (Phase.UniqueTiles && howMany > 1 && backup.Count >= howMany)
                {
                    backup.RemoveRange(0,backup.Count-howMany+1);
                }
                o.Clear();
                best = v;
            }
            if (v >= best) o.Add(t);
        }
        List<GameTile> r = new List<GameTile>();
        for (int n = 0; n < howMany; n++)
        {
            if (o.Count == 0)
            {
                if(backup.Count == 0) break;
                o = backup;
            }
            GameTile chosen = o.Random();
            r.Add(chosen);
            if (Phase.UniqueTiles) o.Remove(chosen);
        }
        return r;
    }

    public float GetValue(GameTile t,ActionScript main=null)
    {
        float r = 1;   //The value of the tile in and of itself
        float mod = 0; //For things like how dangerous it is to stand there
        if (main != null) //main exists only for moves to get a NPC in place for another act
        {
            r = 10 - Mathf.Abs(t.BestPDistance-main.Range);
        }
        // Debug.Log("GET VALUE: " + Who + " / " + Type + " / " + r + " / " + mod);
        EventInfo e = God.E(EventTypes.ActionValue).Set(t).SetF(r).SetF("Mod",mod).Set("Main Action",main).Set(this);
        TakeEvent(e);
        Who.TakeEvent(e);
        r = e.GetF();
        mod = e.GetF("Mod");
        // Debug.Log("END VAL: " + Who + " / " + Type + " / " + r + " / " + mod);
        return r + mod;
    }

    public bool Execute()
    {
        return Execute(Phase,Info);
    }

    public List<GameTile> GetPattern(ActionPhase p, ActionEvent a,GameTile target)
    {
        GameTile source = Who.Location;
        if (a.Target == ActEventTarget.Self) target = source;
        ActPattern actPattern = a.Pattern;
        int patternSize=a.Size;
        Directions rot = God.GetDir(source.Location,target.Location);
        List<GameTile> r = new List<GameTile>();
        switch (actPattern)
        {
            case ActPattern.None:case ActPattern.TargetOnly:
                r.Add (target);
                break;
            case ActPattern.Blast:
                r.AddRange (target.Flood (patternSize,NeighborMode.WallsBlock));
                break;
            case ActPattern.Cone:
            {
                List<GameTile> active = new List<GameTile>(){target};
				
                bool left = true;
                bool right = true;
                r.Add(target);
                for (int n = 0; n < patternSize; n++)
                {
                    List<GameTile> temp = new List<GameTile>();
                    //Every existing tile goes forward
                    foreach (GameTile gt in active)
                    {
                        if (gt == null)
                            continue;
                        GameTile t = gt.Neighbor(God.Rotate(new Vector2Int(0, 1), rot));
                        if (t != null && gt.ValidNeighbor(t,NeighborMode.WallsBlock))
                            temp.Add(t);
                    }

                    if (left && active.Count > 0)
                    {
                        GameTile t = active[0].Neighbor(God.Rotate(new Vector2Int(-1, 1), rot));
                        if (t != null && active[0].ValidNeighbor(t,NeighborMode.WallsBlock))
                            temp.Insert(0, t);
                        else
                            left = false;
                    }
                    if (right && active.Count > 0)
                    {
                        GameTile t = active[active.Count-1].Neighbor(God.Rotate(new Vector2Int(1, 1), rot));
                        if (t != null && active[active.Count-1].ValidNeighbor(t,NeighborMode.WallsBlock ))
                            temp.Add(t);
                        else
                            right = false;
                    }
                    foreach(GameTile gt in temp)
                        r.Add(gt);
                    active = temp;
                }
                break;
            }
            case ActPattern.Horizontal:
            {
                for (int n = 0; n < patternSize; n++)
                {
                    r.Add(target.Neighbor(God.Rotate(new Vector2Int(-n, 0), rot)));
                    if (n != 0)
                        r.Add(target.Neighbor(God.Rotate(new Vector2Int(n, 0), rot)));
                }

                break;
            }
            case ActPattern.Pierce:
            {
                List<GameTile> active = new List<GameTile>() { target };
                r.Add(target);
                for (int n = 0; n < patternSize; n++)
                {
                    List<GameTile> temp = new List<GameTile>();
                    //Every existing tile goes forward
                    foreach (GameTile gt in active)
                    {
                        if (gt == null)
                            continue;
                        GameTile t = gt.Neighbor(God.Rotate(new Vector2Int(0, 1), rot));
                        if (t != null && gt.ValidNeighbor(t, NeighborMode.WallsBlock))
                            temp.Add(t);
                    }

                    foreach (GameTile gt in temp)
                        r.Add(gt);
                    active = temp;
                }
                break;
            }
            case ActPattern.Source:
            {
                r.Add(source);
                break;
            }
            default:
            {
                God.LogWarning(("UNPROGRAMMED PATTERN: " + actPattern));
                r.Add(target);
                break;
            }
        }
        while (r.Contains(null))
            r.Remove(null);
        return r;
    }
    
    public bool Execute(ActionPhase p,ActionInfo i)
    {
        foreach (GameTile t in i.Tiles)
        {

            Cutscene cut = ActCut(p, t);
            if (cut != null)
            {
                God.GM.AddCut(cut);
            }


            
            foreach (ActionEvent a in p.Events)
            {
                List<GameTile> pat = GetPattern(p,a,t);
                foreach (GameTile tt in pat)
                {
                    ActorThing targ = tt.Contents;
                    // if (a.Target == ActEventTarget.Self) targ = Who;
                    
                    foreach (EventInfo e in a.Events)
                    {
                        if (e.Type == EventTypes.Summon && targ == null)
                        {
                            CharClass cc = e.GetClass();
                            if (cc != CharClass.None)
                            {
                                ActorThing sum = new ActorThing(e.GetClass(), tt);
                                God.GM.SpawnActor(sum);    
                            }
                            continue;
                        }
                        if (targ == null) continue;
                        EventInfo ae = God.E();
                        ae.Clone(e);
                        ae.Set("Target", t).Set("Source", Who);
                        AuditEvent(ae);
                        targ.TakeEvent(ae);
                    }
                }

            }
        }
        EndSelect();
        PhaseI++;
        if (PhaseI >= Phases.Count)
        {
            End();
            return true;
        }
        else
        {
            Info = new ActionInfo(this, PhaseI);
            God.GM.TakeEvent(God.E(EventTypes.NewPhase).Set(Who).Set(PhaseI));
            return false;
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
        c.Imprint(God.Library.GetIcon(Icon),Type.ToString(),"");
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
    Ultimate=5,
    Sprint=6,
}

public enum AITarget
{
    None=0,
    Empty=1,
    Enemies=2,
    Allies=3,
    Anyone=4,
    EmptyByEnemy=5,
}