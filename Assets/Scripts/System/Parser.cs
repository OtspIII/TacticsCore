using System.Collections.Generic;

public static class Parser
{
    public static bool Setup = false;
    public static Dictionary<Traits, TraitThing> TraitDict = new Dictionary<Traits, TraitThing>();

    public static void Init()
    {
        if (Setup) return;
        Setup = true;
        TraitDict.Add(Traits.Alive, new AliveTrait());
        TraitDict.Add(Traits.Player, new PlayerTrait());
        TraitDict.Add(Traits.Universal, new UniversalTrait());
        TraitDict.Add(Traits.Mobile, new MobileTrait());
        TraitDict.Add(Traits.Stunned, new StunTrait());
        TraitDict.Add(Traits.Taunted, new TauntedTrait());
        TraitDict.Add(Traits.RatBiteFever, new RatBiteFeverTrait());
    }
    
    public static TraitThing Get(Traits t)
    {
        if (TraitDict.TryGetValue(t, out TraitThing r)) return r;
        God.LogError("ERROR MISSING TRAIT: " + t+"\nMust add to TraitManager");
        return null;
    }
    
}