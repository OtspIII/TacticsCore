using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UniversalTrait : TraitThing
{
    public UniversalTrait()
    {
        Type = Traits.Universal;
        AddListen(EventTypes.StartTurn,4);
        AddListen(EventTypes.GainTrait);
        AddListen(EventTypes.LoseTrait);
    }

    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.StartTurn:
            {
                foreach (IntStats st in i.Who.StatMods.Keys)
                {
                    foreach (StatMod m in i.Who.StatMods[st].ToArray())
                    {
                        if (m.Duration < 0) continue;
                        m.Duration--;
                        if (m.Duration <= 0)
                            i.Who.RemoveMod(m);
                    }
                }
                foreach (TraitInfo ti in i.Who.Trait.Values.ToArray())
                {
                    int dur = ti.GetInt("Duration", 0);
                    if(dur <= 0) continue;
                    int d = ti.Change("Duration", -1).V();
                    if (d <= 0)
                    {
                        God.GM.AddCut(Parser.Get(ti.Trait).LoseCut(ti,e,i.Who));
                        i.Who.RemoveTrait(ti.Trait);
                    }
                }
                
                break;
            }
            case EventTypes.GainTrait:
            {
                Traits tr = e.GetTrait();
                string res = e.GetString("Resist");
                int dur = e.GetInt("Duration", -1);
                int amt = e.GetInt();
                if (res != "" && i.Who.Resist(res, e.GetActor("Source"))) break;
                EventInfo seed = new EventInfo(amt).Set("Duration", dur);
                God.GM.AddCut(Parser.Get(tr).GainCut(seed,e,i.Who));
                i.Who.AddTrait(tr,seed,e);
                break;
            }
            case EventTypes.LoseTrait:
            {
                Traits tr = i.GetTrait();
                int dur = e.GetInt("Duration", 0);
                int amt = e.GetInt("",0);
                TraitInfo ti = i.Who.Get(tr);
                bool remove = true;
                if (dur > 0 || amt > 0)
                {
                    remove = false;
                    if (dur > 0 && ti.Change("Duration", -dur).V() <= 0) remove = true;
                    if (amt > 0 && ti.Change("", -amt).V() <= 0) remove = true;
                }
                God.GM.AddCut(Parser.Get(tr).LoseCut(ti,e,i.Who));
                if (remove) i.Who.RemoveTrait(tr);
                break;
            }
        }
    }
}

public class PlayerTrait : TraitThing
{
    public PlayerTrait()
    {
        Type = Traits.Player;
        // AddListen(EventTypes.StartTurn);
    }

    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        // switch (e.Type)
        // {
        //     case EventTypes.StartTurn:
        //     {
        //         break;
        //     }
        // }
    }
}

public class MobileTrait : TraitThing
{
    public MobileTrait()
    {
        Type = Traits.Mobile;
        AddListen(EventTypes.WalkTo);
        AddListen(EventTypes.Knockback);
        AddListen(EventTypes.StartTurn);
    }
    
    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.StartTurn:
            {
                i.Who.Set(IntStats.MoveLeft,i.Who.Get(IntStats.Movespeed));
                break;
            }
            case EventTypes.WalkTo:
            {
                GameTile t = e.GetTile();
                if (t == null) t = e.GetTile("Target");
                i.Who.Walk(t);
                break;
            }
            case EventTypes.Knockback:
            {
                ActorThing s = e.GetActor("Source");
                GameTile src = s != null ? s.Location : e.GetTile("Target");
                GameTile t = s != null ? e.GetTile("Target") : i.Who.Location;
                if (src == t) t = i.Who.Location;
                Vector2Int dir = God.RoundDir(t.Location-src.Location) * (e.GetInt("",1,s));
                GameTile dest = i.Who.Location.Neighbor(dir);
                if(dest != null) //bug: if pushed against edge should stop at edge, push others they bump into, etc
                    i.Who.Walk(dest,false);
                break;
            }
        }
    }
}

public class AliveTrait : TraitThing
{
    public AliveTrait()
    {
        Type = Traits.Alive;
        AddListen(EventTypes.Setup);
        AddListen(EventTypes.StartTurn);
        AddListen(EventTypes.Damage);
        AddListen(EventTypes.Death);
        AddListen(EventTypes.TrueDeath);
        AddListen(EventTypes.TempDefense);
        AddListen(EventTypes.Heal);
        AddListen(EventTypes.ChangeStat);
        AddListen(EventTypes.CanAct,1);
        AddListen(EventTypes.ProvokeAoO); //No definition, just for watches
        AddListen(EventTypes.LeaveTile);
        AddListen(EventTypes.ArriveTile);
        
    }
    
    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.Setup:
            {
                break;
            }
            case EventTypes.StartTurn:
            {
                i.Who.ActionsLeft.Clear();
                i.Who.ActionsLeft.Add(ActionCost.Major);
                i.Who.ActionsLeft.Add(ActionCost.Bonus);
                i.Who.ActionsLeft.Add(ActionCost.Move);
                i.Who.ActionsLeft.Add(ActionCost.Reaction);
                int arm = i.Who.Get(IntStats.Armor);
                i.Who.Set(IntStats.Defense,arm);
                i.Who.Body.HP.SetArmor(arm,arm);
                // Debug.Log("SET DEF: " + i.Who.Class + " / " + i.Who.Get(IntStats.Defense));
                break;
            }
            case EventTypes.Damage:
            {
                int hp = i.Who.Get(IntStats.HP);
                int startHP = hp;
                int dmg = e.GetInt();
                int vul = i.Who.Get(IntStats.Vulnerable);
                dmg += vul;
                int def = Mathf.Min(dmg,i.Who.Get(IntStats.Defense));
                if (e.GetBool("IgnoreArmor")) def = 0;
                if (def > 0)
                {
                    dmg -= def;
                    int visdef = i.Who.Change(IntStats.Defense, -def);
                    God.GM.AddCut(new HeadtextCut(i.Who, "<" + def + ">", -1,visdef,-1, -1,Colors.Resist));
                }
                if (dmg <= 0 || e.GetBool("ArmorOnly")) return;
                hp -= dmg;
                int injury = Mathf.Max(dmg - startHP, 0);
                dmg -= injury;
                //Take 1/3 of damage taken as injury, too.
                int injrate = i.Who.Get(IntStats.InjuryRate,false,3);
                int fractI = i.Who.Get(IntStats.InjFraction) + dmg;
                while (fractI >= injrate)
                {
                    fractI -= injrate;
                    injury++;
                }
                i.Who.Set(IntStats.InjFraction,fractI);
                i.Who.Set(IntStats.HP,hp);
                injury = Mathf.Min(injury, i.Who.Get(IntStats.MaxHP, true) - i.Who.Get(IntStats.Injury, true));
                i.Who.Change(IntStats.Injury, injury);
                int mhp = i.Who.Get(IntStats.MaxHP);
                God.GM.AddCut(new HeadtextCut(i.Who,"-"+dmg,hp,-1,mhp,injury,Colors.Damage));
                if(hp <= 0)
                    i.Who.TakeEvent(God.E(EventTypes.Death),true);
                break;
            }
            case EventTypes.TempDefense:
            {
                int amt= e.GetInt();
                int n = i.Who.Change(IntStats.Defense, amt);
                God.GM.AddCut(new HeadtextCut(i.Who, "+" + amt, -1,n,-1, -1,Colors.Resist));
                break;
            }
            case EventTypes.Heal:
            {
                int amt= e.GetInt();
                int max = i.Who.GetMaxHP();
                int now = i.Who.Get(IntStats.HP);
                if (amt > max - now) amt = max - now;
                int n = i.Who.Change(IntStats.HP, amt);
                //Debug.Log("HEAL: " + amt + " / " + max + " / " + now + " / " + n);
                God.GM.AddCut(new HeadtextCut(i.Who, "+" + amt, n,-1,-1, -1,Colors.Healing));
                break;
            }
            case EventTypes.Death:
            {
                if (!i.Who.Has(Traits.Player))
                {
                    i.Who.TakeEvent(EventTypes.TrueDeath);
                    return;
                }
                i.Who.Tags.Add(CTags.Corpse);
                God.GM.TakeEvent(e);
                God.GM.AddCut(new DeathCut(i.Who,false));
                break;
            }
            case EventTypes.TrueDeath:
            {
                i.Who.Destruct();
                break;
            }
            case EventTypes.ChangeStat:
            {
                IntStats st = e.GetStat();
                int amt = e.GetInt();
                int dur = e.GetInt("Duration",-1,e.GetActor("Source"));
                string resist = e.GetString("Resist");
                // Debug.Log("CHANGE STAT: " + st + " / " + amt + " / " + dur + " / " + resist);
                if (resist != "" && i.Who.Resist(resist, e.GetActor("Source"))) break;
                string txt = st + (amt >= 0 ? " +" : " ") + amt;
                God.GM.AddCut(new HeadtextCut(i.Who,txt,Colors.StatusEffect));
                i.Who.AddMod(new StatMod(st, amt, dur));
                break;
            }
            case EventTypes.CanAct:
            {
                e.Set(!i.Who.Has(CTags.Corpse));
                break;
            }
            case EventTypes.LeaveTile:
            {
                i.ClearWatches();
                break;
            }
            case EventTypes.ArriveTile:
            {
                i.Who.AddWatch(EventTypes.ProvokeAoO,i,i.Who.Location.Neighbors().ToArray());
                break;
            }
        }
    }

    public override void TakeWatch(TraitInfo i, EventInfo e, ActorThing who, GameTile tile)
    {
        switch (e.Type)
        {
            case EventTypes.ProvokeAoO:
            {
                if (!i.Who.IsEnemy(who)) return;
                if (!i.Who.ActionsLeft.Contains(ActionCost.Reaction)) return;
                ActionScript a = i.Who.GetAct(ActionSlot.Reaction);
                a.QuickExecute(tile,ActionCost.Reaction);
                break;
            }
        }
    }
}