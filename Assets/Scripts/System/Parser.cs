using System.Collections.Generic;

public static class Parser
{
    public static bool Setup = false;
    public static Dictionary<Traits, TraitThing> TraitDict = new Dictionary<Traits, TraitThing>();

    public static void Init()
    {
        if (Setup) return;
        Setup = true;
        TraitDict.Add(Traits.Health, new HealthTrait());
        TraitDict.Add(Traits.Player, new PlayerTrait());
    }
    
    public static TraitThing Get(Traits t)
    {
        if (TraitDict.TryGetValue(t, out TraitThing r)) return r;
        God.LogError("ERROR MISSING TRAIT: " + t+"\nMust add to TraitManager");
        return null;
    }
}