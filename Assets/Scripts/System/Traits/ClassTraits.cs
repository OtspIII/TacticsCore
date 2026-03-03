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
                i.Who.Set(IntStats.Defense,i.Who.Get(IntStats.Armor));
                // Debug.Log("SET DEF: " + i.Who.Class + " / " + i.Who.Get(IntStats.Defense));
                break;
            }
            case EventTypes.Damage:
            {
                int hp = i.Who.Get(IntStats.HP);
                int dmg = e.GetInt();
                int vul = i.Who.Get(IntStats.Vulnerable);
                dmg += vul;
                int def = Mathf.Min(dmg,i.Who.Get(IntStats.Defense));
                if (e.GetBool("IgnoreArmor")) def = 0;
                if (def > 0)
                {
                    dmg -= def;
                    int visdef = i.Who.Change(IntStats.Defense, -def);
                    God.GM.AddCut(new HeadtextCut(i.Who, "<" + def + ">", -1, visdef,Colors.Resist));
                }
                if (dmg <= 0 || e.GetBool("ArmorOnly")) return;
                hp -= dmg;
                i.Who.Set(IntStats.HP,hp);
                God.GM.AddCut(new HeadtextCut(i.Who,"-"+dmg,hp,-1,Colors.Damage));
                if(hp <= 0)
                    i.Who.TakeEvent(EventTypes.Death);
                break;
            }
            case EventTypes.Death:
            {
                i.Who.Destruct();
                break;
            }
        }
    }
}