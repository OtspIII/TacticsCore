using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dictionary<int, Dictionary<int, TileController>> Map = new Dictionary<int, Dictionary<int, TileController>>();
    public List<TileController> AllTiles = new List<TileController>();
    public List<ActorController> AllActors = new List<ActorController>();

    void Awake()
    {
        God.GM = this;
        ThingBuilder.Setup();
    }

    void Start()
    {
       BuildLevel(); 
    }

    public void BuildLevel()
    {
        foreach (ActorController a in AllActors.ToArray()) Destroy(a);
        foreach (TileController t in AllTiles.ToArray()) Destroy(t);
        AllActors.Clear();
        AllTiles.Clear();
        Map.Clear();
        LevelThing l = new LevelThing();
        foreach (TileThing t in l.AllTiles)
        {
            SpawnTile(t);
            if (t.Contents != null)
            {
                SpawnActor(t.Contents);
            }
        }
    }

    public TileController SpawnTile(TileThing t)
    {
        TileController r = Instantiate(God.Library.TilePrefab);
        r.Setup(t);
        if(!Map.ContainsKey(t.X)) Map.Add(t.X,new Dictionary<int, TileController>());
        Map[t.X].Add(t.Y,r);
        return r;
    }
    
    public ActorController SpawnActor(ActorThing t)
    {
        ActorController r = Instantiate(God.Library.ActorPrefab);
        r.Setup(t);
        return r;
    }
    
    public TileController GetTile(int x, int y)
    {
        if (!Map.ContainsKey(x)) return null;
        return Map[x].TryGetValue(y, out TileController r) ? r : null;
    }

    public TileController GetTile(TileThing t)
    {
        return GetTile(t.X, t.Y);
    }

    public ActorController GetActor(ActorThing a)
    {
        return a.Body;
    }
}
