using System.Collections;
using UnityEngine;

public class Cutscene
{

    public Cutscenes Type;
    public float Duration;
    public float Timer;

    public void Run()
    {
        if (Duration > 0)
        {
            Timer += Time.deltaTime;
            if (Timer >= Duration)
            {
                End();
                return;
            }
        }
        OnRun();
    }

    public virtual void OnRun()
    {

    }
    
    public void Begin()
    {
        God.GM.StartCoroutine(Script());
        OnBegin();
    }
    
    public virtual void OnBegin()
    {

    }

    public void End()
    {
        OnEnd();
        God.GM.EndCut(this);
    }
    
    public virtual void OnEnd()
    {

    }

    public virtual IEnumerator Script()
    {
        yield return null;
    }

    public virtual bool Merge(Cutscene c)
    {
        return false;
    }

    public float Perc()
    {
        if (Duration <= 0) return 0;
        return Timer / Duration;
    }

    public virtual void TileClick(GameTile t)
    {
        
    }
}

public enum Cutscenes
{
    None=0,
    Movement=1,
    Attack=2,
}