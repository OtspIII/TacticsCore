using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCut : Cutscene
{
    ActorThing Src;
    GameTile Targ;
    
    public AttackCut(ActorThing src,GameTile targ)
    {
        Type = Cutscenes.Attack;
        Src = src;
        Targ = targ;
    }
    
    public override IEnumerator Script()
    {
        Vector3 s = Src.Location.Body.GetContentPos(Src);
        Vector3 e = Targ.Body.GetContentPos(null);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.2f;
            Vector3 p = Vector3.Lerp(s, e, t);
            Src.Body.transform.position = p;
            yield return null;
        }
        while (t > 0)
        {
            t -= Time.deltaTime / 0.2f;
            Vector3 p = Vector3.Lerp(s, e, t);
            Src.Body.transform.position = p;
            yield return null;
        }
        Src.Body.transform.position = s;
        End();
    }
}