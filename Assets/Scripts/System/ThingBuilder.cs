using System.Collections.Generic;
using UnityEngine;

public static class ThingBuilder
{
    public static bool IsSetup = false;
    public static List<ActorPrefab> ActorList = new List<ActorPrefab>();
    public static Dictionary<Actors, ActorPrefab> ActorDict = new Dictionary<Actors, ActorPrefab>();
    public static List<ClassPrefab> ClassList = new List<ClassPrefab>();
    public static Dictionary<CharClass, ClassPrefab> ClassDict = new Dictionary<CharClass, ClassPrefab>();
    public static List<ClassPrefab> PlayerList = new List<ClassPrefab>();
    public static Dictionary<Actions,ActionPrefab> ActionDict = new Dictionary<Actions, ActionPrefab>();

    public static void Setup()
    {
        if (IsSetup) return;
        IsSetup = true;
        AddPlayer(CharClass.Fighter,10,3,3,"1d8").Act(Actions.GuardedStrike).Act(Actions.Taunt);
        AddPlayer(CharClass.Wizard,5,0,3,"1d4").Act(Actions.FireDart).Act(Actions.IcyWind);
        AddPlayer(CharClass.Cleric,8,2,3,"1d6").Act(Actions.KnockbackStrike).Act(Actions.Heal);
        AddPlayer(CharClass.Thief,7,1,4,"1d8").Act(Actions.HitAndRun).Act(Actions.SandInEyes);
        AddNPC(CharClass.RatmanCardTosser,"Ratfolk Card Tosser",4,0,4,"1d3");
        AddNPC(CharClass.RatmanGourmand,"Ratfolk Gourmand",8,0,3,"1d6");
        AddNPC(CharClass.RatmanPrayerSqueak,"Ratfolk Prayer-Squeak",4,0,4,"1d3");
        AddNPC(CharClass.RatmanMutant,"Ratfolk Mutant",4,0,4,"2d4");

        AddAction(Actions.Walk, "Walk","Movement",ActionCost.None, ActionSlot.BasicMove).Move();
        AddAction(Actions.BasicAttack, "Attack","Melee",ActionCost.Major, ActionSlot.BasicAttack).SingleTarget(1,God.E(EventTypes.Damage).Set("Roll","W"));
        //Fighter
        AddAction(Actions.GuardedStrike, "Guarded Strike", "Melee",ActionCost.Major, ActionSlot.BasicAttack).Set(CharClass.Fighter) 
            .Attack(1,"W").PAdd(ActEventTarget.Self,God.E(EventTypes.TempDefense).Set(2));
        AddAction(Actions.Taunt, "Taunt","Mental",ActionCost.Bonus, ActionSlot.Secondary).Set(UsesNum.eConstant).Set(CharClass.Fighter)
            .Attack(4,"1"); //ActAnims.Yell
        //Wizard
        AddAction(Actions.FireDart, "Fire Dart", "Ranged",ActionCost.Major, ActionSlot.BasicAttack).Set(CharClass.Wizard) 
            .Attack(5,"1d6",DamageTypes.Fire); //Set ground on fire
        AddAction(Actions.IcyWind, "Icy Wind","Ranged",ActionCost.Major, ActionSlot.Secondary).Set(UsesNum.dOften).Set(CharClass.Wizard)
            .Attack(1,ActPattern.Cone,1,"1d5",DamageTypes.Cold).EAdd(God.E(EventTypes.ChangeStat).Set(IntStats.Movespeed).Set(-2).Set("Resist","1d10+5"))
            .PAdd(ActPattern.Cone,1,ActEventTarget.Everything,God.E(EventTypes.Knockback).Set(3));
        //Cleric
        AddAction(Actions.KnockbackStrike, "Knockback Strike", "Melee",ActionCost.Major, ActionSlot.BasicAttack).Set(CharClass.Cleric) 
            .Attack(1,"W").PAdd(ActEventTarget.Characters,God.E(EventTypes.Knockback).Set(1));
        AddAction(Actions.Heal, "Heal","Heal",ActionCost.Bonus, ActionSlot.Secondary).Set(UsesNum.dOften).Set(CharClass.Cleric)
            .SingleTarget(2,God.E(EventTypes.Heal).Roll("1d6+1")); 
        //Thief
        AddAction(Actions.HitAndRun, "Hit & Run", "Melee",ActionCost.Major, ActionSlot.BasicAttack).Set(CharClass.Thief) 
            .Attack(1,"W").Move();
        AddAction(Actions.SandInEyes, "Sand In The Eyes","Mental",ActionCost.Bonus, ActionSlot.Secondary).Set(UsesNum.eConstant).Set(CharClass.Thief)
            .GTrait(2,Traits.Stunned,"2d6+4",1);
        /*
         
         
         
         */
        
    }

    public static ClassPrefab AddPlayer(CharClass c,int hp,int def,int spd,string dmg)
    {
        ClassPrefab r = AddClass(c, c.ToString(),hp, def, spd, dmg);
        PlayerList.Add(r);
        r.Trait(Traits.Player);
        r.Team = GameTeam.Player;
        return r;
    }
    
    public static ClassPrefab AddNPC(CharClass c,string name,int hp,int def,int spd,string dmg,bool aggro=true)
    {
        ClassPrefab r = AddClass(c,name, hp, def, spd, dmg);
        if (aggro) r.Team = GameTeam.Enemy;
        ClassList.Add(r);
        return r;
    }
    
    public static ClassPrefab AddClass(CharClass c,string name,int hp,int def,int spd,string dmg)
    {
        ClassPrefab r = new ClassPrefab(c);
        r.Name = name;
        ClassDict.Add(c,r);
        r.Trait(Traits.Universal);
        r.Trait(Traits.Alive);
        if (spd > 0)
        {
            r.Trait(Traits.Mobile);
            r.Stats.Add(IntStats.Movespeed,spd);
        }
        r.Stats.Add(IntStats.HP,hp);
        r.Stats.Add(IntStats.Armor,def);
        r.Stats.Add(IntStats.InjuryRate,3);
        r.TxtStats.Add(StrStats.Damage,dmg);
        return r;
    }
    
    public static ActionPrefab AddAction(Actions t,string name,string icon,ActionCost cost,ActionSlot slot,params Traits[] traits)
    {
        ActionPrefab r = new ActionPrefab(t,name,icon,cost,slot,traits);
        ActionDict.Add(t,r);
        return r;
    }

    public static List<CharClass> GetClasses(int level)
    {
        List<CharClass> r = new List<CharClass>();
        foreach(ClassPrefab c in ClassList)
            r.Add(c.Class);
        return r;
    }
    
    public static List<CharClass> GetPlayers()
    {
        List<CharClass> r = new List<CharClass>();
        foreach(ClassPrefab c in PlayerList)
            r.Add(c.Class);
        return r;
    }
    
    public static ActionScript MakeAction(Actions a)
    {
        ActionScript r = new ActionScript();
        ActionPrefab p = ActionDict[a];
        r.Imprint(p);
        return r;
    }
    
}
