using System.Collections.Generic;
using UnityEngine;

public class LevelThing
{
    public Dictionary<int, Dictionary<int, GameTile>> Map = new Dictionary<int, Dictionary<int, GameTile>>();
    public List<GameTile> AllTiles = new List<GameTile>();
    public List<ActorThing> AllActors = new List<ActorThing>();
    
    public LevelThing()
    {
        for(int x=0;x<5;x++)
            for (int y = 0; y < 5; y++)
                AddTile(x, y);
        List<GameTile> OpenTiles = new List<GameTile>();
        OpenTiles.AddRange(AllTiles);
        List<CharClass> playerOpts = ThingBuilder.GetPlayers();
        for (int n = 0; n < 2; n++)
        {
            if (OpenTiles.Count == 0) break;
            GameTile chosen = OpenTiles.Random();
            OpenTiles.Remove(chosen);
            AddActor(playerOpts.RandomE(), chosen);
        }
        List<CharClass> classOpts = ThingBuilder.GetClasses(1);
        for (int n = 0; n < 3; n++)
        {
            if (OpenTiles.Count == 0) break;
            GameTile chosen = OpenTiles.Random();
            OpenTiles.Remove(chosen);
            AddActor(classOpts.RandomE(), chosen);
        }
    }

    public GameTile AddTile(int x, int y)
    {
        GameTile t = new GameTile(x, y);
        AllTiles.Add(t);
        if(!Map.ContainsKey(x)) Map.Add(x,new Dictionary<int, GameTile>());
        Map[x].Add(y, t);
        return t;
    }

    public GameTile GetTile(int x, int y)
    {
        if (!Map.ContainsKey(x)) return null;
        return Map[x].TryGetValue(y, out GameTile r) ? r : null;
    }

    public ActorThing AddActor(CharClass c, GameTile t)
    {
        ActorThing r = new ActorThing(c, t);
        return r;
    }
    
    public ActorThing AddActor(Actors a, GameTile t)
    {
        ActorThing r = new ActorThing(a, t);
        return r;
    }
}