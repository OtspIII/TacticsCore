using System.Collections.Generic;
using UnityEngine;

public class PlayerTrait : TraitThing
{
    public PlayerTrait()
    {
        Type = Traits.Player;
        AddListen(EventTypes.StartTurn);
    }

    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.StartTurn:
            {
                i.Who.ActionsLeft.Clear();
                i.Who.ActionsLeft.Add(ActionCost.Major);
                i.Who.ActionsLeft.Add(ActionCost.Bonus);
                i.Who.ActionsLeft.Add(ActionCost.Move);
                break;
            }
        }
    }
}

public class MobileTrait : TraitThing
{
    public MobileTrait()
    {
        Type = Traits.Mobile;
        AddListen(EventTypes.WalkTo);
    }
    
    public override void TakeEvent(TraitInfo i, EventInfo e)
    {
        switch (e.Type)
        {
            case EventTypes.WalkTo:
            {
                GameTile t = e.GetTile();
                if (t == null) t = e.GetTile("Target");
                i.Who.Walk(t);
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
                    i.Who.TakeEvent(EventTypes.Death);
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
        }
    }
}