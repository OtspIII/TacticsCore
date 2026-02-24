using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

///The class used to send messages between Things and generally shoot data around
public class EventInfo
{
    public EventTypes Type;
    public bool Abort=false;
    public Dictionary<string, float> Numbers = new Dictionary<string, float>();
    public Dictionary<string, string> Texts = new Dictionary<string, string>();
    public List<string> Bools = new List<string>();
    public Dictionary<string, ActorThing> Things = new Dictionary<string, ActorThing>();
    public Dictionary<string, Vector2Int> Vectors = new Dictionary<string, Vector2Int>();
    public Dictionary<string, Traits> TraitI = new Dictionary<string, Traits>();

    public EventInfo(){ }
    
    public EventInfo(EventTypes t)
    {
        Type = t;
    }

    public EventInfo(float n)
    {
        Numbers.Add("",n);
    }
    
    public EventInfo(EventInfo i){ Clone(i); }

    public virtual void Clone(EventInfo i)
    {
        if (i == null) return;
        Type = i.Type;
        foreach(string n in i.Numbers.Keys) Numbers.Add(n,i.Numbers[n]);
        foreach(string n in i.Texts.Keys) Texts.Add(n,i.Texts[n]);
        foreach(string n in i.Bools) Bools.Add(n);
        foreach(string n in i.Things.Keys) Things.Add(n,i.Things[n]);
        foreach(string n in i.Vectors.Keys) Vectors.Add(n,i.Vectors[n]);
        foreach(string n in i.TraitI.Keys) TraitI.Add(n,i.TraitI[n]);
    }
    
    //Numbers
    public EventInfo Set(string i, float f)
    {
        return SetFloat(i, f);
    }
    public EventInfo Set(float f)
    {
        return SetFloat("", f);
    }
    public EventInfo SetFloat(string i, float f)
    {
        if (!Numbers.TryAdd(i,f)) Numbers[i]=f;
        return this;
    }
    public EventInfo SetInt(string i, int f)
    {
        if (!Numbers.TryAdd(i,f)) Numbers[i]=f;
        return this;
    }
    public float Get(string i,float def)
    {
        return GetFloat(i,def);
    }
    public float GetN(string i="",float def=0)
    {
        return GetFloat(i,def);
    }
    public float GetFloat(string i="",float def=0)
    {
        if (Numbers.TryGetValue(i, out float r)) return r;
        return def;
    }
    public int GetInt(string i="",int def=0)
    {
        if (Numbers.TryGetValue(i, out float r)) return (int)r;
        return def;
    }
    public float Change(float f)
    {
        return Change("", f);
    }
    public float Change(string i, float f)
    {
        float r = GetN(i);
        r += f;
        Set(i, r);
        return r;
    }
    
    //Text
    public EventInfo Set(string s)
    {
        return SetString("", s);
    }
    public EventInfo Set(string i, string s)
    {
        return SetString(i, s);
    }
    public EventInfo SetString(string i, string s)
    {
        if (!Texts.TryAdd(i,s)) Texts[i]=s;
        return this;
    }
    public string GetString(string i="")
    {
        if (Texts.TryGetValue(i, out string r)) return r;
        return "";
    }
    
    
    public EventInfo Set(Traits v)
    {
        Set("", v);
        return this;
    }
    public EventInfo Set(string s,Traits v)
    {
        TraitI.Add(s,v);
        return this;
    }

    public Traits GetTrait(string i="")
    {
        if (TraitI.TryGetValue(i, out Traits r)) return r;
        return Traits.None;
    }
    
    //Bools
    public EventInfo Set(bool v=true)
    {
        return SetBool("", v);
    }
    public EventInfo Set(string i, bool v=true)
    {
        return SetBool(i, v);
    }
    public EventInfo SetBool(string i, bool v=true)
    {
        bool has = Bools.Contains(i); 
        if(v && !has) Bools.Add(i);
        else if (!v && has) Bools.Remove(i);
        return this;
    }
    public bool GetBool(string b="")
    {
        return Bools.Contains(b);
    }
    
    //Thing
    public EventInfo Set(ActorController a)
    {
        return SetActor("", a != null ? a.Info : null);
    }
    public EventInfo Set(ActorThing a)
    {
        return SetActor("", a);
    }
    public EventInfo Set(string i, ActorThing a)
    {
        return SetActor(i, a);
    }
    public EventInfo SetActor(string i, ActorThing a)
    {
        if (!Things.TryAdd(i,a)) Things[i]=a;
        return this;
    }
    public ActorThing GetActor(string i="")
    {
        if (Things.TryGetValue(i, out ActorThing r)) return r;
        return null;
    }
    
    
    //Vector
    public EventInfo Set(Vector2Int a)
    {
        return SetVector("", a);
    }
    public EventInfo Set(string i, Vector2Int a)
    {
        return SetVector(i, a);
    }
    public EventInfo SetVector(string i, Vector2Int a)
    {
        if (!Vectors.TryAdd(i,a)) Vectors[i]=a;
        return this;
    }
    public Vector2Int Get(string i)
    {
        return GetVector(i);
    }
    public Vector2Int GetVector(string i="")
    {
        if (Vectors.TryGetValue(i, out Vector2Int r)) return r;
        return Vector2Int.zero;
    }
    
    public override string ToString()
    {
        return "[EVENT:" + Type + "]("+BuildString()+")";
    }

    public virtual string BuildString()
    {
        string r = "";
        if (Abort) r += "##ABORT##";
        foreach (string l in Numbers.Keys) r = God.AddList(r, l+"<"+Numbers[l].ToString()+">");
        foreach (string l in Texts.Keys) r = God.AddList(r, l+"<"+Texts[l].ToString()+">");
        foreach (string l in Bools) r = God.AddList(r, l.ToString());
        foreach (string l in Things.Keys) r = God.AddList(r, l+"<"+Things[l].ToString()+">");
        foreach (string l in Vectors.Keys) r = God.AddList(r, l+"<"+Vectors[l].ToString()+">");
        foreach (string l in TraitI.Keys) r = God.AddList(r, l+"<"+TraitI[l].ToString()+">");
        return r;
    }
    
    
}

public enum EventTypes{
    //Core
    None            =0000,
    Setup           =0001,
    
    StartTurn       =0100,
    
    GainTrait       =0210,
    LoseTrait       =0211,
    
    ActionStart     =1000,
    ActionPhaseEnd  =1001,
    ActionEnd       =1002,
    
}