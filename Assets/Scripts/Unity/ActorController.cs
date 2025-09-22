using System;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public ActorThing Info;
    public SpriteRenderer Portrait;
    public SpriteRenderer Ring;
    
    public void Setup(ActorThing src)
    {
        Info = src;
        Info.Body = this;
        TileController loc = God.GM.GetTile(Info.Location);
        transform.parent = loc.transform;
        transform.localPosition = new Vector3(0, 0, -10);
        // Debug.Log(Info.Class);
        if (Info.Class != Classes.None)
        {
            Sprite s = God.Library.GetPortrait(Info.Class);
            if (s != null) Portrait.sprite = s;
            // Debug.Log("SPRITE: " + s);
        }
    }

    private void Awake()
    {
        if(!God.GM.AllActors.Contains(this))
            God.GM.AllActors.Add(this);
    }

    private void OnDestroy()
    {
        God.GM.AllActors.Remove(this);
    }
}
