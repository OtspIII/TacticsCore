using System.Collections.Generic;
using UnityEngine;

public class StunTrait : TraitThing
{
    public StunTrait()
    {
        Type = Traits.Stunned;
        AddListen(EventTypes.StartTurn,5);
        AddListen(EventTypes.CanAct);
    }

    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.StartTurn:
            {
                i.Who.ActionsLeft.Clear();
                break;
            }
            case EventTypes.CanAct:
            {
                e.Set(false);
                break;
            }
        }
    }

    public override void OnAdd(TraitInfo i, EventInfo e = null)
    {
        i.Who.ActionsLeft.Clear();
        God.GM.TakeEvent(God.E(EventTypes.BecomeIncap).Set(i.Who));
    }
}

public class TauntedTrait : TraitThing
{
    public TauntedTrait()
    {
        Type = Traits.Taunted;
        AddListen(EventTypes.ActionValue);
    }

    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.ActionValue:
            {
                ActorThing targ = i.GetActor("Target");
                if (targ == null)
                {
                    God.LogWarning("TAUNTED BUT WITHOUT A TARGET: " + i.Who);
                    break;
                }
                GameTile t = e.GetTile();
                float r = e.GetF();
                float mod = e.GetF("Mod",0);
                ActionScript main = e.GetThing("Main Action") as ActionScript;
                ActionScript act = e.GetThing() as ActionScript;
                if (main != null)
                {
                    int dist = t.PDistance.ContainsKey(targ) ? t.PDistance[targ] : t.BestPDistance;
                    r = 10 - Mathf.Abs(dist-main.Range);
                }
                else 
                {
                    if (t == targ.Location) r += 10;
                }
                e.SetF(r);
                e.SetF("Mod",mod);
                break;
            }
        }
    }
    
    public override void OnAdd(TraitInfo i, EventInfo e = null)
    {
        // Debug.Log("T ONADD: " + e);
        if (e == null) return;
        i.Set("Target", e.GetActor("Source"));
        // Debug.Log("SOURCE: " + e.GetActor("Source"));
    }

    public override void ReUp(TraitInfo i, EventInfo e)
    {
        if (e == null) return;
        i.Set("Target", e.GetActor("Source"));
    }


}