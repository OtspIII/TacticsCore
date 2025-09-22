using System.Collections.Generic;
using UnityEngine;

public static class ThingBuilder
{
    public static bool IsSetup = false;
    public static List<ActorPrefab> ActorList = new List<ActorPrefab>();
    public static Dictionary<Actors, ActorPrefab> ActorDict = new Dictionary<Actors, ActorPrefab>();
    public static List<ClassPrefab> ClassList = new List<ClassPrefab>();
    public static Dictionary<Classes, ClassPrefab> ClassDict = new Dictionary<Classes, ClassPrefab>();

    public static void Setup()
    {
        if (IsSetup) return;
        IsSetup = true;
        AddClass(Classes.Fighter);
        AddClass(Classes.Dog);
        AddClass(Classes.FireBeetle);
    }

    public static void AddClass(Classes c)
    {
        ClassPrefab r = new ClassPrefab(c);
        ClassList.Add(r);
        ClassDict.Add(c,r);
    }

    public static List<Classes> GetClasses(int level)
    {
        List<Classes> r = new List<Classes>();
        foreach(ClassPrefab c in ClassList)
            r.Add(c.Type);
        return r;
    }
}

public class ActorPrefab
{
    public Actors Type;

    public ActorPrefab(Actors t)
    {
        Type = t;
    }
}

public class ClassPrefab
{
    public Classes Type;
    
    public ClassPrefab(Classes t)
    {
        Type = t;
    }
}
