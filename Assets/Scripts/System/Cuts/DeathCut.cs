using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCut : Cutscene
{
    ActorThing Who;
    bool TrueDeath;
    
    public DeathCut(ActorThing who,bool trueDeath)
    {
        Type = Cutscenes.Death;
        Who = who;
        TrueDeath = trueDeath;
    }
    
    public override IEnumerator Script()
    {
        if (Who?.Body == null )
        {
            God.LogError("TRIED TO ATTACK ANIM WITH NULL ACTOR: " + Who);
            End();
            yield break;
        }
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.2f;
            float rot = TrueDeath ? t * 360 : t * 180;
            Who.Body.transform.rotation = Quaternion.Euler(0,0,rot);
            yield return null;
        }

        if (TrueDeath)
            Who.Body.Destruct();
        else
            Who.Body.FakeDie();
        End();
    }
}