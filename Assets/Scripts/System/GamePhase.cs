using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseScript
{
    public Phases Type;
    public float Duration;
    public float Timer;
    public List<EventTypes> Listeners = new List<EventTypes>();
    public Dictionary<TileTints,TileTint> Tints = new Dictionary<TileTints, TileTint>();

    public void Run()
    {
        if (Duration > 0)
        {
            Timer+=Time.deltaTime;
            if (Timer >= Duration)
            {
                God.GM.StartPhase(NextPhase());
                return;
            }
        }
        OnRun();
    }
    
    public virtual void OnRun()
    {
        
    }

    public virtual void Begin()
    {
        
    }
    
    public virtual void Resume()
    {
        
    }

    public virtual void End()
    {
        
    }
    

    public virtual PhaseScript NextPhase()
    {
        return null;
    }
    
    public virtual IEnumerator Script()
    {
        yield return null;
    }
    
    public float Perc()
    {
        if (Duration <= 0) return 0;
        return Timer / Duration;
    }

    public virtual void TileClick(GameTile t)
    {
        
    }
    
    public virtual void TakeEvent(EventInfo e)
    {
        
    }

    
    public virtual void WipeTint(TileTints type=TileTints.None)
    {
        if (type != TileTints.None)
        {
            if (Tints.ContainsKey(type))
            {
                Tints[type].End();
                Tints.Remove(type);
            }
            return;
        }
        foreach (TileTint t in Tints.Values) t.End();
        Tints.Clear();
    }
    public void SetTint(TileTints t, params GameTile[] tiles)
    {
        TileTint tt = new TileTint(t, tiles);
        if (Tints.ContainsKey(t))
        {
            Tints[t] = tt;
            return;
        }
        Tints.Add(t,tt);
    }
    public void SetTint(TileTints t, List<GameTile> tiles)
    {
        TileTint tt = new TileTint(t, tiles);
        if (Tints.ContainsKey(t))
        {
            Tints[t] = tt;
            return;
        }
        Tints.Add(t,tt);
    }
}

public enum Phases
{
    None=0,
    BuildLevel=1,
    PlayerTurn=2,
    EnemyTurn=3,
    Environment=4,
}
