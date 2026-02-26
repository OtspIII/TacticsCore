using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCut : Cutscene
{
    ActorThing Who;
    
    public DeathCut(ActorThing who)
    {
        Type = Cutscenes.Death;
        Who = who;
    }
    
    public override IEnumerator Script()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.2f;
            Who.Body.transform.rotation = Quaternion.Euler(0,0,t*360);
            yield return null;
        }
        Who.Body.Destruct();
        End();
    }
}