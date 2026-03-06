
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
    public GameTeam Team = GameTeam.None;

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
}

public class ClassPrefab : ActorPrefab
{
    public CharClass Class;

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

    public ActionPrefab(Actions t,string name,string icon,ActionCost cost,ActionSlot slot,params Traits[] tr)
    {
        Name = name;
        Icon = icon;
        Type = t;
        Cost = cost;
        Slot = slot;
        Trait.AddRange(tr);
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

    public ActionPrefab Attack(int range, ActPattern pat, int size,string dmg, DamageTypes type = DamageTypes.Normal,
        params EventInfo[] events)
    {
        List<EventInfo> e = new List<EventInfo>(){God.E(EventTypes.Damage).Roll(dmg).Set(type)};
        e.AddRange(events);
        ActionPhase p = new ActionPhase(range,Cutscenes.Attack,TargetType.Tile);
        p.Add(pat,size, ActEventTarget.Characters,e.ToArray());
        Phases.Add(p);
        return this;
    }
    
    public ActionPrefab Attack(int range,string dmg,DamageTypes type=DamageTypes.Normal,params EventInfo[] events)
    {
        Attack(range, ActPattern.TargetOnly,1,dmg, type, events);
        return this;
    }
    
    public ActionPrefab Move(int range=0)
    {
        ActionPhase p = new ActionPhase(God.N(IntStats.MoveLeft),Cutscenes.None,TargetType.EmptyTile,AITarget.Empty);
        p.Add(ActEventTarget.Self, God.E(EventTypes.WalkTo));
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