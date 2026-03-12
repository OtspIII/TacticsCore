using System.Collections.Generic;
using UnityEngine;

public class LevelThing
{
    public Dictionary<int, Dictionary<int, GameTile>> Map = new Dictionary<int, Dictionary<int, GameTile>>();
    public List<GameTile> AllTiles = new List<GameTile>();
    public List<ActorThing> AllActors = new List<ActorThing>();

    public LevelThing()
    {
        
    }
    public void Build()
    {
        for(int x=0;x<God.LevelSize.x;x++)
            for (int y = 0; y < God.LevelSize.y; y++)
                AddTile(x, y);
        List<GameTile> OpenTiles = new List<GameTile>();
        OpenTiles.AddRange(AllTiles);
        List<CharClass> playerOpts = ThingBuilder.GetPlayers();
        List<GameTile> PlayerStart = new List<GameTile>();
        for(int x=0;x<=1;x++)
        for (int y = 0; y <= 1; y++)
            PlayerStart.Add(GetTile(x+(God.LevelSize.x/2)-1,y));
        foreach (CharClass c in playerOpts)
        {
            if (PlayerStart.Count == 0)
            {
                if (OpenTiles.Count == 0) break;
                PlayerStart.AddRange(OpenTiles);
            }
            GameTile chosen = PlayerStart.Random();
            OpenTiles.Remove(chosen);
            PlayerStart.Remove(chosen);
            AddActor(c, chosen);
        }
        List<CharClass> classOpts = ThingBuilder.GetClasses(1);
        foreach (CharClass c in classOpts)
        {
            if (OpenTiles.Count == 0) break;
            GameTile chosen = OpenTiles.Random();
            OpenTiles.Remove(chosen);
            AddActor(c, chosen);
        }
        // for (int n = 0; n < 3; n++)
        // {
        //     if (OpenTiles.Count == 0) break;
        //     GameTile chosen = OpenTiles.Random();
        //     OpenTiles.Remove(chosen);
        //     AddActor(classOpts.RandomE(), chosen);
        // }
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