using System.Collections.Generic;
using UnityEngine;

public class TraitInfo : EventInfo
{
    public Traits Trait;
    public ActorThing Who;

    public TraitInfo(Traits t, ActorThing who, EventInfo i)
    {
        Clone(i);
        Trait = t;
        Who= who;
    }

    public void Init() { Parser.Get(Trait).Init(this); }
    public void ReUp(EventInfo i) { Parser.Get(Trait).ReUp(this,i); }
    public void Remove(EventInfo e) { Parser.Get(Trait).Remove(this,e); }
    public void PreEvent(EventInfo e) { Parser.Get(Trait).PreEvent(this,e); }
    public void TakeEvent(EventInfo e) { Parser.Get(Trait).TakeEvent(this,e); }

    public override string ToString()
    {
        return Trait + "["+BuildString()+"]";
    }
}

public class TraitThing
{
    public Traits Type;
    public Dictionary<EventTypes,float> PreListen = new Dictionary<EventTypes,float>();
    public Dictionary<EventTypes,float> TakeListen = new Dictionary<EventTypes,float>();
    public Dictionary<EventTypes,float> Audit = new Dictionary<EventTypes,float>();

    public void Init(TraitInfo i)
    {
        foreach (EventTypes e in PreListen.Keys)
            i.Who.AddListen(e, Type, true);
        foreach (EventTypes e in TakeListen.Keys)
            i.Who.AddListen(e, Type, false);
    }

    public void Init(ActionScript a)
    {
        foreach (EventTypes e in TakeListen.Keys)
            a.AddListen(e, Type);
        foreach (EventTypes e in Audit.Keys)
            a.AddListen(e, Type,true);
    }


    public virtual void ReUp(TraitInfo old,EventInfo n)
    {
        //Called when you gain a trait when you already had it
    }
    
    public void Remove(TraitInfo i,EventInfo n)
    {
        //Called when you lose a trait
        foreach (EventTypes e in PreListen.Keys)
            i.Who.RemoveListen(e,Type,true);
        foreach (EventTypes e in TakeListen.Keys)
            i.Who.RemoveListen(e,Type,false);
        i.Who.Trait.Remove(Type);
        OnRemove(i,n);
    }
    
    protected virtual void OnRemove(TraitInfo i,EventInfo n)
    {
        //Called when a trait first gets added to an actor
    }
    
    public virtual void PreEvent(TraitInfo i, EventInfo e)
    {
        //Called when an event hits an actor, runs before TakeEvent
        //Use this for things like elemental vulnerabilities--things that
        // mod the result, but don't do any effects just yet
    }
    
    public virtual void TakeEvent(TraitInfo i, EventInfo e)
    {
        
    }
    
    public virtual void TakeEvent(ActionScript a, EventInfo e)
    {
        
    }
    
    public void AddListen(EventTypes e, float prio = 3)
    {
        TakeListen.Add(e,prio);
    }
    public void AddPreListen(EventTypes e, float prio = 3)
    {
        PreListen.Add(e,prio);
    }
}
