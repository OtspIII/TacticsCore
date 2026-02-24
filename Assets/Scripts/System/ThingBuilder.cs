using System.Collections.Generic;
using UnityEngine;

public static class ThingBuilder
{
    public static bool IsSetup = false;
    public static List<ActorPrefab> ActorList = new List<ActorPrefab>();
    public static Dictionary<Actors, ActorPrefab> ActorDict = new Dictionary<Actors, ActorPrefab>();
    public static List<ClassPrefab> ClassList = new List<ClassPrefab>();
    public static Dictionary<Classes, ClassPrefab> ClassDict = new Dictionary<Classes, ClassPrefab>();
    public static List<ClassPrefab> PlayerList = new List<ClassPrefab>();

    public static void Setup()
    {
        if (IsSetup) return;
        IsSetup = true;
        AddPlayer(Classes.Fighter,5);
        AddClass(Classes.Dog,3);
        AddClass(Classes.FireBeetle,2);
    }

    public static ClassPrefab AddPlayer(Classes c,int hp)
    {
        ClassPrefab r = new ClassPrefab(c);
        PlayerList.Add(r);
        ClassDict.Add(c,r);
        r.Trait(Traits.Player);
        r.Trait(Traits.Health,hp);
        return r;
    }
    
    public static ClassPrefab AddClass(Classes c,int hp)
    {
        ClassPrefab r = new ClassPrefab(c);
        ClassList.Add(r);
        ClassDict.Add(c,r);
        r.Trait(Traits.Health,hp);
        return r;
    }

    public static List<Classes> GetClasses(int level)
    {
        List<Classes> r = new List<Classes>();
        foreach(ClassPrefab c in ClassList)
            r.Add(c.Class);
        return r;
    }
    
    public static List<Classes> GetPlayers()
    {
        List<Classes> r = new List<Classes>();
        foreach(ClassPrefab c in PlayerList)
            r.Add(c.Class);
        return r;
    }
    
}

public class ActorPrefab
{
    public Actors Type;
    public List<TraitBuilder> TraitList = new List<TraitBuilder>();

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
}

public class ClassPrefab : ActorPrefab
{
    public Classes Class;
    
    public ClassPrefab(Classes t)
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
