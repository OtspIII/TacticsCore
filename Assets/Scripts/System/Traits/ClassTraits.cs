using System.Collections.Generic;
using UnityEngine;

public class PlayerTrait : TraitThing
{
    public PlayerTrait()
    {
        Type = Traits.Player;
    }
}


public class HealthTrait : TraitThing
{
    public HealthTrait()
    {
        Type = Traits.Health;
    }
}