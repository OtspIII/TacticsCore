using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCut : Cutscene
{
    ActorThing A;
    GameTile Old;
    GameTile New;
    public List<MiniMoveCut> Moves = new List<MiniMoveCut>();
    
    public MoveCut(ActorThing a,GameTile old,GameTile n)
    {
        Type = Cutscenes.Movement;
        A = a;
        Old = old;
        New = n;
        Moves.Add(new MiniMoveCut(a,old,n));
    }
    
    public override IEnumerator Script()
    {
        foreach (MiniMoveCut mmc in Moves)
        {
            Vector3 s = mmc.Old.Body.GetContentPos(mmc.A);
            Vector3 e = mmc.New.Body.GetContentPos(mmc.A);
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / GetSpeed();
                Vector3 p = Vector3.Lerp(s, e, t);
                mmc.A.Body.transform.position = p;
                yield return null;
            }
            mmc.A.Body.transform.position = e;
        }
        End();
    }

    public float GetSpeed()
    {
        return 0.5f;
    }

    public override bool Merge(Cutscene c)
    {
        if (c.Type == Cutscenes.Movement)
        {
            MoveCut mc = (MoveCut)c;
            Moves.AddRange(mc.Moves);
            return true;
        }
        return false;
    }

    public class MiniMoveCut
    {
        public ActorThing A;
        public GameTile Old;
        public GameTile New;

        public MiniMoveCut(ActorThing a,GameTile old,GameTile n)
        {
            A = a;
            Old = old;
            New = n;
        }
    }
}