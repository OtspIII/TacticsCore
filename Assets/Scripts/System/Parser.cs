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
        TraitDict.Add(Traits.Mobile, new MobileTrait());
    }
    
    public static TraitThing Get(Traits t)
    {
        if (TraitDict.TryGetValue(t, out TraitThing r)) return r;
        God.LogError("ERROR MISSING TRAIT: " + t+"\nMust add to TraitManager");
        return null;
    }
    
    public static ActionScript Get(Actions t,ActorThing a)
    {
        switch (t)
        {
            case Actions.Walk: return new WalkAction(a);
        }
        God.LogError("ERROR MISSING ACTION: " + t);
        return null;
    }
}