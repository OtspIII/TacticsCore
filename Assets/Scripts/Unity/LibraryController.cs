using UnityEngine;

public class LibraryController : MonoBehaviour
{
    public ActorController ActorPrefab;
    public TileController TilePrefab;
    
    void Awake()
    {
        God.Library = this;
    }
}
