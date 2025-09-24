using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseScript
{
    public Phases Type;
    public float Duration;
    public float Timer;

    public void Run()
    {
        if (Duration > 0)
        {
            Timer+=Time.deltaTime;
            if(Timer >= Duration)
                God.GM.StartPhase(NextPhase());
        }
    }
    
    public virtual void OnRun()
    {
        
    }

    public virtual void Begin()
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
}

public enum Phases
{
    None=0,
    BuildLevel=1,
    PlayerTurn=2,
}