using System;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public SpriteRenderer SR;
    public GameTile Info;
    
    public void Setup(GameTile src)
    {
        Info = src;
        Info.Body = this;
        transform.position = new Vector3(src.X, src.Y, 10);
    }
    
    private void Awake()
    {
        if(!God.GM.AllTiles.Contains(this))
            God.GM.AllTiles.Add(this);
    }

    private void OnDestroy()
    {
        God.GM.AllTiles.Remove(this);
    }

    public Vector3 GetContentPos(ActorThing a)
    {
        Vector3 r = transform.position;
        r.z = 0;
        return r;
    }

    void OnMouseDown()
    {
        God.GM.TileClick(Info);
    }

    void OnMouseEnter()
    {
        God.GM.TileMouseEnter(Info);
    }

    void OnMouseExit()
    {
        God.GM.TileMouseExit(Info);
    }

    public void Audit()
    {
        Info.WipeTint(); //IDK, prob too simple
    }
}
