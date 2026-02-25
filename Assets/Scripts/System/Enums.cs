public enum Actors
{
    None=0,
    Character=1
}

public enum Classes
{
    None=0,
    Fighter=1,
    Dog=2,
    FireBeetle=3,
}

public enum Traits
{
    None=0,
    Player=1,
    Health=2,
    Mobile=3,
}

public enum Actions
{
    None               =0000,
    Walk               =0001,
    GenericAttack      =0002,
}

public enum IntStats{
    None=0,
    MaxHP=1,
    Movespeed=2,
    Defense=3,
    Damage=5,
    InjuryRate=6,
	
    Reactions=8,
    Vision=9,//How far can you see?
    HP=10,
    Injury=11,
    GPValue=12,
    Armor=15,
    InjFraction=16,
    Level=18,
    MaxDamage=19,
    Vulnerable=20,
    DamageTaken=21,
}

public enum CTags{
    None=0,
    //---Misc Tags---
    Big=0001,
    BlocksLoS=0002,
    PCOwned=0003,	//The PCs brought it with them--doesn't count towards loot gains
    Treasure=0004,	//Turns into money when brought out of dungeon
    Wall=0005,		//Things can't spawn inside
    Used=0006,		//Has been viewed already
    Searched=0007,	//Has been searched already
    UtilityGear=0008,//Characters start with just one of these
    //---Character Tags---
    Character=1000, //Sort of like Big, but lets allies through
    Corpse=1001, 	//You used to be a character, but aren't right now
    Player=1002,
    Summoned=1003,	//Makes you vanish at the end of the fight
    Webwalker=1004, //You don't treat webs as difficult terrain
    Boss=1005,		//Your team doesn't surrender without you
    Support=1006,   //You aren't a front line fighter
    Beast=1007,		//Can't do most interacts
    RemoteControl=1008,//Shows up in the PC control list
    NonCombatant=1009,//Low priority target by NPCs
    GetKOed=1010,	//Don't die if you hit 0HP
    Transformed=1012,//Shows you've transformed in some way, mostly used for portrait change
    //---CLass Tags---
    Fighter=2001,
    Wizard=2002,
    Cleric=2003,
    Thief=2004,
	
	
    //---Feature Tags---
    Terrain=3001,	//Means it's drawn on the bottom level
    Web=3002,		//Pairs with webwalker--doesn't count as difficult terrain if you have it
    Open=3003,		//Shows that a door/chest/etc is open
    DimLoS=304,		//Increased cost for vision
}

public enum AIPriority{
    None=0,
    LastResort=1,
    RatherNot=2,
    BreadAndButter=3,
    Opportunity=4,
    Mandatory=5
}


public enum ActPattern{
	None=0,
	Blast=1,		//Hits anything within Size range
	Horizontal=2,	//Hits anything to the left or right of target within Size range
	Pierce=3,		//Hits anything behind target within Size range
//	Cross=4,		//Hits anything left,right,behind,front of target within Size range
	Cone=5,			//Hits in a cone, Size deep
	Source=6,		//Only hits the source
	TargetOnly=7,	//Just hits the target
	Wave=8,			//Three wide, size deep
}

public enum DamageTypes{
	None=0,			//Just whatever the creature's default damage type is
	Normal=1,		//Physical damage, like from a stabbing. Triggers TakePoke
	Fire=2,			//Heat damage, like from a fireball. Triggers TakeFire
	Arcane=3,		//Magic damage, like from a magic missile. Triggers TakePoke
	Poison=4,		//Dealt by the Poison status effect
//	Sonic=5,		//Sound/air pressure damage, like from a scream. Triggers TakeWind
//	Emotional=6,	//Saps your will to fight. Can't kill you
	Holy=7,			//Happens when you try to heal an undead
	Cold=8,			//From coldness. Triggers TakeIce
	Evil=9,			//Supernatural magic damage. Causes pure injury
	Psychic=10,		//Weird non-physical damage
	Debuff=11,		//For debuffs
}

public enum ATags{
	None=0,
	ExtraMove=2,
	SafeMove=3,
	Flee=5,			//The target runs away, probably should be moved to trait
	Unsafe=8,		//Provokes AoO
	FromItem=10,	//Grants this action to those who carry it
	FromEquip=11,	//Grants this action to those who equip it
	Consumable=12,	//Destroys the item that grants it
	Fast=13,		//NPCs doing this move go first (for buffs/etc)
	Hidden=14,		//When you Command someone, this option doesn't show up
	Buff=15,		//Only do this to non-supports
	IncapOk=16,		//You can do this even if you're incapacitated (like sleeping)
	RandomTarget=17,//Picks a random target
	FriendlyFireOk=18,//Good if you hit your allies with this
	HitsTeam=19,	//For altars, which hit your whole team with an effect
	Aggressive=20,	//Makes sure you try to move next to an enemy, even if this isn't an attack
	LoseWhenTransform=21,//For lycanthropes, lose these abilities when you transform
	RequireBloodied=22,//You only want to do this move if you've been hurt
	Slow=23,		//Go after all other monsters have acted
//	IgnoreChosenTarg=24,//Don't bother setting a chosen target for this, just use it on the most appropriate at the time
}

public enum ActMove{
	Normal=0,		//You take a normal move
	None=1,			//You can't move
	OneStep=2,		//You can only move one
	Double=3,		//Move at twice your normal speed
	Half=4,			//Move at half your normal speed
}


public enum StrStats
{
	None = 0,
	Lore=1,		//Paired with the Readable trait
	Tags=2,		//Displayed in description
	Description=3,//Shows up in description
}


public enum ATraits
{
	None=0,
	PickOff=1,		//+2 damage if target not standing within 2 squares of any allies
	RerollDmg=2,	//Roll damage twice, pick best
	Universal=3,	//All have this
	TeamDmg=4,		//+
	
}


public enum GEvents
{
	None=0,
	//Prompts
	DoMove=1, 		//This is a command telling a character to move to a new space
	LookAround=2,	//Tells a character to update their list of seen tiles, mostly called when moving
	Interact=3,		//Happens when someone interacts with you
	BeSeen=4,		//Happens when you become visible from not being visible
	TurnStart=5,	//Happens when a new round of combat begins
	ChooseAction=6,	//Picks an action for this combat turn
	ChooseMove=7,	//Picks move tile on the combat action you're taking this turn
	ChooseTarget=8,	//Picks target on the combat action you're taking this turn
	PushBump=9,		//Happens when someone gets moved into a space they shouldn't be
	TakeDamage=10,	//Take damage!
	Die=11,			//Leave the world a little earlier than your companions. Drop below 0hp
	Heal=12,		//Recover damage!
	GetUp=13,		//Stop being dead
	TakeStatus=14,	//Try to have a status effect applied on you
	GainStatus=15,	//When the status actually gets applied
	GainQuest=16,	//Gain a quest!
	LeaveTile=17,
	ArriveAtTile=18,
	TakePush=19,	//Be pushed or pulled towards/away from the source
	ActionEnd=20,	//Called after you take your turn
	ActionStart=21,	//Called after you start your turn
	LoseStatus=22,	//Lose a trait or lose duration of it
	LeaveDungeon=23,//End a delve and go home
	Poke=24,	//Be Poked
	ProvokeAoO=25,	//Do something that provokes an AoO
	TakeFire=26,	//Be Poked
	TakeWind=27,	//Be Poked
	TakeIce=28,		//Be Poked
	TakeCurse=29,	//Be Poked
	Spawn=30,		//Spawn a new actor 
	FullDie=31,		//Take so much injury you're just gone
	Destruct=32,	//Called right before you're destroyed
	Dreameat=33,	//Specialty action: does damage to sleepers
	CombatStart=34, //Fight just started, or you joined it late
	Any=35,			//A listener-only thing. Says to pass along the message no matter what.
	UpdateAppearance=36,//Take your bodies and make them match their states
	TakeTaunt=37,	//The person who taunted you becomes your default attack target
	CombatEnd=38,	//The fight just ended
	WinFight=39,
	TurnTick=40,	//Called once per combat round or explore step.
	DayTick=41,		//Called once per day. Heal injuries and regain abilities and stuff!
	FullHeal=42,	//Brings you to full health
	DelveEnd=43,	//The delve just ended!
	Search=44,
	CureStatus=45,	//Purges negative traits
	GainStat=46,	//Gain a StatTrait
	TakeStat=47,	//Gain a StatTrait if under a HP threshold
	TakeMsg=48,	//Take any EventMsg if under a HP threshold
	TakeAoO=49,//Lets adjacent enemies AoO you
	IgnoreMe=50,	//Makes the target untargetable for a turn
	EndMove=51,		//Sets your remaining move to 0
	SpecialEffect=52,//Says to use the attached special effect to resolve what happens
	HealInjury=53,	//Lose Amt in injuries  
	SetPStat=54,	//Changes a character's stat permanently
	HeatArmor=55,	//Deals damage equal to the target's defense
	BeSpawned=56,	//Called when a thing is spawned
	//Questions
	IsIncapacitated=1001,//Are they able to act?
	GetName=1002,	//What's your name?
	IsScared=1003,	//Are they ready to surrender?
	StatusDesc=1004,//What's my status?
	CalcRecoverTime=1005,//How long will it take me to recover?
	GetDesc=1006,	//What's your description?
	GetTags=1007,	//What's your list of tags?
	IsHidden=1008,	//Can this be seen?
	MoveCost=1009,	//How much does it cost to stand on this?
	GetASummary=1010,//Ask an action to summarize itself for an actor's description
	IsAlive=1011,		//Not fully dead
}

public enum TileTints
{	//The order of these is how high priority they are in determining icon colors
	None=0,
	Inactive=1,
	ActiveThing=3,
	OkayOption=6,
	GoodOption=9,
	Threat=12,
	Harmful=15,
	Custom=18,
	
	
}

public enum Colors
{
	None=0,
	DoAction=1,
	SfX=2,
	Resist=3,
	Death=4,
	Damage=5,
	Healing=6,
	StatusEffect=7,
	Info=8,//8D5DFF
	Dialogue=9,//E57919
	Alert=10,
	
	PCRing=101,
	AllyRing=102,
	EnemyRing=103,
	NPCRing=104,
	
	MainBG=201,
	RightBarBG=202,
	Tab=203,
	MenuYes=204,
	MenuNo=205,
	Card=206,
	ButtonBG=207,
}


public enum ActAnims
{
	None,
	Dagger,
	Interact,
	Magic,
	Yell,
	Heal
}

public enum SoundEffect{
	None=0,
	//Menu stuff
	Confirm=1,
	Back=2,
	Error=3,
	MenuMove=4,
	OpenMenu=5,
	//UI stuff
	Flyby=50,
	DoorOpen=51,
	RumorSpawn=52,
	TutorialCard=53,
	QuestComplete=54,
	TPK=55,
	//Character stuff
	Auto=99,//Pick for me
	TakeStep=100,
	Stab=101,
//	Magic=102,
//	Yell=103,
//	Heal=104,
	Interact=105,
	Die=106,
	ThingHappens=107,
	Dialogue=108,
	Spawn=109,
	Declare=110,
	FullDie=111,
	Alert=112,
	TakeDamage=113,
	TakeArmor=114,
	StatusEffect=115,
}

public enum CursorState
{
	None,
	Default,
	Walk,
	Target,
	Interact,
	Examine,
}


public enum RollMode
{
	None=0,
	Normal=1,
	Min=2,
	Max=3
}

public enum Icons
{
	None=0,
	LeftClick=1,
	RightClick=2,
	Esc=3,
	MWheel=4
}

public enum ZLayers
{
	None=0,
	Terrain=1,
	OnGround=10,
	Character=20,
	Foreground=30
}

public enum TileZones
{
	None=0,
	Open=1,
	ByWall=2,
	InCorner=3,
	BetweenWalls=4,
	Nook=5,
	ByDoor=6,
	All=7,
	Empty=8,
	NotFilled=9,
	//These over 100 don't get calculated automatically
	NoMonster=101,
}

public enum UsesNum
{
	None=0,
	aOnce=1,
	bTwice=2,
	cSometimes=3,
	dOften=5,
	eConstant=8,
}

public enum IColors
{
	None=0,
	Red=1,
	Green=2,
	Blue=3,
	Yellow=4,
	Pink=5,
	Gray=6,
	Brown=7,
	Black=8,
}