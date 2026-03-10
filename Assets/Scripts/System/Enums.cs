public enum Actors
{
    None=0,
    Character=1
}


public enum Traits
{
	None=0,
	//The Basics	#0000
	Universal		=0001,		//Everything has this
	Mobile			=0002,		//Lets you walk
	Hidden			=0003,		//Makes you possibly not visible
	Player			=004,		//Makes you a player
	//Characters	#1000
	Alive			=1001,		//Lets you act in a fight
	Vision			=1002,		//Lets you see
	CanTalk			=1003,		//Lets you do social events
	Rescuable		=1004,		//If you see them they join your group
	TimeLimit		=1005,		//Vanish after X turns
	Gossip			=1006,		//
	//Monsters		#2000
	Undead			=2001,		//Takes damage from healing, heals from necrotic
	Lycanthrope		=2002,		//Transform when you first take damage
	//Items			#3000
	Equipment		=3001,		//Lets you
	Takeable		=3002,		//You can pick it up and take it with you
	GP				=3003,		//Adds to your gold score
	Gem				=3004,		//Randomized gem
	RLoot			=3005,		//Randomized treasure
	//Scenery		#5000
	IsDoor			=5001,		//Blocks movement and LoS until opened
	IsCage			=5002,		//Traps you
	IsExit			=5003,		//Lets you leave the dungeon
	IsTrap			=5004,		//Triggers when you open the thing
	IsContainer		=5005,		//Can hold items and be looted
	FragileFloor	=5006,		//Breaks from stepping on, lots of other stuff
	Pinata			=5007,		//Can hold items, drops them on the foor when destroyed
	Disguise		=5008,		//Is an illusion, hides another thing inside
	Disguised		=5009,		//On the contents of a disguise object
	Lootable		=5010,		//Like a container you can't close
	DifficultTerrain=5011,		//Costs extra to stand on
	Readable		=5012,		//You can read this!
	ActOnBump		=5013,		//Poke it and it spills chems everywhere
	Flammable		=5014,		//Can catch fire from fire damage
	DamageTerrain   =5015,		//Take damage if you move onto or start on
	WinGame			=5016,		//Ends the game when you use it
	Destructable	=5017,		//Can be destroyed with damage
	Throne	        =5018,		//Sit for a twist
	Fountain	    =5019,		//Drink for a permanent change
	Lever	        =5020,		//Pull for a environmental effect
	Altar	        =5021,		//Pray for a party-wide twist
	//Status Effects#7000
	RatBiteFever	=7001,		//Makes you take a while to heal when you get home##
	Asleep			=7002,		//Miss your turns, wake up on damage
	InCage			=7003,		//You've been trapped in a cage!
	Poisoned		=7004,		//Take damage at the start of every turn
	Bound			=7005,		//Stuck in webs/ropes/etc
	Afraid			=7006,		//Run away with your move action
	OnFire			=7007,		//Take damage each turn
	Stunned			=7008,		//Lose your turn
	Charmed			=7009,		//Switch teams
	Baited			=7010,		//Marks someone to be attacked by monsters
	Regenerating	=7011,		//Heal 1HP every turn
	Confused		=7012,		//Pick random targets
	Paralyzed		=7013,		//Ghoul paralysiz
	TrollRegen		=7014,		//Full health regen every turn, hurt by fire
	LashOut			=7015,		//Instant effect, attack a random gsquare
	Taunted			=7016,
	//Buffs			#8000
	GearReward		=8001,
	GearPowerup		=8002,
	StatReward		=8003,
	BoonReward		=8004,
	
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
    MoveLeft=22,
    MoveUsed=23,
    DamageRoll=24,
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
	Quick=4,		//##Goes before players
	Unsafe=8,		//##Provokes AoO
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
	Damage=4,
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

public enum CharClass{
	None=0,
	Narrator=1,
	LichPope=2,
	AncientHero=3,
	Misha=4,
	Auto=5,
//Playable			 1XXX
	Fighter				=1001,
	Wizard				=1002,
	Thief				=1003,
	Cleric				=1004,
//NPCs				 2XXX
	//Mundane		=21XX
	Villager			=2101,
	PhantomServant		=2102,
	//Dungeon		 22XX
	Gnome				=2201,
	//Story			 23XX
	Munchmoss			=2301,
//Beasts			 3XXX
	//Pets				 31XX
	Dog					=3101,
	//Solo Threats	 	32XX
	Bear				=3210,
	//Amphibians & Reptiles
	GiantFrog			=3301,
//Humanoids			 4XXX
	//Goblins		 401X
	GoblinTroublemaker	=4010,
	GoblinBigmouth		=4011,
	GoblinChief			=4012,
	GoblinBeasttamer	=4013,
	GoblinPyro			=4014,
	//Trolls		 410X
	Troll				=4101,
	Ogre				=4102,
	Witch				=4111,
	//Fae			 420X
	Pixie				=4201,
	ElvenFarmer			=4211,
	MushroomMan			=4221,
	SporeCorpse			=4222,
	MiniMushroom		=4223,
	//Ratfolk		 430X
	RatmanCardTosser	=4301,
	RatmanGourmand		=4302,
	RatmanPrayerSqueak	=4303,
	RatmanMutant		=4304,
	//Misc				 49XX
	Doppleganger		=4900,
	Werewolf			=4901,
//Undead&Constructs		 5XXX
	Skeleton			=5010,
	Zombie				=5011,
	Ghoul				=5021,
	Wight				=5030,
	//Constructs
	LivingStatue		=5901,
	StoneAngel			=5902,
	Gargoyle			=5903,
	LivingBoulder		=5904,
//MagicCreatures	 6XXX
	Dragon				=6010,
//Extradimensional	 7XXX
//Vermin			 8XXX
	//Bugs			 81XX
	GiantSpider			=8100,
	DreameaterSpider	=8101,
	GiantCentipede		=8102,
	FireBeetle			=8110,
	BombBeetle			=8111,
	//Mammals		 82XX
	GiantRat			=8200,
	GiantBat			=8210,
	GoblinDog			=8230,
	//Effects
	Test				=9000,
}

public enum Actions{
	//0-Generic,2-Player,3-Monster,4-Items
	//Generic: 01-Basic,02-Forced
	None=0,
	//Basic Actions
	BasicAttack=	0010001,
	Sprint=			0010002,
	RunAway=		0010003,
	Interact=		0010004,
	Walk=			0010005,
//Player: 01-Fighter,02-Wizard,03-Cleric,04-Thief
	//1-MainH,2-OffH,3-Helm,4-Armor,5-Hands,6-Feet,7-Tool
//Any				100XXXX
	//Feet
	SafeStep=		1006001,
	SwapOut=		100602,
	//Tools
	TenFootPoke=	1007001,
	HealingPotion=	1007002,
	Caltrops=		1007003,
	StrengthPotion=	1007004,
//Fighter			101XXXX
	//MainH
	SureStrike=		1011001,
	FlamingStrike=	1011002,
	PressingBlow=	1011003,
	PiercingStrike=	1011004,
	//OffH
	Cleave=			1012001,
	GuardedStrike=	1012102,
	Yank=			1012103,
	//Helm
	Taunt=			1013001,
	Grab=		1013002,
	//Armor
	Shout=			1014001,
	LureToSlaughter=1014002,//
	ComeAtMe=		1014003,
	MightyBlow=	1014004,
	//Hands
	YellCommand=	1015001,
	SecondWind=		1015002,
	Block=			1015003,
	//Feet
	Leap=   		1016001,
	HoldYourGround= 1016002,
	LastStand=		1016003,
//Wizard			102XXXX
	//MainH
	FireDart=		1021001,
	ArcaneBlast=	1021002,
	Freeze=			1021003,
	ForceBolt=		1021004,
	//OffH
	IcyWind=		1022001,
	Confuse=		1022002,
	Distract=		1022003,
	//Helm
	Charm=			1023001,
	Death=			1023002,
	//Armor
	Fireball=		1024001,
	Sleep=			1024002,
	SummonRats=		1024003,
	//Hands
	MageHand=		1025001,
	PhantomServant=	1025002,
	HeatArmor=		1025003,
	//Feet
	Teleport=		1026001,
	Blink=			1026002,
	ExplosiveTeleport=1026003,
//Cleric			103XXXX
	//MainH
	KnockbackStrike=1031001,
	ExposeOpening=	1031002,
	RallyingStrike=	1031003,
	StunningStrike=	1031004,
	//OffH
	Heal=			1032001,
	Shield=			1032002,
	Strengthen=		1032003,
	//Helm
	Fear=			1033001,
	Weaken=			1033002,
	RendArmor=		1033003,
	//Armor
	Bless=			1034001,
	BlessingOfSpeed=1034002,
	StunningWord=	1034003,
	WaveOfHealth=	1034004,
	//Hands
	SummonWall=		1035001,
	CureStatus=		1035002,
	Regen=			1035003,
	//Feet
	WellspringOfHealth=1036001,
	ShockwaveStomp=	1036002,
	
//Thief				104XXXX
	//MainH
	HitAndRun=		1041001,
	SwappingBlow=	1041002,
	StabOfOpportunity=1041003,
	PoisonedStrike=		1041004,
	//OffH
	FireFlask=		1042001,
	ShootArrow=		1042002,
	Shove=			1042003,
	//Helm
	SandInEyes=		1043001,
	EvadeNotice=	1043002,
	Stumble=		1043003,
	//Armor
	PoisonBomb=		1044001,
	Backstab=		1044003,
	BypassArmor=	1044004,
	//Hands
	QuickInteract=	1045001,
	ThrowKnife=		1045002,
	RagePowder=		1045003,
	//Feet
	Tumble=			1046001,
	QuickStep=		1046002,
	Swap=			1046003,

//Misc
	ThrowRock=		3000001,
	Test=			3000002,
//	ThrowGoopyFruit=3070005,
}

public enum ActorP
{
	None=0,
	Gear=1,
	Auto=2,
	F1Terrain=     1001,
	F2Terrain=     1002,
	F3Terrain=     1003,
	F4Terrain=     1004,
	F5Terrain=     1005,
	//			  #2XXX Terrain
	Water=		   2001,//Needs transforms
	Ice=		   2002,//Needs transforms
	Snow=		   2003,//
	Poison=        2004,
	HealingGoo=    2005,//
	Grass=		   2101,//
	Flowers=	   2101,//
	Mushrooms=	   2101,//
	Hellshrooms=   2101,//
	Brambles=	   2101,//
	Flames=        2201,//
	Lava=		   2202,//
	Guano=         2203,
	Mirror=        2206,//
	Sand=		   2207,//
	Hellfire=	   2208,//
	Web=           2301,
	WebClump=      2302,
	ThickWeb=      2303,
	BonePile=      2304,
	Caltrops=      2401,
	Trash=         2501,
	Graffiti=      2502,
	Metal=         2503,//
	Circuits=      2504,//
	

	//			  #3XXX Gasses
	Smoke=		   3001,
	Darkness=	   3002,
	Steam=	       3003,//
	PoisonGas=	   3004,//
	Dust=	       3005,//
	SnowFlurry=	   3006,//
	Mist=	       3007,//
	HealingMist=   3008,//
	ChaosGas=	   3009,//

	//            #4000 Items
//	PlateMail=    4004,
//	LeatherArmor= 4005,
	GoopyFruit=   4015,
	//            #5000 Furniture
	StoneDoor=     5001,
	WoodDoor=      5002,
	Tree=          5005,
	Statue=        5006,
	Table=         5007,
	Barricade=     5008,
	Cage=          5009,
	ExitDoor=      5010,
	Idol=          5011,
	Wall=          5012,
	Barrel=        5014,
	Sack=          5015,
	Sarcophagus=   5017,
	Throne=        5018,
	Chest=         5019,
	Book=          5023,
	Vat=           5024,
	Shelves=       5025,
	StairsDown=    5028,
	Treasure=      5029,
	GoldCoins=     5030,
	BronzeDoor=    5031,
	PhantomWall=   5032,
	Fountain=	   5033,
	Altar=	       5034,
	Lever=	       5036,
	Gem=		   5037,
	RLoot=		   5038,
	//            #6000 Traps
	RockFallTrap  =6001,
	PoisonNeedleTrap  =6002,
	//			  #7XXX Loot
	FireBeetleAss	=7001,
	//			  	#8XXX Rewards
	GearRewardF=	8001,
	GearRewardW=	8002,
	GearRewardC=	8003,
	GearRewardT=	8004,
	GearPowerupF=	8011,
	GearPowerupW=	8012,
	GearPowerupC=	8013,
	GearPowerupT=	8014,
	BoonReward=		8030,
	StatRewardF=	8041,
	StatRewardW=	8042,
	StatRewardC=	8043,
	StatRewardT=	8044,
	
	//			  #9XXX CustomEffects
	NoMonster			=9001,//Monsters shouldn't spawn here
	
}

public enum AImprovements
{
	None=0,
				  //0XX - Stat boosts
	Damage=			001,
	MaxDamage=		002,
				  //1XX - Numerical boosts
	Range=			101,
	PatternSize=	102,
	Uses=			103,
	
				  //2XX - Core Changes
	PatternShape=	201,
	AddTag=			202,
	RemoveTag=		203,
	DType=			204,
}