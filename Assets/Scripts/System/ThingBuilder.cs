using System.Collections.Generic;
using UnityEngine;

public static class ThingBuilder
{
    public static bool IsSetup = false;
    public static List<ActorPrefab> ActorList = new List<ActorPrefab>();
    public static Dictionary<Actors, ActorPrefab> ActorDict = new Dictionary<Actors, ActorPrefab>();
    public static List<ClassPrefab> ClassList = new List<ClassPrefab>();
    public static Dictionary<CharClass, ClassPrefab> ClassDict = new Dictionary<CharClass, ClassPrefab>();
    public static List<ClassPrefab> PlayerList = new List<ClassPrefab>();
    public static Dictionary<Actions,ActionPrefab> ActionDict = new Dictionary<Actions, ActionPrefab>();

    public static void Setup()
    {
        if (IsSetup) return;
        IsSetup = true;
        AddPlayer(CharClass.Fighter,10,3,4,"1d8");
        AddPlayer(CharClass.Wizard,5,0,4,"1d4");
        AddPlayer(CharClass.Cleric,8,2,4,"1d6");
        AddPlayer(CharClass.Thief,7,1,5,"1d8");
        AddNPC(CharClass.RatmanCardTosser,4,0,5,"1d3");
        AddNPC(CharClass.RatmanGourmand,8,0,4,"1d6");
        AddNPC(CharClass.RatmanPrayerSqueak,4,0,5,"1d3");
        AddNPC(CharClass.RatmanMutant,4,0,5,"2d4");

        AddAction(Actions.Walk, ActionCost.Move, ActionSlot.BasicMove).Move();
        AddAction(Actions.BasicAttack, ActionCost.Major, ActionSlot.BasicAttack).SingleTarget(1,God.E(EventTypes.Damage).Set("Roll","W"));
    }

    public static ClassPrefab AddPlayer(CharClass c,int hp,int def,int spd,string dmg)
    {
        ClassPrefab r = AddClass(c, hp, def, spd, dmg);
        PlayerList.Add(r);
        r.Trait(Traits.Player);
        r.Team = GameTeam.Player;
        return r;
    }
    
    public static ClassPrefab AddNPC(CharClass c,int hp,int def,int spd,string dmg,bool aggro=true)
    {
        ClassPrefab r = AddClass(c, hp, def, spd, dmg);
        if (aggro) r.Team = GameTeam.Enemy;
        ClassList.Add(r);
        return r;
    }
    
    public static ClassPrefab AddClass(CharClass c,int hp,int def,int spd,string dmg)
    {
        ClassPrefab r = new ClassPrefab(c);
        ClassDict.Add(c,r);
        r.Trait(Traits.Alive);
        if (spd > 0)
        {
            r.Trait(Traits.Mobile);
            r.Stats.Add(IntStats.Movespeed,spd);
        }
        r.Stats.Add(IntStats.HP,hp);
        r.Stats.Add(IntStats.Defense,def);
        r.TxtStats.Add(StrStats.Damage,dmg);
        return r;
    }
    
    public static ActionPrefab AddAction(Actions t,ActionCost cost,ActionSlot slot,params Traits[] traits)
    {
        ActionPrefab r = new ActionPrefab(t,cost,slot,traits);
        ActionDict.Add(t,r);
        return r;
    }

    public static List<CharClass> GetClasses(int level)
    {
        List<CharClass> r = new List<CharClass>();
        foreach(ClassPrefab c in ClassList)
            r.Add(c.Class);
        return r;
    }
    
    public static List<CharClass> GetPlayers()
    {
        List<CharClass> r = new List<CharClass>();
        foreach(ClassPrefab c in PlayerList)
            r.Add(c.Class);
        return r;
    }
    
    public static ActionScript MakeAction(Actions a)
    {
        ActionScript r = new ActionScript();
        ActionPrefab p = ActionDict[a];
        r.Imprint(p);
        return r;
    }
    
}

public class ActorPrefab
{
    public Actors Type;
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
    public ActionCost Cost;
    public ActionSlot Slot;
    public List<ActionPhase> Phases = new List<ActionPhase>();
    public List<Traits> Trait = new List<Traits>();

    public ActionPrefab(Actions t,ActionCost cost,ActionSlot slot,params Traits[] tr)
    {
        Type = t;
        Cost = cost;
        Slot = slot;
        Trait.AddRange(tr);
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
    
    public ActionPrefab Move(int range=0)
    {
        ActionPhase p = new ActionPhase(God.N(IntStats.Movespeed),Cutscenes.None,TargetType.EmptyTile,AITarget.Empty);
        p.Add(ActEventTarget.Self, God.E(EventTypes.WalkTo));
        Phases.Add(p);
        return this;
    }
}