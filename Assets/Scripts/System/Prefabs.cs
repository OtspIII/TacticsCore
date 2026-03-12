
using System.Collections.Generic;
using UnityEngine;

public class ActorPrefab
{
    public Actors Type;
    public string Name;
    public List<TraitBuilder> TraitList = new List<TraitBuilder>();
    public Dictionary<IntStats, int> Stats = new Dictionary<IntStats, int>();
    public Dictionary<StrStats, string> TxtStats = new Dictionary<StrStats, string>();
    public List<Actions> KnownActions = new List<Actions>();
    public List<ActionPrefab> ActionPs = new List<ActionPrefab>();
    public GameTeam Team = GameTeam.None;
    public List<CTags> Tags = new List<CTags>();
    public int Level = 0;

    public ActorPrefab() { }
    
    public ActorPrefab(Actors t)
    {
        Type = t;
    }

    public ActorPrefab Trait(Traits tr,int n=0)
    {
        TraitBuilder t = new TraitBuilder(tr);
        TraitList.Add(t);
        if (n != 0) t.E.Set(n);
        return this;
    }

    public ActorPrefab Act(Actions a)
    {
        KnownActions.Add(a);
        return this;
    }
    
    public ActorPrefab Act(ActionPrefab a,bool react=false)
    {
        if(react) a.Tags.Add(ATags.ReactionOK);
        ActionPs.Add(a);
        
        return this;
    }
    
    public ActorPrefab Tag(params CTags[] a)
    {
        Tags.AddRange(a);
        return this;
    }
}

public class ClassPrefab : ActorPrefab
{
    public CharClass Class;
    public float Cost=1;

    public ClassPrefab(CharClass t)
    {
        Type = Actors.Character;
        Class = t;
    }
}

public class TraitBuilder
{
    public Traits Type;
    public EventInfo E = new EventInfo();

    public TraitBuilder(Traits t)
    {
        Type = t;
    }
}

public class ActionPrefab
{
    public Actions Type;
    public string Name;
    public UsesNum Uses = UsesNum.None;
    public ActionCost Cost;
    public ActionSlot Slot;
    public List<ActionPhase> Phases = new List<ActionPhase>();
    public List<Traits> Trait = new List<Traits>();
    public CharClass Class=CharClass.None;
    public string Icon="";
    public List<ATags> Tags = new List<ATags>();

    public ActionPrefab(string name,string icon,ActionSlot slot,ActionCost cost=ActionCost.Major,params Traits[] tr)
    {
        Name = name;
        Icon = icon;
        Type = Actions.None;
        Cost = cost;
        Slot = slot;
        Trait.AddRange(tr);
    }
    
    public ActionPrefab(Actions t,string name,string icon,ActionSlot slot,ActionCost cost=ActionCost.Major,params Traits[] tr)
    {
        Name = name;
        Icon = icon;
        Type = t;
        Cost = cost;
        Slot = slot;
        Trait.AddRange(tr);
    }

    public ActionPrefab Tag(params ATags[] tags)
    {
        Tags.AddRange(tags);
        return this;
    }
    
    public ActionPrefab Set(UsesNum u)
    {
        Uses = u;
        return this;
    }
    public ActionPrefab Set(CharClass c)
    {
        Class = c;
        return this;
    }

    public ActionPrefab Phase(ActionPhase p)
    {
        Phases.Add(p);
        return this;
    }
    
    public ActionPrefab SingleTarget(int range,params EventInfo[] events)
    {
        ActionPhase p = new ActionPhase(range,Cutscenes.Attack,TargetType.Tile);
        p.Add(ActEventTarget.Characters, events);
        Phases.Add(p);
        return this;
    }
    
    public ActionPrefab PTarg(TargetType tt=TargetType.None,AITarget at=AITarget.None,Cutscenes cut=Cutscenes.None,bool unique=false)
    {
        if (Phases.Count == 0)
        {
            God.LogWarning("NO PHASE ADDED YET BUT TRIED TOA DD PTARG: " + Name);
            return this;
        }
        ActionPhase p = Phases[Phases.Count - 1];
        if(tt != TargetType.None) p.Target = tt;
        if(cut != Cutscenes.None) p.Cut = cut;
        if(at != AITarget.None) p.AI = at;
        p.UniqueTiles = unique;
        return this;
    }

    public ActionPrefab AttackTag(int range, ActPattern pat, int size,string dmg="W", DamageTypes type = DamageTypes.Normal,
        params string[] tags)
    {
        EventInfo atk = God.E(EventTypes.Damage).Roll(dmg).Set(type);
        foreach (string t in tags) atk.Set(t, true);
        List<EventInfo> e = new List<EventInfo>(){atk};
        ActionPhase p = new ActionPhase(range,Cutscenes.Attack,TargetType.Tile);
        p.Add(pat,size, ActEventTarget.Characters,e.ToArray());
        Phases.Add(p);
        return this;
    }
    
    public ActionPrefab Attack(int range, ActPattern pat, int size,string dmg="W", DamageTypes type = DamageTypes.Normal,
        params EventInfo[] events)
    {
        List<EventInfo> e = new List<EventInfo>(){God.E(EventTypes.Damage).Roll(dmg).Set(type)};
        e.AddRange(events);
        ActionPhase p = new ActionPhase(range,Cutscenes.Attack,TargetType.Tile);
        p.Add(pat,size, ActEventTarget.Characters,e.ToArray());
        Phases.Add(p);
        return this;
    }
    
    public ActionPrefab ATrait(params Traits[] t)
    {
        Trait.AddRange(t);
        return this;
    }
    
    public ActionPrefab GTrait(int range,Traits tr,string res, int dur,params EventInfo[] events)
    {
        GTrait(range, ActPattern.TargetOnly,1,tr, res,dur);
        return this;
    }
    
    public ActionPrefab GTrait(int range, ActPattern pat, int size,Traits tr,string res, 
        int dur=-1,int amt=0,ActEventTarget targ=ActEventTarget.Characters)
    {
        List<EventInfo> e = new List<EventInfo>(){God.E(EventTypes.GainTrait).Resist(res).Set(tr).Set("Duration",dur).Set(amt)};
        ActionPhase p = new ActionPhase(range,Cutscenes.Attack,TargetType.Tile);
        p.Add(pat,size, targ,e.ToArray());
        Phases.Add(p);
        return this;
    }
    
    public ActionPrefab Attack(int range,string dmg="W",DamageTypes type=DamageTypes.Normal,params EventInfo[] events)
    {
        Attack(range, ActPattern.TargetOnly,1,dmg, type, events);
        return this;
    }
    
    public ActionPrefab Move(Number range=null,bool teleport=false,List<EventInfo> preE=null,List<EventInfo> postE=null)
    {
        if (range == null) range = God.N(IntStats.MoveLeft);
        ActionPhase p = new ActionPhase(range,Cutscenes.None,TargetType.EmptyTile,AITarget.Empty);
        List<EventInfo> evs = new List<EventInfo>();
        if(preE!=null) evs.AddRange(preE);
        evs.Add(God.E(EventTypes.WalkTo).Set("Teleport",teleport));
        if(postE!=null) evs.AddRange(postE);
        p.Add(ActEventTarget.Self, evs.ToArray());
        Phases.Add(p);
        return this;
    }
    
    public ActionPrefab Self(params EventInfo[] events)
    {
        ActionPhase p = new ActionPhase(0,Cutscenes.None,TargetType.Self,AITarget.Self);
        p.Add(ActEventTarget.Self, events);
        Phases.Add(p);
        return this;
    }
    
    public ActionPrefab EmptyTile(int range, params EventInfo[] events)
    {
        ActionPhase p = new ActionPhase(range,Cutscenes.Attack,TargetType.EmptyTile,AITarget.EmptyByEnemy);
        p.Add(ActEventTarget.Tile, events);
        Phases.Add(p);
        return this;
    }

    public ActionPrefab PAdd(ActPattern pat, int size,ActEventTarget t, params EventInfo[] events)
    {
        ActionPhase p = Phases[Phases.Count - 1];
        p.Add(pat, size, t, events);
        return this;
    }
    
    public ActionPrefab PAdd(ActEventTarget t,params EventInfo[] events)
    {
        ActionPhase p = Phases[Phases.Count - 1];
        p.Add(t, events);
        return this;
    }
    
    public ActionPrefab EAdd(params EventInfo[] events)
    {
        ActionPhase p = Phases[Phases.Count - 1];
        if (p.Events.Count == 0)
        {
            God.LogWarning("EADD ON A PHASE WITH NO EVENTS: " + Name);
            p.Events.Add(new ActionEvent(events));
            return this;
        }
        ActionEvent e = p.Events[p.Events.Count - 1];
        e.Events.AddRange(events);
        return this;
    }
}