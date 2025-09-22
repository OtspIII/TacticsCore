using System.Collections.Generic;
using UnityEngine;

public class LevelThing
{
    public Dictionary<int, Dictionary<int, TileThing>> Map = new Dictionary<int, Dictionary<int, TileThing>>();
    public List<TileThing> AllTiles = new List<TileThing>();
    public List<ActorThing> AllActors = new List<ActorThing>();
    
    public LevelThing()
    {
        for(int x=0;x<5;x++)
            for (int y = 0; y < 5; y++)
                AddTile(x, y);
        List<TileThing> OpenTiles = new List<TileThing>();
        OpenTiles.AddRange(AllTiles);
        List<Classes> classOpts = ThingBuilder.GetClasses(1);
        for (int n = 0; n < 3; n++)
        {
            if (OpenTiles.Count == 0) break;
            TileThing chosen = OpenTiles.Random();
            OpenTiles.Remove(chosen);
            AddActor(classOpts.RandomE(), chosen);
        }
    }

    public TileThing AddTile(int x, int y)
    {
        TileThing t = new TileThing(x, y);
        AllTiles.Add(t);
        if(!Map.ContainsKey(x)) Map.Add(x,new Dictionary<int, TileThing>());
        Map[x].Add(y, t);
        return t;
    }

    public TileThing GetTile(int x, int y)
    {
        if (!Map.ContainsKey(x)) return null;
        return Map[x].TryGetValue(y, out TileThing r) ? r : null;
    }

    public ActorThing AddActor(Classes c, TileThing t)
    {
        ActorThing r = new ActorThing(c, t);
        return r;
    }
    
    public ActorThing AddActor(Actors a, TileThing t)
    {
        ActorThing r = new ActorThing(a, t);
        return r;
    }
}