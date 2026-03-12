using System.Collections.Generic;
using UnityEngine;

public class WalkPath
{
    public ActorThing Who;
    public GameTile Dest;
    public GameTile Start;
    public int Range;
    public List<GameTile> Path = new List<GameTile>();
    public NeighborMode NMode;

    public WalkPath(ActorThing who, GameTile dest, bool teleport=false,int range=9999)
    {
        Who = who;
        Start = Who.Location;
        Dest = dest;
        Range = range;
        NMode = teleport ? NeighborMode.None : NeighborMode.Walking;
        Calc();
    }

    public void Calc()
    {
        WalkTile dest = null;
        Dictionary<GameTile,WalkTile> done = new Dictionary<GameTile,WalkTile>();
        Dictionary<GameTile,WalkTile> active = new Dictionary<GameTile,WalkTile>();
        foreach (GameTile t in Start.Neighbors(NMode, Who))
        {
            if (t == Dest)
            {
                if(t.Contents == null) Path.Add(t);
                return;
            }
            active.Add(t,new WalkTile(t,this,null));
        }

        int safety = 9999;
        while (dest == null && active.Count > 0 && safety > 0)
        {
            safety--;
            WalkTile b = GetBest(active);
            active.Remove(b.Tile);
            done.Add(b.Tile,b);
            foreach (GameTile t in b.Tile.Neighbors())
            {
                bool valid = b.Tile.ValidNeighbor(t, NMode,Who);
                if (t == Dest)
                {
                    dest = valid && t.Contents == null ? new WalkTile(t, this, b) : b;
                    break;
                }
                if (!valid) continue;
                if (done.ContainsKey(t))
                {
                    // WalkTile  w = done[t];
                    // done.Remove(t);
                    done[t].SetFrom(b);
                    // active.Add(w.Tile,w);
                }
                else if (active.ContainsKey(t))
                {
                    active[t].SetFrom(b);
                }
                else
                {
                    WalkTile w = new WalkTile(t, this, b);
                    active.Add(w.Tile,w);
                }
            }
        }

        if (dest == null)
        {
            return;
        }
        WalkTile wt = dest;
        int safe2 = 999;
        while (safe2 > 0)
        {
            safe2--;
            Path.Insert(0, wt.Tile);
            wt = wt.FromSafe;
            if (wt == null) break;
        }
    }

    public WalkTile GetBest(Dictionary<GameTile,WalkTile> active)
    {
        int best = 9999;
        WalkTile b = null;
        foreach (WalkTile t in active.Values)
        {
            if (t.Dist < best)
            {
                b = t;
                best = t.Dist;
            }
        }
        return b;
    }

}

public class WalkTile
{
    public WalkPath Path; 
    public GameTile Tile;
    public int Dist;
    public int Cost = 1;
    public int SafeSteps=9999;
    public WalkTile FromSafe=null;
    // public float UnsafeSteps;
    // public WalkTile FromUnsafe;
    public List<WalkTile> Buds =  new List<WalkTile>();

    public WalkTile(GameTile t,WalkPath p,WalkTile from)
    {
        Tile = t;
        Path = p;
        Vector2Int dist = Tile.Location - p.Dest.Location;
        Dist = Mathf.Abs(dist.x) + Mathf.Abs(dist.y);
        Cost = Tile.GetCost(p.Who);
        // if (t.Contents != null) DontStop = true;
        SetFrom(from);
    }

    public void SetFrom(WalkTile t,int safety=999)
    {
        if (t == null)
        {
            SafeSteps = Cost;
            return;
        }
        safety--;
        if (safety <= 0)
        {
            God.LogError("INFINITE SETFROM LOOP");
            return;
        }
        if (t.SafeSteps + Cost >= SafeSteps) return;
        SafeSteps = t.SafeSteps + Cost;
        if (FromSafe != t)
        {
            if (FromSafe != null) FromSafe.Buds.Remove(this);
            FromSafe = t;
            t.Buds.Add(this);
        }
        foreach (WalkTile b in Buds)
        {
            b.SetFrom(this,safety);
        }
    }
}