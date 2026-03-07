using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActorThing : Thing
{
    public Actors Type;
    public string Name;
    public CharClass Class = CharClass.None;
    public GameTile Location;
    public ActorController Body;
    public GameTeam Team = GameTeam.None;
    
    //These variables handle the Trait subsystem within a Thing
    //Connects enums to the trait's state variables
    public Dictionary<Traits, TraitInfo> Trait = new Dictionary<Traits, TraitInfo>(); 
    //Traits can register the types of events they listen for. PreListen runs first, then TakeListen
    public Dictionary<EventTypes, List<Traits>> PreListen = new Dictionary<EventTypes, List<Traits>>();
    public Dictionary<EventTypes, List<Traits>> TakeListen = new Dictionary<EventTypes, List<Traits>>();
    //Sometimes Events cause more events. Those get added to a queue, rather than being resolved instantly by default
    public List<EventInfo> EventQueue = new List<EventInfo>();
    //A variable that marks if we're in the middle of resolving an event (ie-if a new event should go in the queue or not)
    [HideInInspector]public bool MidEvent = false;

    //These variables are just used for internal bookkeeping
    [HideInInspector]public bool IsSetup = false;     //Have I run setup already? Prevents setup from running twice
    [HideInInspector]public bool Destroyed = false; //Have I been marked for destruction? So I don't trigger death effects multiple times

    public List<ActionCost> ActionsLeft = new List<ActionCost>();
    public ActionScript SelectedAction;
    
    // public ActionScript MoveAction =  new ActionScript();
    public Dictionary<ActionSlot,ActionScript> KnownActions = new  Dictionary<ActionSlot,ActionScript>();
    public Dictionary<IntStats, int> Stats = new Dictionary<IntStats, int>();
    public Dictionary<IntStats, List<StatMod>> StatMods = new Dictionary<IntStats, List<StatMod>>();
    public Dictionary<StrStats, string> TxtStats = new Dictionary<StrStats, string>();
    public DieRoll BaseDamage;
    public List<CTags> Tags = new List<CTags>();
    
    public ActorThing(Actors type,GameTile l)
    {
        Type = type;
        Setup(l);
    }
    
    public ActorThing(CharClass type,GameTile l)
    {
        Type = Actors.Character;
        Class = type;
        Setup(l);
    }

    private void Setup(GameTile l)
    {
        ActorPrefab pre;
        if (Class != CharClass.None) pre = ThingBuilder.ClassDict[Class];
        else pre = ThingBuilder.ActorDict[Type];
        Team = pre.Team;
        Name = pre.Name;
        foreach (TraitBuilder t in pre.TraitList)
        {
            AddTrait(t.Type, t.E);
        }
        foreach (IntStats s in pre.Stats.Keys)
        {
            Stats.Add(s,pre.Stats[s]);
            if(s == IntStats.HP) 
                Stats.Add(IntStats.MaxHP,pre.Stats[s]);
        }
        foreach (StrStats s in pre.TxtStats.Keys)
        {
            TxtStats.Add(s,pre.TxtStats[s]);
            if (s == StrStats.Damage)
                BaseDamage = new DieRoll(pre.TxtStats[s]);
        }
        TakeEvent(EventTypes.Setup);
        foreach(Actions a in pre.KnownActions) AddAction(a);
        if(!KnownActions.ContainsKey(ActionSlot.BasicMove)) AddAction(Actions.Walk);
        if(!KnownActions.ContainsKey(ActionSlot.BasicAttack)) AddAction(Actions.BasicAttack);
        // if(Actions.Count == 0) Actions.Add(new AttackAction(this));//Placeholder!
        SetLocation(l);
    }

    public void AddAction(Actions a)
    {
        ActionScript act = ThingBuilder.MakeAction(a);
        act.Who = this;
        if(KnownActions.ContainsKey(act.Slot)) KnownActions[act.Slot] = act;
        else KnownActions.Add(act.Slot, act);
    }

    public void SetLocation(GameTile l)
    {
        if (Location != null)
        {
            Location.Contents = null;
        }
        Location = l;
        if (l == null) return;
        l.Contents = this;
    }
    
    ///Adds a new trait to the Thing
    public TraitInfo AddTrait(Traits t,EventInfo i=null,EventInfo e=null)
    {
        //If we add a trait mid-game it throws an event to let other traits know
        if (IsSetup)
        {
            //We can include an EventInfo that tells us more about how we got the trait
            //But if we didn't make a blank one
            if (e == null) e = God.E();
            e.Type = EventTypes.GainTrait;
            e.Set(t);
            //Pass that event along to all our other traits
            TakeEvent(e, true);
            //If any of those events yelled 'Abort', we're immune to the trait and shouldn't gain it
            if (e.Abort) return null;
        }
        //Get our info already assodiated with the trait, if we have any
        TraitInfo r = Get(t);
        //If we found some, it means we're re-upping an existing trait, not gaining a new one
        //Example: we got hit with an attack that sets us on fire, but we're already on fire
        if (r != null)
        {
            //Tell the trait to run its 'ReUp' code
            r.ReUp(i);
        }
        else //Otherwise, we're gaining a new trait
        {
            //Make new trait info and add it to us/set it up
            r = new TraitInfo(t, this, i);
            Trait.Add(t,r);
            r.Init();
        }
        return r; //Return the trait info
    }
    
    ///Removes an existing trait from a thing
    public bool RemoveTrait(Traits t,EventInfo e=null)
    {
        TraitInfo r = Get(t);  //Get our existing trait info, if any exists
        if (r == null) return false; //If we don't find any, we don't have the trait so we can abort
        //If this is happening mid-game, we shoot an event to our traits about it
        if (IsSetup)
        {
            if (e == null) e = God.E();
            e.Type = EventTypes.LoseTrait;
            e.Set(t);
            TakeEvent(e, true);
            if (e.Abort) return false; //If any of them object, we don't lose the trait
        }
        r.Remove(e); //Tell the trait to remove itself
        return true;
    }
    
    ///Returns the trait info of a selected trait
    public TraitInfo Get(Traits t)
    {
        if (Trait.TryGetValue(t, out TraitInfo r)) return r;
        return null;
    }

    ///Returns true if a Thing has a trait, false otherwise
    public bool Has(Traits t)
    {
        return Trait.ContainsKey(t);
    }
    
    ///Adds a listener for a certain type of event, so that when you get the event you tell a specific trait about it
    ///There are two types of listeners: pre and normal. Pre runs first, and is for making changes to the event itself
    public void AddListen(EventTypes e, Traits t,bool pre = false)
    {
        //Which dictionary are we adding this to? Pre or normal listen?
        Dictionary<EventTypes, List<Traits>> d = pre ? PreListen : TakeListen;
        if(!d.ContainsKey(e)) d.Add(e,new List<Traits>()); //If the dictionary doesn't already have this event, add it
        if(!d[e].Contains(t)) d[e].Add(t);                 //If the event doesn't have this trait, add it
        //Listens have priority. If this happened mid-game, recalculate the order traits hear about the events in
        if(IsSetup) SortListen(e,pre);
    }

    ///Removes the listeners for an event for a trait
    public void RemoveListen(EventTypes e, Traits t, bool pre = false)
    {
        Dictionary<EventTypes, List<Traits>> d = pre ? PreListen : TakeListen;
        if (!d.ContainsKey(e)) return;
        List<Traits> l = d[e];
        l.Remove(t);
    }

    ///Traits can set what order they get events in
    /// Ie-Invincible should handle TakeDamage before Health, so it can negate the damage
    /// This function makes a list of them in order
    public void SortListen(EventTypes e, bool pre = false)
    {
        //#######BOOKMARK########
        List<Traits> l = pre ? PreListen[e] : TakeListen[e];
        if (l.Count <= 1) return;
        Dictionary<Traits, float> prio = new Dictionary<Traits, float>();
        foreach (Traits t in l)
        {
            TraitThing tr = Parser.Get(t);
            float pr = pre ? tr.PreListen[e] : tr.TakeListen[e];
            prio.Add(t,pr);
        }
        l.Sort((a, b) => { return prio[a] > prio[b] ? 1 : -1; });
    }
    
    public void TakeEvent(EventTypes e)
    {
        TakeEvent(new EventInfo(e));
    }
    public void TakeEvent(EventInfo e,bool instant=false,int safety=999)
    {
        // Debug.Log("TAKE EVENT B: " + e.Type + " / " + PreListen.Keys.Count + " / " + TakeListen.Keys.Count + " / " + Name);
        safety--;
        if (safety <= 0)
        {
            Debug.Log("INFINITE EVENT LOOP: " + e);
            return;
        }
        if (MidEvent && !instant)
        {
            EventQueue.Add(e);
            return;
        }
        e.Set("Who", this);
        MidEvent = true;
        PreListen.TryGetValue(e.Type, out List<Traits> pre);
        if(pre != null) {
            foreach (Traits t in pre)
            {
                Get(t).PreEvent(e);
                if (e.Abort) break;
            }
        }

        if (e.Abort) return;
        
        TakeListen.TryGetValue(e.Type, out List<Traits> take);
        // Debug.Log("TAKE EVENT C: " + e.Type + " / " + (take == null ? "X" : take.Count));
        if(take != null)
            foreach (Traits t in take)
            {
                Get(t).TakeEvent(e);
                if(e.Abort) break;
            }
        MidEvent = false;
        if (EventQueue.Count > 0)
        {
            EventInfo next = EventQueue[0];
            EventQueue.RemoveAt(0);
            TakeEvent(next,false,safety);
        }
    }

    public EventInfo Ask(EventTypes e)
    {
        EventInfo r = God.E(e);
        return Ask(r);
    }
    
    public EventInfo Ask(EventInfo e,int safety=99)
    {
        safety--;
        if (safety <= 0)
        {
            Debug.Log("INFINITE ASK LOOP: " + e);
            return e;
        }
        TakeEvent(e,true);
        return e;
    }

    public void Walk(GameTile t,bool useMove=true)
    {
        if (t == null)
        {
            God.LogError("Tried to walk to null tile: " + this);
            return;
        }
        GameTile o = Location;
        if (useMove)
        {
            int dist = Mathf.Abs(o.X - t.X) + Mathf.Abs(o.Y - t.Y);
            int left = Change(IntStats.MoveLeft, -dist);
            if (left <= 0) ActionsLeft.Remove(ActionCost.Move);
        }

        SetLocation(t);
        God.GM.AddCut(new MoveCut(this,o,t));
    }

    public void Destruct(bool silent=false)
    {
        Destroyed = true;
        Location.Contents = null;
        if(silent)
            Body.Destruct();
        else
            God.GM.AddCut(new DeathCut(this,true));
    }

    public override void ImprintCard(CardScript c)
    {
        Sprite s = null;
        string name = Type.ToString();
        string desc = "";
        if (Class != CharClass.None)
        {
            s = God.Library.GetPortrait(Class);
            name = Class.ToString();
        }

        int inj = Get(IntStats.Injury);
        desc += "HP: " + Get(IntStats.HP) + " / " + (Get(IntStats.MaxHP)-inj);
        if (inj > 0) desc += " (" + inj + ")";
        desc += "\nSpeed: " + Get(IntStats.Movespeed);
        c.Imprint(s,name,desc);
    }

    public int Get(IntStats k,bool raw=false,int def=0)
    {
        int r = Stats.TryGetValue(k, out int s) ? s : def;
        if (!raw && StatMods.TryGetValue(k,out List<StatMod> mods))
        {
            foreach (StatMod mod in mods)
                r += mod.Amount;
        }
        return r;
    }
    
    public string Get(StrStats k, string def = "")
    {
        return TxtStats.TryGetValue(k, out string r) ? r : def;
    }

    public void Set(IntStats k, int amt)
    {
        if (Stats.ContainsKey(k)) Stats[k] = amt;
        else Stats.Add(k,amt);
    }
    
    public void Set(StrStats k, string v)
    {
        if (TxtStats.ContainsKey(k)) TxtStats[k] = v;
        else TxtStats.Add(k,v);
    }
    
    public int Change(IntStats k, int amt,Vector2Int minmax=new Vector2Int())
    {
        if (Stats.ContainsKey(k)) Stats[k] += amt;
        else Stats.Add(k,amt);
        int r = Stats[k];
        if (minmax != Vector2Int.zero)
        {
            if (r < minmax.x)
            {
                r = minmax.x;
                Stats[k] = r;
            }
            else if (r > minmax.y)
            {
                r = minmax.y;
                Stats[k] = r;
            }
        }
        return r;
    }

    public int AddMod(StatMod s,TraitInfo t=null,int dur=-1)
    {
        if(!StatMods.ContainsKey(s.Stat))
            StatMods.Add(s.Stat,new List<StatMod>());
        StatMod actual = new StatMod(s, this, dur);
        StatMods[s.Stat].Add(actual);
        if(t != null) t.Mods.Add(actual);
        switch (s.Stat)
        {
            case IntStats.Movespeed: Change(IntStats.MoveLeft, s.Amount); break;
        }
        return Get(s.Stat);
    }

    public int RemoveMod(StatMod s)
    {
        if (!StatMods.ContainsKey(s.Stat)) return Get(s.Stat);
        StatMods[s.Stat].Remove(s);
        return Get(s.Stat);
    }

    public int GetMaxHP()
    {
        int r = Get(IntStats.MaxHP);
        int inj = Get(IntStats.Injury);
        return r - inj;
    }

    public override string ToString()
    {
        return "Actor[" + Class + "]";
    }

    public ActionScript GetAct(ActionSlot a)
    {
        return KnownActions.TryGetValue(a, out ActionScript r) ? r : null;
    }

    public bool IsEnemy(ActorThing a)
    {
        if (a == null) return false;
        if (Team == GameTeam.None || a.Team == GameTeam.None || Team == a.Team) return false;
        return true;
    }

    public bool Has(CTags t)
    {
        return Tags.Contains(t);
    }

    public bool Resist(string resist,ActorThing src=null,bool silent=false)
    {
        int hp = Get(IntStats.HP);
        DieRoll die = new DieRoll(resist, src);
        int roll = die.Roll();
        Debug.Log("ROLL RESIST: " + resist + " / " + roll + " / " + hp + " / " + die);
        if (hp > roll)
        {
            if(!silent) God.GM.AddCut(new HeadtextCut(this,"RESIST",Colors.Resist));
            return true;
        }

        return false;
    }
}


public enum GameTeam
{
    None=0,
    Player=1,
    Enemy=2,
    Berserk=3,
}

public class StatMod
{
    public ActorThing Who;
    public IntStats Stat;
    public int Amount;
    public int Duration=-1;

    public StatMod(IntStats s, int a,int dur=-1)
    {
        Stat = s;
        Amount = a;
        Duration = dur;
    }

    public StatMod(StatMod s,ActorThing who,int dur=-1)
    {
        Who = who;
        Stat = s.Stat;
        Amount = s.Amount;
        Duration = dur != -1 ? dur : s.Duration;
    }
}