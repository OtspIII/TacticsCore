using System.Collections.Generic;
using UnityEngine;

public class PickOffTrait : TraitThing
{
    public PickOffTrait()
    {
        Type = Traits.PickOff;
    }

    public override void TakeEvent(ActionScript a, EventInfo e)
    {
        switch (e.Type)
        {
            
        }
    }
}