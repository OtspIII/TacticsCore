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
    public Dictionary<string, Number> Numbers = new Dictionary<string, Number>();
    public Dictionary<string, float> Floats = new Dictionary<string, float>();
    // public Dictionary<string, Number> Ns = new Dictionary<string, Number>();
    public Dictionary<string, string> Texts = new Dictionary<string, string>();
    public List<string> Bools = new List<string>();
    public Dictionary<string, ActorThing> Actors = new Dictionary<string, ActorThing>();
    public Dictionary<string, Thing> Things = new Dictionary<string, Thing>();
    public Dictionary<string, GameTile> Tiles = new Dictionary<string, GameTile>();
    public Dictionary<string, Vector2Int> Vectors = new Dictionary<string, Vector2Int>();
    public Dictionary<string, Traits> TraitI = new Dictionary<string, Traits>();
    public Dictionary<string, IntStats> Stats = new Dictionary<string, IntStats>();
    public Dictionary<string, DamageTypes> DTypes = new Dictionary<string, DamageTypes>();
    public Dictionary<string, CharClass> Classes = new Dictionary<string, CharClass>();
    public DieRoll DRoll;

    public EventInfo(){ }
    
    public EventInfo(EventTypes t)
    {
        Type = t;
    }

    public EventInfo(int n)
    {
        Numbers.Add("",God.N(n));
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
        foreach(string n in i.Actors.Keys) Actors.Add(n,i.Actors[n]);
        foreach(string n in i.Vectors.Keys) Vectors.Add(n,i.Vectors[n]);
        foreach(string n in i.TraitI.Keys) TraitI.Add(n,i.TraitI[n]);
        foreach(string n in i.Stats.Keys) Stats.Add(n,i.Stats[n]);
        foreach(string n in i.Tiles.Keys) Tiles.Add(n,i.Tiles[n]);
        foreach(string n in i.DTypes.Keys) DTypes.Add(n,i.DTypes[n]);
        foreach(string n in i.Floats.Keys) Floats.Add(n,i.Floats[n]);
        foreach(string n in i.Classes.Keys) Classes.Add(n,i.Classes[n]);
    }
    
    //Numbers
    public EventInfo Set(string i, int f)
    {
        return SetN(i, f);
    }
    public EventInfo Set(int f)
    {
        return SetN("", f);
    }
    public EventInfo Set(Number f)
    {
        return SetN("", f);
    }
    public EventInfo SetN(string i, int f)
    {
        SetN(i, God.N(f));
        return this;
    }
    public EventInfo SetN(string i, Number f)
    {
        if (!Numbers.TryAdd(i,f)) Numbers[i]=f;
        return this;
    }
    // public EventInfo SetFloat(string i, float f)
    // {
    //     if (!Numbers.TryAdd(i,f)) Numbers[i]=f;
    //     return this;
    // }
    // public EventInfo SetInt(string i, int f)
    // {
    //     if (!Numbers.TryAdd(i,f)) Numbers[i]=f;
    //     return this;
    // 
    public Number GetN(string i="",int def=0)
    {
        return GetN(i,new Number(def));
    }
    public Number GetN(string i,Number def)
    {
        if (Numbers.TryGetValue(i, out Number r)) return r;
        return def != null ? def : new Number(0);
    }
    
    public int GetInt(string i="",int def=0,ActorThing who=null)
    {
        if (Numbers.TryGetValue(i, out Number r)) return r.V(who);
        return def;
    }
    public Number Change(int f)
    {
        return Change("", f);
    }
    public Number Change(string i, int f)
    {
        Number r = GetN(i);
        r.N += f;
        return r;
    }
    
    //Text
    public EventInfo Roll(string s)
    {
        return SetString("Roll", s);
    }
    public EventInfo Resist(string s)
    {
        return SetString("Resist", s);
    }
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
    
    public EventInfo SetF(float s)
    {
        return SetF("", s);
    }
    public EventInfo Set(string i, float s)
    {
        return SetF(i, s);
    }
    public EventInfo SetF(string i, float s)
    {
        if (!Floats.TryAdd(i,s)) Floats[i]=s;
        return this;
    }

    public float GetF(string i="",float def=0)
    {
        if (Floats.TryGetValue(i, out float r)) return r;
        return def;
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
    
    //Actors
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
        if (!Actors.TryAdd(i,a)) Actors[i]=a;
        return this;
    }
    public ActorThing GetActor(string i="")
    {
        if (Actors.TryGetValue(i, out ActorThing r)) return r;
        return null;
    }
    
    //Things
    public EventInfo Set(Thing a)
    {
        return SetThing("", a);
    }
    public EventInfo Set(string i, Thing a)
    {
        return SetThing(i, a);
    }
    public EventInfo SetThing(string i, Thing a)
    {
        if (!Things.TryAdd(i,a)) Things[i]=a;
        return this;
    }
    public Thing GetThing(string i="")
    {
        if (Things.TryGetValue(i, out Thing r)) return r;
        return null;
    }
    
    //Tiles
    public EventInfo Set(GameTile a)
    {
        return SetTile("", a);
    }
    public EventInfo Set(string i, GameTile a)
    {
        return SetTile(i, a);
    }
    public EventInfo SetTile(string i, GameTile a)
    {
        if (!Tiles.TryAdd(i,a)) Tiles[i]=a;
        return this;
    }
    public GameTile GetTile(string i="")
    {
        if (Tiles.TryGetValue(i, out GameTile r)) return r;
        return null;
    }
    
    //Classes
    public EventInfo Set(CharClass a)
    {
        return SetClass("", a);
    }
    public EventInfo Set(string i, CharClass a)
    {
        return SetClass(i, a);
    }
    public EventInfo SetClass(string i, CharClass a)
    {
        if (!Classes.TryAdd(i,a)) Classes[i]=a;
        return this;
    }
    public CharClass GetClass(string i="")
    {
        return Classes.GetValueOrDefault(i, CharClass.None);
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
    
    //Stats
    public EventInfo Set(IntStats s)
    {
        return SetStat("", s);
    }
    public EventInfo Set(string i, IntStats s)
    {
        return SetStat(i, s);
    }
    public EventInfo SetStat(string i, IntStats s)
    {
        if (!Stats.TryAdd(i,s)) Stats[i]=s;
        return this;
    }
    public IntStats GetStat(string i="")
    {
        if (Stats.TryGetValue(i, out IntStats r)) return r;
        return IntStats.None;
    }
    
    //DType
    public EventInfo Set(DamageTypes s)
    {
        return SetDType("", s);
    }
    public EventInfo Set(string i, DamageTypes s)
    {
        return SetDType(i, s);
    }
    public EventInfo SetDType(string i, DamageTypes s)
    {
        if (!DTypes.TryAdd(i,s)) DTypes[i]=s;
        return this;
    }
    public DamageTypes GetDType(string i="")
    {
        if (DTypes.TryGetValue(i, out DamageTypes r)) return r;
        return DamageTypes.None;
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
    ChangeStat      =0220,
    TempDefense     =0221,
    
    ActionStart     =1000,
    NewPhase  =1001,
    ActionEnd       =1002,
    PostAudit       =1003,
    
    Damage          =2000,
    Death           =2001,
    TrueDeath       =2002,
    Heal            =2010,
    Knockback       =2100,
    ProvokeAoO      =2101, //##
    
    WalkTo          =3000,
    LeaveTile       =3001,
    ArriveTile      =3002,
    Summon          =3100,
    
    CanAct          =4000,
    
    ActionValue     =5000,
    
    SelectCard      =9000,
    EndTurn         =9001,
    BecomeIncap     =9002,
}