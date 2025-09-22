using UnityEngine;

public class TileController : MonoBehaviour
{
    public TileThing Info;
    
    public void Setup(TileThing src)
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
}
