using System.Collections.Generic;
using UnityEngine;

public class GameTile : Thing
{
    public int X { get { return Location.x; } }
    public int Y { get { return Location.y; } }
    public Vector2Int Location;
    public ActorThing Contents;
    public TileController Body;
    public List<TileTint> Tints = new  List<TileTint>();
    public Dictionary<ActorThing,int> PDistance = new Dictionary<ActorThing, int>();
    public int BestPDistance = 0;

    public GameTile(int x, int y)
    {
        Location = new Vector2Int(x, y);
    }
    
    public GameTile Neighbor(int x, int y)
    {
        return God.GM.Level.GetTile(X + x, Y + y);
    }
    
    public GameTile Neighbor(Vector2Int d)
    {
        return God.GM.Level.GetTile(X + d.x, Y + d.y);
    }
    
    public GameTile Neighbor(Directions d)
    {
        Vector2Int dir = God.Dir2Vector2Int(d);
        return Neighbor(dir.x, dir.y);
    }

    public Color GetTint()
    {
        Color r = Color.white;
        int best = 0;
        foreach (TileTint t in Tints)
        {
            if(t.Priority < best) continue;
            r = t.GetColor();
            best = t.Priority;
        }
        return r;
    }

    public void AddTint(TileTint t)
    {
        Tints.Add(t);
        Body.SR.color = GetTint();
    }
    
    public void RemoveTint(TileTint t)
    {
        Tints.Remove(t);
        Body.SR.color = GetTint();
    }

    public void WipeTint()
    {
        Tints.Clear();
        Body.SR.color = Color.white;
    }
    
    public List<GameTile> Neighbors(NeighborMode nm=NeighborMode.None,ActorThing who=null,bool eightDir=false) //NeighborMode m=0,ActorThing who=null,
    {
        List<GameTile> r = new List<GameTile> ();
        foreach (Directions p in eightDir ? God.EightDir : God.Cardinal) {
            GameTile n = Neighbor (p);
            if (ValidNeighbor(n,nm))
                r.Add (n);
        }
        return r;
    }
    
    public List<GameTile> Flood(int dist,NeighborMode nm=NeighborMode.None,ActorThing who=null,bool eightDir=false) //NeighborMode m=0,ActorThing who=null,
    {
        List<GameTile> r = new List<GameTile> ();
        List<GameTile> active = new List<GameTile> (){this};
        for (int n = 0; n < dist; n++)
        {
            List<GameTile> current = new List<GameTile>();
            current.AddRange(active);
            r.AddRange(active);
            active.Clear();
            foreach (GameTile t in current)
            {
                List<GameTile> maybe = t.Neighbors(nm,who,eightDir);
                foreach (GameTile m in maybe)
                {
                    if (!r.Contains(m) && !active.Contains(m))
                    {
                        active.Add(m);
                    }
                }
            }
        }
        r.AddRange(active);
        if (nm == NeighborMode.Walking) r.Remove(this);
        return r;
    }

    public bool ValidNeighbor(GameTile t,NeighborMode nm=NeighborMode.None)
    {
        if (t == null) return false;
        switch (nm)
        {
            case NeighborMode.Walking: return t.Contents == null;
        }
        return true;
    }

    public void TakeEvent(EventInfo e,ActEventTarget t,ActorThing src,ActionScript act)
    {
        switch (e.Type)
        {
            case EventTypes.Summon:
            {
                if (Contents != null)
                {
                    God.LogWarning("TRIED TO SUMMON INTO NON-EMPTY TILE: " + Location + " / " + e + " / " + src + " / " + act);
                    break;
                }
                CharClass cc = e.GetClass();
                if (cc != CharClass.None)
                {
                    God.GM.Summon(cc,this,e);    
                }
                break;
            }
        }
        // if (Contents != null)
        // {
        //     EventInfo ee = new EventInfo();
        //     ee.Clone(e);
        //     Contents.TakeEvent(ee);
        // }
    }

    public Thing GetThing()
    {
        if (Contents != null) return Contents;
        return null;
    }
}

public enum NeighborMode
{
    None=0,
    Vision=1,
    Walking=2,
    AIPath=3,
    AIPathLong=4,
    WallsBlock=5,
    Targeting=6,
}