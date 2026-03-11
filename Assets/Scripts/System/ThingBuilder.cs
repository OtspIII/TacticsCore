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
        AddPlayer(CharClass.Fighter,10,3,3,"1d8").Act(Actions.GuardedStrike).Act(Actions.Taunt);//.Act(new ActionPrefab("Snack Break","Melee",ActionSlot.Secondary).Self(God.E(EventTypes.Heal).Roll("2d4+2")));
        AddPlayer(CharClass.Wizard,5,0,3,"1d4").Act(Actions.FireDart).Act(Actions.IcyWind);
        AddPlayer(CharClass.Cleric,8,2,3,"1d6").Act(Actions.KnockbackStrike).Act(Actions.Heal);
        AddPlayer(CharClass.Thief,7,1,4,"1d8").Act(Actions.HitAndRun).Act(Actions.SandInEyes);
        
        //---Level 1---
        //Ratfolk
        AddNPC(CharClass.RatmanCardTosser,1,"Ratfolk Card-Tosser",4,0,4,"1d3")
            .Act(new ActionPrefab("Card Spray","Ranged",ActionSlot.BasicAttack).Attack(1,ActPattern.Cone,1,"1d4"),true)
            .Act(new ActionPrefab("Summon Rat","Item",ActionSlot.Secondary).Tag(ATags.Unsafe,ATags.Quick).EmptyTile(4,God.E(EventTypes.Summon).Set(CharClass.GiantRat)).Tag(ATags.NearEnemy));
        AddNPC(CharClass.RatmanGourmand,1,"Ratfolk Gourmand",8,0,3,"1d6")
            .Act(new ActionPrefab("Long Fork","Melee",ActionSlot.BasicAttack).Attack(1,ActPattern.Pierce,1),true)
            .Act(new ActionPrefab("Snack Break","Melee",ActionSlot.Secondary).Tag(ATags.RequireBloodied,ATags.DontMove).Self(God.E(EventTypes.Heal).Roll("2d4+2")));
        AddNPC(CharClass.RatmanPrayerSqueak, 1, "Ratfolk Prayer-Squeak", 4, 0, 4, "1d3").Tag(CTags.Support)
            .Act(new ActionPrefab("Fever Flame", "Ranged", ActionSlot.BasicAttack).AttackTag(4, ActPattern.Blast, 1,"1",DamageTypes.Poison,"IgnoreArmor")
                .EAdd(God.E(EventTypes.ChangeStat).Set(IntStats.Vulnerable).Set(1)))
            .Act(new ActionPrefab("Plague Prayer", "Ranged", ActionSlot.Secondary).SingleTarget(4,God.E(EventTypes.Heal).Roll("2d4+2"),
                God.E(EventTypes.ChangeStat).Set(IntStats.Vulnerable).Set(1),God.E(EventTypes.ChangeStat).Set(IntStats.Damage).Set(1)).PTarg(TargetType.Character,AITarget.HurtAllies));
        AddNPC(CharClass.RatmanMutant,1,"Ratfolk Mutant",4,0,4,"2d4")
            .Act(new ActionPrefab("Flailing Claws","Melee",ActionSlot.BasicAttack).AttackTag(1,ActPattern.Blast,1,"W",DamageTypes.Normal,"IgnoreSelf").PTarg(TargetType.Self),true)
            .Act(new ActionPrefab("Pounce","Movement",ActionSlot.BeforePlayers,ActionCost.Move).Move(God.N(IntStats.Movespeed)),true);
        //Vermin
        AddNPC(CharClass.GiantRat,1,"Giant Rat",3,0,5,"1d4").Tag(CTags.Beast)
            .Act(new ActionPrefab("Filthy Nibble","Melee",ActionSlot.BasicAttack).Attack(1).EAdd(God.E(EventTypes.GainTrait).Set(Traits.RatBiteFever).Resist("2d6-6")),true);//##Implement fever
        AddNPC(CharClass.GoblinDog,1,"Goblin Dog",4,1,4,"1d4").Tag(CTags.Beast)
            .Act(new ActionPrefab("Heel Gnaw","Melee",ActionSlot.BasicAttack).Attack(1).EAdd(God.E(EventTypes.ChangeStat).Set(IntStats.Movespeed).Set(-2)));
        AddNPC(CharClass.GiantBat,1,"Evil Bat",6,0,5,"1d6").Tag(CTags.Beast)
            .Act(new ActionPrefab("Blood Drink","Heal",ActionSlot.BasicAttack).Attack(1,"1d4"),true)//##Lifedrain
            .Act(new ActionPrefab("Dive Attack","Melee",ActionSlot.Secondary).Attack(1).Move())
            .Act(new ActionPrefab("Guano Explosion","Interact",ActionSlot.OnDeath).AttackTag(1,ActPattern.Blast,1,"1",DamageTypes.Normal,"IgnoreSelf").PTarg(TargetType.Self));//##Spawn Guano Instead
        AddNPC(CharClass.GiantCentipede,1,"Giant Centipede",3,1,4,"1d3").Tag(CTags.Beast)
            .Act(new ActionPrefab("Nauseating Bite","Melee",ActionSlot.BasicAttack).Attack(1).EAdd(God.E(EventTypes.ChangeStat).Set(IntStats.MaxDamage).Set(-1).Resist("1d6+1")),true);//##Maybe last one full extra round?
        //Goblins
        AddNPC(CharClass.GoblinPyro,1,"Goblin Pyromaniac",3,0,3,"1d4")
            .Act(new ActionPrefab("Throw Molotov","Ranged",ActionSlot.BasicAttack).Attack(4,ActPattern.Blast,0,"1d4",DamageTypes.Fire))
            .Act(new ActionPrefab("Self Destruct","Interact",ActionSlot.OnDeath).AttackTag(1,ActPattern.Blast,1,"1d6",DamageTypes.Fire,"IgnoreSelf").PTarg(TargetType.Self));
        AddNPC(CharClass.GoblinTroublemaker,1,"Goblin Troublemaker",5,0,3,"1d6")
            .Act(new ActionPrefab("Shank","Melee",ActionSlot.BasicAttack).Attack(1).ATrait(Traits.PickOff));//##need to implement pick off
        AddNPC(CharClass.GoblinBigmouth,1,"Goblin Bigmouth",4,0,3,"1d4").Tag(CTags.Support)
            .Act(new ActionPrefab("Slap","Melee",ActionSlot.BasicAttack).Attack(1,"1").PAdd(ActEventTarget.Characters,God.E(EventTypes.Knockback).Set(1)).Tag(ATags.LowPriority))
            .Act(new ActionPrefab("Goad On", "Mental", ActionSlot.Secondary).SingleTarget(5,God.E(EventTypes.ChangeStat).Set(IntStats.Damage).Set(2))
                .PTarg(TargetType.Character,AITarget.HurtAllies).Tag(ATags.Fast,ATags.Buff))
            .Act(new ActionPrefab("Hurl Insult", "Mental", ActionSlot.Secondary).SingleTarget(5,God.E(EventTypes.Knockback).Set(-3)).Tag(ATags.Fast));
        AddNPC(CharClass.GoblinBeasttamer,1,"Goblin Ratwhipper",4,1,3,"1d4")
            .Act(new ActionPrefab("Goblin Whip","Melee",ActionSlot.BasicAttack).Attack(2),true)
            .Act(new ActionPrefab("Throw Bait","Ranged",ActionSlot.Secondary).Attack(4,"1",DamageTypes.Fire).EAdd(God.E(EventTypes.TakeAoO)).Tag(ATags.Slow));//##need to implement takeAoO and maybe ai trait
        /*
                  
        
         */

        AddAction(Actions.Walk, "Walk","Movement",ActionCost.None, ActionSlot.BasicMove).Move();
        AddAction(Actions.Sprint, "Sprint","Movement",ActionCost.Major, ActionSlot.Sprint).Move(God.N(IntStats.MoveLeft,1,IntStats.Movespeed)).Tag(ATags.NearEnemy);
        AddAction(Actions.BasicAttack, "Attack","Melee",ActionCost.Major, ActionSlot.BasicAttack).SingleTarget(1,God.E(EventTypes.Damage).Set("Roll","W"));
        //Fighter
        AddAction(Actions.GuardedStrike, "Guarded Strike", "Melee",ActionCost.Major, ActionSlot.BasicAttack).Set(CharClass.Fighter) 
            .Attack(1).PAdd(ActEventTarget.Self,God.E(EventTypes.TempDefense).Set(2));
        AddAction(Actions.Taunt, "Taunt","Mental",ActionCost.Bonus, ActionSlot.Secondary).Set(UsesNum.eConstant).Set(CharClass.Fighter)
            .GTrait(2,Traits.Taunted,"",1); //ActAnims.Yell
        //Wizard
        AddAction(Actions.FireDart, "Fire Dart", "Ranged",ActionCost.Major, ActionSlot.BasicAttack).Set(CharClass.Wizard) 
            .Attack(5,"1d6",DamageTypes.Fire); //Set ground on fire
        AddAction(Actions.IcyWind, "Icy Wind","Ranged",ActionCost.Major, ActionSlot.Secondary).Set(UsesNum.dOften).Set(CharClass.Wizard)
            .Attack(1,ActPattern.Cone,1,"1d5",DamageTypes.Cold).EAdd(God.E(EventTypes.ChangeStat).Set(IntStats.Movespeed).Set(-2).Set("Resist","1d10+5"))
            .PAdd(ActPattern.Cone,1,ActEventTarget.Everything,God.E(EventTypes.Knockback).Set(3));
        //Cleric
        AddAction(Actions.KnockbackStrike, "Knockback Strike", "Melee",ActionCost.Major, ActionSlot.BasicAttack).Set(CharClass.Cleric) 
            .Attack(1).PAdd(ActEventTarget.Characters,God.E(EventTypes.Knockback).Set(1));
        AddAction(Actions.Heal, "Heal","Heal",ActionCost.Bonus, ActionSlot.Secondary).Set(UsesNum.dOften).Set(CharClass.Cleric)
            .SingleTarget(2,God.E(EventTypes.Heal).Roll("1d6+1")); 
        //Thief
        AddAction(Actions.HitAndRun, "Hit & Run", "Melee",ActionCost.Major, ActionSlot.BasicAttack).Set(CharClass.Thief) 
            .Attack(1).Move();
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
    
    public static ClassPrefab AddNPC(CharClass c,int lvl,string name,int hp,int def,int spd,string dmg,float cost=1,bool aggro=true)
    {
        ClassPrefab r = AddClass(c,name, hp, def, spd, dmg);
        if (aggro) r.Team = GameTeam.Enemy;
        r.Cost = cost;
        r.Level = lvl;
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
        ActionPrefab r = new ActionPrefab(t,name,icon,slot,cost,traits);
        ActionDict.Add(t,r);
        return r;
    }

    public static List<CharClass> GetClasses(int level)
    {
        List<CharClass> r = new List<CharClass>();
        foreach (ClassPrefab c in ClassList)
        {
            if (c.Level != level) continue;
            r.Add(c.Class);
        }
        return r;
    }
    
    public static List<CharClass> GetPlayers()
    {
        List<CharClass> r = new List<CharClass>();
        foreach(ClassPrefab c in PlayerList)
            r.Add(c.Class);
        return r;
    }
    
    public static ActionScript MakeAction(Actions a,ActorThing who)
    {
        ActionScript r = new ActionScript();
        ActionPrefab p = ActionDict[a];
        r.Imprint(p,who);
        return r;
    }
    
    public static ActionScript MakeAction(ActionPrefab p,ActorThing who)
    {
        ActionScript r = new ActionScript();
        r.Imprint(p,who);
        return r;
    }
    
}
